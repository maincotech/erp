using Maincotech.Domain.Specifications;
using Maincotech.Erp.Models;
using System;

namespace Maincotech.Erp
{
    internal static class ErpSpecifications
    {
        public static ISpecification<ProductTag> TagsWithProductId(Guid id)
        {
            return Specification<ProductTag>.Eval(entity => entity.SourceId == id);
        }

        public static ISpecification<ProductImage> ImageWithProductId(Guid id)
        {
            return Specification<ProductImage>.Eval(entity => entity.SourceId == id);
        }
    }
}