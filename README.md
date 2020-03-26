# Librerias .NET
Conjunto de librerías de uso común para el desarrollo de aplicaciones para .NET

Consiste en un proyecto de consola y librerias de clase del .net Standard, en el proyecto de consola creado con .net core se prueban las funcionalidades de cada una de las librerías existentes.

<h2>Librerias agregadas</h2>
<h3>Métodos de extensión:</h3>
<p>Consiste en un proyecto de libreria de clases del .NET standard el cual tiene métodos de extensión reutilizables, éstos métodos extienden la clase <b>Object</b> de la cual heredan todas las clases, esto significa que los métodos estarán disponibles en todas las clases existente y futuras. <a href='https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods' target="_blank">Más información sobre los métodos de extensión</a> <br/><br/>
<ul>
  <li><b>ConsultaApi:</b> Consulta y retorna los datos de una API.</li>
  <li><b>EnvioDeCorreo:</b></b> Realiza el envío de correo a uno o varios destinatarios.</li>
  <li><b>DividirCadena:</b> Divide un cadena de caracteres.</li>
  <li><b>Encrypt:</b> Encripta una cadena de caracteres.</li>
  <li><b>Decrypt:</b> Desencripta una cadena de caracteres.</li>
  <li><b>ToByte:</b> Convierte a byte nulo.</b></li>
  <li><b>ToByteNotNull:</b> Convierte a byte no nulo.</li>
  <li><b>ToByteArray:</b> Convierte a Array de Bytes.</li>
  <li><b>ToShort:</b> Convierte a Short.</li>
  <li><b>ToInt:</b> Convierte a Int.</li>
  <li><b>ToIntNotNull:</b> Convierte a Int no nulo.</li>
  <li><b>ToDouble:</b> Convierte a double.</li>
  <li><b>ToDoubleNotNull:</b> Convierte a double no nulo.</li>
  <li><b>ToDecimal:</b> Convierte a decimal.</li>
  <li><b>ToDateTime:</b> Convierte a DateTime.</li>
  <li><b>ToDateTimeNoNull:</b> Convierte a DateTime no nulo.</li>
  <li><b>ToDate:</b> Convierte a Date.</li>
  <li><b>ToTimeSpan:</b> Convierte a TimeSpan.</li>
  <li><b>ToBool:</b> Convierte a bool.</li>
  <li><b>ToBoolNotNull:</b> Convierte a bool no nulo.</li>
  <li><b>StringLeft:</b> Envia toda la cadena de texto a la izquierda.</li>
  <li><b>StringRight:</b> Envia toda la cadena de texto a la derecha</li>
  <li><b>StringCenter:</b> Envia toda la cadena de texto al centro.</li>
  <li><b>ValidarDocumento:</b> Valida un RNC o Cédula.</li>
  <li><b>IpPublica:</b> Retorna la ip publica actual.</li>
  <li><b>Numaletra:</b> Convierte cualquier número a su representación en letras.</li>
</ul>
<a href="https://github.com/kenllyacosta/Librerias-.NET/blob/master/Metodos_de_extension/Metodos.cs">Listado de métodos agregados</><p/>

<h3>RepositorioEF</h3>
<p>Consiste en un proyecto de libreria de clases del .Net Framwork para acceder a cualquier fuente de datos usando EntityFramwork o T-SQL</p>
<p>Métodos de Repositorio&lt;TEntity&gt;</p>
<ul>
  <li><b>Create:</b> Este método crea un registro nuevo en la tabla.</li>
  <li><b>Retrieve:</b> Este método retorna un listado de registros desde la tabla.</li>
  <li><b>Update:</b> Este actualiza el registro en la tabla.</li>
  <li><b>Delete:</b> Este método elimina un registro de la tabla.</li>
</ul>
<p>Métodos de Repositorio</p>
<ul>
  <li><b>QuerySQL:</b> Este método acepta una sentencia T-SQL para trabajar con bases de datos Microsoft SQL.</li>
  <li><b>QueryMySQL:</b> Este método acepta una sentencia T-SQL para trabajar con bases de datos MySQL.</li>
  <li><b>QueryCommonSQL:</b> Este método acepta una sentencia T-SQL para trabajar con bases de datos de otros tipos.</li>
  <li><b>MakeEntityFromDataTable:</b> Este método convierte desde la consulta T-SQL a su tipo de dato.</li>
  <li><b>GetProviderFactoryClasses:</b> Este método obtiene todos los proveedores instalados.</li>
  <li><b>GetConnectionStringByProvider:</b> Este método obtiene la cadena de conexión por cada proveedor de datos.</li>
</ul>
<em>Nota: todos poseen un evento Exception que notifica en caso haya pasado algún error en el componente.</em>
<a href="https://github.com/kenllyacosta/Librerias-.NET/blob/master/RepositorioEF/Repositorio.cs">Listado de métodos agregados</><p/>
