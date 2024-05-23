
using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Recursos.Singleton;
using DoughMinder___Client.Vista.Emergentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para RegistroPedido.xaml
    /// </summary>
    public partial class RegistroPedido : Page
    {
        private int ultimaFila = 1;
        private List<Producto> productos = new List<Producto>();
        private string[] productosAgregados = new string[5];
        private bool esCmbSeleccionadoPorEmpleado;


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
            SetEmpleado();
        }

        private void SetEmpleado()
        {
            lblEmpleado.Content = "Empleado: " + SesionSingleton.Instance.Nombre;
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
            cmbTipoPago.SelectedIndex = 0;
        }

        private void SetProductos()
        {
            ProductoClient productoClient = new ProductoClient();

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
            }
        }

        private void SetCostoTotal()
        {
            decimal total = 0;

            for (int i = 0; i <= ultimaFila; i++)
            {
                Label lblCosto = ObtenerLabelCostoProducto(i);

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

        private void MostrarMensajeRegistroExitoso(string mensaje)
        {
            RegistroExitoso registroExitoso = new RegistroExitoso();
            registroExitoso.SetContenidoMensaje(mensaje);
            registroExitoso.Show();
        }

        private void MostrarVentanaMenuPedidos()
        {
            MenuPedidos menuPedidos = new MenuPedidos();
            NavigationService.Navigate(menuPedidos);
        }

        private void MostrarMensajeInformacionIncorrecta(string mensaje)
        {
            InformacionIncorrecta informacionIncorrecta = new InformacionIncorrecta();
            informacionIncorrecta.SetContenidoMensaje(mensaje);
            informacionIncorrecta.Show();
        }

        private void SetComboBoxCantidad(ComboBox comboBoxProducto, ComboBox comboBoxCantidad)
        {
            var productoSeleccionado = comboBoxProducto.SelectedItem;

            if (productoSeleccionado != null)
            {
                foreach (var producto in productos)
                {
                    if (productoSeleccionado.ToString().Contains(producto.Nombre))
                    {
                        RegistrarCantidades(comboBoxCantidad, (int)producto.Cantidad);
                    }
                }
            }
        }

        private void RegistrarCantidades(ComboBox comboBoxCantidad, int cantidad)
        {
            comboBoxCantidad.Items.Clear();
            comboBoxCantidad.IsEnabled = true;

            for (int i = 1; i <= cantidad; i++)
            {
                comboBoxCantidad.Items.Add(i);
            }
        }

        private void QuitarProductoSeleccionado(ComboBox comboBox, int numComboBox)
        {
            if (esCmbSeleccionadoPorEmpleado && comboBox.SelectedIndex > -1)
            {
                if (!string.IsNullOrEmpty(productosAgregados[numComboBox]))
                {
                    string productoAnterior = productosAgregados[numComboBox];
                    AgregarProductoAnteriorEnCombobox(numComboBox, productoAnterior);
                }

                string productoSeleccionado = comboBox.SelectedValue.ToString();
                productosAgregados[numComboBox] = productoSeleccionado;
                QuitarProductoSeleccionadoEnCombobox(numComboBox, productoSeleccionado);
            }
        }


        private void AgregarProductoAnteriorEnCombobox(int numComboBox, string productoAnterior)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i != numComboBox)
                {
                    ComboBox cmbProducto = ObtenerComboBoxProducto(i);
                    cmbProducto.Items.Add(productoAnterior);
                }
            }
        }

        private void QuitarProductoSeleccionadoEnCombobox(int numComboBox, string productoSeleccionado)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i != numComboBox)
                {
                    ComboBox cmbProducto = ObtenerComboBoxProducto(i);
                    cmbProducto.Items.Remove(productoSeleccionado);
                }
            }
        }

        private void CmbProducto0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbProducto0, 0);
            SetComboBoxCantidad(cmbProducto0, cmbCantidad0);
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
            QuitarProductoSeleccionado(cmbProducto1, 1);
            SetComboBoxCantidad(cmbProducto1, cmbCantidad1);
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
            QuitarProductoSeleccionado(cmbProducto2, 2);
            SetComboBoxCantidad(cmbProducto2, cmbCantidad2);
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
            QuitarProductoSeleccionado(cmbProducto3, 3);
            SetComboBoxCantidad(cmbProducto3, cmbCantidad3);
            SetCosto(cmbCantidad3, cmbProducto3, lblCosto3);
            SetCostoTotal();
        }

        private void CmbProducto4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbProducto4, 4);
            SetComboBoxCantidad(cmbProducto4, cmbCantidad4);
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
            AgregarProducto();
        }

        private void AgregarProducto()
        {
            if (ultimaFila < 5)
            {
                ComboBox cmbProducto = ObtenerComboBoxProducto(ultimaFila);
                cmbProducto.Visibility = Visibility.Visible;

                ComboBox cmbCantidad = ObtenerComboBoxCantidad(ultimaFila);
                cmbCantidad.Visibility = Visibility.Visible;

                Image imgEliminar = ObtenerImageEliminar(ultimaFila);
                imgEliminar.Visibility = Visibility.Visible;

                Label lblCosto = ObtenerLabelCostoProducto(ultimaFila);
                lblCosto.Visibility = Visibility.Visible;

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
            gpDomicilio.Visibility = Visibility.Collapsed;
            gpSitio.Visibility = Visibility.Visible;
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
            else
            {
                MostrarMensajeInformacionIncorrecta("Favor de verificar la información ingresada.");
            }
        }

        private List<PedidoProducto> RecuperarProductosAgregados()
        {
            List<PedidoProducto> pedidosAgregados = new List<PedidoProducto>();

            PedidoProducto producto0 = RecuperarProducto(cmbProducto0, cmbCantidad0);
            PedidoProducto producto1 = RecuperarProducto(cmbProducto1, cmbCantidad1);
            PedidoProducto producto2 = RecuperarProducto(cmbProducto2, cmbCantidad2);
            PedidoProducto producto3 = RecuperarProducto(cmbProducto3, cmbCantidad3);
            PedidoProducto producto4 = RecuperarProducto(cmbProducto4, cmbCantidad4);

            if (producto0 != null)
            {
                pedidosAgregados.Add(producto0);
            }

            if (producto1 != null)
            {
                pedidosAgregados.Add(producto1);
            }

            if (producto2 != null)
            {
                pedidosAgregados.Add(producto2);
            }

            if (producto3 != null)
            {
                pedidosAgregados.Add(producto3);
            }

            if (producto4 != null)
            {
                pedidosAgregados.Add(producto4);
            }

            return pedidosAgregados;
        }

        private decimal RecuperarCostoTotal()
        {
            string costoString = lblCostoTotalSitio.Content.ToString().Substring(14);
            decimal total = decimal.Parse(costoString);
            return total;
        }

        private DateTime RecuperarFecha()
        {
            string fechaString = lblFecha.Content.ToString();
            DateTime fecha = DateTime.Parse(fechaString);
            return fecha;
        }

        private void RegistrarPedido()
        {
            PedidoClient pedidoClient = new PedidoClient();
            List<PedidoProducto> pedidoProductos = RecuperarProductosAgregados();
            Pedido pedido = new Pedido();

            if (ValidarActualizacionCantidadProducto(pedidoProductos))
            {
                pedido.Fecha = RecuperarFecha();
                pedido.CostoTotal = RecuperarCostoTotal();
                pedido.Estado = "Ordenado";
                pedido.Usuario = SesionSingleton.Instance.Usuario;

                if (chxDomicilio.IsChecked == true)
                {
                    string nombreCliente = txtbCliente.Text;
                    pedido.NombreCliente = nombreCliente;

                    string domicilio = txtbDireccion.Text;
                    pedido.Direccion = domicilio;

                    string telefono = txtbTelefono.Text;
                    pedido.TelefonoCliente = telefono;

                    pedido.TipoEntrega = "Domicilio";
                }
                else
                {
                    pedido.TipoEntrega = "En sitio";
                }

                try
                {
                    string clave = pedidoClient.RegistrarPedido(pedido, pedidoProductos.ToArray());
                    if (clave.Contains("PED-"))
                    {
                        MostrarMensajeRegistroExitoso("Pedido " + clave + " registrado correctamente");
                        MostrarVentanaMenuPedidos();
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

        private bool ValidarActualizacionCantidadProducto(List<PedidoProducto> pedidoProductos)
        {
            string productosNoValidos = string.Empty;
            bool esValido = false;

            foreach (PedidoProducto pedidoProducto in pedidoProductos)
            {
                Producto producto = BuscarProducto(pedidoProducto);

                if (producto.Cantidad >= pedidoProducto.Cantidad)
                {
                    esValido = true;
                }
                else
                {
                    productosNoValidos.Concat(producto.Nombre + ", ");
                }
            }

            if (!string.IsNullOrEmpty(productosNoValidos))
            {
                MostrarMensajeInformacionIncorrecta("No existen cantidades suficientes de los siguientes productos: " + productosNoValidos);
                esValido = false;
            }

            return esValido;
        }

        private Producto BuscarProducto(PedidoProducto pedidoProducto)
        {
            List<Producto> productos = RecuperarProductos();
            Producto productoEncontrado = new Producto();

            foreach (Producto producto in productos)
            {
                if (pedidoProducto.ClaveProducto.Equals(producto.CodigoProducto))
                {
                    productoEncontrado = producto;
                    break;
                }
            }

            return productoEncontrado;
        }

        private List<Producto> RecuperarProductos()
        {
            ProductoClient productoClient = new ProductoClient();
            List<Producto> productos = new List<Producto>();

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

        private void AgregarProductoTodosCombobox(ComboBox combobox, int numCombobox)
        {
            if (combobox.SelectedIndex > -1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i != numCombobox)
                    {
                        ComboBox cmbProducto = ObtenerComboBoxProducto(i);
                        cmbProducto.Items.Add(combobox.SelectedValue.ToString());
                    }
                }
            }
        }

        private void AgregarProductoAComboboxRecorrido(ComboBox cmbProductoSiguiente, int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxProducto(numCombobox - 1);

            if (cmbProductoActual.SelectedIndex > -1)
            {
                AgregarProductoAnteriorEnCombobox(numCombobox - 1, cmbProductoActual.SelectedValue.ToString());
            }

            if (cmbProductoSiguiente.SelectedIndex > -1)
            {
                cmbProductoActual.Items.Add(cmbProductoSiguiente.SelectedValue.ToString());
            }
        }

        private void ActualizarProductoAgregado()
        {
            for (int i = 0; i < 5; i++)
            {
                ComboBox cmbProducto = ObtenerComboBoxProducto(i);

                if (cmbProducto != null && cmbProducto.SelectedIndex > -1)
                {
                    productosAgregados[i] = cmbProducto.SelectedValue.ToString();
                }
                else
                {
                    productosAgregados[i] = string.Empty;
                }
            }

        }

        private void RecorrerProductos(ComboBox cmbProducto)
        {
            int num = ObtenerNumeroComboBox(cmbProducto);
            bool haRecorridoInformacion = false;
            bool esUltimo = false;
            int numUltimoCombobox = 0;


            for (int i = num; i < 5; i++)
            {
                ComboBox cmbSiguiente = ObtenerComboBoxProducto(i + 1);
                ComboBox cmbActual = ObtenerComboBoxProducto(i);

                if (cmbSiguiente != null)
                {
                    if (cmbSiguiente.SelectedIndex > -1)
                    {
                        RecorrerInformacionProducto(i);
                        haRecorridoInformacion = true;
                    }
                    else if (!cmbSiguiente.IsVisible && cmbSiguiente.SelectedIndex == -1)
                    {
                        numUltimoCombobox = i;
                        break;
                    }
                    else if (cmbActual.SelectedIndex > -1)
                    {
                        numUltimoCombobox = i;
                        AgregarProductoTodosCombobox(cmbActual, numUltimoCombobox);
                        ReestablecerComboboxProducto(i);
                    }
                }
                else
                {
                    numUltimoCombobox = i;
                    esUltimo = true;
                }
            }

            if (!haRecorridoInformacion && !esUltimo)
            {
                ComboBox cmbActual = ObtenerComboBoxProducto(numUltimoCombobox);
                AgregarProductoTodosCombobox(cmbActual, numUltimoCombobox);
            }

            OcultarInformacionProducto(numUltimoCombobox);
            ultimaFila--;
            lblProductoError.Visibility = Visibility.Collapsed;
        }

        private void RecorrerInformacionProducto(int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxProducto(numCombobox);
            ComboBox cmbProductoSiguiente = ObtenerComboBoxProducto(numCombobox + 1);
            ComboBox cmbCantidadSiguiente = ObtenerComboBoxCantidad(numCombobox + 1);
            ComboBox cmbCantidadActual = ObtenerComboBoxCantidad(numCombobox);

            if (cmbProductoSiguiente != null && cmbCantidadSiguiente != null)
            {
                AgregarProductoAComboboxRecorrido(cmbProductoSiguiente, numCombobox + 1);

                cmbProductoActual.SelectedValue = cmbProductoSiguiente.SelectedValue;
                cmbCantidadActual.SelectedValue = cmbCantidadSiguiente.SelectedValue;
                cmbProductoActual.Visibility = Visibility.Visible;
                cmbCantidadActual.Visibility = Visibility.Visible;

                if (cmbProductoSiguiente.SelectedIndex > -1)
                {
                    cmbProductoSiguiente.Items.Remove(cmbProductoSiguiente.SelectedValue.ToString());
                }

                Label lblCostoActual = ObtenerLabelCostoProducto(numCombobox);
                lblCostoActual.Visibility = Visibility.Visible;

                Image imgEliminarActual = ObtenerImageEliminar(numCombobox);
                imgEliminarActual.Visibility = Visibility.Visible;
            }
        }

        private void OcultarInformacionProducto(int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxProducto(numCombobox);
            ComboBox cmbCantidadActual = ObtenerComboBoxCantidad(numCombobox);

            cmbProductoActual.Visibility = Visibility.Collapsed;
            cmbProductoActual.SelectedIndex = -1;
            cmbCantidadActual.Visibility = Visibility.Collapsed;
            cmbCantidadActual.SelectedIndex = -1;
            cmbCantidadActual.IsEnabled = false;

            Label lblCostoActual = ObtenerLabelCostoProducto(numCombobox);

            if (lblCostoActual != null)
            {
                lblCostoActual.Visibility = Visibility.Collapsed;
            }

            Image imgEliminarActual = ObtenerImageEliminar(numCombobox);

            if (imgEliminarActual != null)
            {
                imgEliminarActual.Visibility = Visibility.Collapsed;
            }
        }

        private void ReestablecerComboboxProducto(int numCombobox)
        {
            ComboBox cmbProducto = ObtenerComboBoxProducto(numCombobox);
            ComboBox cmbCantidad = ObtenerComboBoxCantidad(numCombobox);
            Label lblCostoActual = ObtenerLabelCostoProducto(numCombobox);

            cmbProducto.SelectedIndex = -1;
            cmbCantidad.SelectedIndex = -1;
            cmbCantidad.IsEnabled = false;
            lblCostoActual.Content = "$0";
        }

        private Image ObtenerImageEliminar(int num)
        {
            string nombreImage = "imgEliminar" + num.ToString();
            Image image = FindName(nombreImage) as Image;

            return image;
        }

        private Label ObtenerLabelCostoProducto(int num)
        {
            string nombreCosto = "lblCosto" + num.ToString();
            Label label = FindName(nombreCosto) as Label;

            return label;
        }

        private ComboBox ObtenerComboBoxProducto(int num)
        {
            string nombreProducto = "cmbProducto" + num.ToString();
            ComboBox comboBox = this.FindName(nombreProducto) as ComboBox;

            return comboBox;
        }

        private ComboBox ObtenerComboBoxCantidad(int num)
        {
            string nombreCantidad = "cmbCantidad" + num.ToString();
            ComboBox comboBox = this.FindName(nombreCantidad) as ComboBox;

            return comboBox;
        }

        private int ObtenerNumeroComboBox(ComboBox cmbProducto)
        {
            string nombreComboBox = cmbProducto.Name;
            string numString = nombreComboBox.Substring(nombreComboBox.Length - 1);
            return int.Parse(numString);
        }

        private void LimpiarCampos()
        {
            RegistroPedido registroPedido = new RegistroPedido();
            NavigationService.Navigate(registroPedido);
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            MostrarVentanaMenuPedidos();
        }

        private void BtnLimpiar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LimpiarCampos();
        }

        private void ImgEliminar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerProductos(cmbProducto1);
            ActualizarProductoAgregado();
        }

        private void ImgEliminar2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerProductos(cmbProducto2);
            ActualizarProductoAgregado();
        }

        private void ImgEliminar3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerProductos(cmbProducto3);
            ActualizarProductoAgregado();
        }

        private void ImgEliminar4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerProductos(cmbProducto4);
            ActualizarProductoAgregado();
        }

        private void CmbProducto_DropDownOpened(object sender, EventArgs e)
        {
            esCmbSeleccionadoPorEmpleado = true;
        }

        private void CmbProducto_DropDownClosed(object sender, EventArgs e)
        {
            esCmbSeleccionadoPorEmpleado = false;
        }
    }
}
