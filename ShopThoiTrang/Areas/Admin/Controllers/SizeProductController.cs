using ShopThoiTrang.App_Start;
using ShopThoiTrang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class SizeProductController : Controller
    {
        // GET: Admin/SizeProduct
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();
        //9	    Xem_Size
        //10	Them_size
        //11	Xoa_size
        //12	Sua_size

        //9	    Xem_Size
        [AdminAuthorize(idChucNang = 9)]

        public ActionResult Show(string searching)
        {
            return View(db.KichThuoc.Where(x => x.TenKichThuoc.Contains(searching) || searching == null).ToList());
        }

        //10	Them_size
        [AdminAuthorize(idChucNang = 10)]

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KichThuoc size)
        {
            if (ModelState.IsValid)
            {
                db.KichThuoc.Add(size);
                db.SaveChanges();
                return RedirectToAction("Show");
            }
            return View(size);
        }

        //12	Sua_size
        [AdminAuthorize(idChucNang = 12)]
        public ActionResult Edit(int id = 0)
        {
            KichThuoc size = db.KichThuoc.Single(d => d.MaKichThuoc == id);
            if (size == null)
            {
                return HttpNotFound();
            }
            return View(size);
        }
        [HttpPost]
        public ActionResult Edit(KichThuoc size)
        {
            if (ModelState.IsValid)
            {
                db.KichThuoc.Attach(size);
                db.Entry(size).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Show", "Color");
            }
            return View(size);
        }


        //11	Xoa_size
        [AdminAuthorize(idChucNang = 11)]
        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Kiểm tra xem có sản phẩm nào sử dụng MaDanhMuc này không
                    var isUsed = db.ChiTietSanPham.Any(sp => sp.MaKichThuoc == itemId);

                    if (isUsed)
                    {
                        // Hiển thị thông báo không thể xóa nếu MaDanhMuc này được sử dụng trong sản phẩm
                        TempData["ErrorMessage"] = "Không thể xóa màu  này vì có sản phẩm sử dụng.";
                        return RedirectToAction("Show");
                    }
                }

                // Tiến hành xóa nếu không có sản phẩm nào sử dụng MaDanhMuc này
                var listSize = db.KichThuoc.Where(x => selectedItems.Contains(x.MaKichThuoc)).ToList();
                db.KichThuoc.RemoveRange(listSize);
                db.SaveChanges();
            }

            return RedirectToAction("Show");
        }
    }
}