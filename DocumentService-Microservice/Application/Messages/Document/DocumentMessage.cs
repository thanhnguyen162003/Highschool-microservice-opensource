namespace Application.Messages.Document
{
    public static class DocumentMessage
    {
        public static readonly string SUBJECT_IS_REQUIRED = "Môn học không được bỏ trống";
        public static readonly string SUBJECT_NOT_FOUND = "Môn học này không tồn tại.";
        public static readonly string DOCUMENT_NAME_INVALID_MAX_LENGTH = "Tên của tài liệu không được vượt quá 200 ký tự";
        public static readonly string DOCUMENT_DESCRIPTION_INVALID_MAX_LENGTH = "Mô tả của tài liệu không được vượt quá 1000 ký tự";
        public static readonly string DOCUMENT_YEAR_INVALID_MAX = "Năm của tài liệu không thể là năm trong tương lai";
        public static readonly string DOCUMENT_YEAR_INVALID_MIN = "Ngày lập tài liệu đã quá xa trong quá khứ";
        //public static readonly string DOCUMENT_AUTHOR_INVALID_MAX_LENGTH = "Author name cannot have more than 255 characters.";
    }
}
