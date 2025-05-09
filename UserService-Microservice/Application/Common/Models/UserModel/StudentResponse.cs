namespace Application.Common.Models.UserModel
{
	public class StudentResponse : BaseUserResponse
	{
		public int Grade { get; set; }

		public string? SchoolName { get; set; } = string.Empty;

        public string? CardUrl { get; set; }
    }
}
