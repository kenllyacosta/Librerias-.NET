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
            //Me inscribo al evento por si pasa alguna exepción en el componente "Métodos de Extensión"
            Metodos.Excepcion += Metodos_Excepcion;

            Console.WriteLine(args.Numaletra(123456));
            Console.WriteLine(args.Numaletra("123456"));

            args.ValidarDocumento("001-1653664-1");            

            var datos = args.ConsultaApi("https://ticapacitacion.com/cats");
            var datosDeGatos = args.ConsultaApi<List<Cat>>("https://ticapacitacion.com/cats15151");//Disparará un exepción porque este enlace no existe
            List<MailAddress> correosDestino = new List<MailAddress>
            {
                new MailAddress("kenllyacosta@hotmail.com", "Kenlly Acosta Gmail"),
                new MailAddress("kenllyacosta@gmail.com", "Kenlly Acosta Hotmail")
            };
            args.EnvioDeCorreo(correosDestino, null, null, null, "tucorreo@gmail.com", "tuclave", "Correo desde las librerías .NET - Kenlly Acosta", "Test", "Hola Mundo en texto plano y <b>Hola Mundo en HTML</b>", true, MailPriority.High);

            //Quito el manejo de exepciones de la memoria porque no lo necesitaré ☺
            Metodos.Excepcion -= Metodos_Excepcion;
        }

        /// <summary>
        /// Este código se ejecuta si sucede una exepción en el componente externo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void Metodos_Excepcion(object sender, ExceptionEvenArgs ex)
        {
            Console.WriteLine(ex.Message);
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
