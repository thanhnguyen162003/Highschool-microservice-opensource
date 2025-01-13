using Application.Common.Models;
using Application.Common.Models.CategoryModel;
using Application.Common.Models.ChapterModel;
using Application.Common.Models.CurriculumModel;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardFeatureModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.InformationModel;
using Application.Common.Models.LessonModel;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.Models.SubjectCurriculumModel;
using Application.Common.Models.SubjectModel;
using Application.Common.Models.TheoryModel;
using Application.Common.Models.UserQuizProgress;
using Domain.CustomModel;
using Domain.Entities;
using Infrastructure.Constraints;
using SharedProject.ConsumeModel;


namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SubjectModel, SubjectResponseModel>()
            .ReverseMap();
        
        CreateMap<SubjectModel, SubjectUpdateModelRequest>()
            .ReverseMap();
        
        CreateMap<Subject, SubjectCreateRequestModel>()
            .ReverseMap();
        
        // //Flashcard Feature Study Mapping
        // CreateMap<FlashcardContent, FlashcardContentQuestionModel>()
        //     .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
        //     .ReverseMap();
        
        CreateMap<Flashcard, FlashcardModel>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.SubjectSlug, opt => opt.MapFrom(src => src.Subject.Slug))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Subject.Category.CategoryName))
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ReverseMap();
        
        CreateMap<Flashcard, FlashcardCreateRequestModel>()
            .ReverseMap();
        
        CreateMap<Flashcard, FlashcardUpdateRequestModel>()
            .ReverseMap();
        
        CreateMap<FlashcardContent, FlashcardContentResponseModel>()
            .ReverseMap();
        
        CreateMap<FlashcardContent, FlashcardContentModel>()
            .ReverseMap();
        
        CreateMap<FlashcardContentResponseModel, FlashcardContentModel>()
            .ReverseMap();
        
        CreateMap<FlashcardContent, FlashcardContentCreateRequestModel>()
            .ReverseMap();
        
        CreateMap<FlashcardContent, FlashcardContentUpdateRequestModel>()
            .ReverseMap();
        
        CreateMap<FlashcardModel, FlashcardDetailResponseModel>()
            .ReverseMap();
        
        CreateMap<FlashcardModel, FlashcardCreateRequestModel>()
            .ReverseMap();
        
        CreateMap<FlashcardModel, FlashcardResponseModel>()
            .ReverseMap();
        
        CreateMap<FlashcardModel, FlashcardRecommendResponseModel>()
            .ReverseMap();
        
        CreateMap<Chapter, ChapterModel>()
            .ForMember(dest => dest.NumberLesson, opt => opt.MapFrom(src => src.Lessons.Count(x => x.DeletedAt.Equals(null))))
            .ReverseMap();
        
        CreateMap<ChapterUpdateRequestModel, Chapter>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.HasValue ? src.Semester.Value.ToString() : null))
            .ReverseMap();
        
        CreateMap<Chapter, ChapterSubjectModel>()
            .ReverseMap();
        
        CreateMap<ChapterResponseModel, ChapterModel>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.HasValue ? src.Semester.Value.ToString() : null))
            .ReverseMap();
        
        CreateMap<Chapter, ChapterResponseModel>()
            .ForMember(dest => dest.NumberLesson, opt => opt.MapFrom(src => src.Lessons.Count(x => x.DeletedAt.Equals(null))))
            .ForMember(dest => dest.CurriculumName, opt => opt.MapFrom(src=>src.SubjectCurriculum.Curriculum.CurriculumName))
            .ForMember(dest => dest.SubjectCurriculumId, opt => opt.MapFrom(src=>src.SubjectCurriculumId))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.ToString()))
            .ReverseMap();
        
        CreateMap<ChapterCreateRequestModel, ChapterModel>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.HasValue ? src.Semester.Value.ToString() : null))
            .ReverseMap();
        
        CreateMap<ChapterCreateRequestModel, Chapter>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.HasValue ? src.Semester.Value.ToString() : null))
            .ReverseMap();
        
        CreateMap<ChapterUpdateRequestModel, ChapterModel>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.HasValue ? src.Semester.Value.ToString() : null))
            .ReverseMap();
        
        CreateMap<FlashcardDraftResponseModel, Flashcard>()
            .ReverseMap();
        
        CreateMap<Flashcard, FlashcardResponseModel>()
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ReverseMap();
        
        CreateMap<Flashcard, FlashcardRecommendResponseModel>()
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ReverseMap();

        CreateMap<FolderUser, FolderUserResponse>()
            .ForMember(dest => dest.CountFlashCard, opt => opt.MapFrom(src => src.FlashcardFolders.Count(x => x.Flashcard.Status.Equals(StatusConstrains.OPEN))))
            .ForMember(dest => dest.CountDocument, opt => opt.MapFrom(src => src.DocumentFolders.Count()))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<FolderUser, ExisItemFolderUserResponse>()
            .ForMember(dest => dest.CountFlashCard, opt => opt.MapFrom(src => src.FlashcardFolders.Count(x => x.Flashcard.Status.Equals(StatusConstrains.OPEN))))
            .ForMember(dest => dest.CountDocument, opt => opt.MapFrom(src => src.DocumentFolders.Count()))
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility))
            .ForMember(dest => dest.IsFlashcardInclude, opt => opt.MapFrom((src, dest, _, context) => src.FlashcardFolders.Any(fc => fc.FlashcardId.ToString().Equals(context.Items["FlashcardId"]?.ToString()))))
            .ForMember(dest => dest.IsDocumentInclude, opt => opt.MapFrom((src, dest, _, context) => src.DocumentFolders.Any(fc => fc.DocumentId.ToString().Equals(context.Items["DocumentId"]?.ToString()))))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Mapping Lesson -> LessonModel
        CreateMap<Lesson, LessonModel>()
            .ForMember(dest => dest.TheoryCount, opt => opt.MapFrom(src => src.Theories.Count(x => x.DeletedAt == null)))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<LessonModel, LessonResponseModel>()
            .ReverseMap();

        CreateMap<Lesson, LessonDetailResponseModel>()
            .ForMember(dest => dest.TheoryCount, opt => opt.MapFrom(src => src.Theories.Count(x => x.DeletedAt == null)))
            .ForMember(dest => dest.Theories, opt => opt.MapFrom(src => src.Theories
                .Where(x => x.DeletedAt == null)
                .Select(x => new TheoryLessonResponseModel { Id = x.Id, TheoryTitle = x.TheoryName, TheoryDescription = x.TheoryDescription, TheoryContentJson = x.TheoryContentJson, TheoryContentHtml = x.TheoryContentHtml })))
            .ReverseMap();
            
        // Mapping Lesson -> LessonResponseModel (Filter Theories)
        CreateMap<Lesson, LessonResponseModel>()
            .ReverseMap();
            

        CreateMap<LessonResponseModel, Lesson>()
            .ReverseMap();
        
        CreateMap<LessonCreateRequestModel, LessonModel>()
            .ReverseMap();
        
        CreateMap<LessonCreateRequestModel, Lesson>()
            .ReverseMap();
        
        CreateMap<LessonUpdateRequestModel, LessonModel>()
            .ReverseMap();
        
        CreateMap<LessonDetailResponseModel, LessonModel>()
            .ReverseMap();
        
        CreateMap<Subject, SubjectModel>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.CategoryName)) 
            .ReverseMap();
        
        CreateMap<TheoryCreateRequestModel, Theory>()
            .ReverseMap();
        
        CreateMap<TheoryResponseModel, Theory>()
            .ReverseMap();
        
        CreateMap<TheoryUpdateRequestModel, Theory>()
            .ReverseMap();
        
        CreateMap<RecommendedDataModel, RecommendedData>()
            .ForMember(dest => dest.ObjectId,
                opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
        
        CreateMap<Category, CategoryCreateRequestModel>()
            .ReverseMap();
        
        CreateMap<Category, CategoryReponseModel>()
            .ReverseMap();
        
        //other information
       
        CreateMap<Province, ProvinceCreateRequestModel>()
            .ForMember(dest => dest.ProvinceId,
                opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
        
        CreateMap<School, SchoolCreateRequestModel>()
            .ForMember(dest => dest.ProvinceId,
                opt => opt.MapFrom(src => src.ProvinceId))
            .ReverseMap();
        
        CreateMap<Province, ProvinceResponseModel>()
            .ForMember(dest => dest.ProvinceId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NumberSchool, opt => opt.MapFrom(src => src.Schools.Count(x => x.DeletedAt == null)))
            .ReverseMap();
        
        CreateMap<School, SchoolResponseModel>()
            .ForMember(dest => dest.ProvinceId,
                opt => opt.MapFrom(src => src.ProvinceId))
            .ForMember(dest => dest.ProvinceName,
                opt => opt.MapFrom(src => src.Province.ProvinceName))
            .ForMember(dest => dest.NumberDocuments, opt => opt.MapFrom(src => src.Documents.Count(x => x.DeletedAt == null)))
            .ReverseMap();
        
        CreateMap<Curriculum, CurriculumResponseModel>()
            .ReverseMap();
        
        CreateMap<Curriculum, CurriculumCreateRequestModel>()
            .ReverseMap();

        CreateMap<SubjectCurriculum, SubjectCurriculumResponseModel>()
            .ForMember(dest => dest.SubjectCurriculumId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SubjectName,
                opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.CurriculumName,
                opt => opt.MapFrom(src => src.Curriculum.CurriculumName))
            .ReverseMap();

        CreateMap<SubjectCurriculum, SubjectCurriculumCreateRequestModel>()
            .ReverseMap();

        CreateMap<Question, QuestionResponseModel>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.QuestionAnswers))
            .ReverseMap();

        CreateMap<QuestionAnswer, QuestionAnswerResponseModel>()
            .ReverseMap();

        CreateMap<UserQuizProgress, UserQuizProgressResponseModel>().ReverseMap();

        #region Document
        CreateMap<CreateDocumentRequestModel, Document>()
            .BeforeMap((_, dest) =>
            {
                dest.Like = 0;
                dest.View = 0;
                dest.Download = 0;
            });


        CreateMap<UpdateDocumentRequestModel, Document>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Document, DocumentResponseModel>()
            .ForPath(dest => dest.SubjectCurriculum.SubjectCurriculumId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Id))
            .ForPath(dest => dest.SubjectCurriculum.SubjectId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.SubjectId))
            .ForPath(dest => dest.SubjectCurriculum.SubjectName,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.SubjectName))
            .ForPath(dest => dest.SubjectCurriculum.CurriculumId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.CurriculumId))
            .ForPath(dest => dest.SubjectCurriculum.CurriculumName,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Curriculum.CurriculumName))
            .ForPath(dest => dest.Category.CategoryId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.Category.Id))
            .ForPath(dest => dest.Category.CategoryName,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.Category.CategoryName))
            .ForPath(dest => dest.Category.CategorySlug,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.Category.CategorySlug))
            .ForMember(dest => dest.DocumentSlug,
                opt => opt.MapFrom(src => src.Slug));
        #endregion

        #region Base Entity Response Model
        CreateMap<Subject, EntitySimpleModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SubjectName))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug));

        CreateMap<Chapter, EntitySimpleModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ChapterName));

        CreateMap<Lesson, EntitySimpleModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LessonName))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug));
        #endregion
    }
}