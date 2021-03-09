using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Domain
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<ClientApplicationJson, ClientApplication>();
            CreateMap<ClientApplication, ClientApplicationJson>()
                .ForMember(destination => destination.Configuration, options => options.MapFrom(source => source.Configuration.ResolveJson()));
        }
    }

    public static class MapperExtensions
    {
        public static JsonElement ResolveJson(this string json)
        {
            return JsonSerializer.Deserialize<JsonElement>(json);
        }
    }
}
