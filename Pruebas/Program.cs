using System;
using System.Collections.Generic;
using System.Net.Mail;
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
            //var datos = args.ConsultaApi("https://ticapacitacion.com/cats");
            //var datosDeGatos = args.ConsultaApi<List<Cat>>("https://ticapacitacion.com/cats");

            List<MailAddress> correosDestino = new List<MailAddress>
            {
                new MailAddress("kenllyacosta@hotmail.com", "Kenlly Acosta Gmail"),
                new MailAddress("kenllyacosta@gmail.com", "Kenlly Acosta Hotmail")
            };
            args.EnvioDeCorreo(correosDestino, null, null, null, "tucorreo@gmail.com", "tuclave", "Correo de las librerías .NET - Kenlly Acosta", "Test", "Hola Mundo en texto plano y <b>Hola Mundo en HTML</b>", true, MailPriority.High);
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
