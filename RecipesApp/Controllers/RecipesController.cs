using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;
using RecipesApp.Extensions;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace RecipesApp.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes.ToListAsync();
            return View(await _context.Recipes.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Steps)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        public IActionResult Search(
            string query,
            int? cookingTime,
            List<string> ingredients,
            List<double> quantities,
            List<string> units,
            string sortOrder,
            bool useFridge = false)
        {
            var model = new RecipeSearchViewModel
            {
                Name = query,
                Time = cookingTime ?? 0,
                SelectedIngredients = ingredients?.Select(i => new IngredientSelection { Name = i}).ToList()
                ?? new List<IngredientSelection>(),
                SortOrder = sortOrder
            };

            //fridge setcion
            if (useFridge)
            {
                var fridge = HttpContext.Session.Get<List<IngredientViewModel>>("Fridge");
                if(fridge!=null && fridge.Any())
                {
                    ingredients = fridge.Select(i => i.Name).ToList();
                    model.SelectedIngredients = fridge.Select(i => new IngredientSelection
                    {
                        Name = i.Name,
                        Unit = i.Unit,
                        Quantity = 1
                    }).ToList();
                }
            }

            var recipeQuery = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                recipeQuery = recipeQuery.Where(r => r.Name.ToLower().Contains(query.ToLower()));
            }

            if (cookingTime.HasValue)
            {
                recipeQuery = recipeQuery.Where(r => r.Time <= cookingTime.Value);
            }

            if(ingredients != null && ingredients.Any())
            {
                model.SelectedIngredients = ingredients
            .Zip(quantities, (name, qty) => new { name, qty })
            .Zip(units, (temp, unit) => new IngredientSelection
            {
                Name = temp.name,
                Quantity = temp.qty,
                Unit = unit
            })
            .ToList();

                var ingredientNames = ingredients.Select(i => i.ToLower()).ToList();
                recipeQuery = recipeQuery.Where(r => r.RecipeIngredients
                .Any(ri => ingredientNames.Contains(ri.Ingredient.Name.ToLower())));

                //foreach(var ingredientName in ingredients)
                //{
                //    var lowerName = ingredientName.ToLower();
                //    recipeQuery = recipeQuery.Where(r => r.RecipeIngredients
                //    .Any(ri => ri.Ingredient.Name.ToLower() == lowerName));
                //}
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                recipeQuery = sortOrder switch
                {
                    "time_asc" => recipeQuery.OrderBy(r => r.Time),
                    "time_desc" => recipeQuery.OrderByDescending(r => r.Time),
                    _ => recipeQuery
                };
            }
            else
            {
                if (string.IsNullOrEmpty(query))
                {
                    recipeQuery = recipeQuery
                        .OrderByDescending(r => r.RecipeIngredients
                        .Count(ri => ingredients.Contains(ri.Ingredient.Name)))
                        .ThenBy(r => r.Time);
                }
                else
                {
                    recipeQuery = recipeQuery
                        .OrderBy(r => r.Name)
                        .ThenBy(r => r.Time);
                }
            }



            model.Results = recipeQuery
            .AsEnumerable()
            .Select(r => new RecipeSearchResult
            {
                Recipe = r,
                MatchedIngredients = r.RecipeIngredients
                .Where(ri => ingredients.Contains(ri.Ingredient.Name))
                .Select(ri => ri.Ingredient.Name)
                .ToList(),
                MissingIngredients = r.RecipeIngredients
                .Where(ri => !ingredients.Contains(ri.Ingredient.Name))
                .Select(ri => ri.Ingredient.Name)
                .ToList()
            })
            .OrderByDescending(r => r.MatchedIngredients.Count)
            .ThenBy(r => r.MissingIngredients.Count)
            .ThenBy(r => r.Recipe.Time)
            .ToList();

            return View(model);
            
        }

        public IActionResult Fridge()
        {
            var fridgeIngredients = HttpContext.Session.Get<List<IngredientViewModel>>("Fridge")
                ?? new List<IngredientViewModel>();
            return View(fridgeIngredients);
        }

        [HttpPost]
        public IActionResult AddToFridge(string name, string unit)
        {
            var fridge = HttpContext.Session.Get<List<IngredientViewModel>>("Fridge")
                ?? new List<IngredientViewModel>();

            if (!fridge.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                fridge.Add(new IngredientViewModel
                {
                    Name = name,
                    Unit = unit
                });
                HttpContext.Session.Set("Fridge", fridge);
            }

            return RedirectToAction("Fridge");
        }

        [HttpPost]
        public IActionResult RemoveFromFridge(string name)
        {
            var fridge = HttpContext.Session.Get<List<IngredientViewModel>>("Fridge");
            if(fridge != null)
            {
                fridge.RemoveAll(i => i.Name.Equals(name,StringComparison.OrdinalIgnoreCase));
                HttpContext.Session.Set("Fridge", fridge);
            }
            return RedirectToAction("Fridge");
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Recipe recipe)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
            }

            return View(recipe);
        }


        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> SearchIngredients(string term)
        {
            var ingredients = await _context.Ingredients
                .Where(i => i.Name.ToLower().Contains(term.ToLower())) 
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    Name = i.Name,
                    Unit = i.Unit
                })
                .Take(10)
                .ToListAsync();

            return Ok(ingredients);
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int recipeId)
        {
            var favorites = HttpContext.Session.Get<List<int>>("Favorites") ?? new List<int>();

            if (favorites.Contains(recipeId))
            {
                favorites.Remove(recipeId);
            }
            else
            {
                favorites.Add(recipeId);
            }

            HttpContext.Session.Set("Favorites", favorites);
            return RedirectToAction("Details", new { id = recipeId });
        }

        public IActionResult Favorites()
        {
            var favorites = HttpContext.Session.Get<List<int>>("Favorites") ?? new List<int>();
            var recipes = _context.Recipes
                .Where(r => favorites.Contains(r.Id))
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToList();

            return View(recipes);
        }


        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Instructions,Time")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CookRecipe(int id)
        {
            var recipe = _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefault(r => r.Id == id);

            if(recipe == null)
            {
                return NotFound();
            }

            var fridge = HttpContext.Session.Get<List<IngredientViewModel>>("Fridge") ?? new List<IngredientViewModel>();

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var fridgeIngredient = fridge.FirstOrDefault(f => f.Name == recipeIngredient.Ingredient.Name);

                if (fridgeIngredient!= null)
                {
                    fridge.Remove(fridgeIngredient);
                }
            }
            HttpContext.Session.Set("Fridge", fridge);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult TestSession()
        {
            //Test
            var id = 23;

            var favorites = HttpContext.Session.Get<List<int>>("Favorites") ?? new List<int>();
            if (!favorites.Contains(id))
            {
                favorites.Add(id);
                HttpContext.Session.Set("Favorites", favorites);
            }

            return RedirectToAction("Favorites");
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        

    }
}
