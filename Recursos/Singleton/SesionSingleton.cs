using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoughMinder___Client.Recursos.Singleton
{
    internal class SesionSingleton
    {
        private static SesionSingleton instance;
        public string Usuario { get; private set; }
        public string Nombre { get; private set; }
        public int Puesto { get; private set; }

        private SesionSingleton() { }

        public static SesionSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SesionSingleton();
                }
                return instance;
            }
        }

        public void SetUsuario(string usuario)
        {
            Usuario = usuario;
        }

        public void SetNombre(string nombre)
        {
            Nombre = nombre;
        }

        public void SetPuesto(int puesto)
        {
            Puesto = puesto;
        }

        public void Clear()
        {
            Usuario = null;
            Nombre = null;
            Puesto = 0;
        }
    }
}
