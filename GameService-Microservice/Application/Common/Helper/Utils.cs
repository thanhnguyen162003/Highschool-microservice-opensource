namespace Application.Common.Helper
{
    public class Utils
    {
        /// <summary>
        /// Generates a random number with a length of 6 digits.
        /// </summary>
        /// <returns>Generate an number with 6 characters</returns>
        public static string GenerateRandomNumber()
        {
            // Get the current timestamp as a base
            string timestamp = DateTime.Now.ToString("HHmmss");

            // Shuffle the timestamp digits to make it less predictable
            char[] digits = timestamp.ToCharArray();
            Random random = new Random();

            for (int i = digits.Length - 1; i > 0; i--)
            {
                // Swap random indices
                int j = random.Next(0, i + 1);
                (digits[i], digits[j]) = (digits[j], digits[i]);
            }

            return new string(digits);

        }

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


        /// <summary>
        /// Calculates the score based on the user's average time and custom min/max scores.
        /// </summary>
        /// <param name="timeAverage">The average time of the user.</param>
        /// <param name="minTime">The minimum time threshold for the maximum score.</param>
        /// <param name="maxTime">The maximum time threshold for the minimum score.</param>
        /// <param name="minScore">The minimum score.</param>
        /// <param name="maxScore">The maximum score.</param>
        /// <returns>Calculated score within the specified range.</returns>
        public static double CalculateScore(double timeAverage, double minTime, double maxTime, double minScore, double maxScore)
        {
            if (minTime >= maxTime)
                throw new ArgumentException("minTime must be less than maxTime.");

            if (minScore >= maxScore)
                throw new ArgumentException("minScore must be less than maxScore.");

            // Normalize time to a value between 0 and 1
            double normalizedTime = (maxTime - timeAverage) / (maxTime - minTime);

            // Clamp normalized time to the range [0, 1]
            normalizedTime = Math.Clamp(normalizedTime, 0, 1);

            // Map the normalized time to the score range [minScore, maxScore]
            double score = minScore + (normalizedTime * (maxScore - minScore));

            return score;
        }
    }
}
