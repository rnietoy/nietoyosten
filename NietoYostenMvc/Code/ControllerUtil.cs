using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NietoYostenMvc.Code
{
    public static class ControllerUtil
    {
        public static int GetCurrentPage(this Controller controller)
        {
            int page;

            // If TryParse fails, it sets page to 0, so we set it to 1 (since there is no zeroth page)
            if (!int.TryParse(controller.Request.QueryString["page"], out page))
            {
                page = 1;
            }
            return page;
        }
    }
}