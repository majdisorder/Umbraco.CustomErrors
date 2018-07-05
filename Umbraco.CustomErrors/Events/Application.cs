using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.CustomErrors.Filters;

namespace Umbraco.CustomErrors.Events
{
    public class Application : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RegisterFilters();

        }
       
        private void RegisterFilters()
        {
            GlobalFilters.Filters.Add(new ErrorPageFilter());
        }


    }
}