using Microsoft.AspNetCore.Mvc;
using ST10026525.PROG7312.MunicipalPOE.Models;
using ST10026525.PROG7312.MunicipalPOE.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ST10026525.PROG7312.MunicipalPOE.Controllers
{
    public class ReportController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IWebHostEnvironment _env;

        public ReportController(IDataService dataService, IWebHostEnvironment env)
        {
            _dataService = dataService;
            _env = env;
        }

        [HttpGet]
        public IActionResult ReportForm()
        {
            ViewBag.Categories = _dataService.GetCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportForm(Report model, IFormFile? attachment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _dataService.GetCategories();
                return View(model);
            }

            if (attachment != null && attachment.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var filename = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
                var filePath = Path.Combine(uploadsPath, filename);

                await using var stream = System.IO.File.Create(filePath);
                await attachment.CopyToAsync(stream);

                model.MediaFileName = $"/uploads/{filename}";
            }

    
            _dataService.AddReport(model);

            TempData["Success"] = "Report submitted successfully!";
            return RedirectToAction("ViewReports");
        }

        [HttpGet]
        public IActionResult ViewReports()
        {
            var reports = _dataService.Reports.OrderByDescending(r => r.SubmittedAt).ToList();
            ViewBag.Success = TempData["Success"];
            return View(reports);
        }
    }
}
