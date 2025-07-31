using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;

namespace LadeviVentasApi
{
    public static class Utils
    {
        public static List<string> GenerateAuditLogMessages<T>(T originalObject, T changedObject)
        {
            List<string> list = new List<string>();
            string className = string.Concat("[", originalObject.GetType().Name, "] ");

            foreach (PropertyInfo property in originalObject.GetType().GetProperties())
            {
                object originalPropertyValue = property.GetValue(originalObject, null);
                object newPropertyValue = changedObject != null ? property.GetValue(changedObject, null) : null;

                if (originalPropertyValue != null && !originalPropertyValue.Equals(newPropertyValue))
                {
                    bool isDate = originalPropertyValue is DateTime;
                    list.Add(string.Concat(property.Name,
                        "': '", (originalPropertyValue != null) ? (isDate ? ((DateTime)originalPropertyValue).ToString("dd/MM/yyyy") : originalPropertyValue.ToString()) : "[NULL]",
                        "' >> '", (newPropertyValue != null) ? (isDate ? ((DateTime)newPropertyValue).ToString("dd/MM/yyyy") : newPropertyValue.ToString()) : "[NULL]", "'"));
                }
            }

            return list;
        }

        public static DateTime GetArgentinaDateTime()
        {
            // Implementación igual que la propiedad anterior
            string timeZoneId = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
                ? "Argentina Standard Time"
                : "America/Argentina/Buenos_Aires";

            try
            {
                TimeZoneInfo argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, argentinaTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return DateTime.UtcNow.AddHours(-3);
            }
        }

        public static JsonResult ActionResultForModelStateValidation(ModelStateDictionary modelState, HttpResponse response)
        {
            var namingConvention = new CamelCaseNamingStrategy();
            var dict = modelState.ToDictionary(
                kvp => namingConvention.GetPropertyName(kvp.Key, false),
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
            );
            response.StatusCode = 400;
            response.ContentType = "application/json";
            return new JsonResult(new
            {
                Errors = dict
            });
        }
    }
}
