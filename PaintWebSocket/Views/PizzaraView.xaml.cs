using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfPaint4.ViewModels;

namespace WpfPaint4.Views
{
    /// <summary>
    /// Lógica de interacción para PizzaraView.xaml
    /// </summary>
    public partial class PizzaraView : UserControl
    {
        Object Context;
        public PizzaraView()
        {
            InitializeComponent();
        }

        public void GetContext(object o)
        {
            Type type = o.GetType();
            if (type == typeof(ServidorViewModel))
            {
                Context = (ServidorViewModel)this.DataContext;
            }
            else
            {
                Context = (ClienteViewModel)this.DataContext;
            }
        }

        private void cnvPaint_MouseMove(object sender, MouseEventArgs e)
        {
            

        }

        private void cnvPaint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext.GetType()==typeof(ServidorViewModel))
            {
                ServidorViewModel server = (ServidorViewModel)this.DataContext;
                Point p = e.GetPosition(this.cnvPaint);
                server.paintCircle(p);
            }
            else if(this.DataContext.GetType() == typeof(ClienteViewModel))
            {
                ClienteViewModel cliente = (ClienteViewModel)this.DataContext;
                Point p = e.GetPosition(this.cnvPaint);
                cliente.paintCircle(p);
            }
        }
    }
}
