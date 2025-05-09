using Application.Common.Interfaces.CacheInterface;
using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Domain.Common.Messages;
using Domain.Common.UUID;
using Domain.MongoEntities;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.University.Commands
{
    public record UniversityUpdateCommand : IRequest<ResponseModel>
    {
        public Guid Id { get; init; }
        public UniversityRequestModel Request { get; init; }
    }

    public class UniversityUpdateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper,
            ICleanCacheService cleanCacheService) : IRequestHandler<UniversityUpdateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ICleanCacheService _cleanCacheService = cleanCacheService;

        public async Task<ResponseModel> Handle(UniversityUpdateCommand request, CancellationToken cancellationToken)
        {
            // Find the university to update
            var university = await _context.Universities.Find(x => x.id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (university == null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy trường đại học", null);
            }

                var cleanTags = request.Request.Tags?.Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new List<string>();
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
            
            var filter = Builders<Domain.MongoEntities.UniversityMajor>.Filter.Eq(x => x.UniCode, university.unicode);
            var update = Builders<Domain.MongoEntities.UniversityMajor>.Update.Set(x => x.UniCode, request.Request.UniCode);

            await _context.UniversityMajors.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
			await _cleanCacheService.ClearRelatedCacheUniversity();
			// Update and save
			_mapper.Map(request.Request, university);
            await _context.Universities.ReplaceOneAsync(
                x => x.id == request.Id,
                university,
                cancellationToken: cancellationToken
            );

            return new ResponseModel(HttpStatusCode.OK, MessageCommon.UpdateSuccesfully, university);
        }
    }

}
