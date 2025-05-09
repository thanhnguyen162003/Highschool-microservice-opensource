using Application.Common.Models.AssignmentModel;
using Application.Common.Models.OtherModel;
using Application.Common.Models.SubmissionContent;
using Application.Common.Models.TestContent;
using Application.Common.Models.ZoneMembershipModel;
using Application.Common.Models.ZoneModel;
using Application.Features.AssignmentFeatures.Commands;
using Application.Features.ZoneFeatures.Commands;
using AutoMapper;
using Domain.DaprModels;
using Domain.Entity;
using Domain.Models.Common;
using Google.Api;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace Application.Common.Mappings
{
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {
            Config();
        }

        private void Config()
        {
            #region Zone
            CreateMap<CreateZoneCommand, Zone>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateZoneCommand, Zone>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.TransferOwner))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Zone, ZoneResponseModel>()
            .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.DocumentIds.Count()))
            .ForMember(dest => dest.FlashcardCount, opt => opt.MapFrom(src => src.FlashcardIds.Count()))
            .ForMember(dest => dest.FolderCount, opt => opt.MapFrom(src => src.FolderIds.Count()))
            .ForMember(dest => dest.AssignmentCount, opt => opt.MapFrom(src => src.Assignments.Count(x=>x.Published == true)))
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.ZoneMemberships.Count(x => x.DeletedAt == null)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.Value.ToString() : "Unknown"))
            .ForMember(dest => dest.IsOwner, opt => opt.MapFrom((src, dest, _, context) => context.Items["UserId"].Equals(src.CreatedBy)))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PagedList<Zone>, PagedList<ZoneResponseModel>>();

            CreateMap<Zone, ZoneDetailResponseModel>()
            .ForMember(dest => dest.ZoneMembershipsCount, opt => opt.MapFrom(src => src.ZoneMemberships.Count()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.Value.ToString() : "Unknown"))
            .ForMember(dest => dest.ZoneBansCount, opt => opt.MapFrom(src => src.ZoneBans.Count()))
            .ForMember(dest => dest.PendingZoneInvitesCount, opt => opt.MapFrom(src => src.PendingZoneInvites.Count()))
            .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.Assignments))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserResponseDapr, UserModel>();

            #endregion

            #region ZoneMembership
            CreateMap<ZoneMembership, ZoneMembershipResponseModel>()
            .ForMember(dest => dest.SubmissionsCount, opt => opt.MapFrom(src => src.Submissions.Count()))
            .ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region Assignment
            CreateMap<CreateAssignmentCommand, Assignment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateAssignmentCommand, Assignment>()
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Assignment, AssignmentResponseModel>()
                .ForMember(dest => dest.SubmissionsCount, opt => opt.MapFrom(src => src.Submissions.Count()))
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Assignment, AssignmentDetailResponseModel>()
                .ForMember(dest => dest.Submissions, opt => opt.MapFrom(src => src.Submissions))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PagedList<Assignment>, PagedList<AssignmentResponseModel>>();
            #endregion

            #region Submissions
            CreateMap<Submission, SubmissionResponseModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Member.UserId))
                .ReverseMap()
                .ForAllMembers(opts =>opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region TestContent
            CreateMap<TestContentCreateModel, TestContent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Assignmentid, opt => opt.MapFrom((src, dest, _, context) => context.Items["Assignmentid"]))
                .ForMember(dest => dest.Order, opt => opt.MapFrom((src, dest, _, context) => context.Items["Order"]))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => SerializeAnswers(src.Answers)));

            CreateMap<TestContent, TestContentResponseModel>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => DeserializeAnswers(src.Answers)))
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TestContent, TestContentSubmissionResponseModel>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            #endregion

            #region Member

            CreateMap<ZoneMembership, MemberZoneResponseModel>()
                .ForMember(src => src.Role, opt => opt.MapFrom(src => src.Type));

            CreateMap<PendingZoneInvite, MemberPendingResponseModel>()
                .ForMember(src => src.Role, opt => opt.MapFrom(src => src.Type));

            CreateMap<PagedList<ZoneMembership>, PagedList<MemberZoneResponseModel>>();
            CreateMap<PagedList<PendingZoneInvite>, PagedList<MemberPendingResponseModel>>();

            CreateMap<AcademicUserResponse, UserModel>()
                .ForMember(src => src.UserId, opt => opt.MapFrom(src => src.UserId));

            #endregion
        }
        private static string SerializeAnswers(List<string>? answers)
        {
            if (answers == null) return null;
            return JsonSerializer.Serialize(answers);
        }

        private static List<string>? DeserializeAnswers(string? answersJson)
        {
            if (string.IsNullOrEmpty(answersJson)) return null; // Handle null or empty JSON string

            try
            {
                return JsonSerializer.Deserialize<List<string>>(answersJson); // Or JsonConvert.DeserializeObject<List<string>>(answersJson);
            }
            catch (JsonException ex) // Handle potential JSON parsing errors
            {
                // Log the exception (recommended)
                Console.WriteLine($"Error deserializing answers: {ex.Message}");
                // Or throw the exception if you want it to propagate
                // throw;

                return null; // Or return an empty list: return new List<string>(); if that's more appropriate for your use case
            }
        }

    }

}
