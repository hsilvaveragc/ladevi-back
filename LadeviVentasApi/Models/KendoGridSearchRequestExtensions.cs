using KendoNET.DynamicLinq;

namespace LadeviVentasApi.Models
{
    public static class KendoGridSearchRequestExtensions
    {
        public static DataSourceResult ToDataSourceResult<T>(this IQueryable<T> queryable,
            KendoGridSearchRequest request)
        {
            if (request.filter != null)
            {
                FilterToPascalCase(request.filter);
            }
            return queryable.ToDataSourceResult(request.take, request.skip, request.sort, request.filter, request.aggregates, request.group);
        }

        public class KendoGridSearchRequest
        {
            public int page;
            public int pageSize;
            public int take;
            public int skip;
            public IEnumerable<Sort> sort;
            public KendoNET.DynamicLinq.Filter filter;
            public IEnumerable<Aggregator> aggregates;
            public IEnumerable<KendoNET.DynamicLinq.Group> group;
        }

        public static void FilterToPascalCase(KendoNET.DynamicLinq.Filter filter)
        {
            if (filter == null) return;
            filter.Field = string.IsNullOrWhiteSpace(filter.Field) ?
                filter.Field
                : (filter.Field.Length > 0 ? filter.Field.Substring(0, 1).ToUpperInvariant() : "") + (filter.Field.Length > 1 ? filter.Field.Substring(1) : "");
            filter.Filters?.ToList().ForEach(FilterToPascalCase);
        }
    }
}