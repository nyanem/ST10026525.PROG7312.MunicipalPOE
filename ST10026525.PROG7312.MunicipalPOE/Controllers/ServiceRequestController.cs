using Microsoft.AspNetCore.Mvc;
using ST10026525.PROG7312.MunicipalPOE.Models;
using ST10026525.PROG7312.MunicipalPOE.Services;

namespace ST10026525.PROG7312.MunicipalPOE.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IDataService _dataService;

        public ServiceRequestController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // =================== VIEWS ===================

        // Show the service request form
        public IActionResult ServiceRequestForm()
        {
            ViewBag.Categories = _dataService.RequestsByCategory.Keys.OrderBy(c => c);
            return View();
        }

        // Handle request submission
        [HttpPost]
        public IActionResult ServiceRequestForm(ServiceRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            request.Id = Guid.NewGuid();
            request.DateSubmitted = DateTime.Now;
            request.Status ??= "Pending";

            _dataService.AddRequest(request);

            TempData["Success"] = "✅ Service request submitted successfully!";
            return RedirectToAction("ViewRequests");
        }

        // Display all requests (with optional filters)
        public IActionResult ViewRequests(string searchId, string category, string status, DateTime? from, DateTime? to)
        {
            var requests = _dataService.GetAllRequests();

            if (!string.IsNullOrEmpty(searchId) && Guid.TryParse(searchId, out Guid id))
            {
                var match = _dataService.SearchById(id);
                requests = match != null ? new List<ServiceRequest> { match } : new List<ServiceRequest>();
            }
            else if (!string.IsNullOrEmpty(category))
            {
                requests = _dataService.GetRequestsByCategory(category);
            }
            else if (!string.IsNullOrEmpty(status))
            {
                requests = _dataService.GetRequestsByStatus(status);
            }
            else if (from.HasValue || to.HasValue)
            {
                requests = _dataService.GetRequestsByDateRange(from ?? DateTime.MinValue, to ?? DateTime.MaxValue);
            }

            ViewBag.Categories = _dataService.RequestsByCategory.Keys.OrderBy(c => c);
            ViewBag.Statuses = _dataService.RequestsByStatus.Keys.OrderBy(s => s);

            return View(requests);
        }

        // Update request status (from dropdown)
        [HttpPost]
        public IActionResult UpdateStatus(Guid id, string newStatus)
        {
            _dataService.UpdateRequestStatus(id, newStatus);
            return RedirectToAction("ViewRequests");
        }
    }
}
