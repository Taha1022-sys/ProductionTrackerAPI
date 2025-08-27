using ProductionTrackerAPI.Models;

namespace ProductionTrackerAPI.Services
{
    public interface IProductionService
    {
        Task<ProductionEntry> CreateProductionEntryAsync(ProductionEntryDto dto);
        Task<ProductionEntry> CreateProductionEntryWithPhotoAsync(ProductionEntryDto dto, IFormFile? photo);
        Task<List<ProductionEntry>> GetAllProductionEntriesAsync();
        Task<ProductionEntry?> GetProductionEntryByIdAsync(int id);
        Task<ProductionEntryViewDto?> GetProductionEntryViewByIdAsync(int id);
        Task<ProductionSummary> GetCurrentSummaryAsync();
        Task<ProductionSummary> CalculateAndUpdateSummaryAsync();
        Task<List<ProductionEntry>> GetProductionEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<ProductionEntry>> GetProductionEntriesByCreatedDateRangeAsync(DateTime startDate, DateTime endDate);
        
        // Düzenleme metodlarý
        Task<ProductionEntry?> UpdateProductionEntryAsync(int id, ProductionEntryUpdateDto dto);
        Task<EditabilityCheckDto> CheckEditabilityAsync(int id);
        Task<bool> CanEditAsync(int id);
    }
}
