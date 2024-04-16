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
    /// Interaction logic for Empleados.xaml
    /// </summary>
    public partial class Empleados : Page
    {
        private List<EmpleadoItem> listaEmpleadosCompleta;

        public Empleados()
        {
            InitializeComponent();
            CargarEmpleados();
        }


        public class EmpleadoItem
        {
            public string Nombre { get; set; }
            public string RFC { get; set; }

        }


        private void CargarEmpleados()
        {
            Dictionary<string, string> dictEmpleados = RecuperarEmpleados();

            listaEmpleadosCompleta = new List<EmpleadoItem>(); // Inicializamos la lista completa de empleados

            foreach (var empleado in dictEmpleados)
            {
                listaEmpleadosCompleta.Add(new EmpleadoItem { Nombre = empleado.Key, RFC = empleado.Value });
            }

            // Mostramos la lista completa de empleados en el ListBox
            lstEmpleados.ItemsSource = listaEmpleadosCompleta;
        }



        private Dictionary<string, string> RecuperarEmpleados()
        {
            Dictionary<string, string> listaEmpleados = new Dictionary<string, string>();
            try
            {
                DoughMinderServicio.EmpleadoClient cliente = new DoughMinderServicio.EmpleadoClient();
                listaEmpleados = cliente.RecuperarEmpleados();

                if (listaEmpleados == null)
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

            return listaEmpleados;
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

        private void Empleados_Loaded(object sender, RoutedEventArgs e)
        {
            lstEmpleados.MouseDoubleClick += MostrarEmpleado;
        }


        private void MostrarEmpleado(object sender, MouseButtonEventArgs e)
        {
            EmpleadoItem empleadoSeleccionado = lstEmpleados.SelectedItem as EmpleadoItem;

            if (empleadoSeleccionado != null)
            {
                RegistrarEmpleado registrarEmpleadoPage = new RegistrarEmpleado(false, empleadoSeleccionado.RFC);
                NavigationService.Navigate(registrarEmpleadoPage);
            }
        }

        private void AbrirRegistrarEmpleado(object sender, MouseButtonEventArgs e)
        {
            RegistrarEmpleado registrarEmpleadoPage = new RegistrarEmpleado(true, "");
            NavigationService.Navigate(registrarEmpleadoPage);
        }

        private void RegresarVentanaAnterior(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BuscarEnLista(object sender, TextChangedEventArgs e)
        {
            string textoBusqueda = tbBusqueda.Text.ToLower();

            if (listaEmpleadosCompleta != null) // Verificamos que la lista completa de empleados no sea nula
            {
                // Filtramos los empleados cuyo nombre o RFC contenga el texto de búsqueda
                var empleadosFiltrados = listaEmpleadosCompleta.Where(emp => emp.Nombre.ToLower().Contains(textoBusqueda) || emp.RFC.ToLower().Contains(textoBusqueda)).ToList();

                // Actualizamos la lista de empleados mostrada en el ListBox con los empleados filtrados
                lstEmpleados.ItemsSource = empleadosFiltrados;
            }
        }

    }
}
