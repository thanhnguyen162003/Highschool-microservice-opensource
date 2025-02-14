namespace Domain.Models.Common
{
    public class DefaultSystem
    {
        /// <summary>
        /// Short Name of system
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Full Name of system
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Size File is 1GB
        /// </summary>
        public int LimitSizeFile { get; set; } = 1073741824;

        /// <summary>
        /// Time to cache is 60 minutes
        /// </summary>
        public int CacheTime { get; set; } = 60;
    }
}
