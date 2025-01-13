namespace Domain.Models.UserModels
{
    public class AuthorResponseModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
