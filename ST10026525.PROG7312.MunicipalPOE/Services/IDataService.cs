using ST10026525.PROG7312.MunicipalPOE.Models;
using ST10026525.PROG7312.MunicipalPOE.DataStructures;

namespace ST10026525.PROG7312.MunicipalPOE.Services
{
    public interface IDataService
    {
        // Part 1: Reports
        List<Report> Reports { get; }
        void AddReport(Report report);
        IEnumerable<string> GetCategories();

        // Part 2: Events
        SortedDictionary<DateTime, List<Event>> EventsByDate { get; }
        HashSet<string> EventCategories { get; }
        Queue<Event> UpcomingEventQueue { get; }

        void AddEvent(Event ev);
        void DeleteEvent(Event ev);
        List<Event> GetAllEvents();
        List<Event> SearchEvents(string keyword);
        List<Event> RecommendEvents();

        // Part 3: Service Requests 
        BSTree<ServiceRequest> RequestsById { get; }
        AVLTree<ServiceRequest> RequestsByDate { get; }
        Graph<ServiceRequest> RequestsGraph { get; }
        MinHeap<ServiceRequest> PriorityQueue { get; }
        Dictionary<string, List<ServiceRequest>> RequestsByCategory { get; }
        Dictionary<string, List<ServiceRequest>> RequestsByStatus { get; }

        void AddRequest(ServiceRequest request);
        List<ServiceRequest> GetAllRequests();
        ServiceRequest? SearchById(Guid id);
        List<ServiceRequest> GetRequestsByDateRange(DateTime start, DateTime end);
        List<ServiceRequest> GetRequestsByCategory(string category);
        List<ServiceRequest> GetRequestsByStatus(string status);
        ServiceRequest? GetHighestPriorityRequest();
        void UpdateRequestStatus(Guid id, string newStatus);
    }
}

