using Microsoft.EntityFrameworkCore;
using ProductionTrackerAPI.Data;
using ProductionTrackerAPI.Models;

namespace ProductionTrackerAPI.Services
{
    public class ProductionService : IProductionService
    {
        private readonly ProductionDbContext _context;
        private readonly IWebHostEnvironment _environment;
        
        // Düzenleme süresi (saat)
        private const int EDIT_TIME_LIMIT_HOURS = 1;

        public ProductionService(ProductionDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ProductionEntry> CreateProductionEntryAsync(ProductionEntryDto dto)
        {
            return await CreateProductionEntryWithPhotoAsync(dto, dto.Photo);
        }

        public async Task<ProductionEntry> CreateProductionEntryWithPhotoAsync(ProductionEntryDto dto, IFormFile? photo)
        {
            var entry = new ProductionEntry
            {
                Date = dto.Date,
                MachineNo = dto.MachineNo,
                MkCycleSpeed = dto.MkCycleSpeed,
                Shift = dto.Shift,
                MoldNo = dto.MoldNo,
                Steam = dto.Steam,
                FormCount = dto.FormCount,
                MatchingPersonnelCount = dto.MatchingPersonnelCount,
                TablePersonnelCount = dto.TablePersonnelCount,
                ModelNo = dto.ModelNo,
                SizeNo = dto.SizeNo,
                ItemsPerPackage = dto.ItemsPerPackage,
                PackagesPerBag = dto.PackagesPerBag,
                BagsPerBox = dto.BagsPerBox,
                TableTotalPackage = dto.TableTotalPackage,
                MeasurementError = dto.MeasurementError,
                KnittingError = dto.KnittingError,
                ToeDefect = dto.ToeDefect,
                OtherDefect = dto.OtherDefect,
                RemainingOnTableCount = dto.RemainingOnTableCount,
                CountTakenFromTable = dto.CountTakenFromTable,
                Note = dto.Note,
                CreatedAt = DateTime.Now
            };

            // Fotoğraf yükleme işlemi
            if (photo != null && photo.Length > 0)
            {
                entry.PhotoPath = await SavePhotoAsync(photo);
            }

            // Toplam hataları ve oranları hesapla
            CalculateDefectValues(entry);

            _context.ProductionEntries.Add(entry);
            await _context.SaveChangesAsync();

            // Özet verilerini güncelle
            await CalculateAndUpdateSummaryAsync();

            return entry;
        }

        public async Task<ProductionEntry?> UpdateProductionEntryAsync(int id, ProductionEntryUpdateDto dto)
        {
            var entry = await _context.ProductionEntries.FindAsync(id);
            if (entry == null)
                return null;

            // Düzenleme süresini kontrol et
            if (!await CanEditAsync(id))
            {
                throw new InvalidOperationException("Bu kayıt artık düzenlenemez. Düzenleme süresi dolmuştur (1 saat).");
            }

            // Eski fotoğraf path'ini sakla
            var oldPhotoPath = entry.PhotoPath;

            // Verileri güncelle
            entry.Date = dto.Date;
            entry.MachineNo = dto.MachineNo;
            entry.MkCycleSpeed = dto.MkCycleSpeed;
            entry.Shift = dto.Shift;
            entry.MoldNo = dto.MoldNo;
            entry.Steam = dto.Steam;
            entry.FormCount = dto.FormCount;
            entry.MatchingPersonnelCount = dto.MatchingPersonnelCount;
            entry.TablePersonnelCount = dto.TablePersonnelCount;
            entry.ModelNo = dto.ModelNo;
            entry.SizeNo = dto.SizeNo;
            entry.ItemsPerPackage = dto.ItemsPerPackage;
            entry.PackagesPerBag = dto.PackagesPerBag;
            entry.BagsPerBox = dto.BagsPerBox;
            entry.TableTotalPackage = dto.TableTotalPackage;
            entry.MeasurementError = dto.MeasurementError;
            entry.KnittingError = dto.KnittingError;
            entry.ToeDefect = dto.ToeDefect;
            entry.OtherDefect = dto.OtherDefect;
            entry.RemainingOnTableCount = dto.RemainingOnTableCount;
            entry.CountTakenFromTable = dto.CountTakenFromTable;
            entry.Note = dto.Note;
            entry.UpdatedAt = DateTime.Now;

            // Fotoğraf işlemleri
            if (dto.DeleteCurrentPhoto && !string.IsNullOrEmpty(oldPhotoPath))
            {
                // Mevcut fotoğrafı sil
                await DeletePhotoAsync(oldPhotoPath);
                entry.PhotoPath = null;
            }

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                // Yeni fotoğraf varsa eski fotoğrafı sil (silinmemişse)
                if (!dto.DeleteCurrentPhoto && !string.IsNullOrEmpty(oldPhotoPath))
                {
                    await DeletePhotoAsync(oldPhotoPath);
                }
                
                // Yeni fotoğrafı kaydet
                entry.PhotoPath = await SavePhotoAsync(dto.Photo);
            }

            // Toplam hataları ve oranları yeniden hesapla
            CalculateDefectValues(entry);

            await _context.SaveChangesAsync();

            // Özet verilerini güncelle
            await CalculateAndUpdateSummaryAsync();

            return entry;
        }

        public async Task<EditabilityCheckDto> CheckEditabilityAsync(int id)
        {
            var entry = await _context.ProductionEntries.FindAsync(id);
            if (entry == null)
            {
                return new EditabilityCheckDto
                {
                    CanEdit = false,
                    Message = "Kayıt bulunamadı.",
                    CreatedAt = DateTime.MinValue,
                    EditDeadline = DateTime.MinValue
                };
            }

            var editDeadline = entry.CreatedAt.AddHours(EDIT_TIME_LIMIT_HOURS);
            var now = DateTime.Now;
            var canEdit = now <= editDeadline;
            var timeRemaining = canEdit ? editDeadline - now : TimeSpan.Zero;

            return new EditabilityCheckDto
            {
                CanEdit = canEdit,
                Message = canEdit 
                    ? $"Bu kayıt {timeRemaining:h\\:mm\\:ss} süre daha düzenlenebilir."
                    : "Bu kayıt artık düzenlenemez. Düzenleme süresi dolmuştur.",
                TimeRemaining = canEdit ? timeRemaining : null,
                CreatedAt = entry.CreatedAt,
                EditDeadline = editDeadline
            };
        }

        public async Task<bool> CanEditAsync(int id)
        {
            var entry = await _context.ProductionEntries.FindAsync(id);
            if (entry == null)
                return false;

            var editDeadline = entry.CreatedAt.AddHours(EDIT_TIME_LIMIT_HOURS);
            return DateTime.Now <= editDeadline;
        }

        public async Task<List<ProductionEntry>> GetAllProductionEntriesAsync()
        {
            return await _context.ProductionEntries
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductionEntry?> GetProductionEntryByIdAsync(int id)
        {
            return await _context.ProductionEntries.FindAsync(id);
        }

        public async Task<ProductionEntryViewDto?> GetProductionEntryViewByIdAsync(int id)
        {
            var entry = await _context.ProductionEntries.FindAsync(id);
            if (entry == null)
                return null;

            var editabilityCheck = await CheckEditabilityAsync(id);

            return new ProductionEntryViewDto
            {
                Id = entry.Id,
                Date = entry.Date,
                MachineNo = entry.MachineNo,
                MkCycleSpeed = entry.MkCycleSpeed,
                Shift = entry.Shift,
                MoldNo = entry.MoldNo,
                Steam = entry.Steam,
                FormCount = entry.FormCount,
                MatchingPersonnelCount = entry.MatchingPersonnelCount,
                TablePersonnelCount = entry.TablePersonnelCount,
                ModelNo = entry.ModelNo,
                SizeNo = entry.SizeNo,
                ItemsPerPackage = entry.ItemsPerPackage,
                PackagesPerBag = entry.PackagesPerBag,
                BagsPerBox = entry.BagsPerBox,
                TableTotalPackage = entry.TableTotalPackage,
                MeasurementError = entry.MeasurementError,
                KnittingError = entry.KnittingError,
                ToeDefect = entry.ToeDefect,
                OtherDefect = entry.OtherDefect,
                TotalDefects = entry.TotalDefects,
                RemainingOnTableCount = entry.RemainingOnTableCount,
                CountTakenFromTable = entry.CountTakenFromTable,
                MeasurementErrorRate = entry.MeasurementErrorRate,
                KnittingErrorRate = entry.KnittingErrorRate,
                ToeDefectRate = entry.ToeDefectRate,
                OtherDefectRate = entry.OtherDefectRate,
                GeneralErrorRate = entry.GeneralErrorRate,
                PhotoPath = entry.PhotoPath,
                Note = entry.Note,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,
                CanEdit = editabilityCheck.CanEdit,
                TimeRemainingForEdit = editabilityCheck.TimeRemaining,
                EditStatus = editabilityCheck.Message
            };
        }

        public async Task<ProductionSummary> GetCurrentSummaryAsync()
        {
            var summary = await _context.ProductionSummaries
                .OrderByDescending(s => s.CalculatedAt)
                .FirstOrDefaultAsync();

            if (summary == null)
            {
                summary = await CalculateAndUpdateSummaryAsync();
            }

            return summary;
        }

        public async Task<ProductionSummary> CalculateAndUpdateSummaryAsync()
        {
            var entries = await _context.ProductionEntries.ToListAsync();

            var summary = new ProductionSummary
            {
                // Toplam masa sayıları
                TotalTableCount = entries.Sum(e => e.CountTakenFromTable),
                TotalTableCountDozen = Math.Round((decimal)entries.Sum(e => e.CountTakenFromTable) / 12, 1),

                // Toplam hata sayıları
                TotalErrorCount = entries.Sum(e => e.TotalDefects),
                TotalErrorCountDozen = Math.Round((decimal)entries.Sum(e => e.TotalDefects) / 12, 1),

                // Hata türlerine göre dağılım
                MeasurementErrorCount = entries.Sum(e => e.MeasurementError),
                MeasurementErrorDozen = Math.Round((decimal)entries.Sum(e => e.MeasurementError) / 12, 1),
                MeasurementErrorRate = entries.Sum(e => e.CountTakenFromTable) > 0 
                    ? Math.Round((decimal)entries.Sum(e => e.MeasurementError) / entries.Sum(e => e.CountTakenFromTable) * 100, 2)
                    : 0,

                KnittingErrorCount = entries.Sum(e => e.KnittingError),
                KnittingErrorDozen = Math.Round((decimal)entries.Sum(e => e.KnittingError) / 12, 1),
                KnittingErrorRate = entries.Sum(e => e.CountTakenFromTable) > 0
                    ? Math.Round((decimal)entries.Sum(e => e.KnittingError) / entries.Sum(e => e.CountTakenFromTable) * 100, 2)
                    : 0,

                ToeDefectCount = entries.Sum(e => e.ToeDefect),
                ToeDefectDozen = Math.Round((decimal)entries.Sum(e => e.ToeDefect) / 12, 1),
                ToeDefectRate = entries.Sum(e => e.CountTakenFromTable) > 0
                    ? Math.Round((decimal)entries.Sum(e => e.ToeDefect) / entries.Sum(e => e.CountTakenFromTable) * 100, 2)
                    : 0,

                OtherDefectCount = entries.Sum(e => e.OtherDefect),
                OtherDefectDozen = Math.Round((decimal)entries.Sum(e => e.OtherDefect) / 12, 1),
                OtherDefectRate = entries.Sum(e => e.CountTakenFromTable) > 0
                    ? Math.Round((decimal)entries.Sum(e => e.OtherDefect) / entries.Sum(e => e.CountTakenFromTable) * 100, 2)
                    : 0,

                // Genel hata oranı
                OverallErrorRate = entries.Sum(e => e.CountTakenFromTable) > 0
                    ? Math.Round((decimal)entries.Sum(e => e.TotalDefects) / entries.Sum(e => e.CountTakenFromTable) * 100, 2)
                    : 0,

                CalculatedAt = DateTime.Now
            };

            _context.ProductionSummaries.Add(summary);
            await _context.SaveChangesAsync();

            return summary;
        }

        public async Task<List<ProductionEntry>> GetProductionEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Date alanına göre filtreleme yapıyoruz (gün bazında)
            return await _context.ProductionEntries
                .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ProductionEntry>> GetProductionEntriesByCreatedDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // CreatedAt alanına göre filtreleme yapıyoruz (saat dahil)
            return await _context.ProductionEntries
                .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // Yardımcı metodlar
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "production-photos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{photo.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return $"/uploads/production-photos/{uniqueFileName}";
        }

        private async Task DeletePhotoAsync(string photoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(photoPath))
                    return;

                // Relatif path'i fiziksel path'e çevir
                var fileName = Path.GetFileName(photoPath);
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "production-photos");
                var fullPath = Path.Combine(uploadsFolder, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception)
            {
                // Fotoğraf silinememesi critical bir hata değil, log'lanabilir
            }
        }

        private void CalculateDefectValues(ProductionEntry entry)
        {
            // Toplam hataları hesapla
            entry.TotalDefects = entry.MeasurementError + entry.KnittingError + entry.ToeDefect + entry.OtherDefect;

            // Hata oranlarını hesapla
            if (entry.CountTakenFromTable > 0)
            {
                entry.MeasurementErrorRate = Math.Round((decimal)entry.MeasurementError / entry.CountTakenFromTable * 100, 2);
                entry.KnittingErrorRate = Math.Round((decimal)entry.KnittingError / entry.CountTakenFromTable * 100, 2);
                entry.ToeDefectRate = Math.Round((decimal)entry.ToeDefect / entry.CountTakenFromTable * 100, 2);
                entry.OtherDefectRate = Math.Round((decimal)entry.OtherDefect / entry.CountTakenFromTable * 100, 2);
                entry.GeneralErrorRate = Math.Round((decimal)entry.TotalDefects / entry.CountTakenFromTable * 100, 2);
            }
        }
    }
}
