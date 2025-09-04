
using LadeviVentasApi.Controllers;
using Tests.Helpers.Fixtures;

namespace Tests.Helpers
{
    public class BasicDataSeed
    {
        public static void Run(WebAppFixtureBase fixture)
        {
            var seed = fixture.Send<IntegrationTestSeedController>(
                nameof(IntegrationTestSeedController.BasicDataSeed),
                routeValues: new { token = "tfENXZ840DEO7GKVPQi3" }
            ).Result;
            fixture.Tokens.Add("admin", seed["token"].ToString());
            fixture.CurrentUser = "admin";
        }
    }
}