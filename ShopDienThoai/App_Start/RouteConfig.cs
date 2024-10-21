using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopDienThoai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UserDetail",
                url: "thong-tin-tai-khoan/{id}",
                defaults: new { controller = "Home", action = "UserDetail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductSearch",
                url: "san-pham/tim-kiem/{id}",
                defaults: new { controller = "Home", action = "ProductSearch", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "OrderUser",
                url: "don-hang/{id}",
                defaults: new { controller = "Home", action = "ViewOrders", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Checkout",
                url: "dat-hang/{id}",
                defaults: new { controller = "Home", action = "Checkout", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ViewCart",
                url: "gio-hang/{id}",
                defaults: new { controller = "Home", action = "ViewCart", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductDetail",
                url: "dien-thoai/{id}",
                defaults: new { controller = "Home", action = "ProductDetail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductFollowCategory",
                url: "hang-dien-thoai/{id}",
                defaults: new { controller = "Home", action = "ProductFollowCategory", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SupplierEdit",
                url: "admin/supplier/edit/{id}",
                defaults: new { controller = "Admin", action = "SupplierEdit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SupplierCreate",
                url: "admin/supplier/add/{id}",
                defaults: new { controller = "Admin", action = "SupplierCreate", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "ManufacturerEdit",
                url: "admin/manufacturer/edit/{id}",
                defaults: new { controller = "Admin", action = "ManufacturerEdit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ManufacturerCreate",
                url: "admin/manufacturer/add/{id}",
                defaults: new { controller = "Admin", action = "ManufacturerCreate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AccountEdit",
                url: "admin/account/edit/{id}",
                defaults: new { controller = "Admin", action = "AccountEdit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AccountCreate",
                url: "admin/account/add/{id}",
                defaults: new { controller = "Admin", action = "AccountCreate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ClientEdit",
                url: "admin/client/edit/{id}",
                defaults: new { controller = "Admin", action = "ClientEdit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ClientCreate",
                url: "admin/client/add/{id}",
                defaults: new { controller = "Admin", action = "ClientCreate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductEdit",
                url: "admin/product/edit/{id}",
                defaults: new { controller = "Admin", action = "ProductEdit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProductCreate",
                url: "admin/product/add/{id}",
                defaults: new { controller = "Admin", action = "ProductCreate", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Dashboard",
                url: "admin/dashboard/{id}",
                defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
