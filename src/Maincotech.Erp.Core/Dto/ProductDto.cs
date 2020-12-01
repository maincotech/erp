using System;
using System.Collections.Generic;

namespace Maincotech.Erp.Dto
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string HtmlDescription { get; set; }

        public string MarkdownDescription { get; set; }

        public Guid CategoryId { get; set; }

        public string Tags { get; set; }

        public bool IsRecommended { get; set; }

        public List<ProductImageDto> Images { get; set; }
    }
}