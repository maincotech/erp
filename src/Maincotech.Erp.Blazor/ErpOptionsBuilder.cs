using Maincotech.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Maincotech.Erp
{
    public class ErpOptionsBuilder
    {
        public ErpOptionsBuilder()
        {
            Options = new ErpOptions
            {
                SupportLanguages = new List<string> { "en-US", "ja-JP", "zh-CN" },
                JqueryUpload = "/api/files/jqueryUpload",
                VditorUpload = "/api/files/vditorUpload",
            };
        }

        public ErpOptions Options { get; }

        public ErpOptionsBuilder UseLayout(Type layoutType)
        {
            Options.Layout = layoutType;
            return this;
        }

        public ErpOptionsBuilder UseUploadAPI(string jqueryUpload, string vditorUpload)
        {
            Options.JqueryUpload = jqueryUpload;
            Options.VditorUpload = vditorUpload;
            return this;
        }

        public ErpOptionsBuilder RegisterCmsLocalizer(IServiceProvider serviceProvider)
        {
            var localizer = serviceProvider.GetRequiredService<ILocalizer>();
            localizer.AddAdditionalLocalizer(new ErpLocalizer(localizer.CurrentCulture));
            return this;
        }

        public ErpOptionsBuilder SetSupportLanguages(List<string> SupportLanguages)
        {
            Options.SupportLanguages = SupportLanguages;
            return this;
        }

        public ErpOptionsBuilder UseAreaName(IServiceProvider serviceProvider, string areaName)
        {
            Options.AreaName = areaName;
            return this;
        }
    }
}