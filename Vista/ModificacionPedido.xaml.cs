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
    /// Lógica de interacción para ModificacionPedido.xaml
    /// </summary>
    public partial class ModificacionPedido : Page
    {
        private string clave;
        private string[] estados =
            {
                "Ordenado",
                "En proceso",
                "Preparado",
                "Entregado",
                "Cancelado"
            };
        private int ultimaFila = 1;
        private List<PedidoProducto> productosRecuperados = new List<PedidoProducto>();

        public ModificacionPedido(String clave)
        {
            InitializeComponent();
            this.clave = clave;
            SetProductos();
            SetPedido();
        }

        private void SetPedido()
        {
            Pedido pedido = RecuperarPedido();
            productosRecuperados = RecuperarProductos(pedido.IdPedido);
            lblClave.Content = clave;
            lblTipoEntrega.Content = pedido.TipoEntrega;
            lblTotal.Content = pedido.CostoTotal.ToString();

            if (pedido.TipoEntrega == "Domicilio")
            {
                lblCliente.Content = pedido.NombreCliente;
                lblTelefono.Content = pedido.TelefonoCliente;
                lblDireccion.Text = pedido.Direccion;
            }

            int totalProductos = productosRecuperados.Count;

            for (int i = 0; i < totalProductos; i++)
            {
                string nombreComboBoxProducto = "cmbProducto" + i;
                ComboBox cmbProducto = FindName(nombreComboBoxProducto) as ComboBox;

                if (cmbProducto != null)
                {
                    PedidoProducto productoActual = productosRecuperados[i];
                    cmbProducto.Visibility = Visibility.Visible;


                    if (productoActual != null)
                    {
                        BuscarProducto(cmbProducto, productoActual.ClaveProducto);
                    }
                }
            }
        }

        private void BuscarProducto(ComboBox comboBox, string claveProducto)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var producto = comboBox.Items[i];

                if (producto != null && producto.ToString().Contains(claveProducto))
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
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
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
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

        private void SetProductos()
        {
            ProductoClient productoClient = new ProductoClient();
            List<Producto> productos = new List<Producto>();

            try
            {
                productos = productoClient.RecuperarProductosParaPedido().ToList();

                if (productos == null)
                {
                    MostrarMensajeSinConexionBase();
                }
                else
                {
                    foreach (var producto in productos)
                    {
                        cmbProducto0.Items.Add(producto.Nombre + " (" + producto.CodigoProducto + "): $" + producto.Precio);
                        cmbProducto1.Items.Add(producto.Nombre + " (" + producto.CodigoProducto + "): $" + producto.Precio);
                        cmbProducto2.Items.Add(producto.Nombre + " (" + producto.CodigoProducto + "): $" + producto.Precio);
                        cmbProducto3.Items.Add(producto.Nombre + " (" + producto.CodigoProducto + "): $" + producto.Precio);
                        cmbProducto4.Items.Add(producto.Nombre + " (" + producto.CodigoProducto + "): $" + producto.Precio);
                    }

                    for (int i = 1; i <= 10; i++)
                    {
                        cmbCantidad0.Items.Add(i);
                        cmbCantidad1.Items.Add(i);
                        cmbCantidad2.Items.Add(i);
                        cmbCantidad3.Items.Add(i);
                        cmbCantidad4.Items.Add(i);
                    }
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
        }

        private void CmbProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = cmbProducto0.Text;
            List<object> items = new List<object>();

            foreach (var item in cmbProducto0.ItemsSource)
            {
                if (item != null && item.ToString().ToLower().Contains(texto))
                {
                    items.Add(item);
                }
            }

            cmbCantidad0.ItemsSource = items;
            SetCostoTotal();
        }

        private void SetCosto(ComboBox cmbCantidad, ComboBox cmbProducto, Label lblCosto)
        {
            if (cmbProducto.SelectedItem != null && cmbCantidad.SelectedItem != null)
            {
                string item = cmbProducto.SelectedItem.ToString();
                int indexPrecio = item.IndexOf('$');

                if (indexPrecio != -1)
                {
                    string precioString = item.Substring(indexPrecio + 1).Trim();
                    decimal precio = decimal.Parse(precioString);

                    decimal cantidad = decimal.Parse(cmbCantidad.SelectedItem.ToString());

                    lblCosto.Visibility = Visibility.Visible;
                    decimal costo = precio * cantidad;
                    lblCosto.Content = "$" + costo.ToString();
                }

                lblProductoError.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblCosto.Content = "$0";
                lblProductoError.Visibility = Visibility.Visible;
            }
        }

        private void SetCostoTotal()
        {
            decimal total = 0;

            for (int i = 0; i <= ultimaFila; i++)
            {
                string nombreCosto = "lblCosto" + i.ToString();
                Label lblCosto = this.FindName(nombreCosto) as Label;

                if (lblCosto != null)
                {
                    string item = lblCosto.Content.ToString();
                    int indexCosto = item.IndexOf('$');

                    if (indexCosto != -1)
                    {
                        string costoString = item.Substring(indexCosto + 1).Trim();
                        total += decimal.Parse(costoString);
                    }
                }
            }

            lblTotal.Content = "Costo total: $" + total.ToString();
        }

        private void CmbProducto0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad0, cmbProducto0, lblCosto0);
            SetCostoTotal();
        }

        private void CmbCantidad0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad0, cmbProducto0, lblCosto0);
            SetCostoTotal();
        }

        private void CmbProducto1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad1, cmbProducto1, lblCosto1);
            SetCostoTotal();
        }

        private void CmbCantidad1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad1, cmbProducto1, lblCosto1);
            SetCostoTotal();
        }

        private void CmbProducto2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad2, cmbProducto2, lblCosto2);
            SetCostoTotal();
        }

        private void CmbCantidad2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad2, cmbProducto2, lblCosto2);
            SetCostoTotal();
        }

        private void CmbProducto3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad3, cmbProducto3, lblCosto3);
            SetCostoTotal();
        }

        private void CmbProducto4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad4, cmbProducto4, lblCosto4);
            SetCostoTotal();
        }

        private void CmbCantidad3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad3, cmbProducto3, lblCosto3);
            SetCostoTotal();
        }

        private void CmbCantidad4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad4, cmbProducto4, lblCosto4);
            SetCostoTotal();
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (ultimaFila < 5)
            {
                string nombreInsumo = "cmbProducto" + ultimaFila.ToString();
                ComboBox comboBox = this.FindName(nombreInsumo) as ComboBox;

                if (comboBox != null)
                {
                    comboBox.Visibility = Visibility.Visible;
                }

                string nombreCantidad = "cmbCantidad" + ultimaFila.ToString();
                ComboBox comboBox1 = this.FindName(nombreCantidad) as ComboBox;

                if (comboBox1 != null)
                {
                    comboBox1.Visibility = Visibility.Visible;
                }

                ultimaFila++;
            }
            else
            {
                MessageBox.Show("No se pueden agregar más productos");
            }
        }

        private bool ValidarProductos()
        {
            bool esValido = true;

            if (cmbCantidad0.IsVisible && (cmbCantidad0.SelectedItem == null || cmbProducto0.SelectedItem == null))
            {
                esValido = false;
            }
            if (cmbCantidad1.IsVisible && (cmbCantidad1.SelectedItem == null || cmbProducto1.SelectedItem == null))
            {
                esValido = false;
            }
            if (cmbCantidad2.IsVisible && (cmbCantidad2.SelectedItem == null || cmbProducto2.SelectedItem == null))
            {
                esValido = false;
            }
            if (cmbCantidad3.IsVisible && (cmbCantidad3.SelectedItem == null || cmbProducto3.SelectedItem == null))
            {
                esValido = false;
            }
            if (cmbCantidad4.IsVisible && (cmbCantidad4.SelectedItem == null || cmbProducto4.SelectedItem == null))
            {
                esValido = false;
            }

            return esValido;
        }

        private bool ValidarInformacion()
        {
            bool esValido = true;

            if (!ValidarProductos())
            {
                esValido = false;
                lblProductoError.Content = "Verifica los productos y la cantidad seleccionada";
                lblProductoError.Visibility = Visibility.Visible;
            }

            return esValido;
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
    }
}
