using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Report;
using System;
using System.IO;
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
