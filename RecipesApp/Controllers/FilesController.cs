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
                    pythonResult = await outputTask;
                    pythonError = await errorTask;

                    if (!string.IsNullOrEmpty(pythonError)) _logger.LogWarning($"python errors:{pythonError}");
                    if (!string.IsNullOrEmpty(pythonResult)) _logger.LogInformation($"python script returned:{pythonResult}");
                    string outputResult = pythonResult.Trim();
                    string errorResult = pythonError.Trim();
                    _logger.LogInformation($"получено: {outputResult}");
                    _logger.LogInformation($"ошибка: {errorResult}");
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

    }
}
