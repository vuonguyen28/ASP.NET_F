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
    public class VoucherController : Controller
    {
        // GET: Voucher
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
        public ActionResult ShowVoucher()
        {


            int maKhachHang = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out maKhachHang);
            }

            if(maKhachHang==0)
            {
                return RedirectToAction("Login", "Customer");
            } 
            return View(db.VOUCHER.ToList());
        }

        public ActionResult SaveVoucher(string MaVoucher)
        {
            
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
                VOUCHER voucher = db.VOUCHER.Where(x => x.MA_VOUCHER == MaVoucher).FirstOrDefault();
                int? sl_voucher = voucher.SoLuongVOUCHER;


                if (voucher != null)
                {
                    // Lấy giỏ hàng từ cơ sở dữ liệu
                    VOUCHER_KHACHHANG voucher_KH = db.VOUCHER_KHACHHANG.SingleOrDefault(g => g.MaKH == maKhachHang && g.MA_VOUCHER == MaVoucher);

                    if (voucher_KH == null)
                    {
                        voucher_KH = new VOUCHER_KHACHHANG
                        {
                            MaKH = maKhachHang,
                            MA_VOUCHER = MaVoucher,
                           
                        };
                        voucher.SoLuongVOUCHER -= 1;

                        // Thêm giỏ hàng mới vào cơ sở dữ liệu
                        db.VOUCHER_KHACHHANG.Add(voucher_KH);
                    }
                    else
                    {
                        ViewBag.thongbao = "voucher này bạn đã lưu";
                    }
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    return RedirectToAction("ShowVoucher", "Voucher");
                }
                else
                {
                    return RedirectToAction("NotFound", "Error");
                }
            }
        }

        //public ActionResult Voucher()
        //{

        //    int maKhachHang = 0;
        //    HttpCookie cookie = Request.Cookies["CustomerInfo"];

        //    if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
        //    {
        //        int.TryParse(cookie["MaKH"], out maKhachHang);
        //    }

        //    vou

        //}

        public ActionResult MyVoucher()
        {
            int maKhachHang = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out maKhachHang);
            }

            var listMyVoucher = db.VOUCHER_KHACHHANG.Include("VOUCHER").Where(x => x.MaKH == maKhachHang && x.note!= "Đã sử dụng").ToList();
            return View(listMyVoucher);
        }



    }
}