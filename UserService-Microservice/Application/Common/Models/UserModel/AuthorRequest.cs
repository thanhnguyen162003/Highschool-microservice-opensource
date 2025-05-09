namespace Application.Common.Models.UserModel
{
    public class AuthorRequest
    {
        public IEnumerable<Guid> UserIds { get; set; } = new List<Guid>();
    }
}
