using System.ComponentModel.DataAnnotations;

namespace RecipesApp.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int Time { get; set; }
        public string? ImageUrl { get; set; }

        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
        public List<Step> Steps { get; set; } = new();
    }
}
