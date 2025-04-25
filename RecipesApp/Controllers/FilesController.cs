using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipesApp.Data;
using RecipesApp.Models;
using RecipesApp.Extensions;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using RecipesApp.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace RecipesApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string pythonPath = null;
        private readonly string pythonScriptPath = null;
        private readonly ApplicationDbContext _context;


        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger,IWebHostEnvironment env, ApplicationDbContext context)
        {
            _logger = logger;
            _env = env;
            _context = context;


            pythonPath = Path.Combine(_env.ContentRootPath, "PythonScripts", "venv","Scripts","python.exe");
            pythonScriptPath = Path.Combine(_env.ContentRootPath, "PythonScripts","process_image.py");

            if (!System.IO.File.Exists(pythonScriptPath))
            {
                 _logger.LogWarning("Python script not found at configured path: {ScriptPath}", pythonScriptPath);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFridgeImage(IFormFile fridgeImage)
        {
            if (fridgeImage == null || fridgeImage.Length == 0)
            {
                TempData["UploadError"] = "Пожалуйста, выберите файл изображения";
                return RedirectToAction("Fridge","Recipes");
            }
                string tempFilePath = Guid.NewGuid().ToString() + Path.GetExtension(fridgeImage.FileName);
                string pythonResult = null;
                string pythonError = null;

            try
            {

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await fridgeImage.CopyToAsync(stream);
                }
                _logger.LogInformation($"Изображение {fridgeImage.FileName}");

                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    ArgumentList = { pythonScriptPath, tempFilePath },
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };
                _logger.LogInformation($"Скрипт запущен");

                startInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";

                using (var process = Process.Start(startInfo))
                {
                    if (process == null) throw new InvalidOperationException("ошибка запуска");

                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();
                    bool exited = await Task.Run(() => process.WaitForExit(60000));

                    if (!exited)
                    {
                        _logger.LogWarning("таймаут");
                        try { process.Kill(true); } catch { }
                        throw new TimeoutException($"Время обработки запроса истекло");
                    }

                    if (process.ExitCode != 0)
                    {
                        return RedirectToAction("Fridge", "Recipes");
                    }

                    pythonResult = await outputTask;
                    pythonError = await errorTask;

                    if (!string.IsNullOrEmpty(pythonError)) _logger.LogWarning($"python errors:{pythonError}");
                    if (!string.IsNullOrEmpty(pythonResult)) _logger.LogInformation($"python script returned:{pythonResult}");

                    var ingredientItems = pythonResult.Split(',');
                    if (ingredientItems.Contains("Ничего"))
                    {
                        return RedirectToAction("Fridge", "Recipes");
                    }
                    var dbIngredients = await _context.Ingredients
                        .Select(i => i.Name.ToLowerInvariant())
                        .ToListAsync();
                    var dbIngredientsSet = new HashSet<string>(dbIngredients);

                    List<string> matched = new List<string>();
                    List<string> unmatched = new List<string>();

                    foreach(var item in ingredientItems)
                    {
                        if (string.IsNullOrWhiteSpace(item)) continue;

                        string formattedItem = item.Trim().ToLowerInvariant();
                        if (dbIngredientsSet.Contains(formattedItem))
                        {
                            var matchingItem = await _context.Ingredients
                                .Where(i => i.Name != null && i.Name.ToLower().Contains(formattedItem))
                                .Select(i => i.Name)
                                .FirstOrDefaultAsync();
                            matched.Add(matchingItem ?? item.Trim());
                        }
                        else
                        {
                            unmatched.Add(item.Trim());
                        }
                    }

                    _logger.LogInformation($"Matched:{matched.Count}");
                    _logger.LogInformation($"Unmatched:{unmatched.Count}");

                    TempData["MatchedIngredients"] = JsonSerializer.Serialize(matched);
                    TempData["UnmatchedIngredients"] = JsonSerializer.Serialize(unmatched);
                    TempData["Processed"] = true;
                }


                return RedirectToAction("Fridge", "Recipes");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Fridge", "Recipes");
            }
            finally
            {
                if (tempFilePath != null && System.IO.File.Exists(tempFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(tempFilePath);
                        _logger.LogInformation($"изображение {tempFilePath} удалено");
                    }
                    catch (IOException ioEx)
                    {
                        _logger.LogInformation($"{ioEx}");
                    }
                }

            }

            
        }
        public async Task<bool> DoesIngredientExist(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string formatedName = name.Trim().ToLowerInvariant();

            bool exists = await _context.Ingredients.AnyAsync(ing => ing.Name.ToLower() == formatedName);

            return exists;
        }

    }
}
