using LadeviVentasApi.Projections;

namespace LadeviVentasApi.Helpers.Utilities
{
    public class AdvertisingSpaceClassifier
    {
        // Métodos principales que devuelven TODOS los espacios encontrados
        public static List<InventorySpaceProjection> GetCoverEye(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            // Buscar en todos los tipos disponibles
            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsOjoTapaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsOjoTapaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsOjoTapaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetCoverFooter(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsPieTapaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsPieTapaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsPieTapaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetInsideCover(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsRetTapaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsRetTapaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsRetTapaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetPage(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsPaginaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsPaginaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsPaginaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetInsideBackCover(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsRetContratapaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsRetContratapaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsRetContratapaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetBackCover(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsContratapaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsContratapaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsContratapaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetHalfPage(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsMediaPaginaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsMediaPaginaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsMediaPaginaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetCuarterPage(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsCuartoPaginaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsCuartoPaginaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsCuartoPaginaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetFooterPage(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();
            var results = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var foundSpaces = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => IsPiePaginaPrint(x.ProductAdvertisingSpaceName)),
                    "digital" => spaces.Where(x => IsPiePaginaDigital(x.ProductAdvertisingSpaceName)),
                    _ => spaces.Where(x => IsPiePaginaGeneric(x.ProductAdvertisingSpaceName))
                };

                results.AddRange(foundSpaces);
            }

            return results;
        }

        public static List<InventorySpaceProjection> GetOtherSpaces(IEnumerable<InventorySpaceProjection> inventorySpaces, List<string> spaceTypes)
        {
            var spaces = inventorySpaces.ToList();

            // Primero filtrar por tipos disponibles
            var availableSpaces = new List<InventorySpaceProjection>();

            foreach (var spaceType in spaceTypes)
            {
                var spacesOfType = spaceType.ToLower() switch
                {
                    "print" => spaces.Where(x => x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("print")),
                    "digital" => spaces.Where(x => x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("digital")),
                    _ => spaces.Where(x => !x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("print") &&
                                          !x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("digital"))
                };

                availableSpaces.AddRange(spacesOfType);
            }

            // Excluir todas las categorías ya clasificadas
            return availableSpaces.Where(x => !IsClassifiedSpace(x.ProductAdvertisingSpaceName)).ToList();
        }

        // =========== MÉTODOS PRIVADOS PARA VERIFICAR CADA CATEGORÍA ===========

        // GetCoverEye - Ojos de tapa (Print/Digital/Generic)
        private static bool IsOjoTapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ojos de tapa print" || cleanName == "ojos tapa print";
        }

        private static bool IsOjoTapaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ojos de tapa digital" || cleanName == "ojos tapa digital";
        }

        private static bool IsOjoTapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ojos de tapa" ||
                   cleanName == "ojos tapa" ||
                   cleanName == "cover eyes" ||
                   cleanName == "cover eyes/ojos de tapa" ||
                   cleanName == "ojos de tapa/cover eyes";
        }

        // GetCoverFooter - Pie de tapa (Print/Digital/Generic)
        private static bool IsPieTapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de tapa print";
        }

        private static bool IsPieTapaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de tapa digital";
        }

        private static bool IsPieTapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de tapa" ||
                   cleanName == "cover footer" ||
                   cleanName == "footer" ||
                   cleanName == "cover footer/pie de tapa" ||
                   cleanName == "footer / pie de tapa" ||
                   cleanName == "pie de página / pie de tapa";
        }

        // GetInsideCover - Ret. tapa (Print/Digital/Generic)
        private static bool IsRetTapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret. tapa print";
        }

        private static bool IsRetTapaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret. tapa digital" ||
                   cleanName == "ret tapa digital";
        }

        private static bool IsRetTapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret. tapa" ||
                   cleanName == "ret tapa" ||
                   cleanName == "retiración de tapa" ||
                   cleanName == "inside cover";
        }

        // GetPage - Páginas completas (Print/Digital/Generic)
        private static bool IsPaginaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pagina print";
        }

        private static bool IsPaginaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pagina digital";
        }

        private static bool IsPaginaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "full page" ||
                   cleanName == "full page/página" ||
                   cleanName == "pagina";
        }

        // GetInsideBackCover - Ret contratapa (Print/Digital/Generic)
        private static bool IsRetContratapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret contratapa print" || cleanName == "ret. contratapa print";
        }

        private static bool IsRetContratapaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret contratapa digital" ||
                   cleanName == "ret. contratapa digital";
        }

        private static bool IsRetContratapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret contratapa" ||
                   cleanName == "ret. contratapa" ||
                   cleanName == "retiración de contratapa" ||
                   cleanName == "inside back cover";
        }

        // GetBackCover - Contratapa (Print/Digital/Generic)
        private static bool IsContratapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "contratapa print" ||
                   cleanName == "back cover print";
        }

        private static bool IsContratapaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "contratapa digital" ||
                   cleanName == "back cover digital";
        }

        private static bool IsContratapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "contratapa" ||
                   cleanName == "back cover" ||
                   cleanName == "tapa 4" ||
                   cleanName == "contraportada" ||
                   cleanName == "back cover / tapa 4 / contratapa" ||
                   cleanName == "back cover/tapa 4/contratapa" ||
                   cleanName == "contraportada / tapa 4 / contratapa";
        }

        // GetHalfPage - Media página (Print/Digital/Generic)
        private static bool IsMediaPaginaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "media americana print" || cleanName == "media horizontal print";
        }

        private static bool IsMediaPaginaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "media americana digital" || cleanName == "media horizontal digital";
        }

        private static bool IsMediaPaginaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "1/2 horizontal" ||
                   cleanName == "half page/media horizontal" ||
                   cleanName == "media pagina";
        }

        // GetCuarterPage - Cuarto de página (Print/Digital/Generic)
        private static bool IsCuartoPaginaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "cuarto de pagina print" || cleanName == "cuarto pagina print";
        }

        private static bool IsCuartoPaginaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "cuarto de pagina digital" || cleanName == "cuarto pagina digital";
        }

        private static bool IsCuartoPaginaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "1/4 horizontal page" ||
                   cleanName == "1/4 page" ||
                   cleanName == "1/4 de pagina horizontal" ||
                   cleanName == "1/4 horizontal page" ||
                   cleanName == "1/4 página horizontal" ||
                   cleanName == "cuarto de pagina" ||
                   cleanName == "cuarto pagina";
        }

        // GetFooterPage - Pie de página (Print/Digital/Generic)
        private static bool IsPiePaginaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de pagina print";
        }

        private static bool IsPiePaginaDigital(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de pagina digital";
        }

        private static bool IsPiePaginaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "pie de pagina" ||
                   cleanName == "footer page" ||
                   cleanName == "page footer";
        }

        // Método helper para verificar si un espacio ya está clasificado
        private static bool IsClassifiedSpace(string name)
        {
            return IsOjoTapaPrint(name) || IsOjoTapaDigital(name) || IsOjoTapaGeneric(name) ||
                   IsPieTapaPrint(name) || IsPieTapaDigital(name) || IsPieTapaGeneric(name) ||
                   IsRetTapaPrint(name) || IsRetTapaDigital(name) || IsRetTapaGeneric(name) ||
                   IsPaginaPrint(name) || IsPaginaDigital(name) || IsPaginaGeneric(name) ||
                   IsRetContratapaPrint(name) || IsRetContratapaDigital(name) || IsRetContratapaGeneric(name) ||
                   IsContratapaPrint(name) || IsContratapaDigital(name) || IsContratapaGeneric(name) ||
                   IsMediaPaginaPrint(name) || IsMediaPaginaDigital(name) || IsMediaPaginaGeneric(name) ||
                   IsCuartoPaginaPrint(name) || IsCuartoPaginaDigital(name) || IsCuartoPaginaGeneric(name) ||
                   IsPiePaginaPrint(name) || IsPiePaginaDigital(name) || IsPiePaginaGeneric(name);
        }
    }
}