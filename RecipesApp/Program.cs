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
                new Ingredient { Name = "����", Unit = "������" },
                new Ingredient { Name = "�����", Unit = "������" },
                new Ingredient { Name = "����", Unit = "������ �����" },
                new Ingredient { Name = "����", Unit = "�����" },
                new Ingredient { Name = "������", Unit = "����������" },
                new Ingredient { Name = "�������", Unit = "������" },
                new Ingredient { Name = "������", Unit = "�����" },
                new Ingredient { Name = "�������", Unit = "������" },
                new Ingredient { Name = "���������", Unit = "�����" },
                new Ingredient { Name = "�������", Unit = "�����" },
                new Ingredient { Name = "���", Unit = "�����" },
                new Ingredient { Name = "��������", Unit = "������" },
                new Ingredient { Name = "�������� �����", Unit = "�������� �����" },
                new Ingredient { Name = "�������", Unit = "������" },
                new Ingredient { Name = "������ ������������", Unit = "�����" },
                new Ingredient { Name = "�������", Unit = "������" }
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
                    Name = "�����",
                    Instructions = "1. ������� ����, ������, ����, ���� � �����\n2. ������ �� ������ ���������� ��������� � ���� ������",
                    Time = 30
                },
                new Recipe
                {
                    Name = "����",
                    Instructions = "1. ������� ������ �� ��������\n2. �������� ���������� �����\n3. ������ �� ���������� 1.5 ����",
                    Time = 90
                },
                new Recipe
                {
                    Name = "������",
                    Instructions = "1. �������� ����� � ����\n2. �������� ��� ����������� ��������\n3. ������� � ���������",
                    Time = 60
                }
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();

            var ingredients = context.Ingredients.ToDictionary(i => i.Name, i => i.Id);

            var recipeIngredients = new List<RecipeIngredient>
            {
                // �����
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["����"], Quantity = 300, Unit = "������" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["������"], Quantity = 500, Unit = "����������" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["����"], Quantity = 3, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[0].Id, IngredientId = ingredients["����"], Quantity = 0.5, Unit = "������ �����" },

                // ����
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["��������"], Quantity = 500, Unit = "������" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["������"], Quantity = 2, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["�������"], Quantity = 300, Unit = "������" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["���������"], Quantity = 4, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["�������"], Quantity = 1, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["���"], Quantity = 1, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[1].Id, IngredientId = ingredients["�������� �����"], Quantity = 2, Unit = "�������� �����" },

                // ������
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["���������"], Quantity = 4, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["�������"], Quantity = 2, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["�������"], Quantity = 200, Unit = "������" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["������ ������������"], Quantity = 3, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["����"], Quantity = 4, Unit = "�����" },
                new RecipeIngredient { RecipeId = recipes[2].Id, IngredientId = ingredients["�������"], Quantity = 100, Unit = "������" }
            };

            context.RecipeIngredients.AddRange(recipeIngredients);
            context.SaveChanges();
        }
    }
}
