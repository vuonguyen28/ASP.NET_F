using ShopThoiTrang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;

namespace ShopThoiTrang.Controllers
{
    public class CustomerController : Controller
    {
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();


        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        // GET: /KhachHang/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Email, Password_hs5, ConfirmPassword")] KHACHHANG model)
        {
            HashMD hs = new HashMD();
            if (ModelState.IsValid)
            {
                // Check if the email is already in use: check email đã được sử dụng
                if (db.KHACHHANG.Any(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(model);
                }

                // Check if the password and password confirmation match : kiểm tra trùng giữa mật khẩu và xác nhận mật khẩu
                if (model.Password_hs5 != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and confirmation do not match.");
                    return View(model);
                }
                KHACHHANG a = new KHACHHANG();
                a.Email = model.Email;
                a.Password_hs5 = hs.CalculateMD5Hash(model.Password_hs5);
                db.KHACHHANG.Add(a);
                db.SaveChanges();

                return RedirectToAction("Login", "Customer");
            }

            return View(model);
        }



        // GET: /KhachHang/Logout
        public ActionResult Logout()
        {
            // Xóa Cookie hoặc Session lưu thông tin của khách hàng
            if (Request.Cookies["CustomerInfo"] != null)
            {
                var customerCookie = new HttpCookie("CustomerInfo");
                customerCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(customerCookie);
            }

            // Hoặc xóa thông tin trong Session (nếu đã sử dụng Session)
            Session["CustomerInfo"] = null;

            // Redirect hoặc thực hiện các thao tác khác sau khi đăng xuất
            return RedirectToAction("IndexProduct", "MainCustomer"); // Chuyển hướng đến trang chính của ứng dụng sau khi đăng xuất
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /KhachHang/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password_hs5)
        {
            HashMD hs = new HashMD();
            string mk = hs.CalculateMD5Hash(Password_hs5);
            if (ModelState.IsValid)
            {
                var customer = db.KHACHHANG.FirstOrDefault(c => c.Email == Email && c.Password_hs5 == mk);
                var nhanVien = db.NHANVIEN.FirstOrDefault(c => c.Email == Email && c.Password == Password_hs5);
                if (customer != null)
                {
                    if (customer.TrangThai == "Chặn")
                    {
                        ViewBag.thongBao = "Tài Khoản Đang Bị Chặn!";
                        return View();
                    }
                    else
                    {
                        // Tạo một Cookie để lưu thông tin khách hàng
                        HttpCookie customerCookie = new HttpCookie("CustomerInfo");
                        customerCookie["MaKH"] = customer.MaKH.ToString();
                        customerCookie["TenKH"] = customer.TenKH;

                        //load hình ảnh khách hàng
                        customerCookie["Avatar"] = customer.Avatar;
                        // Thêm thông tin khác cần lưu vào cookie ở đây

                        //customerCookie["IsAdmin"] = "False"; // Khách hàng không phải là admin

                        // Đặt thời gian sống của cookie (ví dụ: 7 ngày)
                        customerCookie.Expires = DateTime.Now.AddDays(7);

                        // Thêm cookie vào Response để lưu trữ trên máy khách
                        Response.Cookies.Add(customerCookie);


                        return RedirectToAction("indexProduct", "MainCustomer"); // Chuyển hướng đến trang chính của ứng dụng
                    }

                }
                else if (nhanVien != null)
                {


                    Session["user"] = nhanVien;

                    // Lưu mã id nhân viên vào Session
                    Session["MaNV"] = nhanVien.MaNV;
                    //// Tạo một Cookie để lưu thông tin khách hàng
                    //HttpCookie NhanVienCookie = new HttpCookie("NhanVienInfo");

                    //// Thêm thông tin khác cần lưu vào cookie ở đây

                    //// Đặt thời gian sống của cookie (ví dụ: 7 ngày)
                    //NhanVienCookie.Expires = DateTime.Now.AddDays(7);

                    ////NhanVienCookie["IsAdmin"] = "True"; // Nhân viên là admin

                    //// Thêm cookie vào Response để lưu trữ trên máy khách
                    //Response.Cookies.Add(NhanVienCookie);

                    return RedirectToAction("Home", "Admin"); // Chuyển hướng đến trang chính của ứng dụng
                }
                else
                {
                    //ModelState.AddModelError("", "Thông tin đăng nhập không chính xác");
                    ViewBag.thongBao = "Thông tin đăng nhập không chính xác";
                    return View();
                }
            }

            return View();

        }
        //public ActionResult Login(KHACHHANG model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var customer = db.KHACHHANG.FirstOrDefault(c => c.Email == model.Email && c.Password_hs5 == model.Password_hs5);

        //        if (customer != null)
        //        {
        //            // Tạo một Cookie để lưu thông tin khách hàng
        //            HttpCookie customerCookie = new HttpCookie("CustomerInfo");
        //            customerCookie["MaKH"] = customer.MaKH.ToString();
        //            customerCookie["TenKH"] = customer.TenKH;

        //            //load hình ảnh khách hàng
        //            customerCookie["Avatar"] = customer.Avatar;
        //            // Thêm thông tin khác cần lưu vào cookie ở đây

        //            // Đặt thời gian sống của cookie (ví dụ: 7 ngày)
        //            customerCookie.Expires = DateTime.Now.AddDays(7);

        //            // Thêm cookie vào Response để lưu trữ trên máy khách
        //            Response.Cookies.Add(customerCookie);

        //            return RedirectToAction("indexProduct", "MainCustomer"); // Chuyển hướng đến trang chính của ứng dụng
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Thông tin đăng nhập không chính xác");
        //            return View(model);
        //        }
        //    }

        //    return View(model);

        //}


        public ActionResult ProductDetailItem()
        {
            CustomerInfoModel customerInfo = new CustomerInfoModel();

            // Lấy thông tin từ Cookie nếu tồn tại
            HttpCookie customerCookie = Request.Cookies["CustomerInfo"];

            if (customerCookie != null)
            {
                // Lấy thông tin từ Cookie và gán vào Model
                customerInfo.MaKH = customerCookie["MaKH"];
                customerInfo.TenKH = customerCookie["TenKH"];
                // Gán các thông tin khác cần thiết từ Cookie vào Model ở đây
            }
            else
            {
                // Xử lý khi không có thông tin trong Cookie (có thể làm gì đó khác)
                customerInfo.TenKH = "chưa đăng nhập";
            }

            // Trả về View với Model chứa thông tin người dùng
            return View(customerInfo);
        }

        //EDIT PROFILE CUSTOMER: CHỈNH SỬA THÔNG TIN CÁ NHÂN CỦA KHÁCH HÀNG
        [HttpGet]
        public ActionResult EditProfile()
        {
            // Get the current customer's information from the cookie: lấy thông tin khách hàng từ cookies đăng nhập
            HttpCookie customerCookie = Request.Cookies["CustomerInfo"];

            if (customerCookie != null)
            {

                int maKH;
                if (int.TryParse(customerCookie["MaKH"], out maKH))
                {
                    // Retrieve customer information from the database: truy xuất thông tin khách hàng từ csdl
                    KHACHHANG customer = db.KHACHHANG.Find(maKH);

                    if (customer != null)
                    {
                        return View(customer);
                    }
                }
            }

            // If there is an issue, redirect to the login page: nếu có sự cố chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(KHACHHANG model, HttpPostedFileBase file)
        {
            // Check if the model state is valid: nếu model hợp lệ
            if (ModelState.IsValid)
            {
                // Check if a new avatar file is uploaded: kiểm tra nếu avatar mới được upload lên 
                if (file != null && file.ContentLength > 0)
                {
                    // Generate a unique filename for the avatar: tạo tên tệp cho avatar
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);

                    // Save the avatar file to the server: lưu file avatar vào server
                    string path = Server.MapPath("~/Image/");
                    file.SaveAs(System.IO.Path.Combine(path, fileName));

                    // Update the customer's avatar path in the database: cập nhật đường dẫn avatar vào database
                    model.Avatar = fileName;
                }

                // Update other customer information in the database: cập những thông tin còn lại vào database
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

                // Update the avatar information in the cookie if it exists: cập nhật lại avatar nếu cookies tồn tại (khi đăng nhập vào sẽ lấy cookies)
                if (Request.Cookies["CustomerInfo"] != null)
                {
                    HttpCookie customerCookie = Request.Cookies["CustomerInfo"];
                    customerCookie["Avatar"] = model.Avatar;
                    Response.Cookies.Add(customerCookie);
                }

                TempData["UpdatedAvatar"] = model.Avatar; // Lưu giá trị Avatar vào TempData
                return RedirectToAction("IndexProduct", "MainCustomer");
            }

            // If the model state is not valid, return to the edit profile page with the model
            return View(model);
        }


        //QUÊN MẬT KHẨU
        public string RanDomMK()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int passwordLength = 6;

            Random random = new Random();
            var otp = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                otp[i] = chars[random.Next(chars.Length)];
            }

            return new string(otp);

        }
        public ActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuenMatKhau(KHACHHANG kh)
        {
            QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
            var s = db.KHACHHANG.Where(row => row.Email == kh.Email).FirstOrDefault();
            TempData["KhachHangInfo"] = s;
            if (s != null)
            {
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("Huynhsang1020@gmail.com", "loaz bqyu vrrv kiap"),
                    EnableSsl = true
                };

                var message = new MailMessage();
                message.From = new MailAddress("Huynhsang1020@gmail.com");
                message.ReplyToList.Add("Huynhsang1020@gmail.com");
                message.To.Add(new MailAddress(s.Email));
                message.Subject = "Thông Báo Về Việc Thay Đổi Mật Khẩu FaceBook của bạn  ";

                // Cập nhật mật khẩu
                string otp = RanDomMK();
                Session["otp"] = otp;

                //s.Password_hs5 = mk;

                //// Thêm s vào db.KHACHHANG
                //db.SaveChanges();

                message.Body = "OTP CỦA BẠN LÀ " + otp;

                try
                {
                    smtpClient.Send(message);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"SMTP Error: {ex.StatusCode}");
                    Console.WriteLine($"Message: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            return RedirectToAction("XacNhanOTP", "Customer");

        }
        public ActionResult XacNhanOTP()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XacNhanOTP(string Maotp)
        {
            string otp = Session["otp"] as string;
            if (otp == Maotp)
            {
                return RedirectToAction("MatKhauMoi");

            }
            else
            {
                ModelState.AddModelError(Maotp, "Mã OTP KHÔNG ĐÚNG");
            }
            return View();

        }

        public ActionResult MatKhauMoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MatKhauMoi(KHACHHANG kh, string XacNhanMK)
        {
            HashMD hs = new HashMD();
            if (kh.Password_hs5 == XacNhanMK)
            {
                var khachHangInfo = TempData["KhachHangInfo"] as KHACHHANG;

                if (khachHangInfo != null)
                {
                    KHACHHANG kkk = db.KHACHHANG.Where(row => row.Email == khachHangInfo.Email).FirstOrDefault();
                    kkk.Password_hs5 = hs.CalculateMD5Hash(kh.Password_hs5);
                    db.KHACHHANG.Add(kh);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Customer");

                }

            }
            return View();
        }


    }
}