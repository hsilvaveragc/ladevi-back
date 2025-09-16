using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class AdvertisingSpaceClassifier
{
    // Método principal para obtener cada tipo específico
    public static string GetOjoTapaPrint(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo[s]?\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetOjoTapaDigital(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo[s]?\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetOjoTapa(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bojo[s]?\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPieTapa(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPieTapaPrint(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPieTapaDigital(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpie\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\btapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPaginaPrint(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPaginaDigital(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetPagina(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"^página?$", RegexOptions.IgnoreCase) ||
                (Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase) &&
                 !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital|media|completa|entera|de)\b", RegexOptions.IgnoreCase)))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetMediaPaginaPrint(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bmedia\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bpagina\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetContratapa(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                !Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\b(print|digital)\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetContratapaPrint(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bprint\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    public static string GetContratapaDigital(IEnumerable<dynamic> inventorySpaces)
    {
        return inventorySpaces
            .FirstOrDefault(ipas =>
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bcontratapa\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(ipas.ProductAdvertisingSpaceName, @"\bdigital\b", RegexOptions.IgnoreCase))
            ?.ProductAdvertisingSpaceName;
    }

    // Método para clasificar todo en una sola pasada
    public static Dictionary<string, List<string>> ClassifyAllSpaces(IEnumerable<string> allSpaces)
    {
        var classified = new Dictionary<string, List<string>>
        {
            ["OjoTapaPrint"] = new List<string>(),
            ["OjoTapaDigital"] = new List<string>(),
            ["OjoTapa"] = new List<string>(),
            ["PieTapa"] = new List<string>(),
            ["PieTapaPrint"] = new List<string>(),
            ["PieTapaDigital"] = new List<string>(),
            ["PaginaPrint"] = new List<string>(),
            ["PaginaDigital"] = new List<string>(),
            ["Pagina"] = new List<string>(),
            ["MediaPaginaPrint"] = new List<string>(),
            ["Contratapa"] = new List<string>(),
            ["ContratapaPrint"] = new List<string>(),
            ["ContratapaDigital"] = new List<string>(),
            ["Others"] = new List<string>()
        };

        foreach (var space in allSpaces)
        {
            if (Regex.IsMatch(space, @"\bojo[s]?\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(space, @"\btapa\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(space, @"\bprint\b", RegexOptions.IgnoreCase))
                    classified["OjoTapaPrint"].Add(space);
                else if (Regex.IsMatch(space, @"\bdigital\b", RegexOptions.IgnoreCase))
                    classified["OjoTapaDigital"].Add(space);
                else
                    classified["OjoTapa"].Add(space);
            }
            else if (Regex.IsMatch(space, @"\bpie\b", RegexOptions.IgnoreCase) &&
                     Regex.IsMatch(space, @"\btapa\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(space, @"\bprint\b", RegexOptions.IgnoreCase))
                    classified["PieTapaPrint"].Add(space);
                else if (Regex.IsMatch(space, @"\bdigital\b", RegexOptions.IgnoreCase))
                    classified["PieTapaDigital"].Add(space);
                else
                    classified["PieTapa"].Add(space);
            }
            else if (Regex.IsMatch(space, @"\bcontratapa\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(space, @"\bprint\b", RegexOptions.IgnoreCase))
                    classified["ContratapaPrint"].Add(space);
                else if (Regex.IsMatch(space, @"\bdigital\b", RegexOptions.IgnoreCase))
                    classified["ContratapaDigital"].Add(space);
                else
                    classified["Contratapa"].Add(space);
            }
            else if (Regex.IsMatch(space, @"\bmedia\b", RegexOptions.IgnoreCase) &&
                     Regex.IsMatch(space, @"\bpagina\b", RegexOptions.IgnoreCase) &&
                     Regex.IsMatch(space, @"\bprint\b", RegexOptions.IgnoreCase))
            {
                classified["MediaPaginaPrint"].Add(space);
            }
            else if (Regex.IsMatch(space, @"\bpagina\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(space, @"\bprint\b", RegexOptions.IgnoreCase) &&
                    !Regex.IsMatch(space, @"\bmedia\b", RegexOptions.IgnoreCase))
                    classified["PaginaPrint"].Add(space);
                else if (Regex.IsMatch(space, @"\bdigital\b", RegexOptions.IgnoreCase) &&
                         !Regex.IsMatch(space, @"\bmedia\b", RegexOptions.IgnoreCase))
                    classified["PaginaDigital"].Add(space);
                else if (Regex.IsMatch(space, @"^página?$", RegexOptions.IgnoreCase) ||
                         !Regex.IsMatch(space, @"\b(print|digital|media|completa|entera|de)\b", RegexOptions.IgnoreCase))
                    classified["Pagina"].Add(space);
                else
                    classified["Others"].Add(space);
            }
            else
            {
                classified["Others"].Add(space);
            }
        }

        return classified;
    }
}

