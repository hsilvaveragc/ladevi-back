using System;
using System.Collections.Generic;
using System.Linq;
using LadeviVentasApi.Controllers;
using LadeviVentasApi.Models.Domain;
using Tests.Auth;
using Xunit;

namespace Tests.ClientsMgmt
{
    public class ClientsMgmtIntegrationTests : AuthWebAppWithUserCollection
    {
        public ClientsMgmtIntegrationTests(AuthWebAppWithUserFixture fixture) : base(fixture) { }

        [Fact]
        public void CreateTaxTypeAndClient()
        {
            /*var countryId = Fixture.Search<Country, CountryController>().First().Id;

            //creamos tax type invalido por el pais
            var error01 = Fixture.Send<TaxTypeController>(
                nameof(TaxTypeController.Post),
                bodyData: new TaxTypeWritingDto
                {
                    CountryId = -1,
                    Name = "IVA",
                    Options = new string[0]
                },
                shouldSucceed: false
            ).Result;
            Assert.NotNull(error01["errors"]["countryId"]);

            //creamos tax type invalido por el pais
            var error02 = Fixture.Send<TaxTypeController>(
                nameof(TaxTypeController.Post),
                bodyData: new TaxTypeWritingDto
                {
                    CountryId = countryId,
                    Name = null,
                    Options = new string[0]
                },
                shouldSucceed: false
            ).Result;
            Assert.NotNull(error02["errors"]["name"]);

            //creamos tax type valido
            var taxtype = Fixture.Send<TaxTypeController>(
                nameof(TaxTypeController.Post),
                bodyData: new TaxTypeWritingDto
                {
                    CountryId = countryId,
                    Name = "IVA",
                    Options = new[] { "21", "11" },
                    IsIdentificationField = false
                }
            ).Result;
            Assert.NotNull(taxtype["id"]);

            //creamos tax identificator valido
            var taxIdentificator = Fixture.Send<TaxTypeController>(
                nameof(TaxTypeController.Post),
                bodyData: new TaxTypeWritingDto
                {
                    CountryId = countryId,
                    Name = "CUIT",
                    Options = new string[0],
                    IsIdentificationField = true
                }
            ).Result;
            Assert.NotNull(taxtype["id"]);

            //creamos el client con ciudad invalida
            var clientError01 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: GetValidClient(Fixture, c => c.CityId = -1),
                shouldSucceed: false
            ).Result;
            Assert.NotNull(clientError01["errors"]["cityId"]);

            //creamos el client con ciudad valida pero que es de otro pais distinto del usuario vendedor, tmb pincha porque no hay tax types
            var jurere = Fixture.SearchByAttr<City, CityController>(nameof(City.Name), "Jurere").First();
            var clientWritingDto = GetValidClient(Fixture, c => c.CityId = jurere.Id);
            Fixture.CurrentUser = "supervisor";
            var clientError02 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: clientWritingDto,
                shouldSucceed: false
            ).Result;*/
            //Assert.NotNull(clientError02["errors"]["cityId"]);

            //creamos el client por un vendedor pero asignado a otro vendedor
            /*var comturSeller = Fixture.SearchByAttr<ApplicationUser, ApplicationUsersController>(nameof(ApplicationUser.Initials), "comtur-seller").First();
            var clientWritingDto2 = GetValidClient(Fixture, c => c.ApplicationUserSellerId = comturSeller.Id);
            Fixture.CurrentUser = "national-seller";
            var clientError03 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: clientWritingDto2,
                shouldSucceed: false
            ).Result;*/
            //Assert.NotNull(clientError03["errors"]["applicationUserSellerId"]);

            //creamos el client con un supervisor, de manera ok
            /*Fixture.CurrentUser = "supervisor";
            var clientOk01 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: GetValidClient(Fixture)
            ).Result;*/
            /*Assert.NotNull(clientOk01["id"]);
            Assert.Equal("123", clientOk01["clientTaxes"][0]["value"]);
            Assert.Equal("21", clientOk01["clientTaxes"][1]["value"]);*/

            //creamos el client, con un vendedor pero asignandoselo a otro, por ende falla
            /*Fixture.CurrentUser = comturSeller.Initials;
            var clientError04 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: GetValidClient(Fixture),
                shouldSucceed: false
            ).Result;*/
            //Assert.NotNull(clientError04["errors"]["applicationUserSellerId"]);

            //creamos el client ok, con un vendedor asignandoselo al cliente a el mismo
            /*Fixture.CurrentUser = comturSeller.Initials;
            var clientOk02 = Fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: GetValidClient(Fixture, c =>
                {
                    c.ApplicationUserSellerId = c.ApplicationUserDebtCollectorId = comturSeller.Id;
                    c.BrandName = c.LegalName = "cli 02";
                })
            ).Result;
            Assert.NotNull(clientOk02["id"]);
            Assert.Equal("123", clientOk02["clientTaxes"][0]["value"]);
            Assert.Equal("21", clientOk02["clientTaxes"][1]["value"]);*/

        }

        public static ClientWritingDto GetValidClient(WebAppFixture fixture, Action<ClientWritingDto> postBuild = null)
        {
            var cityId = fixture.Search<City, CityController>().First().Id;
            var taxTypes = fixture.Search<TaxType, TaxTypeController>().ToList();
            var userId = fixture.Search<ApplicationUser, ApplicationUsersController>().First().Id;
            var taxIdentificationField = taxTypes.FirstOrDefault(t => t.IsIdentificationField);
            var clientTaxesWritingDtos = new List<ClientTaxesWritingDto>();
            if (taxIdentificationField != null)
            {
                clientTaxesWritingDtos.Add(new ClientTaxesWritingDto
                {
                    TaxTypeId = taxIdentificationField.Id,
                    Value = "123"
                });
            }
            taxTypes.Where(t => !t.IsIdentificationField)
                .ToList().ForEach(taxType => clientTaxesWritingDtos.Add(
                        new ClientTaxesWritingDto
                        {
                            TaxTypeId = taxType.Id,
                            Value = taxType.Options.Any() ? taxType.Options.First() : ""
                        }
                    ));

            var c = new ClientWritingDto
            {
                CityId = cityId,
                // ClientTaxes = clientTaxesWritingDtos,
                BrandName = "cli 01",
                LegalName = "cli 01",
                Address = "dire 123",
                MainEmail = "mail@pepe.com",
                AlternativeEmail = "mail2@pepe.com",
                ElectronicBillByMail = true,
                IsEnabled = true,
                ApplicationUserDebtCollectorId = userId,
                ApplicationUserSellerId = userId,
                BillingPointOfSale = "1234",
                PostalCode = "1234",
                TelephoneCountryCode = "54",
                TelephoneAreaCode = "11",
                TelephoneNumber = "12345678"
            };
            postBuild?.Invoke(c);
            return c;
        }
    }
}
