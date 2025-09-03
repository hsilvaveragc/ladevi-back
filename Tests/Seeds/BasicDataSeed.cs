
using LadeviVentasApi.Controllers;
using LadeviVentasApi.DTOs;
using LadeviVentasApi.Models.Domain;

namespace Tests
{
    public class BasicDataSeed
    {
        public static void Run(WebAppFixture fixture)
        {
            var seed = fixture.Send<IntegrationTestSeedController>(
                nameof(IntegrationTestSeedController.RegisterUserSeed),
                routeValues: new { token = "tfENXZ840DEO7GKVPQi3" }
            ).Result;
            // fixture.Tokens.Add("admin", seed["token"].ToString());
            // fixture.CurrentUser = "admin";

            // //creamos algunos datos geograficos
            // var c1 = fixture.SearchByAttr<Country, CountryController>(nameof(Country.Name), "Argentina").First();

            // var s1 = fixture.Send<State, StateController>
            // (
            //     nameof(StateController.Post),
            //     bodyData: new State { Name = "Buenos Aires", CountryId = c1.Id }
            // ).Result;
            // var d1 = fixture.Send<District, DistrictController>
            // (
            //     nameof(DistrictController.Post),
            //     bodyData: new DistrictWritingDto { Name = "La Matanza", StateId = s1.Id }
            // ).Result;
            // var city1 = fixture.Send<City, CityController>
            // (
            //     nameof(CityController.Post),
            //     bodyData: new CityWritingDto { Name = "Ciudadela", DistrictId = d1.Id }
            // ).Result;


            // var country2 = fixture.Send<Country, CountryController>
            // (
            //     nameof(CountryController.Post),
            //     bodyData: new Country { Name = "Brasil" }
            // ).Result;
            // var s2 = fixture.Send<State, StateController>
            // (
            //     nameof(StateController.Post),
            //     bodyData: new State { Name = "Santa Catarina", CountryId = country2.Id }
            // ).Result;
            // var d2 = fixture.Send<District, DistrictController>
            // (
            //     nameof(DistrictController.Post),
            //     bodyData: new DistrictWritingDto { Name = "Floripa", StateId = s2.Id }
            // ).Result;
            // var city2 = fixture.Send<City, CityController>
            // (
            //     nameof(CityController.Post),
            //     bodyData: new CityWritingDto { Name = "Jurere", DistrictId = d2.Id }
            // ).Result;
        }
    }
}