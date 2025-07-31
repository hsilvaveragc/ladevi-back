using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LadeviVentasApi.Controllers;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Xunit;

namespace Tests.Auth
{
    public class UserCreationAndLoginWorkflowIntegrationTests : AuthWebAppCollection
    {
        public UserCreationAndLoginWorkflowIntegrationTests(AuthWebAppFixture fixture) : base(fixture) { }

        [Fact]
        public async Task UserCreationWorkflow()
        {
            //verificamos que no hay users
            /*var users = await Fixture.Send<DataSourceResult<ApplicationUser>, ApplicationUsersController>(
                nameof(ApplicationUsersController.Search),
                bodyData: new { take = 10 }
            );
            Assert.Single(users.Data);

            //intentamos crear un user invalido sin datos
            var errorsResponse01 = await Fixture.Send<ErrorsApiResponse, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto(),
                shouldSucceed: false
            );
            Assert.NotEmpty(errorsResponse01.Errors);

            //intentamos crear un user invalido con password invalido
            var errorsResponse02 = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto { Password = "123" },
                shouldSucceed: false
            );
            Assert.Contains(
                errorsResponse02["errors"]["password"],
                x => x.ToString().ToLowerInvariant().Contains("password")
            );

            //intentamos crear un user invalido con password invalido
            var errorsResponse03 = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto { Password = "123aaaaaaaa" },
                shouldSucceed: false
            );
            Assert.Contains(
                errorsResponse03["errors"]["password"],
                x => x.ToString().ToLowerInvariant().Contains("password")
            );

            //intentamos crear un user invalido con pais invalido
            var errorsResponse04 = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto
                {
                    Email = "pepe@mail.com",
                    Password = "Xaaa111--",
                    CountryId = -1
                },
                shouldSucceed: false
            );
            Assert.Contains(
                errorsResponse04["errors"]["countryId"],
                x => true
            );

            string confirmationCode = null;
            EmailSenderExtensions.OnEmailEvents += (sender, args) => confirmationCode = sender.ToString();
            //intentamos crear un user valido
            var user01 = await Fixture.Send<ApplicationUserWritingDto, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto
                {
                    Email = "pepe@mail.com",
                    Password = "Xaaa111--",
                    FullName = "pepe admin",
                    Initials = "PA",
                    CountryId = Fixture.Search<Country, CountryController>().First().Id,
                    ApplicationRoleId = Fixture.SearchByAttr<ApplicationRole, ApplicationRoleController>("Name", ApplicationRole.SuperuserRole).First().Id
                }
            );
            Assert.NotNull(user01);*/

            //probamos login correcto pero sin confirmar
            /*var errorSinConfirmar = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Login),
                routeValues: new
                {
                    email = user01.Email,
                    password = user01.Password
                },
                shouldSucceed: false
            );
            Assert.NotNull(errorSinConfirmar);
            Assert.Null(errorSinConfirmar["token"]);*/

            //hacemos el confirmar
            /*var userFull = Fixture.GetById<ApplicationUser, ApplicationUsersController>(user01.Id);
            Assert.False(userFull.CredentialsUser.EmailConfirmed);
            var userConfirmedOk = await Fixture.Send<ApplicationUser, ApplicationUsersController>(
                nameof(ApplicationUsersController.Confirm),
                routeValues: new
                {
                    userId = userFull.CredentialsUser.Id,
                    code = confirmationCode
                },
                method: HttpMethod.Get
            );
            Assert.NotNull(userConfirmedOk);
            userFull = Fixture.GetById<ApplicationUser, ApplicationUsersController>(userConfirmedOk.Id);
            Assert.True(userFull.CredentialsUser.EmailConfirmed);

            //probamos login correcto pero luego de confirmar
            var loginOk = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Login),
                routeValues: new
                {
                    email = user01.Email,
                    password = user01.Password
                }
            );
            Assert.False(string.IsNullOrWhiteSpace(loginOk["token"].ToString()));
            Assert.False(string.IsNullOrWhiteSpace(loginOk["refreshToken"].ToString()));
            Assert.NotNull(loginOk["user"]["id"]);

            //busco el usuario creado
            var userSearch = Fixture
                .SearchByAttr<ApplicationUser, ApplicationUsersController>(nameof(userConfirmedOk.FullName), userConfirmedOk.FullName)
                .Single();
            Assert.NotNull(userSearch);
            Assert.Equal(user01.Id, userSearch.Id);*/
        }
    }
}
