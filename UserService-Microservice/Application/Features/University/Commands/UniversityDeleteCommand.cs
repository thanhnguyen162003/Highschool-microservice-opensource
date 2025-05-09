using Application.Common.Interfaces.CacheInterface;
using Application.Common.Models.Common;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.University.Commands
{
    public record UniversityDeleteCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class UniversityDeleteCommandHandler(CareerMongoDatabaseContext context, ICleanCacheService cleanCacheService) : IRequestHandler<UniversityDeleteCommand, bool>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly ICleanCacheService _cleanCacheService = cleanCacheService;

        public async Task<bool> Handle(UniversityDeleteCommand request, CancellationToken cancellationToken)
        {
            var university = await _context.Universities.Find(x => x.id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (university == null)
            {
                return false;
            }

            await _context.Universities.DeleteOneAsync(x => x.id == request.Id, cancellationToken);

            await _context.UniversityMajors.DeleteManyAsync(x => x.UniCode == university.unicode, cancellationToken);

            await _context.SavedUniversities.DeleteManyAsync(x => x.UniversityId == university.id, cancellationToken);

			await _cleanCacheService.ClearRelatedCacheUniversity();

			return true;
        }
    }

}
