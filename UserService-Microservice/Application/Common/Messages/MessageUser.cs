namespace Domain.Common.Messages
{
	public class MessageUser
	{
		public const string RoleNotSupport = "Vai trò không được hỗ trợ";
        public const string Blocked = "Người dùng bị chặn";
        public const string NotEnoughInformation = "Không đủ thông tin";
        public const string YouHaveBeenCompleteSetup = "Bạn đã hoàn thành thiết lập!";
        public const string ThankYou = "Cảm ơn bạn đã cập nhật thông tin, chúc bạn học tập vui vẻ và đạt nhiều kết quả tốt";

        //User Message response
        public const string UserNotFound = "Không tìm thấy người dùng";
		public const string LoginFailed = "Tài khoản hoặc mật khẩu không đúng";
		public const string LoginSuccess = "Đăng nhập thành công";
		public const string RegisterSuccess = "Đăng ký thành công";
		public const string RegisterFailed = "Đăng ký thất bại";
		public const string LogoutSuccess = "Đăng xuất thành công";
        public const string LogoutFailed = "Đăng xuất thất bại";

        public const string OTPSuccess = "Đã gửi OTP";
		public const string OTPExpired = "OTP đã hết hạn";
		public const string OTPNotValid = "OTP không hợp lệ";

		public const string TokenSuccess = "Đã gửi token";
        public const string TokenExpired = "Token đã hết hạn";
        public const string TokenInvalid = "Token không hợp lệ";

        public const string SendMailSuccess = "Đã gửi mail, xin hãy kiểm tra email để tiếp tục!";
		public const string SendMailFailed = "Gửi mail thất bại";

        public const string PhoneExisted = "Số điện thoại đã được đăng ký";
		public const string UserBlocked = "Người dùng đã bị đình chỉ";
		public const string UserDeleted = "Người dùng đã bị xóa";

		public const string LoginWithGmailInstead = "Vui lòng đăng nhập bằng Gmail!";

		public const string UserIsNotStudent = "Người dùng không phải học sinh";
		public const string UserIsNotTeacher = "Người dùng không phải giáo viên";
		public const string YouNeedToChoseRoleBefore = "Bạn cần chọn vai trò trước khi thực hiện hành động này";
		public const string MissingProfileStudent = "Học sinh chưa có thông tin cá nhân! Xin hãy cập nhật.";
		public const string MissingProfileTeacher = "Giáo viên chưa có thông tin cá nhân! Xin hãy cập nhật.";

        public const string ResetPasswordSuccess = "Đã thiết lập lại mật khẩu";
		public const string ResetPasswordFailed = "Thiết lập lại mật khẩu thất bại";

		//Authentication Message response

		public const string InvalidSubject = "Môn học không hợp lệ";

		public const string UserNotPermissionUpdateRole = "Bạn không thể thực hiện hành động này";

		// Refresh Token
		public const string RefreshTokenNotFound = "Không tìm thấy Refresh Token";
		public const string RefreshTokenSuccess = "Refresh Token thành công";
		public const string RefreshTokenFailed = "Refresh Token thất bại";

		// User Subject
		public const string YouNeedToChoseSubjectAndTypeExamBefore = "Bạn cần chọn môn học và loại kỳ thi trước khi thực hiện hành động này";
		public const string YouNeedToTestHollandAndMBTIBefore = "Bạn cần hoàn thành bài test Holland và MBTI trước khi thực hiện hành động này";
		public const string YouNeedToChoseSubjectAndUniversityAndWorkPlaceBefore = "Bạn cần chọn môn học, trường đại học và nơi làm việc trước khi thực hiện hành động này";
		public const string YouNeedWaitToVerifyIsTeacherBefore = "Bạn cần chờ xác nhận là giáo viên trước khi thực hiện hành";

    }
}
