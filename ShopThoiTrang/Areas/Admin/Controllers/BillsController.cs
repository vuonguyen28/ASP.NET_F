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
using System.Data.SqlClient;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class BillsController : Controller
    {

        //36	NULL XemHoaDon        

        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // GET: Admin/Bills
        public ActionResult Index()
        {
            return View();
        }

        //load bill
        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowBills()
        {
            var bills = db.DonHang.ToList();
            
            return View(bills);
        }

        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowCart(string searching)
        {
            var gioHangItems = db.GioHang
                    .Include("KHACHHANG")
                    .Include("ChiTietSanPham.SanPham")
                    .Include("ChiTietSanPham.MauSac")
                    .Include("ChiTietSanPham.KichThuoc")
                    .Include("ChiTietSanPham.SanPham.HinhAnh")
                    .Where(x => x.KHACHHANG.TenKH.Contains(searching) || x.ChiTietSanPham.SanPham.TenSP.Contains(searching) || searching == null)
                    .ToList();

            foreach (var item in gioHangItems)
            {
                // Lấy giá của sản phẩm và đặt vào ViewBag
                if (item.ChiTietSanPham != null && item.ChiTietSanPham.SanPham != null)
                {
                    double? giaSanPham = item.ChiTietSanPham.SanPham.Gia - item.ChiTietSanPham.SanPham.Gia * item.ChiTietSanPham.SanPham.PhanTramGiamGia / 100;
                    ViewBag.GiaSanPham = giaSanPham;
                }
            }
            return View(gioHangItems);
        }


        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowBillsItem(int MaDonHang)
        {

            var donHang = db.DonHang
                        .Include("KhachHang") 
                        .FirstOrDefault(x => x.MaDonHang == MaDonHang);
            ViewBag.MaDH = donHang.MaDonHang;
            ViewBag.Tenkh = donHang.KHACHHANG.TenKH;
            ViewBag.sdt = donHang.KHACHHANG.SoDienThoai;
            ViewBag.email = donHang.KHACHHANG.Email;
            ViewBag.ngaydat = donHang.NgayDat;
            ViewBag.ngayGiaoHang = donHang.NgayDuKienGiaoHang;
            ViewBag.diachigiao = donHang.DiaChiGiaoHang;
            ViewBag.tonghoadon = donHang.TongGia;
            ViewBag.hinhthucthanhtoan = donHang.HinhThucThanhToan;


            var gioHangItems = db.ChiTietDonHang
                    .Include("DonHang")
                    .Include("ChiTietSanPham.SanPham")
                    .Include("ChiTietSanPham.MauSac")
                    .Include("ChiTietSanPham.KichThuoc")
                    .Include("ChiTietSanPham.SanPham.HinhAnh")
                    .Where(x=>x.MaDonHang == MaDonHang)
                    .ToList();
            return View(gioHangItems);
        }
        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowCartBuyer(int MaKhachHang)
        {
            var gioHangItems = db.GioHang
                    .Include("KHACHHANG")
                    .Include("ChiTietSanPham.SanPham")
                    .Include("ChiTietSanPham.MauSac")
                    .Include("ChiTietSanPham.KichThuoc")
                    .Include("ChiTietSanPham.SanPham.HinhAnh")
                    .Where(x=>x.MaKhachHang==MaKhachHang)
                    .ToList();

            foreach (var item in gioHangItems)
            {
                // Lấy giá của sản phẩm và đặt vào ViewBag
                if (item.ChiTietSanPham != null && item.ChiTietSanPham.SanPham != null)
                {
                    double? giaSanPham = item.ChiTietSanPham.SanPham.Gia - item.ChiTietSanPham.SanPham.Gia * item.ChiTietSanPham.SanPham.PhanTramGiamGia / 100;
                    ViewBag.GiaSanPham = giaSanPham;
                }
            }
            return View(gioHangItems);
        }




        [AdminAuthorize(idChucNang = 36)]
        public ActionResult UpdateBill(int MaDonHang = 0)
        {
            DonHang bill = db.DonHang.Single(d => d.MaDonHang == MaDonHang);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        [HttpPost]
        public ActionResult UpdateBill(int MaDonHang, string TrangThaiDonHang, DateTime NgayDuKienGiaoHang)
        {
            DonHang bill = db.DonHang.Single(d => d.MaDonHang == MaDonHang);
            if (bill == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                bill.TrangThaiDonHang = TrangThaiDonHang;
                bill.NgayDuKienGiaoHang = NgayDuKienGiaoHang;
                db.Entry(bill).Property("TrangThaiDonHang").IsModified = true; // Chỉ đánh dấu trường TrangThaiDonHang đã được thay đổi
                db.Entry(bill).Property("NgayDuKienGiaoHang").IsModified = true; // Chỉ đánh dấu trường NgayDuKienGiaoHang đã được thay đổi

                db.SaveChanges();
                return RedirectToAction("ShowBillsDate", "Bills");
            }
            return View(bill);
        }




        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowBillsBuyer(int MaKhachHang)
        {
            return View(db.DonHang.Where(x=>x.MaKhachHang==MaKhachHang).ToList());
                   
        }


        //public ActionResult ShowBillsDate()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult ShowBillsDate(DateTime startTime, DateTime endTime)
        //{
        //    var bills = db.LayDonHangTrongKhoangThoiGian(startTime, endTime).ToList();
        //    return View("HienThiDonHang", bills);
        //}

        //public ActionResult ShowBillsDate1(DateTime? startTime, DateTime? endTime)
        //{
        //    if (!startTime.HasValue || !endTime.HasValue)
        //    {
        //        // Nếu thời gian không được cung cấp, sử dụng thời gian mặc định
        //        startTime = DateTime.Now;
        //        endTime = DateTime.Now;
        //    }

        //    var bills = db.LayDonHangTrongKhoangThoiGian(startTime.Value, endTime.Value).ToList();
        //    return View(bills);
        //}


        [AdminAuthorize(idChucNang = 36)]
        public ActionResult ShowBillsDate(DateTime? startTime, DateTime? endTime, string trangThaiDonHang, string hinhThucThanhToan, int? maKhachHang, string trangThaiThanhToan, decimal? giaMin, decimal? giaMax)
        {
            if (!startTime.HasValue || !endTime.HasValue )
            {
                // Nếu thời gian không được cung cấp, gán giá trị null
                startTime = DateTime.Now;
                endTime = DateTime.Now;
            }

            trangThaiDonHang = trangThaiDonHang ?? null;
            hinhThucThanhToan = string.IsNullOrEmpty(hinhThucThanhToan) ? null : hinhThucThanhToan;
            maKhachHang = maKhachHang ?? null;
            trangThaiThanhToan = trangThaiThanhToan ?? null;
            giaMin = giaMin ?? null;
            giaMax = giaMax ?? null;


            var bills = db.LocDonHangTheoGioiGian_TongTien_MaKH_TrangThai(
                startTime,
                endTime,
                //trangThaiDonHang = null,
                string.IsNullOrEmpty(trangThaiDonHang) ? null : trangThaiDonHang,
                string.IsNullOrEmpty(hinhThucThanhToan) ? null : hinhThucThanhToan,
                maKhachHang,
                trangThaiThanhToan,
                giaMin,
                giaMax
            ).ToList();

            return View(bills);
        }

        //public ActionResult ShowBillsDate2(DateTime? startTime, DateTime? endTime, string trangThaiDonHang, string hinhThucThanhToan, int? maKhachHang, string trangThaiThanhToan, decimal? giaMin, decimal? giaMax)
        //{
        //    if (!startTime.HasValue || !endTime.HasValue)
        //    {
        //        // Nếu thời gian không được cung cấp, gán giá trị null
        //        startTime = DateTime.Now;
        //        endTime = DateTime.Now;
        //    }

        //    var query = "SELECT * FROM dbo.LocDonHangTheoGioiGian_TongTien_MaKH_TrangThai(@startTime, @endTime, @trangThaiDonHang, @hinhThucThanhToan, @maKhachHang, @trangThaiThanhToan, @giaMin, @giaMax)";

        //    var bills = db.Database.SqlQuery<LocDonHangTheoGioiGian_TongTien_MaKH_TrangThai_Result>(query,
        //        new SqlParameter("@startTime", startTime ?? DateTime.Now),
        //        new SqlParameter("@endTime", endTime ?? DateTime.Now),
        //        new SqlParameter("@trangThaiDonHang", trangThaiDonHang ?? (object)DBNull.Value),
        //        new SqlParameter("@hinhThucThanhToan", string.IsNullOrEmpty(hinhThucThanhToan) ? (object)DBNull.Value : hinhThucThanhToan),
        //        new SqlParameter("@maKhachHang", maKhachHang ?? (object)DBNull.Value),
        //        new SqlParameter("@trangThaiThanhToan", trangThaiThanhToan ?? (object)DBNull.Value),
        //        new SqlParameter("@giaMin", giaMin ?? (object)DBNull.Value),
        //        new SqlParameter("@giaMax", giaMax ?? (object)DBNull.Value)
        //    ).ToList();

        //    return View(bills);
        //}


        //update bill
    }
}