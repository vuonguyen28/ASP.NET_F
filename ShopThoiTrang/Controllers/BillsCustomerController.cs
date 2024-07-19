using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;

namespace ShopThoiTrang.Controllers
{
    public class BillsCustomerController : Controller
    {
        // GET: BillsCustomer
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        // hóa đơn đang chờ xác nhận
        public ActionResult InvoiceAwaitingConfirmation()
        {
            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }

            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            var invoicesAwaitingConfirmation = db.DonHang
                .Where(x => x.MaKhachHang == customerId && x.TrangThaiDonHang == "Đang chờ xác nhận")
                .ToList();

            return View(invoicesAwaitingConfirmation);
        }

        // đơn hàng đang vận chuyển
        public ActionResult BeingTransported()
        {
            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }
            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }
            var invoicesAwaitingConfirmation = db.DonHang
                .Where(x => x.MaKhachHang == customerId && x.TrangThaiDonHang == "Đang giao hàng")
                .ToList();

            return View(invoicesAwaitingConfirmation);
        }

        // đơn hàng đã giao
        public ActionResult BillsDelivered()
        {
            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }

            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            var invoicesAwaitingConfirmation = db.DonHang
                .Where(x => x.MaKhachHang == customerId && x.TrangThaiDonHang == "Đã Giao")
                .ToList();

            return View(invoicesAwaitingConfirmation);
        }

        // đơn hàng đã hủy
        public ActionResult canceled()
        {

            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }
            if (customerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }

            var invoicesAwaitingConfirmation = db.DonHang
                .Where(x => x.MaKhachHang == customerId && x.TrangThaiDonHang == "Đã hủy")
                .ToList();

            return View(invoicesAwaitingConfirmation);
        }
        // cập nhật địa chỉ nhận hàng
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
        public ActionResult UpdateBill(int MaDonHang, string DiaChiGiaoHang)
        {
            DonHang bill = db.DonHang.Single(d => d.MaDonHang == MaDonHang);
            if (bill == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                bill.DiaChiGiaoHang = DiaChiGiaoHang;
                db.Entry(bill).Property("DiaChiGiaoHang").IsModified = true; 

                db.SaveChanges();
                return RedirectToAction("InvoiceAwaitingConfirmation", "BillsCustomer");
            }
            return View(bill);
        }


        public ActionResult CancledBills(int MaDonHang)
        {

            DonHang bill = db.DonHang.Single(d => d.MaDonHang == MaDonHang);
            if (bill == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                bill.TrangThaiDonHang = "Đã Hủy";
                db.Entry(bill).Property("DiaChiGiaoHang").IsModified = true;

                db.SaveChanges();
                return RedirectToAction("canceled", "BillsCustomer");
            }
            return View(bill);
        }

        public ActionResult MuaLaiSanPham(int maChiTiet, string strURL)
        {
            int soLuong = 1; // Số lượng mặc định khi mua lại sản phẩm
            int maKhachHang = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out maKhachHang);
            }

            if (maKhachHang == 0)
            {
                return RedirectToAction("Login", "Customer");
            }
            else
            {
                // Lấy chi tiết sản phẩm từ cơ sở dữ liệu
                ChiTietSanPham chiTietSanPham = LayChiTietSanPhamTuDatabase(maChiTiet);

                if (chiTietSanPham != null)
                {
                    // Lấy giỏ hàng từ cơ sở dữ liệu
                    GioHang gioHang = db.GioHang.SingleOrDefault(g => g.MaKhachHang == maKhachHang && g.MaChiTietSanPham == maChiTiet);

                    if (gioHang == null)
                    {
                        gioHang = new GioHang
                        {
                            MaKhachHang = maKhachHang,
                            MaChiTietSanPham = maChiTiet,
                            SoLuong = soLuong,
                            ChiTietSanPham = chiTietSanPham
                        };

                        // Thêm giỏ hàng mới vào cơ sở dữ liệu
                        db.GioHang.Add(gioHang);
                    }
                    else
                    {
                        gioHang.SoLuong = gioHang.SoLuong + soLuong;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    return RedirectToAction("IndexProduct", "MainCustomer");
                }
                else
                {
                    // Xử lý khi không tìm thấy chi tiết sản phẩm
                    return RedirectToAction("NotFound", "Error");
                }
            }
        }

        private ChiTietSanPham LayChiTietSanPhamTuDatabase(int maChiTiet)
        {
            return db.ChiTietSanPham.SingleOrDefault(ct => ct.MaChiTiet == maChiTiet);
        }

    }
}