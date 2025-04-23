using Microsoft.EntityFrameworkCore;
using RecipesApp.Controllers;
using RecipesApp.Data;
using RecipesApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Seed(app);





app.Run();


//static void Seed(IHost app)
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var services = scope.ServiceProvider;
//        try
//        {
//            var context = services.GetRequiredService<ApplicationDbContext>();

//            // Ensure the database is created
//            context.Database.EnsureCreated();

//            // **Step 1: Seed Tags**
//            // Define all unique tags used across recipes
//            var tagNames = new List<string>
//            {
//                "Завтрак", "Обед", "Ужин", "Паста", "Быстро", "Десерт", "Выпечка", "Шоколад"
//            };
//            foreach (var tagName in tagNames)
//            {
//                if (!context.Tags.Any(t => t.Name == tagName))
//                {
//                    context.Tags.Add(new Tag { Name = tagName });
//                }
//            }
//            context.SaveChanges();

//            // **Step 2: Seed Ingredients**
//            // Define all unique ingredients with their units
//            var ingredientsData = new List<(string Name, string Unit)>
//            {
//                ("Макароны", "гр."),
//                ("Бекон", "гр."),
//                ("Яйцо", "шт."),
//                ("Сыр", "гр."),
//                ("Перец черный молотый", "гр."),
//                ("Молоко", "мл"),
//                ("Петрушка", "гр."),
//                ("Соль", "чайные ложки"),
//                ("Сливочное масло", "гр."),
//                ("Мука пшеничная", "гр."),
//                ("Сахар", "гр."),
//                ("Какао", "гр."),
//                ("Масло подсолнечное", "мл"),
//                ("Разрыхлитель теста", "чайные ложки")
//            };
//            foreach (var (name, unit) in ingredientsData)
//            {
//                if (!context.Ingredients.Any(i => i.Name == name))
//                {
//                    context.Ingredients.Add(new Ingredient { Name = name, Unit = unit });
//                }
//            }
//            context.SaveChanges();

//            // **Step 3: Seed Recipes**
//            // Recipe 1: Carbonara
//            var recipeCarbonara = new Recipe
//            {
//                Name = "Carbonara",
//                Time = 30,
//                ImageUrl = "carbonara.jpg",
//                Tags = context.Tags.Where(t => new[] { "Обед", "Ужин", "Паста" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "Сварить макароны согласно инструкции на упаковке.", StepOrder = 1 },
//                    new Step { Description = "Обжарить бекон до хрустящей корочки.", StepOrder = 2 },
//                    new Step { Description = "Смешать яйца, сыр и перец в миске.", StepOrder = 3 },
//                    new Step { Description = "Смешать макароны, бекон и яичную смесь.", StepOrder = 4 },
//                    new Step { Description = "Подавать немедленно.", StepOrder = 5 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Макароны"), Quantity = 200, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Бекон"), Quantity = 100, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Яйцо"), Quantity = 2, Unit = "шт." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Сыр"), Quantity = 50, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Перец черный молотый"), Quantity = 1, Unit = "гр." }
//                }
//            };

//            // Recipe 2: Omelette with Cheese and Herbs
//            var recipeOmelette = new Recipe
//            {
//                Name = "Омлет с сыром и зеленью",
//                Time = 15,
//                ImageUrl = "omelette.jpg",
//                Tags = context.Tags.Where(t => new[] { "Завтрак", "Быстро" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "Взбить яйца с молоком, солью и перцем.", StepOrder = 1 },
//                    new Step { Description = "Натереть сыр, мелко нарезать зелень.", StepOrder = 2 },
//                    new Step { Description = "Растопить масло на сковороде, вылить яичную смесь.", StepOrder = 3 },
//                    new Step { Description = "Готовить на среднем огне 5 минут, посыпать сыром и зеленью.", StepOrder = 4 },
//                    new Step { Description = "Сложить омлет пополам и подавать горячим.", StepOrder = 5 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Яйцо"), Quantity = 3, Unit = "шт." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Молоко"), Quantity = 50, Unit = "мл" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Сыр"), Quantity = 50, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Петрушка"), Quantity = 10, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Соль"), Quantity = 2, Unit = "чайные ложки" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Перец черный молотый"), Quantity = 1, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Сливочное масло"), Quantity = 10, Unit = "гр." }
//                }
//            };

//            // Recipe 3: Chocolate Muffin
//            var recipeMuffin = new Recipe
//            {
//                Name = "Шоколадный маффин",
//                Time = 50,
//                ImageUrl = "muffin.jpg",
//                Tags = context.Tags.Where(t => new[] { "Десерт", "Выпечка", "Шоколад" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "Разогреть духовку до 180°C и подготовить формочки для маффинов.", StepOrder = 1 },
//                    new Step { Description = "Смешать муку, сахар, какао и разрыхлитель.", StepOrder = 2 },
//                    new Step { Description = "Добавить яйцо, молоко и масло, замесить тесто.", StepOrder = 3 },
//                    new Step { Description = "Разложить тесто по формочкам и выпекать 20 минут.", StepOrder = 4 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Мука пшеничная"), Quantity = 150, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Сахар"), Quantity = 100, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Какао"), Quantity = 20, Unit = "гр." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Яйцо"), Quantity = 1, Unit = "шт." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Молоко"), Quantity = 100, Unit = "мл" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Масло подсолнечное"), Quantity = 50, Unit = "мл" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "Разрыхлитель теста"), Quantity = 1, Unit = "чайные ложки" }
//                }
//            };

//            // Add all recipes to the context
//            context.Recipes.AddRange(recipeCarbonara, recipeOmelette, recipeMuffin);

//            // Save all changes to the database
//            context.SaveChanges();
//        }
//        catch (Exception ex)
//        {
//            // Handle the exception (e.g., log it in a real application)
//            return;
//        }
//    }
//    return;
//}