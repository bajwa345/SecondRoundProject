using AutoMapper;
using SecondRoundProject.DTOs;
using SecondRoundProject.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SecondRoundProject.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Client, ClientDTO>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDTO
            {
                Country = src.Country,
                City = src.City,
                Street = src.Street,
                ZipCode = src.ZipCode
            }))
            .ReverseMap()
            .ForPath(src => src.Country, opt => opt.MapFrom(dest => dest.Address.Country))
            .ForPath(src => src.City, opt => opt.MapFrom(dest => dest.Address.City))
            .ForPath(src => src.Street, opt => opt.MapFrom(dest => dest.Address.Street))
            .ForPath(src => src.ZipCode, opt => opt.MapFrom(dest => dest.Address.ZipCode));

            CreateMap<ClientAccount, ClientAccountDTO>().ReverseMap();
        }
    }
}
