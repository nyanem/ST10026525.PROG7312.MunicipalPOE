
using ST10026525.PROG7312.MunicipalPOE.DataStructures;
using ST10026525.PROG7312.MunicipalPOE.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST10026525.PROG7312.MunicipalPOE.Services
{
    public class MunicipalDataService : IDataService
    {
        private readonly object _lock = new();

        // ================= Part 1: Reports =================
        private readonly List<Report> _reports = new();
        private readonly HashSet<string> _categories = new(StringComparer.OrdinalIgnoreCase)
        {
            "Roads", "Water", "Electricity", "Sanitation", "Other"
        };

        public List<Report> Reports => _reports;

        public void AddReport(Report report)
        {
            report.Id = _reports.Count > 0 ? _reports.Max(r => r.Id) + 1 : 1;
            report.SubmittedAt = DateTime.Now;
            _reports.Add(report);
        }

        public IEnumerable<string> GetCategories() => _categories.OrderBy(c => c);

        // ================= Part 2: Events =================
        public SortedDictionary<DateTime, List<Event>> EventsByDate { get; } = new();
        public HashSet<string> EventCategories { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Queue<Event> UpcomingEventQueue { get; } = new();
        private readonly Dictionary<string, int> _searchCounts = new(StringComparer.OrdinalIgnoreCase);

        public void AddEvent(Event ev)
        {
            lock (_lock)
            {
                if (!EventsByDate.ContainsKey(ev.Date.Date))
                    EventsByDate[ev.Date.Date] = new List<Event>();

                EventsByDate[ev.Date.Date].Add(ev);

                if (ev.Date.Date >= DateTime.Today)
                    UpcomingEventQueue.Enqueue(ev);

                EventCategories.Add(ev.Category);
            }
        }

        public void DeleteEvent(Event ev)
        {
            lock (_lock)
            {
                if (EventsByDate.ContainsKey(ev.Date.Date))
                {
                    EventsByDate[ev.Date.Date].Remove(ev);
                    if (EventsByDate[ev.Date.Date].Count == 0)
                        EventsByDate.Remove(ev.Date.Date);
                }

                var tempQueue = new Queue<Event>();
                while (UpcomingEventQueue.Count > 0)
                {
                    var e = UpcomingEventQueue.Dequeue();
                    if (e.Id != ev.Id)
                        tempQueue.Enqueue(e);
                }
                while (tempQueue.Count > 0)
                    UpcomingEventQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        public List<Event> SearchEvents(string keyword)
        {
            lock (_lock)
            {
                keyword = keyword?.Trim() ?? "";
                if (string.IsNullOrEmpty(keyword))
                    return GetAllEvents();

                if (!_searchCounts.ContainsKey(keyword))
                    _searchCounts[keyword] = 0;
                _searchCounts[keyword]++;

                return EventsByDate.Values
                    .SelectMany(list => list)
                    .Where(e => e.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || e.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(e => e.Date)
                    .ToList();
            }
        }

        public List<Event> GetAllEvents()
        {
            lock (_lock)
            {
                return EventsByDate.Values.SelectMany(list => list).OrderBy(e => e.Date).ToList();
            }
        }

        public List<Event> RecommendEvents()
        {
            lock (_lock)
            {
                if (_searchCounts.Count == 0) return new List<Event>();

                var topKeywords = _searchCounts.OrderByDescending(kv => kv.Value)
                                               .Take(3)
                                               .Select(kv => kv.Key)
                                               .ToList();

                var recommended = new List<Event>();
                foreach (var kw in topKeywords)
                    recommended.AddRange(SearchEvents(kw));

                return recommended.Distinct().OrderBy(e => e.Date).ToList();
            }
        }

        // ================= Part 3: Service Requests =================
        public BSTree<ServiceRequest> RequestsById { get; } = new();
        public AVLTree<ServiceRequest> RequestsByDate { get; } = new();
        public Graph<ServiceRequest> RequestsGraph { get; } = new();
        public MinHeap<ServiceRequest> PriorityQueue { get; }

        public Dictionary<string, List<ServiceRequest>> RequestsByCategory { get; } = new();
        public Dictionary<string, List<ServiceRequest>> RequestsByStatus { get; } = new(StringComparer.OrdinalIgnoreCase);

        public MunicipalDataService()
        {
            // Initialize MinHeap with custom comparer
            PriorityQueue = new MinHeap<ServiceRequest>((a, b) => b.Priority.CompareTo(a.Priority));

            // Seed sample data
            SeedSampleEvents();
            SeedSampleRequests();
        }

        public void AddRequest(ServiceRequest request)
        {
            lock (_lock)
            {
                RequestsById.Insert(request.Id, request);
                RequestsByDate.Insert(request.DateSubmitted, request);
                PriorityQueue.Insert(request);

                if (!RequestsByCategory.ContainsKey(request.Category))
                    RequestsByCategory[request.Category] = new List<ServiceRequest>();
                RequestsByCategory[request.Category].Add(request);

                if (!RequestsByStatus.ContainsKey(request.Status))
                    RequestsByStatus[request.Status] = new List<ServiceRequest>();
                RequestsByStatus[request.Status].Add(request);

                RequestsGraph.AddNode(request);
            }
        }

        public List<ServiceRequest> GetAllRequests() => RequestsById.InOrderTraversal();
        public ServiceRequest? SearchById(Guid id) => RequestsById.Search(id);
        public List<ServiceRequest> GetRequestsByDateRange(DateTime start, DateTime end) => RequestsByDate.RangeQuery(start, end);
        public List<ServiceRequest> GetRequestsByCategory(string category) => RequestsByCategory.ContainsKey(category) ? RequestsByCategory[category] : new List<ServiceRequest>();
        public List<ServiceRequest> GetRequestsByStatus(string status) => RequestsByStatus.ContainsKey(status) ? RequestsByStatus[status] : new List<ServiceRequest>();
        public ServiceRequest? GetHighestPriorityRequest() => PriorityQueue.Peek();

        public void UpdateRequestStatus(Guid id, string newStatus)
        {
            var req = SearchById(id);
            if (req == null) return;

            if (RequestsByStatus.ContainsKey(req.Status))
                RequestsByStatus[req.Status].Remove(req);

            req.Status = newStatus;

            if (!RequestsByStatus.ContainsKey(newStatus))
                RequestsByStatus[newStatus] = new List<ServiceRequest>();
            RequestsByStatus[newStatus].Add(req);
        }

        // ================= Seed Methods =================
        private void SeedSampleRequests()
        {
            for (int i = 1; i <= 15; i++)
            {
                var req = new ServiceRequest
                {
                    Title = $"Request {i}",
                    Description = $"This is request number {i}",
                    Category = i % 3 == 0 ? "Plumbing" : i % 3 == 1 ? "Electricity" : "Roads",
                    Priority = 16 - i,
                    Status = i % 2 == 0 ? "Pending" : "In Progress",
                    DateSubmitted = DateTime.Now.AddDays(-i),
                    Location = i % 2 == 0 ? "Sector A" : "Sector B"
                };
                AddRequest(req);
            }

            var all = GetAllRequests();
            if (all.Count >= 3)
                RequestsGraph.AddEdge(all[0], all[1])
                             .AddEdge(all[1], all[2]);
        }

        private void SeedSampleEvents()
        {
            var sampleEvents = new List<Event>
            {
                new() { Id = Guid.NewGuid(), Title = "City Cleanup Drive", Category = "Community", Date = DateTime.Today.AddDays(3), Location = "Union Building", Description = "Join us to clean up the park and surroundings." },
                new() { Id = Guid.NewGuid(), Title = "Blood Donation Camp", Category = "Health", Date = DateTime.Today.AddDays(7), Location = "Community Hall", Description = "Donate blood and save lives." },
                new() { Id = Guid.NewGuid(), Title = "Food Drive for Homeless", Category = "Charity", Date = DateTime.Today.AddDays(1), Location = "Sammy Marks Square", Description = "Help distribute food to the needy." },
                new() { Id = Guid.NewGuid(), Title = "Local Farmers Market", Category = "Market", Date = DateTime.Today.AddDays(5), Location = "Market Street", Description = "Fresh produce from local farmers." },
                new() { Id = Guid.NewGuid(), Title = "Art & Craft Exhibition", Category = "Culture", Date = DateTime.Today.AddDays(10), Location = "Art Museum", Description = "Display of local art and crafts." },
                new() { Id = Guid.NewGuid(), Title = "Neighborhood Watch Meeting", Category = "Safety", Date = DateTime.Today.AddDays(2), Location = "Community Hall", Description = "Discuss safety strategies with local police." },
                new() { Id = Guid.NewGuid(), Title = "Summer Coding Bootcamp", Category = "Education", Date = DateTime.Today.AddDays(14), Location = "Recreational Center", Description = "Learn coding basics in 2 weeks." },
                new() { Id = Guid.NewGuid(), Title = "Marathon for Charity", Category = "Sports", Date = DateTime.Today.AddDays(8), Location = "Lodium Stadium", Description = "Run to support local charities." },
                new() { Id = Guid.NewGuid(), Title = "Music in the Park", Category = "Entertainment", Date = DateTime.Today.AddDays(6), Location = "City Park", Description = "Local bands performing live." },
                new() { Id = Guid.NewGuid(), Title = "Community Health Checkup", Category = "Health", Date = DateTime.Today.AddDays(4), Location = "Any clinic", Description = "Free health screening for residents." },
                new() { Id = Guid.NewGuid(), Title = "Environmental Awareness Workshop", Category = "Education", Date = DateTime.Today.AddDays(9), Location = "Museum", Description = "Learn how to reduce your carbon footprint." },
                new() { Id = Guid.NewGuid(), Title = "Book Fair", Category = "Culture", Date = DateTime.Today.AddDays(12), Location = "Library of South Africa", Description = "Books for all ages and genres." },
                new() { Id = Guid.NewGuid(), Title = "Community Yoga Session", Category = "Health", Date = DateTime.Today.AddDays(11), Location = "Jan Cilliers Park", Description = "Morning yoga session for beginners." },
                new() { Id = Guid.NewGuid(), Title = "Pet Adoption Day", Category = "Charity", Date = DateTime.Today.AddDays(15), Location = "SPCA", Description = "Find your furry friend." },
                new() { Id = Guid.NewGuid(), Title = "Tech Talk: Smart Cities", Category = "Education", Date = DateTime.Today.AddDays(13), Location = "Community Hall", Description = "Discussion on technology and city planning." }
            };

            foreach (var e in sampleEvents)
                AddEvent(e);
        }
    }
}
