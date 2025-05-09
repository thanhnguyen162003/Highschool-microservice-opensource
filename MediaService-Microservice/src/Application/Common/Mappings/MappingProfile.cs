using Application.Common.Models.DocumentModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.NewsTagModel;
using Application.Common.Models.TheoryModel;
using Domain.CustomEntities;
using Domain.Entities;
using Domain.Entities.SqlEntites;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DocumentFile, DocumentFileResponseModel>();
        CreateMap<TheoryFile, TheoryFileResponseModel>().ReverseMap();
        CreateMap<TheoryFile, TheoryFileUploadRequestModel>().ReverseMap();
        CreateMap<NewsFile, NewsFileResponseModel>().ReverseMap();
        CreateMap<NewsFile, NewsFileUploadRequestModel>().ReverseMap();

        #region News
        CreateMap<NewsCreateRequestModel, News>()
            .ReverseMap();
        CreateMap<News, NewsUpdateRequestModel>()
            .ReverseMap();
        CreateMap<News, NewsResponseModel>()
            .ReverseMap();
        #endregion

        #region NewsTag
        CreateMap<NewsTag, NewsTagCreateRequestModel>()
            .ReverseMap();
        CreateMap<NewsTag, NewsTagResponseModel>()
            .ReverseMap();
        CreateMap<NewsTag, NewsTagUpdateRequestModel>()
            .ReverseMap();

        #endregion
    }

}
