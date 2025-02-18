namespace RecipesApp.Models
{
    public class RecipeSearchViewModel
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public string? ImageUrl { get; set; }
        public List<IngredientSelection> SelectedIngredients { get; set; } = new List<IngredientSelection>();
        public List<Recipe> Results { get; set; } = new List<Recipe>();
        public string SortOrder { get; set; }
    }

    public class IngredientSelection
    {
        public string Name { get; set; }
        public double Quantity { get; set; } = 1;
        public string Unit { get; set; }
    }
}
