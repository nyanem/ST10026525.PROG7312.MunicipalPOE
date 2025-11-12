using Microsoft.AspNetCore.Mvc;
using ST10026525.PROG7312.MunicipalPOE.Models;
using ST10026525.PROG7312.MunicipalPOE.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST10026525.PROG7312.MunicipalPOE.Controllers
{
    public class EventsController : Controller
    {
        private readonly IDataService _data;

        public EventsController(IDataService data)
        {
            _data = data;
        }

        // Main page showing events and recommended events
        public IActionResult Index()
        {
            var allEvents = _data.GetAllEvents();
            ViewBag.Categories = _data.EventCategories.ToList();
            ViewBag.Recommended = _data.RecommendEvents(); // based on user search patterns

            return View(allEvents);
        }

        // AJAX search endpoint
        public IActionResult Search(string q, string category, string sortBy, DateTime? from, DateTime? to)
        {
            var events = _data.GetAllEvents();

            // Keyword search
            if (!string.IsNullOrWhiteSpace(q))
            {
                events = _data.SearchEvents(q);
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                events = events.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Date filter
            if (from.HasValue)
                events = events.Where(e => e.Date >= from.Value).ToList();

            if (to.HasValue)
                events = events.Where(e => e.Date <= to.Value).ToList();

            // Sorting
            events = sortBy switch
            {
                "date" => events.OrderBy(e => e.Date).ToList(),
                "date_desc" => events.OrderByDescending(e => e.Date).ToList(),
                "name" => events.OrderBy(e => e.Title).ToList(),
                "category" => events.OrderBy(e => e.Category).ToList(),
                _ => events
            };

            return Json(events);
        }

        // Admin dashboard (no login required)
        public IActionResult Dashboard()
        {
            var events = _data.GetAllEvents();
            return View(events);
        }

        // Create Event (GET)
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // Create Event (POST)
        [HttpPost]
        public IActionResult CreateEvent(Event e)
        {
            if (ModelState.IsValid)
            {
                _data.AddEvent(e);
                return RedirectToAction("Dashboard");
            }
            return View(e);
        }

        // Delete Event
        [HttpPost]
        public IActionResult DeleteEvent(Guid id)
        {
            var ev = _data.GetAllEvents().FirstOrDefault(x => x.Id == id);
            if (ev != null)
            {
                _data.DeleteEvent(ev);
            }
            return RedirectToAction("Dashboard");
        }
    }
}

