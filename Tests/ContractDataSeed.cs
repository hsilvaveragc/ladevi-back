using LadeviVentasApi.Controllers;
using LadeviVentasApi.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.ClientsMgmt;

namespace Tests
{
    public class ContractDataSeed
    {
        public static void Run(WebAppFixture fixture)
        {
            /* var seed = fixture.Send<IntegrationTestSeedController>(
                nameof(IntegrationTestSeedController.RegisterUserSeed),
                routeValues: new { token = "tfENXZ840DEO7GKVPQi3" }
            ).Result;
            fixture.Tokens.Add("admin", seed["token"].ToString());
            fixture.CurrentUser = "admin"; */

            /*var c1 = fixture.SearchByAttr<Country, CountryController>(nameof(Country.Name), "Argentina").First();

            //Tipo producto
            var tp = fixture.Send<ProductType, ProductTypeController>
            (
                nameof(ProductTypeController.Post),
                bodyData: new ProductType { Name = "Test" }
            ).Result;

            // Producto
            var p = fixture.Send<Product, ProductsController>
            (
                nameof(ProductsController.Post),
                bodyData: new ProductWritingDto
                {
                    ProductTypeId = tp.Id,
                    CountryId = c1.Id,
                    Name = "Revista Ole",
                    DiscountForCheck = 0.05,
                    DiscountForLoyalty = 0.07,
                    DiscountForAgency = 0.04,
                    DiscountForSameCountry = 0.05,
                    DiscountForOtherCountry = 0.06,
                    DiscountSpecialBySeller = 0.08,
                    DiscountByManager = 0.03,
                    MaxAplicableDiscount = 0.1,
                    AliquotForSalesCommission = 2,
                    ProductCurrencyParities = new List<ProductCurrencyParity>
                    {
                        { new ProductCurrencyParity() { Start = DateTime.Now.AddDays(-10), End = DateTime.Now.AddYears(2), LocalCurrencyToDollarExchangeRate = 65 } }
                    }
                }
            ).Result;*/

            //Ediciones de producto
            //Edicion cerrada
            /*var ep1 = fixture.Send<ProductEdition, ProductEditionController>
                (
                    nameof(ProductEditionController.Post),
                    bodyData: new ProductEditionWritingDto
                    {
                        Code = "A01",
                        Name = "Edicion Noviembre Revista Ole",
                        ProductId = p.Id,
                        Start = new DateTime(2019, 11, 1),
                        End = new DateTime(2019, 11, 30, 23, 59, 59)
                    }
                );

            //Edicion abierta
            var ep2 = fixture.Send<ProductEdition, ProductEditionController>
                (
                    nameof(ProductEditionController.Post),
                    bodyData: new ProductEditionWritingDto
                    {
                        Code = "A02",
                        Name = "Edicion actual Revista Ole",
                        ProductId = p.Id,
                        Start = DateTime.Now.AddDays(-30),
                        End = DateTime.Now.AddDays(1)
                    }
                );
            //Tipo de espacio
            var te = fixture.Send<ProductAdvertisingSpace, ProductAdvertisingSpaceController>
                (
                    nameof(ProductAdvertisingSpaceController.Post),
                    bodyData: new ProductAdvertisingSpaceWritingDto
                    {
                        DollarPrice = 10,
                        Height = 10,
                        Name = "Tapa Revista Ole",
                        Width = 5,
                        ProductId = p.Id
                    }
                ).Result;

            //Tax types
            var tt = fixture.Send<TaxType, TaxTypeController>
                (
                    nameof(TaxTypeController.Post),
                    bodyData: new TaxTypeWritingDto
                    {
                        CountryId = c1.Id,
                        Name = "IVA",
                        Options = new[] { "21", "11" },
                        IsIdentificationField = false
                    }
                ).Result;*/

            /*var clientOk01 = fixture.Send<ClientsController>(
                nameof(ClientsController.Post),
                bodyData: ClientsMgmtIntegrationTests.GetValidClient(fixture)
            ).Result;
            var clientId = clientOk01["id"].ToObject<long>();*/
        }
    }
}
