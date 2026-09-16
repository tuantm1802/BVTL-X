using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using WebApp.Controllers;
using WebApp.Service;

namespace BVTL.Tests
{
    [TestClass]
    public class ExcelReportServiceTests
    {
        private IExcelReportService _excelService;

        [TestInitialize]
        public void Setup()
        {
            _excelService = new ExcelReportService();
        }

        [TestMethod]
        public void ExportReport_WithValidData_ShouldReturnNonEmptyByteArray()
        {
            // Arrange
            var listData = new List<BaoCaoModel>
            {
                new BaoCaoModel
                {
                    STT = "1",
                    ThongTinBC = "Số tiếp cận truyền thông",
                    Tong = 150,
                    MSM = 100,
                    PUD = 30,
                    SW = 20,
                    IsShow = "Y",
                    BoldText = "Y"
                },
                new BaoCaoModel
                {
                    STT = "2",
                    ThongTinBC = "Số khách hàng nhận vật phẩm",
                    Tong = 80,
                    MSM = 50,
                    PUD = 20,
                    SW = 10,
                    IsShow = "Y",
                    BoldText = "N"
                }
            };

            var user = new UserLogin
            {
                Name = "Người dùng thử nghiệm",
                UserName = "testuser"
            };

            string outFileName = string.Empty;

            // Act
            byte[] fileBytes = _excelService.ExportReport(
                listData,
                "BÁO CÁO THỰC NGHIỆM UNIT TEST",
                "Sheet1",
                user,
                "Tất cả nhóm",
                "BaoCaoTest",
                out outFileName
            );

            // Assert
            Assert.IsNotNull(fileBytes);
            Assert.IsTrue(fileBytes.Length > 0);
            Assert.IsFalse(string.IsNullOrEmpty(outFileName));
            // Verify PKZip magic header (0x50 0x4B) for openxml .xlsx format
            Assert.AreEqual((byte)0x50, fileBytes[0]);
            Assert.AreEqual((byte)0x4B, fileBytes[1]);
        }

        [TestMethod]
        public void BaoCaoCD45_ExportExcel_ShouldSupportBothGetAndPost()
        {
            var method = typeof(BaoCaoCD45Controller).GetMethod("ExportExcel");
            Assert.IsNotNull(method, "ExportExcel method must exist on BaoCaoCD45Controller");

            var attr = method.GetCustomAttributes(typeof(AcceptVerbsAttribute), false)
                             .Cast<AcceptVerbsAttribute>()
                             .FirstOrDefault();
            Assert.IsNotNull(attr, "ExportExcel must have AcceptVerbs attribute");
            CollectionAssert.Contains(attr.Verbs.ToList(), "GET");
            CollectionAssert.Contains(attr.Verbs.ToList(), "POST");

            // Verify actual export execution
            var controller = new BaoCaoCD45Controller(null, null, new BaoCaoCD45DA(), null);
            var result = controller.ExportExcel("26/07/2026", "25/08/2026", null, null, null) as FileContentResult;
            Assert.IsNotNull(result, "Export result should be FileContentResult");
            Assert.IsNotNull(result.FileContents, "FileContents should not be null");
            Assert.IsTrue(result.FileContents.Length > 0, "FileContents should not be empty");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }

        [TestMethod]
        public void BaoCaoTCVCD45_ExportExcel_ShouldSupportBothGetAndPost()
        {
            var tcvType = typeof(BaoCaoTCVCD45Controller);

            // ExportExcel (alias)
            var methodExport = tcvType.GetMethod("ExportExcel");
            Assert.IsNotNull(methodExport, "ExportExcel alias must exist on BaoCaoTCVCD45Controller");
            var attrExport = methodExport.GetCustomAttributes(typeof(AcceptVerbsAttribute), false)
                                         .Cast<AcceptVerbsAttribute>()
                                         .FirstOrDefault();
            Assert.IsNotNull(attrExport, "ExportExcel must have AcceptVerbs attribute");
            CollectionAssert.Contains(attrExport.Verbs.ToList(), "GET");
            CollectionAssert.Contains(attrExport.Verbs.ToList(), "POST");

            // ExportSingleExcel
            var methodSingle = tcvType.GetMethod("ExportSingleExcel");
            Assert.IsNotNull(methodSingle, "ExportSingleExcel method must exist");
            var attrSingle = methodSingle.GetCustomAttributes(typeof(AcceptVerbsAttribute), false)
                                         .Cast<AcceptVerbsAttribute>()
                                         .FirstOrDefault();
            Assert.IsNotNull(attrSingle, "ExportSingleExcel must have AcceptVerbs attribute");
            CollectionAssert.Contains(attrSingle.Verbs.ToList(), "GET");
            CollectionAssert.Contains(attrSingle.Verbs.ToList(), "POST");

            // ExportExcelZip
            var methodZip = tcvType.GetMethod("ExportExcelZip");
            Assert.IsNotNull(methodZip, "ExportExcelZip method must exist");
            var attrZip = methodZip.GetCustomAttributes(typeof(AcceptVerbsAttribute), false)
                                   .Cast<AcceptVerbsAttribute>()
                                   .FirstOrDefault();
            Assert.IsNotNull(attrZip, "ExportExcelZip must have AcceptVerbs attribute");
            CollectionAssert.Contains(attrZip.Verbs.ToList(), "GET");
            CollectionAssert.Contains(attrZip.Verbs.ToList(), "POST");

            // Verify actual export execution
            var controller = new BaoCaoTCVCD45Controller(null, null, new BaoCaoCD45DA());
            var result = controller.ExportExcel("26/07/2026", "25/08/2026", "CD45_HN_01", "TCV01", "Bùi Văn Bằng", "Nhóm Test") as FileContentResult;
            Assert.IsNotNull(result, "Export result should be FileContentResult");
            Assert.IsNotNull(result.FileContents, "FileContents should not be null");
            Assert.IsTrue(result.FileContents.Length > 0, "FileContents should not be empty");
        }

        [TestMethod]
        public void BaoCaoCD45_SearchBaoCao_WhenToDateBeforeFromDate_ShouldReturnValidationFailure()
        {
            var controller = new BaoCaoCD45Controller(null, null, new BaoCaoCD45DA(), null);
            // From: 25/08/2026, To: 26/07/2026 -> Đến ngày nhỏ hơn Từ ngày
            var res = controller.SearchBaoCao("25/08/2026", "26/07/2026", null, null, null);
            Assert.IsNotNull(res, "JsonResult should not be null");
            Assert.IsNotNull(res.Data, "JsonResult.Data should not be null");

            var success = (bool)res.Data.GetType().GetProperty("Success").GetValue(res.Data, null);
            var message = res.Data.GetType().GetProperty("Message").GetValue(res.Data, null)?.ToString();

            Assert.IsFalse(success, "Success must be false when ToDate < FromDate");
            Assert.IsNotNull(message);
            Assert.IsTrue(message.Contains("không được nhỏ hơn"), "Message must explain ToDate cannot be smaller than FromDate");
        }

        [TestMethod]
        public void BaoCaoTCVCD45_SearchBaoCao_WhenToDateBeforeFromDate_ShouldReturnValidationFailure()
        {
            var controller = new BaoCaoTCVCD45Controller(null, null, new BaoCaoCD45DA());
            // From: 25/08/2026, To: 26/07/2026 -> Đến ngày nhỏ hơn Từ ngày
            var res = controller.SearchBaoCao("25/08/2026", "26/07/2026", null, null);
            Assert.IsNotNull(res, "JsonResult should not be null");
            Assert.IsNotNull(res.Data, "JsonResult.Data should not be null");

            var success = (bool)res.Data.GetType().GetProperty("Success").GetValue(res.Data, null);
            var message = res.Data.GetType().GetProperty("Message").GetValue(res.Data, null)?.ToString();

            Assert.IsFalse(success, "Success must be false when ToDate < FromDate");
            Assert.IsNotNull(message);
            Assert.IsTrue(message.Contains("không được nhỏ hơn"), "Message must explain ToDate cannot be smaller than FromDate");
        }

        [TestMethod]
        public void BaoCaoCD45_ExportExcel_WhenToDateBeforeFromDate_ShouldReturnContentResultWithError()
        {
            var controller = new BaoCaoCD45Controller(null, null, new BaoCaoCD45DA(), null);
            var result = controller.ExportExcel("25/08/2026", "26/07/2026", null, null, null);
            Assert.IsInstanceOfType(result, typeof(ContentResult), "Should return ContentResult with alert instead of exporting file");
            var content = (ContentResult)result;
            Assert.IsTrue(content.Content.Contains("không được nhỏ hơn"));
        }

        [TestMethod]
        public void BaoCaoTCVCD45_ExportExcel_WhenToDateBeforeFromDate_ShouldReturnContentResultWithError()
        {
            var controller = new BaoCaoTCVCD45Controller(null, null, new BaoCaoCD45DA());
            var result = controller.ExportExcel("25/08/2026", "26/07/2026", null, null, null, null);
            Assert.IsInstanceOfType(result, typeof(ContentResult), "Should return ContentResult with alert instead of exporting file");
            var content = (ContentResult)result;
            Assert.IsTrue(content.Content.Contains("không được nhỏ hơn"));
        }

        [TestMethod]
        public void BaoCaoCD45DA_GetDrillDown_ShouldEnrichNamesForTinhNhomTCV()
        {
            var da = new BaoCaoCD45DA();
            var list = da.GetDrillDown("II_2", null, null, null, null, null, null);

            Assert.IsNotNull(list);
            if (list.Count > 0)
            {
                var first = list[0];
                Assert.IsFalse(string.IsNullOrEmpty(first.RECORD_ID));
                Assert.IsFalse(string.IsNullOrEmpty(first.TEN_TINH), "TEN_TINH should not be empty");
                Assert.IsFalse(string.IsNullOrEmpty(first.TEN_NHOM), "TEN_NHOM should not be empty");
                Assert.IsFalse(string.IsNullOrEmpty(first.TEN_TCV), "TEN_TCV should not be empty");

                var dnaCustomer = list.FirstOrDefault(x => x.RECORD_ID == "DNA200157");
                if (dnaCustomer != null)
                {
                    Assert.AreEqual("Nghệ An", dnaCustomer.TEN_TINH);
                    Assert.AreEqual("Liên Minh MSM", dnaCustomer.TEN_NHOM);
                    Assert.AreEqual("Trần Trung Đức", dnaCustomer.TEN_TCV);
                }
            }
        }
    }
}
