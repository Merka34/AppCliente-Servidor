using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfPaint4.Models;
using WpfPaint4.Views;

namespace WpfPaint4.ViewModels
{
    public class ClienteViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Circulo> circulos;
        Dispatcher dispatcher;
        public ObservableCollection<Circulo> Circulos
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

        public int Color { get; set; }
        private int size;
        public int Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged(); }
        }

        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; OnPropertyChanged(); }
        }

        private int posicion;

        public int Posicion
        {
            get { return posicion; }
            set { posicion = value; OnPropertyChanged(); }
        }

        public ICommand DesconectarCommand { get; set; }


        ClientWebSocket cliente = new ClientWebSocket();
        RolViewModel rolViewM;
        public ClienteViewModel(string direccion, RolViewModel rolVM)
        {
            
            dispatcher = Dispatcher.CurrentDispatcher;
            Direccion = direccion;
            circulos = new ObservableCollection<Circulo>();
            DesconectarCommand = new RelayCommand(Desconectar);
            rolViewM = rolVM;
            Conectar();
        }

        public void paintCircle(System.Windows.Point position)
        {
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
            Trazos.Add(c);
            Enviar(c);
        }

        public void Desconectar()
        {
            dispatcher.Invoke(() => {
                if (cliente != null && cliente.State == WebSocketState.Open)
                {
                    cliente.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
                RolView rolView = new RolView();
                rolViewM.Errores = Error;
                rolView.DataContext = rolViewM;
                rolViewM.Control = rolView;
            });
        }

        public async void Cerrar()
        {
            if (cliente != null && cliente.State == WebSocketState.Open)
            {
                await cliente.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }

        private async void Conectar()
        {
            try
            {
                Error = "";
                //await cliente.ConnectAsync(new Uri($"ws://{Direccion}:12500/websocket/"), CancellationToken.None);
                await cliente.ConnectAsync(new Uri($"{Direccion}"), CancellationToken.None);

                _ = Task.Run(Recibir);
            }
            catch (Exception)
            {
                Error = "No se logro conectarse con el servidor";
                Desconectar();
            }
        }

        private async void Recibir()
        {
            while (cliente.State == WebSocketState.Open)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 10];
                    WebSocketReceiveResult resultado = await cliente.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (resultado.MessageType == WebSocketMessageType.Close)
                    {
                        //await cliente.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        Error = "El servidor ha cerrado la conexión";
                        Desconectar();
                    }
                    else if (resultado.MessageType == WebSocketMessageType.Text)
                    {
                        var json = Encoding.UTF8.GetString(buffer);
                        //string json = json2.Replace("\0", "");
                        Datos datos = JsonConvert.DeserializeObject<Datos>(json);

                        if (datos.Trazo != null)
                        {
                            datos.Trazo.Color.Freeze();
                            dispatcher.Invoke(() => Trazos.Add(datos.Trazo));
                        }
                        else if (datos.ListaTrazos != null)
                        {
                            /*
                            var Circulo = Trazos[0];
                            Circulo.Color = datos.ListaTrazos[0].Color;
                            Circulo.Color.Freeze();
                            Circulo.Diametro = datos.ListaTrazos[0].Diametro;
                            Circulo.Top = datos.ListaTrazos[0].Top;
                            Circulo.Left = datos.ListaTrazos[0].Left;
                            */
                            foreach (Circulo c in datos.ListaTrazos)
                            {
                                c.Color.Freeze();
                                dispatcher.Invoke(() => Trazos.Add(c));
                            }

                            //await this.dispatcher.BeginInvoke(new ThreadStart(() => AgregarCirculos(datos.ListaTrazos)));
                            /*dispatcher.Invoke(() =>
                            {
                                foreach (var l in datos.ListaTrazos)
                                {
                                    MyTrazos.Add(new Circulo { Color = l.Color, Diametro = l.Diametro, Left = l.Left, Top = l.Top });
                                }
                            });*/
                        }
                        else
                        {
                            Circulo c = JsonConvert.DeserializeObject<Circulo>(json);
                            c.Color.Freeze();
                            dispatcher.Invoke(() => Trazos.Add(c));
                        }
                        /*
                        else
                        {

                            var casilla = Lugares.FirstOrDefault(x => x.Posicion == datos.Lugar.Posicion);

                            if (casilla != null && string.IsNullOrWhiteSpace(casilla.Nombre))//si es disponible para guardar el nombre
                            {
                                //Buscar si ya existe en otra casilla
                                var anterior = Lugares.FirstOrDefault(x => x.Nombre == datos.Lugar.Nombre && x.IP == datos.Lugar.IP);
                                if (anterior != null)
                                {
                                    anterior.Nombre = "";
                                    anterior.IP = "";
                                }
                                casilla.Nombre = datos.Lugar.Nombre;
                                casilla.IP = datos.Lugar.IP;
                            }
                        }*/

                    }
                }
                catch (Exception)
                {
                    Desconectar();
                }
            }
        }

        private void AgregarCirculos(List<Circulo> miLista)
        {
            foreach (Circulo l in miLista)
            {
                Trazos.Add(l);
            }
        }

        public async void Enviar(object o)
        {
            var json = JsonConvert.SerializeObject(o);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            await cliente.SendAsync(new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public string Direccion { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
