using ClosedXML.Excel;
using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend;
using Model.ModelExtend.Report;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WebApp.Services;

namespace BVTL.Tests
{
    [TestClass]
    public class ScheduledReportTests
    {
        // Ensure EntityFramework.SqlServer provider is loaded into memory
        private static readonly Type _efSql = typeof(System.Data.Entity.SqlServer.SqlProviderServices);

        [TestMethod]
        public void ScheduledReportDA_GetSettings_ShouldReturnConfiguredValues()
        {
            var da = new ScheduledReportDA();
            var settings = da.GetSettings();

            Assert.IsNotNull(settings, "Settings should not be null");
            Assert.IsTrue(settings.RunDay >= 1 && settings.RunDay <= 31, "Day of month should be between 1 and 31");
            Assert.AreEqual(8, settings.RunHour, "Default hour should be 8");
            Assert.IsTrue(settings.IsActive, "Auto export should be active");
        }

        [TestMethod]
        public void ScheduledReportDA_CheckDataAvailability_ShouldExecuteSuccessfully()
        {
            var da = new ScheduledReportDA();
            // Test check CD45 report data
            bool hasDataCD45 = da.CheckDataAvailability("HOAT_DONG_CD45", 2026, 8);
            Assert.IsTrue(hasDataCD45, "August 2026 should have CD45 report data");

            // Test check TCV CD45 report data
            bool hasDataTCV = da.CheckDataAvailability("TCV_CD45", 2026, 8);
            Assert.IsTrue(hasDataTCV, "August 2026 should have TCV report data");
        }

        [TestMethod]
        public void ScheduledReportDA_GetExportLogs_ShouldReturnPagedListWithoutException()
        {
            var da = new ScheduledReportDA();
            int totalRows;
            var logs = da.GetExportLogs(null, null, null, 1, 10, out totalRows);

            Assert.IsNotNull(logs, "Logs list should not be null");
            Assert.IsTrue(totalRows >= 0, "Total rows should be >= 0");
        }

        [TestMethod]
        public void ScheduledReportSettingModel_DefaultConstructor_ShouldHaveSaneDefaults()
        {
            var model = new ScheduledReportSettingModel();
            Assert.AreEqual(5, model.RunDay);
            Assert.AreEqual(8, model.RunHour);
            Assert.IsTrue(model.IsActive);
        }

        [TestMethod]
        public void ReportExportService_ExportTCVCD45ZipAsync_ShouldCreateValidZip()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Test export for The Times group (HNO - tt)
                var task = service.ExportTCVCD45ZipAsync(2026, 8, "HNO", "tt", "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileSizeBytes > 0, "ZIP file should have size > 0");
                Assert.IsTrue(result.TotalItems > 0, "Should have exported at least 1 TCV");
                generatedFilePath = result.FilePath;

                // Kiểm tra cấu trúc thư mục phân cấp bên trong file ZIP
                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        Assert.IsTrue(archive.Entries.Count > 0, "ZIP must contain entries");

                        bool hasProvinceSummary = false;
                        bool hasGroupSummary = false;
                        bool hasTcvDetail = false;

                        foreach (var entry in archive.Entries)
                        {
                            // Kiểm tra đường dẫn có phân cấp thư mục (dấu /)
                            Assert.IsTrue(entry.FullName.Contains("/"), $"Entry '{entry.FullName}' should be in a folder");

                            if (entry.FullName.Contains("BaoCao_TongHop_HNO")) hasProvinceSummary = true;
                            if (entry.FullName.Contains("BaoCao_TongHop_") && !entry.FullName.Contains("BaoCao_TongHop_HNO")) hasGroupSummary = true;
                            if (entry.FullName.Contains("BaoCao_TCV_")) hasTcvDetail = true;
                        }

                        Assert.IsTrue(hasProvinceSummary, "ZIP should contain Province summary report (BaoCao_TongHop_HNO)");
                        Assert.IsTrue(hasGroupSummary, "ZIP should contain Group summary report (BaoCao_TongHop_...)");
                        Assert.IsTrue(hasTcvDetail, "ZIP should contain individual TCV report (BaoCao_TCV_)");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportTCVCD45ZipAsync_ForHCM_ShouldExportCityAndGroupSummaries()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportTCVCD45ZipAsync(2026, 8, "HCM", null, "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export should succeed for HCM: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileSizeBytes > 0, "ZIP file should have size > 0");
                Assert.IsTrue(result.TotalItems >= 5, $"Should have exported at least 5 summaries for HCM (actual: {result.TotalItems})");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        Assert.IsTrue(archive.Entries.Count >= 5, $"ZIP must contain at least 5 entries (actual: {archive.Entries.Count})");

                        bool hasProvinceSummary = false;
                        bool hasAlocare = false;
                        bool hasG3vn = false;
                        bool hasMyhands = false;
                        bool hasTheGate = false;
                        int tcvFileCount = 0;

                        foreach (var entry in archive.Entries)
                        {
                            Assert.IsTrue(entry.FullName.Contains("Hồ Chí Minh/"), $"All entries for HCM should be in Hồ Chí Minh folder: {entry.FullName}");

                            if (entry.Name == "BaoCao_TongHop_HCM.xlsx") hasProvinceSummary = true;
                            if (entry.FullName.Contains("Alocare")) hasAlocare = true;
                            if (entry.FullName.Contains("G3VN")) hasG3vn = true;
                            if (entry.FullName.Contains("Myhands")) hasMyhands = true;
                            if (entry.FullName.Contains("The Gate")) hasTheGate = true;
                            if (entry.Name.StartsWith("BaoCao_TCV_")) tcvFileCount++;
                        }

                        Assert.IsTrue(hasProvinceSummary, "ZIP should contain Province summary report for HCM (BaoCao_TongHop_HCM.xlsx)");
                        Assert.IsTrue(hasAlocare, "ZIP should contain Group summary report for Alocare");
                        Assert.IsTrue(hasG3vn, "ZIP should contain Group summary report for G3VN");
                        Assert.IsTrue(hasMyhands, "ZIP should contain Group summary report for Myhands");
                        Assert.IsTrue(hasTheGate, "ZIP should contain Group summary report for The Gate");
                        Assert.AreEqual(17, tcvFileCount, "HCM should have 17 TCV detail files for the 17 active TCVs in database");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportTCVCD45ZipAsync_AllProvinces_ShouldCreateHierarchicalStructure()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportTCVCD45ZipAsync(2026, 8, null, null, "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk");
                Assert.IsTrue(result.TotalItems >= 100, $"Total exported TCVs should be >= 100 (actual: {result.TotalItems})");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        Assert.IsTrue(archive.Entries.Count > 100, "ZIP should contain > 100 entries");

                        bool hasHanoi = false;
                        bool hasHaiPhong = false;
                        bool hasNgheAn = false;
                        bool hasHcm = false;
                        int provinceSummaryCount = 0;
                        int groupSummaryCount = 0;
                        int tcvCount = 0;

                        foreach (var entry in archive.Entries)
                        {
                            if (entry.FullName.StartsWith("Hà Nội/")) hasHanoi = true;
                            if (entry.FullName.StartsWith("Hải Phòng/")) hasHaiPhong = true;
                            if (entry.FullName.StartsWith("Nghệ An/")) hasNgheAn = true;
                            if (entry.FullName.Contains("Hồ Chí Minh/")) hasHcm = true;

                            var parts = entry.FullName.Split('/');
                            if (parts.Length == 2 && entry.Name.StartsWith("BaoCao_TongHop_"))
                                provinceSummaryCount++;
                            else if (parts.Length == 3 && entry.Name.StartsWith("BaoCao_TongHop_"))
                                groupSummaryCount++;
                            else if (entry.Name.StartsWith("BaoCao_TCV_"))
                                tcvCount++;
                        }

                        Assert.IsTrue(hasHanoi, "ZIP should contain folder for Hà Nội");
                        Assert.IsTrue(hasHaiPhong, "ZIP should contain folder for Hải Phòng");
                        Assert.IsTrue(hasNgheAn, "ZIP should contain folder for Nghệ An");
                        Assert.IsTrue(hasHcm, "ZIP should contain folder for TP. Hồ Chí Minh");
                        Assert.IsTrue(provinceSummaryCount >= 6, $"Should contain summary reports for at least 6 provinces (actual: {provinceSummaryCount})");
                        Assert.IsTrue(groupSummaryCount >= 14, $"Should contain summary reports for groups (actual: {groupSummaryCount})");
                        Assert.IsTrue(tcvCount >= 100, $"Should contain >= 100 TCV reports (actual: {tcvCount})");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ScheduledReportDA_SaveOrUpdateExportLog_And_DeleteExportLog_ShouldUpsertAndCleanup()
        {
            var da = new ScheduledReportDA();
            long firstId = 0;
            try
            {
                // 1. Tạo bản ghi lần 1
                var model1 = new ExportedReportLogModel
                {
                    ReportType = "TEST_UPSERT",
                    ReportName = "Test Upsert Report",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 01/2099",
                    Year = 2099,
                    Month = 1,
                    FileName = "Test_File_V1.xlsx",
                    FilePath = "C:\\Temp\\Test_File_V1.xlsx",
                    Status = "Success",
                    TriggerType = "UnitTest",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };

                firstId = da.SaveOrUpdateExportLog(model1);
                Assert.IsTrue(firstId > 0, "First insert should succeed and return positive ID");

                // 2. Xuất lại lần 2 cho cùng kỳ (2099/01, TEST_UPSERT)
                var model2 = new ExportedReportLogModel
                {
                    ReportType = "TEST_UPSERT",
                    ReportName = "Test Upsert Report",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 01/2099",
                    Year = 2099,
                    Month = 1,
                    FileName = "Test_File_V2.xlsx",
                    FilePath = "C:\\Temp\\Test_File_V2.xlsx",
                    Status = "Success",
                    TriggerType = "UnitTest",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };

                long secondId = da.SaveOrUpdateExportLog(model2);
                Assert.AreEqual(firstId, secondId, "Subsequent export for same period should UPDATE the existing record and return identical ID");

                // Kiểm tra lại log trong DB
                var fetched = da.GetById(firstId);
                Assert.IsNotNull(fetched);
                Assert.AreEqual("Test_File_V2.xlsx", fetched.FileName, "FileName should be updated to V2");
            }
            finally
            {
                if (firstId > 0)
                {
                    string delPath;
                    bool deleted = da.DeleteExportLog(firstId, out delPath);
                    Assert.IsTrue(deleted, "Cleanup delete should succeed");
                }
            }
        }

        [TestMethod]
        public void ScheduledReportDA_ExcelAndPdf_SamePeriod_ShouldPreserveBothRecords()
        {
            var da = new ScheduledReportDA();
            long excelId = 0;
            long pdfId = 0;

            try
            {
                // 1. Tạo bản ghi định dạng EXCEL
                var modelExcel = new ExportedReportLogModel
                {
                    ReportType = "TEST_FORMAT_DIFF",
                    ReportName = "Báo cáo Test Excel",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 05/2099",
                    Year = 2099,
                    Month = 5,
                    FileName = "Test_Report_Thang05_2099.xlsx",
                    FilePath = "C:\\Temp\\Test_Report_Thang05_2099.xlsx",
                    FileFormat = "EXCEL",
                    Status = "Success",
                    TriggerType = "UnitTest",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };
                excelId = da.SaveOrUpdateExportLog(modelExcel);
                Assert.IsTrue(excelId > 0, "Insert Excel record should succeed");

                // 2. Tạo bản ghi định dạng PDF cùng loại & cùng kỳ
                var modelPdf = new ExportedReportLogModel
                {
                    ReportType = "TEST_FORMAT_DIFF",
                    ReportName = "Báo cáo Test PDF",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 05/2099",
                    Year = 2099,
                    Month = 5,
                    FileName = "Test_Report_Thang05_2099.pdf",
                    FilePath = "C:\\Temp\\Test_Report_Thang05_2099.pdf",
                    FileFormat = "PDF",
                    Status = "Success",
                    TriggerType = "UnitTest",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };
                pdfId = da.SaveOrUpdateExportLog(modelPdf);
                Assert.IsTrue(pdfId > 0, "Insert PDF record should succeed");

                // 3. Khẳng định 2 bản ghi có ID khác nhau (không bị ghi đè lẫn nhau)
                Assert.AreNotEqual(excelId, pdfId, "Excel and PDF for the same period should have distinct IDs and not overwrite each other");

                // 4. Kiểm tra dữ liệu từng bản ghi trong DB
                var fetchedExcel = da.GetById(excelId);
                var fetchedPdf = da.GetById(pdfId);

                Assert.IsNotNull(fetchedExcel, "Excel log should exist");
                Assert.AreEqual("EXCEL", fetchedExcel.Format, "Format should be EXCEL");
                Assert.AreEqual("Test_Report_Thang05_2099.xlsx", fetchedExcel.FileName);

                Assert.IsNotNull(fetchedPdf, "PDF log should exist");
                Assert.AreEqual("PDF", fetchedPdf.Format, "Format should be PDF");
                Assert.AreEqual("Test_Report_Thang05_2099.pdf", fetchedPdf.FileName);

                // 5. Xuất lại Excel lần 2 -> chỉ cập nhật bản ghi Excel, bản ghi PDF vẫn giữ nguyên
                var modelExcelV2 = new ExportedReportLogModel
                {
                    ReportType = "TEST_FORMAT_DIFF",
                    ReportName = "Báo cáo Test Excel V2",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 05/2099",
                    Year = 2099,
                    Month = 5,
                    FileName = "Test_Report_Thang05_2099_V2.xlsx",
                    FilePath = "C:\\Temp\\Test_Report_Thang05_2099_V2.xlsx",
                    FileFormat = "EXCEL",
                    Status = "Success",
                    TriggerType = "UnitTest",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };
                long updatedExcelId = da.SaveOrUpdateExportLog(modelExcelV2);
                Assert.AreEqual(excelId, updatedExcelId, "Re-exporting Excel should update the existing Excel record");

                var fetchedPdfAfter = da.GetById(pdfId);
                Assert.IsNotNull(fetchedPdfAfter, "PDF log must still exist after Excel re-export");
                Assert.AreEqual("Test_Report_Thang05_2099.pdf", fetchedPdfAfter.FileName, "PDF FileName should be unchanged");
            }
            finally
            {
                string p;
                if (excelId > 0) da.DeleteExportLog(excelId, out p);
                if (pdfId > 0) da.DeleteExportLog(pdfId, out p);
            }
        }

        [TestMethod]
        public void ScheduledReportDA_GetExportLogs_WithFormatFilter_ShouldFilterAccurately()
        {
            var da = new ScheduledReportDA();
            long excelId = 0;
            long pdfId = 0;

            try
            {
                var modelExcel = new ExportedReportLogModel
                {
                    ReportType = "TEST_FORMAT_FILTER",
                    ReportName = "Test Filter Excel",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 06/2099",
                    Year = 2099,
                    Month = 6,
                    FileName = "Test_Filter_Thang06_2099.xlsx",
                    FilePath = "C:\\Temp\\Test_Filter_Thang06_2099.xlsx",
                    FileFormat = "EXCEL",
                    Status = "Success",
                    TriggerType = "TestFilter",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };
                excelId = da.AddExportLog(modelExcel);

                var modelPdf = new ExportedReportLogModel
                {
                    ReportType = "TEST_FORMAT_FILTER",
                    ReportName = "Test Filter PDF",
                    PeriodType = "Month",
                    PeriodValue = "Tháng 06/2099",
                    Year = 2099,
                    Month = 6,
                    FileName = "Test_Filter_Thang06_2099.pdf",
                    FilePath = "C:\\Temp\\Test_Filter_Thang06_2099.pdf",
                    FileFormat = "PDF",
                    Status = "Success",
                    TriggerType = "TestFilter",
                    CreatedBy = "Tester",
                    CreatedDate = DateTime.Now
                };
                pdfId = da.AddExportLog(modelPdf);

                int total;
                // Lọc EXCEL
                var excelLogs = da.GetExportLogs("TEST_FORMAT_FILTER", 2099, 6, 1, 10, out total, "Month", "EXCEL");
                Assert.IsTrue(excelLogs.Count >= 1, "Should find Excel logs");
                Assert.IsTrue(excelLogs.TrueForAll(x => x.Format == "EXCEL"), "All returned logs should have EXCEL format");

                // Lọc PDF
                var pdfLogs = da.GetExportLogs("TEST_FORMAT_FILTER", 2099, 6, 1, 10, out total, "Month", "PDF");
                Assert.IsTrue(pdfLogs.Count >= 1, "Should find PDF logs");
                Assert.IsTrue(pdfLogs.TrueForAll(x => x.Format == "PDF"), "All returned logs should have PDF format");
            }
            finally
            {
                string p;
                if (excelId > 0) da.DeleteExportLog(excelId, out p);
                if (pdfId > 0) da.DeleteExportLog(pdfId, out p);
            }
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45ExcelAsync_ShouldSucceedAndDisplayDashesForZero()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportHoatDongCD45ExcelAsync(2026, 8, null, null, "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export HoatDong should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "Excel file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileSizeBytes > 0, "Excel file should have size > 0");
                generatedFilePath = result.FilePath;

                // Kiểm tra nội dung các ô trong file Excel
                using (var wb = new XLWorkbook(result.FilePath))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    Assert.IsNotNull(ws, "Worksheet must exist");

                    // Dòng 6 là Section I (tiêu đề bold) -> các ô cột 3..8 phải để trống
                    string headerTong = ws.Cell(6, 3).GetString();
                    Assert.AreEqual(string.Empty, headerTong, "Header Section cell should be empty");

                    // Kiểm tra các dòng dữ liệu: nếu giá trị bằng 0 phải hiển thị ký tự '-'
                    bool foundDash = false;
                    for (int r = 7; r <= 31; r++)
                    {
                        string valTong = ws.Cell(r, 3).GetString();
                        if (valTong == "-")
                        {
                            foundDash = true;
                            break;
                        }
                    }
                    Assert.IsTrue(foundDash, "Data rows with 0 value must display '-' character");
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_CalculatePeriodDateRange_ShouldFollow26To25Rule()
        {
            string fromDate, toDate, periodValue;

            // 1. Month 8/2026 -> 26/07/2026 to 25/08/2026
            ReportExportService.CalculatePeriodDateRange("Month", 2026, 8, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/07/2026", fromDate);
            Assert.AreEqual("25/08/2026", toDate);
            Assert.AreEqual("Tháng 08/2026", periodValue);

            // 2. Month 1/2026 -> 26/12/2025 to 25/01/2026
            ReportExportService.CalculatePeriodDateRange("Month", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/12/2025", fromDate);
            Assert.AreEqual("25/01/2026", toDate);
            Assert.AreEqual("Tháng 01/2026", periodValue);

            // 3. Quarter 1/2026 -> 26/12/2025 to 25/03/2026
            ReportExportService.CalculatePeriodDateRange("Quarter", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/12/2025", fromDate);
            Assert.AreEqual("25/03/2026", toDate);
            Assert.AreEqual("Quý I/2026", periodValue);

            // 4. Quarter 2/2026 -> 26/03/2026 to 25/06/2026
            ReportExportService.CalculatePeriodDateRange("Quarter", 2026, 2, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/03/2026", fromDate);
            Assert.AreEqual("25/06/2026", toDate);
            Assert.AreEqual("Quý II/2026", periodValue);

            // 5. Quarter 3/2026 -> 26/06/2026 to 25/09/2026
            ReportExportService.CalculatePeriodDateRange("Quarter", 2026, 3, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/06/2026", fromDate);
            Assert.AreEqual("25/09/2026", toDate);
            Assert.AreEqual("Quý III/2026", periodValue);

            // 6. Quarter 4/2026 -> 26/09/2026 to 25/12/2026
            ReportExportService.CalculatePeriodDateRange("Quarter", 2026, 4, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/09/2026", fromDate);
            Assert.AreEqual("25/12/2026", toDate);
            Assert.AreEqual("Quý IV/2026", periodValue);

            // 7. Year 2026 -> 26/12/2025 to 25/12/2026
            ReportExportService.CalculatePeriodDateRange("Year", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("26/12/2025", fromDate);
            Assert.AreEqual("25/12/2026", toDate);
            Assert.AreEqual("Năm 2026", periodValue);
        }

        [TestMethod]
        public void ScheduledReportDA_CheckDataAvailability_ShouldSupportPeriodTypes()
        {
            var da = new ScheduledReportDA();

            // Month check (26/07/2026 to 25/08/2026)
            bool monthHasData = da.CheckDataAvailability("HOAT_DONG_CD45", 2026, 8, "Month");
            Assert.IsTrue(monthHasData, "August 2026 should have data with 26-25 range");

            // Quarter check (Q3/2026: 26/06/2026 to 25/09/2026)
            bool quarterHasData = da.CheckDataAvailability("HOAT_DONG_CD45", 2026, 3, "Quarter");
            Assert.IsTrue(quarterHasData, "Q3 2026 should have data with 26-25 range");

            // Year check (2026: 26/12/2025 to 25/12/2026)
            bool yearHasData = da.CheckDataAvailability("HOAT_DONG_CD45", 2026, 1, "Year");
            Assert.IsTrue(yearHasData, "Year 2026 should have data with 26-25 range");
        }

        [TestMethod]
        public void ScheduledReportDA_GetExportLogs_WithPeriodTypeFilter_ShouldSucceed()
        {
            var da = new ScheduledReportDA();
            int totalRows;
            var logs = da.GetExportLogs(null, null, null, 1, 10, out totalRows, "Month");

            Assert.IsNotNull(logs, "Logs list with periodType filter should not be null");
            Assert.IsTrue(totalRows >= 0, "Total rows should be >= 0");
        }

        [TestMethod]
        public void CD45KhachHangDA_GetCustomerDetail_ShouldNumberTuVanSequentially()
        {
            var da = new CD45KhachHangDA();
            // Test with customer having both F7 (L1) and F8 (L2) sessions
            var detail = da.GetCustomerDetail("DNA190017");
            Assert.IsNotNull(detail, "Detail should not be null for DNA190017");
            Assert.IsNotNull(detail.ListTuVan, "ListTuVan should not be null");
            Assert.IsTrue(detail.ListTuVan.Count >= 2, "Customer should have at least 2 tu van sessions (F7 + F8)");

            // Verify sequential numbering: 1, 2, 3...
            for (int i = 0; i < detail.ListTuVan.Count; i++)
            {
                Assert.AreEqual(i + 1, detail.ListTuVan[i].SoThuTu, $"Session at index {i} must have SoThuTu = {i + 1}");
            }

            // Verify F7 is first
            Assert.AreEqual(1, detail.ListTuVan[0].LanTuVan, "First session should be F7 (LanTuVan = 1)");
            // Verify F8 is next
            Assert.AreEqual(2, detail.ListTuVan[1].LanTuVan, "Second session should be F8 (LanTuVan = 2)");
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45ExcelAsync_ProvinceScope_NgheAn_ShouldSucceed()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportHoatDongCD45ExcelAsync(2026, 8, "NAN", null, "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export HoatDong for Nghe An should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "Excel file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileName.Contains("NAN"), "FileName should contain province code NAN: " + result.FileName);
                generatedFilePath = result.FilePath;

                using (var wb = new XLWorkbook(result.FilePath))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    Assert.IsNotNull(ws, "Worksheet must exist");

                    string monthTitle = ws.Cell(2, 1).GetString();
                    Assert.AreEqual("Báo cáo tháng 08/2026", monthTitle);
                    string subtitle = ws.Cell(3, 1).GetString();
                    Assert.IsTrue(subtitle.Contains("Nghệ An"), "Subtitle must mention Nghệ An: " + subtitle);

                    // Verify equal column width 10.0 for C..H
                    for (int c = 3; c <= 8; c++)
                    {
                        Assert.AreEqual(10.0, ws.Column(c).Width, 0.1, $"Column {c} width must be 10.0");
                    }

                    // Verify A4 Portrait
                    Assert.AreEqual(XLPaperSize.A4Paper, ws.PageSetup.PaperSize);
                    Assert.AreEqual(XLPageOrientation.Portrait, ws.PageSetup.PageOrientation);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45ExcelAsync_GroupScope_BinhMinh_ShouldSucceed()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportHoatDongCD45ExcelAsync(2026, 8, "HPG", "bm", "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export HoatDong for Binh Minh group should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "Excel file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileName.Contains("HPG") && result.FileName.Contains("bm"), "FileName should contain HPG and bm: " + result.FileName);
                generatedFilePath = result.FilePath;

                using (var wb = new XLWorkbook(result.FilePath))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    Assert.IsNotNull(ws, "Worksheet must exist");

                    string monthTitle = ws.Cell(2, 1).GetString();
                    Assert.AreEqual("Báo cáo tháng 08/2026", monthTitle);
                    string subtitle = ws.Cell(3, 1).GetString();
                    Assert.IsTrue(subtitle.Contains("Bình Minh") || subtitle.Contains("Hải Phòng"), "Subtitle must mention Binh Minh or Hai Phong: " + subtitle);

                    // Verify equal column width 10.0 for C..H
                    for (int c = 3; c <= 8; c++)
                    {
                        Assert.AreEqual(10.0, ws.Column(c).Width, 0.1, $"Column {c} width must be 10.0");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void NhomChucDanh_DefaultValues_And_SignerFormatting_ShouldBeAccurate()
        {
            // HCM default is Giam doc (no double quotes)
            var nhomHcm = new Model.Model.BVTL_NHOM_TBH { city_code = "HCM", tennhom_tbh = "Bầu Trời Xanh" };
            Assert.AreEqual("Giám đốc", nhomHcm.GetChucDanh());
            Assert.AreEqual("Giám đốc Bầu Trời Xanh", nhomHcm.GetDaiDienNhomSigner());

            // The Gate (HCM) default is Giam doc without double quotes
            var nhomTheGate = new Model.Model.BVTL_NHOM_TBH { city_code = "HCM", tennhom_tbh = "The Gate", CHUC_DANH = "Giám đốc" };
            Assert.AreEqual("Giám đốc", nhomTheGate.GetChucDanh());
            Assert.AreEqual("Giám đốc The Gate", nhomTheGate.GetDaiDienNhomSigner());

            // The Time default is Giam doc (no double quotes)
            var nhomTheTime = new Model.Model.BVTL_NHOM_TBH { manhom_tbh = "HN_TT", tennhom_tbh = "The Time" };
            Assert.AreEqual("Giám đốc", nhomTheTime.GetChucDanh());
            Assert.AreEqual("Giám đốc The Time", nhomTheTime.GetDaiDienNhomSigner());

            // Other groups default is Truong nhom (no double quotes)
            var nhomHaiDang = new Model.Model.BVTL_NHOM_TBH { city_code = "HPG", tennhom_tbh = "Hải Đăng" };
            Assert.AreEqual("Trưởng nhóm", nhomHaiDang.GetChucDanh());
            Assert.AreEqual("Trưởng nhóm Hải Đăng", nhomHaiDang.GetDaiDienNhomSigner());

            // Custom ChucDanh override (no double quotes)
            var nhomCustom = new Model.Model.BVTL_NHOM_TBH { CHUC_DANH = "Điều phối viên", tennhom_tbh = "Khát Vọng Sống" };
            Assert.AreEqual("Điều phối viên", nhomCustom.GetChucDanh());
            Assert.AreEqual("Điều phối viên Khát Vọng Sống", nhomCustom.GetDaiDienNhomSigner());

            // CD45_NhomTcvViewModel tests
            var vmHcm = new CD45_NhomTcvViewModel { CITY_CODE = "HCM", TEN_NHOM = "Alocare" };
            Assert.AreEqual("Giám đốc", vmHcm.GetChucDanh());
            Assert.AreEqual("Giám đốc Alocare", vmHcm.GetDaiDienNhomSigner());

            var vmOther = new CD45_NhomTcvViewModel { CITY_CODE = "DNA", TEN_NHOM = "Sông Hàn" };
            Assert.AreEqual("Trưởng nhóm", vmOther.GetChucDanh());
            Assert.AreEqual("Trưởng nhóm Sông Hàn", vmOther.GetDaiDienNhomSigner());
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45ExcelAsync_ShouldIncludeFooterSignatures()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Test for The Time group in Hanoi (HNO, tt)
                var task = service.ExportHoatDongCD45ExcelAsync(2026, 8, "HNO", "tt", "UnitTest", "Tester");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Success, "Export for The Time should succeed");
                Assert.IsTrue(File.Exists(result.FilePath));
                generatedFilePath = result.FilePath;

                using (var wb = new XLWorkbook(result.FilePath))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    Assert.IsNotNull(ws);

                    // Scan column A to find the signature block
                    int lastRow = ws.LastRowUsed().RowNumber();
                    int signRow = -1;
                    for (int r = 5; r <= lastRow; r++)
                    {
                        string cellVal = ws.Cell(r, 1).GetString();
                        if (!string.IsNullOrEmpty(cellVal) && (cellVal.Contains("Giám đốc") || cellVal.Contains("Trưởng nhóm")))
                        {
                            signRow = r;
                            break;
                        }
                    }

                    Assert.IsTrue(signRow > 0, "Signature block must be found in sheet");

                    // 1. Signer 1: Dai dien nhom (Col A:B) -> Giám đốc The Time (No double quotes)
                    string signer1 = ws.Cell(signRow, 1).GetString();
                    Assert.IsTrue(signer1.Contains("Giám đốc"), "Signer 1 should contain Giám đốc: " + signer1);
                    Assert.IsTrue(signer1.Contains("The Time") || signer1.Contains("Time"), "Signer 1 should contain group name: " + signer1);
                    Assert.IsFalse(signer1.Contains("\""), "Signer 1 must NOT contain double quotes: " + signer1);
                    string signer1Note = ws.Cell(signRow + 1, 1).GetString();
                    Assert.AreEqual("(Ký, ghi rõ họ tên)", signer1Note.Trim());

                    // 2. Signer 2: Can bo du an (Col C:E)
                    string signer2 = ws.Cell(signRow, 3).GetString();
                    Assert.AreEqual("Cán bộ dự án", signer2.Trim());
                    string signer2Note = ws.Cell(signRow + 1, 3).GetString();
                    Assert.AreEqual("(Ký, ghi rõ họ tên)", signer2Note.Trim());

                    // 3. Signer 3: MnE (Col F:H)
                    string signer3 = ws.Cell(signRow, 6).GetString();
                    Assert.AreEqual("MnE", signer3.Trim());
                    string signer3Note = ws.Cell(signRow + 1, 6).GetString();
                    Assert.AreEqual("(Ký, ghi rõ họ tên)", signer3Note.Trim());
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45_TheGate_Footer_ShouldBeGiamDocTheGate_WithoutQuotes()
        {
            var service = new ReportExportService();
            string generatedExcelPath = null;
            string generatedPdfPath = null;
            try
            {
                // Test for The Gate in HCM (HCM, HC_TGA)
                var taskExcel = service.ExportHoatDongCD45ExcelAsync(2026, 8, "HCM", "HC_TGA", "UnitTest", "Tester", "EXCEL");
                var resultExcel = taskExcel.GetAwaiter().GetResult();

                Assert.IsNotNull(resultExcel);
                Assert.IsTrue(resultExcel.Success, "Export Excel for The Gate should succeed: " + resultExcel.Message);
                Assert.IsTrue(File.Exists(resultExcel.FilePath));
                generatedExcelPath = resultExcel.FilePath;

                using (var wb = new XLWorkbook(resultExcel.FilePath))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    Assert.IsNotNull(ws);

                    int lastRow = ws.LastRowUsed().RowNumber();
                    int signRow = -1;
                    for (int r = 5; r <= lastRow; r++)
                    {
                        string cellVal = ws.Cell(r, 1).GetString();
                        if (!string.IsNullOrEmpty(cellVal) && (cellVal.Contains("Giám đốc") || cellVal.Contains("Trưởng nhóm")))
                        {
                            signRow = r;
                            break;
                        }
                    }

                    Assert.IsTrue(signRow > 0, "Signature block must be found in sheet");

                    string signer1 = ws.Cell(signRow, 1).GetString();
                    // Must be 'Giám đốc The Gate' exactly without any double quotes
                    Assert.AreEqual("Giám đốc The Gate", signer1.Trim(), "Signer 1 must be 'Giám đốc The Gate' without double quotes");
                    Assert.IsFalse(signer1.Contains("\""), "Signer 1 must NOT contain double quotes: " + signer1);
                }

                // For The Gate in HCM, if no data in period, PDF correctly skips per business rules
                var taskPdf = service.ExportHoatDongCD45ExcelAsync(2026, 8, "HCM", "HC_TGA", "UnitTest", "Tester", "PDF");
                var resultPdf = taskPdf.GetAwaiter().GetResult();
                Assert.IsNotNull(resultPdf);
                if (resultPdf.Success && File.Exists(resultPdf.FilePath))
                {
                    generatedPdfPath = resultPdf.FilePath;
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedExcelPath) && File.Exists(generatedExcelPath))
                {
                    try { File.Delete(generatedExcelPath); } catch { }
                }
                if (!string.IsNullOrEmpty(generatedPdfPath) && File.Exists(generatedPdfPath))
                {
                    try { File.Delete(generatedPdfPath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ScheduledReportDA_CheckDataAvailability_BacSiCD45_ShouldReturnTrue()
        {
            var da = new ScheduledReportDA();
            bool hasData = da.CheckDataAvailability("BACSI_CD45", 2026, 9);
            Assert.IsTrue(hasData, "September 2026 should have doctor consultation data in CD45_CHAN_DOAN");
        }

        [TestMethod]
        public void ReportExportService_ExportBacSiCD45ExcelAsync_ForSingleProvince_HaNoi_ShouldCreateExcelWithDoctorSheets()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Test xuất báo cáo Bác sĩ tháng 9/2026 tỉnh Hà Nội (HNO)
                var task = service.ExportBacSiCD45ExcelAsync(2026, 9, "HNO", "UnitTest", "Tester", "Month");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export for Hanoi should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "Excel file should exist: " + result.FilePath);
                Assert.IsTrue(result.FilePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase), "File should be .xlsx");
                Assert.IsTrue(result.TotalItems > 0, "Should have exported doctor consultation cases");
                generatedFilePath = result.FilePath;

                using (var wb = new XLWorkbook(result.FilePath))
                {
                    Assert.IsTrue(wb.Worksheets.Count > 0, "Workbook must contain at least 1 worksheet");

                    // Kiểm tra cấu trúc của từng Sheet (mỗi Sheet tương ứng với một Bác sĩ)
                    foreach (var ws in wb.Worksheets)
                    {
                        // 1. Header: Tiêu đề báo cáo & Tháng báo cáo
                        string title = ws.Cell(1, 1).GetString();
                        Assert.IsTrue(title.Contains("DANH SÁCH KHÁCH HÀNG ĐƯỢC NHẬP LIỆU TRÊN REDCAP DỰ ÁN DREAMH"), 
                            "Title must match user specification: " + title);

                        string reportMonth = ws.Cell(2, 1).GetString();
                        Assert.IsTrue(reportMonth.Contains("Báo cáo tháng 09/2026"), "Cell A2 must contain 'Báo cáo tháng 09/2026'");

                        // 2. Header: Tỉnh, Bác sĩ, Tháng (Merge Cột A & B cho Tiêu đề, Cột C & D cho Giá trị)
                        Assert.IsTrue(ws.Range(4, 1, 4, 2).IsMerged(), "Range A4:B4 should be merged for Province label");
                        string provinceLabel = ws.Cell(4, 1).GetString();
                        Assert.IsTrue(ws.Range(4, 3, 4, 4).IsMerged(), "Range C4:D4 should be merged for Province value");
                        string provinceVal = ws.Cell(4, 3).GetString();
                        Assert.IsTrue(provinceLabel.Contains("Tỉnh"), "Cell (4,1) should be Province label");
                        Assert.IsTrue(provinceVal.Contains("Hà Nội"), "Cell (4,3) should be Hà Nội");

                        string periodText = ws.Cell(4, 7).GetString();
                        Assert.IsTrue(periodText.Contains("Từ 01/09/2026 đến 30/09/2026"), 
                            "Doctor report period in Header must be from first day to last day of month: " + periodText);

                        Assert.IsTrue(ws.Range(5, 1, 5, 2).IsMerged(), "Range A5:B5 should be merged for Doctor label");
                        string doctorLabel = ws.Cell(5, 1).GetString();
                        Assert.IsTrue(ws.Range(5, 3, 5, 4).IsMerged(), "Range C5:D5 should be merged for Doctor value");
                        string doctorVal = ws.Cell(5, 3).GetString();
                        Assert.IsTrue(doctorLabel.Contains("Bác sĩ"), "Cell (5,1) should be Doctor label");
                        Assert.IsFalse(string.IsNullOrWhiteSpace(doctorVal), "Doctor name should not be empty");

                        // 3. Header bảng chi tiết ca khám (Dòng 7)
                        Assert.AreEqual("STT", ws.Cell(7, 1).GetString());
                        Assert.AreEqual("Mã Khách hàng", ws.Cell(7, 2).GetString());
                        Assert.AreEqual("Ngày khám", ws.Cell(7, 3).GetString());
                        Assert.AreEqual("Cơ sở khám / Bệnh viện", ws.Cell(7, 4).GetString());
                        Assert.AreEqual("Nhóm CBO", ws.Cell(7, 5).GetString());
                        Assert.AreEqual("Tiếp cận viên (TCV)", ws.Cell(7, 6).GetString());
                        Assert.AreEqual("Lần khám", ws.Cell(7, 7).GetString());
                        Assert.AreEqual("Chẩn đoán chính", ws.Cell(7, 8).GetString());
                        Assert.AreEqual("Hình thức điều trị", ws.Cell(7, 9).GetString());

                        // 4. Footer: Tìm chữ 'Xác nhận của SCDI'
                        int lastRow = ws.LastRowUsed().RowNumber();
                        int footerRow = -1;
                        for (int r = 8; r <= lastRow; r++)
                        {
                            string cellVal = ws.Cell(r, 7).GetString();
                            if (!string.IsNullOrEmpty(cellVal) && cellVal.Contains("Xác nhận của SCDI"))
                            {
                                footerRow = r;
                                break;
                            }
                        }

                        Assert.IsTrue(footerRow > 0, "Footer 'Xác nhận của SCDI' must be found in worksheet: " + ws.Name);

                        // Kiểm tra ngày tháng năm footer
                        string dateFooter = ws.Cell(footerRow - 1, 7).GetString();
                        Assert.IsTrue(dateFooter.Contains("Hà Nội"), "Footer date should contain province name: " + dateFooter);
                        Assert.IsTrue(dateFooter.Contains("Ngày") && dateFooter.Contains("tháng") && dateFooter.Contains("năm"), 
                            "Footer date format invalid: " + dateFooter);

                        // Kiểm tra chữ ký SCDI
                        string scdiSignNote = ws.Cell(footerRow + 1, 7).GetString();
                        Assert.AreEqual("(Ký, ghi rõ họ tên)", scdiSignNote.Trim());

                        // Kiểm tra khối Bác sĩ phụ trách bên trái
                        string docFooterTitle = ws.Cell(footerRow, 1).GetString();
                        Assert.AreEqual("Bác sĩ phụ trách", docFooterTitle.Trim());
                        string docFooterName = ws.Cell(footerRow + 5, 1).GetString();
                        Assert.AreEqual(doctorVal, docFooterName.Trim(), "Doctor name in footer must match header");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportBacSiCD45ExcelAsync_ForAllProvinces_ShouldCreateZipWithProvinceExcels()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Test xuất tất cả các tỉnh (cityCode = null) cho tháng 9/2026 -> sinh file .ZIP
                var task = service.ExportBacSiCD45ExcelAsync(2026, 9, null, "UnitTest", "Tester", "Month");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export all provinces should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist: " + result.FilePath);
                Assert.IsTrue(result.FilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase), "File should be .zip");
                Assert.IsTrue(result.TotalItems > 0, "Should have exported cases across all provinces");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    Assert.IsTrue(archive.Entries.Count >= 6, $"Archive should contain at least 6 province files, found {archive.Entries.Count}");

                    bool hasHanoi = false;
                    bool hasHcm = false;
                    bool hasHaiPhong = false;

                    foreach (var entry in archive.Entries)
                    {
                        Assert.IsTrue(entry.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase), "All entries in zip should be Excel files: " + entry.Name);
                        if (entry.Name.Contains("HaNoi")) hasHanoi = true;
                        if (entry.Name.Contains("HoChiMinh")) hasHcm = true;
                        if (entry.Name.Contains("HaiPhong")) hasHaiPhong = true;
                    }

                    Assert.IsTrue(hasHanoi, "ZIP should contain Hanoi excel report");
                    Assert.IsTrue(hasHcm, "ZIP should contain HCM excel report");
                    Assert.IsTrue(hasHaiPhong, "ZIP should contain Hai Phong excel report");

                    // Mở thử 1 file Excel bên trong ZIP để verify
                    var hanoiEntry = archive.Entries.FirstOrDefault(e => e.Name.Contains("HaNoi"));
                    Assert.IsNotNull(hanoiEntry, "Hanoi entry should exist");
                    using (var entryStream = hanoiEntry.Open())
                    using (var ms = new MemoryStream())
                    {
                        entryStream.CopyTo(ms);
                        ms.Position = 0;
                        using (var wb = new XLWorkbook(ms))
                        {
                            Assert.IsTrue(wb.Worksheets.Count > 0, "Hanoi excel inside ZIP should have worksheets");
                            var firstWs = wb.Worksheets.First();
                            string title = firstWs.Cell(1, 1).GetString();
                            Assert.IsTrue(title.Contains("DANH SÁCH KHÁCH HÀNG ĐƯỢC NHẬP LIỆU TRÊN REDCAP DỰ ÁN DREAMH"));
                        }
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_CalculateDoctorPeriodDateRange_ShouldFollowCalendarMonthRule()
        {
            string fromDate, toDate, periodValue;

            // Month 9/2026 (September has 30 days)
            ReportExportService.CalculateDoctorPeriodDateRange("Month", 2026, 9, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/09/2026", fromDate);
            Assert.AreEqual("30/09/2026", toDate);
            Assert.AreEqual("Tháng 09/2026", periodValue);

            // Month 1/2026 (January has 31 days)
            ReportExportService.CalculateDoctorPeriodDateRange("Month", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/01/2026", fromDate);
            Assert.AreEqual("31/01/2026", toDate);
            Assert.AreEqual("Tháng 01/2026", periodValue);

            // Month 2/2024 (Leap year has 29 days)
            ReportExportService.CalculateDoctorPeriodDateRange("Month", 2024, 2, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/02/2024", fromDate);
            Assert.AreEqual("29/02/2024", toDate);
            Assert.AreEqual("Tháng 02/2024", periodValue);

            // Month 2/2026 (Non-leap year has 28 days)
            ReportExportService.CalculateDoctorPeriodDateRange("Month", 2026, 2, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/02/2026", fromDate);
            Assert.AreEqual("28/02/2026", toDate);
            Assert.AreEqual("Tháng 02/2026", periodValue);

            // Quarter 1/2026
            ReportExportService.CalculateDoctorPeriodDateRange("Quarter", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/01/2026", fromDate);
            Assert.AreEqual("31/03/2026", toDate);
            Assert.AreEqual("Quý I/2026", periodValue);

            // Quarter 2/2026
            ReportExportService.CalculateDoctorPeriodDateRange("Quarter", 2026, 2, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/04/2026", fromDate);
            Assert.AreEqual("30/06/2026", toDate);
            Assert.AreEqual("Quý II/2026", periodValue);

            // Quarter 3/2026
            ReportExportService.CalculateDoctorPeriodDateRange("Quarter", 2026, 3, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/07/2026", fromDate);
            Assert.AreEqual("30/09/2026", toDate);
            Assert.AreEqual("Quý III/2026", periodValue);

            // Quarter 4/2026
            ReportExportService.CalculateDoctorPeriodDateRange("Quarter", 2026, 4, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/10/2026", fromDate);
            Assert.AreEqual("31/12/2026", toDate);
            Assert.AreEqual("Quý IV/2026", periodValue);

            // Year 2026
            ReportExportService.CalculateDoctorPeriodDateRange("Year", 2026, 1, out fromDate, out toDate, out periodValue);
            Assert.AreEqual("01/01/2026", fromDate);
            Assert.AreEqual("31/12/2026", toDate);
            Assert.AreEqual("Năm 2026", periodValue);
        }

        [TestMethod]
        public void BaoCaoBacSiCD45_TongHop_And_ChiTiet_ShouldMatch_WithoutDuplicates()
        {
            var da = new BaoCaoBacSiCD45DA();

            // 1. Kiểm tra toàn bộ dữ liệu dự án không filter
            var summaryAll = da.GetBaoCaoTongHop(null, null, null, null, null);
            var detailsAll = da.GetBaoCaoChiTiet(null, null, null, null, null);

            Assert.IsNotNull(summaryAll);
            Assert.IsNotNull(detailsAll);
            int sumCasesAll = summaryAll.Sum(x => x.TongSoCa);
            Assert.AreEqual(detailsAll.Count, sumCasesAll, "Tổng số ca Tổng hợp và Chi tiết toàn dự án phải khớp 100%");

            // Kiểm tra không có duplicate ID trong chi tiết
            var duplicateKeys = detailsAll
                .GroupBy(x => x.ID)
                .Where(g => g.Count() > 1)
                .ToList();
            Assert.AreEqual(0, duplicateKeys.Count, "Báo cáo chi tiết không được chứa bất kỳ bản ghi duplicate nào");

            // 2. Kiểm tra theo từng tỉnh
            string[] testCities = { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" };
            foreach (var city in testCities)
            {
                var summaryCity = da.GetBaoCaoTongHop(null, null, city, null, null);
                var detailsCity = da.GetBaoCaoChiTiet(null, null, city, null, null);
                int sumCity = summaryCity.Sum(x => x.TongSoCa);
                Assert.AreEqual(detailsCity.Count, sumCity, $"Số ca Tổng hợp và Chi tiết của tỉnh {city} phải khớp 100%");
            }

            // 3. Kiểm tra theo khoảng thời gian tháng 9/2026
            var summarySep = da.GetBaoCaoTongHop("01/09/2026", "30/09/2026", null, null, null);
            var detailsSep = da.GetBaoCaoChiTiet("01/09/2026", "30/09/2026", null, null, null);
            Assert.AreEqual(detailsSep.Count, summarySep.Sum(x => x.TongSoCa), "Số ca Tháng 09/2026 giữa Tổng hợp và Chi tiết phải khớp 100%");
        }

        [TestMethod]
        public void BaoCaoBacSiDetailModel_FormatHinhThucDieuTri_ShouldMapCorrectly()
        {
            // Đơn giá trị
            Assert.AreEqual("Ngoại trú", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("1"));
            Assert.AreEqual("Nội trú", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("2"));
            Assert.AreEqual("Tư vấn tâm lý", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("3"));
            Assert.AreEqual("Khác", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("4"));

            // Đa giá trị phân tách dấu phẩy hoặc chấm phẩy
            Assert.AreEqual("Ngoại trú, Tư vấn tâm lý", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("1,3"));
            Assert.AreEqual("Ngoại trú, Tư vấn tâm lý", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("1, 3"));
            Assert.AreEqual("Ngoại trú, Nội trú, Tư vấn tâm lý", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("1,2,3"));
            Assert.AreEqual("Ngoại trú, Khác", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("1;4"));

            // Trường hợp null, rỗng, khoảng trắng
            Assert.AreEqual("-", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri(null));
            Assert.AreEqual("-", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri(""));
            Assert.AreEqual("-", BaoCaoBacSiDetailModel.FormatHinhThucDieuTri("   "));

            // Property TEN_HINH_THUC_DIEU_TRI trên instance
            var model = new BaoCaoBacSiDetailModel { HINH_THUC_DIEU_TRI = "1,3" };
            Assert.AreEqual("Ngoại trú, Tư vấn tâm lý", model.TEN_HINH_THUC_DIEU_TRI);
        }

        [TestMethod]
        public void BaoCaoBacSiCD45_GroupFilter_ShouldReturnDoctorsAndMatchDetail()
        {
            var da = new BaoCaoBacSiCD45DA();

            // 1. Lọc theo nhóm chuẩn The Times (HN_TT)
            var summaryHnTT = da.GetBaoCaoTongHop(null, null, null, null, "HN_TT");
            var detailsHnTT = da.GetBaoCaoChiTiet(null, null, null, null, "HN_TT");

            Assert.IsNotNull(summaryHnTT);
            Assert.IsNotNull(detailsHnTT);
            Assert.IsTrue(summaryHnTT.Count > 0, "Nhóm HN_TT phải có bác sĩ tham gia khám");
            Assert.IsTrue(detailsHnTT.Count > 0, "Nhóm HN_TT phải có ca khám chi tiết");
            Assert.AreEqual(detailsHnTT.Count, summaryHnTT.Sum(x => x.TongSoCa), "Tổng số ca giữa Tổng hợp và Chi tiết của nhóm HN_TT phải khớp 100%");
            Assert.IsTrue(detailsHnTT.All(x => x.MA_NHOM == "tt" || x.TEN_NHOM == "The Times"), "Tất cả ca khám phải thuộc nhóm The Times (tt)");

            // 2. Lọc theo nhóm REDCap map code (tt) - kết quả phải đồng nhất
            var summaryTT = da.GetBaoCaoTongHop(null, null, null, null, "tt");
            var detailsTT = da.GetBaoCaoChiTiet(null, null, null, null, "tt");
            Assert.AreEqual(summaryHnTT.Count, summaryTT.Count, "Lọc theo mã chuẩn hay mã map phải trả về cùng số lượng bác sĩ");
            Assert.AreEqual(detailsHnTT.Count, detailsTT.Count, "Lọc theo mã chuẩn hay mã map phải trả về cùng số lượng ca khám chi tiết");

            // 3. Lọc theo nhóm Alocare (HC_ALO)
            var summaryAlo = da.GetBaoCaoTongHop(null, null, null, null, "HC_ALO");
            var detailsAlo = da.GetBaoCaoChiTiet(null, null, null, null, "HC_ALO");
            Assert.IsNotNull(summaryAlo);
            Assert.IsNotNull(detailsAlo);
            Assert.IsTrue(summaryAlo.Count > 0, "Nhóm HC_ALO phải có bác sĩ tham gia khám");
            Assert.IsTrue(detailsAlo.Count > 0, "Nhóm HC_ALO phải có ca khám chi tiết");
            Assert.AreEqual(detailsAlo.Count, summaryAlo.Sum(x => x.TongSoCa), "Tổng số ca giữa Tổng hợp và Chi tiết của nhóm HC_ALO phải khớp 100%");
            Assert.IsTrue(detailsAlo.All(x => x.MA_NHOM == "alo" || x.TEN_NHOM == "Alocare"), "Tất cả ca khám phải thuộc nhóm Alocare (alo)");
        }

        [TestMethod]
        public void BaoCaoBacSiCD45Controller_SearchBaoCao_WithGroupFilter_ShouldReturnSummaryAndDetails()
        {
            var cityDa = new CityDA();
            var nhomDa = new BVTL_NHOM_TBHDA();
            var bsDa = new BaoCaoBacSiCD45DA();
            var controller = new WebApp.Controllers.BaoCaoBacSiCD45Controller(cityDa, nhomDa, bsDa);

            var result = controller.SearchBaoCao("01/01/2024", "31/12/2026", null, null, "HN_TT") as System.Web.Mvc.JsonResult;
            Assert.IsNotNull(result);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
            Assert.IsTrue(json.Contains("\"Success\":true"));
            Assert.IsTrue(json.Contains("Summary"));
            Assert.IsTrue(json.Contains("Details"));
            Assert.IsFalse(json.Contains("\"Summary\":[]"), "Summary khi lọc nhóm HN_TT không được rỗng");
            Assert.IsFalse(json.Contains("\"Details\":[]"), "Details khi lọc nhóm HN_TT không được rỗng");
        }

        [TestMethod]
        public void BaoCaoBacSiCD45_GetBaoCaoChiTiet_ShouldEnrichDiagnoseNames_IncludingCode76And98()
        {
            var da = new BaoCaoBacSiCD45DA();

            // 1. Kiểm tra toàn bộ ca khám trong CSDL có chẩn đoán chính thì TEN_CHAN_DOAN phải được ánh xạ tên tiếng Việt, không bị để raw ID
            var allDetails = da.GetBaoCaoChiTiet(null, null, null, null, null);
            Assert.IsNotNull(allDetails);
            Assert.IsTrue(allDetails.Count > 0);

            // Kiểm tra các ca có chẩn đoán 76 (F16.7)
            var cases76 = allDetails.Where(x => x.CHAN_DOAN_CHINH == "76").ToList();
            Assert.IsTrue(cases76.Count > 0, "Cơ sở dữ liệu phải có ca khám có mã chẩn đoán 76");
            foreach (var c in cases76)
            {
                Assert.IsTrue(c.TEN_CHAN_DOAN.StartsWith("F16.7"), "Mã chẩn đoán 76 phải bắt đầu bằng F16.7");
                Assert.IsTrue(c.TEN_CHAN_DOAN.Contains("ảo giác"), "Mã 76 phải chứa tên chất gây ảo giác");
            }

            // Kiểm tra các ca có chẩn đoán 98 (F19.7)
            var cases98 = allDetails.Where(x => x.CHAN_DOAN_CHINH == "98").ToList();
            Assert.IsTrue(cases98.Count > 0, "Cơ sở dữ liệu phải có ca khám có mã chẩn đoán 98");
            foreach (var c in cases98)
            {
                Assert.IsTrue(c.TEN_CHAN_DOAN.StartsWith("F19.7"), "Mã chẩn đoán 98 phải bắt đầu bằng F19.7");
            }

            // Kiểm tra không có bất kỳ ca nào có CHAN_DOAN_CHINH mà TEN_CHAN_DOAN lại bằng chính mã số (chưa map)
            var unmappedCases = allDetails
                .Where(x => !string.IsNullOrEmpty(x.CHAN_DOAN_CHINH) && x.TEN_CHAN_DOAN == x.CHAN_DOAN_CHINH)
                .ToList();
            Assert.AreEqual(0, unmappedCases.Count, "Tất cả các ca khám có chẩn đoán chính phải được map tên 100%, không được để sót mã số thuần túy");
        }

        [TestMethod]
        public void CD45KhachHangDA_FilterChuDeSinhHoatNhom_AllTopics_ShouldFilterCorrectly()
        {
            var khDA = new CD45KhachHangDA();

            // 0. Tổng số khách hàng toàn bộ dự án
            var allResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { PageIndex = 1, PageSize = 10 });
            Assert.IsNotNull(allResult);
            int totalAll = allResult.recordsTotal;
            Assert.IsTrue(totalAll > 0, "Tổng số KH toàn dự án phải > 0");

            // STT 1: PTSD (PCL5_POSITIVE = 1)
            var ptsdResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "PTSD", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(ptsdResult.recordsTotal > 0 && ptsdResult.recordsTotal <= totalAll, "PTSD phải có KH thỏa mãn");
            Assert.IsTrue(ptsdResult.recordsTotal >= 718, "PTSD phai co it nhat 718 KH");

            // STT 2: Rối loạn sử dụng chất (CHAT: QA2 = 1)
            var chatResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHAT", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(chatResult.recordsTotal > 0 && chatResult.recordsTotal <= totalAll, "CHAT phải có KH thỏa mãn");
            Assert.IsTrue(chatResult.recordsTotal >= 541, "chatResult phai co it nhat 541 KH");

            // STT 3: Chemsex (CHEMSEX: QA2 = 1 AND QA5 IN (2, 3, 4))
            var chemsexResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHEMSEX", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(chemsexResult.recordsTotal > 0 && chemsexResult.recordsTotal <= chatResult.recordsTotal, "CHEMSEX là tập con của CHAT");
            Assert.IsTrue(chemsexResult.recordsTotal >= 484, "chemsexResult phai co it nhat 484 KH");

            // STT 4: Người bán dâm (SW: DOI_TUONG = 5 OR DOI_TUONG_KHAC có 5)
            var swResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "SW", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(swResult.recordsTotal > 0 && swResult.recordsTotal <= totalAll, "SW phải có KH thỏa mãn");
            Assert.IsTrue(swResult.recordsTotal >= 611, "swResult phai co it nhat 611 KH");

            // STT 5: Người có HIV (PLHIV: DOI_TUONG = 2 OR DOI_TUONG_KHAC có 2)
            var plhivResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "PLHIV", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(plhivResult.recordsTotal > 0 && plhivResult.recordsTotal <= totalAll, "PLHIV phải có KH thỏa mãn");
            Assert.IsTrue(plhivResult.recordsTotal >= 580, "plhivResult phai co it nhat 580 KH");

            // STT 6 & 7: Người chuyển giới (TG_VAN_DE & TG_HORMONE: DOI_TUONG = 3 OR DOI_TUONG_KHAC có 3)
            var tgVanDeResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "TG_VAN_DE", PageIndex = 1, PageSize = 10 });
            var tgHormoneResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "TG_HORMONE", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(tgVanDeResult.recordsTotal >= 135, "tgVanDeResult phai co it nhat 135 KH");
            Assert.IsTrue(tgHormoneResult.recordsTotal >= 135, "tgHormoneResult phai co it nhat 135 KH");

            // STT 8: Kỳ thị và tự kỳ thị (KY_THI: Tất cả KH dự án)
            var kyThiResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "KY_THI", PageIndex = 1, PageSize = 10 });
            Assert.AreEqual(totalAll, kyThiResult.recordsTotal, "KY_THI không lọc điều kiện phụ, phải bằng tổng số KH");

            // Kiểm tra hàm GetAllForExport cũng áp dụng đúng bộ lọc
            var exportChemsex = khDA.GetAllForExport(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHEMSEX" });
            Assert.IsTrue(exportChemsex.Count >= 484, "Export danh sach CHEMSEX phai co it nhat 484 KH");
        }

        [TestMethod]
        public void BaoCaoTuVanCD45DA_GetBaoCaoTuVan_L1_And_L2_ShouldReturnData()
        {
            var da = new BaoCaoTuVanCD45DA();

            // 1. Lần 1 (F7)
            var listL1 = da.GetBaoCaoTuVanL1("01/01/2024", "31/12/2026", null, null, null);
            Assert.IsNotNull(listL1, "Danh sách tư vấn L1 không được null");
            Assert.IsTrue(listL1.Count > 0, "Dự án phải có dữ liệu tư vấn L1");
            var itemL1 = listL1.FirstOrDefault(x => !string.IsNullOrEmpty(x.Q3A_TEXT) || !string.IsNullOrEmpty(x.Q3C_NHU_CAU));
            Assert.IsNotNull(itemL1, "Phải có ít nhất 1 ca tư vấn L1 có nội dung Q3a hoặc Q3c");
            Assert.IsFalse(string.IsNullOrEmpty(itemL1.RECORD_ID), "Record ID không được rỗng");

            // 2. Lần 2+ (F8)
            var listL2 = da.GetBaoCaoTuVanL2("01/01/2024", "31/12/2026", null, null, null);
            Assert.IsNotNull(listL2, "Danh sách tư vấn L2 không được null");
            Assert.IsTrue(listL2.Count > 0, "Dự án phải có dữ liệu tư vấn L2+");
            var itemL2 = listL2.FirstOrDefault(x => !string.IsNullOrEmpty(x.Q21A_CAM_THAY) || !string.IsNullOrEmpty(x.Q32_CAN_THIEP_CHINH));
            Assert.IsNotNull(itemL2, "Phải có ít nhất 1 ca tư vấn L2+ có nội dung");
            Assert.IsTrue(!string.IsNullOrEmpty(itemL2.LAN_TU_VAN), "Lần tư vấn phải được điền nhãn (VD: Lần 2)");
        }

        [TestMethod]
        public void ExcelTuVanHelper_BuildWorksheets_ShouldCreateExpectedSheetsAndColumns()
        {
            var da = new BaoCaoTuVanCD45DA();
            var listL1 = da.GetBaoCaoTuVanL1("01/01/2024", "31/12/2026", null, null, null);
            var listL2 = da.GetBaoCaoTuVanL2("01/01/2024", "31/12/2026", null, null, null);

            using (var wb = new XLWorkbook())
            {
                var ws1 = wb.Worksheets.Add("TuVan_L1");
                ExcelTuVanHelper.BuildTuVanL1Worksheet(ws1, listL1.Take(5).ToList(), "01/01/2024", "31/12/2026", "Nhóm Test", "TCV Test");

                Assert.AreEqual("BÁO CÁO CHI TIẾT TƯ VẤN LẦN 1 (F7) - DỰ ÁN CD45", ws1.Cell("A1").GetString());
                Assert.AreEqual("Báo cáo tháng 12/2026", ws1.Cell("A2").GetString());
                Assert.AreEqual("Kỳ báo cáo: Từ 01/01/2024 đến 31/12/2026", ws1.Cell("A3").GetString());
                Assert.AreEqual("STT", ws1.Cell("A6").GetString());
                Assert.AreEqual("Record ID", ws1.Cell("B6").GetString());
                Assert.AreEqual("Tỉnh / Thành phố", ws1.Cell("C6").GetString());
                Assert.AreEqual("6.1 Ngày hẹn tiếp theo", ws1.Cell("N6").GetString());
                Assert.AreEqual(14, ws1.Range("A6:N6").ColumnCount(), "Sheet L1 phải có đủ 14 cột sau khi bỏ 3b và 3b.1");

                var ws2 = wb.Worksheets.Add("TuVan_L2");
                ExcelTuVanHelper.BuildTuVanL2Worksheet(ws2, listL2.Take(5).ToList(), "01/01/2024", "31/12/2026", "Nhóm Test", "TCV Test");

                Assert.AreEqual("BÁO CÁO CHI TIẾT TƯ VẤN TỪ LẦN 2 TRỞ ĐI (F8) - DỰ ÁN CD45", ws2.Cell("A1").GetString());
                Assert.AreEqual("Báo cáo tháng 12/2026", ws2.Cell("A2").GetString());
                Assert.AreEqual("Kỳ báo cáo: Từ 01/01/2024 đến 31/12/2026", ws2.Cell("A3").GetString());
                Assert.AreEqual("STT", ws2.Cell("A6").GetString());
                Assert.AreEqual("Record ID", ws2.Cell("B6").GetString());
                Assert.AreEqual("Lần tư vấn", ws2.Cell("C6").GetString());
                Assert.AreEqual("Ngày hẹn tiếp theo", ws2.Cell("N6").GetString());
                Assert.AreEqual(14, ws2.Range("A6:N6").ColumnCount(), "Sheet L2 phải có đủ 14 cột sau khi bỏ 9 cột");
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    Assert.IsTrue(ms.Length > 0, "Workbook phải lưu thành công và có dung lượng > 0");
                }
            }
        }

        [TestMethod]
        public void BaoCaoTuVanCD45Controller_SearchTuVan_ShouldReturnJsonWithBothTypes()
        {
            var da = new BaoCaoTuVanCD45DA();
            var controller = new WebApp.Controllers.BaoCaoTuVanCD45Controller(new CityDA(), new BVTL_NHOM_TBHDA(), new BaoCaoCD45DA(), da);
            
            var result = controller.SearchTuVan("01/01/2024", "31/12/2026", null, null, null, "ALL") as System.Web.Mvc.JsonResult;
            Assert.IsNotNull(result, "Result must not be null");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
            Assert.IsTrue(json.Contains("\"Success\":true"), "SearchTuVan phải thành công");
            Assert.IsTrue(json.Contains("\"CountL1\":"), "CountL1 phải có trong kết quả");
            Assert.IsTrue(json.Contains("\"CountL2\":"), "CountL2 phải có trong kết quả");
            Assert.IsFalse(json.Contains("\"CountL1\":0"), "Dự án phải có ca tư vấn L1");
            Assert.IsFalse(json.Contains("\"CountL2\":0"), "Dự án phải có ca tư vấn L2+");
        }

        [TestMethod]
        public void BaoCaoTuVanCD45Controller_SearchTuVan_WithSpecificTcv_ShouldFilterOnlyTargetTcv()
        {
            var tcvDa = new BaoCaoCD45DA();
            var allTcvs = tcvDa.GetListTCV(null, null);
            var tcvLuan = allTcvs.FirstOrDefault(x => x.TEN_TCV != null && x.TEN_TCV.Contains("Luận"));
            Assert.IsNotNull(tcvLuan, "Phải tìm thấy TCV Đỗ Thị Luận trong danh mục");

            var da = new BaoCaoTuVanCD45DA();
            var controller = new WebApp.Controllers.BaoCaoTuVanCD45Controller(new CityDA(), new BVTL_NHOM_TBHDA(), tcvDa, da);
            
            // Tìm theo tcvId của Đỗ Thị Luận khi chưa chọn nhóm và tỉnh
            var result = controller.SearchTuVan("26/09/2026", "25/10/2026", null, null, null, "ALL", tcvLuan.ID) as System.Web.Mvc.JsonResult;
            Assert.IsNotNull(result, "Result must not be null");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
            Assert.IsTrue(json.Contains("\"Success\":true"), "SearchTuVan phải thành công");
            Assert.IsTrue(json.Contains("DHN020066"), "Phải chứa Record ID của Đỗ Thị Luận (DHN020066)");
            Assert.IsFalse(json.Contains("Nguyễn Thị Xuân Phúc"), "Tuyệt đối không được chứa ca của TCV khác (Nguyễn Thị Xuân Phúc)");
            Assert.IsFalse(json.Contains("Nguyễn Thị Loan"), "Tuyệt đối không được chứa ca của TCV khác (Nguyễn Thị Loan)");
        }

        [TestMethod]
        public void ReportExportService_ExportTCVCD45PdfZipAsync_ShouldFilterEmptyAndExportPdf()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Xuất thử nghiệm PDF cho The Times group (HNO - tt) tháng 08/2026
                var task = service.ExportTCVCD45ZipAsync(2026, 8, "HNO", "tt", "UnitTest", "Tester", format: "PDF");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export PDF should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileName.EndsWith("_PDF.zip", StringComparison.OrdinalIgnoreCase), "File name should end with _PDF.zip");
                Assert.IsTrue(result.FileSizeBytes > 0, "ZIP file size > 0");
                Assert.IsTrue(result.TotalItems > 0, "Total items exported > 0");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    Assert.IsTrue(archive.Entries.Count > 0, "ZIP must contain entries");

                    bool hasProvincePdf = false;
                    bool hasGroupPdf = false;
                    bool hasTcvPdf = false;

                    foreach (var entry in archive.Entries)
                    {
                        Assert.IsTrue(entry.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase), $"Every entry inside PDF zip must be .pdf, found: {entry.FullName}");
                        Assert.IsFalse(entry.FullName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase), $"No .xlsx file should be in PDF zip: {entry.FullName}");

                        if (entry.FullName.Contains("BaoCao_TongHop_HNO.pdf")) hasProvincePdf = true;
                        if (entry.FullName.Contains("BaoCao_TongHop_") && !entry.FullName.Contains("BaoCao_TongHop_HNO")) hasGroupPdf = true;
                        if (entry.FullName.Contains("BaoCao_TCV_")) hasTcvPdf = true;
                    }

                    Assert.IsTrue(hasProvincePdf, "ZIP should contain Province PDF report (BaoCao_TongHop_HNO.pdf)");
                    Assert.IsTrue(hasGroupPdf, "ZIP should contain Group PDF report (BaoCao_TongHop_...)");
                    Assert.IsTrue(hasTcvPdf, "ZIP should contain individual TCV PDF reports (BaoCao_TCV_...)");
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportBacSiCD45PdfZipAsync_SingleProvince_EachDoctorOnePdf()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Xuất PDF Bác sĩ tỉnh Hà Nội tháng 08/2026
                var task = service.ExportBacSiCD45ExcelAsync(2026, 8, "HNO", "UnitTest", "Tester", format: "PDF");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export Doctor PDF for HNO should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileName.EndsWith("_PDF.zip", StringComparison.OrdinalIgnoreCase), "File name should end with _PDF.zip");
                Assert.IsTrue(result.TotalItems > 0, "Total doctor consultation cases should be > 0");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    Assert.IsTrue(archive.Entries.Count > 0, "ZIP must contain doctor PDF entries");

                    foreach (var entry in archive.Entries)
                    {
                        Assert.IsTrue(entry.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase), $"Entry must be .pdf: {entry.FullName}");
                        Assert.IsTrue(entry.FullName.StartsWith("BaoCao_BacSi_", StringComparison.OrdinalIgnoreCase), $"Entry must be doctor report: {entry.FullName}");
                        
                        // Kiểm tra file PDF không rỗng (magic header %PDF-)
                        using (var es = entry.Open())
                        {
                            byte[] header = new byte[5];
                            int read = es.Read(header, 0, 5);
                            Assert.AreEqual(5, read, "Must read 5 bytes header");
                            string magic = System.Text.Encoding.ASCII.GetString(header);
                            Assert.AreEqual("%PDF-", magic, "Entry must be a valid PDF file");
                        }
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportBacSiCD45PdfZipAsync_AllProvinces_HierarchicalPdfs()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                // Xuất PDF Bác sĩ toàn bộ các tỉnh tháng 08/2026
                var task = service.ExportBacSiCD45ExcelAsync(2026, 8, null, "UnitTest", "Tester", format: "PDF");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export Doctor PDF all provinces should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "ZIP file should exist on disk");
                Assert.IsTrue(result.FileName.Contains("_PDF.zip"), "File name should contain _PDF.zip");
                generatedFilePath = result.FilePath;

                using (var zipStream = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    Assert.IsTrue(archive.Entries.Count > 0, "ZIP must contain doctor PDF entries");

                    bool hasFolderStructure = false;
                    foreach (var entry in archive.Entries)
                    {
                        Assert.IsTrue(entry.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase), $"Entry must be .pdf: {entry.FullName}");
                        if (entry.FullName.Contains("/"))
                        {
                            hasFolderStructure = true;
                        }
                    }

                    Assert.IsTrue(hasFolderStructure, "Doctor PDFs in multi-province zip must be categorized into province folders");
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }

        [TestMethod]
        public void ReportExportService_ExportHoatDongCD45Pdf_ShouldCreateValidPdf()
        {
            var service = new ReportExportService();
            string generatedFilePath = null;
            try
            {
                var task = service.ExportHoatDongCD45ExcelAsync(2026, 8, "HNO", "tt", "UnitTest", "Tester", format: "PDF");
                var result = task.GetAwaiter().GetResult();

                Assert.IsNotNull(result, "Result must not be null");
                Assert.IsTrue(result.Success, "Export Hoat Dong CD45 PDF should succeed: " + result.Message);
                Assert.IsTrue(File.Exists(result.FilePath), "PDF file should exist on disk: " + result.FilePath);
                Assert.IsTrue(result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase), "File name must end with .pdf");
                Assert.IsTrue(result.FileSizeBytes > 0, "PDF size must be > 0");
                generatedFilePath = result.FilePath;

                // Kiểm tra magic header %PDF-
                byte[] header = new byte[5];
                using (var fs = new FileStream(result.FilePath, FileMode.Open, FileAccess.Read))
                {
                    int read = fs.Read(header, 0, 5);
                    Assert.AreEqual(5, read, "Must read 5 bytes header");
                }
                string magic = System.Text.Encoding.ASCII.GetString(header);
                Assert.AreEqual("%PDF-", magic, "File must be valid PDF");
            }
            finally
            {
                if (!string.IsNullOrEmpty(generatedFilePath) && File.Exists(generatedFilePath))
                {
                    try { File.Delete(generatedFilePath); } catch { }
                }
            }
        }
    }
}

