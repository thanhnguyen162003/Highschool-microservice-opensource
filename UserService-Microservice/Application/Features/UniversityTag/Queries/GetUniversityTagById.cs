using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.UniversityTag.Queries
{
    public record GetUniversityTagById : IRequest<ResponseModel>
    {
        public Guid Id { get; set; }
    }

    public class GetUniversityTagByIdHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<GetUniversityTagById, ResponseModel>
    {
        public async Task<ResponseModel> Handle(GetUniversityTagById request, CancellationToken cancellationToken)
        {
            var university = await context.UniversityTags.Find(uni => uni.Id == request.Id).FirstOrDefaultAsync();
            var result = mapper.Map<UniversityTagResponseModel>(university);
            if (university != null)
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Get success",
                    Data = result
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
