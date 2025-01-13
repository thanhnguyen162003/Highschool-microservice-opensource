using Domain.CustomEntities;
using Domain.CustomModel;

namespace Application.Common.Models.ChapterModel
{
    public class PagedChapterResponseModelWithProgress
    {
        public Domain.CustomModel.SubjectModel? SubjectModel { get; set; }
        public PagedList<ChapterResponseModel> Items { get; set; }        
    }
}
