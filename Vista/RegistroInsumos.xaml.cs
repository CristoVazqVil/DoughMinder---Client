﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
using DoughMinder___Client.Vista.Emergentes;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for RegistroInsumos.xaml
    /// </summary>
    public partial class RegistroInsumos : Page
    {
        private string rutaImagenSeleccionada;

        public RegistroInsumos()
        {
            InitializeComponent();
        }

        private void Registrar(object sender, MouseButtonEventArgs e)
        {
            ReiniciarMarcadores();

            if (!ValidarCamposVacios())
            {
                CamposVacios camposVacios = new CamposVacios();
                camposVacios.Show();
            } 
            else
            {
                try
                {
                    DoughMinderServicio.InsumoClient cliente = new DoughMinderServicio.InsumoClient();
                    DoughMinderServicio.Insumo insumo = new DoughMinderServicio.Insumo();

                    decimal precio; 
                    decimal.TryParse(txbPrecioInsumo.Text, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out precio);

                    insumo.Nombre = txbNombreInsumo.Text;
                    insumo.Codigo = txbCodigoInsumo.Text;
                    insumo.CantidadKiloLitro = double.Parse(txbCantidadInsumo.Text);
                    insumo.PrecioKiloLitro = precio;
                    insumo.RutaFoto = txbImagenInsumo.Text;
                    insumo.Estado = true;

                    int codigo = cliente.GuardarInsumo(insumo);

                    if (codigo == 1)
                    {
                        MostrarMensajeRegistroExitoso();
                        NavigationService.GoBack();
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensajeInsumoExistente();
                        }
                        else
                        {
                            MostrarMensajeSinConexionBase();
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

        private bool ValidarCamposVacios()
        {
            bool camposValidos = true;

            if (string.IsNullOrEmpty(txbCantidadInsumo.Text))
            {
                lblCantidad.Foreground = Brushes.Red;
                camposValidos = false; 
            }

            if (string.IsNullOrEmpty(txbNombreInsumo.Text))
            {
                lblNombre.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbCodigoInsumo.Text))
            {
                lblCodigoInsumo.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbPrecioInsumo.Text))
            {
                lblPrecio.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbImagenInsumo.Text) || txbImagenInsumo.Text.Equals("Sin imagen adjunta..."))
            {
                lblImagen.Foreground = Brushes.Red;
                camposValidos = false;
            }

            return camposValidos;
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

                txbImagenInsumo.Text = nombreImagen;
            }
        }

        private void ReiniciarMarcadores()
        {
            lblCantidad.Foreground = Brushes.Black;
            lblNombre.Foreground = Brushes.Black;
            lblCodigoInsumo.Foreground = Brushes.Black;
            lblPrecio.Foreground = Brushes.Black;
            lblImagen.Foreground = Brushes.Black;
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

        private void MostrarMensajeInsumoExistente()
        {
            InsumoExistente insumoExistente = new InsumoExistente();
            insumoExistente.ShowDialog();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.ShowDialog();
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LimpiarCampos(object sender, MouseButtonEventArgs e)
        {
            ReiniciarMarcadores();
            txbNombreInsumo.Clear();
            txbImagenInsumo.Text = "Sin imagen adjunta...";
            txbCodigoInsumo.Clear();
            txbCantidadInsumo.Clear();
            txbPrecioInsumo.Clear();
        }

        //Validaciones de entradas y cambios gráficos
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

        private void EliminarCaracteresCantidad(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Length >= 12)
            {
                e.Handled = true;
                return;
            }

            Regex regex = new Regex(@"^\d+(\.\d{0,3})?$");
            e.Handled = !regex.IsMatch(textBox.Text + e.Text);
        }

        private void EliminarCaracteresNombre(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Length >= 60 || !Regex.IsMatch(e.Text, @"^[a-zA-ZáéíóúüÁÉÍÓÚÜ]+$"))
            {
                e.Handled = true;
                return;
            }
        }

        private void EliminarCaracteresCodigo(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Regex regex = new Regex(@"^[a-zA-Z0-9\-]*$");

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

        private void CambiarRegistrarAzul(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarRegistrarVerde(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void CambiarLimpiarAzul(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarLimpiarVerde(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
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
