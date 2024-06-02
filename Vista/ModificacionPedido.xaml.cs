
using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private int id;
        private List<Producto> productos = new List<Producto>();
        private string[] productosAgregados = new string[5];
        private bool esCmbSeleccionadoPorEmpleado;
        private string[] estados =
            {
                "Ordenado",
                "En proceso",
                "Preparado",
                "Entregado",
                "Cancelado"
            };
        private int ultimaFila;
        private List<PedidoProducto> productosRecuperados = new List<PedidoProducto>();

        public ModificacionPedido(String clave)
        {
            InitializeComponent();
            this.clave = clave;
            SetPedido();
            SetUltimaFila();
        }

        private void SetUltimaFila()
        {
            ultimaFila = productosRecuperados.Count;
        }

        private void SetPedido()
        {
            Pedido pedido = RecuperarPedido();
            productosRecuperados = RecuperarProductos(pedido.IdPedido);

            SetProductos(productosRecuperados);

            lblClave.Content = clave;
            id = pedido.IdPedido;
            lblTipoEntrega.Content = "Tipo de entrega: " + pedido.TipoEntrega;
            lblTotal.Content = "Costo total: $" + pedido.CostoTotal.ToString();
            lblFecha.Content = "Fecha: " + pedido.Fecha.ToString();
            lblEmpleado.Text = "Empleado: " + RecuperarEmpleado(pedido.Usuario);

            if (pedido.TipoEntrega.Equals("Domicilio"))
            {
                lblCliente.Content = "Cliente: " + pedido.NombreCliente;
                lblCliente.Visibility = Visibility.Visible;

                lblTelefono.Content = "Teléfono: " + pedido.TelefonoCliente;
                lblTelefono.Visibility = Visibility.Visible;

                lblDireccion.Text = "Dirección: " + pedido.Direccion;
                lblDireccion.Visibility = Visibility.Visible;
            }

            SetEstado(pedido.Estado);
            VerificarEstadoPedido(pedido.Estado);

            int totalProductos = productosRecuperados.Count;

            for (int i = 0; i < totalProductos; i++)
            {
                ComboBox cmbProducto = ObtenerComboBoxProducto(i);
                ComboBox cmbCantidad = ObtenerComboBoxCantidad(i);
                Image imgEliminar = ObtenerImageEliminar(i);
                Label lblCosto = ObtenerLabelCostoProducto(i);

                string nombreComboBoxCantidad = "cmbCantidad" + i;
                ComboBox cmbCantidad = FindName(nombreComboBoxCantidad) as ComboBox;

                string nombreEliminar = "imgEliminar" + i;
                Image imgEliminar = FindName(nombreEliminar) as Image;

                string nombreCosto = "lblCosto" + i;
                Label lblCosto = FindName(nombreCosto) as Label;

                if (cmbProducto != null)
                {
                    PedidoProducto productoActual = productosRecuperados[i];
                    cmbProducto.Visibility = Visibility.Visible;
                    cmbCantidad.Visibility = Visibility.Visible;

                    if (productoActual != null)
                    {
                        int posicionProducto = BuscarProductoEnCombobox(cmbProducto, productoActual.ClaveProducto);
                        cmbProducto.SelectedIndex = posicionProducto;
                        QuitarProductoSeleccionado(cmbProducto, i);
                        int posicionCantidad = BuscarCantidad(cmbCantidad, (int)productoActual.Cantidad);
                        cmbCantidad.SelectedIndex = posicionCantidad;
                    }
                }

                if (i > 0)
                {
                    imgEliminar.Visibility = Visibility.Visible;
                    lblCosto.Visibility = Visibility.Visible;
                }

                if (i > 0)
                {
                    imgEliminar.Visibility = Visibility.Visible;
                    lblCosto.Visibility = Visibility.Visible;
                }
            }
        }

        private int BuscarProductoEnCombobox(ComboBox comboBox, string claveProducto)
        {
            int index = -1;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var producto = comboBox.Items[i];

                if (producto != null && producto.ToString().Contains(claveProducto))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private Producto RecuperarProductoPorClave(string clave)
        {
            Producto producto = new Producto();
            ProductoClient productoClient = new ProductoClient();

            try
            {
                producto = productoClient.RecuperarProducto(clave);

                if (producto == null)
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

            return producto;
        }

        private int BuscarCantidad(ComboBox comboBox, int cantidadProducto)
        {
            int index = -1;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                var cantidad = comboBox.Items[i];

                if (cantidad.ToString().Equals(cantidadProducto.ToString()))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private string RecuperarEmpleado(string usuario)
        {
            EmpleadoClient empleadoClient = new EmpleadoClient();
            Empleado empleado = new Empleado();

            try
            {
                empleado = empleadoClient.BuscarEmpleado(usuario);

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

        private void VerificarEstadoPedido(string estado)
        {
            if (estado == "Cancelado" || estado == "Entregado")
            {
                btnModificar.IsEnabled = false;
                btnAgregarProducto.IsEnabled = false;
                cmbEstado.IsEnabled = false;

                cmbCantidad0.IsEnabled = false;
                cmbCantidad1.IsEnabled = false;
                cmbCantidad2.IsEnabled = false;
                cmbCantidad3.IsEnabled = false;
                cmbCantidad4.IsEnabled = false;

                cmbProducto0.IsEnabled = false;
                cmbProducto1.IsEnabled = false;
                cmbProducto2.IsEnabled = false;
                cmbProducto3.IsEnabled = false;
                cmbProducto4.IsEnabled = false;

                imgEliminar1.IsEnabled = false;
                imgEliminar2.IsEnabled = false;
                imgEliminar3.IsEnabled = false;
                imgEliminar4.IsEnabled = false;
                imgEliminar1.Visibility = Visibility.Collapsed;
                imgEliminar2.Visibility = Visibility.Collapsed;
                imgEliminar3.Visibility = Visibility.Collapsed;
                imgEliminar4.Visibility = Visibility.Collapsed;
            }
            else if (estado == "En proceso" || estado == "Preparado")
            {
                btnAgregarProducto.IsEnabled = false;

                cmbCantidad0.IsEnabled = false;
                cmbCantidad1.IsEnabled = false;
                cmbCantidad2.IsEnabled = false;
                cmbCantidad3.IsEnabled = false;
                cmbCantidad4.IsEnabled = false;

                cmbProducto0.IsEnabled = false;
                cmbProducto1.IsEnabled = false;
                cmbProducto2.IsEnabled = false;
                cmbProducto3.IsEnabled = false;
                cmbProducto4.IsEnabled = false;

                imgEliminar1.IsEnabled = false;
                imgEliminar2.IsEnabled = false;
                imgEliminar3.IsEnabled = false;
                imgEliminar4.IsEnabled = false;
                imgEliminar1.Visibility = Visibility.Collapsed;
                imgEliminar2.Visibility = Visibility.Collapsed;
                imgEliminar3.Visibility = Visibility.Collapsed;
                imgEliminar4.Visibility = Visibility.Collapsed;
            }
        }

        private void SetEstado(string estado)
        {
            cmbEstado.ItemsSource = estados;

            for (int i = 0; i < cmbEstado.Items.Count; i++)
            {
                var item = cmbEstado.Items[i];

                if (item.ToString() == estado)
                {
                    cmbEstado.SelectedIndex = i;
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

        private void AgregarProductosRecuperados(List<PedidoProducto> pedidoProductos)
        {
            foreach (PedidoProducto pedidoProducto in pedidoProductos)
            {
                if (!EstaEnListaProductos(pedidoProducto))
                {
                    Producto producto = RecuperarProductoPorClave(pedidoProducto.ClaveProducto);
                    producto.Cantidad = pedidoProducto.Cantidad;
                    productos.Add(producto);
                }
            }
        }

        private bool EstaEnListaProductos(PedidoProducto pedidoProducto)
        {
            bool seEncuentra = false;

            foreach (var producto in productos)
            {
                if (producto.CodigoProducto.Equals(pedidoProducto.ClaveProducto))
                {
                    seEncuentra = true;
                }
            }

            return seEncuentra;
        }

        private void SetProductos(List<PedidoProducto> pedidoProductos)
        {
            ProductoClient productoClient = new ProductoClient();

            try
            {
                productos = productoClient.RecuperarProductosParaPedido().ToList();
                AgregarProductosRecuperados(pedidoProductos);

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

            lblTotal.Content = "Costo total: $" + total.ToString();
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
            if (comboBox.SelectedIndex > -1)
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
            if (esCmbSeleccionadoPorEmpleado)
            {
                QuitarProductoSeleccionado(cmbProducto0, 0);
            }

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
            if (esCmbSeleccionadoPorEmpleado)
            {
                QuitarProductoSeleccionado(cmbProducto1, 1);
            }

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
            if (esCmbSeleccionadoPorEmpleado)
            {
                QuitarProductoSeleccionado(cmbProducto2, 2);
            }

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
            if (esCmbSeleccionadoPorEmpleado)
            {
                QuitarProductoSeleccionado(cmbProducto3, 3);
            }

            SetComboBoxCantidad(cmbProducto3, cmbCantidad3);
            SetCosto(cmbCantidad3, cmbProducto3, lblCosto3);
            SetCostoTotal();
        }

        private void CmbProducto4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (esCmbSeleccionadoPorEmpleado)
            {
                QuitarProductoSeleccionado(cmbProducto4, 4);
            }

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

                string nombreEliminar = "imgEliminar" + ultimaFila.ToString();
                Image image = this.FindName(nombreEliminar) as Image;

                if (image != null)
                {
                    image.Visibility = Visibility.Visible;
                }

                string nombreCosto = "lblCosto" + ultimaFila.ToString();
                Label label = FindName(nombreCosto) as Label;

                if (label != null)
                {
                    label.Visibility = Visibility.Visible;
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
                MostrarMensajeCamposVacios();
            }

            return esValido;
        }

        private void CancelarPedido()
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("¿Seguro que desea cancelar el pedido? \nNo se podrá revertir el cambio.", "Cancelar pedido", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                PedidoClient pedidoClient = new PedidoClient();
                int codigo = 0;

                try
                {
                    codigo = pedidoClient.CancelarPedido(clave);
                    if (codigo > 0)
                    {
                        MostrarMensajeModificacionExitosa();
                        MostrarVentanaVistaPedido();
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

        private void ModificarPedido()
        {
            PedidoClient pedidoClient = new PedidoClient();
            List<PedidoProducto> productosAgregados = RecuperarProductosAgregados();
            Pedido pedido = new Pedido();

            if (ValidarActualizacionCantidadProducto(productosAgregados))
            {
                pedido.Estado = cmbEstado.SelectedItem.ToString();

                string costoString = lblTotal.Content.ToString().Substring(14);
                decimal costoTotal = decimal.Parse(costoString);
                pedido.CostoTotal = costoTotal;
                pedido.IdPedido = id;

                try
                {
                    int codigo = pedidoClient.ModificarPedido(pedido, productosAgregados.ToArray());
                    if (codigo > 0)
                    {
                        MostrarMensajeModificacionExitosa();
                        MostrarVentanaVistaPedido();
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

        private void CancelarPedido()
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("¿Seguro que desea cancelar el pedido? \nNo se podrá revertir el cambio.", "Cancelar pedido", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                PedidoClient pedidoClient = new PedidoClient();
                int codigo = 0;

                try
                {
                    codigo = pedidoClient.CancelarPedido(clave);
                    if (codigo > 0)
                    {
                        MostrarMensajeModificacionExitosa();
                        MostrarMenuPedidos();
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

        private void MostrarVentanaVistaPedido()
        {
            PedidoVista pedidoVista = new PedidoVista(clave);
            NavigationService.Navigate(pedidoVista);
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

        private void MostrarMensajeCamposVacios()
        {
            CamposVacios camposVacios = new CamposVacios();
            camposVacios.Show();
        }

        private void MostrarMensajeModificacionExitosa()
        {
            ModificacionExitosa modificacionExitosa = new ModificacionExitosa();
            modificacionExitosa.Show();
        }

        private void MostrarMensajeInformacionIncorrecta(string mensaje)
        {
            InformacionIncorrecta informacionIncorrecta = new InformacionIncorrecta();
            informacionIncorrecta.SetContenidoMensaje(mensaje);
            informacionIncorrecta.Show();
        }

        private void BtnAtras_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("¿Seguro que desea cancelar la modificación?", "Cancelar modificación", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                MostrarVentanaVistaPedido();
            }
        }

        private void BtnModificar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidarInformacion())
            {
                System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("¿Seguro que desea modificar el pedido?", "Confirmar modificación", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    if (cmbEstado.SelectedItem.Equals(estados[4]))
                    {
                        CancelarPedido();
                    }
                    else
                    {
                        ModificarPedido();
                    }
                }
            }

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
            ModificacionPedido modificacionPedido = new ModificacionPedido(clave);
            NavigationService.Navigate(modificacionPedido);
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

        private void BtnLimpiar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LimpiarCampos();
        }
    }
}
