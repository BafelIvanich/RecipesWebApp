using RecipesApp.Models;

namespace RecipesApp.ViewModels
{
    public class HomeViewModel
    {
        public List<Recipe> Recipes { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public string? SelectedTag { get; set; }
    }
}
