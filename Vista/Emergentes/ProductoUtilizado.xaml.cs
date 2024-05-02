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
using System.Windows.Shapes;

namespace DoughMinder___Client.Vista.Emergentes
{
    /// <summary>
    /// Interaction logic for ProductoUtilizado.xaml
    /// </summary>
    public partial class ProductoUtilizado : Window
    {
        public ProductoUtilizado()
        {
            InitializeComponent();
        }

        private void CerrarMensaje(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CambiarAzul(object sender, MouseEventArgs e)
        {
            btnOK.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void Restaurar(object sender, MouseEventArgs e)
        {
            btnOK.Source = new BitmapImage(new Uri("/Recursos/BotonAmarillo.png", UriKind.Relative));
        }
    }
}
