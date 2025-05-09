using Application.Common.Models.Common;
using Application.Common.Models.OccupationModel;
using Domain.Common.Messages;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.Occupation.Commands
{
    public record OccupationCreateCommand : IRequest<ResponseModel>
    {
        public List<OccupationRequestModel> OccupationRequestModelList { get; init; }
    }

    public class OccupationCreateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<OccupationCreateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(OccupationCreateCommand request, CancellationToken cancellationToken)
        {
            foreach (var occupationRequest in request.OccupationRequestModelList)
            {
                foreach (var majorCode in occupationRequest.MajorCodes)
                {
                    var majorExist = await _context.Majors.Find(x => x.MajorCode == majorCode).FirstOrDefaultAsync(cancellationToken);
                    if (majorExist == null)
                    {
                        return new ResponseModel(HttpStatusCode.BadRequest, $"Nghề nghiệp {occupationRequest.Name} có mã ngành không hợp lệ");
                    }
                }
            }
            var occupations = _mapper.Map<List<Domain.MongoEntities.Occupation>>(request.OccupationRequestModelList);
            await _context.Occupations.InsertManyAsync(occupations, cancellationToken: cancellationToken);
            return new ResponseModel(HttpStatusCode.Created, MessageCommon.CreateSuccesfully, occupations);
        }
    }

}
