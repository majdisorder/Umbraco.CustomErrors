using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Umbraco.CustomErrors.Controllers
{
    public class ErrorController : SurfaceController
    {
        [HttpGet]
        public ActionResult GiveMe500()
        {
            throw new Exception("There you go!");
        }

        [HttpGet]
        public ActionResult GiveMe400()
        {
            return new HttpStatusCodeResult(400, "Bad request.");
        }
    }
}