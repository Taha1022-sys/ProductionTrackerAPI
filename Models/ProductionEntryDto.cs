using System.ComponentModel.DataAnnotations;

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
        
        public int SampleFormCount { get; set; }
        public int RepeatFormCount { get; set; }
        public int YesterdayRemainingCount { get; set; }
        public int UnmatchedProductCount { get; set; }
        public int AQualityProductCount { get; set; }
        public int ThreadedProductCount { get; set; }
        public int StainedProductCount { get; set; }
        public int CountTakenFromMachine { get; set; } 
        
        public int MeasurementError { get; set; }
        public int KnittingError { get; set; }
        public int ToeDefect { get; set; }
        public int OtherDefect { get; set; }
        
        public int? RemainingOnTableCount { get; set; }
        
        [Required]
        public int CountTakenFromTable { get; set; }
        
        public string? Note { get; set; }
    }
}
