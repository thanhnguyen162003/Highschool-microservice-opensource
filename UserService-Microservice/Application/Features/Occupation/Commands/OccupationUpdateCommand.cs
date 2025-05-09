using Application.Common.Models.Common;
using Application.Common.Models.OccupationModel;
using Domain.Common.Messages;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.Occupation.Commands
{
    public record OccupationUpdateCommand : IRequest<ResponseModel>
    {
        public string Id { get; init; }
        public OccupationRequestModel Request { get; init; }
    }

    public class OccupationUpdateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<OccupationUpdateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(OccupationUpdateCommand request, CancellationToken cancellationToken)
        {
            // Check if MajorCodes exist
            foreach (var majorCode in request.Request.MajorCodes)
            {
                var majorExists = await _context.Majors
                    .Find(x => x.MajorCode == majorCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (majorExists == null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, $"Không tìm thấy mã chuyên ngành {majorCode}", null);
                }
            }

            // Find the Occupation to update
            var occupation = await _context.Occupations.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (occupation == null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy nghề nghiệp", null);
            }

            // Update and save
            _mapper.Map(request.Request, occupation);
            await _context.Occupations.ReplaceOneAsync(
                x => x.Id == request.Id,
                occupation,
                cancellationToken: cancellationToken
            );

            return new ResponseModel(HttpStatusCode.OK, MessageCommon.UpdateSuccesfully, occupation);
        }
    }

}
