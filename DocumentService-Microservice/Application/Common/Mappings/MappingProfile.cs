using Application.Common.Models;
using Application.Common.Models.CalendarModel;
using Application.Common.Models.ChapterModel;
using Application.Common.Models.ContainerModel;
using Application.Common.Models.CurriculumModel;
using Application.Common.Models.DocumentModel;
using Application.Common.Models.FlashcardContentModel;
using Application.Common.Models.FlashcardFeatureModel;
using Application.Common.Models.FlashcardFolderModel;
using Application.Common.Models.FlashcardModel;
using Application.Common.Models.FSRSPresetModel;
using Application.Common.Models.InformationModel;
using Application.Common.Models.LessonModel;
using Application.Common.Models.MasterSubjectModel;
using Application.Common.Models.NewsModel;
using Application.Common.Models.QuestionAnswerModel;
using Application.Common.Models.QuestionModel;
using Application.Common.Models.StarredTermModel;
using Application.Common.Models.StudiableTermModel;
using Application.Common.Models.SubjectCurriculumModel;
using Application.Common.Models.SubjectModel;
using Application.Common.Models.TagModel;
using Application.Common.Models.TheoryModel;
using Application.Common.Models.UserQuizProgress;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Constraints;
using SharedProject.ConsumeModel;


namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
		#region Subject
		CreateMap<SubjectModel, SubjectResponseModel>()
            .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
        
        CreateMap<SubjectModel, SubjectUpdateModelRequest>()
            .ReverseMap();
        
        CreateMap<Subject, SubjectCreateRequestModel>()
            .ReverseMap();

        CreateMap<Subject, SubjectModel>()
            .ForMember(dest => dest.MasterSubjectId,
                opt => opt.MapFrom(src => src.MasterSubject.Id))
            .ForMember(dest => dest.MasterSubjectName,
                opt => opt.MapFrom(src => src.MasterSubject.MasterSubjectName))
			.ForMember(dest => dest.MasterSubjectSlug,
				opt => opt.MapFrom(src => src.MasterSubject.MasterSubjectSlug))
			.ReverseMap();

        CreateMap<Subject, SubjectResponseModel>()
            .ForMember(dest => dest.MasterSubjectId,
                opt => opt.MapFrom(src => src.MasterSubject.Id))
            .ForMember(dest => dest.MasterSubjectName,
                opt => opt.MapFrom(src => src.MasterSubject.MasterSubjectName))
            .ForMember(dest => dest.MasterSubjectSlug,
                opt => opt.MapFrom(src => src.MasterSubject.MasterSubjectSlug))
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

		CreateMap<Curriculum, CurriculumResponseModel>()
			.ReverseMap();

		CreateMap<Curriculum, CurriculumCreateRequestModel>()
			.ReverseMap();

		CreateMap<Curriculum, CurriculumUpdateRequestModel>()
			.ReverseMap();
		#endregion

		#region Flashcard
		CreateMap<Flashcard, FlashcardModel>()
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Subject ? src.Subject.SubjectName :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.LessonName :
                src.FlashcardType == FlashcardType.Chapter ? src.Chapter.ChapterName :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculum.SubjectCurriculumName : null))
            .ForMember(dest => dest.EntitySlug, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Subject ? src.Subject.Slug :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.Slug : null))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Subject.Category))
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.FlashcardTags.Select(ft => ft.Tag.Name).ToList()))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null));

        CreateMap<Flashcard, FlashcardStudyResponseModel>()
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Subject ? src.Subject.SubjectName :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.LessonName :
                src.FlashcardType == FlashcardType.Chapter ? src.Chapter.ChapterName :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculum.SubjectCurriculumName : null))
            .ForMember(dest => dest.EntitySlug, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Subject ? src.Subject.Slug :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.Slug : null))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Subject.Category))
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.FlashcardTags.Select(ft => ft.Tag.Name).ToList()))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null));

        CreateMap<FlashcardModel, Flashcard>()
            .ForMember(dest => dest.FlashcardContents, opt => opt.Ignore())
            .ForMember(dest => dest.FlashcardTags, opt => opt.Ignore())
            .ForMember(dest => dest.Subject, opt => opt.Ignore())
            .ForMember(dest => dest.Chapter, opt => opt.Ignore())
            .ForMember(dest => dest.Lesson, opt => opt.Ignore())
            .ForMember(dest => dest.SubjectCurriculum, opt => opt.Ignore())
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.EntityId : null))
            .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Chapter ? src.EntityId : null))
            .ForMember(dest => dest.SubjectCurriculumId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.EntityId : null))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Subject ? src.EntityId : null));

		CreateMap<FlashcardContentAIRequestModel, FlashcardContentCreateRequestModel>()
			.ReverseMap();

        CreateMap<FlashcardContent, FlashcardContentCreateRequestModel>()
            .ReverseMap();

        CreateMap<FlashcardUpdateRequestModel, Flashcard>()
            .ForMember(dest => dest.FlashcardTags, opt => opt.Ignore())
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.EntityId : null))
            .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Chapter ? src.EntityId : null))
            .ForMember(dest => dest.SubjectCurriculumId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.EntityId : null))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Subject ? src.EntityId : null))
            .ReverseMap();

        CreateMap<Flashcard, FlashcardUpdateRequestModel>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.FlashcardTags.Select(ft => ft.Tag.Name).ToList()))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null))
            .ReverseMap();

        CreateMap<FlashcardContent, FlashcardContentResponseModel>()
            .ForMember(dest => dest.IsStarred, opt => opt.Ignore())
            .ForMember(dest => dest.IsLearned, opt => opt.Ignore())
            .ReverseMap();

		CreateMap<FlashcardContent, FlashcardContentListCreateRequestModel>()
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
            .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<FlashcardModel, FlashcardRecommendResponseModel>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ReverseMap();

        CreateMap<Flashcard, FlashcardResponseModel>()
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.FlashcardTags.Select(ft => ft.Tag.Name).ToList()))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null))
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Subject ? src.Subject.SubjectName :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.LessonName :
                src.FlashcardType == FlashcardType.Chapter ? src.Chapter.ChapterName :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculum.SubjectCurriculumName : null))
            .ReverseMap();

        CreateMap<Flashcard, FlashcardRecommendResponseModel>()
            .ForMember(dest => dest.NumberOfFlashcardContent, opt => opt.MapFrom(src => src.FlashcardContents.Count(x => x.DeletedAt.Equals(null))))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.FlashcardTags.Select(ft => ft.Tag.Name).ToList()))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null))
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Subject ? src.Subject.SubjectName :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.LessonName :
                src.FlashcardType == FlashcardType.Chapter ? src.Chapter.ChapterName :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculum.SubjectCurriculumName : null))
            .ReverseMap();

        CreateMap<FlashcardDraftResponseModel, Flashcard>();
        CreateMap<Flashcard, FlashcardDraftResponseModel>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null))
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src =>
                src.FlashcardType == FlashcardType.Subject ? src.Subject.SubjectName :
                src.FlashcardType == FlashcardType.Lesson ? src.Lesson.LessonName :
                src.FlashcardType == FlashcardType.Chapter ? src.Chapter.ChapterName :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculum.SubjectCurriculumName : null));

        CreateMap<Flashcard, FlashcardCreateRequestModel>()
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => 
                src.FlashcardType == FlashcardType.Lesson ? src.LessonId :
                src.FlashcardType == FlashcardType.Chapter ? src.ChapterId :
                src.FlashcardType == FlashcardType.SubjectCurriculum ? src.SubjectCurriculumId :
                src.FlashcardType == FlashcardType.Subject ? src.SubjectId : null));
                
        CreateMap<FlashcardCreateRequestModel, Flashcard>();
        #endregion

        #region Chapter
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
		#endregion

		#region Folder
		CreateMap<FolderUser, FolderUserResponse>()
            .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CountFlashCard, opt => opt.MapFrom(src => src.FlashcardFolders.Count(x => x.Flashcard.Status.Equals(StatusConstrains.OPEN))))
            .ForMember(dest => dest.CountDocument, opt => opt.MapFrom(src => src.DocumentFolders.Count()))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<FolderUser, ExisItemFolderUserResponse>()
            .ForMember(dest => dest.CountFlashCard, opt => opt.MapFrom((src, dest, _, context) =>
            {
                var isMine = context.Items.ContainsKey("IsMine") && bool.TryParse(context.Items["IsMine"]?.ToString(), out var val) ? val : false;
                return isMine ? src.FlashcardFolders.Count() : src.FlashcardFolders.Count(x => x.Flashcard.Status.Equals(StatusConstrains.OPEN));
            }))
            .ForMember(dest => dest.CountDocument, opt => opt.MapFrom(src => src.DocumentFolders.Count()))
            .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility))
            .ForMember(dest => dest.IsFlashcardInclude, opt => opt.MapFrom((src, dest, _, context) => src.FlashcardFolders.Any(fc => fc.FlashcardId.ToString().Equals(context.Items["FlashcardId"]?.ToString()))))
            .ForMember(dest => dest.IsDocumentInclude, opt => opt.MapFrom((src, dest, _, context) => src.DocumentFolders.Any(fc => fc.DocumentId.ToString().Equals(context.Items["DocumentId"]?.ToString()))))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
		#endregion

		#region Lesson
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
		#endregion

		#region Theory
		CreateMap<TheoryCreateRequestModel, Theory>()
            .ReverseMap();
        
        CreateMap<TheoryResponseModel, Theory>()
            .ReverseMap();
        
        CreateMap<TheoryUpdateRequestModel, Theory>()
            .ReverseMap();
		#endregion

		#region Other Information
		CreateMap<RecommendedDataModel, RecommendedData>()
            .ForMember(dest => dest.ObjectId,
                opt => opt.MapFrom(src => src.Id))
            .ReverseMap();
        
        CreateMap<MasterSubject, MasterSubjectCreateRequestModel>()
            .ReverseMap();
        
        CreateMap<MasterSubject, MasterSubjectReponseModel>()
            .ReverseMap();
       
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
		#endregion

		#region Question, Quiz
		CreateMap<Question, QuestionResponseModel>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.QuestionAnswers))
            .ReverseMap();

        CreateMap<QuestionAnswer, QuestionAnswerResponseModel>()
            .ReverseMap();

        CreateMap<UserQuizProgress, UserQuizProgressResponseModel>().ReverseMap();
		#endregion

		#region Container
		CreateMap<Container, ContainerResponseModel>().ReverseMap();
		CreateMap<Container, ContainerUpdateRequestModel>().ReverseMap();
        #endregion
        
        #region StarredTerm

        #endregion

        #region StudiableTerm
        CreateMap<StudiableTermRequestModel, StudiableTerm>()
            .ForMember(dest => dest.Mode, opt => opt.MapFrom(src => src.Mode ? "Learn" : "Flashcard"))
            .ReverseMap();
        CreateMap<StudiableTerm, StudiableTermResponseModel>()
            .ReverseMap();
        #endregion

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
            .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.Id))
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
            .ForPath(dest => dest.Category,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.Category))
            .ForPath(dest => dest.MasterSubject.MasterSubjectId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.Id))
            .ForPath(dest => dest.MasterSubject.MasterSubjectName,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.MasterSubjectName))
            .ForPath(dest => dest.MasterSubject.MasterSubjectSlug,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.MasterSubjectSlug))
            .ForMember(dest => dest.DocumentSlug,
                opt => opt.MapFrom(src => src.Slug)).ReverseMap();

        CreateMap<Document, DocumentManagementResponseModel>()
            .ForMember(dest => dest.ObjectID, opt => opt.MapFrom(src => src.Id))
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
            .ForPath(dest => dest.Category,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.Category))
            .ForPath(dest => dest.MasterSubject.MasterSubjectId,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.Id))
            .ForPath(dest => dest.MasterSubject.MasterSubjectName,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.MasterSubjectName))
            .ForPath(dest => dest.MasterSubject.MasterSubjectSlug,
                opt => opt.MapFrom(src => src.SubjectCurriculum.Subject.MasterSubject.MasterSubjectSlug))
            .ForMember(dest => dest.DocumentSlug,
                opt => opt.MapFrom(src => src.Slug))
            .ForPath(dest => dest.School.SchoolId,
                opt => opt.MapFrom(src => src.School.Id))
            .ForPath(dest => dest.School.SchoolName,
                opt => opt.MapFrom(src => src.School.SchoolName)).ReverseMap();
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

        #region Calendar
        CreateMap<EventCalendarCreateModel, EventCalendarModel>()
            .ReverseMap();
        CreateMap<EventTimeRequest, EventTime>()
            .ReverseMap();
        CreateMap<Attendee, AttendeeCreateRequest>()
            .ReverseMap();
        CreateMap<ConferenceDataCreateRequest, ConferenceData>()
            .ReverseMap();
        CreateMap<CreateRequest, CreateRequestForConferenceData>()
            .ReverseMap();
        #endregion

        #region Tag
        CreateMap<Tag, TagResponseModel>()
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => src.FlashcardTags.Count));
        #endregion

        #region FSRS
        CreateMap<FSRSPreset, FSRSPresetResponseModel>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new Author
            {
                AuthorId = src.UserId
            }))
            .ReverseMap();
        CreateMap<FSRSPreset, FSRSPresetCreateRequest>()
            .ReverseMap();
        #endregion

    }
}