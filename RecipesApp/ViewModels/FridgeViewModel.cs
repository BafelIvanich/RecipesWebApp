namespace RecipesApp.ViewModels
{
    public class FridgeViewModel
    {
        public List<IngredientViewModel> fridgeItems { get; set; } = new List<IngredientViewModel>();
        public List<string>? matching { get; set; } = new List<string>();
        public List<string>? missing { get; set; } = new List<string>();
    }
}
