using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using SuingInsurance.Domain;

namespace SuingInsurance.Api.Webrole
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        internal ServiceLocatorBase ServiceLocator { get; private set; }

        public override void Init()
        {
            base.Init();
            this.ServiceLocator = new ServiceLocatorProduction();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
