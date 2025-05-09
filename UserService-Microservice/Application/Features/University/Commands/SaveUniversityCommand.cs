using Application.Common.Interfaces.CacheInterface;
using Application.Common.Models.Common;
using Application.Common.Models.UniversityModel;
using Domain.Common.Ultils;
using Domain.MongoEntities;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;

namespace Application.Features.University.Commands
{
    public record SaveUniversityCommand : IRequest<ResponseModel>
    {
        public Guid UniversityId { get; init; }
    }
    
    public class SaveUniversityCommandHandler(CareerMongoDatabaseContext context,
        IHttpContextAccessor httpContextAccessor, ICleanCacheService cleanCacheService) : IRequestHandler<SaveUniversityCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(SaveUniversityCommand request, CancellationToken cancellationToken)
        {
            var university = context.Universities.Find(uni => uni.id == request.UniversityId);

            if (university == null) 
            {
                return new ResponseModel()
                {
                    Status = System.Net.HttpStatusCode.BadRequest,
                    Message = "Trường đại học này không tồn tại trong hệ thống."
                };
            }

            var saveUniversityCommand = new SavedUniversity()
            {
                UniversityId = request.UniversityId,
                UserId = httpContextAccessor.HttpContext.User.GetUserIdFromToken(),
                CreatedAt = DateTime.Now
            };

            await context.SavedUniversities.InsertOneAsync(saveUniversityCommand);
			await cleanCacheService.ClearRelatedCacheUniversitySave();
			return new ResponseModel()
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Lưu thành công"
            };
        }
    }
}
