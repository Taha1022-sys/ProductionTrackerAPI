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
                    return NotFound(new { message = "Kay�t bulunamad�." });
                }
                return Ok(updatedEntry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"G�ncellem hatas�: {ex.Message}" });
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
            [FromQuery] string filterBy = "date", // "date" veya "created" se�enekleri
            [FromQuery] string? startTime = null, // HH:mm format�nda (�rn: "08:30")
            [FromQuery] string? endTime = null)   // HH:mm format�nda (�rn: "17:30")
        {
            try
            {
                // 1. Tarihleri parse et
                if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
                    return BadRequest("Ba�lang�� ve biti� tarihleri gereklidir.");

                DateTime start;
                DateTime end;

                // Ba�lang�� tarihi parse
                if (!DateTime.TryParse(startDate, out start))
                    return BadRequest("Ge�ersiz ba�lang�� tarihi format�.");

                // Biti� tarihi parse
                if (!DateTime.TryParse(endDate, out end))
                    return BadRequest("Ge�ersiz biti� tarihi format�.");

                // Saat bilgilerini i�le
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    if (TimeSpan.TryParse(startTime, out TimeSpan startTimeSpan))
                    {
                        start = start.Date.Add(startTimeSpan);
                    }
                    else
                    {
                        return BadRequest("Ge�ersiz ba�lang�� saati format�. HH:mm format�nda olmal�d�r.");
                    }
                }
                else
                {
                    // Sadece tarih gelirse ba�lang�� saati 00:00:00
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
                        return BadRequest("Ge�ersiz biti� saati format�. HH:mm format�nda olmal�d�r.");
                    }
                }
                else
                {
                    // Sadece tarih gelirse biti� saati 23:59:59
                    end = end.Date.AddDays(1).AddMilliseconds(-1);
                }

                // 2. Filtreleme alan�n� belirle ve sorgu
                List<ProductionEntry> entries;
                
                if (filterBy.ToLower() == "created")
                {
                    // CreatedAt alan�na g�re filtrele (saat dahil)
                    entries = await _dbContext.ProductionEntries
                        .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
                        .OrderByDescending(e => e.CreatedAt)
                        .ToListAsync();
                }
                else
                {
                    // Date alan�na g�re filtrele
                    if (string.IsNullOrWhiteSpace(startTime) && string.IsNullOrWhiteSpace(endTime))
                    {
                        // Sadece tarih filtrelemesi (g�n baz�nda)
                        entries = await _dbContext.ProductionEntries
                            .Where(e => e.Date.Date >= start.Date && e.Date.Date <= end.Date)
                            .OrderByDescending(e => e.Date)
                            .ThenByDescending(e => e.CreatedAt)
                            .ToListAsync();
                    }
                    else
                    {
                        // Tarih + saat filtrelemesi (Date alan� + saat kontrol� CreatedAt �zerinden)
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
                return BadRequest(new { message = $"Filtreleme hatas�: {ex.Message}" });
            }
        }

        [HttpPost("summary/calculate")]
        public async Task<ActionResult<ProductionSummary>> CalculateSummary()
        {
            var summary = await _productionService.CalculateAndUpdateSummaryAsync();
            return Ok(summary);
        }

        // Test i�in - filtreleme parametrelerini g�ster
        [HttpGet("entries/filter-info")]
        public ActionResult GetFilterInfo()
        {
            return Ok(new
            {
                FilterTypes = new[]
                {
                    new { Value = "date", Label = "�retim Tarihine G�re", Description = "Date alan�na g�re filtreleme" },
                    new { Value = "created", Label = "Kay�t Tarihine G�re", Description = "CreatedAt alan�na g�re filtreleme" }
                },
                TimeFormat = "HH:mm (�rn: 08:30, 17:45)",
                Examples = new[]
                {
                    "Sadece tarih: ?startDate=2025-01-01&endDate=2025-01-31",
                    "Tarih + saat (�retim): ?startDate=2025-01-01&endDate=2025-01-01&startTime=08:00&endTime=17:00&filterBy=date",
                    "Tarih + saat (kay�t): ?startDate=2025-01-01&endDate=2025-01-01&startTime=08:00&endTime=17:00&filterBy=created"
                },
                EditInfo = new
                {
                    EditTimeLimit = "1 saat",
                    Description = "Kay�tlar olu�turulduktan sonra 1 saat s�reyle d�zenlenebilir."
                }
            });
        }
    }
}
