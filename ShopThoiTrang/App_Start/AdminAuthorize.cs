﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        public int idChucNang { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            NHANVIEN nvSession = (NHANVIEN)HttpContext.Current.Session["user"];

            if(nvSession != null)
            {
                // kiểm tra quyền
                QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
                var count = db.PhanQuyen.Count(m => m.MaNV == nvSession.MaNV & m.MaChucNang == idChucNang);// kiểm tra chức năng nè
                if (count != 0)
                {
                    // đúng quyền (ROLE) thì nó đc vào
                    return;
                }
                else
                {   
                    // chuyển về trang lõi bạn ko có quyền này
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(
                        new
                        {
                            Controller = "Home",
                            action = "CanNotAccess",
                            area = "Admin",
                            returnUrl = returnUrl.ToString()
                        }));
                }

                return;
            }    
            else
            {
                // chưa đăng nhập thì nó bắt mình đăng nhập
                var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(
                    new
                    {

                        Controller = "Home",
                        action = "DangNhap",
                        area = "Admin",
                        returnUrl = returnUrl.ToString()
                    }));
            }

        }
    }
}