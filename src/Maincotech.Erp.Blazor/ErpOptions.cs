using Maincotech.Localization;
using System;
using System.Collections.Generic;

namespace Maincotech.Erp
{
    public class ErpOptions
    {
        public string JqueryUpload { get; set; }
        public string VditorUpload { get; set; }
        public List<string> SupportLanguages { get; set; }
        public ILocalizer AdditionalLocalizer { get; internal set; }

        public string AreaName { get; set; }
        public Type Layout { get; set; }
    }
}