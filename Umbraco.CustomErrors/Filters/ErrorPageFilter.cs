using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.PublishedContentModels;

namespace Umbraco.CustomErrors.Filters
{
    public class ErrorPageFilter : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var content = ((filterContext.Result as ViewResultBase)?.Model as RenderModel)?.Content;
            if (content?.DocumentTypeAlias == ErrorPage.ModelTypeAlias)
            {
                if (int.TryParse(content.Name, out var statusCode))
                {
                    statusCode = 500; //something is definitely wrong
                }

                filterContext.HttpContext.Response.StatusCode = statusCode;
            }
        }
    }
}