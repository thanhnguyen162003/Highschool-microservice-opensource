using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class UserSubjectRepository(UserDatabaseContext context) : BaseRepository<UserSubjectRepository>(context), IUserSubjectRepository
	{
		private readonly UserDatabaseContext _context = context;

        public async Task Add(Guid userId, IEnumerable<string> subjectIds)
		{
			var userSubjects = _context.UserSubjects.Where(x => x.UserId.Equals(userId));

			_context.UserSubjects.RemoveRange(userSubjects);

			foreach (var subjectId in subjectIds)
			{
				await _context.UserSubjects.AddAsync(new UserSubject()
				{
					UserId = userId,
					SubjectId = Guid.Parse(subjectId)
				});
			}
		}

		public async Task Delete(Guid userId)
		{
            var userSubjects = _context.UserSubjects.Where(x => x.UserId.Equals(userId));

            _context.UserSubjects.RemoveRange(userSubjects);

			await Task.CompletedTask;
        }
	}
}
