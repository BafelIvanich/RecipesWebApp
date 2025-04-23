using System.ComponentModel.DataAnnotations;

namespace RecipesApp.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(16)]
        public string Name { get; set; }

        public List<Recipe> Recipes { get; set; } = new();
    }
}
