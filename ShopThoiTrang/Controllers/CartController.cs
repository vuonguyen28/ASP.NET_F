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
    public class CartController : Controller
    {
        // GET: Cart
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        //public ActionResult CreateFeedback( int? MaChiTiet, string noiDung, double? danhGia)
        //{
        //    ChiTietDonHang ctdh = db.ChiTietDonHang.FirstOrDefault(x => x.MaChiTiet == MaChiTiet);

        //    int maKhachHang = 0;
        //    HttpCookie cookie = Request.Cookies["CustomerInfo"];

        //    if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
        //    {
        //        int.TryParse(cookie["MaKH"], out maKhachHang);
        //    }



        //    // Tạo đối tượng Feedback mới
        //    FeedBack newFeedback = new FeedBack
        //    {
        //        MaKH = maKhachHang,
        //        MaChiTietDH = MaChiTiet,
        //        NoiDung = noiDung,
        //        ThoiGianfeedback = DateTime.Now, 
        //        DanhGia = danhGia
        //    };

        //    // Lưu đối tượng Feedback vào cơ sở dữ liệu
        //    db.FeedBack.Add(newFeedback);
        //    db.SaveChanges();
        //    return View();
        //}

        public ActionResult CreateFeedback(int? MaChiTiet, string noiDung, double? danhGia)
        {
            // Lấy thông tin ChiTietDonHang từ MaChiTiet
            ChiTietDonHang ctdh = db.ChiTietDonHang.FirstOrDefault(x => x.MaChiTiet == MaChiTiet);

            var cartItems = db.ChiTietDonHang
                .Include(g => g.ChiTietSanPham.SanPham.HinhAnh)
                .Where(g => g.MaChiTiet == MaChiTiet)
                .ToList();
            // Lấy danh sách hình ảnh sản phẩm từ mã chi tiết đơn hàng
            var danhSachHinhAnh = db.HinhAnh
                .Where(hinhAnh => hinhAnh.SanPham.ChiTietSanPham
                    .Any(ctsp => ctsp.ChiTietDonHang.Any(x => x.MaChiTiet == MaChiTiet)))
                .ToList();

            // Đưa danh sách hình ảnh vào ViewBag để sử dụng trong View
            ViewBag.DanhSachHinhAnh = danhSachHinhAnh;


            if (ctdh != null)
            {
                // Lấy thông tin ChiTietSanPham từ ChiTietDonHang
                ChiTietSanPham chiTietSanPham = ctdh.ChiTietSanPham;

                if (chiTietSanPham != null)
                {
                    // Lấy thông tin sản phẩm từ ChiTietSanPham
                    SanPham sanPham = chiTietSanPham.SanPham;

                    if (sanPham != null)
                    {
                        // Lấy thông tin màu từ ChiTietSanPham
                        MauSac mauSac = chiTietSanPham.MauSac;
                        KichThuoc size = chiTietSanPham.KichThuoc;

                        // Đẩy thông tin sản phẩm và màu vào ViewBag để sử dụng trong View
                        ViewBag.TenSanPham = sanPham.TenSP;
                        ViewBag.TenMau = mauSac != null ? mauSac.TenMau : "Không có màu";
                        ViewBag.KichThuoc = size != null ? size.TenKichThuoc : "Không có kích thước";


                        int maKhachHang = 0;
                        HttpCookie cookie = Request.Cookies["CustomerInfo"];

                        if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
                        {
                            int.TryParse(cookie["MaKH"], out maKhachHang);
                        }

                        // Tạo đối tượng Feedback mới
                        FeedBack newFeedback = new FeedBack
                        {
                            MaKH = maKhachHang,
                            MaChiTietDH = MaChiTiet,
                            NoiDung = noiDung,
                            ThoiGianfeedback = DateTime.Now,
                            DanhGia = danhGia
                        };

                        // Lưu đối tượng Feedback vào cơ sở dữ liệu
                        db.FeedBack.Add(newFeedback);
                        db.SaveChanges();

                        return View(); // Trả về View để hiển thị thông tin sản phẩm và màu


                    }
                }
            }

            return View("Error"); // Trả về View thông báo lỗi nếu không tìm thấy thông tin sản phẩm hoặc màu
        }




        // xóa giỏ hàng
        [HttpPost]
        public ActionResult XoaGioHang(int maGioHang)
        {
            var gioHang = db.GioHang.FirstOrDefault(g => g.MaGioHang == maGioHang);

            if (gioHang != null)
            {
                db.GioHang.Remove(gioHang);
                db.SaveChanges();
                return RedirectToAction("ShoppingCart", "Cart"); // Redirect to Home/Index after successful deletion
            }

            return RedirectToAction("ShoppingCart", "Cart"); // Redirect to Home/Index if gioHang is null
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThemGioHang(int maChiTiet,int soLuong, string strURL)
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

                    //return Redirect(strURL);
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

        public ActionResult ChiTietSanPham(int MaSP)
        {
            return View(db.ChiTietSanPham.Where(x => x.MaSP == MaSP).ToList());
        }
       


        public ActionResult ShoppingCart()
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
            // lấy địa chỉ khách hàng thông qua MaKH ở coking
            var customer = db.KHACHHANG.FirstOrDefault(c => c.MaKH == customerId);

            if (customer != null)
            {
                // Sử dụng thông tin khách hàng để lấy địa chỉ
                string customerAddress = customer.DiaChi; // Giả sử địa chỉ lưu trong trường DiaChi của bảng KHACHHANG

                // Xử lý địa chỉ khách hàng ở đây
                ViewBag.CustomerAddress = customerAddress;
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin khách hàng
                ViewBag.CustomerAddress = "Không tìm thấy thông tin khách hàng";
            }
            var cartItems = db.GioHang
                .Include(g => g.ChiTietSanPham.SanPham)  // Include related entities
                .Include(g => g.ChiTietSanPham.MauSac)
                .Include(g => g.ChiTietSanPham.KichThuoc)
                .Include(g => g.ChiTietSanPham.SanPham.HinhAnh)
                .Where(g => g.MaKhachHang == customerId)
                .ToList();

            double totalCartPrice = (double)cartItems.Sum(cartItem =>
                         cartItem.ChiTietSanPham.SanPham.Gia *
                         (1 - (cartItem.ChiTietSanPham.SanPham.PhanTramGiamGia ?? 0) / 100) *
                         cartItem.SoLuong
                     );
            double phiVanChuyen ;
            if (totalCartPrice >= 300000)
                phiVanChuyen = 0;
            else
                phiVanChuyen = 25000;

            // tổng dơn hàng có phí vận chuyển
            

            var cartViewModel = cartItems
                .Select(cartItem => new ShoppingCartItemViewModel
                {
                    CartItemId = cartItem.MaGioHang,
                    ProductName = cartItem.ChiTietSanPham.SanPham.TenSP,
                    Price = cartItem.ChiTietSanPham.SanPham.Gia,
                    Sale = cartItem.ChiTietSanPham.SanPham.PhanTramGiamGia,
                    Images = cartItem.ChiTietSanPham.SanPham.HinhAnh.Select(ha => ha.hinhanh1).ToList(),
                    Quantity = cartItem.SoLuong,
                    ColorName = cartItem.ChiTietSanPham.MauSac.TenMau,
                    SizeName = cartItem.ChiTietSanPham.KichThuoc.TenKichThuoc,
                    TotalPrice = cartItem.ChiTietSanPham.SanPham.Gia * (1 - (cartItem.ChiTietSanPham.SanPham.PhanTramGiamGia ?? 0) / 100) * cartItem.SoLuong

                })
                .ToList();
            double tongVAT = totalCartPrice + phiVanChuyen;

            DateTime? NgayDat = DateTime.Now;
            DateTime? NgayGiaoHangDuKien = DateTime.Now.AddDays(3);


            double? tongVATSauGiamGIa = Convert.ToDouble(TempData["tongVATSauGiamGIa"]);
            if (tongVATSauGiamGIa == 0)
            {
                tongVATSauGiamGIa = tongVAT;
            }    

            ViewBag.tongtiendonhang = totalCartPrice;
            ViewBag.phivanchuyen = phiVanChuyen;
            ViewBag.tongDonHangCoPhiVanChuyen = tongVAT;
            ViewBag.ngaygiao = NgayDat;
            ViewBag.ngaynhan = NgayGiaoHangDuKien;
            ViewBag.TongVATsauGiamGia = tongVATSauGiamGIa;
            TempData["TongVAT"] = tongVAT;

            var message = TempData["Message"];

            return View(cartViewModel);
        }

        [HttpPost]
        public ActionResult submitVoucher(string MaVoucher)
        {
            double? tongVATSauGiamGIa;
            double? tongVAT = Convert.ToDouble(TempData["TongVAT"]);
            int maKhachHang = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out maKhachHang);
            }
            VOUCHER_KHACHHANG voucher_KH = db.VOUCHER_KHACHHANG.SingleOrDefault(g => g.MaKH == maKhachHang && g.MA_VOUCHER == MaVoucher && g.note== null);
            if (voucher_KH != null)
            {
                VOUCHER voucher = db.VOUCHER.Where(x=>x.MA_VOUCHER == MaVoucher).SingleOrDefault();
                double? phantram = voucher.PhanTramGiam;
                
                tongVATSauGiamGIa = tongVAT - (tongVAT * phantram / 100);

                TempData["tongVATSauGiamGIa"] = tongVATSauGiamGIa;
                Session["tongVATSauGiamGIa"] = tongVATSauGiamGIa;
                Session["MaVoucher"] = MaVoucher;


            }
            else
            {
                tongVATSauGiamGIa = tongVAT;
                TempData["tongVATSauGiamGIa"] = tongVATSauGiamGIa;
                Session["tongVATSauGiamGIa"] = tongVATSauGiamGIa;
                Session["MaVoucher"] = MaVoucher;

                // thông báo lxoi
                TempData["Message"] = "Voucher này sử dụng hoặc không tồn tại voucher này!"; 

            }
            return RedirectToAction("ShoppingCart", "Cart");
        }

        // chức năng thanh toán đơn hàng
        public ActionResult MuaNgay(string paymentMethod)
        {
            HttpCookie cookie = Request.Cookies["CustomerInfo"];
            if (cookie != null)
            {
                int maKhachHang;
                if (int.TryParse(cookie["MaKH"], out maKhachHang))
                {
                    var listCart = db.GioHang
                        .Include(g => g.ChiTietSanPham.SanPham)  // Include related entities
                        .Include(g => g.ChiTietSanPham.MauSac)
                        .Include(g => g.ChiTietSanPham.KichThuoc)
                        .Include(g => g.ChiTietSanPham.SanPham.HinhAnh)
                        .Where(g => g.MaKhachHang == maKhachHang)
                        .ToList();
            
                    if (listCart != null && listCart.Any())
                    {
                        // Tạo đơn hàng mới
                        DonHang ddh = new DonHang();
                        ddh.MaKhachHang = maKhachHang;
                        ddh.NgayDat = DateTime.Now;
                        ddh.NgayDuKienGiaoHang = DateTime.Now.AddDays(3);
                        ddh.TrangThaiThanhToan = 0;

                        var customer = db.KHACHHANG.FirstOrDefault(c => c.MaKH == maKhachHang);
                        if (customer != null)
                        {
                            ddh.DiaChiGiaoHang = customer.DiaChi;
                        }
                        else
                        {
                        
                            ddh.DiaChiGiaoHang = "chưa cập nhật địa chỉ";
                        }




                        // Thêm đơn hàng vào cơ sở dữ liệu
                        db.DonHang.Add(ddh);
                        db.SaveChanges();

                        double? totalOrderPrice = 0; // Tổng giá của đơn hàng

                        // Tạo các chi tiết đơn hàng từ danh sách sản phẩm trong giỏ hàng
                        foreach (var item in listCart)
                        {
                            ChiTietDonHang ctdh = new ChiTietDonHang();
                            ctdh.MaDonHang = ddh.MaDonHang;
                            ctdh.MaChiTietSanPham = item.MaChiTietSanPham;
                            //ctdh.SoLuong = item.SoLuong;
                            //kiểm tra số lượng tồn nếu số lượng mua lớn hơn số lượng tồn thì hiển thị thông báo và sản phầm ko mua đc
                            
                            ChiTietSanPham ctsp = db.ChiTietSanPham.Find(item.MaChiTietSanPham);
                            if (item.SoLuong > ctsp.SoLuongTon)
                            {
                                return RedirectToAction("NotFound", "MainCustomer");
                            }
                            else
                            {
                                ctdh.SoLuong = item.SoLuong;
                            }

                            // Tính giá của từng sản phẩm trong đơn hàng
                            double? giamGia = 0;
                            if (item.ChiTietSanPham.SanPham.Gia != null && item.ChiTietSanPham.SanPham.PhanTramGiamGia != null)
                            {
                                giamGia = item.ChiTietSanPham.SanPham.Gia- item.ChiTietSanPham.SanPham.Gia * item.ChiTietSanPham.SanPham.PhanTramGiamGia / 100;
                            }
                            else
                            if (item.ChiTietSanPham.SanPham.PhanTramGiamGia == null)
                            {
                                giamGia = item.ChiTietSanPham.SanPham.Gia;
                            }

                            // Lưu giá của từng sản phẩm vào chi tiết đơn hàng
                            ctdh.Gia = giamGia;

                            // Thêm giá của từng sản phẩm vào tổng giá đơn hàng
                            totalOrderPrice += ctdh.Gia * ctdh.SoLuong;
                            
                            // Thêm chi tiết đơn hàng vào cơ sở dữ liệu
                            db.ChiTietDonHang.Add(ctdh);

                            // Xóa sản phẩm trong giỏ hàng sau khi thêm vào đơn hàng
                            var productInCart = db.GioHang.FirstOrDefault(g => g.MaKhachHang == maKhachHang && g.MaChiTietSanPham == item.MaChiTietSanPham);
                            if (productInCart != null)
                            {
                                db.GioHang.Remove(productInCart);
                            }
                        }

                        // Cập nhật tổng giá của đơn hàng
                        ddh.TongGia = totalOrderPrice;

                        // cập nhật phí vận chuyển
                        double phiVanChuyen;
                        if (totalOrderPrice >= 300000)
                            phiVanChuyen = 0;
                        else
                            phiVanChuyen = 25000;
                        ddh.PhiVanChuyen = phiVanChuyen;
                        ddh.TrangThaiDonHang = "Đang chờ xác nhận";
                        ddh.HinhThucThanhToan = paymentMethod;



                        // cập nhất tổng tiền sau khi giảm gái tỏng db
                        double? tongVATSauGiamGIa = Session["tongVATSauGiamGIa"] as double?;
                        if (tongVATSauGiamGIa == null || tongVATSauGiamGIa == 0)
                        {
                            tongVATSauGiamGIa = totalOrderPrice;
                        }

                        // tiền sau khi giảm
                        ddh.TongTienSauKhiGiamGia = tongVATSauGiamGIa;

                        // Lưu Mã voucher

                        string MaVoucher = Session["MaVoucher"] as string;
                        ddh.MaVoucher = MaVoucher;

                        VOUCHER_KHACHHANG voucher_kh = db.VOUCHER_KHACHHANG.Where(x => x.MA_VOUCHER == MaVoucher && x.MaKH== maKhachHang).FirstOrDefault();
                        if(voucher_kh != null)
                        {
                            voucher_kh.note = "Đã sử dụng";
                        }    
                        db.SaveChanges();

                        return RedirectToAction("InvoiceAwaitingConfirmation", "BillsCustomer"); // Chuyển hướng đến trang thông báo đặt hàng thành công
                    }
                }
            }

            return RedirectToAction("InvoiceAwaitingConfirmation", "BillsCustomer"); // Chuyển hướng đến trang thông báo đặt hàng thành công
        }

        
        // hàm tự động cập nhật số lượng trong giỏ hàng
        [HttpPost]
        public ActionResult UpdateQuantity(int cartItemId, int newQuantity)
        {
            // Implement the logic to update the quantity in the database
            var cartItem = db.GioHang.Find(cartItemId);
            if (cartItem != null)
            {
                cartItem.SoLuong = newQuantity;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        public ActionResult DonHangDaMua()
        {
            return View();
            //int customerId = 0;
            //HttpCookie cookie = Request.Cookies["CustomerInfo"];

            //if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            //{
            //    int.TryParse(cookie["MaKH"], out customerId);

            //    // Lấy danh sách đơn hàng đã mua của khách hàng từ cơ sở dữ liệu
            //    var donHangs = db.DonHang.Where(dh => dh.MaKhachHang == customerId).ToList();

            //    List<DonHangViewModel> donHangViewModels = new List<DonHangViewModel>();

            //    foreach (var donHang in donHangs)
            //    {
            //        // Lấy danh sách chi tiết đơn hàng của mỗi đơn hàng
            //        var chiTietDonHangs = db.ChiTietDonHang.Where(ctdh => ctdh.MaDonHang == donHang.MaDonHang).ToList();

            //        DonHangViewModel donHangViewModel = new DonHangViewModel
            //        {
            //            DonHang = donHang,
            //            ChiTietDonHangs = chiTietDonHangs
            //        };

            //        donHangViewModels.Add(donHangViewModel);
            //    }

            //    return View(donHangViewModels);
            //}

            //// Nếu không tìm thấy thông tin khách hàng hoặc không có đơn hàng, có thể chuyển hướng đến trang thông báo không có dữ liệu
            //return View("NoDataFoundView");
        }

        // đang lõi
        public ActionResult XemDonHangTruocKhiDat()
        {
            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }

            var cartItems = db.GioHang
                .Include(g => g.ChiTietSanPham.SanPham) 
                .Include(g => g.ChiTietSanPham.MauSac)
                .Include(g => g.ChiTietSanPham.KichThuoc)
                .Include(g => g.ChiTietSanPham.SanPham.HinhAnh)
                .Where(g => g.MaKhachHang == customerId)
                .ToList();

            var cartViewModel = cartItems
                .Select(cartItem => new ShoppingCartItemViewModel
                {
                    CartItemId = cartItem.MaGioHang,
                    ProductName = cartItem.ChiTietSanPham.SanPham.TenSP,
                    Price = cartItem.ChiTietSanPham.SanPham.Gia,
                    Images = cartItem.ChiTietSanPham.SanPham.HinhAnh.Select(ha => ha.hinhanh1).ToList(),
                    Quantity = cartItem.SoLuong,
                    ColorName = cartItem.ChiTietSanPham.MauSac.TenMau,
                    SizeName = cartItem.ChiTietSanPham.KichThuoc.TenKichThuoc,
                    TotalPrice = (cartItem.ChiTietSanPham.SanPham.Gia - cartItem.ChiTietSanPham.SanPham.Gia*(cartItem.ChiTietSanPham.SanPham.PhanTramGiamGia/100))* cartItem.SoLuong
                })
                .ToList();

            return View(cartViewModel);
        }





        public ActionResult CapNhatDonHangVaChiTietDonHang()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CapNhatDonHangVaChiTietDonHang(DonHang donHang, List<ChiTietDonHang> chiTietDonHangList)
        {
            if (ModelState.IsValid)
            {
                using (var db = new QL_SHOPTHOITRANG_DOANEntities()) // Thay YourDbContext bằng context của bạn
                {
                    // Cập nhật thông tin đơn hàng
                    var existingDonHang = db.DonHang.Find(donHang.MaDonHang);

                    if (existingDonHang != null)
                    {
                        existingDonHang.NgayDat = donHang.NgayDat;
                        existingDonHang.TongGia = donHang.TongGia;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                    }

                    // Cập nhật thông tin các chi tiết đơn hàng
                    foreach (var chiTietDonHang in chiTietDonHangList)
                    {
                        var existingChiTietDonHang = db.ChiTietDonHang.Find(chiTietDonHang.MaChiTiet);

                        if (existingChiTietDonHang != null)
                        {
                            existingChiTietDonHang.SoLuong = chiTietDonHang.SoLuong;
                            existingChiTietDonHang.Gia = chiTietDonHang.Gia;

                            // Lưu thay đổi vào cơ sở dữ liệu
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index"); // Chuyển hướng sau khi cập nhật thành công
            }

            return View(); // Hiển thị lại View nếu dữ liệu không hợp lệ
        }
        // Lưu hình thức thanh toán xuống db
        // Trong CartController
        [HttpPost]
        public ActionResult ProcessPayment(string paymentMethod)
        {
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                int? maKhachHang = null;

                // Kiểm tra và lấy MaKH từ Cookie
                if (Request.Cookies["MaKH"] != null)
                {
                    maKhachHang = int.TryParse(Request.Cookies["MaKH"].Value, out int maKH) ? maKH : (int?)null;
                }

                if (maKhachHang.HasValue)
                {
                    // Lấy đơn hàng mới nhất của MaKH từ cơ sở dữ liệu
                    var latestOrder = db.DonHang
                        .Where(dh => dh.MaKhachHang == maKhachHang)
                        .OrderByDescending(dh => dh.NgayDat)
                        .FirstOrDefault();

                    if (latestOrder != null)
                    {
                        // Cập nhật hình thức thanh toán cho đơn hàng mới nhất
                        latestOrder.HinhThucThanhToan = paymentMethod;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();

                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy đơn hàng cho MaKH này." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin MaKH." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng chọn hình thức thanh toán." });
            }
        }


       

        public ActionResult ShowBillCustomer()
        {
            int customerId = 0;
            HttpCookie cookie = Request.Cookies["CustomerInfo"];

            if (cookie != null && !string.IsNullOrEmpty(cookie["MaKH"]))
            {
                int.TryParse(cookie["MaKH"], out customerId);
            }

            return View(db.DonHang.Where(x => x.MaKhachHang == customerId).ToList());   
        }

        public ActionResult ShowBillsItem( int MaHoaDon)
        {
            var gioHangItems = db.ChiTietDonHang
                     .Include("DonHang")
                     .Include("ChiTietSanPham.SanPham")
                     .Include("ChiTietSanPham.MauSac")
                     .Include("ChiTietSanPham.KichThuoc")
                     .Include("ChiTietSanPham.SanPham.HinhAnh")
                     .Where(x => x.MaDonHang == MaHoaDon)
                     .ToList();
            

            return View(gioHangItems);
        }

        public ActionResult ShowBillsItem1(int MaHoaDon)
        {
            var gioHangItems = db.ChiTietDonHang
                     .Include("DonHang")
                     .Include("ChiTietSanPham.SanPham")
                     .Include("ChiTietSanPham.MauSac")
                     .Include("ChiTietSanPham.KichThuoc")
                     .Include("ChiTietSanPham.SanPham.HinhAnh")
                     .Where(x => x.MaDonHang == MaHoaDon)
                     .ToList();


            return View(gioHangItems);
        }

        // tạo feedback cho chit tiết đơn hàng

    }



}
