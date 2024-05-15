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
using static DoughMinder___Client.Vista.MenuEmpleados;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for Productos.xaml
    /// </summary>
    public partial class MenuProductos : Page
    {
        public MenuProductos()
        {
            InitializeComponent();
            CargarProductosParaPedido();
        }


        public List<Producto> listaProductos { get; set; }

        private void CargarProductosParaPedido()
        {
            listaProductos = RecuperarProductos();

            if (listaProductos != null && listaProductos.Count > 0)
            {
                lstProductos.ItemsSource = listaProductos;
            }
            else
            {
                Console.WriteLine("La lista de productos está vacía.");
            }
        }


        private List<Producto> RecuperarProductos()
        {
            List<Producto> productos = new List<Producto>();

            try
            {
                DoughMinderServicio.ProductoClient cliente = new DoughMinderServicio.ProductoClient();
                 productos = cliente.RecuperarProductos().ToList();

                if (productos == null)
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

            return productos;
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


        private void BuscarProducto(object sender, TextChangedEventArgs e)
        { 
            string textoBusqueda = tbBusqueda.Text.ToLower();

            if (listaProductos != null) 
            {
                var productosFiltrados = listaProductos.Where(emp => emp.Nombre.ToLower().Contains(textoBusqueda) || emp.CodigoProducto.ToLower().Contains(textoBusqueda)).ToList();
                lstProductos.ItemsSource = productosFiltrados;
            }
            
        }
          

        private void AbrirRegistroProductos(object sender, MouseButtonEventArgs e)
        {
            RegistroProductos registroProductosPage = new RegistroProductos();
            NavigationService.Navigate(registroProductosPage);
        }

        private void IrAtras(object sender, MouseButtonEventArgs e)
        {
            MenuPrincipal principal = new MenuPrincipal();
            this.NavigationService.Navigate(principal);
        }

        private void ModificarProducto(object sender, MouseButtonEventArgs e)
        {
            Producto productoSeleccionado = (Producto)lstProductos.SelectedItem;

            if (productoSeleccionado != null)
            {
                string codigoProducto = productoSeleccionado.CodigoProducto;
                ModificacionProductos modificacionProductos = new ModificacionProductos(codigoProducto);
                NavigationService.Navigate(modificacionProductos);
            }
        }
    }
}
