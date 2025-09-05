using LadeviVentasApi.Controllers;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;
using Tests.Helpers.Fixtures;
using Xunit;

namespace Tests.Auth
{
    public class UserCreationAndLoginWorkflowIntegrationTests : AuthWebAppCollection
    {
        public UserCreationAndLoginWorkflowIntegrationTests(AuthWebAppFixture fixture) : base(fixture) { }

        [Fact]
        public async Task ShouldCompleteFullUserLifecycle_FromCreationToTokenRefresh()
        {
            // Arrange - Setup data
            var validCountryId = Fixture.Search<Country, CountryController>().First().Id;
            var validRoleId = Fixture.SearchByAttr<ApplicationRole, ApplicationRoleController>("Name", ApplicationRole.SuperuserRole).First().Id;
            var uniqueEmail = $"test_user@mail.com";

            // Act & Assert 1: Verificar estado inicial - solo usuario admin existe
            var initialUsers = await Fixture.Send<DataSourceResult<ApplicationUser>, ApplicationUsersController>(
                nameof(ApplicationUsersController.Search),
                bodyData: new { take = 10 }
            );
            Assert.Single(initialUsers.Data);

            // Act & Assert 2: Validación falla con datos vacíos
            var emptyUserValidationResponse = await Fixture.Send<ErrorsApiResponse, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto(),
                shouldSucceed: false
            );
            AssertRequiredFieldValidationErrors(emptyUserValidationResponse);

            // Act & Assert 3: Validación falla con password débil
            var weakPasswordResponse = await Fixture.Send<ErrorsApiResponse, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto
                {
                    Email = uniqueEmail,
                    FullName = "Integration Test User",
                    Initials = "ITU",
                    CountryId = validCountryId,
                    ApplicationRoleId = validRoleId,
                    Password = "123" // Password débil
                },
                shouldSucceed: false
            );
            Assert.True(weakPasswordResponse.Errors.ContainsKey("password"));

            // Act & Assert 4: Validación falla con país inválido
            var invalidCountryResponse = await Fixture.Send<ErrorsApiResponse, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: new ApplicationUserWritingDto
                {
                    Email = uniqueEmail,
                    FullName = "Integration Test User",
                    Initials = "ITU",
                    CountryId = -1, // País inválido
                    ApplicationRoleId = validRoleId,
                    Password = "ValidPassword123!"
                },
                shouldSucceed: false
            );
            Assert.True(invalidCountryResponse.Errors.ContainsKey("countryId"));

            // Act & Assert 5: Creación exitosa con datos válidos
            var validUserDto = new ApplicationUserWritingDto
            {
                Email = uniqueEmail,
                Password = "ValidPassword123!",
                FullName = "Integration Test User",
                Initials = "ITU",
                CountryId = validCountryId,
                ApplicationRoleId = validRoleId
            };

            var createdUser = await Fixture.Send<ApplicationUserWritingDto, ApplicationUsersController>(
                nameof(ApplicationUsersController.Post),
                bodyData: validUserDto
            );

            Assert.NotNull(createdUser);
            Assert.Equal(validUserDto.Email, createdUser.Email);
            Assert.Equal(validUserDto.FullName, createdUser.FullName);

            // Act & Assert 6: Login exitoso con usuario creado   
            var initialLoginResponse = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.Login),
                routeValues: new
                {
                    email = createdUser.Email,
                    password = validUserDto.Password
                }
            );
            var originalToken = initialLoginResponse["token"]?.ToString();
            var validRefreshToken = initialLoginResponse["refreshToken"]?.ToString();

            Assert.False(string.IsNullOrWhiteSpace(originalToken));
            Assert.False(string.IsNullOrWhiteSpace(validRefreshToken));
            Assert.NotNull(initialLoginResponse["user"]?["id"]);

            // Act & Assert 7: Refresh token falla con token inválido
            var invalidRefreshResponse = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.RefreshToken),
                routeValues: new { refreshToken = "invalid-token" },
                shouldSucceed: false
            );
            Assert.NotNull(invalidRefreshResponse["errors"]);

            // Act & Assert 8: Refresh token exitoso con token válido
            var validRefreshResponse = await Fixture.Send<ApplicationUsersController>(
                nameof(ApplicationUsersController.RefreshToken),
                routeValues: new { refreshToken = validRefreshToken }
            );

            var newToken = validRefreshResponse["token"]?.ToString();
            var newRefreshToken = validRefreshResponse["refreshToken"]?.ToString();

            Assert.False(string.IsNullOrWhiteSpace(newToken));
            Assert.False(string.IsNullOrWhiteSpace(newRefreshToken));
            Assert.NotEqual(originalToken, newToken); // Nuevo token diferente
            Assert.NotEqual(validRefreshToken, newRefreshToken); // Refresh token permanece igual

            // Act & Assert 9: Usuario puede ser encontrado mediante búsqueda
            var foundUsers = Fixture.SearchByAttr<ApplicationUser, ApplicationUsersController>(
                nameof(ApplicationUser.FullName),
                createdUser.FullName
            );

            var foundUser = Assert.Single(foundUsers);
            Assert.Equal(createdUser.Id, foundUser.Id);

        }

        private static void AssertRequiredFieldValidationErrors(ErrorsApiResponse response)
        {
            var requiredFields = new[] { "FullName", "Initials", "Email", "ApplicationRoleId", "CountryId", "Password" };

            foreach (var field in requiredFields)
            {
                Assert.True(response.Errors.ContainsKey(field),
                    $"Expected validation error for required field: {field}");
            }
        }
    }
}