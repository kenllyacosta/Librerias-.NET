using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Metodos_de_extension
{
    public static class Metodos
    {
        /// <summary>
        /// Retorna todo el string consultado
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="enlace">
        /// Enlace de la consulta a la API
        /// </param>
        /// <returns></returns>
        public static string ConsultaApi(this object obj, string enlace)
        {
            string resultado = default;

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(enlace.Trim());

            try
            {
                var response = myReq.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    resultado = reader.ReadToEnd();
                }
            }
            catch { }

            return resultado;
        }

        /// <summary>
        /// Retorna una lista del tipo a consultar.
        /// Referencia: http://json2csharp.com/
        /// Convierte un objeto JSon a una clase de C#
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="enlace"></param>
        /// <returns></returns>
        public static TEntity ConsultaApi<TEntity>(this object obj, string enlace)
        {
            TEntity resultado = default;

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(enlace.Trim());

            try
            {
                var response = myReq.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    resultado = JsonConvert.DeserializeObject<TEntity>(reader.ReadToEnd());
                }
            }
            catch { }

            return resultado;
        }
    }
}
