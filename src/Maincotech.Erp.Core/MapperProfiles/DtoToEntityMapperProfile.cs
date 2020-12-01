using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Cms.Models;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Models;

namespace Maincotech.Cms.MapperProfiles
{
    internal class DtoToEntityMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 2;

        public DtoToEntityMapperProfile()
        {
            CreateMap<ProductDto, Product>();
            CreateMap<ProductImageDto, Attachment>()
                .ForMember(dest => dest.RbsInfo, opt => opt.MapFrom((src, dest, destMember, context) => src.Url));
        }
    }
}