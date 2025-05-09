namespace Infrastructure.Repositories.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
	Task<bool> AddUser(Student student);
	Task<Student?> GetStudentByUserId(Guid userId);
	Task<int> GetTotalStudentAmount();
	Task<bool> UpdateStudent(Student student);
}