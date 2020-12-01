using Maincotech.Cms.Dto;
using Maincotech.Data;
using Maincotech.Erp.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maincotech.Erp.Services
{
    public interface IAdminService
    {
        #region Product Operations

        Task CreateOrUpdateProduct(ProductDto dto);

        Task<ProductDto> GetProduct(Guid id);

        Task<PagedResult<ProductDto>> GetProducts(PagingQuery pagingQuery);

        Task AddProductImage(ProductImageDto dto);


        Task<IEnumerable<CategoryDto>> GetProductCategories();

        #endregion Product Operations

        
    }
}