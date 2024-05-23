using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    /// Lógica de interacción para RegistroMovimiento.xaml
    /// </summary>
    public partial class RegistroMovimiento : Page
    {
        enum TipoMovimiento
        {
            Gastos,
            Ingresos
        }

        public RegistroMovimiento()
        {
            InitializeComponent();
            SetTipoMovimiento();
            SetFecha();
        }

        private void SetTipoMovimiento()
        {
            var tipos = Enum.GetValues(typeof(TipoMovimiento));

            foreach (var tipo in tipos)
            {
                cmbTipo.Items.Add(tipo);
            }
        }

        private void SetFecha()
        {
            DateTime fechaActual = DateTime.Now;
            lblFecha.Content = fechaActual.ToString();
        }

        private bool ValidarInformacion()
        {
            bool esValido = true;

            if (cmbTipo.SelectedItem == null)
            {
                esValido = false;
                lblTipoError.Content = "Este campo no puede estar vacío";
                lblTipoError.Visibility = Visibility.Visible;
                MostrarMensajeCamposVacios();
            }

            if (string.IsNullOrEmpty(txtbDescripcion.Text))
            {
                esValido = false;
                lblDescripcionError.Content = "Este campo no puede estar vacío";
                lblDescripcionError.Visibility = Visibility.Visible;
                MostrarMensajeCamposVacios();
            }

            if (string.IsNullOrEmpty(txtbCostoTotal.Text))
            {
                esValido = false;
                lblCostoError.Content = "Este campo no puede estar vacío";
                lblCostoError.Visibility = Visibility.Visible;
                MostrarMensajeCamposVacios();
            }

            return esValido;
        }

        private decimal RecuperarCostoTotal()
        {
            decimal costo = 0;
            string costoString = txtbCostoTotal.Text;

            if (cmbTipo.SelectedItem.ToString() == TipoMovimiento.Gastos.ToString())
            {
                string costoNegativo = "-" + costoString;
                decimal costoDecimal = Convert.ToDecimal(costoNegativo);
                costo = costoDecimal;
            }
            else
            {
                decimal costoDecimal = Convert.ToDecimal(costoString);
                costo = costoDecimal;
            }

            return costo;
        }

        private DateTime RecuperarFecha()
        {
            string fechaString = lblFecha.Content.ToString();
            DateTime fecha = DateTime.Parse(fechaString);
            return fecha;
        }

        private void RegistrarMovimiento()
        {
            if (ValidarInformacion())
            {
                try
                {
                    MovimientoClient client = new MovimientoClient();
                    Movimiento movimiento = new Movimiento();

                    movimiento.CostoTotal = RecuperarCostoTotal();
                    movimiento.Descripcion = txtbDescripcion.Text;
                    movimiento.Fecha = RecuperarFecha();

                    int codigo = client.RegistrarMovimiento(movimiento);

                    if (codigo == 1)
                    {
                        MostrarMensajeRegistroExitoso();
                        MostrarHistorialMovimientos();
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

        private void CmbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTipoError.Visibility = Visibility.Collapsed;
        }

        private void TxtbCostoTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string nuevoTexto = txtbCostoTotal.Text + e.Text;
            string regexCosto = @"^\d*\.?\d*$";
            Regex regex = new Regex(regexCosto);

            if (!regex.IsMatch(nuevoTexto))
            {
                e.Handled = true;
            }
        }

        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.Show();
        }

        private void MostrarMensajeRegistroExitoso()
        {
            RegistroExitoso registroExitoso = new RegistroExitoso();
            registroExitoso.Show();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.Show();
        }

        private void BtnRegistrar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrarMovimiento();
        }

        private void TxtbDescripcion_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblDescripcionError.Visibility = Visibility.Collapsed;                                                                                                          
        }

        private void TxtbCostoTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblCostoError.Visibility = Visibility.Collapsed;
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            MostrarHistorialMovimientos();
        }

        private void MostrarHistorialMovimientos()
        {
            HistorialMovimientos historialMovimientos = new HistorialMovimientos();
            NavigationService.Navigate(historialMovimientos);
        }

        private void MostrarMensajeCamposVacios()
        {
            CamposVacios camposVacios = new CamposVacios();
            camposVacios.Show();
        }
    }
}

