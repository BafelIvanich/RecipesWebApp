namespace RecipesApp.Models
{
    public class RecipeSearchViewModel
    {
        public string Name { get; set; }
        public int Time { get; set; }
        public string? ImageUrl { get; set; }
        public List<IngredientSelection> SelectedIngredients { get; set; } = new List<IngredientSelection>();
        public List<RecipeSearchResult> Results { get; set; } = new();
        public string SortOrder { get; set; }
    }

    public class IngredientSelection
    {
        public string Name { get; set; }
        public double Quantity { get; set; } = 1;
        public string Unit { get; set; }
    }

    public class RecipeSearchResult
    {
        public Recipe Recipe { get; set; }
        public List<string> MatchedIngredients { get; set; } = new();
        public List<string> MissingIngredients { get; set; } = new();
    }
}
