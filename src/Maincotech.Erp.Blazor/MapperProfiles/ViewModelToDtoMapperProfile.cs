using AntDesign;
using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Pages.Product;

namespace Maincotech.Erp.Blazor.MapperProfiles
{
    public class ViewModelToDtoMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 3;

        public ViewModelToDtoMapperProfile()
        {
            CreateMap<ProductViewModel, ProductDto>();

            CreateMap<UploadFileItem, ProductImageDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom((src, dest, destMember, context) => src.FileName));
        }
    }
}