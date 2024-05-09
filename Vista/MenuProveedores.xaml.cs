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
    /// Interaction logic for Proveedores.xaml
    /// </summary>
    public partial class MenuProveedores : Page
    {
        public MenuProveedores()
        {
            InitializeComponent();
            CargarProveedores();
            lstProveedores.MouseDoubleClick += lstProveedores_MouseDoubleClick;
        }

        public List<Proveedor> listaProveedores { get; set; }


        private void CargarProveedores()
        {
            listaProveedores = RecuperarProvedores();

            if (listaProveedores != null && listaProveedores.Count > 0)
            {
                lstProveedores.ItemsSource = listaProveedores;
            }
            else
            {
                Console.WriteLine("La lista de productos está vacía.");
            }
        }

        private List<Proveedor> RecuperarProvedores()
        {
            List<Proveedor> proveedores = new List<Proveedor>();

            try
            {
                DoughMinderServicio.ProveedorClient cliente = new DoughMinderServicio.ProveedorClient();
                proveedores = cliente.RecuperarProveedores().ToList();

                if (proveedores == null)
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

            return proveedores;
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

        private void BuscarProveedor(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = tbBusqueda.Text.ToLower();

            if (listaProveedores != null)
            {
                var productosFiltrados = listaProveedores.Where(emp => emp.Nombre.ToLower().Contains(textoBusqueda) || emp.RFC.ToLower().Contains(textoBusqueda)).ToList();
                lstProveedores.ItemsSource = productosFiltrados;
            }
        }

        private void lstProveedores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Proveedor proveedorSeleccionado = (Proveedor)lstProveedores.SelectedItem;

            RegistroProveedores registroProveedores = new RegistroProveedores(proveedorSeleccionado);
            this.NavigationService.Navigate(registroProveedores);
        }

        private void AbrirRegistrarNuevoProveedor(object sender, MouseButtonEventArgs e)
        {

            Proveedor proveedorNuevo = new Proveedor();

            RegistroProveedores registroProveedores = new RegistroProveedores(proveedorNuevo);
            this.NavigationService.Navigate(registroProveedores);
        }

        private void clickIrAtrás(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
