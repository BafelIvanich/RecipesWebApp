using System.ComponentModel.DataAnnotations;

namespace RecipesApp.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        public string Name { get;set; }

        public string Unit { get; set; }

        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();

    }
}
