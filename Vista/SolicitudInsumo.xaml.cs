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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Primitives;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para SolicitudInsumo.xaml
    /// </summary>
    /// 

    class InsumoProducto
    {
        public string Clave { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }
    public partial class SolicitudInsumo : Page
    {
        private int ultimaFila = 1;
        private bool esCmbSeleccionadoPorEmpleado;
        private string[] insumosAgregados = new string[5];
        List<InsumoProducto> insumoProductos = new List<InsumoProducto>();

        public SolicitudInsumo()
        {
            InitializeComponent();
            SetFecha();
            SetInsumos();
            SetCantidad();
            SetProveedores();
        }

        private void SetFecha()
        {
            DateTime fechaActual = DateTime.Now;
            lblFecha.Content = fechaActual.ToString();
        }

        private void SetInsumos()
        {
            InsumoClient insumoClient = new InsumoClient();
            List<Insumo> insumos = new List<Insumo>();
            ProductoClient productoClient = new ProductoClient();
            List<Producto> productos = new List<Producto>();

            try
            {
                insumos = insumoClient.RecuperarTodosInsumos().ToList();
                productos = productoClient.RecuperarProductosSinReceta().ToList();

                if (insumos == null || productos == null)
                {
                    MostrarMensajeSinConexionBase();
                }
                else
                {
                    foreach (var insumo in insumos)
                    {
                        InsumoProducto insumoProducto = new InsumoProducto
                        {
                            Clave = insumo.IdInsumo.ToString(),
                            Tipo = "Insumo",
                            Nombre = insumo.Nombre,
                            Precio = (decimal)insumo.PrecioKiloLitro,
                        };

                        cmbInsumo0.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo1.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo2.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo3.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo4.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);

                        insumoProductos.Add(insumoProducto);
                    }

                    foreach (var producto in productos)
                    {
                        InsumoProducto insumoProducto = new InsumoProducto
                        {
                            Clave = producto.CodigoProducto,
                            Tipo = "Producto",
                            Nombre = producto.Nombre,
                            Precio = (decimal)producto.Precio,
                        };

                        cmbInsumo0.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo1.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo2.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo3.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);
                        cmbInsumo4.Items.Add(insumoProducto.Clave + " (" + insumoProducto.Tipo + ") " + insumoProducto.Nombre + ": " + "$" + insumoProducto.Precio);

                        insumoProductos.Add(insumoProducto);
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

        private void CmbInsumo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = cmbInsumo0.Text;
            List<object> items = new List<object>();

            foreach (var item in cmbInsumo0.ItemsSource)
            {
                if (item != null && item.ToString().ToLower().Contains(texto))
                {
                    items.Add(item);
                }
            }

            cmbCantidad0.ItemsSource = items;
            SetCostoTotal();
        }

        private void SetCantidad()
        {
            var cantidades = new List<string>();
            int limiteSuperior = 101;

            for (int i = 1;  i < limiteSuperior; i++)
            {
                cantidades.Add(i.ToString());
            }

            foreach (var cantidad in cantidades)
            {
                cmbCantidad0.Items.Add(cantidad);
                cmbCantidad1.Items.Add(cantidad);
                cmbCantidad2.Items.Add(cantidad);
                cmbCantidad3.Items.Add(cantidad);
                cmbCantidad4.Items.Add(cantidad);
            }
        }

        private void SetProveedores()
        {
            ProveedorClient client = new ProveedorClient();
            List<Proveedor> proveedores = new List<Proveedor>();

            try
            {
                proveedores = client.RecuperarProveedores().ToList();

                if (proveedores == null)
                {
                    MostrarMensajeSinConexionBase();
                }
                else
                {
                    foreach (var proveedor in proveedores)
                    {
                        cmbProveedor.Items.Add(proveedor.IdProveedor + ": " + proveedor.Nombre);
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
                Console.WriteLine(ex.Message);
            }

        }

        private void CmbInsumo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbInsumo0, 0);
            SetCosto(cmbCantidad0, cmbInsumo0, lblCosto0);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbInsumo1, 1);
            SetCosto(cmbCantidad1, cmbInsumo1, lblCosto1);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbInsumo2, 2);
            SetCosto(cmbCantidad2, cmbInsumo2, lblCosto2);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            QuitarProductoSeleccionado(cmbInsumo3, 3);
            SetCosto(cmbCantidad3, cmbInsumo3, lblCosto3);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged4(object sender, SelectionChangedEventArgs e)
        {   
            QuitarProductoSeleccionado(cmbInsumo4, 4);
            SetCosto(cmbCantidad4, cmbInsumo4, lblCosto4);
            SetCostoTotal();
        }

        private void SetCostoTotal()
        {
            decimal total = 0;

            for (int i = 0; i <= ultimaFila; i++)
            {
                Label lblCosto = ObtenerLabelCostoInsumo(i);

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

            lblCostoTotal.Content = "Costo total: $" + total.ToString();
        }

        private void SetCosto(ComboBox cmbCantidad, ComboBox cmbInsumo, Label lblCosto)
        {
            if (cmbInsumo.SelectedItem != null && cmbCantidad.SelectedItem != null)
            {
                string item = cmbInsumo.SelectedItem.ToString();
                int indexPrecio = item.IndexOf('$');

                if (indexPrecio != -1)
                {
                    string precioString = item.Substring(indexPrecio + 1).Trim();
                    decimal precio = decimal.Parse(precioString);

                    if (cmbCantidad.SelectedItem is string cantidadString && decimal.TryParse(cantidadString, out decimal cantidad))
                    {
                        lblCosto.Visibility = Visibility.Visible;
                        decimal costo = precio * cantidad;
                        lblCosto.Content = "$" + costo.ToString();
                    }
                }

                lblInsumoError.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblCosto.Content = "$0";
            }
        }

        private void QuitarProductoSeleccionado(ComboBox comboBox, int numComboBox)
        {
            if (esCmbSeleccionadoPorEmpleado && comboBox.SelectedIndex > -1)
            {
                if (!string.IsNullOrEmpty(insumosAgregados[numComboBox]))
                {
                    string productoAnterior = insumosAgregados[numComboBox];
                    AgregarProductoAnteriorEnCombobox(numComboBox, productoAnterior);
                }

                string productoSeleccionado = comboBox.SelectedValue.ToString();
                insumosAgregados[numComboBox] = productoSeleccionado;
                QuitarProductoSeleccionadoEnCombobox(numComboBox, productoSeleccionado);
            }
        }


        private void AgregarProductoAnteriorEnCombobox(int numComboBox, string productoAnterior)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i != numComboBox)
                {
                    ComboBox cmbProducto = ObtenerComboBoxInsumo(i);
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
                    ComboBox cmbProducto = ObtenerComboBoxInsumo(i);
                    cmbProducto.Items.Remove(productoSeleccionado);
                }
            }
        }

        private void CmbCantidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad0, cmbInsumo0, lblCosto0);
            SetCostoTotal();
        }
        private void CmbCantidad_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad1, cmbInsumo1, lblCosto1);
            SetCostoTotal();
        }
        private void CmbCantidad_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad2, cmbInsumo2, lblCosto2);
            SetCostoTotal();
        }
        private void CmbCantidad_SelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad3, cmbInsumo3, lblCosto3);
            SetCostoTotal();
        }
        private void CmbCantidad_SelectionChanged4(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad4, cmbInsumo4, lblCosto4);
            SetCostoTotal();
        }

        private void BtnAgregarInsumo_Click(object sender, RoutedEventArgs e)
        {
            AgregarInsumo();
        }

        private void AgregarInsumo()
        {
            if (ultimaFila < 5)
            {
                ComboBox cmbProducto = ObtenerComboBoxInsumo(ultimaFila);
                cmbProducto.Visibility = Visibility.Visible;

                ComboBox cmbCantidad = ObtenerComboBoxCantidad(ultimaFila);
                cmbCantidad.Visibility = Visibility.Visible;

                Image imgEliminar = ObtenerImageEliminar(ultimaFila);
                imgEliminar.Visibility = Visibility.Visible;

                Label lblCosto = ObtenerLabelCostoInsumo(ultimaFila);
                lblCosto.Visibility = Visibility.Visible;

                ultimaFila++;
            }
            else
            {
                MessageBox.Show("No se pueden agregar más insumos");
            }
        }

        private bool ValidarInsumos()
        {
            bool esValido = true;

            if (cmbCantidad0.IsVisible && (string.IsNullOrEmpty(cmbCantidad0.Text.Trim()) || cmbInsumo0.SelectedItem == null))
            {
                esValido = false;
            }

            if (cmbCantidad1.IsVisible && (string.IsNullOrEmpty(cmbCantidad1.Text.Trim()) || cmbInsumo1.SelectedItem == null))
            {
                esValido = false;
            }

            if (cmbCantidad2.IsVisible && (string.IsNullOrEmpty(cmbCantidad2.Text.Trim()) || cmbInsumo2.SelectedItem == null))
            {
                esValido = false;
            }

            if (cmbCantidad3.IsVisible && (string.IsNullOrEmpty(cmbCantidad3.Text.Trim()) || cmbInsumo3.SelectedItem == null))
            {
                esValido = false;
            }

            if (cmbCantidad4.IsVisible && (string.IsNullOrEmpty(cmbCantidad4.Text.Trim()) || cmbInsumo4.SelectedItem == null))
            {
                esValido = false;
            }

            return esValido;
        }

        private bool ValidarInformacion()
        {
            bool esValido = true;

            if (!ValidarInsumos() || lblInsumoError.IsVisible)
            {
                esValido = false;
                lblInsumoError.Visibility = Visibility.Visible;
                lblInsumoError.Content = "Verifica los insumos y cantidad seleccionados";
            }

            if (cmbProveedor.SelectedItem == null)
            {
                esValido = false;
                lblProveedorError.Visibility = Visibility.Visible;
                lblProveedorError.Content = "Este campo no puede estar vacío";
            }

            return esValido;
        }

        private void BtnRegistrar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidarInformacion())
            {
                RegistrarInsumo();
            } 
            else
            {
                MostrarMensajeInformacionIncorrecta("Favor de verificar la información ingresada.");
            }
        }

        private SolicitudProducto RecuperarInsumo(ComboBox cmbInsumo, ComboBox cmbCantidad)
        {
            SolicitudProducto solicitudProducto = null;

            if (cmbInsumo.SelectedItem != null && cmbCantidad.SelectedItem != null)
            {
                string insumoItem = cmbInsumo.SelectedItem.ToString();
                string cantidadItem = cmbCantidad.SelectedItem.ToString();

                if (insumoItem.Contains("(Insumo)"))
                {
                    decimal cantidad = decimal.Parse(cantidadItem);
                    string idInsumoString = insumoItem.Substring(0, insumoItem.IndexOf(" "));
                    int idInsumo = int.Parse(idInsumoString);

                    solicitudProducto = new SolicitudProducto
                    {
                        Cantidad = cantidad,
                        IdInsumo = idInsumo,
                };
                }

                if (insumoItem.Contains("(Producto)"))
                {
                    decimal cantidad = decimal.Parse(cantidadItem);
                    string codigoProducto = insumoItem.Substring(0, insumoItem.IndexOf(" "));

                    solicitudProducto = new SolicitudProducto
                    {
                        Cantidad = cantidad,
                        ClaveProducto = codigoProducto,
                    };
                }
            }

            return solicitudProducto;
        }

        private List<SolicitudProducto> RecuperarInsumosAgregados()
        {
            List<SolicitudProducto> solicitudProductos = new List<SolicitudProducto>();

            SolicitudProducto solicitud0 = RecuperarInsumo(cmbInsumo0, cmbCantidad0);
            SolicitudProducto solicitud1 = RecuperarInsumo(cmbInsumo1, cmbCantidad1);
            SolicitudProducto solicitud2 = RecuperarInsumo(cmbInsumo2, cmbCantidad2);
            SolicitudProducto solicitud3 = RecuperarInsumo(cmbInsumo3, cmbCantidad3);
            SolicitudProducto solicitud4 = RecuperarInsumo(cmbInsumo4, cmbCantidad4);

            if (solicitud0 != null)
            {
                solicitudProductos.Add(solicitud0);
            }

            if (solicitud1 != null)
            {
                solicitudProductos.Add(solicitud1);
            }

            if (solicitud2 != null)
            {
                solicitudProductos.Add(solicitud2);
            }

            if (solicitud3 != null)
            {
                solicitudProductos.Add(solicitud3);
            }

            if (solicitud4 != null)
            {
                solicitudProductos.Add(solicitud4);
            }

            return solicitudProductos;
        }

        private decimal RecuperarCostoTotal()
        {
            string costoString = lblCostoTotal.Content.ToString().Substring(14);
            decimal total = -decimal.Parse(costoString);
            return total;
        }

        private DateTime RecuperarFecha()
        {
            string fechaString = lblFecha.Content.ToString();
            DateTime fecha = DateTime.Parse(fechaString);
            return fecha;
        }

        private int RecuperarIdProveedor()
        {
            string proveedor = cmbProveedor.SelectedItem.ToString();
            string idProveedorString = proveedor.Substring(0, proveedor.IndexOf(":"));
            int idProveedor = int.Parse(idProveedorString);

            return idProveedor;
        }

        private void RegistrarInsumo()
        {
            SolicitudClient client = new SolicitudClient();
            List<SolicitudProducto> solicitudProductos = RecuperarInsumosAgregados();

            Solicitud solicitud = new Solicitud
            {
                Fecha = RecuperarFecha(),
                CostoTotal = RecuperarCostoTotal(),
                IdProveedor = RecuperarIdProveedor(),
            };

            try
            {
                string clave = client.RegistrarSolicitud(solicitud, solicitudProductos.ToArray());

                if (clave.Contains("SOL-"))
                {
                    MostrarMensajeRegistroExitoso("Solicitud de insumo " + clave + " registrada correctamente");
                    MostrarVentanaMenuInsumos();
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

        private void AgregarProductoTodosCombobox(ComboBox combobox, int numCombobox)
        {
            if (combobox.SelectedIndex > -1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i != numCombobox)
                    {
                        ComboBox cmbInsumo = ObtenerComboBoxInsumo(i);
                        cmbInsumo.Items.Add(combobox.SelectedValue.ToString());
                    }
                }
            }
        }

        private void AgregarProductoAComboboxRecorrido(ComboBox cmbProductoSiguiente, int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxInsumo(numCombobox - 1);

            if (cmbProductoActual.SelectedIndex > -1)
            {
                AgregarProductoAnteriorEnCombobox(numCombobox - 1, cmbProductoActual.SelectedValue.ToString());
            }

            if (cmbProductoSiguiente.SelectedIndex > -1)
            {
                cmbProductoActual.Items.Add(cmbProductoSiguiente.SelectedValue.ToString());
            }
        }

        private void ActualizarInsumoAgregado()
        {
            for (int i = 0; i < 5; i++)
            {
                ComboBox cmbInsumo = ObtenerComboBoxInsumo(i);

                if (cmbInsumo != null && cmbInsumo.SelectedIndex > -1)
                {
                    insumosAgregados[i] = cmbInsumo.SelectedValue.ToString();
                }
                else
                {
                    insumosAgregados[i] = string.Empty;
                }
            }

        }

        private void RecorrerInsumos(ComboBox cmbProducto)
        {
            int num = ObtenerNumeroComboBox(cmbProducto);
            bool haRecorridoInformacion = false;
            bool esUltimo = false;
            int numUltimoCombobox = 0;


            for (int i = num; i < 5; i++)
            {
                ComboBox cmbSiguiente = ObtenerComboBoxInsumo(i + 1);
                ComboBox cmbActual = ObtenerComboBoxInsumo(i);

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
                ComboBox cmbActual = ObtenerComboBoxInsumo(numUltimoCombobox);
                AgregarProductoTodosCombobox(cmbActual, numUltimoCombobox);
            }

            OcultarInformacionProducto(numUltimoCombobox);
            ultimaFila--;
            lblInsumoError.Visibility = Visibility.Collapsed;
        }

        private void RecorrerInformacionProducto(int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxInsumo(numCombobox);
            ComboBox cmbProductoSiguiente = ObtenerComboBoxInsumo(numCombobox + 1);
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

                Label lblCostoActual = ObtenerLabelCostoInsumo(numCombobox);
                lblCostoActual.Visibility = Visibility.Visible;

                Image imgEliminarActual = ObtenerImageEliminar(numCombobox);
                imgEliminarActual.Visibility = Visibility.Visible;
            }
        }

        private void OcultarInformacionProducto(int numCombobox)
        {
            ComboBox cmbProductoActual = ObtenerComboBoxInsumo(numCombobox);
            ComboBox cmbCantidadActual = ObtenerComboBoxCantidad(numCombobox);

            cmbProductoActual.Visibility = Visibility.Collapsed;
            cmbProductoActual.SelectedIndex = -1;
            cmbCantidadActual.Visibility = Visibility.Collapsed;
            cmbCantidadActual.SelectedIndex = -1;

            Label lblCostoActual = ObtenerLabelCostoInsumo(numCombobox);

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
            ComboBox cmbInsumo = ObtenerComboBoxInsumo(numCombobox);
            ComboBox cmbCantidad = ObtenerComboBoxCantidad(numCombobox);
            Label lblCostoActual = ObtenerLabelCostoInsumo(numCombobox);

            cmbInsumo.SelectedIndex = -1;
            cmbCantidad.SelectedIndex = -1;
            lblCostoActual.Content = "$0";
        }

        private Image ObtenerImageEliminar(int num)
        {
            string nombreImage = "imgEliminar" + num.ToString();
            Image image = FindName(nombreImage) as Image;

            return image;
        }

        private Label ObtenerLabelCostoInsumo(int num)
        {
            string nombreCosto = "lblCosto" + num.ToString();
            Label label = FindName(nombreCosto) as Label;

            return label;
        }

        private ComboBox ObtenerComboBoxInsumo(int num)
        {
            string nombreProducto = "cmbInsumo" + num.ToString();
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
            SolicitudInsumo solicitudInsumo = new SolicitudInsumo();
            NavigationService.Navigate(solicitudInsumo);
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

        private void MostrarVentanaMenuInsumos()
        {
            MenuInsumos menuInsumos = new MenuInsumos();
            NavigationService.Navigate(menuInsumos);
        }

        private void MostrarMensajeInformacionIncorrecta(string mensaje)
        {
            InformacionIncorrecta informacionIncorrecta = new InformacionIncorrecta();
            informacionIncorrecta.SetContenidoMensaje(mensaje);
            informacionIncorrecta.Show();
        }

        private void CmbProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblProveedorError.Visibility = Visibility.Collapsed;
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            MostrarVentanaMenuInsumos();
        }

        private void BtnLimpiar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LimpiarCampos();
        }

        private void ImgEliminar1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerInsumos(cmbInsumo1);
            ActualizarInsumoAgregado();
        }

        private void ImgEliminar2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerInsumos(cmbInsumo2);
            ActualizarInsumoAgregado();
        }

        private void ImgEliminar3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerInsumos(cmbInsumo3);
            ActualizarInsumoAgregado();
        }

        private void ImgEliminar4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RecorrerInsumos(cmbInsumo4);
            ActualizarInsumoAgregado();
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
