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

namespace WpfPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {/*
        private int diametro = (int)Size.chico;
        private Brush brushColor = Brushes.Black;
        private bool isPaint = false;
        private bool isErase = false;
        List<List<Ellipse>> acciones = new List<List<Ellipse>>();
        List<Ellipse> accion;
        Ellipse ellipseTemp = new Ellipse { Width=0};

        private enum Size
        {
            chico=10,
            mediano=15,
            grande=20
        }*/

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {/*
            if (isPaint)
            {
                Point mousePosition = e.GetPosition(cnvPaint);
                paintCircle(brushColor, mousePosition);
            }
            else if (isErase)
            {
                Point mousePosition = e.GetPosition(cnvPaint);
                paintCircle(Brushes.White, mousePosition);
            }*/
        }
        /*
        private void paintCircleTemp(Brush circleColor, Point position)
        {
            if (ellipseTemp.Width!=0)
            {
                cnvPaint.Children.RemoveAt(cnvPaint.Children.Count - 1);
            }
            ellipseTemp = new Ellipse { Fill = circleColor, Width = diametro, Height = diametro };
            Canvas.SetTop(ellipseTemp, position.Y-(diametro/2));
            Canvas.SetLeft(ellipseTemp, position.X- (diametro / 2));
            cnvPaint.Children.Add(ellipseTemp);
        }*/

        

        private void rdbRojo_Checked(object sender, RoutedEventArgs e)
        {
            //brushColor = Brushes.Red;
        }

        private void rdbAzul_Checked(object sender, RoutedEventArgs e)
        {
            //brushColor = Brushes.Blue;
        }

        private void rdbNegro_Checked(object sender, RoutedEventArgs e)
        {
            //brushColor = Brushes.Black;
        }

        private void rdbChico_Checked(object sender, RoutedEventArgs e)
        {
            //diametro = (int)Size.chico;
        }

        private void rdbMediano_Checked(object sender, RoutedEventArgs e)
        {
            //diametro = (int)Size.mediano;
        }

        private void rdbGrande_Checked(object sender, RoutedEventArgs e)
        {
            //diametro = (int)Size.grande;
        }

        private void btnDeshacer_Click(object sender, RoutedEventArgs e)
        {/*
            if (acciones.Count > 0)
            {
                //cnvPaint.Children.RemoveAt(cnvPaint.Children.Count - 1);
                foreach (var item in acciones[acciones.Count-1])
                {
                    //if (cnvPaint.Children.Count > 0)
                    //{
                    //    cnvPaint.Children.RemoveAt(cnvPaint.Children.Count - 1);
                    //}
                }
                acciones.RemoveAt(acciones.Count - 1);
            }*/
        }

        private void cnvPaint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {/*
            ellipseTemp.Width = 0;
            accion = new List<Ellipse>();
            isPaint = true;*/
        }

        private void cnvPaint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*isPaint = false;
            acciones.Add(accion);*/
        }

        private void cnvPaint_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //isErase = true;
        }

        private void cnvPaint_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //isErase = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
