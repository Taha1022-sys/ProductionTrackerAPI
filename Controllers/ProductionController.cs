using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionTrackerAPI.Data;
using ProductionTrackerAPI.Models;
using ProductionTrackerAPI.Services;

namespace ProductionTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;
        private readonly ProductionDbContext _dbContext;

        public ProductionController(IProductionService productionService, ProductionDbContext dbContext)
        {
            _productionService = productionService;
            _dbContext = dbContext;
        }

        [HttpPost("entries")]
        public async Task<ActionResult<ProductionEntry>> CreateProductionEntry([FromForm] ProductionEntryDto dto)
        {
            try
            {
                var entry = await _productionService.CreateProductionEntryWithPhotoAsync(dto, dto.Photo);
                return CreatedAtAction(nameof(GetProductionEntry), new { id = entry.Id }, entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("entries/{id}")]
        public async Task<ActionResult<ProductionEntry>> UpdateProductionEntry(int id, [FromForm] ProductionEntryUpdateDto dto)
        {
            try
            {
                var updatedEntry = await _productionService.UpdateProductionEntryAsync(id, dto);
                if (updatedEntry == null)
                {
                    return NotFound(new { message = "Kayýt bulunamadý." });
                }
                return Ok(updatedEntry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Güncellem hatasý: {ex.Message}" });
            }
        }

        [HttpGet("entries/{id}/editability")]
        public async Task<ActionResult<EditabilityCheckDto>> CheckEditability(int id)
        {
            var result = await _productionService.CheckEditabilityAsync(id);
            return Ok(result);
        }

        [HttpGet("entries")]
        public async Task<ActionResult<List<ProductionEntry>>> GetAllProductionEntries()
        {
            var entries = await _productionService.GetAllProductionEntriesAsync();
            return Ok(entries);
        }

        [HttpGet("entries/{id}")]
        public async Task<ActionResult<ProductionEntry>> GetProductionEntry(int id)
        {
            var entry = await _productionService.GetProductionEntryByIdAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [HttpGet("entries/{id}/view")]
        public async Task<ActionResult<ProductionEntryViewDto>> GetProductionEntryView(int id)
        {
            var entry = await _productionService.GetProductionEntryViewByIdAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ProductionSummary>> GetCurrentSummary()
        {
            var summary = await _productionService.GetCurrentSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("entries/date-range")]
        public async Task<ActionResult<List<ProductionEntry>>> GetEntriesByDateRange(
            [FromQuery] string startDate, 
            [FromQuery] string endDate,
            [FromQuery] string filterBy = "date", // "date" veya "created" seçenekleri
            [FromQuery] string? startTime = null, // HH:mm formatýnda (örn: "08:30")
            [FromQuery] string? endTime = null)   // HH:mm formatýnda (örn: "17:30")
        {
            try
            {
                // 1. Tarihleri parse et
                if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                    return BadRequest("Baþlangýç ve bitiþ tarihleri gereklidir.");

                DateTime start;
                DateTime end;

                // Baþlangýç tarihi parse
                if (!DateTime.TryParse(startDate, out start))
                    return BadRequest("Geçersiz baþlangýç tarihi formatý.");

                // Bitiþ tarihi parse
                if (!DateTime.TryParse(endDate, out end))
                    return BadRequest("Geçersiz bitiþ tarihi formatý.");

                // Saat bilgilerini iþle
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    if (TimeSpan.TryParse(startTime, out TimeSpan startTimeSpan))
                    {
                        start = start.Date.Add(startTimeSpan);
                    }
                    else
                    {
                        return BadRequest("Geçersiz baþlangýç saati formatý. HH:mm formatýnda olmalýdýr.");
                    }
                }
                else
                {
                    // Sadece tarih gelirse baþlangýç saati 00:00:00
                    start = start.Date;
                }

                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    if (TimeSpan.TryParse(endTime, out TimeSpan endTimeSpan))
                    {
                        end = end.Date.Add(endTimeSpan);
                    }
                    else
                    {
                        return BadRequest("Geçersiz bitiþ saati formatý. HH:mm formatýnda olmalýdýr.");
                    }
                }
                else
                {
                    // Sadece tarih gelirse bitiþ saati 23:59:59
                    end = end.Date.AddDays(1).AddMilliseconds(-1);
                }

                // 2. Filtreleme alanýný belirle ve sorgu
                List<ProductionEntry> entries;
                
                if (filterBy.ToLower() == "created")
                {
                    // CreatedAt alanýna göre filtrele (saat dahil)
                    entries = await _dbContext.ProductionEntries
                        .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
                        .OrderByDescending(e => e.CreatedAt)
                        .ToListAsync();
                }
                else
                {
                    // Date alanýna göre filtrele
                    if (string.IsNullOrWhiteSpace(startTime) && string.IsNullOrWhiteSpace(endTime))
                    {
                        // Sadece tarih filtrelemesi (gün bazýnda)
                        entries = await _dbContext.ProductionEntries
                            .Where(e => e.Date.Date >= start.Date && e.Date.Date <= end.Date)
                            .OrderByDescending(e => e.Date)
                            .ThenByDescending(e => e.CreatedAt)
                            .ToListAsync();
                    }
                    else
                    {
                        // Tarih + saat filtrelemesi (Date alaný + saat kontrolü CreatedAt üzerinden)
                        entries = await _dbContext.ProductionEntries
                            .Where(e => e.Date.Date >= start.Date && e.Date.Date <= end.Date && 
                                       e.CreatedAt >= start && e.CreatedAt <= end)
                            .OrderByDescending(e => e.Date)
                            .ThenByDescending(e => e.CreatedAt)
                            .ToListAsync();
                    }
                }

                return Ok(entries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Filtreleme hatasý: {ex.Message}" });
            }
        }

        [HttpPost("summary/calculate")]
        public async Task<ActionResult<ProductionSummary>> CalculateSummary()
        {
            var summary = await _productionService.CalculateAndUpdateSummaryAsync();
            return Ok(summary);
        }

        // Test için - filtreleme parametrelerini göster
        [HttpGet("entries/filter-info")]
        public ActionResult GetFilterInfo()
        {
            return Ok(new
            {
                FilterTypes = new[]
                {
                    new { Value = "date", Label = "Üretim Tarihine Göre", Description = "Date alanýna göre filtreleme" },
                    new { Value = "created", Label = "Kayýt Tarihine Göre", Description = "CreatedAt alanýna göre filtreleme" }
                },
                TimeFormat = "HH:mm (örn: 08:30, 17:45)",
                Examples = new[]
                {
                    "Sadece tarih: ?startDate=2025-01-01&endDate=2025-01-31",
                    "Tarih + saat (üretim): ?startDate=2025-01-01&endDate=2025-01-01&startTime=08:00&endTime=17:00&filterBy=date",
                    "Tarih + saat (kayýt): ?startDate=2025-01-01&endDate=2025-01-01&startTime=08:00&endTime=17:00&filterBy=created"
                },
                EditInfo = new
                {
                    EditTimeLimit = "1 saat",
                    Description = "Kayýtlar oluþturulduktan sonra 1 saat süreyle düzenlenebilir."
                }
            });
        }
    }
}
