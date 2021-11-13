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

        private ObservableCollection<string> _username { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Username
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



        public ServidorViewModel()
        {
            Username.Add("Oscar");
            Username.Add("Manolito34");
            Username.Add("Jorge Moreno");
            Username.Add("Mauricio");
            Username.Add("Luiz Perez");
            IniciarRondaCommand = new RelayCommand(IniciarRonda);
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


                /*
                int bufferLength = (int)context.Request.ContentLength64;
                byte[] buffer = new byte[bufferLength];
                context.Request.InputStream.Read(buffer, 0, bufferLength);
                string c = Encoding.UTF8.GetString(buffer);
                IEnumerable<Sombrero> final = JsonConvert.DeserializeObject<IEnumerable<Sombrero>>(c);
                int y = 2;*/
            }
        }

        public void IniciarRonda()
        {
            GenerarExpresion();
            GenerarIncisos();
            Vista = Ventana.Partida;
            Cronometro.Start();

        }

        public void GenerarExpresion()
        {
            Expresion = "";
            int resultadoTemporal = random.Next(10, 101);
            int posiciones = random.Next(3, 6); //Selecciona la cantidad de variables
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
                        }
                        else if (i == posiciones - 2)
                        {
                            resultadoTemporal -= x;
                            expresionTemp += x;
                        }
                        else
                        {
                            resultadoTemporal -= x;
                            expresionTemp += "+"+x;
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
                            expresionTemp += "-"+x; //-
                        }
                    }
                }
                Expresion = resultadoTemporal + expresionTemp + "=" + resultado;
            }
        }
        public void GenerarIncisos()
        {
            int incisoCorrecto = random.Next(0, 4);
            switch (incisoCorrecto)
            {
                case 0:
                    IncA = Respuesta;
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
                            IncB = random.Next(Respuesta-10, Respuesta+11);
                            IncC = random.Next(Respuesta-10, Respuesta+11);
                            IncD = random.Next(Respuesta-10, Respuesta+11);
                        }
                        while (IncB == IncC || IncB == IncD || IncC == IncD || IncB == Respuesta || IncC == Respuesta || IncD == Respuesta);
                    }
                    break;
                case 1:
                    IncB = Respuesta;
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
                    while (IncA == IncB || IncA == IncC || IncB == IncC);
                    break;
                default:
                    break;
            }
        }

    }
}
