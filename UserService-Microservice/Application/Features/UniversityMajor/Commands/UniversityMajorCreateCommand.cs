using Application.Common.Models.Common;
using Application.Common.Models.UniversityMajor;
using Domain.Common.Messages;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.UniversityMajor.Commands
{
    public record UniversityMajorCreateCommand : IRequest<ResponseModel>
    {
        public List<UniversityMajorRequestModel> UniversityMajorRequestModelList { get; init; }
    }

    public class UniversityMajorCreateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<UniversityMajorCreateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(UniversityMajorCreateCommand request, CancellationToken cancellationToken)
        {
            foreach (var universityMajorRequest in request.UniversityMajorRequestModelList) 
            {
                var majorExist = await _context.Majors.Find(x => x.MajorCode == universityMajorRequest.MajorCode).FirstOrDefaultAsync(cancellationToken);
                if (majorExist == null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, $"Trường đại học {universityMajorRequest.UniCode} có mã chuyên ngành không hợp lệ");
                }

                var uniExist = await _context.Universities.Find(x => x.unicode == universityMajorRequest.UniCode).FirstOrDefaultAsync(cancellationToken);

                if (uniExist == null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, $"Trường đại học {universityMajorRequest.UniCode} có mã trường đại học không hợp lệ");
                }

                var uniMajorExist = await _context.UniversityMajors.Find(x => x.UniCode == universityMajorRequest.UniCode
                                                                    && x.MajorCode == universityMajorRequest.MajorCode
                                                                    && x.DegreeLevel == universityMajorRequest.DegreeLevel).FirstOrDefaultAsync(cancellationToken);

                if (uniMajorExist != null)
                {
                    return new ResponseModel(HttpStatusCode.BadRequest, $"Trường đại học {universityMajorRequest.UniCode} đã có ngành này với phương thức xét tuyển này");
                }
            }
            var universityMajors = _mapper.Map<List<Domain.MongoEntities.UniversityMajor>>(request.UniversityMajorRequestModelList);
            await _context.UniversityMajors.InsertManyAsync(universityMajors, cancellationToken: cancellationToken);
            return new ResponseModel(HttpStatusCode.Created, MessageCommon.CreateSuccesfully, universityMajors);
        }
    }

}
