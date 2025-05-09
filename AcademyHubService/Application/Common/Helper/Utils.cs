namespace Application.Common.Helper
{
    public static class Utils
    {
        /// <summary>
        /// Generates a random string with a length of 6 characters.
        /// </summary>
        /// <param name="handler">An method handler want to get name</param>
        /// <param name="arguments">Get all name parameter in that method</param>
        /// <returns>Name of method include name method and name of parameter</returns>
        public static string GenerateName<T>(object handler, object[] arguments)
        {
            // Get the name of the handler class
            string handlerName = handler.GetType().Name;

            // Extract property values from the request object (assuming it's the first argument)
            if (arguments.Length > 0 && arguments[0] is T query)
            {
                // Use reflection to get property values of the query object
                var queryProperties = typeof(T)
                    .GetProperties()
                    .Select(prop => $"{prop.GetValue(query) ?? "null"}");

                // Join property values with underscores
                return $"{handlerName}_{string.Join("_", queryProperties)}";
            }

            // Fallback if arguments are not in expected format
            return handlerName;
        }

        public static bool IsValidUrl(this string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }
            return false;
        }
    }
}
