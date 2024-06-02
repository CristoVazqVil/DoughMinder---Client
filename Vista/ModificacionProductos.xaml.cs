﻿using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ModificacionProductos.xaml
    /// </summary>
    public partial class ModificacionProductos : Page
    {
        private byte[] imagenProductoBytes;
        private String codigoProducto;
        private DoughMinderServicio.Producto producto;

        public ModificacionProductos(String codigo)
        {
            InitializeComponent();
            codigoProducto = codigo;
            _ = RecuperarRecetas();
            RecuperarProducto();
            ColocarProductoEnCampos();
        }

        private void Modificar(object sender, MouseButtonEventArgs e)
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

                    producto.Nombre = txbNombreProducto.Text;
                    producto.Precio = precio;
                    producto.Descripcion = txbDescripcionProducto.Text;
                    producto.Cantidad = int.Parse(txbCantidadProducto.Text);
                    producto.Restricciones = txbRestriccionesProducto.Text;
                    producto.Foto = imagenProductoBytes;
                    producto.Estado = true;

                    if (ckbSinReceta.IsChecked == false)
                    {
                        RecetaItem recetaSeleccionada = dtgTablaRecetas.SelectedItem as RecetaItem;
                        if (recetaSeleccionada != null)
                        {
                            producto.IdReceta = recetaSeleccionada.IdReceta;
                        }
                    }

                    int codigo = cliente.ModificarProducto(producto,codigoProducto);

                    if (codigo > 0)
                    {
                        MostrarMensajeModificacionExitosa();
                        MenuProductos productos = new MenuProductos();
                        NavigationService.Navigate(productos);
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensajeProductoExistente();
                        }
                        else
                        {
                            if (codigo == -5)
                            {
                                MostrarMensajeProductoUtilizado();
                            }
                            else
                            {
                                MostrarMensajeSinConexionBase();
                            }
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
        }

        private void Deshabilitar(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.DialogResult resultado = System.Windows.Forms.MessageBox.Show("¿Estás seguro de deshabilitar este producto?", "Confirmación deshabilitar", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (resultado == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    DoughMinderServicio.ProductoClient cliente = new DoughMinderServicio.ProductoClient();

                    int codigo = cliente.DeshabilitarProducto(codigoProducto);

                    if (codigo > 0)
                    {
                        MostrarMensajeDeshabilitacionExitosa();
                        MenuProductos productos = new MenuProductos();
                        NavigationService.Navigate(productos);
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensajeProductoExistente();
                        }
                        else
                        {
                            if (codigo == -5)
                            {
                                MostrarMensajeProductoUtilizado();
                            }
                            else
                            {
                                MostrarMensajeSinConexionBase();
                            }
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
        }

        private void RecuperarProducto()
        {
            if (codigoProducto != null)
            {
                try
                {
                    DoughMinderServicio.ProductoClient cliente = new DoughMinderServicio.ProductoClient();
                    producto = cliente.RecuperarProducto(codigoProducto);
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
            else
            {
                MostrarMensajeSinConexionBase();
            }
        }

        private void ColocarProductoEnCampos()
        {
            if (producto != null)
            {
                lblCodigo.Content = producto.CodigoProducto;
                txbNombreProducto.Text = producto.Nombre;
                txbDescripcionProducto.Text = producto.Descripcion;
                txbPrecioProducto.Text = producto.Precio.ToString();
                txbRestriccionesProducto.Text = producto.Restricciones;
                txbCantidadProducto.Text = producto.Cantidad.ToString();
                txbImagenProducto.Text = producto.Nombre;
            }
        }

        public class RecetaItem
        {
            public int IdReceta { get; set; }
            public string Nombre { get; set; }
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
                Console.WriteLine(ex.Message);
                MostrarMensajeSinConexionServidor();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
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
            openFileDialog.Filter = "Archivos de imagen|*.png|Todos los archivos|*.*";
            openFileDialog.Title = "Selecciona una imagen";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                imagenProductoBytes = File.ReadAllBytes(openFileDialog.FileName);

                string nombreImagen = System.IO.Path.GetFileName(openFileDialog.FileName);
                txbImagenProducto.Text = nombreImagen;
            }
        }

        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.ShowDialog();
        }

        private void MostrarMensajeModificacionExitosa()
        {
            ModificacionExitosa modificacionExitosa = new ModificacionExitosa();
            modificacionExitosa.ShowDialog();
        }

        private void MostrarMensajeDeshabilitacionExitosa()
        {
            DeshabilitacionExitosa deshabilitacionExitosa = new DeshabilitacionExitosa();
            deshabilitacionExitosa.ShowDialog();
        }

        private void MostrarMensajeProductoExistente()
        {
            ProductoExistente productoExistente = new ProductoExistente();
            productoExistente.ShowDialog();
        }

        private void MostrarMensajeProductoUtilizado()
        {
            ProductoUtilizado productoUtilizado = new ProductoUtilizado();
            productoUtilizado.ShowDialog();
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
            txbPrecioProducto.Clear();
            txbDescripcionProducto.Clear();
            txbRestriccionesProducto.Clear();
            txbCantidadProducto.Clear();
            txbImagenProducto.Text = "Sin imagen adjunta...";
            dtgTablaRecetas.ItemsSource = null;
            dtgTablaRecetas.Items.Clear();
            _ = RecuperarRecetas();
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            MenuProductos productos = new MenuProductos();
            NavigationService.Navigate(productos);
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

        private void CambiarModificarAzul(object sender, MouseEventArgs e)
        {
            btnModificar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarModificarVerde(object sender, MouseEventArgs e)
        {
            btnModificar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void CambiarLimpiarAzul(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarLimpiarVerde(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void CambiarDeshabilitarAzul(object sender, MouseEventArgs e)
        {
            btnDeshabilitar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarDeshabilitarRojo(object sender, MouseEventArgs e)
        {
            btnDeshabilitar.Source = new BitmapImage(new Uri("/Recursos/BotonRojo.png", UriKind.Relative));
        }

        private void CambiarAdjuntarAzul(object sender, MouseEventArgs e)
        {
            btnAdjuntarImagen.Source = new BitmapImage(new Uri("/Recursos/IconoMasAzul.png", UriKind.Relative));
        }

        private void CambiarAdjuntarVerde(object sender, MouseEventArgs e)
        {
            btnAdjuntarImagen.Source = new BitmapImage(new Uri("/Recursos/IconoMas.png", UriKind.Relative));
        }
    }
}
