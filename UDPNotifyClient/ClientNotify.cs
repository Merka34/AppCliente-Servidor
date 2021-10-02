using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPNotifyClient
{
    public class ClientNotify
    {
        public string Mensaje { get; set; } = "Corriendo en segundo plano";
        public int Tipo { get; set; } = 0;
        public event Action MensajeRecibido;
        UdpClient listener;

        public void Iniciar()
        {
            Task.Run(IniciarServidor);
        }

        private void IniciarServidor()
        {
            try
            {
                int puerto = 35002;
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, puerto);
                listener = new UdpClient();
                listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Client.Bind(ep);
                while (true)
                {
                    byte[] datos = listener.Receive(ref ep);
                    if (datos.Length>0)
                    {
                        string mensaje = Encoding.UTF8.GetString(datos);
                        var infos = mensaje.Split('|');
                        Mensaje = infos[0];
                        Tipo = int.Parse(infos[1]);
                        ThreadSafeEventLaunch();
                    }
                }
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        void ThreadSafeEventLaunch()
        {
            if (MensajeRecibido != null)
            {
                foreach (var d in MensajeRecibido.GetInvocationList())
                {
                    ISynchronizeInvoke i = d.Target as ISynchronizeInvoke;
                    i.Invoke(MensajeRecibido, null);
                }
            }
        }
    }
}
