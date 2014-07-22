using HW1.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW1.Controllers
{
    [IdFilter]
    public class BaseController : Controller
    {
        /// <summary>
        /// * 寫一個 BaseController 並覆寫 HandleUnknownAction 方法，找不到 Action 就顯示一頁自訂的 404 錯誤頁面。
        /// </summary>
        /// <param name="actionName"></param>
        protected override void HandleUnknownAction(string actionName)
        {

            Response.Redirect("/Home/Error404");
        }
	}
}