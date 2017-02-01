using System.Web;
using System.Web.Mvc;

namespace SuingInsurance.Api.Webrole
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
