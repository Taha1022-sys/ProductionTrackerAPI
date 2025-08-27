using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProductionTrackerAPI.Models
{
    public class ProductionEntryDto
    {
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [StringLength(10)]
        public string MachineNo { get; set; } = string.Empty;
        
        public decimal MkCycleSpeed { get; set; }
        
        [Required]
        public int Shift { get; set; }
        
        [Required]
        public int MoldNo { get; set; }
        
        public decimal Steam { get; set; }
        
        [Required]
        public int FormCount { get; set; }
        
        [Required]
        public int MatchingPersonnelCount { get; set; }
        
        [Required]
        public int TablePersonnelCount { get; set; }
        
        [Required]
        public int ModelNo { get; set; }
        
        [Required]
        [StringLength(20)]
        public string SizeNo { get; set; } = string.Empty;
        
        [Required]
        public int ItemsPerPackage { get; set; }
        
        public int? PackagesPerBag { get; set; }
        
        public int? BagsPerBox { get; set; }
        
        [Required]
        public int TableTotalPackage { get; set; }
        
        // 2. Kalite verileri
        public int MeasurementError { get; set; }
        public int KnittingError { get; set; }
        public int ToeDefect { get; set; }
        public int OtherDefect { get; set; }
        
        public int? RemainingOnTableCount { get; set; }
        
        [Required]
        public int CountTakenFromTable { get; set; }
        
        // Fotoðraf dosyasý
        public IFormFile? Photo { get; set; }
        
        // Not alaný
        public string? Note { get; set; }
    }

    // Düzenleme için ayrý DTO
    public class ProductionEntryUpdateDto
    {
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [StringLength(10)]
        public string MachineNo { get; set; } = string.Empty;
        
        public decimal MkCycleSpeed { get; set; }
        
        [Required]
        public int Shift { get; set; }
        
        [Required]
        public int MoldNo { get; set; }
        
        public decimal Steam { get; set; }
        
        [Required]
        public int FormCount { get; set; }
        
        [Required]
        public int MatchingPersonnelCount { get; set; }
        
        [Required]
        public int TablePersonnelCount { get; set; }
        
        [Required]
        public int ModelNo { get; set; }
        
        [Required]
        [StringLength(20)]
        public string SizeNo { get; set; } = string.Empty;
        
        [Required]
        public int ItemsPerPackage { get; set; }
        
        public int? PackagesPerBag { get; set; }
        
        public int? BagsPerBox { get; set; }
        
        [Required]
        public int TableTotalPackage { get; set; }
        
        // 2. Kalite verileri
        public int MeasurementError { get; set; }
        public int KnittingError { get; set; }
        public int ToeDefect { get; set; }
        public int OtherDefect { get; set; }
        
        public int? RemainingOnTableCount { get; set; }
        
        [Required]
        public int CountTakenFromTable { get; set; }
        
        // Fotoðraf dosyasý (opsiyonel - deðiþtirilmek istenmezse null)
        public IFormFile? Photo { get; set; }
        
        // Mevcut fotoðrafý sil flag'i
        public bool DeleteCurrentPhoto { get; set; } = false;
        
        // Not alaný
        public string? Note { get; set; }
    }

    // Görüntüleme için salt okunur DTO
    public class ProductionEntryViewDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string MachineNo { get; set; } = string.Empty;
        public decimal MkCycleSpeed { get; set; }
        public int Shift { get; set; }
        public int MoldNo { get; set; }
        public decimal Steam { get; set; }
        public int FormCount { get; set; }
        public int MatchingPersonnelCount { get; set; }
        public int TablePersonnelCount { get; set; }
        public int ModelNo { get; set; }
        public string SizeNo { get; set; } = string.Empty;
        public int ItemsPerPackage { get; set; }
        public int? PackagesPerBag { get; set; }
        public int? BagsPerBox { get; set; }
        public int TableTotalPackage { get; set; }
        
        // Kalite verileri
        public int MeasurementError { get; set; }
        public int KnittingError { get; set; }
        public int ToeDefect { get; set; }
        public int OtherDefect { get; set; }
        public int TotalDefects { get; set; }
        public int? RemainingOnTableCount { get; set; }
        public int CountTakenFromTable { get; set; }
        
        // Hesaplanan deðerler
        public decimal MeasurementErrorRate { get; set; }
        public decimal KnittingErrorRate { get; set; }
        public decimal ToeDefectRate { get; set; }
        public decimal OtherDefectRate { get; set; }
        public decimal GeneralErrorRate { get; set; }
        
        // Fotoðraf yolu
        public string? PhotoPath { get; set; }
        
        // Not alaný
        public string? Note { get; set; }
        
        // Zaman damgalarý
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Düzenleme durumu
        public bool CanEdit { get; set; }
        public TimeSpan? TimeRemainingForEdit { get; set; }
        public string EditStatus { get; set; } = string.Empty;
    }

    // Düzenleme kontrolü için response DTO
    public class EditabilityCheckDto
    {
        public bool CanEdit { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan? TimeRemaining { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EditDeadline { get; set; }
    }
}
