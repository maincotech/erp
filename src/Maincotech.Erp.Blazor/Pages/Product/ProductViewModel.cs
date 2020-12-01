using AntDesign;
using Maincotech.Cms;
using Maincotech.Erp.Dto;
using Maincotech.Erp.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Maincotech.Erp.Pages.Product
{
    public class ProductViewModel : ReactiveObject
    {
        private static Logging.ILogger _Logger = AppRuntimeContext.Current.GetLogger<ProductViewModel>();
        private readonly IAdminService _dataAdminService;
        private readonly Cms.Services.IAdminService _cmsAdminService;

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Summary { get; set; }

        public string HtmlDescription { get; set; }

        public string MarkdownDescription { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public string Tags { get; set; }

        public bool IsRecommended { get; set; }
        public List<CascaderNode> Categories { get; set; } = new List<CascaderNode>();
        public List<string> AllTags { get; set; } = new List<string>();
        public string[] CurrentTags { get; set; } = Array.Empty<string>();

        public List<UploadFileItem> Images { get; set; } = new List<UploadFileItem>();

        public bool IsLoaded { get; private set; }

        public ReactiveCommand<Unit, Unit> Load { get; }

        public ProductViewModel()
        {
            _dataAdminService = AppRuntimeContext.Current.Resolve<Maincotech.Erp.Services.IAdminService>();
            _cmsAdminService = AppRuntimeContext.Current.Resolve<Cms.Services.IAdminService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
        }

        public async Task LoadAll()
        {
            //Load Categories
            var allCategories = (await _cmsAdminService.GetCategories()).ToList();
            Categories = TypeConverters.Convert(allCategories, Guid.Empty);
            //Load Tags
            AllTags = (await _cmsAdminService.GetPublicTags()).ToList();

            //Load Self
            if (Id.IsNotNullOrEmpty())
            {
                var entity = await _dataAdminService.GetProduct(Guid.Parse(Id));
                this.MergeDataFrom(entity.To<ProductViewModel>(), new List<string> { "Categories", "AllTags", "IsLoaded" });
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }

            IsLoaded = true;
        }

        public async Task Save()
        {
            var dto = this.To<ProductDto>();
            await _dataAdminService.CreateOrUpdateProduct(dto);
        }

        public string CoverUrl
        {
            get
            {
                var coverImage = Images?.FirstOrDefault();
                return coverImage == null ? "_content/Maincotech.Erp.Blazor/noimage.png" : coverImage.Url;
            }
        }
    }
}