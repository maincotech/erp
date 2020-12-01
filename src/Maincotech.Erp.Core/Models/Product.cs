using Maincotech.Cms.Models;
using Maincotech.Domain;
using Maincotech.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Maincotech.Erp.Models
{
    public class Product : EntityBase
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }        
        public bool IsRecommended { get; set; }
        public virtual Category Category { get; set; }
        public string HtmlDescription { get; set; }
        public string MarkdownDescription { get; set; }
        public string Summary { get; set; }
    }

    public class ProductMappingConfiguration : EntityTypeConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.Summary).HasMaxLength(500);
            builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
            base.Configure(builder);
        }
    }
}