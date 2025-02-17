namespace RecipesApp.Models
{
    public class RecipeSearchViewModel
    {
        public string Name { get; set; }
        public int Time { get;set; }
        public string? ImageUrl { get; set; }
        public List<IngredientSelection> SelectedIngredients { get; set; } = new List<IngredientSelection>();
        public List<Recipe> Results { get; set; } = new List<Recipe>();
    }

    public class IngredientSelection
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
    }
}
