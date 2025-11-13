# Municipal Application – README
## Project Overview

The Municipal Application is a web-based system designed to manage local municipal services, announcements, events, reports, and service requests.
The application demonstrates manual implementation of advanced data structures for educational purposes, without relying on  repositories or databases.

The application is built using ASP.NET Core MVC, and all data is stored in-memory for simplicity and reliability. The project is organized into three main tasks:

1. Reports Management

2. Events & Announcements Management

3. Service Requests Management

## Features 
### Task 1 - Report
- Submit reports regarding municipal issues (e.g., roads, electricity, sanitation).

- Categories managed using a HashSet for uniqueness.

- Reports automatically assigned IDs and timestamps.

- View all submitted reports.

### Task 2 - Events & Announcements 

- Add and display events in a SortedDictionary<DateTime, List<Event>>.

- Upcoming events managed with a Queue.

- Categories stored in a HashSet.

- Search and filter events by title, description, category, or date.

- Recommendations generated based on previous searches.
  

