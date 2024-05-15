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

        private Proveedor proveedor;

        public RegistroProveedores(Proveedor proveedor)
        {
            InitializeComponent();
            txbTelefono.PreviewTextInput += TextBoxSoloNumeros_PreviewTextInput;

            this.proveedor = proveedor;

            if (proveedor != null)
            {
                txbNombre.Text = proveedor.Nombre;
                txbCorreo.Text = proveedor.Email;
                txbTelefono.Text = proveedor.Telefono;
                txbRFC.Text = proveedor.RFC;

                if (!string.IsNullOrEmpty(proveedor.Nombre))
                {
                    ProveedorCargado();
                }
            }
        }

        public void ProveedorCargado()
        {
            txbNombre.IsEnabled = false;
            txbCorreo.IsEnabled = false;
            txbTelefono.IsEnabled = false;
            txbRFC.IsEnabled = false;
            btnRegistro.IsEnabled = false;
            btnRegistro.Visibility = Visibility.Collapsed;
            btnModificar.Visibility = Visibility.Visible;

        }
        public void InformacionProveedor(Proveedor proveedor)
        {
            InitializeComponent();
            this.proveedor = proveedor;

            txbNombre.Text = proveedor.Nombre;
            txbCorreo.Text = proveedor.Email;
            txbTelefono.Text = proveedor.Telefono;
            txbRFC.Text = proveedor.RFC;
            
        }

        private void RegistrarProveedor()
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
                    proveedor.RFC = txbRFC.Text;


                    int codigo = cliente.GuardarProveedor(proveedor);


                    if (codigo == 1)
                    {
                        MostrarMensajeRegistroExitoso();

                        MenuProveedores proveedores = new MenuProveedores();
                        this.NavigationService.Navigate(proveedores); 

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
            if (string.IsNullOrWhiteSpace(txbNombre.Text) ||
                string.IsNullOrWhiteSpace(txbTelefono.Text) ||
                string.IsNullOrWhiteSpace(txbCorreo.Text) ||
                string.IsNullOrWhiteSpace(txbRFC.Text))
            {
                return false;
            }

            
            if (!txbCorreo.Text.Contains("@"))
            {
                MessageBox.Show("El correo electrónico debe contener un '@'.", "Correo Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MenuProveedores proveedores = new MenuProveedores();
            this.NavigationService.Navigate(proveedores);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;


            if (textBox.Text.Length > 20)
            {
                textBox.Text = textBox.Text.Substring(0, 20);
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void Modificar(object sender, MouseButtonEventArgs e)
        {
            txbNombre.IsEnabled = true;
            txbCorreo.IsEnabled = true;
            txbTelefono.IsEnabled = true;
            txbRFC.IsEnabled = true;
            btnRegistro.IsEnabled = false;
            btnModificar.Visibility = Visibility.Collapsed;

            btnActualizarProveedor.Visibility = Visibility.Visible;
            btnActualizarProveedor.IsEnabled = true;

        }

        private void ActualizarProveedor(object sender, MouseButtonEventArgs e)
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
                    int codigo = cliente.ReemplazarProveedor(txbRFC.Text);

                    if (codigo == 1)
                    {
                        RegistrarProveedor();
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

        private void ClickRegistrar(object sender, MouseButtonEventArgs e)
        {
            RegistrarProveedor();
            
        }

    }

}

    
