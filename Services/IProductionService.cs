using ProductionTrackerAPI.Models;

namespace ProductionTrackerAPI.Services
{
    public interface IProductionService
    {
        Task<ProductionEntry> CreateProductionEntryAsync(ProductionEntryDto dto);
        Task<List<ProductionEntry>> GetAllProductionEntriesAsync();
        Task<ProductionEntry?> GetProductionEntryByIdAsync(int id);
        Task<ProductionEntry?> UpdateProductionEntryAsync(int id, ProductionEntryDto dto);
        Task<bool> DeleteProductionEntryAsync(int id);
    }
}
