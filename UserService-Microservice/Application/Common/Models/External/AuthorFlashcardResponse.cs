namespace Application.Common.Models.External
{
	public class AuthorFlashcardResponse
	{
		public Guid Id { get; set; }

		public string? Username { get; set; }

		public string? Fullname { get; set; }

		public string? ProfilePicture { get; set; }

		public bool? IsStudent { get; set; }
	}
}
