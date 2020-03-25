using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Metodos_de_extension
{
    //Crear delegado para el manejo de las exceptions
    public delegate void ExceptionEventHandler(object sender, ExceptionEvenArgs ex);
    public static class Metodos
    {
        public static event ExceptionEventHandler Excepcion;

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
            catch (Exception ex)
            {
                EventLog.WriteEntry("ConsultaApi", ex.Message, EventLogEntryType.Error);
                if (ex.InnerException != null)
                {
                    EventLog.WriteEntry("ConsultaApi", ex.InnerException.Message, EventLogEntryType.Error);
                    if (ex.InnerException.InnerException != null)
                    {
                        EventLog.WriteEntry("ConsultaApi", ex.InnerException.InnerException.Message, EventLogEntryType.Error);
                        if (ex.InnerException.InnerException.InnerException != null)
                            EventLog.WriteEntry("ConsultaApi", ex.InnerException.InnerException.InnerException.Message, EventLogEntryType.Error);
                    }
                }
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
                return resultado;
            }

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
            catch (Exception ex)
            {
                EventLog.WriteEntry("ConsultaApi", ex.Message, EventLogEntryType.Error);
                if (ex.InnerException != null)
                {
                    EventLog.WriteEntry("ConsultaApi", ex.InnerException.Message, EventLogEntryType.Error);
                    if (ex.InnerException.InnerException != null)
                    {
                        EventLog.WriteEntry("ConsultaApi", ex.InnerException.InnerException.Message, EventLogEntryType.Error);
                        if (ex.InnerException.InnerException.InnerException != null)
                            EventLog.WriteEntry("ConsultaApi", ex.InnerException.InnerException.InnerException.Message, EventLogEntryType.Error);
                    }
                }
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
                return resultado;
            }

            return resultado;
        }

        /// <summary>
        /// Envia un correo con los parámetros enviados a continuación a un o más destinatarios con 
        /// uno o varios archivos adjunto
        /// </summary>
        /// <param name="correosPara">
        /// Listado de correos destino
        /// </param>
        /// <param name="correosParaCopia">
        /// Listado de correos a los cuales les llegará una copia del mensaje
        /// </param>
        /// <param name="correosParaCopiaOculta">
        /// Listado de correos a los cuales les llegará una copia oculta del mensaje
        /// </param>
        /// <param name="attachments">
        /// Listado de archivos adjuntos (La ruta y su extensión)
        /// </param>
        /// <param name="correoDesde">
        /// Correo desde cual se envia el mensaje
        /// </param>
        /// <param name="clave">
        /// Contraseña del correo para enviar
        /// </param>
        /// <param name="titulo">
        /// Título que aparecerá al recibir el mensaje
        /// </param>
        /// <param name="asunto">
        /// Asunto del mensaje
        /// </param>
        /// <param name="mensaje">
        /// El cuerpo del mensaje, este puede ser texto plano o formateado con etiquetas HTML
        /// </param>
        /// <param name="isHTML">
        /// Indica si en contenido a enviar es con formato HTML
        /// </param>
        /// <param name="mailPriority">
        /// Indica el nivel de prioridad del correo a enviar
        /// </param>
        /// <param name="host">
        /// Aquí pondremos nuestro host, ejemplos: smtp.gmail.com, smtp.live.com, smtp.aslan.com.do...
        /// </param>
        /// <param name="puerto">
        /// Puerto usado por el servidor de correo para enviar el mensaje, comunmente son: 25 o 587
        /// </param>
        /// <param name="enableSsl">
        /// Indica si el mensaje se enviará con SSL habilitado
        /// </param>
        /// <param name="useDefaultCredentials">
        /// Las credenciales a usar al autenticarnos en el servidor de correo
        /// </param>
        /// <returns></returns>
        public static bool EnvioDeCorreo(this object obj, IEnumerable<MailAddress> correosPara, IEnumerable<MailAddress> correosParaCopia, IEnumerable<MailAddress> correosParaCopiaOculta, IEnumerable<Attachment> attachments, string correoDesde, string clave, string titulo, string asunto, string mensaje, bool isHTML = true, MailPriority mailPriority = MailPriority.Normal, string host = "smtp.gmail.com", int puerto = 587, bool enableSsl = true, bool useDefaultCredentials = false)
        {
            MailMessage email = new MailMessage()
            {
                From = new MailAddress(correoDesde, titulo),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = isHTML,
                Priority = mailPriority
            };
            SmtpClient smtp = new SmtpClient
            {
                Host = host,
                Port = puerto,
                EnableSsl = enableSsl,
                UseDefaultCredentials = useDefaultCredentials,
                Credentials = new NetworkCredential(correoDesde, clave)
            };

            if (attachments != null)
                foreach (var item in attachments)
                    email.Attachments.Add(item);

            if (correosPara != null)
                foreach (var correo in correosPara)
                    email.To.Add(correo);

            if (correosParaCopia != null)
                foreach (var correo in correosParaCopia)
                    email.CC.Add(correo);

            if (correosParaCopiaOculta != null)
                foreach (var correo in correosParaCopiaOculta)
                    email.Bcc.Add(correo);

            try
            {
                smtp.Send(email);
                email.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("EnvioDeCorreo", ex.Message, EventLogEntryType.Error);
                if (ex.InnerException != null)
                {
                    EventLog.WriteEntry("EnvioDeCorreo", ex.InnerException.Message, EventLogEntryType.Error);
                    if (ex.InnerException.InnerException != null)
                    {
                        EventLog.WriteEntry("EnvioDeCorreo", ex.InnerException.InnerException.Message, EventLogEntryType.Error);
                        if (ex.InnerException.InnerException.InnerException != null)
                            EventLog.WriteEntry("EnvioDeCorreo", ex.InnerException.InnerException.InnerException.Message, EventLogEntryType.Error);
                    }
                }
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { InnerException = ex.InnerException, Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, TargetSite = ex.TargetSite });
                return false;
            }
        }

        public static string[] DividirCadena(this object obj, string cadena, int value)
        {
            string[] resultado = null;
            if (cadena != "")
            {
                int num = cadena.Count() / value;
                var subCadena = new string[num + 1];
                int numFijo = value;

                for (int i = 0; i < num + 1; i++)
                {
                    int numCararteres;
                    numCararteres = numFijo;
                    if (num == i)
                        numCararteres = cadena.Count() - (i) * numFijo;
                    subCadena[i] = cadena.Substring(i * numFijo, numCararteres);
                }
                resultado = subCadena;
            }
            return resultado;
        }

        public static string Encrypt(this object valor, string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(this object valor, string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static byte? ToByte(this object valor)
        {
            byte temp = 0;
            if (valor != null)
                byte.TryParse(valor.ToString(), out temp);

            byte? resultado = temp;
            return resultado;
        }

        public static byte ToByteNotNull(this object valor)
        {
            byte temp = 0;
            if (valor != null)
                byte.TryParse(valor.ToString(), out temp);

            byte resultado = temp;
            return resultado;
        }

        public static byte[] ToByteArray(this object valor)
        {
            if (valor == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, valor);
                return ms.ToArray();
            }
        }

        public static short? ToShort(this object valor)
        {
            short temp = 0;
            if (valor != null)
                short.TryParse(valor.ToString(), out temp);

            short? resultado = temp;
            return resultado;
        }

        public static int? ToInt(this object valor)
        {
            int temp = 0;
            if (valor != null)
                int.TryParse(valor.ToString(), out temp);

            int? resultado = temp;
            return resultado;
        }

        public static int ToIntNotNull(this object valor)
        {
            int temp = 0;
            if (valor != null)
                int.TryParse(valor.ToString(), out temp);

            int resultado = temp;
            return resultado;
        }

        public static double? ToDouble(this object valor)
        {
            double temp = 0;
            if (valor != null)
                double.TryParse(valor.ToString(), out temp);

            double? resultado = temp;
            return resultado;
        }

        public static double ToDoubleNotNull(this object valor)
        {
            double temp = 0;
            if (valor != null)
                double.TryParse(valor.ToString(), out temp);

            double resultado = temp;
            return resultado;
        }

        public static decimal? ToDecimal(this object valor)
        {
            decimal temp = 0;
            if (valor != null)
                decimal.TryParse(valor.ToString(), out temp);

            decimal? resultado = temp;
            return resultado;
        }

        public static DateTime? ToDateTime(this object valor)
        {
            DateTime? resultado = null;            
            if (valor != null)
            {
                DateTime.TryParse(valor.ToString(), out DateTime temp);
                resultado = temp;
            }
            return resultado;
        }

        public static DateTime ToDateTimeNoNull(this object valor)
        {
            DateTime resultado = DateTime.Now;            
            if (valor != null)
            {
                DateTime.TryParse(valor.ToString(), out DateTime temp);
                resultado = temp;
            }
            return resultado;
        }

        public static DateTime ToDate(this object valor)
        {
            DateTime resultado = DateTime.Now.Date;            
            if (valor != null)
            {
                DateTime.TryParse(valor.ToString(), out DateTime temp);
                resultado = temp;
            }
            return resultado;
        }

        public static TimeSpan ToTimeSpan(this object valor)
        {
            TimeSpan resultado = TimeSpan.Zero;
            if (valor != null)
            {
                TimeSpan.TryParse(valor.ToString(), out TimeSpan temp);
                resultado = temp;
            }
            return resultado;
        }

        public static bool? ToBool(this object valor)
        {
            bool temp = false;
            if (valor != null)
                bool.TryParse(valor.ToString(), out temp);

            bool? resultado = temp;
            return resultado;
        }

        public static bool ToBoolNotNull(this object valor)
        {
            bool temp = false;
            if (valor != null)
                bool.TryParse(valor.ToString(), out temp);

            bool resultado = temp;
            return resultado;
        }

        public static string StringLeft(this string s, int padValue)
        {
            return s.PadRight(padValue);
        }

        public static string StringRight(this string s, int padValue)
        {
            return s.PadLeft(padValue);
        }

        public static string StringCenter(this string stringToCenter, int totalLength)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length).PadRight(totalLength);
        }
    }

    public class ExceptionEvenArgs : EventArgs
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public MethodBase TargetSite { get; set; }
        public Exception InnerException { get; set; }        
    }
}
