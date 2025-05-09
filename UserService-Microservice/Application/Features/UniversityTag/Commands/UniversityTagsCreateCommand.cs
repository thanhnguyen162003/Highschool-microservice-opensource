using Application.Common.Models.Common;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.UniversityTag.Commands
{
    public record UniversityTagsCreateCommand : IRequest<ResponseModel>
    {
        public List<string> NameTag { get; init; }
    }

    public class UniversityTagsCreateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<UniversityTagsCreateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(UniversityTagsCreateCommand request, CancellationToken cancellationToken)
        {
            var originalNames = request.NameTag.Distinct().ToList();

            var nameLookup = originalNames
                .ToDictionary(x => x.ToLower(), x => x); 

            var lowerCaseNames = nameLookup.Keys.ToList();

            var check = await _context.UniversityTags
                .Find(x => lowerCaseNames.Contains(x.Name.ToLower()))
                .ToListAsync(cancellationToken: cancellationToken);

            var existingNames = check
                .Select(x => x.Name.ToLower())
                .ToHashSet();

            var newNames = lowerCaseNames
                .Where(x => !existingNames.Contains(x))
                .Select(x => nameLookup[x])
                .ToList();

            if (!newNames.Any())
            {
                return new ResponseModel(HttpStatusCode.BadRequest, "All tags already exist", check.Select(x => x.Name));
            }

            var universities = newNames.Select(x => new Domain.MongoEntities.UniversityTags
            {
                Id = new UuidV7().Value,
                Name = x,
                CreatedAt = DateTime.UtcNow,
            }).ToList();

            await _context.UniversityTags.InsertManyAsync(universities, cancellationToken: cancellationToken);
            return new ResponseModel(HttpStatusCode.Created, MessageCommon.CreateSuccesfully, universities.Select(x => x.Name));
        }
    }

}
