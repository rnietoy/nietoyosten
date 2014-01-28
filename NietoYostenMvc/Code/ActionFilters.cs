using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NietoYostenMvc.Controllers;

namespace NietoYostenMvc.Code
{
    public class RequireRole : ActionFilterAttribute
    {
        public string Role { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (ApplicationController) filterContext.Controller;

            if (!controller.IsLoggedIn)
            {
                controller.TempData["ReturnUrl"] = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult("~/account/login");
            }
            else if (!controller.CurrentUserHasRole(Role))
            {
                controller.TempData["AlertMessage"] = "Este usuario no tiene accesso a esta sección.";
                controller.TempData["AlertClass"] = "alert-danger";
                controller.TempData["ReturnUrl"] = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult("~/account/login");
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class RequireLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (ApplicationController)filterContext.Controller;

            if (!controller.IsLoggedIn)
            {
                controller.TempData["ReturnUrl"] = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult("~/account/login");
            }

            base.OnActionExecuting(filterContext);
        }
    }

}