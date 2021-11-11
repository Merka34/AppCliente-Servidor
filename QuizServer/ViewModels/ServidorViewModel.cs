using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using QuizServer.Models;
using QuizServer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizServer.ViewModels
{
    public class ServidorViewModel
    {
        List<string> username = new List<string>();
        HttpListener listener = new HttpListener();
        Dispatcher dispatcher;

        private Ventana _view;
        public Ventana Vista
        {
            get => _view;
            set { _view = value; }
        }


        public ServidorViewModel()
        {
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
                int bufferLength = (int)context.Request.ContentLength64;
                byte[] buffer = new byte[bufferLength];
                context.Request.InputStream.Read(buffer, 0, bufferLength);
                string c = Encoding.UTF8.GetString(buffer);
                IEnumerable<Sombrero> final = JsonConvert.DeserializeObject<IEnumerable<Sombrero>>(c);
                int y = 2;
            }
        }
    }

    public class Sombrero
    {
        public string Marca { get; set; }
        public int Valor { get; set; }
    }
}
