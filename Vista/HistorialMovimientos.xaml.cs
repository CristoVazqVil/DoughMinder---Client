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
        public DateTime Fecha { get; set; }
        public decimal CostoTotal { get; set; }
        public string Descripcion { get; set; }
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
        }

        private void SetFecha()
        {
            dpFecha.SelectedDate = RecuperarFechaActual();
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

            FiltrarMovimientos(DateTime.Now);
        }

        private void CalcularCostos(List<MovimientoElemento> movimientos)
        {
            decimal costoTotal = 0;
            decimal ingresos = 0;
            decimal egresos = 0;

            foreach (var movimiento in movimientos)
            {
                costoTotal += movimiento.CostoTotal;

                if (movimiento.CostoTotal.ToString().Contains("-"))
                {
                    egresos += movimiento.CostoTotal;
                }
                else
                {
                    ingresos += movimiento.CostoTotal;
                }
            }

            lblTotal.Content = "Total en caja: $" + costoTotal.ToString();
            lblEgresos.Content = "Total de egresos: $" + egresos.ToString();
            lblIngresos.Content = "Total de ingresos: $" + ingresos.ToString();
        }

        private void FiltrarMovimientos(DateTime fechaSeleccionada)
        {
            List<MovimientoElemento> movimientosParaFiltrar = elementos;
            List<MovimientoElemento> movimientosOrdenados = new List<MovimientoElemento>();

            movimientosOrdenados = OrdenarMovimientos(movimientosParaFiltrar, fechaSeleccionada);
            elementosOrdenados = movimientosOrdenados;
            lstMovimientos.ItemsSource = elementosOrdenados;

            VerificarExistenciaMovimientos();
            CalcularCostos(movimientosOrdenados);
        }

        private List<MovimientoElemento> OrdenarMovimientos(List<MovimientoElemento> movimientos, DateTime fechaSeleccionada)
        {
            if (fechaSeleccionada != null)
            {
                var movimientosDelDia = movimientos.Where(m => m.Fecha.Date == fechaSeleccionada.Date).ToList();
                var movimientosOrdenados = movimientosDelDia.OrderBy(m => m.Fecha).ToList();
                movimientos = movimientosOrdenados;
            }

            return movimientos;
        }

        private void VerificarExistenciaMovimientos()
        {
            if (lstMovimientos == null || lstMovimientos.Items.IsEmpty)
            {
                lstMovimientos.Visibility = Visibility.Collapsed;
                lblMovimientosError.Content = "No se han registrado movimientos";
                lblMovimientosError.Visibility = Visibility.Visible;
                DeshabilitarGenerarReporte();
            }
            else
            {
                lstMovimientos.Visibility = Visibility.Visible;
                lblMovimientosError.Visibility = Visibility.Collapsed;
                HabilitarGenerarReporte();
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

        private DateTime RecuperarFechaSeleccionada()
        {
            DateTime fechaSeleccionada = (DateTime)dpFecha.SelectedDate;
            return fechaSeleccionada;
        }

        private DateTime RecuperarFechaActual()
        {
            DateTime fechaActual = DateTime.Now;
            return fechaActual;
        }

        private void GenerarReporte()
        {
            string nombreArchivo = "reporteMovimientos-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = System.IO.Path.Combine(folder, nombreArchivo);
            int numColumnasTabla = 3;
            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD);

            Document documento = new Document();

            try
            {
                PdfWriter.GetInstance(documento, new FileStream(outputPath, FileMode.Create));
                documento.Open();

                iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("Corte de caja", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                documento.Add(titulo);

                documento.Add(new iTextSharp.text.Paragraph(" "));

                iTextSharp.text.Paragraph fecha = new iTextSharp.text.Paragraph("Fecha de generación: " + RecuperarFechaActual().ToString());
                documento.Add(fecha);

                iTextSharp.text.Paragraph ingresos = new iTextSharp.text.Paragraph(lblIngresos.Content.ToString());
                documento.Add(ingresos);

                iTextSharp.text.Paragraph egresos = new iTextSharp.text.Paragraph(lblEgresos.Content.ToString());
                documento.Add(egresos);

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

        private void ValidarSeleccionDeFecha()
        {
            DateTime fechaSeleccionada = RecuperarFechaSeleccionada();
            DateTime fechaActual = RecuperarFechaActual();

            if (fechaSeleccionada > fechaActual)
            {
                dpFecha.SelectedDate = fechaActual;
                lblFechaError.Content = "No se puede seleccionar una fecha posterior a la actual";
                lblFechaError.Visibility = Visibility.Visible;
                DeshabilitarGenerarReporte();
                lstMovimientos.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblFechaError.Visibility = Visibility.Collapsed;
                lstMovimientos.Visibility = Visibility.Visible;
                FiltrarMovimientos(fechaSeleccionada);
            }
        }

        private void DeshabilitarGenerarReporte()
        {
            btnGenerarReporte.IsEnabled = false;
            lblGenerarReporte.IsEnabled = false;
        }

        private void HabilitarGenerarReporte()
        {
            btnGenerarReporte.IsEnabled = true;
            lblGenerarReporte.IsEnabled = true;
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

        private void DpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidarSeleccionDeFecha();
        }

        private void DpFecha_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
    }
}