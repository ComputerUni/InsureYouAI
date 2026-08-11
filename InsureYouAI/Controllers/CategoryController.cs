using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace InsureYouAI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly InsureContext _context;
        public CategoryController(InsureContext context)
        {
            _context = context;
        }

        public IActionResult CategoryList(int page = 1)
        {
            ViewBag.ControllerName = "Kategoriler";
            ViewBag.PageName = "Kategori Listesi";
            var categories = _context.Categories.ToList();
            return View(categories.ToPagedList(page, 8));
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            var value = _context.Categories.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        public IActionResult DeleteCategory(int id)
        {
            var value = _context.Categories.Find(id);
            _context.Categories.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
    }
}
