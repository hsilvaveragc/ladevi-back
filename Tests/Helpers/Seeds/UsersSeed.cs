using LadeviVentasApi.Controllers;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models;
using LadeviVentasApi.Models.Domain;
using Tests.Helpers.Fixtures;

namespace Tests
{
    public class UsersSeed
    {
        public static string GetUserMail() => IntegrationTestSeedController.GetAdminMail;
        public static string GetPassword() => IntegrationTestSeedController.GetAdminPassword;
        public static void Run(WebAppFixtureBase fixture)
        {
            // CreateUser(fixture, "admin", ApplicationRole.SuperuserRole);
            // CreateUser(fixture, "supervisor", ApplicationRole.SupervisorRole);
            // CreateUser(fixture, "national-seller", ApplicationRole.NationalSellerRole);
            // CreateUser(fixture, "comtur-seller", ApplicationRole.COMTURSellerRole);
            var seed = fixture.Send<IntegrationTestSeedController>(
    nameof(IntegrationTestSeedController.CreateTestUsers),
    routeValues: new { token = "tfENXZ840DEO7GKVPQi3" }
).Result;
        }

        private static void CreateUser(WebAppFixtureBase fixture, string alias, string role)
        {
            string confirmationCode = null;
            EmailSenderExtensions.OnEmailEvents += (sender, args) => confirmationCode = sender.ToString();
            var argentina = fixture.SearchByAttr<Country, CountryController>(nameof(Country.Name), "Argentina").First();
            //intentamos crear un user valido
            var user01 = fixture.Send<ApplicationUserWritingDto, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto
                {
                    Email = $"{alias}@mail.com",
                    Password = GetPassword(),
                    FullName = alias,
                    Initials = alias,
                    CountryId = argentina.Id,
                    ApplicationRoleId = fixture
                        .SearchByAttr<ApplicationRole, ApplicationRoleController>("Name", role)
                        .First().Id
                }
            ).Result;

            //hacemos el confirmar
            var userFull = fixture.GetById<ApplicationUser, ApplicationUsersController>(user01.Id);
            var userConfirmedOk = fixture.Send<ApplicationUser, ApplicationUsersController>(
                nameof(ApplicationUsersController.Confirm),
                routeValues: new
                {
                    userId = userFull.CredentialsUser.Id,
                    code = confirmationCode
                },
                method: HttpMethod.Get
            ).Result;

            //hacemos login correcto pero luego de confirmar
            var loginOk = fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Login),
                routeValues: new
                {
                    email = user01.Email,
                    password = user01.Password
                }
            ).Result;
            var token = loginOk["token"].ToString();

            fixture.Tokens.Add(alias, token);
        }
    }
}