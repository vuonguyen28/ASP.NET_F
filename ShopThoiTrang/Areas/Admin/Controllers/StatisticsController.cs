using ShopThoiTrang.App_Start;
using ShopThoiTrang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class StatisticsController : Controller
    {

        //31	XemThongKe
        // GET: Admin/Statistics
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Customer()
        {
            QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
            int a = db.KHACHHANG.Count();
            return View(a);
        }
        public ActionResult Invoice()
        {
            QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
            int a = db.DonHang.Count(m => m.TrangThaiDonHang == "Đã Giao");
            return View(a);
        }
        public ActionResult Sales()
        {
            double a = 0;
            using (QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities())
            {
                a = db.DonHang.Where(m => m.TongGia != null && m.TongGia != 0).Sum(m => 1.0 * m.TongGia ?? 0);
            }
            return View(a);
        }


        [AdminAuthorize(idChucNang = 31)]

        public ActionResult ThongKeDoanhThuTheoNgay(DateTime? startTime, DateTime? endTime, int? t = 0)
        {
            if (startTime == DateTime.MinValue || endTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
                endTime = DateTime.Now;
            }
            var chart_doanhthu = db.ThongKeDoanhThuTheoNgay(
                startTime,
                endTime,
                "Đã Giao"
            ).ToList();
            if (t == 0)
            {
                t = db.SanPham.Count();
            }
            var chart_tksp = db.ThongKeTopSanPhamBanNhieuNhat(
                startTime,
                endTime,
                t,
                "Đã Giao"
            ).ToList();
            ViewBag.chart_tksp_tong = chart_tksp.Sum(m => m.TongSoLuongBan);
            DateTime startDate_m = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate_m = startDate_m.AddMonths(1).AddDays(-1);
            var chart_tksp_m = db.ThongKeDoanhThuTheoNgay(
                startDate_m,
                endDate_m,
                "Đã Giao"
            ).ToList();
            ViewBag.chart_tksp = chart_tksp;
            ViewBag.chart_tksp_m = chart_tksp_m;
            float tong = chart_doanhthu.Sum(m => (float)m.TongDoanhThu);
            ViewBag.tong = tong;
            ViewBag.chart_doanhthu = chart_doanhthu;
            return View();
        }

        [AdminAuthorize(idChucNang = 31)]

        public ActionResult ThongKeDoanhThuTheoThang(int? startMonth, int? startYear, int? endMonth, int? endYear, int? t = 0)
        {
            if (!startMonth.HasValue || !startYear.HasValue || !endMonth.HasValue || !endYear.HasValue)
            {
                startMonth = DateTime.Now.Month;
                startYear = DateTime.Now.Year;
                endMonth = DateTime.Now.Month;
                endYear = DateTime.Now.Year;
            }
            DateTime selectedStartDate = new DateTime(startYear.Value, startMonth.Value, 1);
            DateTime selectedEndDate = new DateTime(endYear.Value, endMonth.Value, 1).AddMonths(1).AddDays(-1);
            var chart_doanhthu = db.ThongKeDoanhThuTheoThang(
                selectedStartDate,
                selectedEndDate,
                "Đã Giao"
            ).ToList();
            if (t == 0)
            {
                t = db.SanPham.Count();
            }
            var chart_tksp_m = db.ThongKeDoanhThuTheoNgay(
                selectedStartDate,
                selectedEndDate,
                "Đã Giao"
            ).ToList();
            ViewBag.chart_tksp_m = chart_tksp_m;
            float tong = chart_doanhthu.Sum(m => (float)m.TongDoanhThu);
            ViewBag.tong = tong;
            ViewBag.chart_doanhthu = chart_doanhthu;
            return View();
        }


        [AdminAuthorize(idChucNang = 31)]

        public ActionResult ThongKeDoanhThuTheoThang_Huy(int? startMonth, int? startYear, int? endMonth, int? endYear, int? t = 0)
        {
            if (!startMonth.HasValue || !startYear.HasValue || !endMonth.HasValue || !endYear.HasValue)
            {
                startMonth = DateTime.Now.Month;
                startYear = DateTime.Now.Year;
                endMonth = DateTime.Now.Month;
                endYear = DateTime.Now.Year;
            }
            DateTime selectedStartDate = new DateTime(startYear.Value, startMonth.Value, 1);
            DateTime selectedEndDate = new DateTime(endYear.Value, endMonth.Value, 1).AddMonths(1).AddDays(-1);
            var chart_doanhthu = db.ThongKeDonHangTheoThang_Huy(
                selectedStartDate,
                selectedEndDate,
                "Đã Hủy"
            ).ToList();
            if (t == 0)
            {
                t = db.SanPham.Count();
            }
            var chart_tksp_m = db.ThongKeDonTheoNgay_Huy(
                selectedStartDate,
                selectedEndDate,
                "Đã Hủy"
            ).ToList();
            ViewBag.chart_tksp_m = chart_tksp_m;
            float tong = chart_doanhthu.Sum(m => (float)m.SoLuongDonHangHuy);
            ViewBag.tong = tong;
            ViewBag.chart_doanhthu = chart_doanhthu;
            return View();
        }

    }
}