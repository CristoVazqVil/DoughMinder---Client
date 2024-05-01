using DoughMinder___Client.DoughMinderServicio;
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
            SetProductos();
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
                if (producto.RutaFoto == null)
                {
                    producto.RutaFoto = "/Recursos/ProductosBlack.png";
                }

                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(producto.RutaFoto, UriKind.Relative));

                image.Source = bitmapImage;
                image.Tag = producto.RutaFoto;
                image.Width = 80;

                Label lblDescripcion = new Label();
                lblDescripcion.Content = producto.Nombre + "  $" + producto.Precio;

                Grid gpProductoItem = new Grid();
                gpProductoItem.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
                gpProductoItem.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
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
            ModificacionInsumos insumos = new ModificacionInsumos();
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
    }
}
