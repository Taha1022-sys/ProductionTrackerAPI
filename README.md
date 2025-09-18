# ProductionTrackerAPI

ProductionTrackerAPI is a .NET 9 Web API project designed to track and manage production entries efficiently. It provides endpoints for creating, reading, updating, and deleting production data, as well as exporting data to Excel.

## Features
- RESTful API for production management
- Export production data to Excel
- Environment-based configuration

## Technologies Used

### .NET 9 & ASP.NET Core Web API
- The project is built on .NET 9 and uses ASP.NET Core to provide RESTful API endpoints for production management.
- Controllers (see `Controllers/ProductionController.cs`) define the API endpoints and handle HTTP requests.

### Dependency Injection
- Services are registered and injected using ASP.NET Core's built-in dependency injection system.
- Example: `IProductionService` is implemented by `ExcelProductionService` and registered in `Program.cs`:
  ```csharp
  builder.Services.AddScoped<IProductionService, ExcelProductionService>();
  ```

### Excel Integration (ClosedXML)
- Production data is stored and managed in an Excel file using the [ClosedXML](https://github.com/ClosedXML/ClosedXML) library.
- The `ExcelProductionService` handles all CRUD operations by reading/writing to `ProductionEntries.xlsx` in the `Data` folder.

### Configuration
- Application settings (e.g., logging, allowed hosts) are managed via `appsettings.json`.
- Environment variables and URLs are set in `Properties/launchSettings.json`.

### CORS & Static Files
- CORS is enabled for all origins, methods, and headers for easy API access during development.
- Static file serving is enabled to allow access to files in the `wwwroot` or other static directories.

### Swagger (OpenAPI)
- Swagger is enabled in development for interactive API documentation and testing.

## Requirements
- .NET 9 SDK

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd <project-directory>
   ```
2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
3. **Build the project:**
   ```bash
   dotnet build
   ```
4. **Run the project:**
   ```bash
   dotnet run
   ```
   The API will be available at the URL specified in `Properties/launchSettings.json`.

## Configuration
- Update `appsettings.json` for database and other settings.
- Use `Properties/launchSettings.json` to configure environment variables and URLs.

## API Endpoints
- See the `Controllers/ProductionController.cs` for available endpoints.

## License
This project is licensed under the MIT License.

## Contact
For questions or support, contact the maintainer.


# ProductionTrackerAPI

ProductionTrackerAPI, üretim girişlerini verimli bir şekilde takip ve yönetmek için geliştirilmiş bir .NET 9 Web API projesidir. Üretim verilerini oluşturma, okuma, güncelleme, silme ve Excel'e aktarma gibi işlevler sunar.

## Özellikler
- Üretim yönetimi için RESTful API
- Üretim verilerini Excel'e aktarma
- Ortama göre yapılandırma

## Kullanılan Teknolojiler

### .NET 9 & ASP.NET Core Web API
- Proje, .NET 9 üzerinde geliştirilmiş olup, üretim yönetimi için RESTful API uç noktaları ASP.NET Core ile sunulmaktadır.
- API uç noktaları ve HTTP istekleri `Controllers/ProductionController.cs` dosyasında tanımlanır.

### Bağımlılık Enjeksiyonu (Dependency Injection)
- Servisler, ASP.NET Core'un yerleşik bağımlılık enjeksiyonu sistemiyle kaydedilir ve enjekte edilir.
- Örnek: `IProductionService` arayüzü, `ExcelProductionService` ile `Program.cs` dosyasında aşağıdaki gibi kaydedilir:
  ```csharp
  builder.Services.AddScoped<IProductionService, ExcelProductionService>();
  ```

### Excel Entegrasyonu (ClosedXML)
- Üretim verileri, [ClosedXML](https://github.com/ClosedXML/ClosedXML) kütüphanesi kullanılarak Excel dosyasında saklanır ve yönetilir.
- `ExcelProductionService`, tüm CRUD işlemlerini `Data` klasöründeki `ProductionEntries.xlsx` dosyası üzerinden gerçekleştirir.

### Yapılandırma
- Uygulama ayarları (ör. loglama, izin verilen hostlar) `appsettings.json` dosyası ile yönetilir.
- Ortam değişkenleri ve URL'ler `Properties/launchSettings.json` dosyasında tanımlanır.

### CORS & Statik Dosyalar
- Geliştirme sırasında API'ye kolay erişim için tüm origin, method ve header'lara izin veren CORS yapılandırması aktiftir.
- Statik dosya servisi ile `wwwroot` veya diğer statik dizinlerdeki dosyalara erişim sağlanır.

### Swagger (OpenAPI)
- Geliştirme ortamında Swagger ile etkileşimli API dokümantasyonu ve test imkanı sunulur.

## Gereksinimler
- .NET 9 SDK

## Başlarken

1. **Depoyu klonlayın:**
   ```bash
   git clone <repository-url>
   cd <proje-dizini>
   ```
2. **Bağımlılıkları yükleyin:**
   ```bash
   dotnet restore
   ```
3. **Projeyi derleyin:**
   ```bash
   dotnet build
   ```
4. **Projeyi çalıştırın:**
   ```bash
   dotnet run
   ```
   API, `Properties/launchSettings.json` dosyasında belirtilen adreste çalışacaktır.

## Yapılandırma
- Veritabanı ve diğer ayarlar için `appsettings.json` dosyasını güncelleyin.
- Ortam değişkenleri ve URL'ler için `Properties/launchSettings.json` dosyasını kullanın.

## API Uç Noktaları
- Kullanılabilir uç noktalar için `Controllers/ProductionController.cs` dosyasına bakabilirsiniz.

## Lisans
Bu proje MIT Lisansı ile lisanslanmıştır.

## İletişim
Sorularınız veya destek için proje sahibiyle iletişime geçebilirsiniz.
