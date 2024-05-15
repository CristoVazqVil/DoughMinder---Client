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
    /// Interaction logic for MenuInsumos.xaml
    /// </summary>
    public partial class MenuInsumos : Page
    {
        public MenuInsumos()
        {
            InitializeComponent();
            CargarInsumos();
        }

        public List<Insumo> listaInsumos { get; set; }


        private void CargarInsumos()
        {
            listaInsumos = RecuperarInsumos();

            if (listaInsumos != null && listaInsumos.Count > 0)
            {
                lstInsumos.ItemsSource = listaInsumos;
            }
            else
            {
                Console.WriteLine("La lista de productos está vacía.");
            }
        }

        private List<Insumo> RecuperarInsumos()
        {
            List<Insumo> insumos = new List<Insumo>();

            try
            {
                DoughMinderServicio.InsumoClient cliente = new DoughMinderServicio.InsumoClient();
                insumos = cliente.RecuperarTodosInsumos().ToList();

                if (insumos == null)
                {
                    MostrarMensajeSinConexionBase();
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }

            return insumos;
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
            MenuPrincipal principal = new MenuPrincipal();
            this.NavigationService.Navigate(principal);
        }

        private void AbrirRegistrarInsumo(object sender, MouseButtonEventArgs e)
        {
            RegistroInsumos registroInsumos = new RegistroInsumos();
            NavigationService.Navigate(registroInsumos);
        }

        private void AbrirSolicitarInsumos(object sender, MouseButtonEventArgs e)
        {
            SolicitudInsumo solicitudInsumo = new SolicitudInsumo();
            NavigationService.Navigate(solicitudInsumo);
        }

        private void BuscarInsumo(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = tbBusqueda.Text.ToLower();

            if (listaInsumos != null)
            {
                var insumosFiltrados = listaInsumos.Where(emp => emp.Nombre.ToLower().Contains(textoBusqueda) || emp.Codigo.ToLower().Contains(textoBusqueda)).ToList();
                lstInsumos.ItemsSource = insumosFiltrados;
            }
        }

        private void ModificarInsumo(object sender, MouseButtonEventArgs e)
        {
            Insumo insumoSeleccionado = (Insumo)lstInsumos.SelectedItem;

            if (insumoSeleccionado != null)
            {
                string codigoInsumo = insumoSeleccionado.Codigo;
                ModificacionInsumos modificacionInsumos = new ModificacionInsumos(codigoInsumo);
                NavigationService.Navigate(modificacionInsumos);
            }
        }
    }
}
