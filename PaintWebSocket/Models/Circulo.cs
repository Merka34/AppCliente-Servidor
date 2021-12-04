using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfPaint4.Models
{
    public class Circulo : INotifyPropertyChanged
    {
        private Brush color;

        public Brush Color
        {
            get { return color; }
            set { color = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color))); }
        }

        private int diametro;

        public int Diametro
        {
            get { return diametro; }
            set { diametro = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Diametro))); }
        }

        private double left;

        public double Left
        {
            get { return left; }
            set { left = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left))); }
        }

        private double top;

        public double Top
        {
            get { return top; }
            set { top = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top))); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
