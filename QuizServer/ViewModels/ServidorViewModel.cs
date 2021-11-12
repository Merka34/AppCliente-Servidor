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
        private ObservableCollection<string> _username { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Username
        {
            get { return _username; }
            set { _username = value; }
        }

        HttpListener listener = new HttpListener();
        Random random = new Random();
        Dispatcher dispatcher;
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

        public event PropertyChangedEventHandler PropertyChanged;

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
            //int posicionRespues = random.Next(0, posiciones - 1); //Selecciona la posicion del valor a adivinar
            int posicionRespues = posiciones - 1;
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
            }/*
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
                            Respuesta = x;
                            if (i == posiciones - 2)
                                expresionTemp += "X+" + resultadoTemporal;
                            else
                                expresionTemp += "X+";
                        }
                        else if (i == posiciones - 2)
                        {
                            resultadoTemporal -= x;
                            expresionTemp += x;
                        }
                        else
                        {
                            resultadoTemporal -= x;
                            expresionTemp += x + "+";
                        }
                    }
                    else
                    {
                        if (i == posicionRespues)
                        {
                            resultadoTemporal += x;
                            Respuesta = x;
                            if (i == posiciones - 2)
                                expresionTemp += "X-" + resultadoTemporal;
                            else
                                expresionTemp += "X-";
                        }
                        else if (i == posiciones - 2)
                        {
                            resultadoTemporal += x;
                            expresionTemp += x;
                        }
                        else
                        {
                            resultadoTemporal += x;
                            expresionTemp += x + "-";
                        }
                    }
                }
                Expresion = expresionTemp + "=" + resultado;
            }*/
        }
        public void GenerarIncisos()
        {
            int incisoCorrecto = random.Next(0, 4);
            switch (incisoCorrecto)
            {
                case 0:
                    IncA = Respuesta;
                    do
                    {
                        IncB = random.Next(0, 100);
                        IncC = random.Next(0, 100);
                        IncD = random.Next(0, 100);
                    }
                    while (IncB == IncC || IncB==IncD || IncC==IncD);
                    break;
                case 1:
                    do
                    {
                        IncA = random.Next(0, 100);
                        IncB = Respuesta;
                        IncC = random.Next(0, 100);
                        IncD = random.Next(0, 100);
                    }
                    while (IncA == IncC|| IncA == IncD || IncC == IncD);
                    break;
                case 2:
                    do
                    {
                        IncA = random.Next(0, 100);
                        IncB = random.Next(0, 100);
                        IncC = Respuesta;
                        IncD = random.Next(0, 100);
                    }
                    while (IncA == IncB || IncA == IncD || IncB == IncD);
                    break;
                case 3:
                    do
                    {
                        IncA = random.Next(0, 100);
                        IncB = random.Next(0, 100);
                        IncC = random.Next(0, 100);
                        IncD = Respuesta;
                    }
                    while (IncA == IncB || IncA == IncC || IncB == IncC);
                    break;
                default:
                    break;
            }
        }
    }
}
