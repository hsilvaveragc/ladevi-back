// using System;
// using System.Collections.Generic;
// using System.Linq;
// using LadeviVentasApi.Controllers;
// using LadeviVentasApi.Models.Domain;
// using Tests.Auth;
// using Xunit;

// namespace Tests.ProductsMgmt
// {
//     public class ProductsMgmtIntegrationTests : AuthWebAppWithUserCollection
//     {
//         public ProductsMgmtIntegrationTests(AuthWebAppWithUserFixture fixture) : base(fixture) { }

//         [Fact]
//         public void CreateProductsHappyPath()
//         {
//             /* var countryId = Fixture.Search<Country, CountryController>().First().Id;

//             //creamos product type
//             var productTypeId = Fixture.Send<ProductTypeController>(
//                 nameof(ProductTypeController.Post),
//                 bodyData: new ProductType { Name = "Folleto Mensual" }
//             ).Result["id"].ToObject<long>();

//             //creamos tipo de ubicacion de espacio publicitario
//             var advertisingSpaceLocationTypeId = Fixture.Send<AdvertisingSpaceLocationTypeController>(
//                 nameof(AdvertisingSpaceLocationTypeController.Post),
//                 bodyData: new AdvertisingSpaceLocationType { Name = "Antes de Central" }
//             ).Result["id"].ToObject<long>();

//             //creamos producto
//             var product = Fixture.Send<ProductsController>(
//                 nameof(ProductsController.Post),
//                 bodyData: new ProductWritingDto
//                 {
//                     Name = "Folleto de Mendoza",
//                     CountryId = countryId,
//                     ProductTypeId = productTypeId,
//                     ProductCurrencyParities = new List<ProductCurrencyParity>
//                     {
//                         new ProductCurrencyParity
//                         {
//                             Start = new DateTime(2019, 01, 01),
//                             End = new DateTime(2019, 09, 30),
//                             LocalCurrencyToDollarExchangeRate = 43
//                         },
//                         new ProductCurrencyParity
//                         {
//                             Start = new DateTime(2019, 10, 01),
//                             End = new DateTime(2019, 12, 31),
//                             LocalCurrencyToDollarExchangeRate = 46
//                         }
//                     },
//                     ProductVolumeDiscounts = new List<ProductVolumeDiscount>
//                     {
//                         new ProductVolumeDiscount
//                         {
//                             RangeStart = 10,
//                             RangeEnd = 100,
//                             Discount = 0.1
//                         }
//                     },
//                     ProductLocationDiscounts = new List<ProductLocationDiscount>
//                     {
//                         new ProductLocationDiscount
//                         {
//                             AdvertisingSpaceLocationTypeId = advertisingSpaceLocationTypeId,
//                             Discount = 0.1
//                         }
//                     }
//                 }
//             ).Result;
//             Assert.NotNull(product["id"]);
//             Assert.NotNull(product["productCurrencyParities"][1]["id"]);
//             Assert.Equal(46, product["productCurrencyParities"][1]["localCurrencyToDollarExchangeRate"]);
//             Assert.NotNull(product["productVolumeDiscounts"][0]["id"]);
//             Assert.NotNull(product["productLocationDiscounts"][0]["id"]);

//             //creamos edicion
//             var edition = Fixture.Send<ProductEditionController>(
//                 nameof(ProductEditionController.Post),
//                 bodyData: new ProductEditionWritingDto
//                 {
//                     Code = "edicion 01",
//                     Name = "es la edicion nro 01",
//                     Start = new DateTime(2019, 3, 1),
//                     End = new DateTime(2019, 7, 31),
//                     ProductId = product["id"].ToObject<long>()
//                 }
//             ).Result;
//             Assert.NotNull(edition["id"]);

//             //creamos espacio publicitario
//             var productAdvertisingSpace = Fixture.Send<ProductAdvertisingSpaceController>(
//                 nameof(ProductAdvertisingSpaceController.Post),
//                 bodyData: new ProductAdvertisingSpaceWritingDto
//                 {
//                     Name = "foto mediana",
//                     ProductId = product["id"].ToObject<long>(),
//                     DollarPrice = 50,
//                     Height = 12,
//                     Width = 4
//                 }
//             ).Result;
//             Assert.NotNull(productAdvertisingSpace["id"]); */
//         }
//     }
// }
