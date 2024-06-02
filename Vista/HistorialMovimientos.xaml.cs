using DoughMinder___Client.DoughMinderServicio;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace DoughMinder___Client.Vista
{
    /// <summary>
    /// Lógica de interacción para HistorialMovimientos.xaml
    /// </summary>
    /// 
    internal class MovimientoElemento
    {
        public MovimientoElemento() { }
        public DateTime Fecha {  get; set; }
        public decimal CostoTotal { get; set; }
        public string Descripcion {  get; set; }
    }



    public partial class HistorialMovimientos : Page
    {
        private List<MovimientoElemento> elementos = new List<MovimientoElemento>();
        private List<MovimientoElemento> elementosOrdenados = new List<MovimientoElemento>();

        public HistorialMovimientos()
        {
            InitializeComponent();
            SetFecha();
            SetMovimientos();
            SetCostoTotal();
        }

        private void SetFecha()
        {
            DateTime fechaActual = DateTime.Now;
            lblFecha.Content = "Fecha de generación: " + fechaActual.ToString();
        }

        private void SetMovimientos()
        {
            List<Movimiento> movimientos = RecuperarMovimientos();
            List<Pedido> pedidos = RecuperarPedidos();
            List<Solicitud> solicitudes = RecuperarSolicitudesDeInsumo();


            foreach (var movimiento in movimientos)
            {
                elementos.Add(new MovimientoElemento
                {
                    CostoTotal = (decimal)movimiento.CostoTotal,
                    Descripcion = movimiento.Descripcion,
                    Fecha = (DateTime)movimiento.Fecha,
                });
            }

            foreach (var pedido in pedidos)
            {
                elementos.Add(new MovimientoElemento
                {
                    CostoTotal = (decimal)pedido.CostoTotal,
                    Descripcion = pedido.Clave,
                    Fecha = (DateTime)pedido.Fecha,
                });
            }

            foreach (var solicitud in solicitudes)
            {
                elementos.Add(new MovimientoElemento
                {
                    CostoTotal = (decimal)solicitud.CostoTotal,
                    Descripcion = "Solicitud de Insumo",
                    Fecha = (DateTime)solicitud.Fecha,
                });
            }

            OrdenarMovimientos(elementos);
            VerificarExistenciaMovimientos();
        }

        private void SetCostoTotal()
        {
            decimal costoTotal = 0;

            foreach (var elemento in elementos)
            {
                costoTotal += elemento.CostoTotal;
            }

            lblTotal.Content = "Total en caja: $" + costoTotal.ToString();
        }

        private void OrdenarMovimientos(List<MovimientoElemento> movimientos)
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaInicioUltimoMes = fechaActual.AddMonths(-1);

            var movimientosUltimoMes = movimientos.Where(m => m.Fecha >= fechaInicioUltimoMes).ToList();
            var movimientosOrdenados = movimientosUltimoMes.OrderBy(m => m.Fecha).ToList();
            elementosOrdenados = movimientosOrdenados;
            lstMovimientos.ItemsSource = elementosOrdenados;
        }

        private void VerificarExistenciaMovimientos()
        {
            if (lstMovimientos == null || lstMovimientos.Items.IsEmpty)
            {
                lstMovimientos.Visibility = Visibility.Collapsed;
                lblMovimientosError.Content = "No se han registrado movimientos";
                lblMovimientosError.Visibility = Visibility.Visible;
                btnGenerarReporte.IsEnabled = false;
            }
            else
            {
                lstMovimientos.Visibility = Visibility.Visible;
                lblMovimientosError.Visibility = Visibility.Collapsed;
            }
        }

        private List<Movimiento> RecuperarMovimientos()
        {
            MovimientoClient movimientoClient = new MovimientoClient();
            List<Movimiento> movimientos = new List<Movimiento>();

            try
            {
                movimientos = movimientoClient.RecuperarMovimientos().ToList();

                if (movimientos == null)
                {
                    MostrarMensajeSinConexionBase();
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

            return movimientos;
        }

        private List<Pedido> RecuperarPedidos()
        {
            PedidoClient pedidoClient = new PedidoClient();
            List<Pedido> pedidos = new List<Pedido>();

            try
            {
                pedidos = pedidoClient.RecuperarPedidosNoCancelados().ToList();

                if (pedidos == null)
                {
                    MostrarMensajeSinConexionBase();
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

            return pedidos;
        }

        private List<Solicitud> RecuperarSolicitudesDeInsumo()
        {
            SolicitudClient solicitudClient = new SolicitudClient();
            List<Solicitud> solicitudes = new List<Solicitud>();

            try
            {
                solicitudes = solicitudClient.RecuperarSolicitudes().ToList();

                if (solicitudes == null)
                {
                    MostrarMensajeSinConexionBase();
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

            return solicitudes;
        }

        private void GenerarReporte()
        {
            string nombreArchivo = "reporteMovimientos.pdf";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = System.IO.Path.Combine(folder, nombreArchivo);
            int numColumnasTabla = 3;
            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD);

            Document documento = new Document();

            try
            {
                PdfWriter.GetInstance(documento, new FileStream(outputPath, FileMode.Create));
                documento.Open();

                iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("Reporte de caja del último mes", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                documento.Add(titulo);

                documento.Add(new iTextSharp.text.Paragraph(" "));

                iTextSharp.text.Paragraph fecha = new iTextSharp.text.Paragraph(lblFecha.Content.ToString());
                documento.Add(fecha);

                iTextSharp.text.Paragraph total = new iTextSharp.text.Paragraph(lblTotal.Content.ToString());
                documento.Add(total);

                documento.Add(new iTextSharp.text.Paragraph(" "));

                PdfPTable movimientos = new PdfPTable(numColumnasTabla);
                movimientos.AddCell("Monto");
                movimientos.AddCell("Descripción");
                movimientos.AddCell("Fecha");

                foreach (var elemento in elementosOrdenados)
                {
                    string costoTotal = elemento.CostoTotal.ToString();
                    string descripcion = elemento.Descripcion;
                    string fechaElemento = elemento.Fecha.ToString();

                    movimientos.AddCell(costoTotal);
                    movimientos.AddCell(descripcion);
                    movimientos.AddCell(fechaElemento);
                }

                documento.Add(movimientos);

                if (File.Exists(outputPath))
                {
                    MostrarMensajeDescargaExitosa();
                    RecargarVentana();
                }
                else
                {
                    MostrarMensajeErrorGenerarPdf();
                }

            }
            catch (DocumentException ex)
            {
                MostrarMensajeErrorGenerarPdf();
            }
            catch (IOException ex)
            {
                MostrarMensajeErrorGenerarPdf();
            }
            finally
            {
                documento.Close();
            }
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

        private void RecargarVentana()
        {
            NavigationService.Refresh();
        }

        private void MostrarMenuPrincipal()
        {
            MenuPrincipal menuPrincipal = new MenuPrincipal();
            NavigationService.Navigate(menuPrincipal);
        }

        private void BtnAtras_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MostrarMenuPrincipal();
        }

        private void ImgRegistrarMovimiento_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistroMovimiento registroMovimiento = new RegistroMovimiento();
            NavigationService.Navigate(registroMovimiento);
        }

        private void BtnGenerarReporte_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GenerarReporte();
        }
    }
}
