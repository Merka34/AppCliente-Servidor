using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfPaint4.Models;
using WpfPaint4.Views;

namespace WpfPaint4.ViewModels
{
    public class ServidorViewModel : INotifyPropertyChanged
    {
        public bool Servidor { get; set; } = true;

        Dispatcher dispatcher;

        public int Color { get; set; }
        private int size;
        public int Size 
        {
            get { return size; }
            set { size = value; OnPropertyChanged(); }
        }
            

        private ObservableCollection<List<Circulo>> circulos;
        public ObservableCollection<List<Circulo>> Circulos
        {
            get { return circulos; }
            set { circulos = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Circulo> trazos = new ObservableCollection<Circulo>();

        public ObservableCollection<Circulo> Trazos
        {
            get { return trazos; }
            set { trazos = value; OnPropertyChanged(); }
        }

        public ICommand DesconectarCommand { get; set; }

        RolViewModel rolViewM;
        public ServidorViewModel(RolViewModel rolVM)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            DesconectarCommand = new RelayCommand(Desconectar);
            rolViewM = rolVM;
            circulos = new ObservableCollection<List<Circulo>>();
            server = new HttpListener();
            server.Prefixes.Add("http://*:15500/websocket/");
            server.Start();
            Task.Run(RecibirPeticiones);
        }

        List<WebSocket> clientesconectado = new List<WebSocket>();
        Dictionary<WebSocket, string> iplist = new Dictionary<WebSocket, string>();

        private async void RecibirPeticiones()
        {
            try
            {
                while (server.IsListening)
                {
                    var context = server.GetContext();

                    if (context.Request.IsWebSocketRequest) //la petición se un websocket handshake
                    {
                        var cws = await context.AcceptWebSocketAsync(null); //acepta la conexión
                        clientesconectado.Add(cws.WebSocket);
                        iplist[cws.WebSocket] = context.Request.RemoteEndPoint.Address.ToString();
                        Enviar(cws.WebSocket, new Datos { ListaTrazos = Trazos.ToList() }); //enviar todos los ocupados al conectarse

                        _ = Task.Run(() =>
                        {
                            Recibir(cws.WebSocket);
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch
            {

            }
        }

        private async void Recibir(WebSocket webSocket)
        {
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[1024 * 10];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)//El cliente quiere cerrar la conexión
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else
                    {
                        //Recibir un lugar desde el cliente, lo deserializo y lo proceso a la lista
                        var json = Encoding.UTF8.GetString(buffer);
                        Circulo t = JsonConvert.DeserializeObject<Circulo>(json);
                        t.Color.Freeze();
                        dispatcher.Invoke(()=> Trazos.Add(t));
                        foreach (var ws in clientesconectado)
                        {
                            if (ws.State == WebSocketState.Open && ws != webSocket)
                            {
                            Enviar(ws, t);
                            }
                        }
                        /*
                        List<Circulo> t = JsonConvert.DeserializeObject<Datos>(json).ListaTrazos;

                        foreach (Circulo c in t)
                        {
                            Trazos.Add(new Circulo { Color = c.Color, Diametro = c.Diametro, Left = c.Left, Top = c.Top });
                        }
                        */
                        /*
                        l.IP = iplist[webSocket] ?? "";

                        var casilla = Lugares.FirstOrDefault(x => x.Posicion == l.Posicion);

                        if (casilla != null && string.IsNullOrWhiteSpace(casilla.Nombre))//si es disponible para guardar el nombre
                        {
                            //Buscar si ya existe en otra casilla
                            var anterior = Lugares.FirstOrDefault(x => x.Nombre == l.Nombre && x.IP == l.IP);
                            if (anterior != null)
                            {
                                anterior.Nombre = "";
                                anterior.IP = "";
                            }



                            casilla.Nombre = l.Nombre;
                            casilla.IP = l.IP;

                            //Enviar el mensaje a todos los demas clientes conectados
                            foreach (var ws in clientesconectado)
                            {
                                if (ws.State == WebSocketState.Open && ws != webSocket)
                                {
                                    Enviar(ws, new Datos { Lugar = l });
                                }
                            }
                        }
                        else
                        {
                            Enviar(webSocket, new Datos { Mensaje = "El lugar ya esta ocupado. Elige otro." });
                        }
                        */
                    }

                }
            }
            catch
            {

            }
        }

        public async void Desconectar()
        {
            
            foreach (var cliente in clientesconectado)
            {
                if (cliente.State == WebSocketState.Open)
                {
                    await cliente.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
            }
            server.Stop();
            RolView rolView = new RolView();
            rolView.DataContext = rolViewM;
            rolViewM.Control = rolView;
        }

        public async void Cerrar()
        {
            server.Stop();
            foreach (var cliente in clientesconectado)
            {
                if (cliente.State == WebSocketState.Open)
                {
                    await cliente.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
            }

        }

        public async void Enviar(WebSocket webSocket, object o)
        {
            var json = JsonConvert.SerializeObject(o);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text, true, CancellationToken.None);

        }

        HttpListener server;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            dispatcher.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }


        public void paintCircle(System.Windows.Point position)
        {/*
            Ellipse newEllipse = new Ellipse { Fill = circleColor, Width = 10, Height = 10 };
            Canvas.SetTop(newEllipse, position.Y - (10 / 2));
            Canvas.SetLeft(newEllipse, position.X - (10 / 2));*/
            int s = 0;
            switch (Size)   
            {
                case 0:
                    s = 5;
                    break;
                case 1:
                    s = 10;
                    break;
                case 2:
                    s = 15;
                    break;
                case 3:
                    s = 20;
                    break;
                case 4:
                    s = 25;
                    break;
                case 5:
                    s = 30;
                    break;
                default:
                    break;
            }

            Brush b;
            switch (Color)
            {
                case 0:
                    b = Brushes.Red;
                    break;
                case 1:
                    b = Brushes.Blue;
                    break;
                case 2:
                    b = Brushes.Green;
                    break;
                case 3:
                    b = Brushes.Black;
                    break;
                default:
                    b = Brushes.Black;
                    break;
            }
            Circulo c = new Circulo { Color = b, Diametro = s, Left = Math.Round(position.X - (s / 2)), Top = Math.Round(position.Y - (s / 2)) };
            Trazos.Add(c);/*
            cnvPaint.Children.Add(newEllipse);
            accion.Add(newEllipse);*/
            foreach (var ws in clientesconectado)
            {
                if (ws.State==WebSocketState.Open)
                {
                    Enviar(ws, c);
                }
               // if (ws.State == WebSocketState.Open && ws != webSocket)
                //{
                    
               // }
            }
        }
    }
}
