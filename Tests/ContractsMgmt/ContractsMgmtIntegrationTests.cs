// using LadeviVentasApi.Controllers;
// using LadeviVentasApi.DTOs;
// using LadeviVentasApi.Models.Domain;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using Tests.Auth;
// using Xunit;

// namespace Tests.ContractsMgmt
// {
//     public class ContractsMgmtIntegrationTests : AuthWebAppWithContractCollection
//     {
//         public ContractsMgmtIntegrationTests(ContractFixture fixture) : base(fixture)
//         {
//         }

//         [Fact]
//         public void TestCreateContract()
//         {
//             //contrato invalido con fecha de fin de contrato menor a la fecha de inicio
//             /*var error01 = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract(c =>
//                 {
//                     c.Start = DateTime.Now;
//                     c.End = DateTime.Now.AddDays(-1);
//                 }),
//                 shouldSucceed: false
//             ).Result;
//             //Assert.NotNull(error01["errors"]["start"]);

//             //contrato invalido con condicion de facturacion Canje y con factura
//             long canjeId = Fixture.SearchByAttr<BillingCondition, BillingConditionController>(nameof(BillingCondition.Name), BillingCondition.Exchange).First().Id;
//             var error02 = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract(c =>
//                 {
//                     c.BillingConditionId = canjeId;
//                     c.InvoiceNumber = "021-5544987";
//                 }),
//                 shouldSucceed: false
//             ).Result;
//             //Assert.NotNull(error02["errors"]["invoiceNumber"]);

//             //contrato invalido con metodo de pago diferente de documentado y con cheques
//             long onePaymentId = Fixture.SearchByAttr<PaymentMethod, PaymentMethodController>(nameof(PaymentMethod.Name), PaymentMethod.OnePayment).First().Id;
//             var error03 = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract(c =>
//                 {
//                     c.PaymentMethodId = onePaymentId;
//                     c.InvoiceNumber = "021-5544987";
//                     c.CheckQuantity = 3;
//                     c.DaysToFirstPayment = 15;
//                     c.DaysBetweenChecks = 30;
//                 }),
//                 shouldSucceed: false
//             ).Result;
//             //Assert.NotNull(error03["errors"]["paymentMethodId"]);

//             //contrato invalido sin numero de factura y marcada como pagada
//             var error04 = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract(c =>
//                 {
//                     c.InvoiceNumber = string.Empty;
//                     c.PaidOut = true;
//                 }),
//                 shouldSucceed: false
//             ).Result;*/
//             //Assert.NotNull(error04["errors"]["paymentMethodId"]);

//             /*var nationalSeller = Fixture.SearchByAttr<ApplicationUser, ApplicationUsersController>(nameof(ApplicationUser.Initials), "national-seller").First();
//             Fixture.CurrentUser = nationalSeller.Initials;
//             //contrato invalido con vendedor asignado al cliente que no es el usuario logueado siendo este vendedor nacional
//             var error05 = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract(),
//                 shouldSucceed: false
//             ).Result;*/
//             //Assert.NotNull(error05["errors"]["applicationUserSellerId"]);

//             //contrato valido
//             /*var contract = Fixture.Send<ContractController>(
//                 nameof(ContractController.Post),
//                 bodyData: GetValidContract()
//             ).Result;*/
//             //Assert.NotNull(contract["id"]);
//         }

//         private ContractWritingDto GetValidContract(Action<ContractWritingDto> postBuild = null)
//         {
//             var countryId = Fixture.Search<Country, CountryController>().First().Id;
//             var product = Fixture.Search<Product, ProductsController>().First();
//             //var clientId = Fixture.Search<Client, ClientsController>().First().Id;
//             var userId = Fixture.Search<ApplicationUser, ApplicationUsersController>().First().Id;
//             var billingConditionId = Fixture.SearchByAttr<BillingCondition, BillingConditionController>(nameof(BillingCondition.Name), BillingCondition.Anticipated).First().Id;
//             var paymentMethodId = Fixture.Search<PaymentMethod, PaymentMethodController>().First().Id;
//             var currencyId = Fixture.SearchByAttr<Currency, CurrencyController>(nameof(Currency.Name), Currency.USS).First().Id;
//             var advertisingSpaceLocationTypeId = Fixture.SearchByAttr<AdvertisingSpaceLocationType, AdvertisingSpaceLocationTypeController>(
//                     nameof(AdvertisingSpaceLocationType.Name), AdvertisingSpaceLocationType.AfterCentral
//                 )
//                 .First().Id;
//             var productAdversitingSpaceId = Fixture.Search<ProductAdvertisingSpace, ProductAdvertisingSpaceController>().First().Id;
//             var soldSpaces = new List<SoldSpaceWritingDto>();
//             soldSpaces.Add(new SoldSpaceWritingDto
//             {
//                 AdvertisingSpaceLocationTypeId = advertisingSpaceLocationTypeId,
//                 ProductAdvertisingSpaceId = productAdversitingSpaceId,
//                 TypeSpecialDiscount = 1,
//                 TypeGerentialDiscount = 1,
//                 Quantity = 5,
//                 SpecialDiscount = 0.05,
//                 GerentialDiscount = 0.07,
//                 // Total = 53.24,
//                 DescriptionSpecialDiscount = "Descuento espacial",
//                 DescriptionGerentialDiscount = "Descuento Gerencial"
//             });

//             var c = new ContractWritingDto
//             {
//                 ProductId = product.Id,
//                 ClientId = 1,
//                 //ClientId = clientId,
//                 SellerId = userId,
//                 BillingConditionId = billingConditionId,
//                 BillingCountryId = countryId,
//                 PaymentMethodId = paymentMethodId,
//                 CurrencyId = currencyId,
//                 Start = DateTime.Now,
//                 End = DateTime.Now.AddYears(1),
//                 //ApplyDiscountForCheck = true,
//                 //ApplyDiscountForLoyalty = true,
//                 //ApplyDiscountForSameCountry = true,
//                 //ApplyDiscountForOtherCountry = false,
//                 //AppyDiscountForAgency = false,
//                 //ApplyDiscountForVolume = false,
//                 //DiscountForCheck = product.DiscountForCheck,
//                 //DiscountForLoyalty = product.DiscountForLoyalty,
//                 //DiscountForAgency = product.DiscountForAgency,
//                 //DiscountForSameCountry = product.DiscountForSameCountry,
//                 //DiscountForOtherCountry = product.DiscountForOtherCountry,
//                 Name = "Test Contract 1",
//                 SoldSpaces = soldSpaces
//             };

//             postBuild?.Invoke(c);

//             return c;
//         }
//     }
// }
