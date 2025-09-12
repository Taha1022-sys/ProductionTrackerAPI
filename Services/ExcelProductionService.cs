using ProductionTrackerAPI.Models;
using ClosedXML.Excel;

namespace ProductionTrackerAPI.Services
{
    public class ExcelProductionService : IProductionService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _excelFilePath;

        public ExcelProductionService(IWebHostEnvironment environment)
        {
            _environment = environment;
            var dataFolder = Path.Combine(_environment.ContentRootPath, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            
            _excelFilePath = Path.Combine(dataFolder, "ProductionEntries.xlsx");
            
            // Ýlk çalýþtýrmada Excel dosyasýný oluþtur
            InitializeExcelFile();
        }

        private void InitializeExcelFile()
        {
            if (!File.Exists(_excelFilePath))
            {
                CreateExcelFile();
            }
        }

        private void CreateExcelFile()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ProductionEntries");
            // Görseldeki Excel kolon sýrasýna göre baþlýklar
            var headers = new[]
            {
                "id", "date", "machineNo", "mkCycleSpeed", "shift", "moldNo", "steam",
                "formCount", "matchingPersonnelCount", "tablePersonnelCount", "modelNo",
                "sizeNo", "itemsPerPackage", "packagesPerBag", "bagsPerBox", "tableTotalPackage",
                "sampleFormCount", "repeatFormCount", "yesterdayRemainingCount", "unmatchedProductCount",
                "aQualityProductCount", "threadedProductCount", "stainedProductCount", "countTakenFromTable",
                "countTakenFromMachine", "measurementError", "knittingError", "toeDefect", "otherDefect",
                "totalDefects", "remainingOnTableCount", "measurementErrorRate", "knittingErrorRate",
                "toeDefectRate", "otherDefectRate", "generalErrorRate", "createdAt", "updatedAt", "note"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(_excelFilePath);
        }

        public async Task<ProductionEntry> CreateProductionEntryAsync(ProductionEntryDto dto)
        {
            var entry = new ProductionEntry
            {
                Id = await GetNextIdAsync(),
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
                SampleFormCount = dto.SampleFormCount,
                RepeatFormCount = dto.RepeatFormCount,
                YesterdayRemainingCount = dto.YesterdayRemainingCount,
                UnmatchedProductCount = dto.UnmatchedProductCount,
                AQualityProductCount = dto.AQualityProductCount,
                ThreadedProductCount = dto.ThreadedProductCount,
                StainedProductCount = dto.StainedProductCount,
                MeasurementError = dto.MeasurementError,
                KnittingError = dto.KnittingError,
                ToeDefect = dto.ToeDefect,
                OtherDefect = dto.OtherDefect,
                RemainingOnTableCount = dto.RemainingOnTableCount,
                CountTakenFromTable = dto.CountTakenFromTable,
                CountTakenFromMachine = dto.CountTakenFromMachine,
                Note = dto.Note,
                CreatedAt = DateTime.Now
            };

            // Toplam hatalarý ve oranlarý hesapla
            CalculateDefectValues(entry);

            // Excel'e kaydet
            await SaveEntryToExcelAsync(entry);

            return entry;
        }

        public async Task<ProductionEntry?> UpdateProductionEntryAsync(int id, ProductionEntryDto dto)
        {
            var entry = await GetProductionEntryByIdAsync(id);
            if (entry == null)
                return null;

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
            entry.SampleFormCount = dto.SampleFormCount;
            entry.RepeatFormCount = dto.RepeatFormCount;
            entry.YesterdayRemainingCount = dto.YesterdayRemainingCount;
            entry.UnmatchedProductCount = dto.UnmatchedProductCount;
            entry.AQualityProductCount = dto.AQualityProductCount;
            entry.ThreadedProductCount = dto.ThreadedProductCount;
            entry.StainedProductCount = dto.StainedProductCount;
            entry.MeasurementError = dto.MeasurementError;
            entry.KnittingError = dto.KnittingError;
            entry.ToeDefect = dto.ToeDefect;
            entry.OtherDefect = dto.OtherDefect;
            entry.RemainingOnTableCount = dto.RemainingOnTableCount;
            entry.CountTakenFromTable = dto.CountTakenFromTable;
            entry.CountTakenFromMachine = dto.CountTakenFromMachine;
            entry.Note = dto.Note;
            entry.UpdatedAt = DateTime.Now;

            CalculateDefectValues(entry);

            await UpdateEntryInExcelAsync(entry);

            return entry;
        }

        public async Task<bool> DeleteProductionEntryAsync(int id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var workbook = new XLWorkbook(_excelFilePath);
                    var worksheet = workbook.Worksheet(1);
                    
                    var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                    
                    for (int row = 2; row <= lastRow; row++)
                    {
                        if (worksheet.Cell(row, 1).GetValue<int>() == id)
                        {
                            worksheet.Row(row).Delete();
                            workbook.Save();
                            return true;
                        }
                    }
                    
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public async Task<List<ProductionEntry>> GetAllProductionEntriesAsync()
        {
            return await Task.Run(() =>
            {
                var entries = new List<ProductionEntry>();
                
                if (!File.Exists(_excelFilePath))
                    return entries;
                
                using var workbook = new XLWorkbook(_excelFilePath);
                var worksheet = workbook.Worksheet(1);
                
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                
                for (int row = 2; row <= lastRow; row++)
                {
                    var entry = ReadEntryFromRow(worksheet, row);
                    if (entry != null)
                        entries.Add(entry);
                }
                
                return entries.OrderByDescending(e => e.CreatedAt).ToList();
            });
        }

        public async Task<ProductionEntry?> GetProductionEntryByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(_excelFilePath))
                    return null;
                
                using var workbook = new XLWorkbook(_excelFilePath);
                var worksheet = workbook.Worksheet(1);
                
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                
                for (int row = 2; row <= lastRow; row++)
                {
                    if (worksheet.Cell(row, 1).GetValue<int>() == id)
                    {
                        return ReadEntryFromRow(worksheet, row);
                    }
                }
                
                return null;
            });
        }

        // Yardýmcý metodlar
        private async Task<int> GetNextIdAsync()
        {
            return await Task.Run(() =>
            {
                if (!File.Exists(_excelFilePath))
                    return 1;
                
                using var workbook = new XLWorkbook(_excelFilePath);
                var worksheet = workbook.Worksheet(1);
                
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                
                if (lastRow < 2)
                    return 1;
                
                var maxId = 0;
                for (int row = 2; row <= lastRow; row++)
                {
                    var id = worksheet.Cell(row, 1).GetValue<int>();
                    if (id > maxId)
                        maxId = id;
                }
                
                return maxId + 1;
            });
        }

        private async Task SaveEntryToExcelAsync(ProductionEntry entry)
        {
            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook(_excelFilePath);
                var worksheet = workbook.Worksheet(1);
                
                var nextRow = (worksheet.LastRowUsed()?.RowNumber() ?? 1) + 1;
                WriteEntryToRow(worksheet, nextRow, entry);
                
                workbook.Save();
            });
        }

        private async Task UpdateEntryInExcelAsync(ProductionEntry entry)
        {
            await Task.Run(() =>
            {
                using var workbook = new XLWorkbook(_excelFilePath);
                var worksheet = workbook.Worksheet(1);
                
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
                
                for (int row = 2; row <= lastRow; row++)
                {
                    if (worksheet.Cell(row, 1).GetValue<int>() == entry.Id)
                    {
                        WriteEntryToRow(worksheet, row, entry);
                        break;
                    }
                }
                
                workbook.Save();
            });
        }

        private void WriteEntryToRow(IXLWorksheet worksheet, int row, ProductionEntry entry)
        {
            worksheet.Cell(row, 1).Value = entry.Id;
            worksheet.Cell(row, 2).Value = entry.Date;
            worksheet.Cell(row, 3).Value = entry.MachineNo;
            worksheet.Cell(row, 4).Value = entry.MkCycleSpeed;
            worksheet.Cell(row, 5).Value = entry.Shift;
            worksheet.Cell(row, 6).Value = entry.MoldNo;
            worksheet.Cell(row, 7).Value = entry.Steam;
            worksheet.Cell(row, 8).Value = entry.FormCount;
            worksheet.Cell(row, 9).Value = entry.MatchingPersonnelCount;
            worksheet.Cell(row, 10).Value = entry.TablePersonnelCount;
            worksheet.Cell(row, 11).Value = entry.ModelNo;
            worksheet.Cell(row, 12).Value = entry.SizeNo;
            worksheet.Cell(row, 13).Value = entry.ItemsPerPackage;
            worksheet.Cell(row, 14).Value = entry.PackagesPerBag;
            worksheet.Cell(row, 15).Value = entry.BagsPerBox;
            worksheet.Cell(row, 16).Value = entry.TableTotalPackage;
            worksheet.Cell(row, 17).Value = entry.SampleFormCount;
            worksheet.Cell(row, 18).Value = entry.RepeatFormCount;
            worksheet.Cell(row, 19).Value = entry.YesterdayRemainingCount;
            worksheet.Cell(row, 20).Value = entry.UnmatchedProductCount;
            worksheet.Cell(row, 21).Value = entry.AQualityProductCount;
            worksheet.Cell(row, 22).Value = entry.ThreadedProductCount;
            worksheet.Cell(row, 23).Value = entry.StainedProductCount;
            worksheet.Cell(row, 24).Value = entry.CountTakenFromTable;
            worksheet.Cell(row, 25).Value = entry.CountTakenFromMachine;
            worksheet.Cell(row, 26).Value = entry.MeasurementError;
            worksheet.Cell(row, 27).Value = entry.KnittingError;
            worksheet.Cell(row, 28).Value = entry.ToeDefect;
            worksheet.Cell(row, 29).Value = entry.OtherDefect;
            worksheet.Cell(row, 30).Value = entry.TotalDefects;
            worksheet.Cell(row, 31).Value = entry.RemainingOnTableCount;
            worksheet.Cell(row, 32).Value = entry.MeasurementErrorRate;
            worksheet.Cell(row, 33).Value = entry.KnittingErrorRate;
            worksheet.Cell(row, 34).Value = entry.ToeDefectRate;
            worksheet.Cell(row, 35).Value = entry.OtherDefectRate;
            worksheet.Cell(row, 36).Value = entry.GeneralErrorRate;
            worksheet.Cell(row, 37).Value = entry.CreatedAt;
            worksheet.Cell(row, 38).Value = entry.UpdatedAt;
            worksheet.Cell(row, 39).Value = entry.Note ?? "";
        }

        private ProductionEntry? ReadEntryFromRow(IXLWorksheet worksheet, int row)
        {
            try
            {
                return new ProductionEntry
                {
                    Id = worksheet.Cell(row, 1).GetValue<int>(),
                    Date = worksheet.Cell(row, 2).GetValue<DateTime>(),
                    MachineNo = worksheet.Cell(row, 3).GetValue<string>(),
                    MkCycleSpeed = worksheet.Cell(row, 4).GetValue<decimal>(),
                    Shift = worksheet.Cell(row, 5).GetValue<int>(),
                    MoldNo = worksheet.Cell(row, 6).GetValue<int>(),
                    Steam = worksheet.Cell(row, 7).GetValue<decimal>(),
                    FormCount = worksheet.Cell(row, 8).GetValue<int>(),
                    MatchingPersonnelCount = worksheet.Cell(row, 9).GetValue<int>(),
                    TablePersonnelCount = worksheet.Cell(row, 10).GetValue<int>(),
                    ModelNo = worksheet.Cell(row, 11).GetValue<int>(),
                    SizeNo = worksheet.Cell(row, 12).GetValue<string>(),
                    ItemsPerPackage = worksheet.Cell(row, 13).GetValue<int>(),
                    PackagesPerBag = worksheet.Cell(row, 14).GetValue<int?>(),
                    BagsPerBox = worksheet.Cell(row, 15).GetValue<int?>(),
                    TableTotalPackage = worksheet.Cell(row, 16).GetValue<int>(),
                    SampleFormCount = worksheet.Cell(row, 17).GetValue<int>(),
                    RepeatFormCount = worksheet.Cell(row, 18).GetValue<int>(),
                    YesterdayRemainingCount = worksheet.Cell(row, 19).GetValue<int>(),
                    UnmatchedProductCount = worksheet.Cell(row, 20).GetValue<int>(),
                    AQualityProductCount = worksheet.Cell(row, 21).GetValue<int>(),
                    ThreadedProductCount = worksheet.Cell(row, 22).GetValue<int>(),
                    StainedProductCount = worksheet.Cell(row, 23).GetValue<int>(),
                    CountTakenFromTable = worksheet.Cell(row, 24).GetValue<int>(),
                    CountTakenFromMachine = worksheet.Cell(row, 25).GetValue<int>(),
                    MeasurementError = worksheet.Cell(row, 26).GetValue<int>(),
                    KnittingError = worksheet.Cell(row, 27).GetValue<int>(),
                    ToeDefect = worksheet.Cell(row, 28).GetValue<int>(),
                    OtherDefect = worksheet.Cell(row, 29).GetValue<int>(),
                    TotalDefects = worksheet.Cell(row, 30).GetValue<int>(),
                    RemainingOnTableCount = worksheet.Cell(row, 31).GetValue<int?>(),
                    MeasurementErrorRate = worksheet.Cell(row, 32).GetValue<decimal>(),
                    KnittingErrorRate = worksheet.Cell(row, 33).GetValue<decimal>(),
                    ToeDefectRate = worksheet.Cell(row, 34).GetValue<decimal>(),
                    OtherDefectRate = worksheet.Cell(row, 35).GetValue<decimal>(),
                    GeneralErrorRate = worksheet.Cell(row, 36).GetValue<decimal>(),
                    CreatedAt = worksheet.Cell(row, 37).GetValue<DateTime>(),
                    UpdatedAt = worksheet.Cell(row, 38).GetValue<DateTime?>(),
                    Note = worksheet.Cell(row, 39).GetValue<string>()
                };
            }
            catch
            {
                return null;
            }
        }

        private void CalculateDefectValues(ProductionEntry entry)
        {
            // Toplam hatalarý hesapla
            entry.TotalDefects = entry.MeasurementError + entry.KnittingError + entry.ToeDefect + entry.OtherDefect;

            // Hata oranlarýný hesapla
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