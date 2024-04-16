using System;
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

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Interaction logic for MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : Page
    {
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void AbrirProductos(object sender, MouseButtonEventArgs e)
        {
            Productos productos = new Productos();
            this.NavigationService.Navigate(productos);
        }

        private void AbrirEmpleados(object sender, MouseButtonEventArgs e)
        {
            Empleados empleados = new Empleados();
            this.NavigationService.Navigate(empleados);
        }

        private void AbrirInsumos(object sender, MouseButtonEventArgs e)
        {
            RegistroInsumos insumos = new RegistroInsumos();
            this.NavigationService.Navigate(insumos);
        }

        private void AbrirRecetas(object sender, MouseButtonEventArgs e)
        {
            RegistroRecetas recetas = new RegistroRecetas();
            this.NavigationService.Navigate(recetas);
        }

        private void AbrirProveedores(object sender, MouseButtonEventArgs e)
        {
            RegistroProveedores proveedores = new RegistroProveedores();
            this.NavigationService.Navigate(proveedores);
        }

        private void AbrirRegistroPedido(object sender, RoutedEventArgs e)
        {
            RegistroPedido pedido = new RegistroPedido();
            this.NavigationService.Navigate(pedido);
        }

        private void AbrirSolicitudInsumo(object sender, RoutedEventArgs e)
        {
            SolicitudInsumo solicitudInsumo = new SolicitudInsumo();
            this.NavigationService.Navigate(solicitudInsumo);
        }

        private void AbrirRegistroMovimiento(object sender, RoutedEventArgs e)
        {
            RegistroMovimiento registroMovimiento = new RegistroMovimiento();
            this.NavigationService.Navigate(registroMovimiento);
        }
    }
}
