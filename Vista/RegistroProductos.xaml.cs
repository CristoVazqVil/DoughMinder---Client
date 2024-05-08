using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using Microsoft.Win32;
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
using static DoughMinder___Client.Vista.RegistroRecetas;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for RegistroProductos.xaml
    /// </summary>
    public partial class RegistroProductos : Page
    {
        private string rutaImagenSeleccionada;

        public RegistroProductos()
        {
            InitializeComponent();
            RecuperarRecetas();
        }

        public class RecetaItem
        {
            public int IdReceta { get; set; }
            public string Nombre { get; set; }
        }

        private void Registrar(object sender, MouseButtonEventArgs e)
        {
            ReiniciarMarcadores();

            if (!ValidarCamposVacios())
            {
                CamposVacios camposVacios = new CamposVacios();
                camposVacios.ShowDialog();
            }
            else
            {
                try
                {
                    DoughMinderServicio.ProductoClient cliente = new DoughMinderServicio.ProductoClient();
                    DoughMinderServicio.Producto producto = new DoughMinderServicio.Producto();

                    decimal precio;
                    decimal.TryParse(txbPrecioProducto.Text, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out precio);

                    producto.CodigoProducto = txbCodigoProducto.Text;
                    producto.Nombre = txbNombreProducto.Text;
                    producto.Precio = precio;
                    producto.Descripcion = txbDescripcionProducto.Text;
                    producto.Cantidad = int.Parse(txbCantidadProducto.Text);
                    producto.Restricciones = txbRestriccionesProducto.Text;
                    producto.RutaFoto = txbImagenProducto.Text;
                    producto.Estado = true;


                    if (ckbSinReceta.IsChecked == false)
                    {
                        RecetaItem recetaSeleccionada = dtgTablaRecetas.SelectedItem as RecetaItem;
                        if (recetaSeleccionada != null)
                        {
                            producto.IdReceta = recetaSeleccionada.IdReceta;
                        }
                    }

                    int codigo = cliente.GuardarProducto(producto);

                    if (codigo == 1)
                    {
                        MostrarMensajeRegistroExitoso();
                        NavigationService.GoBack();
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensajeProductoExistente();
                        }
                        else
                        {
                            MostrarMensajeSinConexionBase();
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
        }

        private async Task RecuperarRecetas()
        {
            await Task.Delay(200);
            Dictionary<int, string> listaRecetas = new Dictionary<int, string>();
            try
            {
                DoughMinderServicio.RecetaClient cliente = new DoughMinderServicio.RecetaClient();
                listaRecetas = cliente.RecuperarRecetas();

                if (listaRecetas == null)
                {
                    MostrarMensajeSinConexionBase();
                }
                else
                {
                    MostrarListaRecetas(listaRecetas);
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

        private void MostrarListaRecetas(Dictionary<int, string> listaRecetas)
        {
            List<RecetaItem> datosTabla = new List<RecetaItem>();

            foreach (var kvp in listaRecetas)
            {
                RecetaItem recetaItem = new RecetaItem
                {
                    IdReceta = kvp.Key,
                    Nombre = kvp.Value,
                };

                datosTabla.Add(recetaItem);
            }

            dtgTablaRecetas.ItemsSource = datosTabla;
        }

        private bool ValidarCamposVacios()
        {
            bool camposValidos = true;

            if (string.IsNullOrEmpty(txbImagenProducto.Text) || txbImagenProducto.Text.Equals("Sin imagen adjunta..."))
            {
                lblImagen.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbNombreProducto.Text))
            {
                lblNombre.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbCantidadProducto.Text))
            {
                lblCantidad.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbCodigoProducto.Text))
            {
                lblCodigo.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbRestriccionesProducto.Text))
            {
                lblRestricciones.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbPrecioProducto.Text))
            {
                lblPrecio.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbDescripcionProducto.Text))
            {
                lblDescripcion.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (ckbSinReceta.IsChecked == false)
            {
                bool alMenosUnInsumoSeleccionado = dtgTablaRecetas.SelectedItems.Count > 0;

                if (!alMenosUnInsumoSeleccionado)
                {
                    camposValidos = false;
                    lblRecetas.Foreground = Brushes.Red;
                }
            }

            return camposValidos;
        }

        private void ReiniciarMarcadores()
        {
            lblDescripcion.Foreground = Brushes.Black;
            lblPrecio.Foreground = Brushes.Black;
            lblRestricciones.Foreground = Brushes.Black;
            lblCodigo.Foreground = Brushes.Black;
            lblCantidad.Foreground = Brushes.Black;
            lblNombre.Foreground = Brushes.Black;
            lblImagen.Foreground = Brushes.Black;
            lblRecetas.Foreground = Brushes.Black;
        }

        private void AdjuntarImagen(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de imagen|*.png;*.jpeg|Todos los archivos|*.*";
            openFileDialog.Title = "Selecciona una imagen";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                rutaImagenSeleccionada = openFileDialog.FileName;

                string nombreImagen = System.IO.Path.GetFileName(rutaImagenSeleccionada);

                txbImagenProducto.Text = nombreImagen;
            }
        }

        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.ShowDialog();
        }

        private void MostrarMensajeRegistroExitoso()
        {
            RegistroExitoso registroExitoso = new RegistroExitoso();
            registroExitoso.ShowDialog();
        }

        private void MostrarMensajeProductoExistente()
        {
            ProductoExistente productoExistente = new ProductoExistente();
            productoExistente.ShowDialog();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.ShowDialog();
        }

        private void LimpiarCampos(object sender, MouseButtonEventArgs e)
        {
            ReiniciarMarcadores();
            txbNombreProducto.Clear();
            txbCodigoProducto.Clear();
            txbPrecioProducto.Clear();
            txbDescripcionProducto.Clear();
            txbRestriccionesProducto.Clear();
            txbCantidadProducto.Clear();
            txbImagenProducto.Text = "Sin imagen adjunta...";
            dtgTablaRecetas.ItemsSource = null;
            dtgTablaRecetas.Items.Clear();
            RecuperarRecetas();
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        //Validaciones de entradas y cambios gráficos

        private void EliminarCaracteresNombre(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Regex regex = new Regex(@"^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ., ]*$");

            if (textBox != null && textBox.Text.Length < 90)
            {
                if (regex.IsMatch(e.Text))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EliminarCaracteresCodigo(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Regex regex = new Regex(@"^[a-zA-Z0-9]*$");

            if (textBox != null && textBox.Text.Length < 10)
            {
                if (regex.IsMatch(e.Text))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EliminarCaracteresPrecio(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Length >= 10)
            {
                e.Handled = true;
                return;
            }

            Regex regex = new Regex(@"^\d+(\.\d{0,2})?$");
            e.Handled = !regex.IsMatch(textBox.Text + e.Text);
        }

        private void EliminarCaracteresDescripcion(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Regex regex = new Regex(@"^[a-zA-Z0-9.,áéíóúñÁÉÍÓÚÑüÜ\- ]*$");

            if (textBox != null && textBox.Text.Length < 1500 && regex.IsMatch(e.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EliminarCaracteresRestricciones(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Regex regex = new Regex(@"^[a-zA-Z0-9.,áéíóúñÁÉÍÓÚÑüÜ\- ]*$");

            if (textBox != null && textBox.Text.Length < 1500 && regex.IsMatch(e.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EliminarCaracteresCantidad(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && textBox.Text.Length < 7)
            {
                if (!char.IsDigit(e.Text, 0))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void CambiarRegistrarAzul(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarRegistrarVerde(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void CambiarAdjuntarAzul(object sender, MouseEventArgs e)
        {
            btnAdjuntarImagen.Source = new BitmapImage(new Uri("/Recursos/IconoMasAzul.png", UriKind.Relative));
        }

        private void CambiarAdjuntarVerde(object sender, MouseEventArgs e)
        {
            btnAdjuntarImagen.Source = new BitmapImage(new Uri("/Recursos/IconoMas.png", UriKind.Relative));
        }

        private void CambiarLimpiarAzul(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarLimpiarVerde(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }
    }
}
