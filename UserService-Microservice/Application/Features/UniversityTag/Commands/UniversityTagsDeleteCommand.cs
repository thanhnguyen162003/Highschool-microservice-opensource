using Application.Common.Models.Common;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.UniversityTags.Commands
{
    public record UniversityTagsDeleteCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class UniversityTagsDeleteCommandHandler(CareerMongoDatabaseContext context) : IRequestHandler<UniversityTagsDeleteCommand, bool>
    {
        private readonly CareerMongoDatabaseContext _context = context;

        public async Task<bool> Handle(UniversityTagsDeleteCommand request, CancellationToken cancellationToken)
        {
            var tag = await _context.UniversityTags.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return false;
            }

            var university = await _context.Universities.Find(x => x.tags.Contains( tag.Name)).ToListAsync(cancellationToken);

            var universityIds = university.Select(x => x.id).ToList();

            await _context.Universities
                .DeleteManyAsync(x => university.Select(x => x.id).ToList().Contains(x.id), cancellationToken);

            await _context.UniversityMajors.DeleteManyAsync(x => university.Select(x => x.unicode).ToList().Contains(x.UniCode), cancellationToken);

            await _context.SavedUniversities.DeleteManyAsync(x => university.Select(x => x.id).ToList().Contains(x.UniversityId), cancellationToken);
            await _context.UniversityTags.DeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return true;
        }
    }

}
