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

        [TestMethod]
        public void NhomChucDanh_DefaultValues_And_SignerFormatting_ShouldBeAccurate()
        {
            // HCM default is Giam doc
            var nhomHcm = new Model.Model.BVTL_NHOM_TBH { city_code = "HCM", tennhom_tbh = "Bầu Trời Xanh" };
            Assert.AreEqual("Giám đốc", nhomHcm.GetChucDanh());
            Assert.AreEqual("Giám đốc \"Bầu Trời Xanh\"", nhomHcm.GetDaiDienNhomSigner());

            // The Time default is Giam doc
            var nhomTheTime = new Model.Model.BVTL_NHOM_TBH { manhom_tbh = "HN_TT", tennhom_tbh = "The Time" };
            Assert.AreEqual("Giám đốc", nhomTheTime.GetChucDanh());
            Assert.AreEqual("Giám đốc \"The Time\"", nhomTheTime.GetDaiDienNhomSigner());

            // Other groups default is Truong nhom
            var nhomHaiDang = new Model.Model.BVTL_NHOM_TBH { city_code = "HPG", tennhom_tbh = "Hải Đăng" };
            Assert.AreEqual("Trưởng nhóm", nhomHaiDang.GetChucDanh());
            Assert.AreEqual("Trưởng nhóm \"Hải Đăng\"", nhomHaiDang.GetDaiDienNhomSigner());

            // Custom ChucDanh override
            var nhomCustom = new Model.Model.BVTL_NHOM_TBH { CHUC_DANH = "Điều phối viên", tennhom_tbh = "Khát Vọng Sống" };
            Assert.AreEqual("Điều phối viên", nhomCustom.GetChucDanh());
            Assert.AreEqual("Điều phối viên \"Khát Vọng Sống\"", nhomCustom.GetDaiDienNhomSigner());

            // CD45_NhomTcvViewModel tests
            var vmHcm = new CD45_NhomTcvViewModel { CITY_CODE = "HCM", TEN_NHOM = "Alocare" };
            Assert.AreEqual("Giám đốc", vmHcm.GetChucDanh());
            Assert.AreEqual("Giám đốc \"Alocare\"", vmHcm.GetDaiDienNhomSigner());

            var vmOther = new CD45_NhomTcvViewModel { CITY_CODE = "DNA", TEN_NHOM = "Sông Hàn" };
            Assert.AreEqual("Trưởng nhóm", vmOther.GetChucDanh());
            Assert.AreEqual("Trưởng nhóm \"Sông Hàn\"", vmOther.GetDaiDienNhomSigner());
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

                    // 1. Signer 1: Dai dien nhom (Col A:B) -> Giám đốc "The Time"
                    string signer1 = ws.Cell(signRow, 1).GetString();
                    Assert.IsTrue(signer1.Contains("Giám đốc"), "Signer 1 should contain Giám đốc: " + signer1);
                    Assert.IsTrue(signer1.Contains("The Time") || signer1.Contains("Time"), "Signer 1 should contain group name: " + signer1);
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
                        // 1. Header: Tiêu đề báo cáo
                        string title = ws.Cell(1, 1).GetString();
                        Assert.IsTrue(title.Contains("DANH SÁCH KHÁCH HÀNG ĐƯỢC NHẬP LIỆU TRÊN REDCAP DỰ ÁN DREAMH"), 
                            "Title must match user specification: " + title);

                        // 2. Header: Tỉnh, Bác sĩ, Tháng (Merge Cột A & B cho Tiêu đề, Cột C & D cho Giá trị)
                        Assert.IsTrue(ws.Range(3, 1, 3, 2).IsMerged(), "Range A3:B3 should be merged for Province label");
                        string provinceLabel = ws.Cell(3, 1).GetString();
                        Assert.IsTrue(ws.Range(3, 3, 3, 4).IsMerged(), "Range C3:D3 should be merged for Province value");
                        string provinceVal = ws.Cell(3, 3).GetString();
                        Assert.IsTrue(provinceLabel.Contains("Tỉnh"), "Cell (3,1) should be Province label");
                        Assert.IsTrue(provinceVal.Contains("Hà Nội"), "Cell (3,3) should be Hà Nội");

                        string periodText = ws.Cell(3, 7).GetString();
                        Assert.IsTrue(periodText.Contains("Từ 01/09/2026 đến 30/09/2026"), 
                            "Doctor report period in Header must be from first day to last day of month: " + periodText);

                        Assert.IsTrue(ws.Range(4, 1, 4, 2).IsMerged(), "Range A4:B4 should be merged for Doctor label");
                        string doctorLabel = ws.Cell(4, 1).GetString();
                        Assert.IsTrue(ws.Range(4, 3, 4, 4).IsMerged(), "Range C4:D4 should be merged for Doctor value");
                        string doctorVal = ws.Cell(4, 3).GetString();
                        Assert.IsTrue(doctorLabel.Contains("Bác sĩ"), "Cell (4,1) should be Doctor label");
                        Assert.IsFalse(string.IsNullOrWhiteSpace(doctorVal), "Doctor name should not be empty");

                        // 3. Header bảng chi tiết ca khám (Dòng 6)
                        Assert.AreEqual("STT", ws.Cell(6, 1).GetString());
                        Assert.AreEqual("Mã Khách hàng", ws.Cell(6, 2).GetString());
                        Assert.AreEqual("Ngày khám", ws.Cell(6, 3).GetString());
                        Assert.AreEqual("Cơ sở khám / Bệnh viện", ws.Cell(6, 4).GetString());
                        Assert.AreEqual("Nhóm CBO", ws.Cell(6, 5).GetString());
                        Assert.AreEqual("Tiếp cận viên (TCV)", ws.Cell(6, 6).GetString());
                        Assert.AreEqual("Lần khám", ws.Cell(6, 7).GetString());
                        Assert.AreEqual("Chẩn đoán chính", ws.Cell(6, 8).GetString());
                        Assert.AreEqual("Hình thức điều trị", ws.Cell(6, 9).GetString());

                        // 4. Footer: Tìm chữ 'Xác nhận của SCDI'
                        int lastRow = ws.LastRowUsed().RowNumber();
                        int footerRow = -1;
                        for (int r = 7; r <= lastRow; r++)
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
            Assert.AreEqual(569, ptsdResult.recordsTotal, "PTSD phải khớp chính xác 569 KH");

            // STT 2: Rối loạn sử dụng chất (CHAT: QA2 = 1)
            var chatResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHAT", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(chatResult.recordsTotal > 0 && chatResult.recordsTotal <= totalAll, "CHAT phải có KH thỏa mãn");
            Assert.AreEqual(452, chatResult.recordsTotal, "CHAT phải khớp chính xác 452 KH");

            // STT 3: Chemsex (CHEMSEX: QA2 = 1 AND QA5 IN (2, 3, 4))
            var chemsexResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHEMSEX", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(chemsexResult.recordsTotal > 0 && chemsexResult.recordsTotal <= chatResult.recordsTotal, "CHEMSEX là tập con của CHAT");
            Assert.AreEqual(404, chemsexResult.recordsTotal, "CHEMSEX phải khớp chính xác 404 KH");

            // STT 4: Người bán dâm (SW: DOI_TUONG = 5 OR DOI_TUONG_KHAC có 5)
            var swResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "SW", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(swResult.recordsTotal > 0 && swResult.recordsTotal <= totalAll, "SW phải có KH thỏa mãn");
            Assert.AreEqual(577, swResult.recordsTotal, "SW phải khớp chính xác 577 KH");

            // STT 5: Người có HIV (PLHIV: DOI_TUONG = 2 OR DOI_TUONG_KHAC có 2)
            var plhivResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "PLHIV", PageIndex = 1, PageSize = 10 });
            Assert.IsTrue(plhivResult.recordsTotal > 0 && plhivResult.recordsTotal <= totalAll, "PLHIV phải có KH thỏa mãn");
            Assert.AreEqual(509, plhivResult.recordsTotal, "PLHIV phải khớp chính xác 509 KH");

            // STT 6 & 7: Người chuyển giới (TG_VAN_DE & TG_HORMONE: DOI_TUONG = 3 OR DOI_TUONG_KHAC có 3)
            var tgVanDeResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "TG_VAN_DE", PageIndex = 1, PageSize = 10 });
            var tgHormoneResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "TG_HORMONE", PageIndex = 1, PageSize = 10 });
            Assert.AreEqual(101, tgVanDeResult.recordsTotal, "TG_VAN_DE phải khớp chính xác 101 KH");
            Assert.AreEqual(101, tgHormoneResult.recordsTotal, "TG_HORMONE phải có cùng số lượng với TG_VAN_DE (101 KH)");

            // STT 8: Kỳ thị và tự kỳ thị (KY_THI: Tất cả KH dự án)
            var kyThiResult = khDA.GetPagingCustomers(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "KY_THI", PageIndex = 1, PageSize = 10 });
            Assert.AreEqual(totalAll, kyThiResult.recordsTotal, "KY_THI không lọc điều kiện phụ, phải bằng tổng số KH");

            // Kiểm tra hàm GetAllForExport cũng áp dụng đúng bộ lọc
            var exportChemsex = khDA.GetAllForExport(new CD45KhachHangFilterModel { ChuDeSinhHoatNhom = "CHEMSEX" });
            Assert.AreEqual(404, exportChemsex.Count, "Export danh sách CHEMSEX phải đúng 404 KH");
        }
    }
}

