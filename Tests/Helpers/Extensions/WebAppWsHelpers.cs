using KendoNET.DynamicLinq;
using LadeviVentasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Tests.Helpers.Fixtures;

namespace Tests
{
    /// <summary>
    /// Métodos de extensión para WebAppFixture que simplifican las operaciones comunes de testing
    /// contra controladores REST. Proporciona métodos de alto nivel para operaciones CRUD típicas.
    /// </summary>
    /// <remarks>
    /// Estos helpers encapsulan patrones comunes como:
    /// - Obtener entidades por ID
    /// - Realizar búsquedas paginadas 
    /// - Filtrar por atributos específicos
    /// Todos los métodos usan .Result para convertir operaciones async en síncronas 
    /// (apropiado solo para testing, no para código de producción)
    /// </remarks>
    public static class WebAppWsHelpers
    {
        /// <summary>
        /// Obtiene una entidad por su ID realizando un GET request al endpoint GetById del controlador.
        /// </summary>
        /// <typeparam name="T">Tipo de entidad a obtener</typeparam>
        /// <typeparam name="TControllerType">Tipo del controlador que maneja la entidad</typeparam>
        /// <param name="fixture">Fixture de testing a usar</param>
        /// <param name="id">ID de la entidad a buscar</param>
        /// <returns>La entidad encontrada o null si no existe</returns>
        /// <example>
        /// var user = fixture.GetById&lt;ApplicationUser, ApplicationUsersController&gt;(123);
        /// var product = fixture.GetById&lt;Product, ProductsController&gt;(456);
        /// </example>
        public static T GetById<T, TControllerType>(this WebAppFixtureBase fixture, long id) where TControllerType : ControllerBase
        {
            return fixture.Send<T, TControllerType>("GetById", routeValues: new { id }, method: HttpMethod.Get).Result;
        }

        /// <summary>
        /// Realiza una búsqueda paginada usando Kendo Grid request al endpoint Search del controlador.
        /// Si no se proporciona request, usa valores por defecto (take = 10).
        /// </summary>
        /// <typeparam name="T">Tipo de entidad a buscar</typeparam>
        /// <typeparam name="TControllerType">Tipo del controlador que maneja la entidad</typeparam>
        /// <param name="fixture">Fixture de testing a usar</param>
        /// <param name="request">Request de búsqueda con paginación, filtros, etc. Si es null, usa valores por defecto</param>
        /// <returns>Lista de entidades que cumplen los criterios de búsqueda</returns>
        /// <example>
        /// // Búsqueda simple (primeros 10 registros)
        /// var users = fixture.Search&lt;ApplicationUser, ApplicationUsersController&gt;();
        /// 
        /// // Búsqueda con filtros personalizados
        /// var request = new KendoGridSearchRequest { take = 5, skip = 0 };
        /// var products = fixture.Search&lt;Product, ProductsController&gt;(request);
        /// </example>
        public static List<T> Search<T, TControllerType>(
            this WebAppFixtureBase fixture,
            KendoGridSearchRequestExtensions.KendoGridSearchRequest request = null
        ) where TControllerType : ControllerBase
        {
            return fixture.Send<DataSourceResult<T>, TControllerType>("Search", bodyData: (object)request ?? new { take = 10 }).Result.Data;
        }

        /// <summary>
        /// Busca entidades filtrando por un atributo específico con un valor exacto (operador "eq").
        /// Útil para encontrar registros por nombre, código, email, etc.
        /// </summary>
        /// <typeparam name="T">Tipo de entidad a buscar</typeparam>
        /// <typeparam name="TControllerType">Tipo del controlador que maneja la entidad</typeparam>
        /// <param name="fixture">Fixture de testing a usar</param>
        /// <param name="attrName">Nombre del campo/propiedad a filtrar</param>
        /// <param name="attrValue">Valor exacto a buscar</param>
        /// <returns>Lista de entidades donde el campo especificado tiene el valor buscado</returns>
        /// <example>
        /// // Buscar usuario por email
        /// var users = fixture.SearchByAttr&lt;ApplicationUser, ApplicationUsersController&gt;("Email", "admin@test.com");
        /// 
        /// // Buscar país por nombre
        /// var countries = fixture.SearchByAttr&lt;Country, CountryController&gt;("Name", "Argentina");
        /// 
        /// // Buscar rol por nombre usando nameof para type safety
        /// var roles = fixture.SearchByAttr&lt;ApplicationRole, ApplicationRoleController&gt;(
        ///     nameof(ApplicationRole.Name), ApplicationRole.SuperuserRole);
        /// </example>
        public static List<T> SearchByAttr<T, TControllerType>(
            this WebAppFixtureBase fixture,
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