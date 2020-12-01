using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Cms.Models;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Models;

namespace Maincotech.Cms.MapperProfiles
{
    internal class EntityToDtoMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 2;

        public EntityToDtoMapperProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<Attachment, ProductImageDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom((src, dest, destMember, context) => src.RbsInfo));
        }
    }
}