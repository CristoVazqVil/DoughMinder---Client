using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for Recetas.xaml
    /// </summary>
    public partial class Recetas : Page
    {
        public Recetas()
        {
            InitializeComponent();
            CargarRecetas();
        }

        public List<Receta> listaRecetas { get; set; }



        private void CargarRecetas()
        {
            listaRecetas = RecuperarReceta();

            if (listaRecetas != null && listaRecetas.Count > 0)
            {
                lstRecetas.ItemsSource = listaRecetas;
            }
            else
            {
                Console.WriteLine("La lista de productos está vacía.");
            }
        }

        private List<Receta> RecuperarReceta()
        {
            List<Receta> recetas = new List<Receta>();

            try
            {
                DoughMinderServicio.RecetaClient cliente = new DoughMinderServicio.RecetaClient();
                recetas = cliente.RecuperarRecetasCompletas().ToList();

                if (recetas == null)
                {
                    MostrarMensajeSinConexionBase();
                }
            }
            catch (TimeoutException ex)
            {
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                MostrarMensajeSinConexionServidor();
            }

            return recetas;
        }


        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.Show();
        }


        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.Show();
        }

        private void IrAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AbrirRegistrarReceta(object sender, MouseButtonEventArgs e)
        {
            RegistroRecetas registroRecetasPage = new RegistroRecetas();
            NavigationService.Navigate(registroRecetasPage);
        }

        private void BuscarReceta(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = tbBusqueda.Text.ToLower();

            if (listaRecetas != null)
            {
                var productosFiltrados = listaRecetas.Where(emp => emp.Nombre.ToLower().Contains(textoBusqueda) || emp.Codigo.ToLower().Contains(textoBusqueda)).ToList();
                lstRecetas.ItemsSource = productosFiltrados;
            }
        }
    }
}
