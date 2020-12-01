using AntDesign;
using Maincotech.Web.Components;
using Maincotech.Web.Components.Vditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maincotech.Erp.Pages.Product
{
    [Authorize(Policy = "Admin")]
    public partial class Edit
    {
        [Parameter] public string Id { get; set; }
        [Inject] protected IJSRuntime Js { get; set; }
        [Inject] protected MessageService MessageService { get; set; }

        private Dictionary<string, object> EditorOptions => new Dictionary<string, object>
    {
        {"upload", new UploadOptions{ Url = Options.VditorUpload } }
    };

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            IsLoading = true;
            if (Id.IsNotNullOrEmpty())
            {
                ViewModel = new ProductViewModel()
                {
                    Id = Id
                };
            }
            else
            {
                var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                ViewModel = new ProductViewModel()
                {
                };
                //
            }
            IsLoading = true;
            ViewModel.Load.Execute().Subscribe(
                (unit) => { },
                (ex) =>
                {
                    Console.WriteLine(ex);
                    IsLoading = false;
                },
                () =>
                {
                    IsLoading = false;
                });
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        private async ValueTask ExportAsync()
        {
            if (!string.IsNullOrEmpty(ViewModel.MarkdownDescription))
            {
                var fileName = $"{ViewModel.Name ?? "Product"}.md";
                await MaincotechJsInterop.SaveFileAsync(Js, fileName, "text/plain", System.Text.Encoding.UTF8.GetBytes(ViewModel.MarkdownDescription));
                await MessageService.Success("The file has been exported.");
            }
            else
            {
                await MessageService.Info("Nothing to be exported.");
            }
        }

        private async ValueTask OnFinish(EditContext editContext)
        {
            await ViewModel.Save();
            var indexPagePath = Options.AreaName.IsNullOrEmpty() ? "/products" : $"/{Options.AreaName}/products";
            NavigationManager.NavigateTo(indexPagePath, forceLoad: true);
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //   Console.WriteLine($"Failed:{JsonSerializer.Serialize(model)}");
        }

        private void Cancel()
        {
            var indexPagePath = Options.AreaName.IsNullOrEmpty() ? "/products" : $"/{Options.AreaName}/products";
            NavigationManager.NavigateTo(indexPagePath);
        }

        private bool previewVisible = false;
        private string previewTitle = string.Empty;
        private string imgUrl = string.Empty;

        private bool BeforeUpload(UploadFileItem file)
        {
            var isJpgOrPng = file.Type == "image/jpeg" || file.Type == "image/png";
            if (!isJpgOrPng)
            {
                MessageService.Error("You can only upload JPG/PNG file!");
            }
            var isLt2M = file.Size / 1024 / 1024 < 2;
            if (!isLt2M)
            {
                MessageService.Error("Image must smaller than 2MB!");
            }
            return isJpgOrPng && isLt2M;
        }

        private bool _IsUploadingIcon;

        public bool IsUploadingIcon
        {
            get => _IsUploadingIcon;
            set
            {
                if (_IsUploadingIcon != value)
                {
                    _IsUploadingIcon = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        private void HandleChange(UploadInfo fileinfo)
        {
            IsUploadingIcon = fileinfo.File.State == UploadState.Uploading;

            if (fileinfo.File.State == UploadState.Success)
            {
                var uploadResult = fileinfo.File.GetResponse<Maincotech.Web.Components.JQuery.UploadResult>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                fileinfo.File.Url = uploadResult.Url;
            }
            InvokeAsync(StateHasChanged);
        }

        private async Task<bool> HandleRemove(UploadFileItem file)
        {
            //   await _message.Loading($"removing {file.FileName}", 2);
            return await Task.FromResult(true);
        }
    }
}