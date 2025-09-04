using System.Diagnostics;
using System.Net.Http.Headers;
using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Helpers.Attributes;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LadeviVentasApi.Controllers
{
    /// <summary>
    /// Controlador especializado para seeding de datos de testing e integración.
    /// Solo disponible en ambiente "Testing" y con validaciones de seguridad adicionales.
    /// </summary>
    /// <remarks>
    /// Este controlador maneja:
    /// - Creación de datos básicos del sistema (roles, países, monedas, etc.)
    /// - Creación de usuarios de prueba con diferentes roles
    /// - Integración con APIs externas para datos geográficos
    /// - Validaciones de seguridad por token y ambiente
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationTestSeedController : ControllerBase
    {
        #region DTOs para integración con API externa

        public class BaseResponseDto
        {
            public int ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class CountryDataDto
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public string CodigoTelefonico { get; set; }
            public int GrupoId { get; set; }
        }

        public class ProvinciaDataDto
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public long PaisId { get; set; }
        }

        public class CountryResponseDto : BaseResponseDto
        {
            public List<CountryDataDto> Data { get; set; }
        }

        public class RequestProvincias
        {
            public long PaisId { get; set; }
        }

        public class ProvinciaResponseDto : BaseResponseDto
        {
            public List<ProvinciaDataDto> Data { get; set; }
        }

        public class RequestMunicipios
        {
            public long ProvinciaId { get; set; }
        }

        public class MunicipioDataDto
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public long ProvinciaId { get; set; }
        }

        public class MunicipioResponseDto : BaseResponseDto
        {
            public List<MunicipioDataDto> Data { get; set; }
        }

        public class LocalidadDataDto
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public long MunicipioId { get; set; }
            public string CodigoTelefonico { get; set; }
        }

        #endregion

        #region Credenciales por defecto para testing

        /// <summary>
        /// Email del usuario administrador por defecto para testing
        /// </summary>
        public static string GetAdminMail => "admin@admin.com";

        /// <summary>
        /// Contraseña del usuario administrador por defecto para testing
        /// </summary>
        public static string GetAdminPassword => "Xadmin001!";

        #endregion

        #region Dependencias inyectadas

        protected ApplicationDbContext Context { get; set; }
        protected IMapper Mapper { get; }
        private ApplicationUsersController ApplicationUsersController { get; }
        private CountryController CountryController { get; }
        private StateController StateController { get; }
        private DistrictController DistrictController { get; }
        private CityController CityController { get; }

        #endregion

        /// <summary>
        /// Constructor con inyección de dependencias para todos los controladores y servicios necesarios
        /// </summary>
        public IntegrationTestSeedController(
            ApplicationDbContext context, IMapper mapper,
            ApplicationUsersController applicationUsersController,
            CountryController countryController,
            StateController stateController,
            DistrictController districtController,
            CityController cityController)
        {
            Context = context;
            Mapper = mapper;
            ApplicationUsersController = applicationUsersController;
            CountryController = countryController;
            StateController = stateController;
            DistrictController = districtController;
            CityController = cityController;
        }

        /// <summary>
        /// Endpoint principal para crear todos los datos básicos necesarios para testing.
        /// Incluye roles, condiciones de facturación, métodos de pago, monedas, ubicaciones,
        /// tipos de productos, tipos de impuestos y usuario administrador.
        /// </summary>
        /// <param name="token">Token de seguridad para validar acceso al endpoint</param>
        /// <param name="useApi">Si es true, carga datos geográficos desde API externa. Si es false, usa datos prefijados</param>
        /// <returns>Token JWT del usuario administrador creado</returns>
        [TestingOnly]
        [AllowAnonymous]
        [HttpPost("BasicDataSeed/{token}")]
        public async Task<ActionResult> BasicDataSeed(string token, [FromQuery] bool useApi = false)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // Cargar datos geográficos (países, provincias, municipios)
                    await SaveLocationData(useApi);

                    // Crear roles del sistema si no existen
                    if (!Context.ApplicationRole.Any())
                    {
                        Context.ApplicationRole.AddRange(
                            new ApplicationRole { Name = ApplicationRole.SuperuserRole },
                            new ApplicationRole { Name = ApplicationRole.NationalSellerRole },
                            new ApplicationRole { Name = ApplicationRole.COMTURSellerRole },
                            new ApplicationRole { Name = ApplicationRole.SupervisorRole }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear condiciones de facturación si no existen
                    if (!Context.BillingConditions.Any())
                    {
                        Context.BillingConditions.AddRange(
                            new BillingCondition { Name = BillingCondition.Anticipated },
                            new BillingCondition { Name = BillingCondition.AgainstPublication },
                            new BillingCondition { Name = BillingCondition.Exchange },
                            new BillingCondition { Name = BillingCondition.NoFee }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear métodos de pago si no existen
                    if (!Context.PaymentMethods.Any())
                    {
                        Context.PaymentMethods.AddRange(
                            new PaymentMethod { Name = PaymentMethod.Documented },
                            new PaymentMethod { Name = PaymentMethod.OnePayment },
                            new PaymentMethod { Name = PaymentMethod.Another }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear monedas si no existen
                    if (!Context.Currency.Any())
                    {
                        Context.Currency.AddRange(
                            new Currency { Name = Currency.ARS, CountryId = 4 },
                            new Currency { Name = Currency.USS, CountryId = 81 },
                            new Currency { Name = Currency.COL, CountryId = 18 }
                        // Comentadas para futuros cambios:
                        // new Currency { Name = Currency.CHL, CountryId = 16 },
                        // new Currency { Name = Currency.MEX, CountryId = 50 },
                        // new Currency { Name = Currency.PE, CountryId = 56 }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear tipos de ubicación de espacios publicitarios si no existen
                    if (!Context.AdvertisingSpaceLocationTypes.Any())
                    {
                        Context.AdvertisingSpaceLocationTypes.AddRange(
                            new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.BeforeCentral },
                            new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.AfterCentral },
                            new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.RotaryLocation },
                            new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.SilverLocation }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear tipos de productos si no existen
                    if (!Context.ProductTypes.Any())
                    {
                        Context.ProductTypes.AddRange(
                            new ProductType { Name = ProductType.Magazine },
                            new ProductType { Name = ProductType.Newsletter }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Crear tipos de códigos de identificación tributaria si no existen
                    if (!Context.TaxType.Any())
                    {
                        Context.TaxType.AddRange(
                            new TaxType { Name = "CUIT", CountryId = 4, IsIdentificationField = true, OptionsInternal = string.Empty }, // Argentina
                            new TaxType { Name = "TIN", CountryId = 81, IsIdentificationField = true, OptionsInternal = string.Empty }, // USA
                            new TaxType { Name = "NIT", CountryId = 18, IsIdentificationField = true, OptionsInternal = string.Empty } // Colombia

                        // Comentados para futuros cambios:
                        // new TaxType { Name = "RFC", CountryId = 50, IsIdentificationField = true, OptionsInternal=string.Empty }, // Mexico
                        // new TaxType { Name = "RUT", CountryId = 16, IsIdentificationField = true, OptionsInternal=string.Empty }, // Chile
                        // new TaxType { Name = "RUC", CountryId = 56, IsIdentificationField = true, OptionsInternal=string.Empty }, // Peru
                        // new TaxType { Name = "TUR", CountryId = 80, IsIdentificationField = true, OptionsInternal=string.Empty }, // Uruguay
                        // new TaxType { Name = "SIN", CountryId = 15, IsIdentificationField = true, OptionsInternal=string.Empty }, // Canada
                        // new TaxType { Name = "CPF", CountryId = 14, IsIdentificationField = true, OptionsInternal=string.Empty }, // Brasil
                        // new TaxType { Name = "NIT", CountryId = 13, IsIdentificationField = true, OptionsInternal=string.Empty }, // Bolivia
                        // new TaxType { Name = "NIT", CountryId = 54, IsIdentificationField = true, OptionsInternal=string.Empty }, // Panama
                        // new TaxType { Name = "NIT", CountryId = 27, IsIdentificationField = true, OptionsInternal=string.Empty }, // Salvador
                        // new TaxType { Name = "NIT", CountryId = 33, IsIdentificationField = true, OptionsInternal=string.Empty }  // Guatemala
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Configurar el controlador de usuarios para que funcione correctamente
                    ApplicationUsersController.ObjectValidator = ObjectValidator;
                    ApplicationUsersController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                    ApplicationUsersController.Url = Url;

                    // Crear usuario administrador para testing si no existe usando el método reutilizable
                    if (!Context.ApplicationUsers.Any(x => x.Initials.Equals("AD")))
                    {
                        CreateUser("Admin for Tests", ApplicationRole.SuperuserRole, "ARGENTINA", "AD", GetAdminMail);
                    }

                    await transaction.CommitAsync();

                    // Devolver token JWT válido para el usuario creado
                    return Ok(new { token = ApplicationUsersController.GetTokenString(GetAdminMail) });
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Debug.WriteLine($"Error en BasicDataSeed: {e}");
                    return BadRequest(new { error = e.Message, stackTrace = e.StackTrace });
                }
            }
        }

        /// <summary>
        /// Endpoint para crear usuarios de prueba con diferentes roles.
        /// Útil para testing de funcionalidades específicas por rol.
        /// </summary>
        /// <param name="token">Token de seguridad para validar acceso al endpoint</param>
        /// <returns>Confirmación de creación exitosa</returns>
        [TestingOnly]
        [AllowAnonymous]
        [HttpPost("CreateTestUsers/{token}")]
        public async Task<ActionResult> CreateTestUsers(string token)
        {
            // Configurar el controlador de usuarios
            ApplicationUsersController.ObjectValidator = ObjectValidator;
            ApplicationUsersController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            // Crear usuarios con diferentes roles para testing
            CreateUser("admin", ApplicationRole.SuperuserRole, "ARGENTINA");
            CreateUser("supervisor", ApplicationRole.SupervisorRole, "ARGENTINA");
            CreateUser("argSeller", ApplicationRole.NationalSellerRole, "ARGENTINA");
            CreateUser("nationalsellerBr", ApplicationRole.NationalSellerRole, "BRASIL");
            CreateUser("comtur-seller", ApplicationRole.COMTURSellerRole, "ARGENTINA");

            return Ok(new { message = "Test users created successfully" });
        }

        /// <summary>
        /// Carga datos geográficos (países, provincias, municipios) desde API externa o datos prefijados.
        /// </summary>
        /// <param name="useApi">Si es true, conecta a API externa. Si es false, usa datos hardcodeados para testing rápido</param>
        /// <returns>Resumen de datos insertados y errores si los hubo</returns>
        private async Task<object> SaveLocationData(bool useApi = false)
        {
            // Configurar controladores necesarios
            CountryController.ObjectValidator = ObjectValidator;
            CountryController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            StateController.ObjectValidator = ObjectValidator;
            StateController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            DistrictController.ObjectValidator = ObjectValidator;
            DistrictController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            CityController.ObjectValidator = ObjectValidator;
            CityController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            ApplicationUsersController.ObjectValidator = ObjectValidator;
            ApplicationUsersController.ControllerContext.HttpContext = ControllerContext.HttpContext;

            List<string> paisesInsertados = new List<string>();
            List<string> provinciasInsertadas = new List<string>();
            List<string> municipiosInsertados = new List<string>();
            List<string> ciudadesInsertadas = new List<string>();
            string error = string.Empty;

            // Datos prefijados para testing rápido (cuando useApi = false)
            string responsePaisesBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"88\",\"Nombre\":\"sin pais\",\"CodigoTelefonico\":\"0\",\"GrupoId\":\"0\"},{\"Id\":\"4\",\"Nombre\":\"ARGENTINA\",\"CodigoTelefonico\":\"54\",\"GrupoId\":\"1\"},{\"Id\":\"18\",\"Nombre\":\"COLOMBIA\",\"CodigoTelefonico\":\"57\",\"GrupoId\":\"1\"},{\"Id\":\"81\",\"Nombre\":\"USA\",\"CodigoTelefonico\":\"1\",\"GrupoId\":\"3\"},{\"Id\":\"29\",\"Nombre\":\"ESPAÑA\",\"CodigoTelefonico\":\"34\",\"GrupoId\":\"3\"}]}";
            string responseArgentinaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"65\",\"Nombre\":\"BUENOS AIRES\",\"PaisId\":\"4\"},{\"Id\":\"80\",\"Nombre\":\"CAPITAL FEDERAL\",\"PaisId\":\"4\"},{\"Id\":\"132\",\"Nombre\":\"CORDOBA\",\"PaisId\":\"4\"},{\"Id\":\"417\",\"Nombre\":\"SANTA FE\",\"PaisId\":\"4\"}]}";
            string responseColombiaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"22\",\"Nombre\":\"ANTIOQUIA\",\"PaisId\":\"18\"},{\"Id\":\"472\",\"Nombre\":\"VALLE DEL CAUCA\",\"PaisId\":\"18\"}]}";
            string responseUsaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"71\",\"Nombre\":\"CALIFORNIA\",\"PaisId\":\"81\"},{\"Id\":\"454\",\"Nombre\":\"TEXAS\",\"PaisId\":\"81\"}]}";
            string responseEspaniaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"264\",\"Nombre\":\"MADRID\",\"PaisId\":\"29\"},{\"Id\":\"1724\",\"Nombre\":\"CATALUÑA\",\"PaisId\":\"29\"}]}";
            string responsNoDataBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[]}";

            using (var client = new HttpClient())
            {
                try
                {
                    // Obtener países desde API o usar datos prefijados
                    if (useApi)
                    {
                        HttpResponseMessage responsePaises = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetPaises", null);
                        responsePaises.EnsureSuccessStatusCode();
                        responsePaisesBody = await responsePaises.Content.ReadAsStringAsync();
                    }

                    CountryResponseDto countries = JsonConvert.DeserializeObject<CountryResponseDto>(responsePaisesBody);

                    foreach (var c in countries.Data)
                    {
                        // Crear país si no existe
                        if (!Context.Country.Any(x => x.Id == c.Id))
                        {
                            Country pais = new Country();
                            pais.Id = c.Id;
                            pais.Name = c.Nombre.ToUpper();
                            pais.CodigoTelefonico = c.CodigoTelefonico;

                            Context.Add(pais);
                            paisesInsertados.Add(c.Nombre.ToUpper());
                        }

                        // Obtener provincias para cada país
                        string responseProvinciaBody;

                        if (useApi)
                        {
                            RequestProvincias requestData = new RequestProvincias { PaisId = c.Id };
                            var myContent = JsonConvert.SerializeObject(requestData);
                            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                            var byteContent = new ByteArrayContent(buffer);
                            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            HttpResponseMessage responseProvincia = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetProvincias", byteContent);
                            responseProvincia.EnsureSuccessStatusCode();
                            responseProvinciaBody = await responseProvincia.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            // Usar datos prefijados según el país
                            responseProvinciaBody = c.Id == 4 ? responseArgentinaProvinciaBody :
                                                  c.Id == 18 ? responseColombiaProvinciaBody :
                                                  c.Id == 81 ? responseUsaProvinciaBody :
                                                  c.Id == 29 ? responseEspaniaProvinciaBody : responsNoDataBody;
                        }

                        ProvinciaResponseDto provincias = JsonConvert.DeserializeObject<ProvinciaResponseDto>(responseProvinciaBody);

                        if (provincias.Data != null)
                        {
                            foreach (var p in provincias.Data)
                            {
                                // Crear provincia si no existe
                                if (Context.State.Find(p.Id) == null)
                                {
                                    State prov = new State();
                                    prov.CountryId = p.PaisId;
                                    prov.Id = p.Id;
                                    prov.Name = p.Nombre.ToUpper();

                                    Context.Add(prov);
                                    provinciasInsertadas.Add(p.Nombre.ToUpper());
                                }

                                // Obtener municipios para cada provincia (solo desde API)
                                if (useApi)
                                {
                                    RequestMunicipios requestMun = new RequestMunicipios { ProvinciaId = p.Id };
                                    var myContentMun = JsonConvert.SerializeObject(requestMun);
                                    var bufferMun = System.Text.Encoding.UTF8.GetBytes(myContentMun);
                                    var byteContentMun = new ByteArrayContent(bufferMun);
                                    byteContentMun.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                    HttpResponseMessage responseMunicipio = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetMunicipios", byteContentMun);
                                    responseMunicipio.EnsureSuccessStatusCode();
                                    string responseMunicipioBody = await responseMunicipio.Content.ReadAsStringAsync();

                                    MunicipioResponseDto municipios = JsonConvert.DeserializeObject<MunicipioResponseDto>(responseMunicipioBody);

                                    if (municipios.Data != null)
                                    {
                                        foreach (var m in municipios.Data)
                                        {
                                            // Crear municipio si no existe
                                            if (Context.District.Find(m.Id) == null)
                                            {
                                                District district = new District();
                                                district.Id = m.Id;
                                                district.Name = m.Nombre.ToUpper();
                                                district.StateId = m.ProvinciaId;
                                                Context.Add(district);
                                                municipiosInsertados.Add(m.Nombre.ToUpper());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                    // Limpiar listas en caso de error para indicar que no se insertó nada
                    paisesInsertados = new List<string>();
                    provinciasInsertadas = new List<string>();
                    municipiosInsertados = new List<string>();
                    ciudadesInsertadas = new List<string>();
                }
            }

            return new
            {
                error = error,
                paises = paisesInsertados,
                provincias = provinciasInsertadas,
                municipios = municipiosInsertados,
                ciudades = ciudadesInsertadas,
                useApi = useApi
            };
        }

        /// <summary>
        /// Crea un usuario de prueba con los parámetros especificados.
        /// Maneja tanto usuarios normales como el usuario administrador principal.
        /// </summary>
        /// <param name="fullName">Nombre completo del usuario</param>
        /// <param name="roleName">Nombre del rol a asignar</param>
        /// <param name="countryName">Nombre del país del usuario</param>
        /// <param name="initials">Iniciales del usuario (opcional, si no se proporciona usa el fullName)</param>
        /// <param name="email">Email del usuario (opcional, si no se proporciona usa {fullName}@mail.com)</param>
        /// <param name="confirmationEmailRequired">Si es true, simula el proceso de confirmación por email</param>
        private void CreateUser(string fullName, string roleName, string countryName, string initials = null, string email = null, bool confirmationEmailRequired = false)
        {
            string confirmationCode = null;

            if (confirmationEmailRequired)
            {
                EmailSenderExtensions.OnEmailEvents += (sender, args) => confirmationCode = sender.ToString();
            }

            var country = Context.Country.Single(c => c.Name == countryName);
            var role = Context.ApplicationRole.Single(r => r.Name == roleName);

            // Usar valores por defecto si no se proporcionan
            initials = initials ?? fullName;
            email = email ?? $"{fullName.ToLower().Replace(" ", "")}@mail.com";

            // Crear usuario
            var user01 = (ApplicationUsersController.Post(new ApplicationUserWritingDto
            {
                Email = email,
                Password = GetAdminPassword,
                FullName = fullName,
                Initials = initials,
                CountryId = country.Id,
                ApplicationRoleId = role.Id
            }).Result as CreatedResult)?.Value as ApplicationUserWritingDto;


            if (confirmationEmailRequired)
            {
                var userFull = Context.ApplicationUsers.Include(u => u.CredentialsUser).Single(u => u.Id == user01.Id);
                var userConfirmedOk = ApplicationUsersController.Confirm(userFull.CredentialsUser.Id, confirmationCode).Result;
            }
        }
    }
}