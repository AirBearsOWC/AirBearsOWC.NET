using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;
using System.Linq;

namespace AirBears.Web
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.MapFrom(src => src.TeeShirtSize.Name));

            Mapper.CreateMap<User, IdentityViewModel>()
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.MapFrom(src => src.TeeShirtSize.Name));

            Mapper.CreateMap<User, PilotSearchResultViewModel>()
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.MapFrom(src => src.TeeShirtSize.Name))
                .ForMember(dest => dest.Distance, mapper => mapper.Ignore());

            Mapper.CreateMap<PilotRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<AuthorityRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAuthorityAccount, mapper => mapper.UseValue(true))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<UpdatePilotViewModel, User>()
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.Ignore())
                .IgnoreAllNonExisting();

            Mapper.AssertConfigurationIsValid();
        }

        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType) && x.DestinationType.Equals(destinationType));

            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }

            return expression;
        }
    }
}
