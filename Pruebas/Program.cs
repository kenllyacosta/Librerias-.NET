using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net.Mail;
using Metodos_de_extension;

namespace Pruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hola Mundo!");
            Console.ReadLine();
        }

        private static void PruebasRepositorioEFCore()
        {
            //Instalar el Entity Framework Core
            //Aquí probamos nuestro repositorio enviandole cualquier contexto
            DbContext contextoDePrueba = new DbContext("***Your ConectionString***");

            //Podemos utilizarlo de las siguientes formas
            //Uso 1
            //RepositorioEF.Repositorio<TEntity> repositorio = new RepositorioEF.Repositorio<TEntity>(contextoDePrueba);
            //repositorio.Create();
            //repositorio.Retrieve();
            //repositorio.Update();
            //repositorio.Delete();
            //repositorio.Filter();
            //repositorio.Dispose();

            //contextoDePrueba = new DbContext("***Your ConectionString***");
            //Uso 2
            //using (RepositorioEF.Repositorio<TEntity> repositorio = new RepositorioEF.Repositorio<TEntity>(contextoDePrueba))
            //{
            //    repositorio.Create();
            //    repositorio.Retrieve();
            //    repositorio.Update();
            //    repositorio.Delete();
            //    repositorio.Filter();
            //}
        }

        private static void PruebasRepositorioEF()
        {
            //Proveedores
            var proveedoresInstalados = RepositorioEF.Repositorio.GetProviderFactoryClasses();
            for (int i = 0; i < proveedoresInstalados.Rows.Count; i++)
            {
                Console.WriteLine(proveedoresInstalados.Rows[i].ItemArray[0]);
                Console.WriteLine(proveedoresInstalados.Rows[i].ItemArray[1]);
                Console.WriteLine(proveedoresInstalados.Rows[i].ItemArray[2]);
                Console.WriteLine(proveedoresInstalados.Rows[i].ItemArray[3]);
            }

            //Cadena de conexión por cada proveedor
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("System.Data.SqlClient"));
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("System.Data.OracleClient"));
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("System.Data.Odbc"));
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("System.Data.OleDb"));
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("System.Data.SqlServerCe.4.0"));
            Console.WriteLine(RepositorioEF.Repositorio.GetConnectionStringByProvider("MySql.Data.MySqlClient"));

            //Consulta con T-SQL            
            var datosSQL = RepositorioEF.Repositorio.QuerySQL("Select * From NombreTabla", "***Your ConectionString***");
            var datosMySQL = RepositorioEF.Repositorio.QueryMySQL("Select * From NombreTabla", "***Your ConectionString***");
            var datosSQLDataCommon = RepositorioEF.Repositorio.QueryCommonSQL("Select * From NombreTabla", "***Your ConectionString***", "System.Data.SqlClient");

            //En todos los caso de consulta mediante T-SQL podemos convertir los datos retornados al tipo correspondiente usando
            //TEntity datosConSutipoSQL = RepositorioEF.Repositorio.MakeEntityFromDataTable<TEntity>(datosSQL);
            //TEntity datosConSutipoMySQL = RepositorioEF.Repositorio.MakeEntityFromDataTable<TEntity>(datosMySQL);
            //TEntity datosConSutipoSQLDataCommon = RepositorioEF.Repositorio.MakeEntityFromDataTable<TEntity>(datosSQLDataCommon);

            //Instalar el Entity Framework 6.x o posterior
            //Aquí probamos nuestro repositorio enviandole cualquier contexto
            DbContext contextoDePrueba = new DbContext("***Your ConectionString***");

            //Podemos utilizarlo de las siguientes formas
            //Uso 1
            //RepositorioEF.Repositorio<TEntity> repositorio = new RepositorioEF.Repositorio<TEntity>(contextoDePrueba);            
            //repositorio.Create();
            //repositorio.Retrieve();
            //repositorio.Update();
            //repositorio.Delete();
            //repositorio.Filter();
            //repositorio.Dispose();

            //contextoDePrueba = new DbContext("***Your ConectionString***");
            //Uso 2
            //using (RepositorioEF.Repositorio<TEntity> repositorio = new RepositorioEF.Repositorio<TEntity>(contextoDePrueba))
            //{
            //    repositorio.Create();
            //    repositorio.Retrieve();
            //    repositorio.Update();
            //    repositorio.Delete();
            //    repositorio.Filter();
            //}
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
