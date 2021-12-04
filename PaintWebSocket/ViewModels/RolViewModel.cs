using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfPaint4.Models;
using WpfPaint4.Views;

namespace WpfPaint4.ViewModels
{
    public class RolViewModel : INotifyPropertyChanged
    {
        private bool rol = true;

        public bool Rol //ClienteoServidor
        {
            get { return rol; }
            set
            {
                rol = value;
                OnPropertyChanged();
            }
        }

        private string ip = "ws://127.0.0.1:15500/websocket";

        public string IP //IpServidorRemote
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }

        private UserControl control;

        public UserControl Control
        {
            get { return control; }
            set
            {
                control = value;
                OnPropertyChanged();
            }
        }

        private string error;

        public string Errores
        {
            get { return error; }
            set
            {
                error = value;
                OnPropertyChanged();
            }
        }


        public ICommand IniciarCommand { get; set; }
        PizzaraView pizzaraView = new PizzaraView();
        RolView rolView = new RolView();

        public RolViewModel()
        {
            IniciarCommand = new RelayCommand(Iniciar);
            rolView.DataContext = this;
            Control = rolView;
        }

        private void Iniciar()
        {
            Errores = "";

            if (Rol) //Rol true soy server
            {
                ServidorViewModel server = new ServidorViewModel(this);
                pizzaraView.DataContext = server;
                pizzaraView.GetContext(server);
                Control = pizzaraView;
            }
            else //soy cliente
            {
                ClienteViewModel cliente = new ClienteViewModel(IP, this);
                pizzaraView.DataContext = cliente;
                pizzaraView.GetContext(cliente);
                Control = pizzaraView;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void CerrarTodo()
        {
            var db = pizzaraView.DataContext;
            if (db is ServidorViewModel)
            {
            //    ((ServidorViewModel)db).Cerrar();
            }
            else
            {
            //    ((ClienteViewModel)db).Cerrar();
            }
        }
    }
}
