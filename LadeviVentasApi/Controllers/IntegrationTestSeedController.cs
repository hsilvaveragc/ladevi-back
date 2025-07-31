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
            TaxTypeController taxTypeController
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
                try
                {
                    HttpResponseMessage responsePaises = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetPaises", null);
                    responsePaises.EnsureSuccessStatusCode();
                    string responsePaisesBody = await responsePaises.Content.ReadAsStringAsync();

                    CountryResponseDto countries = JsonConvert.DeserializeObject<CountryResponseDto>(responsePaisesBody);

                    //USA y Turquia
                    // var countryFaltante = countries.Data.Where(x => x.Id == 79 || x.Id == 81).ToList();

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

                        HttpResponseMessage responseProvincia = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetProvincias", byteContent);
                        responseProvincia.EnsureSuccessStatusCode();
                        string responseProvinciaBody = await responseProvincia.Content.ReadAsStringAsync();

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

                                        RequestLocalidades requestLoc = new RequestLocalidades { MunicipioId = m.Id };
                                        var myContentLoc = JsonConvert.SerializeObject(requestLoc);
                                        var bufferLoc = System.Text.Encoding.UTF8.GetBytes(myContentLoc);
                                        var byteContentLoc = new ByteArrayContent(bufferLoc);
                                        byteContentLoc.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                        HttpResponseMessage responseLocalidad = await client.PostAsync("http://lectores.ladevi.travel/api/ventas.php/GetLocalidades", byteContentLoc);
                                        responseLocalidad.EnsureSuccessStatusCode();
                                        string responseLocalidadBody = await responseLocalidad.Content.ReadAsStringAsync();

                                        LocalidadResponseDto localidades = JsonConvert.DeserializeObject<LocalidadResponseDto>(responseLocalidadBody);

                                        if (localidades.Data != null)
                                        {
                                            foreach (var l in localidades.Data)
                                            {
                                                if (Context.City.Find(l.Id) == null)
                                                {
                                                    City city = new City();
                                                    city.Id = l.Id;
                                                    city.DistrictId = l.MunicipioId;
                                                    city.Name = l.Nombre.ToUpper();
                                                    city.CodigoTelefonico = l.CodigoTelefonico;
                                                    Context.Add(city);
                                                    ciudadesInsertadas.Add(l.Nombre.ToUpper());
                                                    //await CityController.Post(city);
                                                }
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
            try
            {
                ApplicationRoleController.ObjectValidator = ObjectValidator;
                ApplicationUsersController.ObjectValidator = ObjectValidator;
                CountryController.ObjectValidator = ObjectValidator;
                BillingConditionController.ObjectValidator = ObjectValidator;
                PaymentMethodController.ObjectValidator = ObjectValidator;
                CurrencyController.ObjectValidator = ObjectValidator;
                AdvertisingSpaceLocationTypeController.ObjectValidator = ObjectValidator;
                ProductTypeController.ObjectValidator = ObjectValidator;
                TaxTypeController.ObjectValidator = ObjectValidator;


                ApplicationRoleController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                ApplicationUsersController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                CountryController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                BillingConditionController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                PaymentMethodController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                CurrencyController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                AdvertisingSpaceLocationTypeController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                ProductTypeController.ControllerContext.HttpContext = ControllerContext.HttpContext;
                TaxTypeController.ControllerContext.HttpContext = ControllerContext.HttpContext;

                ApplicationUsersController.Url = Url;

                await SaveLocationData();

                //Roles
                if (!Context.ApplicationRole.Any())
                {
                    await ApplicationRoleController.Post(new ApplicationRole { Name = ApplicationRole.SuperuserRole });
                    await ApplicationRoleController.Post(new ApplicationRole { Name = ApplicationRole.NationalSellerRole });
                    await ApplicationRoleController.Post(new ApplicationRole { Name = ApplicationRole.COMTURSellerRole });
                    await ApplicationRoleController.Post(new ApplicationRole { Name = ApplicationRole.SupervisorRole });
                }

                //Condiciones de facturación
                if (!Context.BillingConditions.Any())
                {
                    await BillingConditionController.Post(new BillingCondition { Name = BillingCondition.Anticipated });
                    await BillingConditionController.Post(new BillingCondition { Name = BillingCondition.AgainstPublication });
                    await BillingConditionController.Post(new BillingCondition { Name = BillingCondition.Exchange });
                    await BillingConditionController.Post(new BillingCondition { Name = BillingCondition.NoFee });
                }


                //Metodos de pago
                if (!Context.PaymentMethods.Any())
                {
                    await PaymentMethodController.Post(new PaymentMethod { Name = PaymentMethod.Documented });
                    await PaymentMethodController.Post(new PaymentMethod { Name = PaymentMethod.OnePayment });
                    await PaymentMethodController.Post(new PaymentMethod { Name = PaymentMethod.Another });
                }

                //Monedas
                if (!Context.Currency.Any())
                {
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.ARS, CountryId = 4 });
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.USS, CountryId = 81 });
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.CHL, CountryId = 16 });
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.COL, CountryId = 18 });
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.MEX, CountryId = 50 });
                    await CurrencyController.Post(new CurrencyWritingDto { Name = Currency.PE, CountryId = 56 });
                }

                //Ubicaciones
                if (!Context.AdvertisingSpaceLocationTypes.Any())
                {
                    await AdvertisingSpaceLocationTypeController.Post(new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.BeforeCentral });
                    await AdvertisingSpaceLocationTypeController.Post(new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.AfterCentral });
                    await AdvertisingSpaceLocationTypeController.Post(new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.RotaryLocation });
                    await AdvertisingSpaceLocationTypeController.Post(new AdvertisingSpaceLocationType { Name = AdvertisingSpaceLocationType.SilverLocation });
                }

                //Tipo de Productos
                if (!Context.ProductTypes.Any())
                {
                    await ProductTypeController.Post(new ProductType { Name = ProductType.Magazine });
                    await ProductTypeController.Post(new ProductType { Name = ProductType.Newsletter });
                }

                //Codigos de identificacion tributaria
                if (!Context.TaxType.Any())
                {
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "RFC", CountryId = 50, IsIdentificationField = true }); //Mexico
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "CUIT", CountryId = 4, IsIdentificationField = true }); //Argentina
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "TIN", CountryId = 81, IsIdentificationField = true }); //USA
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "RUT", CountryId = 16, IsIdentificationField = true }); //Chile
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "NIT", CountryId = 18, IsIdentificationField = true }); //Colombia
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "RUC", CountryId = 56, IsIdentificationField = true }); //Peru
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "TUR", CountryId = 80, IsIdentificationField = true }); //Uruguay
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "SIN", CountryId = 15, IsIdentificationField = true }); //Canada
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "CPF", CountryId = 14, IsIdentificationField = true }); //Brasil
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "NIT", CountryId = 13, IsIdentificationField = true }); //Bolivia
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "NIT", CountryId = 54, IsIdentificationField = true }); //Panama
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "NIT", CountryId = 27, IsIdentificationField = true }); //Salvador
                    await TaxTypeController.Post(new TaxTypeWritingDto { Name = "NIT", CountryId = 33, IsIdentificationField = true }); //Guatemala

                }

                var user = new ApplicationUserWritingDto();
                var confirmationCode = "";
                var applicationUserWritingDto = new ApplicationUserWritingDto
                {
                    Email = GetAdminMail,
                    Password = GetAdminPassword,
                    FullName = "admin for tests",
                    Initials = "AD",
                    ApplicationRoleId = Context.ApplicationRole.FirstOrDefault(role => role.Name.Equals(ApplicationRole.SuperuserRole)).Id,
                    CountryId = 4 // Argentina
                };

                if (!Context.ApplicationUsers.Any(x => x.Initials.Equals("AD")))
                {
                    EmailSenderExtensions.OnEmailEvents += (sender, args) => confirmationCode = sender.ToString();

                    user = (ApplicationUsersController.Post(applicationUserWritingDto).Result as CreatedResult)?.Value as ApplicationUserWritingDto
                        ?? throw new InvalidOperationException("No se pudo crear el usuario");

                    var applicationUser = Context.ApplicationUsers.Include(u => u.CredentialsUser).Single(u => u.Id == user.Id);
                    var userConfirmed = (ApplicationUsersController.Confirm(applicationUser.CredentialsUser.Id, confirmationCode).Result as OkObjectResult)?.Value as ApplicationUser
                                        ?? throw new InvalidOperationException("No se pudo confirmar el usuario");
                }

                return Ok(new { token = ApplicationUsersController.GetTokenString(applicationUserWritingDto.Email) });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
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