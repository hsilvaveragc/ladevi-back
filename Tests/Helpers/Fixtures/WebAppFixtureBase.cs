using System.Diagnostics;
using System.Text;
using LadeviVentasApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Helpers.Fixtures
{
    /// <summary>
    /// Clase base abstracta para fixtures de pruebas de integración que proporciona funcionalidades
    /// para crear y gestionar una aplicación web de prueba con autenticación y comunicación HTTP.
    /// </summary>
    /// <remarks>
    /// Esta clase maneja:
    /// - La creación de una instancia de aplicación web para testing
    /// - Un cliente HTTP configurado para realizar requests
    /// - Gestión de tokens de autenticación JWT por usuario
    /// - Métodos para realizar requests HTTP autenticados
    /// - Serialización/deserialización JSON con formato camelCase
    /// </remarks>
    public abstract class WebAppFixtureBase : IDisposable
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Factory personalizada que crea una instancia de la aplicación web para testing.
        /// Configura el entorno de testing con base de datos en memoria y servicios mock.
        /// </summary>
        public CustomWebApplicationFactory<Program> Factory { get; set; }

        /// <summary>
        /// Cliente HTTP configurado para realizar requests a la aplicación de testing.
        /// Automáticamente configurado con la URL base de la aplicación.
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// Generador de enlaces que permite crear URLs para acciones de controladores.
        /// Utilizado para generar rutas dinámicas en los tests.
        /// </summary>
        public LinkGenerator LinkGenerator { get; }

        /// <summary>
        /// Usuario actualmente "logueado" para los requests HTTP.
        /// Se usa como clave para buscar el token JWT correspondiente en el diccionario Tokens.
        /// Si está vacío o null, los requests se envían sin autenticación.
        /// </summary>
        /// <example>
        /// fixture.CurrentUser = "admin"; // Usará el token del usuario admin
        /// fixture.CurrentUser = null;    // Requests sin autenticación
        /// </example>
        public string CurrentUser { get; set; }

        /// <summary>
        /// Diccionario que almacena tokens JWT por alias de usuario.
        /// La clave es el alias del usuario (ej: "admin", "supervisor") 
        /// y el valor es el token JWT válido para realizar requests autenticados.
        /// </summary>
        /// <example>
        /// Tokens["admin"] = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// Tokens["supervisor"] = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// </example>
        public Dictionary<string, string> Tokens = new Dictionary<string, string>();

        /// <summary>
        /// Constructor que inicializa la aplicación web de testing y sus dependencias.
        /// Crea el factory, cliente HTTP y generador de enlaces necesarios para los tests.
        /// </summary>
        protected WebAppFixtureBase()
        {
            Factory = new CustomWebApplicationFactory<Program>();
            Client = Factory.CreateClient();
            LinkGenerator = Factory.Services.GetRequiredService<LinkGenerator>();
        }

        /// <summary>
        /// Libera los recursos utilizados por la aplicación de testing.
        /// Llamado automáticamente por xUnit al finalizar los tests.
        /// </summary>
        public void Dispose()
        {
            Client?.Dispose();
            Factory?.Dispose();
        }

        /// <summary>
        /// Realiza un request HTTP a un endpoint de controlador y devuelve la respuesta como JObject.
        /// </summary>
        /// <typeparam name="TControllerType">Tipo del controlador al que se realizará el request</typeparam>
        /// <param name="action">Nombre del método/acción del controlador a llamar</param>
        /// <param name="shouldSucceed">Si es true, lanza excepción si la respuesta no es exitosa. 
        /// Si es false, permite respuestas de error para testing de casos negativos</param>
        /// <param name="routeValues">Parámetros de ruta (ej: {id = 1} para endpoints como /api/users/{id})</param>
        /// <param name="bodyData">Datos a enviar en el cuerpo del request (se serializa como JSON)</param>
        /// <param name="method">Método HTTP (GET, POST, PUT, DELETE). Por defecto POST</param>
        /// <returns>Respuesta deserializada como JObject para acceso dinámico a propiedades</returns>
        /// <example>
        /// // POST request con datos
        /// var result = await fixture.Send&lt;UsersController&gt;("CreateUser", bodyData: userData);
        /// 
        /// // GET request con parámetros de ruta
        /// var user = await fixture.Send&lt;UsersController&gt;("GetById", 
        ///     routeValues: new { id = 1 }, method: HttpMethod.Get);
        /// 
        /// // Request que debe fallar
        /// var error = await fixture.Send&lt;UsersController&gt;("InvalidAction", shouldSucceed: false);
        /// </example>
        public async Task<JObject> Send<TControllerType>(string action, bool shouldSucceed = true,
            object routeValues = null, object bodyData = null, HttpMethod method = null)
            where TControllerType : ControllerBase
        {
            method = method ?? HttpMethod.Post;
            var url = LinkGenerator.GetPathByAction(action, typeof(TControllerType).Name.Replace("Controller", ""), routeValues);
            var m = new HttpRequestMessage(method, url);

            if (bodyData != null)
            {
                m.Content = new StringContent(JsonConvert.SerializeObject(bodyData, _serializerSettings), Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrWhiteSpace(CurrentUser))
            {
                if (!Tokens.ContainsKey(CurrentUser))
                    throw new InvalidOperationException($"No token found for user '{CurrentUser}'. Available users: [{string.Join(", ", Tokens.Keys)}]");

                m.Headers.Add("Authorization", $"Bearer {Tokens[CurrentUser]}");
            }

            var httpResponse = await Client.SendAsync(m);
            var json = await httpResponse.Content.ReadAsStringAsync();

            if (shouldSucceed)
            {
                httpResponse.EnsureSuccessStatusCode();
            }
            else
            {
                var statusCode = (int)httpResponse.StatusCode;
                if (statusCode >= 200 && statusCode < 300)
                {
                    throw new InvalidOperationException("Should not be success!");
                }
            }

            return JObject.Parse(json);
        }

        /// <summary>
        /// Versión tipada del método Send que deserializa la respuesta al tipo especificado.
        /// Útil cuando conoces la estructura exacta de la respuesta.
        /// </summary>
        /// <typeparam name="T">Tipo al que deserializar la respuesta</typeparam>
        /// <typeparam name="TControllerType">Tipo del controlador al que realizar el request</typeparam>
        /// <param name="action">Nombre del método/acción del controlador</param>
        /// <param name="shouldSucceed">Si debe ser exitoso o no</param>
        /// <param name="routeValues">Parámetros de ruta</param>
        /// <param name="bodyData">Datos del cuerpo</param>
        /// <param name="method">Método HTTP</param>
        /// <returns>Respuesta deserializada al tipo T especificado</returns>
        /// <example>
        /// // Deserializar directamente a un modelo fuertemente tipado
        /// var user = await fixture.Send&lt;User, UsersController&gt;("GetById", 
        ///     routeValues: new { id = 1 }, method: HttpMethod.Get);
        /// 
        /// // Deserializar a un DTO
        /// var userData = await fixture.Send&lt;UserDto, UsersController&gt;("CreateUser", bodyData: newUser);
        /// </example>
        public async Task<T> Send<T, TControllerType>(string action, bool shouldSucceed = true,
            object routeValues = null, object bodyData = null, HttpMethod method = null) where TControllerType : ControllerBase
        {
            var jobject = await Send<TControllerType>(action, shouldSucceed, routeValues, bodyData, method);
            return jobject.ToObject<T>(JsonSerializer.Create(_serializerSettings));
        }
    }
}