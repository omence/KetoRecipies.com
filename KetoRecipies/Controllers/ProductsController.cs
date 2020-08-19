using KetoRecipies.Data;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using X.PagedList;

namespace KetoRecipies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly KetoDbContext _context;
        public ProductsController(
            KetoDbContext context)
        {
            _context = context;
        }
        // GET: Products
        [AllowAnonymous]
        public ActionResult Index(string filter, int? page)
        {
            var products = _context.Products.ToList();

            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "Cooking Gear":
                        products = products.Where(p => p.ProductType == "Cooking Gear").ToList();
                        break;
                    case "Supplements":
                        products = products.Where(p => p.ProductType == "Supplements").ToList();
                        break;
                    case "Food":
                        products = products.Where(p => p.ProductType == "Food").ToList();
                        break;
                    case "Swag":
                        products = products.Where(p => p.ProductType == "Swag").ToList();
                        break;
                }
            }

            TempData["filter"] = filter;
            int pageSize = 27;
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult ManageProducts()
        {
            
            return View(_context.Products.ToList());
        }
        // GET: Products/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("ProductUrl,ImageUrl,ProductType")] Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("ManageProducts");
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {

            return View(_context.Products.FirstOrDefault(p => p.Id == id));
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,ProductUrl,ImageUrl,ProductType")] Product product)
        {
            try
            {
                _context.Products.Update(product);
                _context.SaveChanges();

                return RedirectToAction("ManageProducts");
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_context.Products.FirstOrDefault(p => p.Id == id));
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Product toRemove = _context.Products.FirstOrDefault(p => p.Id == id);
                _context.Products.Remove(toRemove);
                _context.SaveChanges();

                return RedirectToAction("ManageProducts");
            }
            catch
            {
                return View();
            }
        }
    }
}