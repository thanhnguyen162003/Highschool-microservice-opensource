
using Application.Common.Models;
using Application.Common.Models.RoadmapDataModel;
using Application.Common.Models.SearchModel;
using Application.Common.Models.StatisticModel;
using Domain.Entities;
using Domain.QueriesFilter;
using SharedProject.Models;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RoadMapSectionCreateRequestModel, Roadmap>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoadmapId))
            .ReverseMap();

        CreateMap<RoadmapCreateRequestModel, Roadmap>().ReverseMap();
        
        CreateMap<RoadmapResponseModel, Roadmap>().ReverseMap();
        
        CreateMap<RoadmapDetailResponseModel, Roadmap>().ReverseMap();
        
        CreateMap<RecentViewModel, RecentView>().ReverseMap();
        CreateMap<UserActivityModel, UserActivityResponseModel>().ReverseMap();

        CreateMap<UserDataAnalyseModel, UserAnalyseEntity>()
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects));

        CreateMap<CourseQueryModel, CourseSearchResponseModel>()
            .ForPath(dest => dest.SubjectCurriculum.Id, opt => opt.MapFrom(src => src.SubjectId))
            .ForPath(dest => dest.SubjectCurriculum.Name, opt => opt.MapFrom(src => src.SubjectName))
            .ForPath(dest => dest.Chapter.Id, opt => opt.MapFrom(src => src.ChapterId))
            .ForPath(dest => dest.Chapter.Name, opt => opt.MapFrom(src => src.ChapterName))
            .ForPath(dest => dest.Lesson.Id, opt => opt.MapFrom(src => src.LessonId))
            .ForPath(dest => dest.Lesson.Name, opt => opt.MapFrom(src => src.LessonName))
            .ForPath(dest => dest.Subject.Id, opt => opt.MapFrom(src => src.SubjectId))
            .ForPath(dest => dest.Subject.Name, opt => opt.MapFrom(src => src.SubjectName))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
