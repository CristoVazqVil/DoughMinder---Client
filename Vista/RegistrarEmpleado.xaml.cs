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
using DoughMinder___Client.Vista.Emergentes;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for RegistrarEmpleado.xaml
    /// </summary>
    public partial class RegistrarEmpleado : Page
    {
        public RegistrarEmpleado()
        {
            InitializeComponent();

            //TODO: PERMITIR SOLO LETRAS EN TODOS LOS CAMPOS Y PERMITIR SOLO NÚMEROS EN TELEFONO
            //DESHABILITAR BOTÓN ACTIVO

            txbNombre.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
            txbPaterno.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
            txbMaterno.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
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
                if (!ConfirmarContrasena())
                {
                    return;
                }
                else
                {
                    try
                    {
                        DoughMinderServicio.EmpleadoClient cliente = new DoughMinderServicio.EmpleadoClient();
                        DoughMinderServicio.Empleado empleado = new DoughMinderServicio.Empleado();

                        empleado.Nombre = txbNombre.Text;
                        empleado.Paterno = txbPaterno.Text;
                        empleado.Materno = txbMaterno.Text;
                        empleado.Direccion = txbDireccion.Text;
                        empleado.Telefono = txbTelefono.Text;
                        empleado.Usuario = txbUsuario.Text;
                        empleado.Contraseña = txbContrasena.Password;
                        empleado.Estado = true;

                        ComboBoxItem item = (ComboBoxItem)cbPuesto.SelectedItem;
                        string puestoSeleccionado = item.Content.ToString();

                        switch (puestoSeleccionado)
                        {
                            case "Gerente":
                                empleado.IdPuesto = 1;
                                break;
                            case "Cocinero":
                                empleado.IdPuesto = 2;
                                break;
                            case "Cajero":
                                empleado.IdPuesto = 3;
                                break;
                            default:

                                break;
                        }

                        int codigo = cliente.GuardarEmpleado(empleado);

                        if (codigo == 1)
                        {
                            MostrarMensajeRegistroExitoso();
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
                        MostrarMensajeSinConexionServidor();
                    }
                    catch (CommunicationException ex)
                    {
                        MostrarMensajeSinConexionServidor();
                    }
                }
            }
        }


        private bool ValidarCamposVacios()
        {
            if (string.IsNullOrWhiteSpace(txbNombre.Text) ||
                string.IsNullOrWhiteSpace(txbPaterno.Text) ||
                string.IsNullOrWhiteSpace(txbMaterno.Text) ||
                string.IsNullOrWhiteSpace(txbDireccion.Text) ||
                string.IsNullOrWhiteSpace(txbTelefono.Text) ||
                string.IsNullOrWhiteSpace(txbUsuario.Text) ||
                string.IsNullOrWhiteSpace(txbContrasena.Password) ||
                cbPuesto.SelectedItem == null)
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

        //CORREGIR
        private void MostrarMensajeInsumoExistente()
        {
            InsumoExistente insumoExistente = new InsumoExistente();
            insumoExistente.Show();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.Show();
        }


        private void TextBoxSoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxSoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private bool ConfirmarContrasena()
        {
            if (txbContrasena.Password != txbConfirmaContrasena.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true; 
        }



    }
}

