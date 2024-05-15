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
    public partial class SolicitudInsumo : Page
    {
        private int ultimaFila = 1;
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
            DoughMinderServicio.InsumoClient insumoClient = new DoughMinderServicio.InsumoClient();
            List<DoughMinderServicio.Insumo> insumos = new List<Insumo>();
            DoughMinderServicio.ProductoClient productoClient = new DoughMinderServicio.ProductoClient();
            List<DoughMinderServicio.Producto> productos = new List<Producto>();

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
                    var items = new List<object>();

                    foreach (var insumo in insumos)
                    {
                        items.Add(insumo.IdInsumo + " (Insumo) " + insumo.Nombre + ": " + "$" + insumo.PrecioKiloLitro);
                    }

                    foreach (var producto in productos)
                    {
                        items.Add(producto.CodigoProducto + " (Producto) " + producto.Nombre + ": " + "$" + producto.Precio);
                    }

                    cmbInsumo0.ItemsSource = items;
                    cmbInsumo1.ItemsSource = items;
                    cmbInsumo2.ItemsSource = items;
                    cmbInsumo3.ItemsSource = items;
                    cmbInsumo4.ItemsSource = items;
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
            var cantidades = new List<string>
            {
                "0.5",
                "1",
                "5",
                "10",
                "20",
                "30",
                "40",
                "50",
                "100"
            };

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
            DoughMinderServicio.ProveedorClient client = new DoughMinderServicio.ProveedorClient();
            List<DoughMinderServicio.Proveedor> proveedores = new List<Proveedor>();

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
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
                Console.WriteLine(ex.Message);
            }

        }

        private void CmbInsumo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad0, cmbInsumo0, lblCosto0);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad1, cmbInsumo1, lblCosto1);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad2, cmbInsumo2, lblCosto2);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged3(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad3, cmbInsumo3, lblCosto3);
            SetCostoTotal();
        }
        private void CmbInsumo_SelectionChanged4(object sender, SelectionChangedEventArgs e)
        {
            SetCosto(cmbCantidad4, cmbInsumo4, lblCosto4);
            SetCostoTotal();
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
                lblInsumoError.Visibility = Visibility.Visible;
                lblInsumoError.Content = "Recuerda verificar que ingresas los insumos correctos";
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
            if (ultimaFila < 5)
            {
                string nombreInsumo = "cmbInsumo" + ultimaFila.ToString();
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

        private void RegistrarInsumo()
        {
            SolicitudClient client = new SolicitudClient();
            List<SolicitudProducto> solicitudProductos = new List<SolicitudProducto>();
            int codigo = 0;

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

            if (solicitud3  != null)
            {
                solicitudProductos.Add(solicitud3);
            }

            if (solicitud4 != null)
            {
                solicitudProductos.Add(solicitud4);
            }


            string proveedor = cmbProveedor.SelectedItem.ToString();
            string idProveedorString = proveedor.Substring(0, proveedor.IndexOf(":"));
            int idProveedor = int.Parse(idProveedorString);

            string fechaString = lblFecha.Content.ToString();
            DateTime fecha = DateTime.Parse(fechaString);

            string costoString = "-" + lblCostoTotal.Content.ToString().Substring(14);
            decimal costoTotal = decimal.Parse(costoString);

            DoughMinderServicio.Solicitud solicitud = new Solicitud
            {
                Fecha = fecha,
                CostoTotal = costoTotal,
                IdProveedor = idProveedor,
            };

            try
            {
                codigo = client.RegistrarSolicitud(solicitud, solicitudProductos.ToArray());

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
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
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

        private void CmbProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblProveedorError.Visibility = Visibility.Collapsed;
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
