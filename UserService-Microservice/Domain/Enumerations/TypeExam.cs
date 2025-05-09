using System.ComponentModel;

namespace Domain.Enumerations
{
	public enum TypeExam
	{
		[Description("Kiểm tra 1 tiết")]
		T1H = 1,
		[Description("Kiểm tra cuối kỳ")]
		FIE = 2,
		[Description("THPT Quốc Gia (national high school exam)")]
		NHE = 3,
		[Description("Đánh giá năng lực (capacity assessment)")]
		CAP = 4,
		OTHER = 5
	}
}
