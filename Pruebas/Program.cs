using System;
using System.Collections.Generic;
using Metodos_de_extension;

namespace Pruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            MetodosDeExtension(args);
            Console.WriteLine("Hola mundo!");
            Console.ReadLine();
        }

        /// <summary>
        /// Prueba de los métodos de extensión
        /// </summary>
        /// <param name="args">
        /// Objeto para usar con cualquier método creado en el proyecto Metodos_de_extension
        /// </param>
        private static void MetodosDeExtension(string[] args)
        {
            var datos = args.ConsultaApi("https://ticapacitacion.com/cats");
            var datosDeGatos = args.ConsultaApi<List<Cat>>("https://ticapacitacion.com/cats");
        }
    }

    public class Cat
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string WebSite { get; set; }
        public string Image { get; set; }
    }
}
