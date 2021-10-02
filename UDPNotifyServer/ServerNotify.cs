using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPNotifyServer
{
    public class ServerNotify
    {
        UdpClient server = new UdpClient() { EnableBroadcast = true};

        public void Enviar(string mensaje, int tipo)
        {
            if (!string.IsNullOrWhiteSpace(mensaje) && tipo > 0)
            {
                string datos = $"{mensaje}|{tipo}";
                byte[] buffer = Encoding.UTF8.GetBytes(datos);
                server.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, 35002));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(mensaje))
                {
                    MessageBox.Show("Por favor ingrese un mensaje para poder ser enviado");
                }
                else
                {
                    MessageBox.Show("Por favor seleccione el tipo de mensaje que desea enviar");
                }
            }
        }
    }
}
