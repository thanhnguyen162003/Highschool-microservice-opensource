using Domain.Enumerations;

namespace Domain.Entities;

public class Student
{
	public Guid Id { get; set; }
	public Guid BaseUserId { get; set; }
	public int? Grade { get; set; }
	public string? SchoolName { get; set; }
	//ai làm lưu major vào đây
	public string? Major { get; set; }
	public string? TypeExam { get; set; }
    public MBTIType? MbtiType { get; set; }
	public string? HollandType { get; set; } //"RIA", "ECI"
	public string? CardUrl { get; set; }
    public virtual BaseUser BaseUser { get; set; } = null!;

	public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}