using Microsoft.AspNetCore.Mvc;
using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;

namespace MVCWebApplication.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _cotegoryRepository;
        public CategoryController(ICategoryRepository cotegoryRepository)
        {
            _cotegoryRepository = cotegoryRepository;    
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _cotegoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _cotegoryRepository.Add(category);
                _cotegoryRepository.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _cotegoryRepository.Get(c => c.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _cotegoryRepository.Update(category);
                _cotegoryRepository.Save();
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _cotegoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? category = _cotegoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _cotegoryRepository.Remove(category);
            _cotegoryRepository.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
