﻿using DynamicData;
using Maincotech.Cms.Dto;
using Maincotech.Data;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Maincotech.Erp.Pages.Product
{
    public class IndexViewModel : ReactiveObject
    {
        private static Maincotech.Logging.ILogger _Logger = AppRuntimeContext.Current.GetLogger<IndexViewModel>();

        private readonly Maincotech.Erp.Services.IAdminService _dataAdminService;
        private readonly ObservableAsPropertyHelper<IEnumerable<ProductViewModel>> _items;
        public IEnumerable<ProductViewModel> Items => _items.Value;

        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        public bool IsLoading => _isLoading.Value;

        public ReactiveCommand<Unit, IEnumerable<ProductViewModel>> Load { get; }


        public string SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Total { get; set; }
        public List<string> SelectedCategories { get; set; } = new List<string>();

        public IndexViewModel()
        {
            _dataAdminService = AppRuntimeContext.Current.Resolve<Maincotech.Erp.Services.IAdminService>();
            Load = ReactiveCommand.CreateFromTask(LoadAll);
            _items = Load.ToProperty(this, x => x.Items, scheduler: RxApp.MainThreadScheduler);
            Load.ThrownExceptions.Subscribe(exception =>
            {
                _Logger.Error("Unexpected error occurred.", exception);
            });

            this.WhenAnyObservable(x => x.Load.IsExecuting).ToProperty(this, x => x.IsLoading, out _isLoading);
        }

       

        private async Task<IEnumerable<ProductViewModel>> LoadAll()
        {
            var result = new List<ProductViewModel>();

            var filterRules = new FilterCondition();

            if (SelectedCategories.IsNotNullOrEmpty())
            {
                var filterCategories = new FilterGroup();
                foreach (var category in SelectedCategories)
                {
                    filterCategories.Add(new FilterRule
                    {
                        Field = "CategoryId",
                        FilterOperator = FilterOperator.Equal,
                        PropertyValues = new List<object> { Guid.Parse(category) },
                        LogicalOperator = LogicalOperator.Or
                    });
                }

                filterRules.Add(filterCategories);
            }

            if (SearchText.IsNotNullOrEmpty())
            {
                var filterGroup = new FilterGroup()
                {
                    LogicalOperator = LogicalOperator.And
                };
                filterGroup.Add(new FilterRule("Name", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterGroup.Add(new FilterRule("Summary", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterGroup.Add(new FilterRule("MarkdownDescription", FilterOperator.Contains, LogicalOperator.Or, SearchText));
                filterRules.Add(filterGroup);
            }

            var pageQuery = new PagingQuery
            {
                Pagination = new Pagination
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                },
                FilterCondition = filterRules
            };
            var entities = await _dataAdminService.GetProducts(pageQuery);
            Total = entities.TotalRecords;
            if (entities.Count > 0)
            {
                var viewModels = AppRuntimeContext.Current.Adapt<List<ProductViewModel>>(entities);
                result.AddRange(viewModels);
            }
            //  await Task.Delay(1000 * 3);
            return result;
        }

        public ObservableCollection<CategoryDto> Categories { get; set; } = new ObservableCollection<CategoryDto>();

        private bool _IsLoadingCategories;

        public bool IsLoadingCategories
        {
            get => _IsLoadingCategories;
            set => this.RaiseAndSetIfChanged(ref _IsLoadingCategories, value);
        }

        public async Task LoadCategories()
        {
            try
            {
                IsLoadingCategories = true;
                Categories.Clear();
                var categories = await _dataAdminService.GetProductCategories();
                Categories.AddRange(categories);
            }
            finally
            {
                IsLoadingCategories = false;
            }
        }
    }
}