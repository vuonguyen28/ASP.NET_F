using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;
using ShopThoiTrang.App_Start;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Runtime.Remoting.Contexts;


namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class BuyerController : Controller
    {
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // GET: Admin/Buyer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowBuyer(string searching)
        {
            var buyers = db.KHACHHANG.Where(x => x.TenKH.Contains(searching) || x.Email.Contains(searching) || x.SoDienThoai.Contains(searching) || searching == null).ToList();
            return View(buyers);
        }



        [HttpPost]
        public ActionResult Update_TrangThai(int makh)
        {
            KHACHHANG kh = db.KHACHHANG.SingleOrDefault(m => m.MaKH == makh);
            if (kh != null)
            {
                if (kh.TrangThai == "Được Dùng")
                {
                    kh.TrangThai = "Chặn";
                }
                else
                {
                    kh.TrangThai = "Được Dùng";
                }
                db.SaveChanges();

                // Prepare JSON response indicating success and the updated TrangThai value
                var responseData = new
                {
                    success = true,
                    newTrangThai = kh.TrangThai
                };
                return Json(responseData, JsonRequestBehavior.AllowGet);
            }

            // In case the KHACHHANG with the given ID is not found
            var errorResponse = new
            {
                success = false,
                message = "Không tìm thấy khách hàng."
            };
            return Json(errorResponse, JsonRequestBehavior.AllowGet);
        }

    }
}