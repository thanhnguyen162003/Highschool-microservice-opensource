using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.University.Queries
{
    public record GetUniversityById : IRequest<ResponseModel>
    {
        public Guid UniversityId { get; set; }
    }

    public class GetUniversityByIdHandler(CareerMongoDatabaseContext context) : IRequestHandler<GetUniversityById, ResponseModel>
    {
        public async Task<ResponseModel> Handle(GetUniversityById request, CancellationToken cancellationToken)
        {
            var university = await context.Universities.Find(uni => uni.id == request.UniversityId).FirstOrDefaultAsync();
            if (university != null)
            {              
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Get success",
                    Data = university
                };
            }
            else
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "University does not exist",
                    Data = null
                };
            }
        }
    }
}
