using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPNotifyClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Hide();
        }
        ClientNotify client = new ClientNotify();
        Icon icon0 = new Icon("Assets/notes.ico");
        Icon icon1 = new Icon("Assets/message.ico");
        Icon icon2 = new Icon("Assets/warning.ico");
        Icon icon3 = new Icon("Assets/error.ico");

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            ntfIcon.Visible = true;
            Sta();
            client.MensajeRecibido += Client_MensajeRecibido;
            client.Iniciar();
        }

        private void Sta()
        {
            ntfIcon.BalloonTipTitle = "UDP Mensaje";
            ntfIcon.BalloonTipText = client.Mensaje;
            ntfIcon.Text = client.Mensaje;
            ntfIcon.Icon = icon0;
            ntfIcon.ShowBalloonTip(3000);
        }

        private void Client_MensajeRecibido()
        {
            ntfIcon.BalloonTipTitle = "UDP Mensaje";
            ntfIcon.BalloonTipText = client.Mensaje;
            ntfIcon.Text = client.Mensaje;
            switch (client.Tipo)
            {
                case 1:
                    ntfIcon.Icon = icon1;
                    break;
                case 2:
                    ntfIcon.Icon = icon2;
                    break;
                case 3:
                    ntfIcon.Icon = icon3;
                    break;
                default:
                    ntfIcon.Icon = icon0;
                    break;
            }
            ntfIcon.ShowBalloonTip(3000);
        }

        private void ntfIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
