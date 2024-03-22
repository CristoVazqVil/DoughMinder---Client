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
    /// Interaction logic for RegistroProveedores.xaml
    /// </summary>
    public partial class RegistroProveedores : Page
    {
        public RegistroProveedores()
        {
            InitializeComponent();
            txbTelefono.PreviewTextInput += TextBoxSoloNumeros_PreviewTextInput;

        }

        private void Registrar(object sender, MouseButtonEventArgs e)
        {
            if (!ValidarCamposVacios())
            {
                CamposVacios camposVacios = new CamposVacios();
                camposVacios.Show();
            }
            else
            {
                try
                {
                    DoughMinderServicio.ProveedorClient cliente = new DoughMinderServicio.ProveedorClient();
                    DoughMinderServicio.Proveedor proveedor = new DoughMinderServicio.Proveedor();

                    proveedor.Nombre = txbNombre.Text;
                    proveedor.Email = txbCorreo.Text;
                    proveedor.Telefono = txbTelefono.Text;

                    int codigo = cliente.GuardarProveedor(proveedor);


                    if (codigo == 1)
                    {
                        MostrarMensajeRegistroExitoso();
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensaProveedorExistente();
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

        private bool ValidarCamposVacios()
        {
            if (string.IsNullOrWhiteSpace(txbNombre.Text) ||
                string.IsNullOrWhiteSpace(txbTelefono.Text) ||
                string.IsNullOrWhiteSpace(txbCorreo.Text))
            {
                return false;
            }
            return true;
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

        
        private void MostrarMensaProveedorExistente()
        {
            ProveedorExistente proveedorExistente = new ProveedorExistente();
            proveedorExistente.Show();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.Show();
        }

        private void TextBoxSoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
