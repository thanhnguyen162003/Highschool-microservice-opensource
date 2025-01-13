using Application.Common.Helper;
using Application.Common.Models.UserModels;
using Application.Features.KetFeatures.Command;
using Application.Features.UserFeatures.Command;
using Application.Services.Authentication;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using Domain.Models.Common;
using Domain.Models.KetModels;
using Domain.Models.PlayGameModels;
using Domain.Models.UserModels;
using Newtonsoft.Json;

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
            CreateMap<CreateKetCommand, Ket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom<AuthenticatedUserIdResolver>())
                .ForMember(dest => dest.KetContents, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateKetCommand, Ket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.KetContents, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<KetContentRequestModel, KetContent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.KetId, opt => opt.MapFrom((src, dest, _, context) => context.Items["KetId"]))
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Answers)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Ket, KetResponseModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PagedList<Ket>, PagedList<KetResponseModel>>();

            CreateMap<KetContent, QuestionGame>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<IEnumerable<string>>(src.Answers!)));

            CreateMap<CreateAvatarComand, Avatar>()
                .ForMember(dest => dest.Rarity, opt => opt.MapFrom(src => src.Rarity!.ConvertToValue<AvatarRarity>()!.Value))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<ResultPlayerModel, PlayerGame>()
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom((src, dest, _, context) => context.Items["RoomId"]));

            CreateMap<User, AuthorResponseModel>();

            CreateMap<PlayerGame, HistoryPlay>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserDetailModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class AuthenticatedUserIdResolver : IValueResolver<CreateKetCommand, Ket, Guid>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticatedUserIdResolver(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public Guid Resolve(CreateKetCommand source, Ket destination, Guid member, ResolutionContext context)
        {
            return _authenticationService.GetUserId();
        }
    }
}
