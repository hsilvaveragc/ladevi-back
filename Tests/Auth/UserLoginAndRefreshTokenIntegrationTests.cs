using System.Threading.Tasks;
using LadeviVentasApi.Controllers;
using Xunit;

namespace Tests.Auth
{
    public class UserLoginAndRefreshTokenIntegrationTests : AuthWebAppWithUserCollection
    {
        public UserLoginAndRefreshTokenIntegrationTests(AuthWebAppWithUserFixture fixture) : base(fixture) { }

        [Fact]
        public async Task UserUseRefreshToken()
        {
            //hacemos login correcto con el nuevo pass
            /*var loginOk = Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Login),
                routeValues: new
                {
                    email = UsersSeed.GetUserMail(),
                    password = UsersSeed.GetPassword()
                }
            ).Result;
            var token = loginOk["token"].ToString();
            var refreshToken = loginOk["refreshToken"].ToString();
            Assert.NotNull(token);
            Assert.NotNull(refreshToken);

            //usamos el refresh erroneo
            var refreshError = Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.RefreshToken),
                routeValues: new { refreshToken = "aaa" },
                shouldSucceed: false
            ).Result;
            Assert.NotNull(refreshError["errors"]);

            //usamos el refresh ok
            var refreshOk = Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.RefreshToken),
                routeValues: new { refreshToken }
            ).Result;
            var token2 = refreshOk["token"].ToString();
            var refreshToken2 = loginOk["refreshToken"].ToString();
            Assert.NotNull(token2);
            Assert.NotNull(refreshToken2);
            Assert.NotEqual(token, token2);
            Assert.Equal(refreshToken, refreshToken2);*/
        }
    }
}
