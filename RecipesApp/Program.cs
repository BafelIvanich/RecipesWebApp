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
//                "�������", "����", "����", "�����", "������", "������", "�������", "�������"
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
//                ("��������", "��."),
//                ("�����", "��."),
//                ("����", "��."),
//                ("���", "��."),
//                ("����� ������ �������", "��."),
//                ("������", "��"),
//                ("��������", "��."),
//                ("����", "������ �����"),
//                ("��������� �����", "��."),
//                ("���� ���������", "��."),
//                ("�����", "��."),
//                ("�����", "��."),
//                ("����� ������������", "��"),
//                ("������������ �����", "������ �����")
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
//                Tags = context.Tags.Where(t => new[] { "����", "����", "�����" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "������� �������� �������� ���������� �� ��������.", StepOrder = 1 },
//                    new Step { Description = "�������� ����� �� ��������� �������.", StepOrder = 2 },
//                    new Step { Description = "������� ����, ��� � ����� � �����.", StepOrder = 3 },
//                    new Step { Description = "������� ��������, ����� � ������ �����.", StepOrder = 4 },
//                    new Step { Description = "�������� ����������.", StepOrder = 5 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "��������"), Quantity = 200, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "�����"), Quantity = 100, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����"), Quantity = 2, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "���"), Quantity = 50, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����� ������ �������"), Quantity = 1, Unit = "��." }
//                }
//            };

//            // Recipe 2: Omelette with Cheese and Herbs
//            var recipeOmelette = new Recipe
//            {
//                Name = "����� � ����� � �������",
//                Time = 15,
//                ImageUrl = "omelette.jpg",
//                Tags = context.Tags.Where(t => new[] { "�������", "������" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "������ ���� � �������, ����� � ������.", StepOrder = 1 },
//                    new Step { Description = "�������� ���, ����� �������� ������.", StepOrder = 2 },
//                    new Step { Description = "��������� ����� �� ���������, ������ ������ �����.", StepOrder = 3 },
//                    new Step { Description = "�������� �� ������� ���� 5 �����, �������� ����� � �������.", StepOrder = 4 },
//                    new Step { Description = "������� ����� ������� � �������� �������.", StepOrder = 5 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����"), Quantity = 3, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "������"), Quantity = 50, Unit = "��" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "���"), Quantity = 50, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "��������"), Quantity = 10, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����"), Quantity = 2, Unit = "������ �����" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����� ������ �������"), Quantity = 1, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "��������� �����"), Quantity = 10, Unit = "��." }
//                }
//            };

//            // Recipe 3: Chocolate Muffin
//            var recipeMuffin = new Recipe
//            {
//                Name = "���������� ������",
//                Time = 50,
//                ImageUrl = "muffin.jpg",
//                Tags = context.Tags.Where(t => new[] { "������", "�������", "�������" }.Contains(t.Name)).ToList(),
//                Steps = new List<Step>
//                {
//                    new Step { Description = "��������� ������� �� 180�C � ����������� �������� ��� ��������.", StepOrder = 1 },
//                    new Step { Description = "������� ����, �����, ����� � ������������.", StepOrder = 2 },
//                    new Step { Description = "�������� ����, ������ � �����, �������� �����.", StepOrder = 3 },
//                    new Step { Description = "��������� ����� �� ��������� � �������� 20 �����.", StepOrder = 4 }
//                },
//                RecipeIngredients = new List<RecipeIngredient>
//                {
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "���� ���������"), Quantity = 150, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "�����"), Quantity = 100, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "�����"), Quantity = 20, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����"), Quantity = 1, Unit = "��." },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "������"), Quantity = 100, Unit = "��" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "����� ������������"), Quantity = 50, Unit = "��" },
//                    new RecipeIngredient { Ingredient = context.Ingredients.First(i => i.Name == "������������ �����"), Quantity = 1, Unit = "������ �����" }
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