﻿using DoughMinder___Client.DoughMinderServicio;
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
    /// Interaction logic for RegistroRecetas.xaml
    /// </summary>
    public partial class RegistroRecetas : Page
    {
        public RegistroRecetas()
        {
            InitializeComponent();
            _ = RecuperarInsumos();
        }

        public class InsumoItem
        {
            public int IdInsumo { get; set; }
            public string Nombre { get; set; }
            public bool Seleccionado { get; set; }
            public string Cantidad { get; set; }
        }

        private async Task RecuperarInsumos()
        {
            await Task.Delay(200);
            Dictionary<int, string> listaInsumos = new Dictionary<int, string>();
            try
            {
                DoughMinderServicio.InsumoClient cliente = new DoughMinderServicio.InsumoClient();
                listaInsumos = cliente.RecuperarInsumos();

                if (listaInsumos == null)
                {
                    MostrarMensajeSinConexionBase();
                }
                else
                {
                    MostrarListaInsumos(listaInsumos);
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
                    DoughMinderServicio.RecetaClient cliente = new DoughMinderServicio.RecetaClient();
                    DoughMinderServicio.Receta receta = new DoughMinderServicio.Receta();

                    receta.Nombre = txbNombreReceta.Text;
                    receta.Codigo = txbCodigoReceta.Text;
                    receta.Descripcion = txbProcedimientoReceta.Text;
                    receta.Estado = true;

                    int codigo = cliente.GuardarReceta(receta, CrearListaInsumos());

                    if (codigo > 1)
                    {
                        MostrarMensajeRegistroExitoso();
                        NavigationService.GoBack();
                    }
                    else
                    {
                        if (codigo == 0)
                        {
                            MostrarMensajeRecetaExistente();
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

            if (string.IsNullOrEmpty(txbNombreReceta.Text))
            {
                lblNombreReceta.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbCodigoReceta.Text))
            {
                lblCodigoReceta.Foreground = Brushes.Red;
                camposValidos = false;
            }

            if (string.IsNullOrEmpty(txbProcedimientoReceta.Text))
            {
                lblProcedimiento.Foreground = Brushes.Red;
                camposValidos = false;
            }

            bool alMenosUnInsumoSeleccionado = false;
            foreach (InsumoItem item in drgTablaInsumos.ItemsSource)
            {
                if (item.Seleccionado && !string.IsNullOrEmpty(item.Cantidad))
                {
                    alMenosUnInsumoSeleccionado = true;
                    break;
                }
            }

            if (!alMenosUnInsumoSeleccionado)
            {
                camposValidos = false;
                lblInsumos.Foreground = Brushes.Red;
                lblInsumos2.Foreground = Brushes.Red;
            }

            return camposValidos;
        }

        private void MostrarListaInsumos(Dictionary<int, string> listaInsumos)
        {
            List<InsumoItem> datosTabla = new List<InsumoItem>();

            foreach (var kvp in listaInsumos)
            {
                InsumoItem insumoItem = new InsumoItem
                {
                    IdInsumo = kvp.Key,
                    Nombre = kvp.Value,
                    Seleccionado = false,
                    Cantidad = ""
                };

                datosTabla.Add(insumoItem);
            }

            drgTablaInsumos.ItemsSource = datosTabla;
        }

        private Dictionary<int, float> CrearListaInsumos()
        {
            Dictionary<int, float> insumosSeleccionados = new Dictionary<int, float>();

            foreach (InsumoItem item in drgTablaInsumos.ItemsSource)
            {
                if (item.Seleccionado)
                {
                    if (float.TryParse(item.Cantidad, out float cantidad))
                    {
                        if (cantidad > 0)
                        {
                            insumosSeleccionados.Add(item.IdInsumo, cantidad);
                        }
                    }
                }
            }

            return insumosSeleccionados;
        }

        private void ReiniciarMarcadores()
        {
            lblProcedimiento.Foreground = Brushes.Black;
            lblNombreReceta.Foreground = Brushes.Black;
            lblCodigoReceta.Foreground = Brushes.Black;
            lblInsumos.Foreground = Brushes.Black;
            lblInsumos2.Foreground = Brushes.Black;
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

        private void MostrarMensajeRecetaExistente()
        {
            RecetaExistente recetaExistente = new RecetaExistente();
            recetaExistente.ShowDialog();
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
            txbCodigoReceta.Clear();
            txbNombreReceta.Clear();
            txbProcedimientoReceta.Clear();
            drgTablaInsumos.ItemsSource = null;
            drgTablaInsumos.Items.Clear();
            _ = RecuperarInsumos();
        }

        //Validaciones de entradas y cambios gráficos
        private void CambiarLimpiarAzul(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarLimpiarVerde(object sender, MouseEventArgs e)
        {
            btnLimpiar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void CambiarRegistrarAzul(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarRegistrarVerde(object sender, MouseEventArgs e)
        {
            btnRegistrar.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }

        private void EliminarCaracteresNombre(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            string texto = textBox.Text + e.Text;

            Regex regex = new Regex(@"^[a-zA-Z0-9áéíóúüÁÉÍÓÚÜ,. ]*$");

            if (texto.Length <= 100 && regex.IsMatch(texto))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EliminarCaracteresProcedimiento(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            string texto = textBox.Text + e.Text;

            if (texto.Length <= 1000)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
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
    }
}
