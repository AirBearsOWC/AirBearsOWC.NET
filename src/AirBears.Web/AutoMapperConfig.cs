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
            Mapper.CreateMap<User, UserViewModel>();

            Mapper.CreateMap<User, PilotViewModel>();

            Mapper.CreateMap<User, IdentityViewModel>();

            Mapper.CreateMap<User, IdentityPilotViewModel>();

            Mapper.CreateMap<User, PilotSearchResultViewModel>()
                .ForMember(dest => dest.Distance, mapper => mapper.Ignore());

            Mapper.CreateMap<PilotRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAuthorityAccount, mapper => mapper.UseValue(false))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<AuthorityRegistrationViewModel, User>()
                .ForMember(dest => dest.UserName, mapper => mapper.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsAuthorityAccount, mapper => mapper.UseValue(true))
                .IgnoreAllNonExisting();

            Mapper.CreateMap<PilotViewModel, User>()
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

            Mapper.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Ignores fields that do not exist in the destination type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
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
