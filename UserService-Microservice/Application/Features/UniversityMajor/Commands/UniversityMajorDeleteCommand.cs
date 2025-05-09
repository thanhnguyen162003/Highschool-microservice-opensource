using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.UniversityMajor.Commands
{
    public record UniversityMajorDeleteCommand : IRequest<bool>
    {
        public required string Id { get; set; }
    }

    public class UniversityMajorDeleteCommandHandler(CareerMongoDatabaseContext context) : IRequestHandler<UniversityMajorDeleteCommand, bool>
    {
        private readonly CareerMongoDatabaseContext _context = context;

        public async Task<bool> Handle(UniversityMajorDeleteCommand request, CancellationToken cancellationToken)
        {
            var universityMajor = await _context.UniversityMajors.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (universityMajor == null)
            {
                return false;
            }

            var result = await _context.UniversityMajors.DeleteOneAsync(x => x.Id == request.Id, cancellationToken);
            return result.DeletedCount > 0;
        }
    }

}
