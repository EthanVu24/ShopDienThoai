using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopDienThoai.App_Start
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class isAdmin : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var user = httpContext.Session["user"] as ShopDienThoai.Models.tai_khoan;

            if (user == null)
                return false;

            int roleUser = user.id_vai_tro;

            if (roleUser == 3)
                return false;

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new
                System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" }));
        }
    }
}