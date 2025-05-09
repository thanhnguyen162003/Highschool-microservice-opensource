namespace Infrastructure.Repositories.Interfaces
{
	public interface IUserSubjectRepository : IRepository<UserSubjectRepository>
	{
		Task Add(Guid userId, IEnumerable<string> subjectIds);
		Task Delete(Guid userId);

    }
}
