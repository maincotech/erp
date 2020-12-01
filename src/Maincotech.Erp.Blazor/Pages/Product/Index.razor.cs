using AntDesign;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Maincotech.Erp.Pages.Product
{
    public partial class Index
    {
        [Inject] public IndexViewModel IndexViewModel { get => ViewModel; set => ViewModel = value; }

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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await ViewModel.LoadCategories();
            Search();
        }

        private void Search()
        {
            IsLoading = true;
            ViewModel.Load.Execute().Subscribe(items => { },
                ex =>
                {
                    Console.WriteLine(ex);
                    IsLoading = false;
                },
                () =>
                {
                    IsLoading = false;
                });
        }

        private readonly ListGridType _listGridType = new ListGridType
        {
            Gutter = 24,
            Column = 4
        };

        private void Create()
        {
            var targetPath = Options.AreaName.IsNullOrEmpty() ? "/products/Edit" : $"/{Options.AreaName}/products/Edit";
            NavigationManager.NavigateTo(targetPath);
        }

        private string GetEditLink(string id)
        {
            return Options.AreaName.IsNullOrEmpty() ? $"/products/edit/{id}" : $"/{Options.AreaName}/products/Edit/{id}";
        }
    }
}