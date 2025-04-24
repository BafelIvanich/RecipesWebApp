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

namespace RecipesApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly string pythonPath = "/usr/bin/python3";
        private readonly string pythonScriptPath = "/path/";

        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFridgeImage(IFormFile fridgeImage)
        {
            if (fridgeImage == null || fridgeImage.Length == 0)
            {
                TempData["UploadError"] = "Пожалуйста, выберите файл изображения";
                return RedirectToAction("Fridge","Recipes");
            }
            try
            {
                _logger.LogInformation($"Изображение {fridgeImage.FileName}");
                return RedirectToAction("Fridge", "Recipes");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Fridge", "Recipes");
            }
        }

    }
}
