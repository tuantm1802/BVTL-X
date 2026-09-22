using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Model.ModelExtend;
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
            var controller = new BaoCaoTCVCD45Controller(null, new BVTL_NHOM_TBHDA(), new BaoCaoCD45DA());
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

        [TestMethod]
        public void BaoCaoCD45DA_FormatKhamSKTTChiTiet_ShouldEnrichHospitalAndDiagnosisNames()
        {
            // Case 1: Lần khám: 2 - Cơ sở: 7 - 132
            var item1 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 2 - Cơ sở: 7 - 132" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item1);
            Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: Khác", item1.CHI_TIET);

            // Case 2: Lần khám: 2 - Cơ sở: 7 - Chẩn đoán: 132
            var item2 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 2 - Cơ sở: 7 - Chẩn đoán: 132" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item2);
            Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: Khác", item2.CHI_TIET);

            // Case 3: Lần khám: 2 - Cơ sở: 7 - 126
            var item3 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 2 - Cơ sở: 7 - 126" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item3);
            Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: F51- Rối loạn giấc ngủ không thực tổn", item3.CHI_TIET);

            // Case 4: Lần khám: 1 - Cơ sở: 1 - 131
            var item4 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 1 - Cơ sở: 1 - 131" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item4);
            Assert.AreEqual("Lần khám: 1 - Cơ sở: Hà Nội - Bệnh viện Lão khoa - Chẩn đoán: F41.2- Rối loạn hỗn hợp lo âu và trầm cảm", item4.CHI_TIET);

            // Case 5: Lần khám: 2 - Cơ sở: 1 - 114
            var item5 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 2 - Cơ sở: 1 - 114" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item5);
            Assert.AreEqual("Lần khám: 2 - Cơ sở: Hà Nội - Bệnh viện Lão khoa - Chẩn đoán: F33- Rối loạn trầm cảm tái diễn", item5.CHI_TIET);

            // Case 6: Lần khám: 1 - Cơ sở: 6 - 132
            var item6 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 1 - Cơ sở: 6 - 132" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item6);
            Assert.AreEqual("Lần khám: 1 - Cơ sở: Bệnh viện tâm thần Nghệ An - Chẩn đoán: Khác", item6.CHI_TIET);

            // Case 7: Lần khám: 1 - Cơ sở: 1 - (Không có chẩn đoán)
            var item7 = new CD45_DrillDown_ItemModel { CHI_TIET = "Lần khám: 1 - Cơ sở: 1 - " };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item7);
            Assert.AreEqual("Lần khám: 1 - Cơ sở: Hà Nội - Bệnh viện Lão khoa", item7.CHI_TIET);

            // Case 8: Idempotency (gọi lại không bị biến dạng)
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item1);
            Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: Khác", item1.CHI_TIET);

            // Case 9: Non-kham items remain unchanged
            var item9 = new CD45_DrillDown_ItemModel { CHI_TIET = "Sàng lọc QST" };
            BaoCaoCD45DA.FormatKhamSKTTChiTiet(item9);
            Assert.AreEqual("Sàng lọc QST", item9.CHI_TIET);
        }

        [TestMethod]
        public void BaoCaoCD45DA_GetDrillDown_KhamSKTT_ShouldEnrichHospitalAndDiagnosis()
        {
            var da = new BaoCaoCD45DA();
            var list = da.GetDrillDown("III_3", null, null, null, null, null, 2);

            Assert.IsNotNull(list);
            if (list.Count > 0)
            {
                var hpItem = list.FirstOrDefault(x => x.RECORD_ID == "DHP090029");
                if (hpItem != null)
                {
                    Assert.AreEqual("Hải Phòng", hpItem.TEN_TINH);
                    Assert.AreEqual("Hải Đăng", hpItem.TEN_NHOM);
                    Assert.AreEqual("Nguyễn Hoàng Long", hpItem.TEN_TCV);
                    Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: Khác", hpItem.CHI_TIET);
                }

                var hpItem2 = list.FirstOrDefault(x => x.RECORD_ID == "DHP090053");
                if (hpItem2 != null)
                {
                    Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện SKTT Hải Phòng - Chẩn đoán: F51- Rối loạn giấc ngủ không thực tổn", hpItem2.CHI_TIET);
                }

                var naItem = list.FirstOrDefault(x => x.RECORD_ID == "DNA210040");
                if (naItem != null)
                {
                    Assert.AreEqual("Lần khám: 2 - Cơ sở: Bệnh viện tâm thần Nghệ An - Chẩn đoán: Khác", naItem.CHI_TIET);
                }
            }
        }

        public void BaoCaoTCVCD45_BuildTCVWorksheet_ShouldHaveEqualColumnsAndMergedSignaturesAndA4()
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BaoCao");
                var listData = new List<Model.ModelExtend.BaoCaoCD45Model>
                {
                    new Model.ModelExtend.BaoCaoCD45Model { STT = "1", ChiTieu = "Số tiếp cận truyền thông", Tong = 10, PUD = 2, PLHIV = 1, TG = 1, SW = 2, MSM = 4 }
                };

                var ctrl = (BaoCaoTCVCD45Controller)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(BaoCaoTCVCD45Controller));
                var method = typeof(BaoCaoTCVCD45Controller).GetMethod("BuildTCVWorksheet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(method, "BuildTCVWorksheet method must exist");

                method.Invoke(ctrl, new object[] { ws, listData, "26/07/2026", "25/08/2026", "Nhóm Test", "Hoàng Quang Vinh", null, "Nhóm" });

                // 1. Column widths
                Assert.AreEqual(5.5, ws.Column(1).Width, 0.01, "Col 1 width must be 5.5");
                Assert.AreEqual(44.0, ws.Column(2).Width, 0.01, "Col 2 width must be 44.0");
                for (int c = 3; c <= 8; c++)
                {
                    Assert.AreEqual(10.0, ws.Column(c).Width, 0.01, $"Col {c} width must be exactly 10.0");
                }

                // 2. Merged signatures
                var mergedRanges = ws.MergedRanges.Select(r => r.RangeAddress.ToString()).ToList();
                // A:B for Tiếp cận viên, C:E for Cán bộ dự án, F:H for MnE
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("A") && r.Contains(":B")), "Must have A:B merged signature cell");
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("C") && r.Contains(":E")), "Must have C:E merged signature cell");
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("F") && r.Contains(":H")), "Must have F:H merged signature cell");

                // 3. A4 Portrait Setup
                Assert.AreEqual(ClosedXML.Excel.XLPaperSize.A4Paper, ws.PageSetup.PaperSize, "PaperSize must be A4");
                Assert.AreEqual(ClosedXML.Excel.XLPageOrientation.Portrait, ws.PageSetup.PageOrientation, "PageOrientation must be Portrait");
                Assert.AreEqual(1, ws.PageSetup.PagesWide, "PagesWide must be 1 for fit-to-page");
            }
        }

        [TestMethod]
        public void ReportExportService_BuildTCVWorksheet_ShouldHaveEqualColumnsAndMergedSignaturesAndA4()
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BaoCao");
                var listData = new List<Model.ModelExtend.BaoCaoCD45Model>
                {
                    new Model.ModelExtend.BaoCaoCD45Model { STT = "1", ChiTieu = "Số tiếp cận truyền thông", Tong = 10, PUD = 2, PLHIV = 1, TG = 1, SW = 2, MSM = 4 }
                };

                var svc = (WebApp.Services.ReportExportService)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(WebApp.Services.ReportExportService));
                var method = typeof(WebApp.Services.ReportExportService).GetMethod("BuildTCVWorksheet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(method, "BuildTCVWorksheet method must exist on ReportExportService");

                method.Invoke(svc, new object[] { ws, listData, "26/07/2026", "25/08/2026", "Nhóm Test", "Hoàng Quang Vinh", "Nhóm" });

                // 1. Column widths
                Assert.AreEqual(5.5, ws.Column(1).Width, 0.01, "Col 1 width must be 5.5");
                Assert.AreEqual(44.0, ws.Column(2).Width, 0.01, "Col 2 width must be 44.0");
                for (int c = 3; c <= 8; c++)
                {
                    Assert.AreEqual(10.0, ws.Column(c).Width, 0.01, $"Col {c} width must be exactly 10.0");
                }

                // 2. Merged signatures
                var mergedRanges = ws.MergedRanges.Select(r => r.RangeAddress.ToString()).ToList();
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("A") && r.Contains(":B")), "Must have A:B merged signature cell");
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("C") && r.Contains(":E")), "Must have C:E merged signature cell");
                Assert.IsTrue(mergedRanges.Any(r => r.StartsWith("F") && r.Contains(":H")), "Must have F:H merged signature cell");

                // 3. A4 Portrait Setup
                Assert.AreEqual(ClosedXML.Excel.XLPaperSize.A4Paper, ws.PageSetup.PaperSize, "PaperSize must be A4");
                Assert.AreEqual(ClosedXML.Excel.XLPageOrientation.Portrait, ws.PageSetup.PageOrientation, "PageOrientation must be Portrait");
                Assert.AreEqual(1, ws.PageSetup.PagesWide, "PagesWide must be 1 for fit-to-page");
            }
        }

        [TestMethod]
        public void NhomTBH_PrefixModelHelpers_ShouldProvideProperDefaultsAndFormatting()
        {
            // 1. Default (null / empty prefix)
            var defaultNhom = new Model.Model.BVTL_NHOM_TBH
            {
                manhom_tbh = "bm",
                tennhom_tbh = "Bình Minh"
            };
            Assert.AreEqual("Nhóm", defaultNhom.GetXungDanh());
            Assert.AreEqual("Nhóm", defaultNhom.GetShortXungDanh());
            Assert.AreEqual("Nhóm: Bình Minh", defaultNhom.GetFullDisplayName());
            Assert.AreEqual("Nhóm Bình Minh", defaultNhom.GetTitleName());
            Assert.AreEqual("Nhóm Bình Minh", defaultNhom.GetShortTitleName());

            // 2. Custom Prefix (e.g. HCM Social Enterprise)
            var customNhom = new Model.Model.BVTL_NHOM_TBH
            {
                manhom_tbh = "alo",
                tennhom_tbh = "Alocare",
                PREFIX = "Doanh nghiệp xã hội",
                SHORT_PREFIX = "DNXH"
            };
            Assert.AreEqual("Doanh nghiệp xã hội", customNhom.GetXungDanh());
            Assert.AreEqual("DNXH", customNhom.GetShortXungDanh());
            Assert.AreEqual("Doanh nghiệp xã hội: Alocare", customNhom.GetFullDisplayName());
            Assert.AreEqual("Doanh nghiệp xã hội Alocare", customNhom.GetTitleName());
            Assert.AreEqual("DNXH Alocare", customNhom.GetShortTitleName());

            // 3. ViewModel helper
            var vm = new Model.ModelExtend.CD45_NhomTcvViewModel
            {
                TEN_NHOM = "The Times",
                PREFIX = "Doanh nghiệp xã hội",
                SHORT_PREFIX = "DNXH"
            };
            Assert.AreEqual("Doanh nghiệp xã hội", vm.GetXungDanh());
            Assert.AreEqual("DNXH", vm.GetShortXungDanh());
        }

        [TestMethod]
        public void BVTL_NHOM_TBHDA_GetAll_ShouldRetrievePrefixFromDatabase()
        {
            var da = new BVTL_NHOM_TBHDA();
            var list = da.GetAll();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);

            var hcmItem = list.FirstOrDefault(x => x.city_code == "HCM" || x.manhom_tbh == "HC_ALO" || x.manhom_tbh_map == "alo");
            Assert.IsNotNull(hcmItem, "HCM group should exist in GetAll()");
            Assert.AreEqual("Doanh nghiệp xã hội", hcmItem.PREFIX, $"PREFIX was '{hcmItem.PREFIX}', expected 'Doanh nghiệp xã hội'");
            Assert.AreEqual("Doanh nghiệp xã hội", hcmItem.GetXungDanh());
        }

        [TestMethod]
        public void CD45NhomTcvDA_GetListNhomTcv_ShouldIncludeHcmGroupsWith0Tcv()
        {
            var da = new CD45NhomTcvDA();
            var list = da.GetListNhomTcv("HCM", null, null);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 4, $"Expected at least 4 HCM groups with 0 TCVs, actual count: {list.Count}");

            var alocare = list.FirstOrDefault(x => x.MA_NHOM == "HC_ALO" || x.MA_NHOM == "alo");
            Assert.IsNotNull(alocare, "Alocare group should exist in HCM list");
            Assert.IsNull(alocare.MA_TCV, "MA_TCV should be null for groups without TCV");
            Assert.IsNull(alocare.TEN_TCV, "TEN_TCV should be null for groups without TCV");
            Assert.AreEqual("Doanh nghiệp xã hội", alocare.PREFIX);
            Assert.AreEqual("DNXH", alocare.SHORT_PREFIX);
            Assert.AreEqual("TP Hồ Chí Minh", alocare.CityName);
        }

        [TestMethod]
        public void CD45NhomTcvDA_GetKpiStats_ShouldCountAllGroupsAndProvinces()
        {
            var da = new CD45NhomTcvDA();
            var kpi = da.GetKpiStats();
            Assert.IsNotNull(kpi);
            Assert.IsTrue(kpi.TongNhom >= 22, $"TongNhom must be >= 22 (actual: {kpi.TongNhom})");
            Assert.IsTrue(kpi.TongTinh >= 6, $"TongTinh must be >= 6 (actual: {kpi.TongTinh})");
            Assert.IsTrue(kpi.TongTCV > 0, $"TongTCV must be > 0 (actual: {kpi.TongTCV})");
        }

        [TestMethod]
        public void BaoCaoTCVCD45Controller_GetFilterData_ShouldReturnPrefixAndDisplayName()
        {
            var controller = new BaoCaoTCVCD45Controller(new CityDA(), new BVTL_NHOM_TBHDA(), new BaoCaoCD45DA());
            var jsonResult = controller.GetFilterData() as JsonResult;
            Assert.IsNotNull(jsonResult);

            var jobj = Newtonsoft.Json.Linq.JObject.FromObject(jsonResult.Data);
            Assert.IsTrue((bool)jobj["Success"]);
            var nhoms = jobj["Nhoms"] as Newtonsoft.Json.Linq.JArray;
            Assert.IsNotNull(nhoms);
            Assert.IsTrue(nhoms.Count > 0);

            var firstNhom = nhoms[0];
            Assert.IsNotNull(firstNhom["Prefix"]);
            Assert.IsNotNull(firstNhom["ShortPrefix"]);
            Assert.IsNotNull(firstNhom["DisplayName"]);
        }

        [TestMethod]
        public void ReportExportService_BuildTCVWorksheet_WithCustomPrefix_ShouldRenderInCellA3()
        {
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("BaoCao");
                var listData = new List<Model.ModelExtend.BaoCaoCD45Model>
                {
                    new Model.ModelExtend.BaoCaoCD45Model { STT = "1", ChiTieu = "Số tiếp cận truyền thông", Tong = 10 }
                };

                var svc = (WebApp.Services.ReportExportService)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(WebApp.Services.ReportExportService));
                var method = typeof(WebApp.Services.ReportExportService).GetMethod("BuildTCVWorksheet", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(method, "BuildTCVWorksheet method must exist on ReportExportService");

                // Invoke with custom prefix "Doanh nghiệp xã hội"
                method.Invoke(svc, new object[] { ws, listData, "26/07/2026", "25/08/2026", "Alocare", "Nguyễn Văn A", "Doanh nghiệp xã hội" });

                string cellA3 = ws.Cell("A3").GetString();
                Assert.IsTrue(cellA3.Contains("Doanh nghiệp xã hội: Alocare"), $"Cell A3 must contain custom prefix 'Doanh nghiệp xã hội: Alocare'. Actual: '{cellA3}'");
                Assert.IsTrue(cellA3.Contains("Tiếp cận viên: Nguyễn Văn A"), $"Cell A3 must contain 'Tiếp cận viên: Nguyễn Văn A'. Actual: '{cellA3}'");
            }
        }
    }
}
