using System.ComponentModel.DataAnnotations;

namespace ProductionTrackerAPI.Models
{
    public class ProductionEntry
    {
        public int Id { get; set; }
        
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
        public int TotalDefects { get; set; }
        
        public int? RemainingOnTableCount { get; set; }
        
        [Required]
        public int CountTakenFromTable { get; set; }
        
        // Hesaplanan değerler
        public decimal MeasurementErrorRate { get; set; }
        public decimal KnittingErrorRate { get; set; }
        public decimal ToeDefectRate { get; set; }
        public decimal OtherDefectRate { get; set; }
        public decimal GeneralErrorRate { get; set; }
        
        // Fotoğraf yolu
        [StringLength(500)]
        public string? PhotoPath { get; set; }
        
        // Not alanı
        public string? Note { get; set; }
        
        // Zaman damgası
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
