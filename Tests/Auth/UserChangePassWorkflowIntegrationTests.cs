// using System.Threading.Tasks;
// using LadeviVentasApi.Controllers;
// using Xunit;

// namespace Tests.Auth
// {
//     public class UserChangePassWorkflowIntegrationTests : AuthWebAppWithUserCollection
//     {
//         public UserChangePassWorkflowIntegrationTests(AuthWebAppWithUserFixture fixture) : base(fixture) { }

//         [Fact]
//         public async Task UserChangeMyOwnPasswordWorkflow()
//         {
//             //trato de cambiar mal el password
//             /*var result01 = await Fixture.Send<ApplicationUsersController>(
//                 nameof(ApplicationUsersController.ChangePassword),
//                 bodyData: new ApplicationUsersController.ChangePass
//                 {
//                     Email = UsersSeed.GetUserMail(),
//                     CurrentPassword = "adasd"
//                 },
//                 shouldSucceed: false
//             );
//             Assert.NotNull(result01["errors"]);

//             //trato de cambiar mal el password 2
//             const string newPassword = "Xaaa999--";
//             var result02 = await Fixture.Send<ApplicationUsersController>(
//                 nameof(ApplicationUsersController.ChangePassword),
//                 bodyData: new ApplicationUsersController.ChangePass
//                 {
//                     Email = UsersSeed.GetUserMail(),
//                     CurrentPassword = UsersSeed.GetPassword(),
//                     NewPassword = newPassword,
//                     NewPassword2 = "Xaaa999--X"
//                 },
//                 shouldSucceed: false
//             );
//             Assert.NotNull(result02["errors"]);

//             //cambio el password ok
//             var ok = await Fixture.Send<ApplicationUsersController>(
//                 nameof(ApplicationUsersController.ChangePassword),
//                 bodyData: new ApplicationUsersController.ChangePass
//                 {
//                     Email = UsersSeed.GetUserMail(),
//                     CurrentPassword = UsersSeed.GetPassword(),
//                     NewPassword = newPassword,
//                     NewPassword2 = newPassword
//                 }
//             );
//             Assert.NotNull(ok["id"]);
//             Assert.Null(ok["errors"]);

//             //hacemos login correcto con el nuevo pass
//             var loginOk = Fixture.Send<ApplicationUsersController>(
//                 nameof(ApplicationUsersController.Login),
//                 routeValues: new
//                 {
//                     email = UsersSeed.GetUserMail(),
//                     password = newPassword
//                 }
//             ).Result;
//             var token = loginOk["token"].ToString();
//             Assert.NotNull(token);

//             //cambio el password ok para poner el mismo que antes
//             var ok2 = await Fixture.Send<ApplicationUsersController>(
//                 nameof(ApplicationUsersController.ChangePassword),
//                 bodyData: new ApplicationUsersController.ChangePass
//                 {
//                     Email = UsersSeed.GetUserMail(),
//                     CurrentPassword = newPassword,
//                     NewPassword = UsersSeed.GetPassword(),
//                     NewPassword2 = UsersSeed.GetPassword()
//                 }
//             );
//             Assert.NotNull(ok2["id"]);
//             Assert.Null(ok2["errors"]);*/
//         }
//     }
// }
