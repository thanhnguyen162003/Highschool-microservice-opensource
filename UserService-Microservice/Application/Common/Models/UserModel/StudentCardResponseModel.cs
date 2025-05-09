using Domain.Enumerations;

namespace Application.Common.Models.UserModel
{
    public class StudentCardResponseModel
    {
        public Guid Id { get; set; }

        public Guid BaseUserId { get; set; }

        public int? Grade { get; set; }

        public string? SchoolName { get; set; }

        public string? Major { get; set; }

        public string? TypeExam { get; set; }
        public MBTIType? MbtiType { get; set; }
        public string? HollandType { get; set; } //"RIA", "ECI"
        public BaseUserInforResponseModel BaseUserInfo { get; set; }
    }
}
