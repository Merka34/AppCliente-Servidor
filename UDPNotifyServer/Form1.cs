using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPNotifyServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ServerNotify server = new ServerNotify();

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            server.Enviar(txtMensaje.Text, cmbTipo.SelectedIndex+1);
        }
    }
}
