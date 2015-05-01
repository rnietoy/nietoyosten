using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using NietoYostenMvc.Controllers;

namespace NietoYostenMvc.Code
{
    public class HandleUserFriendlyErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Handling the UserFriendlyException so we can set the AlertMessage and AlertDanger
        /// values in the TempData dictionary. These are can then read by the view
        /// to show an error message to the user.
        /// 
        /// Some of this code is taken from:
        /// http://stackoverflow.com/questions/1794936/how-do-i-pass-viewdata-to-a-handleerror-view
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            UserFriendlyException userFriendlyException = filterContext.Exception as UserFriendlyException;
            if (userFriendlyException != null)
            {

                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    TempDataDictionary tempData = filterContext.Controller.TempData;
                    tempData["AlertMessage"] = userFriendlyException.Message;
                    tempData["AlertClass"] = "alert-danger";

                    filterContext.Result = new ViewResult
                    {
                        ViewName = this.View,
                        MasterName = this.Master,
                        ViewData = filterContext.Controller.ViewData,
                        TempData = filterContext.Controller.TempData
                    };
                    filterContext.ExceptionHandled = true;
                }
                else
                {
                    ErrorController errorController = new ErrorController {ControllerContext = filterContext};
                    ActionResult result = errorController.GetUserFriendlyJsonError(userFriendlyException);
                    result.ExecuteResult(filterContext);
                    filterContext.ExceptionHandled = true;
                }
            }
        }
    }
}