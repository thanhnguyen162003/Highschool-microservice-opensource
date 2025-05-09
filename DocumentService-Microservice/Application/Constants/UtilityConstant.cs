namespace Application.Constants
{
    public static class UtilityConstant
    {
        /// <summary>
        /// KEEP IT HERE, DO NOT DELETE
        /// </summary>
        public static class DocumentInformation
        {
            public static readonly string[] AllowedDocumentExtension =
            {
            //Doc
                ".txt", ".rtf", ".doc", ".docx", ".odt", ".pdf", ".tex", ".wpd", "wps",
            //Spreadsheet
                ".xls", ".xlsx", ".ods", ".osv",
            //Presentation
                ".ppt", ".pptx", ".odp", ".pps", ".ppsx",
            //Other
                ".epub", ".mobi", ".xps", ".pages", ".md", ".log", ".msg"
            };

            public static readonly int MaxFileSizeInBytes = 8 * 1024 * 1024;
        }

    }
}
