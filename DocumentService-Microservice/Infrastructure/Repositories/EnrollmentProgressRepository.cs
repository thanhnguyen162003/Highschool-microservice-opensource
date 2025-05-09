using Infrastructure.Repositories.Interfaces;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories
{
    public class EnrollmentProgressRepository(DocumentDbContext context) : BaseRepository<EnrollmentProgress>(context), IEnrollmentProgressRepository
    {
    }
}
