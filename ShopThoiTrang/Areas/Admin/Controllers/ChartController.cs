using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;
using System.Data.SqlClient;
using System.Net;
using System.Data.Entity;
using System.IO; //để dùng BinaryReader

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class ChartController : Controller
    {
        // GET: Admin/Chart
        public ActionResult ChartData()
        {
            // Lấy dữ liệu từ cơ sở dữ liệu hoặc một nguồn dữ liệu khác
            var labels = new List<string> { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6" };
            var data = new List<int> { 100, 200, 150, 300, 270, 350 };

            return Json(new { labels = labels, data = data }, JsonRequestBehavior.AllowGet);
        }

    }
}