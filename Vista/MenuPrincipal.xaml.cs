using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Recursos.Singleton;
using System;
using System.Collections.Generic;
using System.IO;
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
            SetProductos();
            ValidarPuesto();
        }

        private void SetProductos()
        {
            DoughMinderServicio.ProductoClient productoClient = new DoughMinderServicio.ProductoClient();
            List<Producto> productos = new List<Producto>();
            productos = productoClient.RecuperarProductos().ToList();

            int maxColumn = 3;
            int row = 0;
            int column = 0;

            gpVistaProductos.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            foreach (Producto producto in productos)
            {
                Image image = new Image();

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(producto.Foto);
                bitmapImage.EndInit();

                image.Source = bitmapImage;
                image.Tag = producto.Nombre;
                image.Width = 150;

                Label lblDescripcion = new Label();
                lblDescripcion.Content = producto.Nombre + "  $" + producto.Precio;

                Grid gpProductoItem = new Grid();
                gpProductoItem.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(120) });
                gpProductoItem.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(120) });
                gpProductoItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                Grid.SetRow(image, 0);
                Grid.SetRow(lblDescripcion, 1);
                gpProductoItem.Children.Add(image);
                gpProductoItem.Children.Add(lblDescripcion);

                gpVistaProductos.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                Grid.SetColumn(gpProductoItem, column);
                Grid.SetRow(gpProductoItem, row);
                gpVistaProductos.Children.Add(gpProductoItem);

                column++;

                if (column >= maxColumn)
                {
                    gpVistaProductos.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    column = 0;
                    row++;
                }
            }
        }

        private void AbrirProductos(object sender, MouseButtonEventArgs e)
        {
            MenuProductos productos = new MenuProductos();
            this.NavigationService.Navigate(productos);
        }

        private void AbrirEmpleados(object sender, MouseButtonEventArgs e)
        {
            MenuEmpleados empleados = new MenuEmpleados();
            this.NavigationService.Navigate(empleados);
        }

        private void AbrirInsumos(object sender, MouseButtonEventArgs e)
        {
            MenuInsumos insumos = new MenuInsumos();
            this.NavigationService.Navigate(insumos);
        }

        private void AbrirRecetas(object sender, MouseButtonEventArgs e)
        {
            MenuRecetas recetas = new MenuRecetas();
            this.NavigationService.Navigate(recetas);
        }

        private void AbrirProveedores(object sender, MouseButtonEventArgs e)
        {
            MenuProveedores proveedores = new MenuProveedores();
            this.NavigationService.Navigate(proveedores);
        }

        private void AbrirPedidos(object sender, MouseButtonEventArgs e)
        {
            MenuPedidos pedidos = new MenuPedidos();
            this.NavigationService.Navigate(pedidos);
        }

        private void AbrirFinanzas(object sender, MouseButtonEventArgs e)
        {
            HistorialMovimientos movimientos = new HistorialMovimientos();
            this.NavigationService.Navigate(movimientos);
        }

        private void AbrirInventario(object sender, MouseButtonEventArgs e)
        {
            Inventario inventario = new Inventario();
            this.NavigationService.Navigate(inventario);
        }
        private void Salir(object sender, MouseButtonEventArgs e)
        {
            string appName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(appName);
        }

        private void ValidarPuesto()
        {
            int puesto = SesionSingleton.Instance.Puesto;

            switch (puesto)
            {
                case 1:
                    gpPedidos.Visibility = Visibility.Collapsed;
                    gpReceta.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    gpEmpleados.Visibility = Visibility.Collapsed;
                    imgInventario.Visibility = Visibility.Collapsed;
                    gpFinanzas.Visibility = Visibility.Collapsed;
                    gpInsumo.Visibility = Visibility.Collapsed;
                    gpProveedor.Visibility = Visibility.Collapsed;
                    gpProductos.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    gpEmpleados.Visibility = Visibility.Collapsed;
                    imgInventario.Visibility = Visibility.Collapsed;
                    gpInsumo.Visibility = Visibility.Collapsed;
                    gpProveedor.Visibility = Visibility.Collapsed;
                    gpProductos.Visibility = Visibility.Collapsed;
                    gpReceta.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

    }
}
