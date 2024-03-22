using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para RegistroPedido.xaml
    /// </summary>
    public partial class RegistroPedido : Page
    {

        private int ultimaFila = 1;

        enum TipoPago
        {
            Efectivo,
            Tarjeta,
        }

        public RegistroPedido()
        {
            InitializeComponent();
            SetFecha();
            SetProductos();
            SetTipoPago();
        }

        private void SetFecha()
        {
            DateTime fechaActual = DateTime.Now;
            lblFecha.Content = fechaActual.ToString();
        }

        private void SetTipoPago()
        {
            cmbTipoPago.Items.Add(TipoPago.Efectivo);
            cmbTipoPago.Items.Add(TipoPago.Tarjeta);
        }

        private void SetProductos()
        {
            DoughMinderServicio.ProductoClient productoClient = new DoughMinderServicio.ProductoClient();
            List<DoughMinderServicio.Producto> productos = new List<Producto>();

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

                    for(int i = 1;  i <= 10; i++)
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
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
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
                lblProductoError.Content = "Recuerda verificar que ingresas los productos correctos";
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

            lblCostoTotalSitio.Content = "Costo total: $" + total.ToString();
            lblCostoTotalDomicilio.Content = "Costo total: $" + total.ToString();
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

        private void MostrarMensajeRegistroExitoso()
        {
            RegistroExitoso registroExitoso = new RegistroExitoso();
            registroExitoso.Show();
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

        private void CmbTipoPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTipoPagoError.Visibility = Visibility.Collapsed;

            if (cmbTipoPago.SelectedIndex == 0)
            {
                lblCantidadIngresada.Visibility = Visibility.Collapsed;
                txtbCantidadIngresada.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblCantidadIngresada.Visibility = Visibility.Visible;
                txtbCantidadIngresada.Visibility = Visibility.Visible;
            }
        }

        private void ChxDomicilio_Checked(object sender, RoutedEventArgs e)
        {
            gpSitio.Visibility = Visibility.Collapsed;
            gpDomicilio.Visibility = Visibility.Visible;
            cmbTipoPago.IsEnabled = false;

        }

        private void TxtbCantidadIngresada_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblCantidadIngresadaError.Visibility = Visibility.Collapsed;
        }

        private void ChxDomicilio_Unchecked(object sender, RoutedEventArgs e)
        {
            gpDomicilio.Visibility= Visibility.Collapsed;
            gpSitio.Visibility= Visibility.Visible;
            cmbTipoPago.IsEnabled = true;
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

            if (gpDomicilio.Visibility == Visibility.Visible)
            {
                if (!ValidarNombreCliente(txtbCliente.Text))
                {
                    esValido = false;
                    lblNombreClienteError.Content = "Este campo no puede estar vacío";
                    lblNombreClienteError.Visibility = Visibility.Visible;
                }

                if (!ValidarDomicilio(txtbDireccion.Text))
                {
                    esValido = false;
                    lblDireccionError.Content = "Este campo no puede estar vacío";
                    lblDireccionError.Visibility = Visibility.Visible;
                }

                if (!ValidarTelefono(txtbTelefono.Text))
                {
                    esValido = false;
                    lblTelefonoError.Content = "Este campo no puede estar vacío";
                    lblTelefonoError.Visibility = Visibility.Visible;
                }

                if (!ValidarTarjeta(txtbTarjeta.Text))
                {
                    esValido = false;
                    lblTarjetaError.Content = "Este campo no puede estar vacío y debe tener 16 dígitos";
                    lblTarjetaError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (cmbTipoPago.SelectedIndex == 1)
                {
                    if (!ValidarTarjeta(txtbCantidadIngresada.Text))
                    {
                        esValido = false;
                        lblCantidadIngresadaError.Content = "Este campo no puede estar vacío y debe tener 16 dígitos";
                        lblCantidadIngresadaError.Visibility = Visibility.Visible;
                    }
                }
                else if (cmbTipoPago.SelectedItem == null)
                {
                    esValido = false;
                    lblTipoPagoError.Content = "Este campo no puede estar vacío";
                    lblTipoPagoError.Visibility = Visibility.Visible;
                }
            }

            return esValido;
        }

        private bool ValidarTarjeta(string tarjeta)
        {
            string regex = @"^[0-9]+$";
            return Regex.IsMatch(tarjeta, regex);
        }

        private bool ValidarNombreCliente(string nombre)
        {
            string regex = @"^(?!\s)(?!.*\s$)[a-zA-Z0-9\s]{3,49}$";
            return Regex.IsMatch(nombre, regex);
        }

        private bool ValidarDomicilio(string domicilio)
        {
            string regex = @"^[^\s].*[^\s]$";
            return Regex.IsMatch(domicilio, regex);
        }

        private bool ValidarTelefono(string telefono)
        {
            string regex = @"^[0-9]+$";
            return Regex.IsMatch(telefono, regex);
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

        private void TxtbDireccion_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblDireccionError.Visibility = Visibility.Collapsed;
        }

        private void TxtbCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblNombreClienteError.Visibility = Visibility.Collapsed;
        }

        private void TxtbTelefono_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblTelefonoError.Visibility = Visibility.Collapsed;
        }

        private void TxtbTarjeta_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblTarjetaError.Visibility = Visibility.Collapsed;
        }

        private void BtnPagar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidarInformacion())
            {
                RegistrarPedido();
            }
        }

        private void RegistrarPedido()
        {
            PedidoClient pedidoClient = new PedidoClient();
            List<PedidoProducto> pedidoProductos = new List<PedidoProducto>();
            Pedido pedido = new Pedido();
            int codigo = 0;

            PedidoProducto producto0= RecuperarProducto(cmbProducto0, cmbCantidad0);
            PedidoProducto producto1 = RecuperarProducto(cmbProducto1, cmbCantidad1);
            PedidoProducto producto2 = RecuperarProducto(cmbProducto2, cmbCantidad2);
            PedidoProducto producto3 = RecuperarProducto(cmbProducto3, cmbCantidad3);
            PedidoProducto producto4 = RecuperarProducto(cmbProducto4, cmbCantidad4);

            if (producto0 != null)
            {
                pedidoProductos.Add(producto0);
            }

            if (producto1 != null)
            {
                pedidoProductos.Add(producto1);
            }

            if (producto2 != null)
            {
                pedidoProductos.Add(producto2);
            }

            if (producto3 != null)
            {
                pedidoProductos.Add(producto3);
            }

            if (producto4 != null)
            {
                pedidoProductos.Add(producto4);
            }

            string fechaString = lblFecha.Content.ToString();
            DateTime fecha = DateTime.Parse(fechaString);
            pedido.Fecha = fecha;

            string costoString = lblCostoTotalSitio.Content.ToString().Substring(14);
            decimal costoTotal = decimal.Parse(costoString);
            pedido.CostoTotal = costoTotal;

            pedido.IdEstadoPedido = 1;

            if (chxDomicilio.IsChecked == true)
            {
                string nombreCliente = txtbCliente.Text;
                pedido.NombreCliente = nombreCliente;

                string domicilio = txtbDireccion.Text;
                pedido.Direccion = domicilio;

                string telefono = txtbTelefono.Text;
                pedido.TelefonoCliente = telefono;
            }

            try
            {
                codigo = pedidoClient.RegistrarPedido(pedido, pedidoProductos.ToArray());
                if (codigo > 0)
                {
                    MostrarMensajeRegistroExitoso();
                }
                else
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
        }

        private PedidoProducto RecuperarProducto(ComboBox cmbProducto, ComboBox cmbCantidad)
        {
            PedidoProducto pedidoProducto = null;

            if (cmbProducto.SelectedItem != null && cmbCantidad.SelectedItem != null)
            {
                pedidoProducto = new PedidoProducto();

                string productoItem = cmbProducto.SelectedItem.ToString();
                int inicio = productoItem.IndexOf("(") + 1;
                int fin = productoItem.IndexOf(")");
                string codigoProducto = productoItem.Substring(inicio, fin - inicio);
                pedidoProducto.ClaveProducto = codigoProducto;

                string cantidadItem = cmbCantidad.SelectedItem.ToString();
                int cantidad = int.Parse(cantidadItem);
                pedidoProducto.Cantidad = cantidad;
            }

            return pedidoProducto;
        }
    }
}
