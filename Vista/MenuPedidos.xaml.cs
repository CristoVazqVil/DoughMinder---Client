﻿using DoughMinder___Client.DoughMinderServicio;
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

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para MenuPedidos.xaml
    /// </summary>
    public partial class MenuPedidos : Page
    {
        public MenuPedidos()
        {
            InitializeComponent();
            SetEstados();
            SetPedidos();
        }

        private List<Pedido> pedidos;

        private string[] estados =
            {
                "Todos",
                "Ordenado",
                "En proceso",
                "Preparado",
                "Entregado",
                "Cancelado"
            };


        private void SetEstados()
        {
            cmbEstado.ItemsSource = estados;
            cmbEstado.SelectedIndex = 0;
        }

        private void SetPedidos()
        {
            pedidos = RecuperarPedidos();

            if (pedidos != null && pedidos.Count > 0)
            {
                lstPedidos.Visibility = Visibility.Visible;
                lstPedidos.ItemsSource = pedidos;

            }
            else
            {
                lblPedidosError.Content = "No se han realizado pedidos";
                lblPedidosError.Visibility = Visibility.Visible;
                cmbEstado.IsEnabled = false;
            }
        }

        private List<Pedido> RecuperarPedidos()
        {
            pedidos = new List<Pedido>();

            try
            {
                PedidoClient pedidoClient = new PedidoClient();
                pedidos = pedidoClient.RecuperarPedidos().ToList();

                if (pedidos == null)
                {
                    MostrarMensajeSinConexionBase();
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

            return pedidos;
        }

        

        private void TxtbBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string busqueda = txtbBuscar.Text.ToLower();

            VerificarCampoBusqueda(busqueda);

            if (lstPedidos != null || !lstPedidos.Items.IsEmpty)
            {
                List<Pedido> pedidosFiltrados = FiltrarPedidos(pedidos, busqueda);
                pedidosFiltrados = FiltrarPedidosPorEstado(pedidosFiltrados);
                lstPedidos.ItemsSource = pedidosFiltrados;
                VerificarResultadoFiltrado();
            }
        }

        private void VerificarCampoBusqueda(string busqueda)
        {
            if (string.IsNullOrEmpty(busqueda))
            {
                lblBuscar.Visibility = Visibility.Visible;
            }
            else
            {
                lblBuscar.Visibility = Visibility.Collapsed;
            }
        }

        private List<Pedido> FiltrarPedidos(List<Pedido> listaPedidos, string busqueda)
        {
            List<Pedido> pedidos = new List<Pedido>();

            if (!string.IsNullOrEmpty(busqueda))
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Clave.ToLower().Contains(busqueda) || pedido.Fecha.ToString().ToLower().Contains(busqueda) || pedido.Estado.ToLower().Contains(busqueda)).ToList();
                pedidos = pedidosFiltrados.ToList();
            }

            return pedidos;
        }

        private void BtnAtras_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ImgRealizarPedido_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistroPedido registroPedido = new RegistroPedido();
            NavigationService.Navigate(registroPedido);
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            Button btnModificar = sender as Button;

            if (btnModificar != null)
            {
                Pedido pedido = btnModificar.DataContext as Pedido;

                if (pedido != null)
                {
                    MostrarVentanaModificacionPedido(pedido.Clave);
                }
            }
        }

        private void CmbEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string busqueda = txtbBuscar.Text.ToLower();
            List<Pedido> pedidosFiltrados = FiltrarPedidosPorEstado(pedidos);
            pedidosFiltrados = FiltrarPedidos(pedidosFiltrados, busqueda);
            lstPedidos.ItemsSource = pedidosFiltrados;
            VerificarResultadoFiltrado();
        }

        private void VerificarResultadoFiltrado()
        {
            if (lstPedidos == null || lstPedidos.Items.IsEmpty)
            {
                lstPedidos.Visibility = Visibility.Collapsed;
                lblPedidosError.Content = "No hay coincidencias en la búsqueda";
                lblPedidosError.Visibility = Visibility.Visible;
            }
            else
            {
                lstPedidos.Visibility=Visibility.Visible;
                lblPedidosError.Visibility=Visibility.Collapsed;
            }
        }

        private List<Pedido> FiltrarPedidosPorEstado(List<Pedido> listaPedidos)
        {
            List<Pedido> pedidos = new List<Pedido>();
            string estadoSeleccionado = cmbEstado.SelectedItem as string;

            if (estadoSeleccionado == estados[0])
            {
                pedidos = listaPedidos;
            }
            else if (estadoSeleccionado == estados[1])
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Estado == estados[1]);
                pedidos = pedidosFiltrados.ToList();
            }
            else if (estadoSeleccionado == estados[2])
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Estado == estados[2]);
                pedidos = pedidosFiltrados.ToList();
            }
            else if (estadoSeleccionado == estados[3])
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Estado == estados[3]);
                pedidos = pedidosFiltrados.ToList();
            }
            else if (estadoSeleccionado == estados[4])
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Estado == estados[4]);
                pedidos = pedidosFiltrados.ToList();
            }
            else if (estadoSeleccionado == estados[5])
            {
                var pedidosFiltrados = listaPedidos.Where(pedido => pedido.Estado == estados[5]);
                pedidos = pedidosFiltrados.ToList();
            }

            return pedidos;
        }

        private void MostrarMensajeSinConexionServidor()
        {
            SinConexionServidor sinConexionServidor = new SinConexionServidor();
            sinConexionServidor.Show();
        }


        private void MostrarMensajeSinConexionBase()
        {
            ConexionFallidaBase conexionFallidaBase = new ConexionFallidaBase();
            conexionFallidaBase.Show();
        }

        private void MostrarVentanaModificacionPedido(string clave)
        {
            ModificacionPedido modificacionPedido = new ModificacionPedido(clave);          
            NavigationService.Navigate(modificacionPedido);
        }
    }
}
