using Maincotech.Localization;
using System.Globalization;
using System.Reflection;

namespace Maincotech.Erp
{
    internal class ErpLocalizer : AssemblyBasedLocalizer
    {
        // public override string ResourcesPatten => @"^.*wwwroot\i18n\.(.+)\{0}.json";

        public ErpLocalizer(CultureInfo cultureInfo) : base(Assembly.GetExecutingAssembly(), cultureInfo)
        {
        }
    }
}