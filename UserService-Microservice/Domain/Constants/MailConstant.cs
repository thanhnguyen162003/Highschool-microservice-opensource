namespace Domain.Constants
{
	public class MailConstant
	{
		public class ConfirmMail
		{
			public const string Title = "HighSchool Magic Link";
			public static readonly string TemplatePathOTP = File.ReadAllText("./Resources/VerifyWithOTP.html");
			public static readonly string TemplatePathToken = File.ReadAllText("./Resources/VerifyWithToken.html");
		}

		public class InviteMember
        {
            public const string Title = "Invite Member";
            public static readonly string TemplatePath = File.ReadAllText("./Resources/InviteMember.html");
        }

		public class NotificationAssignment
		{
            public const string Title = "Assignment Notification";
            public static readonly string TemplatePath = File.ReadAllText("./Resources/NotificationAssignment.html");
        }

    }
}
