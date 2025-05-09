using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
	public class ImageReportRepository(UserDatabaseContext context) : BaseRepository<ImageReport>(context), IImageReportRepository
	{
		private readonly UserDatabaseContext _context = context;
    }
}
