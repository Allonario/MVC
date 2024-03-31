using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using MVC.Models.VueModels;

namespace MVCWebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {

            ProductVm productVm = new()
            {
                CategoryList = _unitOfWork.CategoryRepository
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                return View(productVm);
            }
            else
            {
                productVm.Product = _unitOfWork.ProductRepository.Get(u=>u.Id == id);
                return View(productVm);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVm productVm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"img\product");

                    if(!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVm.Product.ImageUrl = @"\img\product\" + fileName;
                }
                if(productVm.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVm.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVm.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVm.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVm);
            }
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    ProductVm productVm = new()
        //    {
        //        CategoryList = _unitOfWork.CategoryRepository
        //        .GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),
        //        Product = _unitOfWork.ProductRepository.Get(c => c.Id == id)
        //    };
        //    if (productVm.Product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productVm);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    if (!string.IsNullOrEmpty(product.ImageUrl))
        //    {
        //        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
        //        if (System.IO.File.Exists(oldImagePath))
        //        {
        //            System.IO.File.Delete(oldImagePath);
        //        }
        //    }
        //    _unitOfWork.ProductRepository.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Category deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            Product? product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (product == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Error while deleting" });
        }
        #endregion
    }
}
