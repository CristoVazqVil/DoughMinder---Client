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

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para SolicitudInsumo.xaml
    /// </summary>
    public partial class SolicitudInsumo : Page
    {
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
            SetComboBoxInsumos(cmbInsumo);
        }

        private void SetComboBoxInsumos(ComboBox cmbInsumo)
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
                        items.Add(insumo.Nombre + ": " + insumo.CantidadKiloLitro + " KG/L (actualmente) - $" + insumo.PrecioKiloLitro);
                    }

                    foreach (var producto in productos)
                    {
                        items.Add(producto.Nombre + ": " + producto.Cantidad + " KG/L/Piezas (actualmente) - $" + producto.Precio);
                    }

                    cmbInsumo.ItemsSource = items;
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

        private void CmbInsumo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = cmbInsumo.Text;
            List<object> items = new List<object>();

            foreach (var item in cmbInsumo.ItemsSource)
            {
                if (item is Insumo && ((Insumo)item).Nombre.ToLower().Contains(texto))
                {
                    items.Add((Insumo)item);
                }
                else if (item is Producto && ((Producto)item).Nombre.ToLower().Contains(texto))
                {
                    items.Add((Producto)item);
                }
            }

            cmbCantidad.ItemsSource = items;
        }

        private void SetCantidad()
        {
            SetComboBoxCantidad(cmbCantidad);
        }

        private void SetComboBoxCantidad(ComboBox cmbCantidad)
        {
            var cantidades = new List<string>
            {
                "0.5",
                "1",
                "5",
                "10",
                "50",
                "100"
            };

            foreach (var cantidad in cantidades)
            {
                cmbCantidad.Items.Add(cantidad);
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
                        cmbProveedor.Items.Add(proveedor.Nombre);
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

        private void CmbCantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string regexCantidad = @"^\d{0,5}$";
            Regex regex = new Regex(regexCantidad);

            if (!regex.IsMatch(cmbCantidad.Text + e.Text))
            {
                e.Handled = true;
            }
        }

        private void CmbInsumo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCantidad.SelectedItem != null)
            {
                if (cmbInsumo.SelectedItem != null)
                {
                    string item = cmbInsumo.SelectedItem.ToString();
                    int indexPrecio = item.IndexOf('$');

                    if (indexPrecio != -1)
                    {
                        string precioString = item.Substring(indexPrecio + 1);
                        decimal precio = decimal.Parse(precioString);

                        string cantidadString = cmbCantidad.Text;
                        decimal cantidad = decimal.Parse(cantidadString);

                        decimal costo = precio * cantidad;
                        lblCostoTotal.Content = costo.ToString();
                    }
                }
            }
        }

        private void SetCostoTotal(ComboBox cmbCantidad, ComboBox cmbInsumo)
        {

        }
    }
}
