# Production Tracker API

A modern .NET 9 Web API for tracking production data using Excel as a database backend.

## Features

- ?? **Production Entry Management**: Create, read, update, and delete production entries
- ?? **Statistics Dashboard**: Get comprehensive production statistics and analytics  
- ?? **Filtering & Pagination**: Filter entries by machine, shift, date range with pagination support
- ?? **RESTful API**: Clean REST endpoints with proper HTTP status codes
- ?? **Excel Integration**: Uses Excel files for data persistence with ClosedXML
- ?? **Swagger Documentation**: Interactive API documentation
- ?? **CORS Support**: Configured for cross-origin requests

## Technology Stack

- **.NET 9**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful web services
- **ClosedXML**: Excel file manipulation
- **Swagger/OpenAPI**: API documentation
- **Excel Database**: Uses Excel files for data storage

## Project Structure

```
ProductionTrackerAPI/
??? Controllers/
?   ??? ProductionController.cs    # Main API controller
??? Models/
?   ??? ProductionEntry.cs         # Production entry model
?   ??? ProductionEntryDto.cs      # Data transfer object
??? Services/
?   ??? IProductionService.cs      # Service interface
?   ??? ExcelProductionService.cs  # Excel-based service implementation
??? Properties/
?   ??? launchSettings.json        # Launch configuration
??? Data/                          # Excel data files (auto-created)
??? Program.cs                     # Application entry point
??? appsettings.json              # Configuration
??? ProductionTrackerAPI.csproj   # Project file
```

## API Endpoints

### Production Entries
- `POST /api/production/entries` - Create new production entry
- `GET /api/production/entries` - Get all entries with filtering and pagination
- `GET /api/production/entries/{id}` - Get specific entry by ID
- `PUT /api/production/entries/{id}` - Update existing entry
- `DELETE /api/production/entries/{id}` - Delete entry

### Analytics & Utilities
- `GET /api/production/entries/summary` - Get summary list of entries
- `GET /api/production/statistics` - Get production statistics
- `GET /api/production/machines` - Get list of machine numbers
- `GET /api/production/excel-info` - Get Excel file information

## Query Parameters

For the GET `/api/production/entries` endpoint:
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10)
- `machineNo` (string): Filter by machine number
- `shift` (int): Filter by shift number
- `startDate` (DateTime): Filter entries from this date
- `endDate` (DateTime): Filter entries until this date

## Production Entry Model

```json
{
  "id": 1,
  "date": "2024-01-15T00:00:00",
  "machineNo": "M001",
  "shift": 1,
  "modelNo": "MODEL123",
  "sizeNo": "SIZE456",
  "formCount": 100,
  "defect1": 2,
  "defect2": 1,
  "defect3": 0,
  "defect4": 1,
  "defect5": 0,
  "defect6": 0,
  "defect7": 0,
  "defect8": 0,
  "defect9": 0,
  "defect10": 0,
  "totalDefects": 4,
  "generalErrorRate": 4.0,
  "createdAt": "2024-01-15T10:30:00"
}
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository:
```bash
git clone https://github.com/Taha1022-sys/ProductionTrackerAPI.git
cd ProductionTrackerAPI
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5002`
- Swagger UI: `http://localhost:5002` (in development mode)

### Development Configuration

The application includes two launch profiles:
- **http**: Runs on `http://0.0.0.0:5002` with browser launch
- **network**: Runs on `http://172.20.10.7:5002` without browser launch

## Data Storage

The application uses Excel files for data persistence:
- Data is stored in the `Data/` folder
- Primary file: `ProductionEntries.xlsx`
- Files are automatically created when needed
- Excel format allows easy data inspection and backup

## Configuration

### CORS
The application is configured to accept requests from any origin in development mode. Modify the CORS policy in `Program.cs` for production use.

### Logging
Standard ASP.NET Core logging is configured. Modify `appsettings.json` to adjust log levels.

## Development

### Adding New Features
1. Update the model classes in `Models/`
2. Extend the service interface `IProductionService`
3. Implement the functionality in `ExcelProductionService`
4. Add new controller actions as needed

### Testing
Access the Swagger UI at the root URL when running in development mode to test all endpoints interactively.

## Deployment

### Production Considerations
1. Update CORS policy for production domains
2. Configure proper logging levels
3. Set up proper error handling
4. Consider database migration from Excel to a proper database for large-scale use
5. Configure HTTPS redirection properly

### Docker Support
The application can be containerized. Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductionTrackerAPI.dll"]
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is open source. See the LICENSE file for details.

## Contact

For questions or support, please open an issue on GitHub.