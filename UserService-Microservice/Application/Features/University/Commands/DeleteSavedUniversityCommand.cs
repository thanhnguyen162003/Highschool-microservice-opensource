using Application.Common.Interfaces.CacheInterface;
using Application.Common.Models.Common;
using Domain.Common.Ultils;
using Domain.MongoEntities;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Features.University.Commands
{
    public class DeleteSavedUniversityCommand : IRequest<ResponseModel>
    {
        public Guid UniversityId { get; init; }
    }

    public class DeleteSavedUniversityCommandHandler(CareerMongoDatabaseContext context,
        IHttpContextAccessor httpContextAccessor, ICleanCacheService cleanCacheService) : IRequestHandler<DeleteSavedUniversityCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(DeleteSavedUniversityCommand request, CancellationToken cancellationToken)
        {
            var filter = Builders<SavedUniversity>.Filter.And(
                Builders<SavedUniversity>.Filter.Eq(uni => uni.UniversityId, request.UniversityId),
                Builders<SavedUniversity>.Filter.Eq(uni => uni.UserId, httpContextAccessor.HttpContext.User.GetUserIdFromToken()));

            await context.SavedUniversities.DeleteOneAsync(filter);
            await cleanCacheService.ClearRelatedCacheUniversitySave();
            return new ResponseModel()
            {
                Status = System.Net.HttpStatusCode.NoContent,
                Message = "Xóa thành công"
            };
        }
    }
}
