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
        context.Database.EnsureCreated();
        if (!context.Ingredients.Any())
        {
            var ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "Мука", Unit = "граммы" },
                new Ingredient { Name = "Сахар", Unit = "граммы" },
                new Ingredient { Name = "Соль", Unit = "чайные ложки" },
                new Ingredient { Name = "Яйца", Unit = "штуки" },
                new Ingredient { Name = "Молоко", Unit = "миллилитры" },
                new Ingredient { Name = "Сметана", Unit = "граммы" },
                new Ingredient { Name = "Свекла", Unit = "штуки" },
                new Ingredient { Name = "Капуста", Unit = "граммы" },
                new Ingredient { Name = "Картофель", Unit = "штуки" },
                new Ingredient { Name = "Морковь", Unit = "штуки" },
                new Ingredient { Name = "Лук", Unit = "штуки" },
                new Ingredient { Name = "Говядина", Unit = "граммы" },
                new Ingredient { Name = "Томатная паста", Unit = "столовые ложки" },
                new Ingredient { Name = "Колбаса", Unit = "граммы" },
                new Ingredient { Name = "Огурцы маринованные", Unit = "штуки" },
                new Ingredient { Name = "Майонез", Unit = "граммы" }
            };
            context.Ingredients.AddRange(ingredients);
            context.SaveChanges();
        }
        if (!context.Recipes.Any())
        {
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Name = "Блины",
                    Instructions = "1. Смешать муку, молоко, яйца, соль и сахар\n2. Жарить на хорошо разогретой сковороде с двух сторон",
                    Time = 30
                },
                new Recipe
                {
                    Name = "Борщ",
                    Instructions = "1. Сварить бульон из говядины\n2. Добавить нарезанные овощи\n3. Варить до готовности 1.5 часа",
                    Time = 90
                },
                new Recipe
                {
                    Name = "Оливье",
                    Instructions = "1. Отварить овощи и яйца\n2. Нарезать все ингредиенты кубиками\n3. Смешать с майонезом",
                    Time = 60
                }
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();

            var ingredients = context.Ingredients.ToDictionary(i => i.Name, i => i.Id);

            var recipeIngredients = new List<RecipeIngredient>
            {
                // Блины
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["Мука"], Quantity = 300, Unit = "граммы" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["Молоко"], Quantity = 500, Unit = "миллилитры" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["Яйца"], Quantity = 3, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["Соль"], Quantity = 0.5, Unit = "чайные ложки" },

                // Борщ
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Говядина"], Quantity = 500, Unit = "граммы" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Свекла"], Quantity = 2, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Капуста"], Quantity = 300, Unit = "граммы" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Картофель"], Quantity = 4, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Морковь"], Quantity = 1, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Лук"], Quantity = 1, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["Томатная паста"], Quantity = 2, Unit = "столовые ложки" },

                // Оливье
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Картофель"], Quantity = 4, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Морковь"], Quantity = 2, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Колбаса"], Quantity = 200, Unit = "граммы" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Огурцы маринованные"], Quantity = 3, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Яйца"], Quantity = 4, Unit = "штуки" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["Майонез"], Quantity = 100, Unit = "граммы" }
            };

            context.RecipeIngredients.AddRange(recipeIngredients);
            context.SaveChanges();
        }
    }
}
