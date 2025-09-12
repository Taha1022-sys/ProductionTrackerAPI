using Microsoft.AspNetCore.Mvc;
using ProductionTrackerAPI.Models;
using ProductionTrackerAPI.Services;

namespace ProductionTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;
        }

        // Yeni �retim kayd� olu�tur
        [HttpPost("entries")]
        public async Task<ActionResult<ProductionEntry>> CreateProductionEntry([FromBody] ProductionEntryDto dto)
        {
            try
            {
                var entry = await _productionService.CreateProductionEntryAsync(dto);
                return CreatedAtAction(nameof(GetProductionEntry), new { id = entry.Id }, entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // T�m �retim kay�tlar�n� getir (sayfalama ile)
        [HttpGet("entries")]
        public async Task<ActionResult<object>> GetAllProductionEntries(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? machineNo = null,
            [FromQuery] int? shift = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var allEntries = await _productionService.GetAllProductionEntriesAsync();
                
                // Filtreleme
                var filteredEntries = allEntries.AsQueryable();
                
                if (!string.IsNullOrEmpty(machineNo))
                {
                    filteredEntries = filteredEntries.Where(e => e.MachineNo.Contains(machineNo, StringComparison.OrdinalIgnoreCase));
                }
                
                if (shift.HasValue)
                {
                    filteredEntries = filteredEntries.Where(e => e.Shift == shift.Value);
                }
                
                if (startDate.HasValue)
                {
                    filteredEntries = filteredEntries.Where(e => e.Date.Date >= startDate.Value.Date);
                }
                
                if (endDate.HasValue)
                {
                    filteredEntries = filteredEntries.Where(e => e.Date.Date <= endDate.Value.Date);
                }
                
                var totalCount = filteredEntries.Count();
                var entries = filteredEntries
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                
                return Ok(new
                {
                    items = entries,
                    totalCount = totalCount,
                    pageNumber = page,
                    pageSize = pageSize,
                    totalPages = totalPages
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // �zet liste g�r�n�m� i�in basit kay�t listesi
        [HttpGet("entries/summary")]
        public async Task<ActionResult<List<object>>> GetProductionEntriesSummary()
        {
            try
            {
                var entries = await _productionService.GetAllProductionEntriesAsync();
                
                var summaryList = entries.Select(e => new
                {
                    id = e.Id,
                    date = e.Date.ToString("dd.MM.yyyy"),
                    machineNo = e.MachineNo,
                    shift = e.Shift,
                    modelNo = e.ModelNo,
                    sizeNo = e.SizeNo,
                    formCount = e.FormCount,
                    totalDefects = e.TotalDefects,
                    generalErrorRate = e.GeneralErrorRate,
                    createdAt = e.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                }).ToList();
                
                return Ok(summaryList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Makine numaralar�n� getir (dropdown i�in)
        [HttpGet("machines")]
        public async Task<ActionResult<List<string>>> GetMachineNumbers()
        {
            try
            {
                var entries = await _productionService.GetAllProductionEntriesAsync();
                var machines = entries.Select(e => e.MachineNo).Distinct().OrderBy(m => m).ToList();
                return Ok(machines);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // �statistik bilgileri
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            try
            {
                var entries = await _productionService.GetAllProductionEntriesAsync();
                
                var stats = new
                {
                    totalEntries = entries.Count,
                    totalFormCount = entries.Sum(e => e.FormCount),
                    totalDefects = entries.Sum(e => e.TotalDefects),
                    averageErrorRate = entries.Any() ? Math.Round(entries.Average(e => e.GeneralErrorRate), 2) : 0,
                    machineCount = entries.Select(e => e.MachineNo).Distinct().Count(),
                    lastEntryDate = entries.Any() ? entries.Max(e => e.CreatedAt).ToString("dd.MM.yyyy HH:mm") : "Kay�t yok",
                    
                    // G�nl�k �retim say�lar� (son 7 g�n)
                    dailyProduction = entries
                        .Where(e => e.Date >= DateTime.Now.AddDays(-7))
                        .GroupBy(e => e.Date.Date)
                        .Select(g => new
                        {
                            date = g.Key.ToString("dd.MM.yyyy"),
                            count = g.Count(),
                            totalForms = g.Sum(e => e.FormCount)
                        })
                        .OrderBy(x => x.date)
                        .ToList()
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Belirli bir �retim kayd�n� getir
        [HttpGet("entries/{id}")]
        public async Task<ActionResult<ProductionEntry>> GetProductionEntry(int id)
        {
            var entry = await _productionService.GetProductionEntryByIdAsync(id);
            if (entry == null)
            {
                return NotFound(new { message = "Kay�t bulunamad�." });
            }
            return Ok(entry);
        }

        // �retim kayd�n� g�ncelle
        [HttpPut("entries/{id}")]
        public async Task<ActionResult<ProductionEntry>> UpdateProductionEntry(int id, [FromBody] ProductionEntryDto dto)
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
            catch (Exception ex)
            {
                return BadRequest(new { message = $"G�ncelleme hatas�: {ex.Message}" });
            }
        }

        // �retim kayd�n� sil
        [HttpDelete("entries/{id}")]
        public async Task<ActionResult> DeleteProductionEntry(int id)
        {
            try
            {
                var success = await _productionService.DeleteProductionEntryAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Kay�t bulunamad�." });
                }
                return Ok(new { message = "Kay�t ba�ar�yla silindi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Silme hatas�: {ex.Message}" });
            }
        }

        // Excel dosyas�n�n yolunu getir (test ama�l�)
        [HttpGet("excel-info")]
        public async Task<ActionResult<object>> GetExcelInfo()
        {
            try
            {
                var entries = await _productionService.GetAllProductionEntriesAsync();
                var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                var excelPath = Path.Combine(dataFolder, "ProductionEntries.xlsx");
                
                return Ok(new
                {
                    excelPath = excelPath,
                    fileExists = System.IO.File.Exists(excelPath),
                    recordCount = entries.Count,
                    lastModified = System.IO.File.Exists(excelPath) ? System.IO.File.GetLastWriteTime(excelPath).ToString("dd.MM.yyyy HH:mm:ss") : "Dosya bulunamad�"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
