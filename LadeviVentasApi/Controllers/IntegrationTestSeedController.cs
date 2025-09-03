using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using LadeviVentasApi.Data;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LadeviVentasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationTestSeedController : ControllerBase
    {
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

        public class RequestLocalidades
        {
            public long MunicipioId { get; set; }
        }

        public class LocalidadDataDto
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public long MunicipioId { get; set; }
            public string CodigoTelefonico { get; set; }
        }

        public class LocalidadResponseDto : BaseResponseDto
        {
            public List<LocalidadDataDto> Data { get; set; }
        }

        public static string GetAdminMail => "admin@admin.com";
        public static string GetAdminPassword => "Xadmin001!";

        protected ApplicationDbContext Context { get; set; }
        protected IMapper Mapper { get; }
        private IConfiguration Configuration { get; }
        private ApplicationRoleController ApplicationRoleController { get; }
        private ApplicationUsersController ApplicationUsersController { get; }
        private CountryController CountryController { get; }
        private BillingConditionController BillingConditionController { get; }
        private PaymentMethodController PaymentMethodController { get; }
        private CurrencyController CurrencyController { get; }
        private AdvertisingSpaceLocationTypeController AdvertisingSpaceLocationTypeController { get; }
        private ProductTypeController ProductTypeController { get; }
        private StateController StateController { get; }
        private DistrictController DistrictController { get; }
        private CityController CityController { get; }
        private TaxTypeController TaxTypeController { get; }
        private UserManager<IdentityUser> UserManager { get; }


        public IntegrationTestSeedController(
            ApplicationDbContext context, IMapper mapper,
            IConfiguration configuration,
            ApplicationRoleController applicationRoleController,
            ApplicationUsersController applicationUsersController,
            CountryController countryController,
            BillingConditionController billingConditionController,
            PaymentMethodController paymentMethodController,
            CurrencyController currencyController,
            AdvertisingSpaceLocationTypeController advertisingSpaceLocationTypeController,
            ProductTypeController productTypeController,
            StateController stateController,
            DistrictController districtController,
            CityController cityController,
            TaxTypeController taxTypeController,
             UserManager<IdentityUser> userManager
            )
        {
            Context = context;
            Mapper = mapper;
            Configuration = configuration;
            ApplicationUsersController = applicationUsersController;
            ApplicationRoleController = applicationRoleController;
            CountryController = countryController;
            BillingConditionController = billingConditionController;
            PaymentMethodController = paymentMethodController;
            CurrencyController = currencyController;
            AdvertisingSpaceLocationTypeController = advertisingSpaceLocationTypeController;
            ProductTypeController = productTypeController;
            StateController = stateController;
            DistrictController = districtController;
            CityController = cityController;
            TaxTypeController = taxTypeController;
            UserManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("CreateTestUsers/{token}")]
        public async Task<ActionResult> CreateTestUsers(string token)
        {
            var configToken = Configuration["IntegrationTestSeedToken"];
            if (!token.Equals(configToken)) return BadRequest(new { error = "Invalid token for endpoint" });

            ApplicationUsersController.ObjectValidator = ObjectValidator;
            ApplicationUsersController.ControllerContext.HttpContext = ControllerContext.HttpContext;
            ApplicationUsersController.Url = Url;

            CreateUser("admin", ApplicationRole.SuperuserRole, "ARGENTINA");
            CreateUser("supervisor", ApplicationRole.SupervisorRole, "ARGENTINA");
            CreateUser("argSeller", ApplicationRole.NationalSellerRole, "ARGENTINA");
            CreateUser("nationalsellerBr", ApplicationRole.NationalSellerRole, "BRASIL");
            CreateUser("comtur-seller", ApplicationRole.COMTURSellerRole, "ARGENTINA");

            return Ok();
        }

        private async Task<object> SaveLocationData()
        {
            CountryController.ObjectValidator = ObjectValidator;
            CountryController.ControllerContext.HttpContext = ControllerContext.HttpContext;
            //CountryController.Url = Url;

            StateController.ObjectValidator = ObjectValidator;
            StateController.ControllerContext.HttpContext = ControllerContext.HttpContext;
            //StateController.Url = Url;

            DistrictController.ObjectValidator = ObjectValidator;
            DistrictController.ControllerContext.HttpContext = ControllerContext.HttpContext;
            //DistrictController.Url = Url;

            CityController.ObjectValidator = ObjectValidator;
            CityController.ControllerContext.HttpContext = ControllerContext.HttpContext;
            //CityController.Url = Url;

            List<string> paisesInsertados = new List<string>();
            List<string> provinciasInsertadas = new List<string>();
            List<string> municipiosInsertados = new List<string>();
            List<string> ciudadesInsertadas = new List<string>();
            string error = string.Empty;

            using (var client = new HttpClient())
            {
                string responsePaisesBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"88\",\"Nombre\":\"sin pais\",\"CodigoTelefonico\":\"0\",\"GrupoId\":\"0\"},{\"Id\":\"4\",\"Nombre\":\"ARGENTINA\",\"CodigoTelefonico\":\"54\",\"GrupoId\":\"1\"},{\"Id\":\"18\",\"Nombre\":\"COLOMBIA\",\"CodigoTelefonico\":\"57\",\"GrupoId\":\"1\"},{\"Id\":\"81\",\"Nombre\":\"USA\",\"CodigoTelefonico\":\"1\",\"GrupoId\":\"3\"},{\"Id\":\"29\",\"Nombre\":\"ESPAÑA\",\"CodigoTelefonico\":\"34\",\"GrupoId\":\"3\"}]}";
                string responseArgentinaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"65\",\"Nombre\":\"BUENOS AIRES\",\"PaisId\":\"4\"},{\"Id\":\"80\",\"Nombre\":\"CAPITAL FEDERAL\",\"PaisId\":\"4\"},{\"Id\":\"132\",\"Nombre\":\"CORDOBA\",\"PaisId\":\"4\"},{\"Id\":\"417\",\"Nombre\":\"SANTA FE\",\"PaisId\":\"4\"}]}";
                string responseColombiaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"22\",\"Nombre\":\"ANTIOQUIA\",\"PaisId\":\"18\"},{\"Id\":\"472\",\"Nombre\":\"VALLE DEL CAUCA\",\"PaisId\":\"18\"}]}";
                string responseUsaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"71\",\"Nombre\":\"CALIFORNIA\",\"PaisId\":\"81\"},{\"Id\":\"454\",\"Nombre\":\"TEXAS\",\"PaisId\":\"81\"}]}";
                string responseEspaniaProvinciaBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[{\"Id\":\"264\",\"Nombre\":\"MADRID\",\"PaisId\":\"29\"},{\"Id\":\"1724\",\"Nombre\":\"CATALUÑA\",\"PaisId\":\"29\"}]}";
                string responsNoDataBody = "{\"ErrorCode\":0,\"ErrorMessage\":\"\",\"Data\":[]}";

                try
                {
                    // HttpResponseMessage responsePaises = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetPaises", null);
                    // responsePaises.EnsureSuccessStatusCode();
                    // string responsePaisesBody = await responsePaises.Content.ReadAsStringAsync();
                    CountryResponseDto countries = JsonConvert.DeserializeObject<CountryResponseDto>(responsePaisesBody);

                    foreach (var c in countries.Data)
                    {
                        if (!Context.Country.Any(x => x.Id == c.Id))
                        {
                            Country pais = new Country();
                            pais.Id = c.Id;
                            pais.Name = c.Nombre.ToUpper();
                            pais.CodigoTelefonico = c.CodigoTelefonico;

                            Context.Add(pais);
                            paisesInsertados.Add(c.Nombre.ToUpper());
                            // await CountryController.Post(pais);
                        }

                        RequestProvincias requestData = new RequestProvincias { PaisId = c.Id };
                        var myContent = JsonConvert.SerializeObject(requestData);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        // HttpResponseMessage responseProvincia = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetProvincias", byteContent);
                        // responseProvincia.EnsureSuccessStatusCode();
                        // string responseProvinciaBody = await responseProvincia.Content.ReadAsStringAsync();

                        string responseProvinciaBody = c.Id == 4 ? responseArgentinaProvinciaBody :
                                                        c.Id == 18 ? responseColombiaProvinciaBody :
                                                        c.Id == 81 ? responseUsaProvinciaBody :
                                                        c.Id == 29 ? responseEspaniaProvinciaBody : responsNoDataBody;

                        ProvinciaResponseDto provincias = JsonConvert.DeserializeObject<ProvinciaResponseDto>(responseProvinciaBody);

                        if (provincias.Data != null)
                        {
                            foreach (var p in provincias.Data)
                            {
                                if (Context.State.Find(p.Id) == null)
                                {
                                    State prov = new State();
                                    prov.CountryId = p.PaisId;
                                    prov.Id = p.Id;
                                    prov.Name = p.Nombre.ToUpper();

                                    Context.Add(prov);
                                    provinciasInsertadas.Add(p.Nombre.ToUpper());
                                    //await StateController.Post(prov);
                                }

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
                                        if (Context.District.Find(m.Id) == null)
                                        {
                                            District district = new District();
                                            district.Id = m.Id;
                                            district.Name = m.Nombre.ToUpper();
                                            district.StateId = m.ProvinciaId;
                                            Context.Add(district);
                                            municipiosInsertados.Add(m.Nombre.ToUpper());
                                            //await DistrictController.Post(district);
                                        }

                                        // RequestLocalidades requestLoc = new RequestLocalidades { MunicipioId = m.Id };
                                        // var myContentLoc = JsonConvert.SerializeObject(requestLoc);
                                        // var bufferLoc = System.Text.Encoding.UTF8.GetBytes(myContentLoc);
                                        // var byteContentLoc = new ByteArrayContent(bufferLoc);
                                        // byteContentLoc.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                        // HttpResponseMessage responseLocalidad = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetLocalidades", byteContentLoc);
                                        // responseLocalidad.EnsureSuccessStatusCode();
                                        // string responseLocalidadBody = await responseLocalidad.Content.ReadAsStringAsync();

                                        // LocalidadResponseDto localidades = JsonConvert.DeserializeObject<LocalidadResponseDto>(responseLocalidadBody);

                                        // if (localidades.Data != null)
                                        // {
                                        //     foreach (var l in localidades.Data)
                                        //     {
                                        //         if (Context.City.Find(l.Id) == null)
                                        //         {
                                        //             City city = new City();
                                        //             city.Id = l.Id;
                                        //             city.DistrictId = l.MunicipioId;
                                        //             city.Name = l.Nombre.ToUpper();
                                        //             city.CodigoTelefonico = l.CodigoTelefonico;
                                        //             Context.Add(city);
                                        //             ciudadesInsertadas.Add(l.Nombre.ToUpper());
                                        //             //await CityController.Post(city);
                                        //         }
                                        //     }
                                        // }
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
                    paisesInsertados = new List<string>();
                    provinciasInsertadas = new List<string>();
                    municipiosInsertados = new List<string>();
                    ciudadesInsertadas = new List<string>();
                }
            }

            return new { error = error, paises = paisesInsertados, provincias = provinciasInsertadas, municipios = municipiosInsertados, ciudades = ciudadesInsertadas };
        }

        [AllowAnonymous]
        [HttpGet("SeedLocationData/{token}")]
        public async Task<ActionResult> SeedLocationData(string token)
        {
            var configToken = Configuration["IntegrationTestSeedToken"];
            if (!token.Equals(configToken)) return BadRequest(new { error = "Invalid token for endpoint" });

            object result = SaveLocationData().Result;

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost("RegisterUserSeed/{token}")]
        public async Task<ActionResult> RegisterUserSeed(string token)
        {
            var configToken = Configuration["IntegrationTestSeedToken"];
            if (!token.Equals(configToken)) return BadRequest(new { error = "Invalid token for endpoint" });

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    await SaveLocationData();

                    // Roles
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

                    // Condiciones de facturación
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

                    // Métodos de pago
                    if (!Context.PaymentMethods.Any())
                    {
                        Context.PaymentMethods.AddRange(
                            new PaymentMethod { Name = PaymentMethod.Documented },
                            new PaymentMethod { Name = PaymentMethod.OnePayment },
                            new PaymentMethod { Name = PaymentMethod.Another }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Monedas
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

                    // Ubicaciones
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

                    // Tipo de Productos
                    if (!Context.ProductTypes.Any())
                    {
                        Context.ProductTypes.AddRange(
                            new ProductType { Name = ProductType.Magazine },
                            new ProductType { Name = ProductType.Newsletter }
                        );
                        await Context.SaveChangesAsync();
                    }

                    // Códigos de identificación tributaria
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

                    // Usuario admin básico para testing
                    if (!Context.ApplicationUsers.Any(x => x.Initials.Equals("AD")))
                    {
                        // var rolesDebug = Context.Database.SqlQuery<string>(new FormattableString( "SELECT \"Name\" FROM \"ApplicationRole\"").ToList();
                        // Console.WriteLine($"Roles found: {string.Join(", ", rolesDebug)}");
                        // // Para testing, crear un usuario básico directamente en la base
                        // var adminRole = Context.ApplicationRole.FirstOrDefault(role => role.Name == ApplicationRole.SuperuserRole);
                        var adminRole = Context.ApplicationRole.FirstOrDefault();
                        if (adminRole != null)
                        {
                            var adminIdentityuser = new IdentityUser { UserName = GetAdminMail, Email = GetAdminMail };
                            var identityResult = await UserManager.CreateAsync(adminIdentityuser, GetAdminPassword);
                            if (identityResult.Succeeded)
                            {
                                var adminUser = new ApplicationUser
                                {
                                    FullName = "Admin for Tests",
                                    Initials = "AD",
                                    ApplicationRoleId = adminRole.Id,
                                    CountryId = 4,
                                    CommisionCoeficient = 0,
                                    CredentialsUser = adminIdentityuser
                                };

                                Context.ApplicationUsers.Add(adminUser);
                                await Context.SaveChangesAsync();
                            }
                        }
                    }

                    await transaction.CommitAsync();
                    return Ok(new { token = "mock_token_for_testing", message = "Seed completed successfully" });
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Debug.WriteLine($"Error en RegisterUserSeed: {e}");
                    return BadRequest(new { error = e.Message, stackTrace = e.StackTrace });
                }
            }
        }

        private void CreateUser(string alias, string roleName, string countryName)
        {
            string confirmationCode = null;
            EmailSenderExtensions.OnEmailEvents += (sender, args) => confirmationCode = sender.ToString();
            var country = Context.Country.Single(c => c.Name == countryName);
            var role = Context.ApplicationRole.Single(r => r.Name == roleName);

            //intentamos crear un user valido
            var user01 = (ApplicationUsersController.Post(new ApplicationUserWritingDto
            {
                Email = $"{alias}@mail.com",
                Password = GetAdminPassword,
                FullName = alias,
                Initials = alias,
                CountryId = country.Id,
                ApplicationRoleId = role.Id
            }
            ).Result as CreatedResult)?.Value as ApplicationUserWritingDto;

            //hacemos el confirmar
            var userFull = Context.ApplicationUsers.Include(u => u.CredentialsUser).Single(u => u.Id == user01.Id);
            var userConfirmedOk = ApplicationUsersController.Confirm(userFull.CredentialsUser.Id, confirmationCode).Result;
        }
    }
}