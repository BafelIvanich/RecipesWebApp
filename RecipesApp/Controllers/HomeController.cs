using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;
using RecipesApp.ViewModels;

namespace RecipesApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {

        _context = context;
    }

    public async Task<IActionResult> Index(string? selectedTag = null)
    {
        //var recipes = await _context.Recipes
        //    .Include(r => r.RecipeIngredients)
        //    .ThenInclude(ri => ri.Ingredient)
        //    .ToListAsync();

        var tags = await _context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();

        IQueryable<Recipe> recipesQuery = _context.Recipes
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Tags);

        if (!string.IsNullOrEmpty(selectedTag))
        {
            recipesQuery = recipesQuery
                .Where(r => r.Tags.Any(t => t.Name == selectedTag));
        }

        var recipes = await recipesQuery
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        var viewModel = new HomeViewModel
        {
            Recipes = recipes,
            Tags = tags,
            SelectedTag = selectedTag
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> RecipeList(string? selectedTag = null)
    {
        IQueryable<Recipe> recipesQuery = _context.Recipes
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Tags);

        if (!string.IsNullOrEmpty(selectedTag))
        {
            string comparisonTag = selectedTag.ToUpper();

            recipesQuery = recipesQuery
            .Where(recipe => recipe.Tags 
                        .Any(tag => tag.Name.ToUpper() == comparisonTag));
        }

        var recipes = await recipesQuery
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        return PartialView("_RecipeList", recipes);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
