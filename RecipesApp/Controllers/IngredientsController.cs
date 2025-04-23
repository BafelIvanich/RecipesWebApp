using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;

namespace RecipesApp.Controllers
{
    public class IngredientsController : Controller
    {
                private readonly ApplicationDbContext _context;


        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;

        }
        // GET: IngredientsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: IngredientsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: IngredientsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IngredientsController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {

            if (ModelState.IsValid)
            {
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
            }

            return View(ingredient);
        }


        // GET: IngredientsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IngredientsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: IngredientsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IngredientsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult ViewIngredients()
        {
            var ingredients = _context.Ingredients.ToList();
            return View(ingredients);
        }
    }
}
