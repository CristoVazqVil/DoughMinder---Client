﻿using System;
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
using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for RegistrarEmpleado.xaml
    /// </summary>
    public partial class RegistroEmpleado : Page
    {

        
        public RegistroEmpleado(bool modificar,string usuario)
        {
            InitializeComponent();

            MostrarEmpleado(usuario);

            txbNombre.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
            txbPaterno.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
            txbMaterno.PreviewTextInput += TextBoxSoloLetras_PreviewTextInput;
            txbTelefono.PreviewTextInput += TextBoxSoloNumeros_PreviewTextInput;


            NoModificar(modificar);
        }


        private void NoModificar(bool permitirModificacion)
        {

            if (!permitirModificacion)
            {
                txbNombre.IsEnabled = false;
                txbPaterno.IsEnabled = false;
                txbMaterno.IsEnabled = false;
                txbTelefono.IsEnabled = false;
                txbDireccion.IsEnabled = false;
                txbCorreo.IsEnabled = false;
                txbUsuario.IsEnabled = false;
                txbContrasena.IsEnabled = false;
                txbConfirmaContrasena.IsEnabled = false;
                cbPuesto.IsEnabled = false;
                txbRFC.IsEnabled = false;
                chkEstado.IsEnabled = false;
            }
        }



        private void MostrarEmpleado(string RFC)
        {
            if (RFC == "")
            {
                btnModificar.Visibility = Visibility.Collapsed;
                lblModificar.Visibility = Visibility.Collapsed;
                bbConfirma.Visibility = Visibility.Visible;
                bbContrasena.Visibility = Visibility.Visible;
                txbConfirmaContrasena.Visibility = Visibility.Visible;
                txbContrasena.Visibility = Visibility.Visible;
                lbConfirmaContrasena.Visibility = Visibility.Visible;
                lbContrasena.Visibility = Visibility.Visible;
            }

            else
            {
                btnRegistrar.Visibility = Visibility.Collapsed;
                lblRegistrar.Visibility = Visibility.Collapsed;
                btnModificar.Visibility = Visibility.Visible;
                lblModificar.Visibility = Visibility.Visible;

                bbConfirma.Visibility = Visibility.Collapsed;
                bbContrasena.Visibility = Visibility.Collapsed;
                txbConfirmaContrasena.Visibility = Visibility.Collapsed;
                txbContrasena.Visibility = Visibility.Collapsed;
                lbConfirmaContrasena.Visibility = Visibility.Collapsed;
                lbContrasena.Visibility = Visibility.Collapsed;




                try
                {

                    DoughMinderServicio.EmpleadoClient cliente = new DoughMinderServicio.EmpleadoClient();
                    DoughMinderServicio.Empleado empleado = new DoughMinderServicio.Empleado();

                    empleado = cliente.BuscarEmpleado(RFC);

                    if (empleado != null)
                    {
                        txbPaterno.Text = empleado.Paterno;
                        txbMaterno.Text = empleado.Materno;
                        txbTelefono.Text = empleado.Telefono;
                        txbDireccion.Text = empleado.Direccion;
                        txbCorreo.Text = empleado.Correo;
                        txbNombre.Text = empleado.Nombre;
                        txbUsuario.Text = empleado.Usuario;
                        txbContrasena.Password = empleado.Contraseña;
                        txbConfirmaContrasena.Password = empleado.Contraseña;
                        txbRFC.Text = empleado.RFC;
                        chkEstado.IsChecked = empleado.Estado ?? false;

                        switch (empleado.IdPuesto)
                        {
                            case 1:
                                cbPuesto.SelectedIndex = 0;
                                break;
                            case 2:
                                cbPuesto.SelectedIndex = 1;
                                break;
                            case 3:
                                cbPuesto.SelectedIndex = 2;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        MostrarMensajeSinConexionServidor();
                    }
                }
                catch (TimeoutException ex)
                {
                    MostrarMensajeSinConexionServidor();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Error de comunicación al intentar recuperar el empleado. Detalles: " + ex.Message);
                    MostrarMensajeSinConexionServidor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Excepción al mostrar empleado: " + ex.Message);
                }
            }
        }



        private void Registrar()
        {
            ReiniciarColoresTextBox();

            if (!ValidarCamposVacios())
            {

                MarcarCamposVacios();
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
                        empleado.Correo = txbCorreo.Text;
                        empleado.Estado = chkEstado.IsChecked ?? false;
                        empleado.RFC = txbRFC.Text;

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

                            MenuEmpleados empleados = new MenuEmpleados();
                            this.NavigationService.Navigate(empleados);

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



        private void MarcarCamposVacios()
        {
            if (string.IsNullOrWhiteSpace(txbNombre.Text))
                brdrNombre.BorderBrush = Brushes.Red;

            if (string.IsNullOrWhiteSpace(txbDireccion.Text))
                brdrDireccion.BorderBrush = Brushes.Red;

            if (string.IsNullOrWhiteSpace(txbMaterno.Text))
                brdrMaterno.BorderBrush = Brushes.Red;

            if (string.IsNullOrWhiteSpace(txbRFC.Text))
                brdrRFC.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbPaterno.Text))
                brdrPaterno.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbRFC.Text))
                brdrRFC.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbTelefono.Text))
                brdrTelefono.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbUsuario.Text))
                brdrUsuario.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbConfirmaContrasena.Password))
                bbConfirma.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbContrasena.Password))
                bbContrasena.BorderBrush = Brushes.Red;


            if (string.IsNullOrWhiteSpace(txbContrasena.Password))
                bbContrasena.BorderBrush = Brushes.Red;

            if (string.IsNullOrWhiteSpace(txbCorreo.Text))
                brdrCorreo.BorderBrush = Brushes.Red;

            if (cbPuesto.SelectedItem == null)
            {
                lblPuesto.Foreground = Brushes.Red;
            }



        }


        private void ReiniciarColoresTextBox()
        {
            brdrTelefono.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrCorreo.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrRFC.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrNombre.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrDireccion.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrRFC.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrUsuario.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrMaterno.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            brdrPaterno.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
            lblPuesto.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF527243"));
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

        private void MostrarMensajeInsumoExistente()
        {
            EmpleadoExistente empleadoExistente = new EmpleadoExistente();
            empleadoExistente.Show();
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

        private void PermitirModificar(object sender, MouseButtonEventArgs e)
        {
            txbNombre.IsEnabled = true;
            txbPaterno.IsEnabled = true;
            txbMaterno.IsEnabled = true;
            txbTelefono.IsEnabled = true;
            txbDireccion.IsEnabled = true;
            txbCorreo.IsEnabled = true;
            txbContrasena.IsEnabled = true;
            txbConfirmaContrasena.IsEnabled = true;
            cbPuesto.IsEnabled = true;
            txbRFC.IsEnabled = true;
            chkEstado.IsEnabled = true;

            btnAceptarModificacion.Visibility = Visibility.Visible;
            lblAceptarModificacion.Visibility = Visibility.Visible;

            btnModificar.Visibility = Visibility.Collapsed;
            lblModificar.Visibility = Visibility.Collapsed;

            bbConfirma.Visibility = Visibility.Visible;
            bbContrasena.Visibility = Visibility.Visible;   

            txbConfirmaContrasena.Visibility = Visibility.Visible;
            txbContrasena.Visibility = Visibility.Visible;


            lbConfirmaContrasena.Visibility = Visibility.Visible;
            lbContrasena.Visibility = Visibility.Visible;




        }

        private void IrAtras(object sender, MouseButtonEventArgs e)
        {
            MenuEmpleados empleados = new MenuEmpleados();
            this.NavigationService.Navigate(empleados);
        }

        private void ModificarEmpleado(object sender, MouseButtonEventArgs e)
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
                    DoughMinderServicio.EmpleadoClient cliente = new DoughMinderServicio.EmpleadoClient();
                    DoughMinderServicio.Empleado empleado = new DoughMinderServicio.Empleado();
                    int codigo = cliente.ReemplazarEmpleado(txbUsuario.Text);

                    if (codigo == 1)
                    {
                        Registrar();
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

        private void ConfirmaRegistro(object sender, MouseButtonEventArgs e)
        {
            Registrar();
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

       
    }
}

