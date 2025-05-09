using Application.Common.Models.Common;
using Application.Common.Models.UniversityMajor;
using Domain.Common.Messages;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Net;

namespace Application.Features.UniversityMajor.Commands
{
    public record UniversityMajorUpdateCommand : IRequest<ResponseModel>
    {
        public string Id { get; init; }
        public UniversityMajorRequestModel Request { get; init; }
    }

    public class UniversityMajorUpdateCommandHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<UniversityMajorUpdateCommand, ResponseModel>
    {
        private readonly CareerMongoDatabaseContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseModel> Handle(UniversityMajorUpdateCommand request, CancellationToken cancellationToken)
        {
            // Check if UniCode exists
            var universityExist = await _context.Universities
                .Find(x => x.unicode == request.Request.UniCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (universityExist == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, $"Mã trường đại học {request.Request.UniCode} không tồn tại", null);
            }

            // Check if MajorCode exists
            var majorExist = await _context.Majors
                .Find(x => x.MajorCode == request.Request.MajorCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (majorExist == null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, $"Mã chuyên ngành {request.Request.MajorCode} không tồn tại", null);
            }

            // Find the UniversityMajor to update
            var universityMajor = await _context.UniversityMajors.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (universityMajor == null)
            {
                return new ResponseModel(HttpStatusCode.NotFound, "Không tìm thấy chuyên ngành đại học", null);
            }

            var uniMajorExist = await _context.UniversityMajors.Find(x => x.UniCode == request.Request.UniCode
                                                                    && x.MajorCode == request.Request.MajorCode
                                                                    && x.DegreeLevel == request.Request.DegreeLevel
                                                                    && x.Id != request.Id).FirstOrDefaultAsync(cancellationToken);
            if (uniMajorExist != null)
            {
                return new ResponseModel(HttpStatusCode.BadRequest, $"Trường đại học {request.Request.UniCode} đã có ngành này với phương thức xét tuyển này");
            }

            // Update and save
            _mapper.Map(request.Request, universityMajor);
            await _context.UniversityMajors.ReplaceOneAsync(
                x => x.Id == request.Id,
                universityMajor,
                cancellationToken: cancellationToken
            );

            return new ResponseModel(HttpStatusCode.OK, MessageCommon.UpdateSuccesfully, universityMajor);
        }
    }

}
