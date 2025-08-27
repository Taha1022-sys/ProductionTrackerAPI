# Production Tracker API

A .NET 9 Web API for tracking production data, defects, and generating reports.

## Features

- Production entry management with photo uploads
- Defect tracking and error rate calculations
- Time-limited editing capabilities (1 hour after creation)
- Production summary reports
- Date range filtering
- Photo management for production entries

## Technologies

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB or full instance)

### Installation

1. Clone the repository
```bash
git clone https://github.com/Taha1022-sys/ProductionTrackerAPI.git
cd ProductionTrackerAPI
```

2. Update the connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductionTrackerDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. Run database migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

The API will be available at `http://localhost:5197` and Swagger UI at the root URL.

## API Endpoints

### Production Entries
- `GET /api/production` - Get all production entries
- `GET /api/production/{id}` - Get production entry by ID
- `POST /api/production` - Create new production entry
- `PUT /api/production/{id}` - Update production entry (time-limited)
- `GET /api/production/{id}/editability` - Check if entry can be edited
- `GET /api/production/date-range` - Get entries by date range

### Reports
- `GET /api/production/summary` - Get production summary
- `GET /api/production/created-date-range` - Get entries by creation date range

## Models

### ProductionEntry
Main production data model including:
- Machine and shift information
- Production counts and defects
- Error rates and calculations
- Photo path and notes
- Creation and update timestamps

### ProductionSummary
Aggregated production statistics including:
- Total production counts
- Error counts and rates by type
- Overall error percentages

## Business Rules

- Production entries can only be edited for 1 hour after creation
- Photos are automatically managed (upload/delete)
- Error rates are automatically calculated
- Production summaries are updated after each entry modification

## File Structure

```
ProductionTrackerAPI/
??? Controllers/         # API controllers
??? Data/               # Database context
??? Migrations/         # EF Core migrations
??? Models/             # Data models and DTOs
??? Services/           # Business logic services
??? Program.cs          # Application entry point
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.