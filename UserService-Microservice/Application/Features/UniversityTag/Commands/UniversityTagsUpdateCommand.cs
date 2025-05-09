using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Domain.Common.Messages;
using Domain.MongoEntities;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.UniversityTags.Commands
{
    public record UniversityTagsUpdateCommand : IRequest<ResponseModel>
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
    }

    public class UniversityTagsUpdateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<UniversityTagsUpdateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(UniversityTagsUpdateCommand request, CancellationToken cancellationToken)
        {
            // Find the university to update
            var tag = await _context.UniversityTags.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (tag == null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy tag", null);
            }
            var universitiesWithTag = await _context.Universities.Find(x => x.tags.Contains(tag.Name)).ToListAsync(cancellationToken);

            if (universitiesWithTag == null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy trường đại học", null);
            }

            foreach (var university in universitiesWithTag)
            {
                int index = university.tags.IndexOf(tag.Name);
                if (index != -1)
                {
                    university.tags[index] = request.Name.ToLower();
                }

                // Optionally update multiple tags if duplicates are possible
                // university.Tags = university.Tags.Select(t => t == oldTagName ? newTagName : t).ToList();

                await _context.Universities.ReplaceOneAsync(
                    x => x.id == university.id,
                    university,
                    cancellationToken: cancellationToken
                );
            }

            // Step 4: Update the tag name itself
            tag.Name = request.Name.ToLower();
            await _context.UniversityTags.ReplaceOneAsync(
                x => x.Id == tag.Id,
                tag,
                cancellationToken: cancellationToken
            );

            return new ResponseModel(HttpStatusCode.OK, MessageCommon.UpdateSuccesfully, tag);
        }
    }

}
