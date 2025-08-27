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
        
        // Foto�raf dosyas�
        public IFormFile? Photo { get; set; }
        
        // Not alan�
        public string? Note { get; set; }
    }

    // D�zenleme i�in ayr� DTO
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
        
        // Foto�raf dosyas� (opsiyonel - de�i�tirilmek istenmezse null)
        public IFormFile? Photo { get; set; }
        
        // Mevcut foto�raf� sil flag'i
        public bool DeleteCurrentPhoto { get; set; } = false;
        
        // Not alan�
        public string? Note { get; set; }
    }

    // G�r�nt�leme i�in salt okunur DTO
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
        
        // Hesaplanan de�erler
        public decimal MeasurementErrorRate { get; set; }
        public decimal KnittingErrorRate { get; set; }
        public decimal ToeDefectRate { get; set; }
        public decimal OtherDefectRate { get; set; }
        public decimal GeneralErrorRate { get; set; }
        
        // Foto�raf yolu
        public string? PhotoPath { get; set; }
        
        // Not alan�
        public string? Note { get; set; }
        
        // Zaman damgalar�
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // D�zenleme durumu
        public bool CanEdit { get; set; }
        public TimeSpan? TimeRemainingForEdit { get; set; }
        public string EditStatus { get; set; } = string.Empty;
    }

    // D�zenleme kontrol� i�in response DTO
    public class EditabilityCheckDto
    {
        public bool CanEdit { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan? TimeRemaining { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EditDeadline { get; set; }
    }
}
