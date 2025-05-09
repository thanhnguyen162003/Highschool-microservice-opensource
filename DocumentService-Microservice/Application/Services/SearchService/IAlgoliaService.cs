using Domain.Enums;

namespace Application.Services.SearchService
{
    public interface IAlgoliaService
    {
        Task<bool> MigrateData();
        Task<bool> AddOrUpdateRecord(IndexName name, string objectID, dynamic model);
        Task<bool> DeleteRecord(IndexName name, string objectID);
        Task<bool> MigrateDataCourse();
        Task<bool> AddOrUpdateCourseRecord(UpdateCourseType type, Guid id, string name);
        Task<bool> DeleteCourseRecord(UpdateCourseType type, Guid id);
    }
}
