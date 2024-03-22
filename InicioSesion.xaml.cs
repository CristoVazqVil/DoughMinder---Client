using DoughMinder___Client.Recursos.Singleton;
using DoughMinder___Client.Vista;
using DoughMinder___Client.Vista.Emergentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace DoughMinder___Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InicioSesion : Window
    {
        public InicioSesion()
        {
            InitializeComponent();
        }

        private void Entrar(object sender, MouseButtonEventArgs e)
        {
            if (!ValidarCamposVacios())
            {
                CamposVacios camposVacios = new CamposVacios();
                camposVacios.ShowDialog();
            }
            else
            {
                try
                {
                    DoughMinderServicio.LoginClient cliente = new DoughMinderServicio.LoginClient();

                    int usuarioExistente = cliente.VerificarUsuario(txbUsuario.Text, pwbContraseña.Password);

                    if (usuarioExistente == 0)
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo.", "Usuario o contraseña incorrectos", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        DoughMinderServicio.Login login = cliente.RecuperarCuenta(txbUsuario.Text, pwbContraseña.Password);
                        SesionSingleton.Instance.SetNombre(login.Nombre);
                        SesionSingleton.Instance.SetUsuario(login.Usuario);
                        SesionSingleton.Instance.SetPuesto(login.Puesto);
                        VentanaPrincipal ventanaPrincipal = new VentanaPrincipal();
                        ventanaPrincipal.Show();
                        this.Close();
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
            bool camposValidos = true;

            if (string.IsNullOrEmpty(txbUsuario.Text))
            {
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(pwbContraseña.Password))
            {
                camposValidos = false;
            }

            return camposValidos;
        }


        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.ShowDialog();
        }

        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.ShowDialog();
        }

        private void CambiarEntrarAzul(object sender, MouseEventArgs e)
        {
            btnEntrar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarEntrarVerde(object sender, MouseEventArgs e)
        {
            btnEntrar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void EliminarCaracteresUsuario(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text.Length >= 50 || !char.IsLetterOrDigit(e.Text[0]))
                {
                    e.Handled = true;
                }
            }
        }

        private void EliminarCaracteresContraseña(object sender, TextCompositionEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox.Password.Length >= 60 || !char.IsLetterOrDigit(e.Text[0]))
            {
                e.Handled = true;
            }
        }
    }
}
