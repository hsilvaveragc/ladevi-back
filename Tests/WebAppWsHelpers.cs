using System.Collections.Generic;
using System.Net.Http;
using KendoNET.DynamicLinq;
using LadeviVentasApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public static class WebAppWsHelpers
    {
        public static T GetById<T, TControllerType>(this WebAppFixture fixture, long id) where TControllerType : ControllerBase
        {
            return fixture.Send<T, TControllerType>("GetById", routeValues: new { id }, method: HttpMethod.Get).Result;
        }

        public static List<T> Search<T, TControllerType>(
            this WebAppFixture fixture,
            KendoGridSearchRequestExtensions.KendoGridSearchRequest request = null
        ) where TControllerType : ControllerBase
        {
            return fixture.Send<DataSourceResult<T>, TControllerType>("Search", bodyData: (object)request ?? new { take = 10 }).Result.Data;
        }

        public static List<T> SearchByAttr<T, TControllerType>(
            this WebAppFixture fixture,
            string attrName, string attrValue
        ) where TControllerType : ControllerBase
        {
            var bodyData = new KendoGridSearchRequestExtensions.KendoGridSearchRequest
            {
                take = 10,
                filter = new Filter
                {
                    Logic = "and",
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Field = attrName,
                            Operator = "eq",
                            Value = attrValue
                        }
                    }
                }
            };
            return fixture.Send<DataSourceResult<T>, TControllerType>("Search", bodyData: bodyData).Result.Data;
        }
    }
}