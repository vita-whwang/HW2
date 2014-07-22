using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HW1.ActionFilter
{
    public class IdFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// * 對於從網址列上出現的 id 值，撰寫一個自訂的 Action Filter (動作過濾器) 檢查傳入的 id 格式是否符合要求，格式不對就導向回首頁。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string id = filterContext.HttpContext.Request.QueryString["id"];
            if (!String.IsNullOrEmpty(id))
            {
                if (Regex.Match(id,@"[\D]+").Success)
                {
                    filterContext.Result = new RedirectToRouteResult(
                                                new RouteValueDictionary 
                                                { 
                                                    { "controller", "Home" }, 
                                                    { "action", "Index" } 
                                                });
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}