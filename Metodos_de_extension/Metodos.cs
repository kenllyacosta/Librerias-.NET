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
using System.Globalization;

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

        /// <summary>
        /// Recibe Cédula y valida si es correcta
        /// </summary>
        /// <param name="ced"></param>
        /// <returns></returns>
        private static bool Valida_Cedula(this object valor, string ced)
        {
            string c = ced.Replace("-", "");
            string Cedula = c.Substring(0, c.Length - 1);
            string Verificador = c.Substring(c.Length - 1, 1);
            decimal suma = 0;

            int mod, res;
            for (int i = 0; i < Cedula.Length; i++)
            {
                if ((i % 2) == 0) mod = 1;
                else mod = 2;

                if (int.TryParse(Cedula.Substring(i, 1), out int dig))
                    res = dig * mod;
                else
                    return false;

                if (res > 9)
                {
                    res = Convert.ToInt32(res.ToString().Substring(0, 1)) +
                    Convert.ToInt32(res.ToString().Substring(1, 1));
                }
                suma += res;

            }

            decimal el_numero = (10 - (suma % 10)) % 10;
            if ((el_numero.ToString() == Verificador) && (Cedula.Substring(0, 3) != "000"))
                return true;
            else
            {
                Excepcion?.Invoke(new object(), new ExceptionEvenArgs() { Message = $"El documento '{ced}' no es valido\n" + "el dígito verificador debió ser " + el_numero.ToString()});
                return false;
            }
        }

        /// <summary>
        /// Recibe RNC y valida si es correcto
        /// </summary>
        /// <param name="rnc"></param>
        /// <returns></returns>        
        private static bool Valida_Rnc(this object valor, string rnc)
        {
            int iDigital, p, t, d, r;
            string sCon;

            rnc = rnc.Replace("-", "");

            if (rnc.Length < 9)
                return false;

            iDigital = int.Parse(rnc.Substring(rnc.Length - 1));
            sCon = "79865432";
            t = 0;
            for (int j = 0; j < 8; j++)
            {
                p = int.Parse(rnc.Substring(j, 1)) * int.Parse(sCon.Substring(j, 1));
                t += p;
            }

            r = t % 11;
            d = 11 - r;
            switch (r)
            {
                case 0:
                    d = 2;
                    break;
                case 1:
                    d = 1;
                    break;
            }

            if (iDigital != d)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Recibe Cédula o RNC y valida si es correcto
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        public static bool ValidarDocumento(this object valor, string documento)
        {
            bool Resultado = false;
            if (string.IsNullOrWhiteSpace(documento))
                return Resultado;
            documento = documento.Trim();
            documento = documento.Replace("-", "");
            switch (documento.Length)
            {
                case 9: //Es un RNC
                    Resultado = Resultado.Valida_Rnc(documento);
                    break;
                case 11: //Es una cédula
                    Resultado = Resultado.Valida_Cedula(documento);
                    break;
                default: //El número es inválido
                    Resultado = false;
                    break;
            }
            return Resultado;
        }

        public static string IpPublica(this object valor, string enlace = "http://icanhazip.com")
        {
            string resultado = "";
            try
            {
                resultado = new WebClient().
                DownloadString(enlace);
            }
            catch { }
            return resultado;
        }

        public static string Numaletra(this object valor, string numero)
        {
            return Numalet.ToString(numero);
        }

        public static string Numaletra(this object valor, int numero)
        {
            return Numalet.ToString(numero);
        }

        public static string Numaletra(this object valor, string numero, CultureInfo cultura)
        {
            Numalet let = new Numalet();
            let.CultureInfo = cultura;
            return let.ToCustomString(numero);
        }

        public static string Numaletra(this object valor, int numero, CultureInfo cultura)
        {
            Numalet let = new Numalet();
            let.CultureInfo = cultura;
            return let.ToCustomString(numero);
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

    public sealed class Numalet
    {
        private const int UNI = 0, DIECI = 1, DECENA = 2, CENTENA = 3;
        private static string[,] _matriz = new string[CENTENA + 1, 10]
            {
                {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
                {" diez"," once"," doce"," trece"," catorce"," quince"," dieciseis"," diecisiete"," dieciocho"," diecinueve"},
                {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
                {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
            };

        #region Miembros estáticos

        private const Char sub = (Char)26;
        //Cambiar acá si se quiere otro comportamiento en los métodos de clase
        public const String SeparadorDecimalSalidaDefault = "con";
        public const String MascaraSalidaDecimalDefault = "00'/100.-'";
        public const Int32 DecimalesDefault = 2;
        public const Boolean LetraCapitalDefault = false;
        public const Boolean ConvertirDecimalesDefault = false;
        public const Boolean ApocoparUnoParteEnteraDefault = false;
        public const Boolean ApocoparUnoParteDecimalDefault = false;

        #endregion

        #region Propiedades de instancia

        private Int32 _decimales = DecimalesDefault;
        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
        private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
        private Int32 _posiciones = DecimalesDefault;
        private String _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private Boolean _esMascaraNumerica = true;
        private Boolean _letraCapital = LetraCapitalDefault;
        private Boolean _convertirDecimales = ConvertirDecimalesDefault;
        private Boolean _apocoparUnoParteEntera = false;
        private Boolean _apocoparUnoParteDecimal;

        /// <summary>
        /// Indica la cantidad de decimales que se pasarán a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
        public Int32 Decimales
        {
            get { return _decimales; }
            set
            {
                if (value > 10) throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                _decimales = value;
            }
        }

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public String SeparadorDecimalSalida
        {
            get { return _separadorDecimalSalida; }
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                if (value.Trim().IndexOf(" ") > 0)
                    _apocoparUnoParteEntera = true;
                else _apocoparUnoParteEntera = false;
            }
        }

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public String MascaraSalidaDecimal
        {
            get
            {
                if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    return _mascaraSalidaDecimal;
                else return "";
            }
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                //que haya al principio de la cadena, y también si es una máscara numérica
                int i = 0;
                while (i < value.Length
                    && (value[i] == '0')
                        | value[i] == '#')
                    i++;
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else _esMascaraNumerica = false;
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'"
                        + value.Substring(_posiciones)
                        .Replace("''", sub.ToString())
                        .Replace("'", String.Empty)
                        .Replace(sub.ToString(), "'") + "'";
                else
                    _mascaraSalidaDecimalInterna = value
                        .Replace("''", sub.ToString())
                        .Replace("'", String.Empty)
                        .Replace(sub.ToString(), "'");
            }
        }

        /// <summary>
        /// Indica si la primera letra del resultado debe estár en mayúscula
        /// </summary>
        public Boolean LetraCapital
        {
            get { return _letraCapital; }
            set { _letraCapital = value; }
        }

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public Boolean ConvertirDecimales
        {
            get { return _convertirDecimales; }
            set
            {
                _convertirDecimales = value;
                _apocoparUnoParteDecimal = value;
                if (value)
                {// Si la máscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                        MascaraSalidaDecimal = "";
                }
                else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                    //Si no hay máscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            }
        }

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public Boolean ApocoparUnoParteEntera
        {
            get { return _apocoparUnoParteEntera; }
            set { _apocoparUnoParteEntera = value; }
        }

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public Boolean ApocoparUnoParteDecimal
        {
            get { return _apocoparUnoParteDecimal; }
            set { _apocoparUnoParteDecimal = value; }
        }

        #endregion

        #region Constructores

        public Numalet()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public Numalet(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
        {
            if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
                this.MascaraSalidaDecimal = MascaraSalidaDecimal;
            if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
                _separadorDecimalSalida = SeparadorDecimalSalida;
            _letraCapital = LetraCapital;
            _convertirDecimales = ConvertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public String ToCustomString(Double Numero)
        { return Convertir((Decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, _apocoparUnoParteDecimal); }

        public String ToCustomString(String Numero)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
                return ToCustomString(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public String ToCustomString(Decimal Numero)
        { return ToString(Convert.ToDouble(Numero)); }

        public String ToCustomString(Int32 Numero)
        { return Convertir((Decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, false); }


        #endregion

        #region Conversores estáticos

        public static String ToString(Int32 Numero)
        {
            return Convertir((Decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static String ToString(Double Numero)
        { return Convertir((Decimal)Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault); }

        public static String ToString(String Numero, CultureInfo ReferenciaCultural)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
                return ToString(dNumero);
            else throw new ArgumentException("'" + Numero + "' no es un número válido.");
        }

        public static String ToString(String Numero)
        {
            return Numalet.ToString(Numero, CultureInfo.CurrentCulture);
        }

        public static String ToString(Decimal Numero)
        { return ToString(Convert.ToDouble(Numero)); }

        #endregion

        private static String Convertir(Decimal Numero, Int32 Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, Boolean EsMascaraNumerica, Boolean LetraCapital, Boolean ConvertirDecimales, Boolean ApocoparUnoParteEntera, Boolean ApocoparUnoParteDecimal)
        {
            Int64 Num;
            Int32 terna, pos, centenaTerna, decenaTerna, unidadTerna, iTerna;
            String numcad, cadTerna;
            StringBuilder Resultado = new StringBuilder();

            Num = (Int64)Math.Abs(Numero);

            if (Num >= 1000000000000 || Num < 0) throw new ArgumentException("El numero '" + Numero.ToString() + "' excedio los limites del conversor: [0;1.000.000.000.000)");
            if (Num == 0)
                Resultado.Append(" cero");
            else
            {
                numcad = Num.ToString();
                iTerna = 0;
                pos = numcad.Length;

                do //Se itera por las ternas de atrás para adelante
                {
                    iTerna++;
                    cadTerna = String.Empty;
                    if (pos >= 3)
                        terna = Int32.Parse(numcad.Substring(pos - 3, 3));
                    else
                        terna = Int32.Parse(numcad.Substring(0, pos));

                    centenaTerna = (Int32)(terna / 100);
                    decenaTerna = terna - centenaTerna * 100;
                    unidadTerna = (decenaTerna - (Int32)(decenaTerna / 10) * 10);

                    if ((decenaTerna > 0) && (decenaTerna < 10))
                        cadTerna = _matriz[UNI, unidadTerna] + cadTerna;
                    else if ((decenaTerna >= 10) && (decenaTerna < 20))
                        cadTerna = cadTerna + _matriz[DIECI, decenaTerna - (Int32)(decenaTerna / 10) * 10];
                    else if (decenaTerna == 20)
                        cadTerna = cadTerna + " veinte";
                    else if ((decenaTerna > 20) && (decenaTerna < 30))
                        cadTerna = " veinti" + _matriz[UNI, unidadTerna].Substring(1, _matriz[UNI, unidadTerna].Length - 1);
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                        if (unidadTerna != 0)
                            cadTerna = _matriz[DECENA, (Int32)(decenaTerna / 10)] + " y" + _matriz[UNI, unidadTerna] + cadTerna;
                        else
                            cadTerna += _matriz[DECENA, (Int32)(decenaTerna / 10)];

                    switch (centenaTerna)
                    {
                        case 1:
                            if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                            else cadTerna = " cien" + cadTerna;
                            break;
                        case 5:
                        case 7:
                        case 9:
                            cadTerna = _matriz[CENTENA, (Int32)(terna / 100)] + cadTerna;
                            break;
                        default:
                            if ((Int32)(terna / 100) > 1) cadTerna = _matriz[UNI, (Int32)(terna / 100)] + "cientos" + cadTerna;
                            break;
                    }
                    //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                    if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                        cadTerna = cadTerna.Replace("veintiuno", "veintiun");
                    else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    //Acentúo 'dieciseís', 'veintidós', 'veintitrés' y 'veintiséis'
                    else if (decenaTerna == 16) cadTerna = cadTerna.Replace("dieciseis", "dieciseis");
                    else if (decenaTerna == 22) cadTerna = cadTerna.Replace("veintidos", "veintidos");
                    else if (decenaTerna == 23) cadTerna = cadTerna.Replace("veintitres", "veintitres");
                    else if (decenaTerna == 26) cadTerna = cadTerna.Replace("veintiseis", "veintiseis");
                    //Reemplazo 'uno' por 'un' si no es en las únidades o si se solicító apocopar (si _apocoparUnoParteEntera es verdadero) 

                    switch (iTerna)
                    {
                        case 3:
                            if (Num < 2000000) cadTerna += " millon";
                            else cadTerna += " millones";
                            break;
                        case 2:
                        case 4:
                            if (terna > 0) cadTerna += " mil";
                            break;
                    }
                    Resultado.Insert(0, cadTerna);
                    pos = pos - 3;
                } while (pos > 0);
            }
            //Se agregan los decimales si corresponde
            if (Decimales > 0)
            {
                Resultado.Append(" " + SeparadorDecimalSalida + " ");
                Int32 EnteroDecimal = (Int32)Math.Round((Double)(Numero - (Int64)Numero) * Math.Pow(10, Decimales), 0);
                if (ConvertirDecimales)
                {
                    Boolean esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                    Resultado.Append(Convertir((Decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                        + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
                }
                else
                    if (EsMascaraNumerica) Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                else Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
            }
            //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
            if (LetraCapital)
                return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
            else
                return Resultado.ToString().Substring(1);
        }
    }
}
