using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace PinPayments
{
    internal static class ParameterBuilder
    {
        public static string ApplyAllParameters(object obj, string url)
        {
            if (obj == null) return url;

            var newUrl = url;

            foreach (var property in obj.GetType().GetTypeInfo().DeclaredProperties)
            {
                foreach (var attribute in property.GetCustomAttributes(false))
                {
                    if (attribute.GetType() != typeof(JsonPropertyAttribute)) continue;

                    var JsonPropertyAttribute = (JsonPropertyAttribute)attribute;

                    var value = property.GetValue(obj, null);

                    if (value != null)
                    {
                        newUrl = ApplyParameterToUrl(newUrl, JsonPropertyAttribute.PropertyName, value.ToString());
                    }
                }
            }

            if (url == "") // othewise it adds a "?" at the start...
            {
                newUrl = newUrl.Substring(1, newUrl.Length - 1);
            }
            return newUrl;
        }

        public static string ApplyParameterToUrl(string url, string argument, string value)
        {
            var token = "&";

            if (!url.Contains("?"))
            {
                token = "?";
            }

            return $"{url}{token}{argument}={System.Net.WebUtility.UrlEncode(value)}";
        }
    }
}
