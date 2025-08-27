using Microsoft.EntityFrameworkCore;
using ProductionTrackerAPI.Models;

namespace ProductionTrackerAPI.Data
{
    public class ProductionDbContext : DbContext
    {
        public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options)
        {
        }

        public DbSet<ProductionEntry> ProductionEntries { get; set; }
        public DbSet<ProductionSummary> ProductionSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductionEntry konfigürasyonu
            modelBuilder.Entity<ProductionEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.MachineNo).HasMaxLength(10).IsRequired();
                entity.Property(e => e.SizeNo).HasMaxLength(20).IsRequired();
                entity.Property(e => e.PhotoPath).HasMaxLength(500);
                entity.Property(e => e.Note).HasMaxLength(2000); // Note alanı için maksimum uzunluk
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                
                // Decimal precision ayarları
                entity.Property(e => e.MkCycleSpeed).HasPrecision(18, 2);
                entity.Property(e => e.Steam).HasPrecision(18, 2);
                entity.Property(e => e.MeasurementErrorRate).HasPrecision(18, 2);
                entity.Property(e => e.KnittingErrorRate).HasPrecision(18, 2);
                entity.Property(e => e.ToeDefectRate).HasPrecision(18, 2);
                entity.Property(e => e.OtherDefectRate).HasPrecision(18, 2);
                entity.Property(e => e.GeneralErrorRate).HasPrecision(18, 2);
            });

            // ProductionSummary konfigürasyonu
            modelBuilder.Entity<ProductionSummary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CalculatedAt).HasDefaultValueSql("GETDATE()");
                
                // Decimal precision ayarları
                entity.Property(e => e.TotalTableCountDozen).HasPrecision(18, 2);
                entity.Property(e => e.TotalErrorCountDozen).HasPrecision(18, 2);
                entity.Property(e => e.MeasurementErrorDozen).HasPrecision(18, 2);
                entity.Property(e => e.MeasurementErrorRate).HasPrecision(18, 2);
                entity.Property(e => e.KnittingErrorDozen).HasPrecision(18, 2);
                entity.Property(e => e.KnittingErrorRate).HasPrecision(18, 2);
                entity.Property(e => e.ToeDefectDozen).HasPrecision(18, 2);
                entity.Property(e => e.ToeDefectRate).HasPrecision(18, 2);
                entity.Property(e => e.OtherDefectDozen).HasPrecision(18, 2);
                entity.Property(e => e.OtherDefectRate).HasPrecision(18, 2);
                entity.Property(e => e.OverallErrorRate).HasPrecision(18, 2);
            });
        }
    }
}
