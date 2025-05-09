using Application.Common.Interfaces.CacheInterface;
using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.MongoEntities;
using Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.University.Commands
{
    public record UniversityCreateCommand : IRequest<ResponseModel>
    {
        public List<UniversityRequestModel> UniversityRequestModelList { get; init; }
    }

    public class UniversityCreateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper,
            ICleanCacheService cleanCacheService) : IRequestHandler<UniversityCreateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICleanCacheService _cleanCacheService = cleanCacheService;

        public async Task<ResponseModel> Handle(UniversityCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.UniversityRequestModelList == null || request.UniversityRequestModelList.Count == 0)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, MessageCommon.InvalidStatus, null);
            }

            foreach (var uniRequest in request.UniversityRequestModelList)
            {
                var cleanTags = uniRequest.Tags?.Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
                var tagEntities = new List<Domain.MongoEntities.UniversityTags>(); // Assuming you have a Tag entity/model in MongoDB

                foreach (var tag in cleanTags)
                {
                    // Check if the tag already exists in DB
                    var existingTag = await _context.UniversityTags.Find(x => x.Name.ToLower() == tag.ToLower()).FirstOrDefaultAsync(cancellationToken);

                    if (existingTag != null)
                    {
                        tagEntities.Add(existingTag);
                    }
                    else
                    {
                        // Create new tag
                        var newTag = new Domain.MongoEntities.UniversityTags
                        {
                            Id = new UuidV7().Value,
                            Name = tag,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _context.UniversityTags.InsertOneAsync(newTag, cancellationToken: cancellationToken);
                        tagEntities.Add(newTag);
                    }
                }
            }
            var universities = _mapper.Map<List<Domain.MongoEntities.University>>(request.UniversityRequestModelList);
            foreach(var item in universities)
            {
                item.id = new UuidV7().Value;
            }
            await _context.Universities.InsertManyAsync(universities, cancellationToken: cancellationToken);
			await _cleanCacheService.ClearRelatedCacheUniversity();

			return new ResponseModel(HttpStatusCode.Created, MessageCommon.CreateSuccesfully, universities);
        }
    }
}
