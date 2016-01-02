using AirBears.Web.Models;
using AirBears.Web.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirBears.Web
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.TeeShirtSize, mapper => mapper.MapFrom(src => src.TeeShirtSize.Name));

            Mapper.AssertConfigurationIsValid();
        }
    }
}
