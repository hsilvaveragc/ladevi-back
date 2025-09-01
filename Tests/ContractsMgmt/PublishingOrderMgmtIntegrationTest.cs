using LadeviVentasApi.Controllers;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests.Auth;
using Xunit;

namespace Tests.ContractsMgmt
{
    public class PublishingOrderMgmtIntegrationTest : AuthWebAppWithContractCollection
    {
        public PublishingOrderMgmtIntegrationTest(ContractFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void TestCreatePublishingOrder()
        {
            //var publishingOrderWritingDto = GetValidPublishingOrder();

            //Publicación válida
            /*var publishingOrder = Fixture.Send<PublishingOrderController>(
                nameof(PublishingOrderController.Post),
                bodyData: publishingOrderWritingDto
            ).Result;
            Assert.NotNull(publishingOrder["id"]);*/

            //var nationalSeller = Fixture.SearchByAttr<ApplicationUser, ApplicationUsersController>(nameof(ApplicationUser.Initials), "national-seller").First();
            /*Fixture.CurrentUser = nationalSeller.Initials;
            //orden de publicacion invalida, con vendedor asignado al cliente, que no es el usuario logueado siendo este vendedor nacional
            var error01 = Fixture.Send<PublishingOrderController>(
                nameof(PublishingOrderController.Post),
                bodyData: publishingOrderWritingDto,
                shouldSucceed: false
            ).Result;
            Assert.NotNull(error01["errors"]["clientId"]);*/

            //Fixture.CurrentUser = "admin";
            //Order de publicación inválida con edición cerrada
            /*var productEditionId = Fixture.SearchByAttr<ProductEdition, ProductEditionController>(nameof(ProductEdition.Code), "A01").First().Id;
            var error02 = Fixture.Send<PublishingOrderController>(
                nameof(PublishingOrderController.Post),
                bodyData: GetValidPublishingOrder(op => op.ProductEditionId = productEditionId),
                shouldSucceed: false
            ).Result;
            Assert.NotNull(error02["errors"]["productEditionId"]);*/

        }

        public PublishingOrderWritingDto GetValidPublishingOrder(Action<PublishingOrderWritingDto> postBuild = null)
        {
            //var clientId = Fixture.Search<Client, ClientsController>().First().Id;
            var productEditionId = Fixture.SearchByAttr<ProductEdition, ProductEditionController>(nameof(ProductEdition.Code), "A02").First().Id;
            var ubicacionId = Fixture.Search<AdvertisingSpaceLocationType, AdvertisingSpaceLocationTypeController>().First().Id;
            var tipoEspacioId = Fixture.Search<ProductAdvertisingSpace, ProductAdvertisingSpaceController>().First().Id;

            var op = new PublishingOrderWritingDto
            {
                Latent = true,
                ClientId = 1,
                //ClientId = clientId,
                ProductEditionId = productEditionId,
                AdvertisingSpaceLocationTypeId = ubicacionId,
                ProductAdvertisingSpaceId = tipoEspacioId,
                ContractId = null,
                PageNumber = "0",
                InvoiceNumber = string.Empty,
                PaidOut = null,
                Quantity = 1,
                Observations = ""
            };

            postBuild?.Invoke(op);

            return op;
        }
    }
}
