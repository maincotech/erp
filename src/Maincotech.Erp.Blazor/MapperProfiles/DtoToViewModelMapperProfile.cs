using AntDesign;
using AutoMapper;
using Maincotech.Adapter;
using Maincotech.Data;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Pages.Product;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maincotech.Erp.Blazor.MapperProfiles
{
    public class DtoToViewModelMapperProfile : Profile, IOrderedMapperProfile
    {
        public int Order => 2;

        public DtoToViewModelMapperProfile()
        {
            CreateMap<ProductImageDto, UploadFileItem>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom((src, dest, destMember, context) => src.Name))
                .ForMember(dest => dest.State, opt => opt.MapFrom((src, dest, destMember, context) => UploadState.Success));
                //.ForMember(dest => dest.Url, opt => opt.MapFrom((src, dest, destMember, context) => src.Url))
            CreateMap<ProductDto, ProductViewModel>()
              .ForMember(dest => dest.CurrentTags, opt => opt.MapFrom((src, dest, destMember, context) => src.Tags.IsNotNullOrEmpty() ? src.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>()));
        }
    }
}