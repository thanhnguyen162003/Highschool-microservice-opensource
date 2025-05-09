//using Application.Common.Models.Common;
//using Application.Common.Models.OccupationModel;
//using Infrastructure.Data;
//using MongoDB.Driver;

//namespace Application.Features.Occupation.Queries
//{
//    public record GetOccupationById : IRequest<ResponseModel>
//    {
//        public string OccupationId { get; set; }
//    }

//    public class GetOccupationByIdHandler(CareerMongoDatabaseContext context, IMapper mapper) : IRequestHandler<GetOccupationById, ResponseModel>
//    {
//        public async Task<ResponseModel> Handle(GetOccupationById request, CancellationToken cancellationToken)
//        {
//            var Occupation = await context.Occupations.Find(uni => uni.Id == request.OccupationId).FirstOrDefaultAsync();
//            if (Occupation != null)
//            {              
//                return new ResponseModel()
//                {
//                    Status = System.Net.HttpStatusCode.OK,
//                    Message = "Get success",
//                    Data = mapper.Map<OccupationResponseModel>(Occupation)
//                };
//            }
//            else
//            {
//                return new ResponseModel()
//                {
//                    Status = System.Net.HttpStatusCode.BadRequest,
//                    Message = "Occupation does not exist",
//                    Data = null
//                };
//            }
//        }
//    }
//}
