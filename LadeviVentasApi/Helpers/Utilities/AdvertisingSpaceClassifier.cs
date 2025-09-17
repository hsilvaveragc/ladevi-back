using LadeviVentasApi.Projections;

namespace LadeviVentasApi.Helpers.Utilities
{
    public class AdvertisingSpaceClassifier
    {
        // Métodos principales que devuelven un valor único o null
        public static InventorySpaceProjection GetCoverEye(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsOjoTapaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsOjoTapaDigital(x.ProductAdvertisingSpaceName)),
                _ => null
            };
        }

        public static InventorySpaceProjection GetCoverFooter(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsPieTapaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsPieTapaDigital(x.ProductAdvertisingSpaceName)),
                _ => null
            };
        }

        public static InventorySpaceProjection GetInsideCover(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsRetTapaPrint(x.ProductAdvertisingSpaceName)),
                _ => null
            };
        }

        public static InventorySpaceProjection GetPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsPaginaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsPaginaDigital(x.ProductAdvertisingSpaceName)),
                _ => spaces.FirstOrDefault(x => IsPaginaGeneric(x.ProductAdvertisingSpaceName))
            };
        }

        public static InventorySpaceProjection GetInsideBackCover(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsRetContratapaPrint(x.ProductAdvertisingSpaceName)),
                _ => null
            };
        }

        public static InventorySpaceProjection GetBackCover(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                _ => spaces.FirstOrDefault(x => IsContratapaGeneric(x.ProductAdvertisingSpaceName))
            };
        }

        public static InventorySpaceProjection GetHalfPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsMediaPaginaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsMediaPaginaDigital(x.ProductAdvertisingSpaceName)),
                _ => spaces.FirstOrDefault(x => IsMediaPaginaGeneric(x.ProductAdvertisingSpaceName))
            };
        }

        public static InventorySpaceProjection GetCuarterPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsCuartoPaginaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsCuartoPaginaDigital(x.ProductAdvertisingSpaceName)),
                _ => spaces.FirstOrDefault(x => IsCuartoPaginaGeneric(x.ProductAdvertisingSpaceName))
            };
        }

        public static InventorySpaceProjection GetFooterPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            return spaceType.ToLower() switch
            {
                "print" => spaces.FirstOrDefault(x => IsPiePaginaPrint(x.ProductAdvertisingSpaceName)),
                "digital" => spaces.FirstOrDefault(x => IsPiePaginaDigital(x.ProductAdvertisingSpaceName)),
                _ => null
            };
        }

        public static IEnumerable<InventorySpaceProjection> GetOtherSpaces(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            var spaces = inventorySpaces.ToList();

            // Filtrar por tipo específico primero
            var spacesOfType = spaceType.ToLower() switch
            {
                "print" => spaces.Where(x => x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("print")),
                "digital" => spaces.Where(x => x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("digital")),
                _ => spaces.Where(x => !x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("print") &&
                                      !x.ProductAdvertisingSpaceName.Trim().ToLower().EndsWith("digital"))
            };

            // Excluir todas las categorías ya clasificadas
            return spacesOfType.Where(x => !IsClassifiedSpace(x.ProductAdvertisingSpaceName));
        }

        // Métodos privados para verificar cada categoría
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

        private static bool IsRetTapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret. tapa print";
        }

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

        private static bool IsRetContratapaPrint(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "ret contratapa print" || cleanName == "ret. contratapa print";
        }

        private static bool IsContratapaGeneric(string name)
        {
            var cleanName = name.Trim().ToLower();
            return cleanName == "contratapa";
        }

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
            return cleanName == "1/4 horizontal page";
        }

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

        private static bool IsClassifiedSpace(string name)
        {
            return IsOjoTapaPrint(name) || IsOjoTapaDigital(name) ||
                   IsPieTapaPrint(name) || IsPieTapaDigital(name) ||
                   IsRetTapaPrint(name) ||
                   IsPaginaPrint(name) || IsPaginaDigital(name) || IsPaginaGeneric(name) ||
                   IsRetContratapaPrint(name) ||
                   IsContratapaGeneric(name) ||
                   IsMediaPaginaPrint(name) || IsMediaPaginaDigital(name) || IsMediaPaginaGeneric(name) ||
                   IsCuartoPaginaPrint(name) || IsCuartoPaginaDigital(name) || IsCuartoPaginaGeneric(name) ||
                   IsPiePaginaPrint(name) || IsPiePaginaDigital(name);
        }
    }
}