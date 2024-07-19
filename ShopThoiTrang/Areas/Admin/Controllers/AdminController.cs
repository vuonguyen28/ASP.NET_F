//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using ShopThoiTrang.Models;

//namespace ShopThoiTrang.Areas.Admin.Controllers
//{
//    public class AdminController : Controller
//    {
//        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
//        // GET: Admin/Admin
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult ShowAdmin(string searching)
//        {
            
//            return View(db.NHANVIEN.Where(x => x.HoTenNV.Contains(searching) || searching == null).ToList());
//        }

//        public ActionResult CreateAdmin()
//        {
//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult CreateAdmin(NHANVIEN nhanvien)
//        {
//            if (ModelState.IsValid)
//            {
//                db.NHANVIEN.Add(nhanvien);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(nhanvien);
//        }
//        public ActionResult EditAdmin(int id = 0)
//        {
//            NHANVIEN cate = db.NHANVIEN.Single(d => d.MaNV == id);
//            if (cate == null)
//            {
//                return HttpNotFound();
//            }
//            return View(cate);
//        }
//        [HttpPost]
//        public ActionResult EditAdmin(NHANVIEN nhanvien)
//        {
//            if (ModelState.IsValid)
//            {
//                db.NHANVIEN.Attach(nhanvien);
//                db.Entry(nhanvien).State = System.Data.Entity.EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index", "Category");
//            }
//            return View(nhanvien);
//        }
//    }
//}