using Application;
using Application.Common.Models.AuthenModel;
using Application.Common.Models.External;
using Application.Common.Models.MajorCategoryModel;
using Application.Common.Models.MajorModel;
using Application.Common.Models.OccupationModel;
using Application.Common.Models.ReportModel;
using Application.Common.Models.UniversityMajor;
using Application.Common.Models.UniversityModel;
using Application.Common.Models.UserModel;
using Application.Features.User.UpdateBaseUser;
using Application.Features.User.UpdateStudent;
using Application.ProduceMessage;
using Domain.Common.Models;
using Domain.Common.Ultils;
using Domain.Entities;
using Domain.Enumerations;
using Domain.MongoEntities;
using SharedProject.Models;

namespace Domain.Common.Mappings;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<BaseUser, LoginResponseModel>()
			.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ProfilePicture))
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => EnumExtensions.ConvertToRoleValue(src.RoleId)!.ToString()))
			.ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateBaseUserCommand, BaseUser>()
			.ForMember(dest => dest.Student, opt => opt.Ignore())
			.ForMember(dest => dest.Teacher, opt => opt.Ignore())
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<StudentRequestModel, Student>()
			.ForMember(dest => dest.TypeExam, opt => opt.MapFrom(src => string.Join(",", src.TypeExams)))
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<UpdateStudentCommand, Student>()
            .ForMember(dest => dest.TypeExam, opt => opt.MapFrom(src => string.Join(",", src.TypeExams)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<TeacherRequestModel, Teacher>()
			.ReverseMap()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<BaseUser, StudentInfoResponseModel>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<BaseUser, TeacherInfoResponseModel>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<Student, StudentInfoResponseModel>()
			.ForMember(dest => dest.MbtiType, opt => opt.MapFrom(src => src.MbtiType.ToString()))
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BaseUser.Id))
			.ReverseMap()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<Teacher, TeacherInfoResponseModel>()
			.ReverseMap()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<BaseUser, BaseUserResponse>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName));

		CreateMap<PagedList<BaseUser>, PagedList<BaseUserResponse>>();

		CreateMap<BaseUser, StudentResponse>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
			.ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Student!.Grade))
			.ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.Student!.SchoolName))
			.ReverseMap();

		CreateMap<PagedList<BaseUser>, PagedList<StudentResponse>>().ReverseMap();

		CreateMap<BaseUser, TeacherResponse>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName))
			.ForMember(dest => dest.SubjectsTaught, opt => opt.MapFrom(src => src.Teacher!.SubjectsTaught))
			.ForMember(dest => dest.GraduatedUniversity, opt => opt.MapFrom(src => src.Teacher!.GraduatedUniversity))
			.ReverseMap();

		CreateMap<PagedList<BaseUser>, PagedList<TeacherResponse>>().ReverseMap();

		CreateMap<BaseUser, AuthorFlashcardResponse>()
			.ForMember(dest => dest.IsStudent, opt => opt.MapFrom(src => (int)RoleEnum.Student == src.RoleId));

		CreateMap<BaseUser, BaseUserInforResponseModel>()
			.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.RoleName));

		CreateMap<Report, ReportResponseModel>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
			.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Fullname))
			.ForMember(dest => dest.ImageReports, opt => opt.MapFrom(src => src.ImageReports.Select(ir => ir.ImageUrl)));

		CreateMap<PagedList<Report>, PagedList<ReportResponseModel>>().ReverseMap();

		CreateMap<Student, UserUpdatedMessage>()
			.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.BaseUserId))
			.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.BaseUser.Address))
			.ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.BaseUser.UserSubjects.Select(us => us.SubjectId)));

		CreateMap<RoadmapUserKafkaMessageModel, Roadmap>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoadmapId))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoadmapName))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RoadmapDescription))
			.ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => string.Join(",", src.RoadmapSubjectIds)))
			.ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => string.Join(",", src.RoadmapDocumentIds)))
			.ForMember(dest => dest.TypeExam, opt => opt.MapFrom(src => string.Join(",", src.TypeExam)))
			.ReverseMap()
			.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

		CreateMap<Roadmap, RoadmapResponseModel>()
			.ForMember(dest => dest.SubjectIds, opt => opt.MapFrom(src => src.SubjectId.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()))
			.ForMember(dest => dest.DocumentIds, opt => opt.MapFrom(src => src.DocumentId.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()))
			.ForMember(dest => dest.TypeExam, opt => opt.MapFrom(src => src.TypeExam.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()));

		CreateMap<UniversityRequestModel, University>();

		CreateMap<University, UniversityResponseModel>();

        CreateMap<MajorCategoryRequestModel, MajorCategory>().ReverseMap();

        CreateMap<MajorCategory, MajorCategoryResponseModel>();

        CreateMap<MajorRequestModel, Major>();

		CreateMap<Major, MajorResponseModel>();

        CreateMap<UniversityMajorRequestModel, UniversityMajor>().ReverseMap();

		CreateMap<UniversityMajor, UniversityMajorResponseModel>().ReverseMap();

        CreateMap<OccupationRequestModel, Occupation>().ReverseMap();

		CreateMap<Occupation, OccupationResponseModel>();
        CreateMap<University, UniversityNameResponseModel>().ReverseMap();
        CreateMap<Major, MajorNameResponseModel>().ReverseMap();
        CreateMap<MajorCategory, MajorCategoryNameResponseModel>().ReverseMap();
		CreateMap<Student, StudentCardResponseModel>()
			.ForMember(dest => dest.BaseUserInfo, opt => opt.MapFrom(src => src.BaseUser));

		CreateMap<BaseUser, AcademicUserResponse>()
			.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.ProfilePicture))
			.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Fullname))
			.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role!.RoleName))
			.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
			.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<UniversityTags, UniversityTagResponseModel>().ReverseMap();
    }
}