using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopThoiTrang.Models;
using ShopThoiTrang.App_Start;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace ShopThoiTrang.Areas.Admin.Controllers
{
    public class ColorController : Controller
    {
        // GET: Admin/Color
        QL_SHOPTHOITRANG_DOANEntities db = new QL_SHOPTHOITRANG_DOANEntities();

        //5	Thêm_Mau
        //6	Xóa_Màu
        //7	Sửa_Màu 
        //8	Xem_Mau


        //8	Xem_Mau
        [AdminAuthorize(idChucNang = 8)]

        public ActionResult Show(string searching)
        {
            return View(db.MauSac.Where(x => x.TenMau.Contains(searching) || searching == null).ToList());
        }


        //5	Thêm_Mau
        [AdminAuthorize(idChucNang = 5)]

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MauSac color)
        {
            if (ModelState.IsValid)
            {
                db.MauSac.Add(color);
                db.SaveChanges();
                return RedirectToAction("Show");
            }
            return View(color);
        }


        //7	Sửa_Màu 
        [AdminAuthorize(idChucNang = 7)]

        public ActionResult Edit(int id = 0)
        {
            MauSac color = db.MauSac.Single(d => d.MaMau == id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }
        [HttpPost]
        public ActionResult Edit(MauSac color)
        {
            if (ModelState.IsValid)
            {
                db.MauSac.Attach(color);
                db.Entry(color).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Show", "Color");
            }
            return View(color);
        }


        //6	Xóa_Màu
        [AdminAuthorize(idChucNang = 6)]

        [HttpPost]
        public ActionResult DeleteSelected(IEnumerable<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var itemId in selectedItems)
                {
                    // Kiểm tra xem có sản phẩm nào sử dụng MaDanhMuc này không
                    var isUsed = db.ChiTietSanPham.Any(sp => sp.MaMau == itemId);

                    if (isUsed)
                    {
                        // Hiển thị thông báo không thể xóa nếu MaDanhMuc này được sử dụng trong sản phẩm
                        TempData["ErrorMessage"] = "Không thể xóa màu  này vì có sản phẩm sử dụng.";
                        return RedirectToAction("Show");
                    }
                }

                // Tiến hành xóa nếu không có sản phẩm nào sử dụng MaDanhMuc này
                var listColor = db.MauSac.Where(x => selectedItems.Contains(x.MaMau)).ToList();
                db.MauSac.RemoveRange(listColor);
                db.SaveChanges();
            }

            return RedirectToAction("Show");
        }

    }
}