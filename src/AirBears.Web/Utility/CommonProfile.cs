using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;

namespace AirBears.Web.Profiles
{
    public class CommonProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Post, PostViewModel>().ReverseMap();

            CreateMap<User, UserViewModel>();

            CreateMap<User, PilotViewModel>();

            CreateMap<User, IdentityViewModel>();

            CreateMap<User, IdentityPilotViewModel>();

            CreateMap<User, PilotSearchResultViewModel>()
                .ForMember(dest => dest.Distance, mapper => mapper.Ignore());

            CreateMap<PilotRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAuthorityAccount, mapper => mapper.UseValue(false))
                .IgnoreAllNonExisting();

            CreateMap<AuthorityRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAuthorityAccount, mapper => mapper.UseValue(true))
                .IgnoreAllNonExisting();

            CreateMap<PilotViewModel, User>()
                //Map all foreign keys
                .ForMember(dest => dest.StateId, mapper => mapper.MapFrom(s => s.State.Id))
                .ForMember(dest => dest.FlightTimeId, mapper => mapper.MapFrom(s => s.FlightTime.Id))
                .ForMember(dest => dest.PayloadId, mapper => mapper.MapFrom(s => s.Payload.Id))
                .ForMember(dest => dest.TeeShirtSizeId, mapper => mapper.MapFrom(s => s.TeeShirtSize.Id))
                //Remove virtual properties
                .ForMember(dest => dest.State, mapper => mapper.UseValue(null))
                .ForMember(dest => dest.FlightTime, mapper => mapper.UseValue(null))
                .ForMember(dest => dest.Payload, mapper => mapper.UseValue(null))
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.UseValue(null))
                .IgnoreAllNonExisting();
        }
    }
}
