using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Seed(app);



app.Run();


static void Seed(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated(); // Ensures the database exists

        // Seed ingredients if table is empty
        if (!context.Ingredients.Any())
        {
            var ingredients = new List<Ingredient>
        {
            new Ingredient { Name = "Flour", Unit = "grams" },
            new Ingredient { Name = "Sugar", Unit = "cups" },
            new Ingredient { Name = "Salt", Unit = "teaspoons" },
            new Ingredient { Name = "Butter", Unit = "tablespoons" },
            new Ingredient { Name = "Milk", Unit = "cups" },
            new Ingredient { Name = "Eggs", Unit = "pieces" },
            new Ingredient { Name = "Baking Powder", Unit = "teaspoons" },
            new Ingredient { Name = "Vanilla Extract", Unit = "teaspoons" },
            new Ingredient { Name = "Cocoa Powder", Unit = "tablespoons" }
        };
            context.Ingredients.AddRange(ingredients);
            context.SaveChanges();
        }

        // Seed multiple recipes if none exist
        if (!context.Recipes.Any())
        {
            var recipes = new List<Recipe>
        {
            new Recipe { Name = "Pancakes", Instructions = "Mix ingredients and cook in a pan.", Time = 15 },
            new Recipe { Name = "Chocolate Cake", Instructions = "Mix, bake at 180°C for 30 min.", Time = 45 },
            new Recipe { Name = "French Toast", Instructions = "Dip bread in egg mixture and fry.", Time = 10 }
        };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();

            // Fetch ingredients
            var ingredientsDict = context.Ingredients.ToDictionary(i => i.Name, i => i.Id);

            // Seed Recipe-Ingredient relationships
            var recipeIngredients = new List<RecipeIngredient>
        {
            // Pancakes
            new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredientsDict["Flour"], Quantity = 200, Unit = "grams" },
            new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredientsDict["Milk"], Quantity = 1, Unit = "cups" },
            new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredientsDict["Eggs"], Quantity = 2, Unit = "pieces" },

            // Chocolate Cake
            new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredientsDict["Flour"], Quantity = 250, Unit = "grams" },
            new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredientsDict["Sugar"], Quantity = 1, Unit = "cups" },
            new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredientsDict["Cocoa Powder"], Quantity = 3, Unit = "tablespoons" },

            // French Toast
            new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredientsDict["Eggs"], Quantity = 2, Unit = "pieces" },
            new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredientsDict["Milk"], Quantity = 1, Unit = "cups" },
            new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredientsDict["Butter"], Quantity = 1, Unit = "tablespoons" }
        };

            context.RecipeIngredients.AddRange(recipeIngredients);
            context.SaveChanges();
        }
    }

}
