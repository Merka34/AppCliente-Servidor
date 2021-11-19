using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using QuizServer.Models;
using QuizServer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizServer.ViewModels
{
    public class ServidorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        HttpListener listener = new HttpListener();
        Random random = new Random();
        Dispatcher dispatcher;

        private ObservableCollection<Respuesta> _respuestas { get; set; } = new ObservableCollection<Respuesta>();
        public ObservableCollection<Respuesta> Respuestas
        {
            get { return _respuestas; }
            set { _respuestas = value; }
        }

        private List<Respuesta> _misRespuestas { get; set; } = new List<Respuesta>();
        public List<Respuesta> MisRespuestas
        {
            get { return _misRespuestas; }
            set { _misRespuestas = value; }
        }

        private List<Usuario> _respuestasFinal { get; set; } = new List<Usuario>();
        public List<Usuario> RespuestasFinal
        {
            get { return _respuestasFinal; }
            set { _respuestasFinal = value; }
        }

        private ObservableCollection<Usuario> _username { get; set; } = new ObservableCollection<Usuario>();
        public ObservableCollection<Usuario> Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private Stopwatch _cronometro { get; set; } = new Stopwatch();
        public Stopwatch Cronometro
        {
            get { return _cronometro; }
            set
            {
                _cronometro = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cronometro)));
            }
        }

        private string _tiempo;
        public string Tiempo
        {
            get { return _tiempo; }
            set
            {
                _tiempo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tiempo)));
            }
        }

        private int _respuesta;
        public int Respuesta
        {
            get { return _respuesta; }
            set
            {
                _respuesta = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Respuesta)));
            }
        }

        private string _expresion;
        public string Expresion
        {
            get { return _expresion; }
            set
            {
                _expresion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Expresion)));
            }
        }

        private string _incCorrecto;
        public string IncCorrecto
        {
            get { return _incCorrecto; }
            set { _incCorrecto = value; }
        }

        private int _incA;
        public int IncA
        {
            get { return _incA; }
            set
            {
                _incA = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncA)));
            }
        }

        private int _incB;
        public int IncB
        {
            get { return _incB; }
            set
            {
                _incB = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncB)));
            }
        }

        private int _incC;
        public int IncC
        {
            get { return _incC; }
            set
            {
                _incC = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncC)));
            }
        }

        private int _incD;
        public int IncD
        {
            get { return _incD; }
            set
            {
                _incD = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncD)));
            }
        }

        private bool _partidaIniciada;

        public bool PartidaIniciada
        {
            get { return _partidaIniciada; }
            set { _partidaIniciada = value; }
        }


        private Ventana _view;
        public Ventana Vista
        {
            get => _view;
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vista)));
            }
        }

        public ICommand IniciarRondaCommand { get; set; }
        public ICommand FinalizarPartidaCommand { get; set; }
        public ICommand VerInicioCommand { get; set; }


        public ServidorViewModel()
        {
            IniciarRondaCommand = new RelayCommand(IniciarRonda);
            FinalizarPartidaCommand = new RelayCommand(FinalizarPartida);
            VerInicioCommand = new RelayCommand(VerInicio);
            dispatcher = Dispatcher.CurrentDispatcher;
            Vista = Ventana.Inicio;
            IniciarServidor();
        }

        public void IniciarServidor()
        {
            if (!listener.IsListening)
            {
                listener.Prefixes.Add("http://*:8050/");
                listener.Start();
                Task.Run(EscucharPeticiones);
            }
        }

        public void EscucharPeticiones()
        {
            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                lock (context)
                {
                    if (context.Request.Url.AbsolutePath == "/Usuario" && context.Request.HttpMethod == "POST") //Registro
                    {
                        string c = BufferString(context);
                        if (Username.Any(x => x.Nombre == c)) //Se encuentra Registrado
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes($"El nombre de usuario {context.Request.QueryString["username"]} ya ha sido registrado en el servidor");
                            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        }
                        else // No esta registrado
                        {
                            dispatcher.Invoke(() => Username.Add(new Usuario { Nombre = c, Puntos = 0 }));
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                        }
                    }
                    else if (context.Request.Url.AbsolutePath == "/Respuesta" && context.Request.HttpMethod == "POST")
                    {
                        if (PartidaIniciada)
                        {

                            string c = BufferString(context);
                            Registro res = JsonConvert.DeserializeObject<Registro>(c);
                            if (Respuestas.Any(x => x.Username == res.Username))
                            {
                                byte[] buffer = Encoding.UTF8.GetBytes("Ya has respondido");
                                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                                context.Response.StatusCode = 409;
                            }
                            else
                            {
                                dispatcher.Invoke(() => Respuestas.Add(new Respuesta { Username = res.Username, IdRespuesta = res.Inciso, Tiempo = Cronometro.ElapsedTicks }));
                                byte[] buffer = Encoding.UTF8.GetBytes(Cronometro.Elapsed.TotalSeconds.ToString("0.00"));
                                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                            }

                        }
                        else
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes("No se recibe respuestas");
                            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    context.Response.Close();
                }
            }
        }

        public async void IniciarRonda()
        {
            GenerarExpresion();
            GenerarIncisos();
            PartidaIniciada = true;
            Vista = Ventana.Partida;
            Cronometro.Start();
            while (Cronometro.Elapsed.TotalSeconds < 30)
            {
                await Task.Delay(10);
                Tiempo = Cronometro.Elapsed.TotalSeconds.ToString("0.00");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cronometro)));
            }
            Cronometro.Reset();
            PartidaIniciada = false;
            VerResultados();
        }

        public void VerResultados()
        {
            Revision();
            MisRespuestas = Respuestas.OrderByDescending(x => x.Correcto).ThenBy(x => x.Tiempo).ToList();
            Respuestas = new ObservableCollection<Respuesta>();
            Vista = Ventana.Resultado;
        }

        public void Revision()
        {
            int puntuacion = 10;
            foreach (var res in Respuestas)
            {
                if (res.IdRespuesta == IncCorrecto)
                {
                    res.Correcto = true;
                    for (int i = 0; i < Username.Count; i++)
                    {
                        if (Username[i].Nombre == res.Username)
                        {
                            Username[i].Puntos += puntuacion;
                        }
                    }
                    if (puntuacion > 1)
                    {
                        puntuacion--;
                    }

                }
                TimeSpan timeSpan = TimeSpan.FromTicks((long)res.Tiempo);
                res.Tiempo = timeSpan.TotalSeconds;
            }
        }

        public string BufferString(HttpListenerContext context)
        {
            int bufferLength = (int)context.Request.ContentLength64;
            byte[] bufferTemp = new byte[bufferLength];
            context.Request.InputStream.Read(bufferTemp, 0, bufferLength);
            return Encoding.UTF8.GetString(bufferTemp);
        }

        public void GenerarExpresion()
        {
            Expresion = "";
            int resultadoTemporal = random.Next(10, 31);
            int posiciones = random.Next(3, 5); //Selecciona la cantidad de variables
            int posicionRespues = random.Next(0, posiciones - 1); //Selecciona la posicion del valor a adivinar
            //int posicionRespues = posiciones - 1;
            string expresionTemp = "";
            if (posicionRespues == posiciones - 1) //Si el valor a adivinar esta posicionado como ultimo seria 3+2=X
            {

                Respuesta = resultadoTemporal;
                for (int i = 0; i < posiciones - 1; i++)
                {
                    int op = random.Next(0, 2);
                    int x = random.Next(1, resultadoTemporal);
                    if (op == 0) // Suma
                    {
                        if (i == posiciones - 2)
                            Expresion = $"{resultadoTemporal}{expresionTemp}=X";
                        else
                        {
                            resultadoTemporal -= x;
                            expresionTemp += "+" + x;
                        }
                    }
                    else //Resta
                    {
                        if (i == posiciones - 2)
                            Expresion = $"{resultadoTemporal}{expresionTemp}=X";
                        else
                        {
                            resultadoTemporal += x;
                            expresionTemp += "-" + x;
                        }
                    }
                }
            }
            else // 7-X=6
            {
                int resultado = resultadoTemporal;
                for (int i = 0; i < posiciones - 1; i++)
                {
                    int op = random.Next(0, 2);
                    int x = random.Next(1, resultadoTemporal);

                    if (op == 0)
                    {
                        if (i == posicionRespues)
                        {
                            resultadoTemporal -= x;
                            expresionTemp += "+X";
                            Respuesta = x;/*
                            if (i == posiciones - 2)
                                expresionTemp += "+X" + resultadoTemporal;
                            else
                                expresionTemp += "+X";*/
                        }/*
                        else if (i == posiciones - 2)
                        {
                            resultadoTemporal -= x;
                            expresionTemp += x;
                        }*/
                        else
                        {
                            resultadoTemporal -= x;
                            expresionTemp += "+" + x;
                        }
                    }
                    else
                    {
                        if (i == posicionRespues)
                        {
                            resultadoTemporal += x;
                            Respuesta = x;
                            expresionTemp += "-X";
                            /*
                            if (i == posiciones - 2)
                                expresionTemp += "-X" + resultadoTemporal;
                            else
                                expresionTemp += "-X";*/
                        }/*
                        else if (i == posiciones - 2)
                        {
                            resultadoTemporal += x;
                            expresionTemp += x;
                        }*/
                        else
                        {
                            resultadoTemporal += x;
                            expresionTemp += "-" + x; //-
                        }
                    }
                }
                Expresion = $"{resultadoTemporal}{expresionTemp}={resultado}";
            }
        }
        public void GenerarIncisos()
        {
            int incisoCorrecto = random.Next(0, 4);
            switch (incisoCorrecto)
            {
                case 0:
                    IncA = Respuesta;
                    IncCorrecto = "A";
                    if (Respuesta < 11)
                    {
                        do
                        {
                            IncB = random.Next(0, 20);
                            IncC = random.Next(0, 20);
                            IncD = random.Next(0, 20);
                        }
                        while (IncB == IncC || IncB == IncD || IncC == IncD || IncB == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    else if (Respuesta > 89)
                    {
                        do
                        {
                            IncB = random.Next(80, 100);
                            IncC = random.Next(80, 100);
                            IncD = random.Next(80, 100);
                        }
                        while (IncB == IncC || IncB == IncD || IncC == IncD || IncB == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    else
                    {
                        do
                        {
                            IncB = random.Next(Respuesta - 10, Respuesta + 11);
                            IncC = random.Next(Respuesta - 10, Respuesta + 11);
                            IncD = random.Next(Respuesta - 10, Respuesta + 11);
                        }
                        while (IncB == IncC || IncB == IncD || IncC == IncD || IncB == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    break;
                case 1:
                    IncB = Respuesta;
                    IncCorrecto = "B";
                    if (Respuesta < 11)
                    {
                        do
                        {
                            IncA = random.Next(0, 20);
                            IncC = random.Next(0, 20);
                            IncD = random.Next(0, 20);
                        }
                        while (IncA == IncC || IncA == IncD || IncC == IncD || IncA == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    else if (Respuesta > 89)
                    {
                        do
                        {
                            IncA = random.Next(80, 100);
                            IncC = random.Next(80, 100);
                            IncD = random.Next(80, 100);
                        }
                        while (IncA == IncC || IncA == IncD || IncC == IncD || IncA == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    else
                    {
                        do
                        {
                            IncA = random.Next(Respuesta - 10, Respuesta + 11);
                            IncC = random.Next(Respuesta - 10, Respuesta + 11);
                            IncD = random.Next(Respuesta - 10, Respuesta + 11);
                        }
                        while (IncA == IncC || IncA == IncD || IncC == IncD || IncA == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    break;
                case 2:
                    IncC = Respuesta;
                    IncCorrecto = "C";
                    if (Respuesta < 11)
                    {
                        do
                        {
                            IncA = random.Next(0, 20);
                            IncB = random.Next(0, 20);
                            IncD = random.Next(0, 20);
                        }
                        while (IncA == IncB || IncA == IncD || IncB == IncD || IncA == Respuesta || IncB == Respuesta || IncD == Respuesta);
                    }
                    else if (Respuesta > 89)
                    {
                        do
                        {
                            IncA = random.Next(80, 100);
                            IncB = random.Next(80, 100);
                            IncD = random.Next(80, 100);
                        }
                        while (IncA == IncB || IncA == IncD || IncB == IncD || IncA == Respuesta || IncB == Respuesta || IncD == Respuesta);
                    }
                    else
                    {
                        do
                        {
                            IncA = random.Next(Respuesta - 10, Respuesta + 11);
                            IncB = random.Next(Respuesta - 10, Respuesta + 11);
                            IncD = random.Next(Respuesta - 10, Respuesta + 11);
                        }
                        while (IncA == IncB || IncA == IncD || IncB == IncD || IncA == Respuesta || IncB == Respuesta || IncD == Respuesta);
                    }
                    break;
                case 3:
                    IncD = Respuesta;
                    IncCorrecto = "D";
                    if (Respuesta < 11)
                    {
                        do
                        {
                            IncB = random.Next(0, 20);
                            IncC = random.Next(0, 20);
                            IncA = random.Next(0, 20);
                        }
                        while (IncB == IncC || IncB == IncA || IncC == IncA || IncB == Respuesta || IncC == Respuesta || IncA == Respuesta);
                    }
                    else if (Respuesta > 89)
                    {
                        do
                        {
                            IncB = random.Next(80, 100);
                            IncC = random.Next(80, 100);
                            IncA = random.Next(80, 100);
                        }
                        while (IncB == IncC || IncB == IncA || IncC == IncA || IncB == Respuesta || IncC == Respuesta || IncA == Respuesta);
                    }
                    else
                    {
                        do
                        {
                            IncB = random.Next(Respuesta - 10, Respuesta + 11);
                            IncC = random.Next(Respuesta - 10, Respuesta + 11);
                            IncA = random.Next(Respuesta - 10, Respuesta + 11);
                        }
                        while (IncB == IncC || IncB == IncA || IncC == IncA || IncB == Respuesta || IncC == Respuesta || IncA == Respuesta);
                    }
                    while (IncA == IncB || IncA == IncC || IncB == IncC) ;
                    break;
                default:
                    break;
            }
        }

        public void FinalizarPartida()
        {
            RespuestasFinal = Username.OrderByDescending(x => x.Puntos).ToList();
            Vista = Ventana.Cierre;
        }

        public void VerInicio()
        {
            foreach (var item in Username)
            {
                item.Puntos = 0;
            }
            RespuestasFinal = new List<Usuario>();
            Vista = Ventana.Inicio;
        }
    }
}
