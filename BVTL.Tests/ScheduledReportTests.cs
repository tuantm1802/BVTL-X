using ClosedXML.Excel;
using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Report;
using System;
using System.IO;
using System.IO.Compression;
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
            Assert.AreEqual(5, settings.RunDay, "Default day of month should be 5");
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
                        Assert.AreEqual(0, tcvFileCount, "HCM should have 0 TCV detail files since it has no individual TCV data in database");
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

                    // Dòng 5 là Section I (tiêu đề bold) -> các ô cột 3..8 phải để trống
                    string headerTong = ws.Cell(5, 3).GetString();
                    Assert.AreEqual(string.Empty, headerTong, "Header Section cell should be empty");

                    // Kiểm tra các dòng dữ liệu: nếu giá trị bằng 0 phải hiển thị ký tự '-'
                    bool foundDash = false;
                    for (int r = 6; r <= 30; r++)
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
            // Test with customer DHY230142 from user request
            var detail = da.GetCustomerDetail("DHY230142");
            Assert.IsNotNull(detail, "Detail should not be null for DHY230142");
            Assert.IsNotNull(detail.ListTuVan, "ListTuVan should not be null");
            Assert.IsTrue(detail.ListTuVan.Count >= 3, "Customer should have at least 3 tu van sessions");

            // Verify sequential numbering: 1, 2, 3...
            for (int i = 0; i < detail.ListTuVan.Count; i++)
            {
                Assert.AreEqual(i + 1, detail.ListTuVan[i].SoThuTu, $"Session at index {i} must have SoThuTu = {i + 1}");
            }

            // Verify F7 is first
            Assert.AreEqual(1, detail.ListTuVan[0].LanTuVan, "First session should be F7 (LanTuVan = 1)");
            // Verify F8 are next
            Assert.AreEqual(2, detail.ListTuVan[1].LanTuVan, "Second session should be F8 (LanTuVan = 2)");
            Assert.AreEqual(2, detail.ListTuVan[2].LanTuVan, "Third session should be F8 (LanTuVan = 2)");
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

                    string subtitle = ws.Cell(2, 1).GetString();
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

                    string subtitle = ws.Cell(2, 1).GetString();
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
    }
}

