﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;

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
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        public IActionResult Search(string query,
            int? cookingTime,
            List<string> ingredients)
        {
            var model = new RecipeSearchViewModel
            {
                Name = query,
                Time = cookingTime ?? 0,
                SelectedIngredients = ingredients?.Select(i => new IngredientSelection { Name = i}).ToList()
                ?? new List<IngredientSelection>()
            };

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
                foreach(var ingredientName in ingredients)
                {
                    var lowerName = ingredientName.ToLower();
                    recipeQuery = recipeQuery.Where(r => r.RecipeIngredients
                    .Any(ri => ri.Ingredient.Name.ToLower() == lowerName));
                }
            }

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

            model.Results = recipeQuery.ToList();

            return View(model);
            
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RecipeSearchViewModel model)
        {
            
            return View(model);
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
                .Where(i => i.Name.ToLower().Contains(term.ToLower())) // Case-insensitive
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

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        public IActionResult ViewIngredients()
        {
            var ingredients = _context.Ingredients.ToList();
            return View(ingredients);
        }

    }
}
