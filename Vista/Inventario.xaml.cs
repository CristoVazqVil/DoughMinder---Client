using DoughMinder___Client.DoughMinderServicio;
using DoughMinder___Client.Vista.Emergentes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Inventario.xaml
    /// </summary>
    public partial class Inventario : Page
    {

        private List<InventarioElemento> inventarioElementos = new List<InventarioElemento>();
        private List<InventarioElemento> insumosActuales = new List<InventarioElemento>();
        private List<InventarioElemento> productosActuales = new List<InventarioElemento>();
        internal class InventarioElemento
        {
            public InventarioElemento() { }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Cantidad { get; set; }
        }
        public Inventario()
        {
            InitializeComponent();
            ColocarInventarioEnTabla();
        }

        private void GenerarReporte(object sender, MouseButtonEventArgs e)
        {
            string nombreArchivo = "Reporte Inventario - " + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = System.IO.Path.Combine(folder, nombreArchivo);
            int numColumnasTabla = 3;
            Font fontTitulo = FontFactory.GetFont(FontFactory.COURIER_BOLD, 16, Font.BOLD);

            Document documento = new Document();

            try
            {
                PdfWriter.GetInstance(documento, new FileStream(outputPath, FileMode.Create));
                documento.Open();

                iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("Inventario Actual", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                documento.Add(titulo);

                documento.Add(new iTextSharp.text.Paragraph(" "));

                iTextSharp.text.Paragraph fecha = new iTextSharp.text.Paragraph("Fecha de generación: " + DateTime.Now.ToString());
                documento.Add(fecha);

                documento.Add(new iTextSharp.text.Paragraph(" "));

                PdfPTable inventario = new PdfPTable(numColumnasTabla);
                inventario.AddCell("Código");
                inventario.AddCell("Insumos");
                inventario.AddCell("Cantidad");

                foreach (var elemento in insumosActuales)
                {
                    string codigo = elemento.Codigo;
                    string nombre = elemento.Nombre;
                    string cantidad = elemento.Cantidad;

                    inventario.AddCell(codigo);
                    inventario.AddCell(nombre);
                    inventario.AddCell(cantidad);
                }

                inventario.AddCell("  ");
                inventario.AddCell("  ");
                inventario.AddCell("  ");

                inventario.AddCell("Código");
                inventario.AddCell("Productos");
                inventario.AddCell("Cantidad");

                foreach (var elemento in productosActuales)
                {
                    string codigo = elemento.Codigo;
                    string nombre = elemento.Nombre;
                    string cantidad = elemento.Cantidad;

                    inventario.AddCell(codigo);
                    inventario.AddCell(nombre);
                    inventario.AddCell(cantidad);
                }

                documento.Add(inventario);

                if (File.Exists(outputPath))
                {
                    MostrarMensajeDescargaExitosa();
                }
                else
                {
                    MostrarMensajeErrorGenerarPdf();
                }

            }
            catch (DocumentException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeErrorGenerarPdf();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                MostrarMensajeErrorGenerarPdf();
            }
            finally
            {
                documento.Close();
            }
        }

        private void ColocarInventarioEnTabla()
        {
            List<Insumo> insumos = RecuperarInsumos();
            List<Producto> productos = RecuperarProductos();

            inventarioElementos.Add(new InventarioElemento
            {
                Codigo = "Insumos",
                Nombre = "",
                Cantidad = "",
            });

            foreach (var insumo in insumos)
            {
                inventarioElementos.Add(new InventarioElemento
                {
                    Codigo = insumo.Codigo,
                    Nombre = insumo.Nombre,
                    Cantidad = insumo.CantidadKiloLitro.ToString() + " Kg/L",
                });
            }

            inventarioElementos.Add(new InventarioElemento
            {
                Codigo = "Productos",
                Nombre = "",
                Cantidad = "",
            });

            foreach (var producto in productos)
            {
                inventarioElementos.Add(new InventarioElemento
                {
                    Codigo = producto.CodigoProducto,
                    Nombre = producto.Nombre,
                    Cantidad = producto.Cantidad.ToString() + " pzas.",
                });
            }

            lstInventario.ItemsSource = inventarioElementos;
            VerificarExistenciaInventario();
        }

        private List<Insumo> RecuperarInsumos()
        {
            InsumoClient cliente = new InsumoClient();
            List<Insumo> insumos = new List<Insumo>();

            try
            {
                insumos = cliente.RecuperarTodosInsumos().ToList();

                foreach (var insumo in insumos)
                {
                    insumosActuales.Add(new InventarioElemento
                    {
                        Codigo = insumo.Codigo,
                        Nombre = insumo.Nombre,
                        Cantidad = insumo.CantidadKiloLitro.ToString() + " Kg/L",
                    });
                }

                if (insumos == null)
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

            return insumos;
        }

        private List<Producto> RecuperarProductos()
        {
            ProductoClient cliente = new ProductoClient();
            List<Producto> productos = new List<Producto>();

            try
            {
                productos = cliente.RecuperarProductos().ToList();

                foreach (var producto in productos)
                {
                    productosActuales.Add(new InventarioElemento
                    {
                        Codigo = producto.CodigoProducto,
                        Nombre = producto.Nombre,
                        Cantidad = producto.Cantidad.ToString() + " pzas.",
                    });
                }

                if (productos == null)
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

            return productos;
        }

        private void VerificarExistenciaInventario()
        {
            if (lstInventario == null || lstInventario.Items.IsEmpty)
            {
                lstInventario.Visibility = Visibility.Collapsed;
                lblInventarioError.Content = "No hay insumos o productos registrados";
                lblInventarioError.Visibility = Visibility.Visible;
                DeshabilitarGenerarReporte();
            }
            else
            {
                lstInventario.Visibility = Visibility.Visible;
                lblInventarioError.Visibility = Visibility.Collapsed;
                btnGenerarReporte.IsEnabled = true;
            }
        }

        private void DeshabilitarGenerarReporte()
        {
            btnGenerarReporte.IsEnabled = false;
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

        private void MostrarMensajeErrorGenerarPdf()
        {
            ErrorGenerarPDF errorGenerarPDF = new ErrorGenerarPDF();
            errorGenerarPDF.Show();
        }

        private void MostrarMensajeDescargaExitosa()
        {
            DescargaExitosa descargaExitosa = new DescargaExitosa();
            descargaExitosa.Show();
        }

        private void AbrirVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void CambiarGenerarAzul(object sender, MouseEventArgs e)
        {
            btnGenerarReporte.Source = new BitmapImage(new Uri("/Recursos/BotonAzul.png", UriKind.Relative));
        }

        private void CambiarGenerarVerde(object sender, MouseEventArgs e)
        {
            btnGenerarReporte.Source = new BitmapImage(new Uri("/Recursos/BotonVerde.png", UriKind.Relative));
        }
    }
}
