namespace ProductionTrackerAPI.Models
{
    public class ProductionSummary
    {
        public int Id { get; set; }
        
        // Toplam masa sayıları
        public int TotalTableCount { get; set; }
        public decimal TotalTableCountDozen { get; set; }
        
        // Toplam hata sayıları
        public int TotalErrorCount { get; set; }
        public decimal TotalErrorCountDozen { get; set; }
        
        // Hata türlerine göre dağılım
        public int MeasurementErrorCount { get; set; }
        public decimal MeasurementErrorDozen { get; set; }
        public decimal MeasurementErrorRate { get; set; }
        
        public int KnittingErrorCount { get; set; }
        public decimal KnittingErrorDozen { get; set; }
        public decimal KnittingErrorRate { get; set; }
        
        public int ToeDefectCount { get; set; }
        public decimal ToeDefectDozen { get; set; }
        public decimal ToeDefectRate { get; set; }
        
        public int OtherDefectCount { get; set; }
        public decimal OtherDefectDozen { get; set; }
        public decimal OtherDefectRate { get; set; }
        
        // Genel hata oranı
        public decimal OverallErrorRate { get; set; }
        
        // Hesaplama tarihi
        public DateTime CalculatedAt { get; set; } = DateTime.Now;
    }
}
