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
    /// Lógica de interacción para PedidoVista.xaml
    /// </summary>

    class ProductoPedidoLista
    {
        public string Producto { get; set; }
        public string Cantidad { get; set; }
        public string Costo { get; set; }
        public string Total { get; set; }
    }

    public partial class PedidoVista : Page
    {
        private string clave;

        public PedidoVista(string clave)
        {
            InitializeComponent();
            this.clave = clave;
            SetPedido();
        }

        private void SetPedido()
        {
            Pedido pedido = RecuperarPedido();

            if (pedido != null)
            {
                lblClave.Content = clave;
                lblTipoEntrega.Content = "Tipo de entrega: " + pedido.TipoEntrega;
                lblTotal.Content = "Costo total: $" + pedido.CostoTotal.ToString();
                lblFecha.Content = "Fecha: " + pedido.Fecha.ToString();
                lblEstado.Content = "Estado: " + pedido.Estado;
                lblEmpleado.Content = "Empleado: " + RecuperarEmpleado(pedido.Usuario);

                if (pedido.TipoEntrega.Equals("Domicilio"))
                {
                    lblCliente.Content = "Cliente: " + pedido.NombreCliente;
                    lblCliente.Visibility = Visibility.Visible;

                    lblTelefono.Content = "Teléfono: " + pedido.TelefonoCliente;
                    lblTelefono.Visibility = Visibility.Visible;

                    lblDireccion.Text = "Dirección: " + pedido.Direccion;
                    lblDireccion.Visibility = Visibility.Visible;
                }

                SetProductos(pedido.IdPedido);
            }
        }

        private void SetProductos(int idPedido)
        {
            List<ProductoPedidoLista> productosPedidoLista = new List<ProductoPedidoLista>();
            List<PedidoProducto> pedidoProductos = RecuperarProductos(idPedido);

            foreach (PedidoProducto productoPedido in pedidoProductos)
            {
                Producto producto = BuscarProducto(productoPedido);

                ProductoPedidoLista productoPedidoLista = new ProductoPedidoLista
                {
                    Cantidad = productoPedido.Cantidad.ToString() + " producto(s)",
                    Costo = "$" + producto.Precio.ToString() + " c/u",
                    Producto = producto.Nombre,
                    Total = "Total: $" + (producto.Precio * productoPedido.Cantidad).ToString(),
                };

                productosPedidoLista.Add(productoPedidoLista);
            }

            lstProductos.ItemsSource = productosPedidoLista;
            lstProductos.Visibility = Visibility.Visible;
        }

        private Producto BuscarProducto(PedidoProducto pedidoProducto)
        {
            List<Producto> productos = RecuperarTodosProductos();
            Producto productoRecuperado = new Producto();

            foreach (Producto producto in productos)
            {
                if (pedidoProducto.ClaveProducto == producto.CodigoProducto)
                {
                    productoRecuperado = producto;
                    break;
                }
            }

            return productoRecuperado;
        }

        private string RecuperarEmpleado(string usuario)
        {
            EmpleadoClient empleadoClient = new EmpleadoClient();
            Empleado empleado = new Empleado();

            try
            {
                empleado = empleadoClient.BuscarEmpleadoPorUsuario(usuario);

                if (empleado == null)
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

            return empleado.Nombre + " " + empleado.Paterno + " " + empleado.Materno;
        }

        private Pedido RecuperarPedido()
        {
            PedidoClient pedidoClient = new PedidoClient();
            Pedido pedido = new Pedido();

            try
            {
                pedido = pedidoClient.RecuperarPedido(clave);

                if (pedido == null)
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

            return pedido;
        }

        private List<PedidoProducto> RecuperarProductos(int idPedido)
        {
            ProductoClient productoClient = new ProductoClient();
            List<PedidoProducto> productos = new List<PedidoProducto>();

            try
            {
                productos = productoClient.RecuperarProductosPorPedido(idPedido).ToList();

                if (productos == null)
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

            return productos;
        }

        private List<Producto> RecuperarTodosProductos()
        {
            List<Producto> productos = new List<Producto>();
            ProductoClient productoClient = new ProductoClient();

            try
            {
                productos = productoClient.RecuperarProductos().ToList();

                if (productos == null)
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

        private void MostrarVentanaMenuPedidos()
        {
            MenuPedidos menuPedidos = new MenuPedidos();
            NavigationService.Navigate(menuPedidos);
        }

        private void MostrarVentanaModificarPedido()
        {
            ModificacionPedido modificacionPedido = new ModificacionPedido(clave);
            NavigationService.Navigate(modificacionPedido);
        }

        private void BtnAtras_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarVentanaMenuPedidos();
        }

        private void BtnModificar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarVentanaModificarPedido();
        }

    }
}