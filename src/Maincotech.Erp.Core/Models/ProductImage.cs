using Maincotech.Cms.Models;
using Maincotech.Domain;
using Maincotech.EF;

namespace Maincotech.Erp.Models
{
    public class ProductImage : AssociationBase<Product, Attachment>
    {
        public ProductImage()
        {
            Relation = "HasZeroToMany";
        }
    }

    public class ProductImageMappingConfiguration : AssociationMappingConfiguration<ProductImage, Product, Attachment>
    {
    }
}