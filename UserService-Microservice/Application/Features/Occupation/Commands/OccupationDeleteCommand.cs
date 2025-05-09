using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.Occupation.Commands
{
    public record OccupationDeleteCommand : IRequest<bool>
    {
        public required string Id { get; set; }
    }

    public class OccupationDeleteCommandHandler(CareerMongoDatabaseContext context) : IRequestHandler<OccupationDeleteCommand, bool>
    {
        private readonly CareerMongoDatabaseContext _context = context;

        public async Task<bool> Handle(OccupationDeleteCommand request, CancellationToken cancellationToken)
        {
            var occupation = await _context.Occupations.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (occupation == null)
            {
                return false;
            }

            var result = await _context.Occupations.DeleteOneAsync(x => x.Id == request.Id, cancellationToken);
            return result.DeletedCount > 0;
        }
    }

}
