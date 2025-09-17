using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LadeviVentasApi.Projections;

namespace LadeviVentasApi.Helpers.Utilities
{
    public class AdvertisingSpaceClassifier
    {
        // Métodos principales que devuelven un valor único o null
        public static InventorySpaceProjection GetCoverEye(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            return spaceType.ToLower() switch
            {
                "print" => GetOjoTapaPrintObjects(inventorySpaces).FirstOrDefault(),
                "digital" => GetOjoTapaDigitalObjects(inventorySpaces).FirstOrDefault(),
                _ => GetOjoTapaGenericObjects(inventorySpaces).FirstOrDefault()
            };
        }

        public static InventorySpaceProjection GetCoverFooter(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            return spaceType.ToLower() switch
            {
                "print" => GetPieTapaPrintObjects(inventorySpaces).FirstOrDefault(),
                "digital" => GetPieTapaDigitalObjects(inventorySpaces).FirstOrDefault(),
                _ => GetPieTapaGenericObjects(inventorySpaces).FirstOrDefault()
            };
        }

        public static InventorySpaceProjection GetPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            return spaceType.ToLower() switch
            {
                "print" => GetPaginaPrintObjects(inventorySpaces).FirstOrDefault(),
                "digital" => GetPaginaDigitalObjects(inventorySpaces).FirstOrDefault(),
                _ => GetPaginaObjects(inventorySpaces).FirstOrDefault()
            };
        }

        public static InventorySpaceProjection GetBackCover(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            return spaceType.ToLower() switch
            {
                "print" => GetContratapaPrintObjects(inventorySpaces).FirstOrDefault(),
                "digital" => GetContratapaDigitalObjects(inventorySpaces).FirstOrDefault(),
                _ => GetContratapaObjects(inventorySpaces).FirstOrDefault()
            };
        }

        public static InventorySpaceProjection GetHalfPage(IEnumerable<InventorySpaceProjection> inventorySpaces, string spaceType)
        {
            return spaceType.ToLower() switch
            {
                "print" => GetMediaPaginaPrintObjects(inventorySpaces).FirstOrDefault(),
                _ => null
            };
        }

        // Métodos específicos internos (ahora privados)
        private static IEnumerable<InventorySpaceProjection> GetOjoTapaObjects(IEnumerable<InventorySpaceProjection> inventorySpaces, string productAdvertisingSpaceType)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojos\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, $@"\b{productAdvertisingSpaceType}\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetOjoTapaPrintObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojos\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetOjoTapaDigitalObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojos\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetOjoTapaGenericObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojos\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPieTapaObjects(IEnumerable<InventorySpaceProjection> inventorySpaces, string productAdvertisingSpaceType)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, $@"\b{productAdvertisingSpaceType}\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPieTapaPrintObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPieTapaDigitalObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPieTapaGenericObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPaginaObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"^página?$", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"^pagina$", RegexOptions.IgnoreCase)) ||
                    ((Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpágina\b", RegexOptions.IgnoreCase) ||
                      Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase)) &&
                     !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital|media|completa|entera|de)\b", RegexOptions.IgnoreCase)));
        }

        private static IEnumerable<InventorySpaceProjection> GetPaginaPrintObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpágina\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetPaginaDigitalObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpágina\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetContratapaObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetContratapaPrintObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetContratapaDigitalObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase));
        }

        private static IEnumerable<InventorySpaceProjection> GetMediaPaginaPrintObjects(IEnumerable<InventorySpaceProjection> inventorySpaces)
        {
            return inventorySpaces
                .Where(ipas =>
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase) &&
                    (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpágina\b", RegexOptions.IgnoreCase) ||
                     Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase)) &&
                    Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase));
        }
    }
}