using AutoMapper;
using Bie.Api.DTOs.Request;
using Bie.Api.DTOs.Response;
using Bie.Business.Models;

namespace Bie.Api.Configuration.Mappings;
public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<CompanyRequestDto, Company>();
        CreateMap<Company, CompanyResponseDto>();

        CreateMap<Company, SchedulingRequestDto>()
        .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now));
    }
}