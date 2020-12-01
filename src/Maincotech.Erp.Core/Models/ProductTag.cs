using Maincotech.Cms.Models;
using Maincotech.Domain;
using Maincotech.EF;

namespace Maincotech.Erp.Models
{
    public class ProductTag : AssociationBase<Product, Tag>
    {
        public ProductTag()
        {
            Relation = "HasZeroToMany";
        }
    }

    public class ProductTagMappingConfiguration : AssociationMappingConfiguration<ProductTag, Product, Tag>
    {
    }
}