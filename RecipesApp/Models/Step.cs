using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesApp.Models
{
    public class Step
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public int StepOrder { get; set; }

        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
    }
}
