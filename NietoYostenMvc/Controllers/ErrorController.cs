using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NietoYostenMvc.Code;

namespace NietoYostenMvc.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult GetUserFriendlyJsonError(UserFriendlyException ex)
        {
            return Json(ex.Message);
        }
    }
}
