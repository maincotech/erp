using Maincotech.Cms.Models;
using Maincotech.EF;
using Maincotech.Erp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Maincotech.Erp
{
    public class ErpDbContext : BoundedDbContext
    {
        public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options)
        {
        }

        protected override IEnumerable<IMappingConfiguration> GetConfigurations()
        {
            var configurations = new List<IMappingConfiguration>
            {
                new TagMappingConfiguration(),
                new AttachmentMappingConfiguration(),
                new CategoryMappingConfiguration(),
                new ProductMappingConfiguration(),
                new ProductTagMappingConfiguration(),
                new ProductImageMappingConfiguration()
            };
            return configurations;
        }

        public override void EnsureCreated()
        {
            base.EnsureCreated(); //Make sure database is created.

            //Make sure table is created.
            this.EnsureTables();
        }

        public override void EnsureDeleted()
        {
            //delete tables
            this.DropTables();
            base.EnsureDeleted();
        }
    }
}