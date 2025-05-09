namespace Application.Common.Ultils
{
	public class AvatarExtension
	{
		public static string GetAvatarV1(string fullName)
		{
			return $"https://ui-avatars.com/api/?name={fullName}&background=random&rounded=true&color=random";
		}

		public static string GetAvatar(string fullName)
		{
			return $"https://avatar.iran.liara.run/username?username={fullName}";
		}
	}
}
