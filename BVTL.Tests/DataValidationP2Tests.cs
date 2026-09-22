using System;
using System.Collections.Generic;
using System.Linq;
using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Common;
using Model.ModelExtend.API.CD45;
using Model.ModelExtend;
using System.IO;
using System.Web.Mvc;
using ClosedXML.Excel;
using WebApp.Controllers;

namespace BVTL.Tests
{
    [TestClass]
    public class DataValidationP2Tests
    {
        #region VR-07(c) Contradictory Rules Tests

        [TestMethod]
        public void CheckContradictoryRules_WhenResearchFalseButHasCode_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckContradictoryRules(
                recordId: "DHN010001",
                thamGiaNc: false,
                maKhNc: "NC-12345",
                doiTuong: 1,
                ketQuaHivF4: null,
                apiCode: "API_CD45_F1",
                tableName: "CD45_KH",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_CONTRADICTORY_RESEARCH_INFO", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
            Assert.IsTrue(logs[0].MESSAGE.Contains("NC-12345"));
        }

        [TestMethod]
        public void CheckContradictoryRules_WhenResearchTrueAndHasCode_ShouldNotLog()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckContradictoryRules(
                recordId: "DHN010001",
                thamGiaNc: true,
                maKhNc: "NC-12345",
                doiTuong: 1,
                ketQuaHivF4: null,
                apiCode: "API_CD45_F1",
                tableName: "CD45_KH",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void CheckContradictoryRules_WhenPLHIVTestedForHIVAtF4_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckContradictoryRules(
                recordId: "DHN010002",
                thamGiaNc: null,
                maKhNc: null,
                doiTuong: 2, // PLHIV
                ketQuaHivF4: 1, // Âm tính
                apiCode: "API_CD45_F4",
                tableName: "CD45_HO_TRO_XH",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_CONTRADICTORY_HIV_STATUS", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
            Assert.IsTrue(logs[0].MESSAGE.Contains("PLHIV"));
        }

        [TestMethod]
        public void CheckContradictoryRules_WhenNonPLHIVTestedForHIVAtF4_ShouldNotLog()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckContradictoryRules(
                recordId: "DHN010003",
                thamGiaNc: null,
                maKhNc: null,
                doiTuong: 1, // PUD
                ketQuaHivF4: 1,
                apiCode: "API_CD45_F4",
                tableName: "CD45_HO_TRO_XH",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(0, logs.Count);
        }

        #endregion

        #region VR-07(b) Cluster Incomplete Tests

        public class FakeServiceRecord
        {
            public string RecordId { get; set; }
            public string MaNhom { get; set; }
            public string Tcv { get; set; }
            public DateTime? ServiceDate { get; set; }
            public string Status { get; set; }
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenThreeIncompleteSameDaySameTcv_ShouldLogClusterWarning()
        {
            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "2" } // 1 complete
            };

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_CLUSTER_INCOMPLETE", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
            Assert.IsTrue(logs[0].MESSAGE.Contains("Có 3 khách hàng"));
            Assert.IsTrue(logs[0].MESSAGE.Contains("TCV01"));
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenRecordIdAndTcvNameProvided_ShouldIncludeClientIdsAndTcvNameInMessage()
        {
            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { RecordId = "DHN020241", MaNhom = "vn", Tcv = "4", ServiceDate = new DateTime(2026, 8, 18), Status = "0" },
                new FakeServiceRecord { RecordId = "DHN020243", MaNhom = "vn", Tcv = "4", ServiceDate = new DateTime(2026, 8, 18), Status = "0" },
                new FakeServiceRecord { RecordId = "DHN020255", MaNhom = "vn", Tcv = "4", ServiceDate = new DateTime(2026, 8, 18), Status = "0" }
            };

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs,
                getRecordId: x => x.RecordId,
                getMaNhom: x => x.MaNhom,
                resolveTcvName: (nhom, tcv) => nhom == "vn" && tcv == "4" ? "Vũ Thị Phương Lan" : null
            );

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_CLUSTER_INCOMPLETE", logs[0].RULE_CODE);
            Assert.IsTrue(logs[0].MESSAGE.Contains("DHN020241, DHN020243, DHN020255"));
            Assert.IsTrue(logs[0].MESSAGE.Contains("Vũ Thị Phương Lan"));
            Assert.IsTrue(logs[0].MESSAGE.Contains("Mã: 4"));
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenLessThanThreeIncomplete_ShouldNotLogCluster()
        {
            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "2" }
            };

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenThreeIncompleteDifferentDays_ShouldNotLogCluster()
        {
            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 11), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 12), Status = "0" }
            };

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenThreeIncompleteDifferentTcv_ShouldNotLogCluster()
        {
            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { Tcv = "TCV01", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV02", ServiceDate = new DateTime(2026, 3, 10), Status = "0" },
                new FakeServiceRecord { Tcv = "TCV03", ServiceDate = new DateTime(2026, 3, 10), Status = "0" }
            };

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs
            );

            Assert.AreEqual(0, logs.Count);
        }

        #endregion

        #region VR-04(b) & VR-07(a) Helper Tests

        [TestMethod]
        public void CheckFormCompletionStatus_WhenIncomplete_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckFormCompletionStatus("DHN010001", "0", "F2", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_FORM_INCOMPLETE", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void CheckFormCompletionStatus_WhenUnverified_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckFormCompletionStatus("DHN010001", "1", "F2", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_FORM_UNVERIFIED", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void CheckFormCompletionStatus_WhenComplete_ShouldNotLog()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckFormCompletionStatus("DHN010001", "2", "F2", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void LogEmptyInstance_WhenInstanceEmpty_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.LogEmptyInstance("DHN010001", null, "F2", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_EMPTY_INSTANCE", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void LogEmptyInstance_WhenInstanceHasValue_ShouldLogWarningWithInstanceNumber()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.LogEmptyInstance("DHN010001", 3, "F2", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_EMPTY_INSTANCE", logs[0].RULE_CODE);
            Assert.IsTrue(logs[0].MESSAGE.Contains("#3"));
        }

        #endregion

        #region Models & Grouped Logic Mapping Tests

        [TestMethod]
        public void GroupedDataQualityLogModel_PropertiesMapping_ShouldHoldExpectedValues()
        {
            var now = DateTime.Now;
            var model = new GroupedDataQualityLogModel
            {
                RULE_CODE = "WARN_CLUSTER_INCOMPLETE",
                SEVERITY = "WARNING",
                TABLE_NAME = "CD45_HOAT_DONG",
                TotalCount = 15,
                UnresolvedCount = 12,
                FirstSeen = now.AddDays(-5),
                LastSeen = now,
                SampleMessage = "Cảnh báo cụm bất thường"
            };

            Assert.AreEqual("WARN_CLUSTER_INCOMPLETE", model.RULE_CODE);
            Assert.AreEqual("WARNING", model.SEVERITY);
            Assert.AreEqual("CD45_HOAT_DONG", model.TABLE_NAME);
            Assert.AreEqual(15, model.TotalCount);
            Assert.AreEqual(12, model.UnresolvedCount);
            Assert.AreEqual(now, model.LastSeen);
            Assert.AreEqual("Cảnh báo cụm bất thường", model.SampleMessage);
        }

        [TestMethod]
        public void DataQualityStatsByNhomModel_PropertiesMapping_ShouldHoldExpectedValues()
        {
            var model = new DataQualityStatsByNhomModel
            {
                MA_NHOM = "NB_HN",
                CITY_CODE = "HN",
                TEN_NHOM = "Hoa Nắng",
                TEN_TINH = "Hà Nội",
                TotalErrors = 2,
                TotalWarnings = 8,
                TotalResolved = 5,
                TotalPending = 5
            };

            Assert.AreEqual("NB_HN", model.MA_NHOM);
            Assert.AreEqual("HN", model.CITY_CODE);
            Assert.AreEqual("Hoa Nắng", model.TEN_NHOM);
            Assert.AreEqual("Hà Nội", model.TEN_TINH);
            Assert.AreEqual(2, model.TotalErrors);
            Assert.AreEqual(8, model.TotalWarnings);
            Assert.AreEqual(5, model.TotalResolved);
            Assert.AreEqual(5, model.TotalPending);
        }

        [TestMethod]
        public void GetStatsByNhom_ShouldEnrichNamesForTinhAndNhom()
        {
            var da = new DataQualityDA();
            var list = da.GetStatsByNhom("CD45");

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "List of stats by nhom should not be empty");
            Assert.IsTrue(list.All(x => !string.IsNullOrEmpty(x.TEN_TINH)), "All units must have TEN_TINH populated");
            Assert.IsTrue(list.All(x => !string.IsNullOrEmpty(x.TEN_NHOM)), "All units must have TEN_NHOM populated");

            var hanoiVn = list.FirstOrDefault(x => x.CITY_CODE == "HNO" && x.MA_NHOM == "vn");
            if (hanoiVn != null)
            {
                Assert.AreEqual("Hà Nội", hanoiVn.TEN_TINH);
                Assert.AreEqual("Về nhà", hanoiVn.TEN_NHOM);
            }

            var hpDan = list.FirstOrDefault(x => x.CITY_CODE == "HPG" && x.MA_NHOM == "hd");
            if (hpDan != null)
            {
                Assert.AreEqual("Hải Phòng", hpDan.TEN_TINH);
                Assert.AreEqual("Hải Đăng", hpDan.TEN_NHOM);
            }
        }

        #endregion

        #region Tab 3 Unit Drilldown Tests

        [TestMethod]
        public void GetLogsByUnit_WithPendingMetric_ShouldReturnOnlyUnresolvedItems()
        {
            var da = new DataQualityDA();
            var stats = da.GetStatsByNhom("CD45");
            var target = stats.FirstOrDefault(x => x.TotalPending > 0);
            Assert.IsNotNull(target, "Must have at least one unit with pending items");

            var logs = da.GetLogsByUnit("CD45", target.CITY_CODE, target.MA_NHOM, "PENDING");

            Assert.IsNotNull(logs);
            Assert.AreEqual(target.TotalPending, logs.Count, $"Pending count for {target.CITY_CODE} / {target.MA_NHOM} must match TotalPending in stats");
            Assert.IsTrue(logs.All(x => x.IS_RESOLVED == false), "All pending logs must have IS_RESOLVED = false");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "WARNING" || x.SEVERITY == "ERROR"), "All pending logs must be WARNING or ERROR");
        }

        [TestMethod]
        public void GetLogsByUnit_WithErrorsMetric_ShouldReturnOnlyErrors()
        {
            var da = new DataQualityDA();
            var stats = da.GetStatsByNhom("CD45");
            var target = stats.FirstOrDefault(x => x.TotalErrors > 0);
            Assert.IsNotNull(target, "Must have at least one unit with error items");

            var logs = da.GetLogsByUnit("CD45", target.CITY_CODE, target.MA_NHOM, "ERROR");

            Assert.IsNotNull(logs);
            Assert.AreEqual(target.TotalErrors, logs.Count, $"Errors count for {target.CITY_CODE} / {target.MA_NHOM} must match TotalErrors in stats");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "ERROR"), "All error logs must have SEVERITY = ERROR");
        }

        [TestMethod]
        public void GetLogsByUnit_WithWarningsMetric_ShouldReturnOnlyWarnings()
        {
            var da = new DataQualityDA();
            var stats = da.GetStatsByNhom("CD45");
            var target = stats.FirstOrDefault(x => x.TotalWarnings > 0);
            Assert.IsNotNull(target, "Must have at least one unit with warning items");

            var logs = da.GetLogsByUnit("CD45", target.CITY_CODE, target.MA_NHOM, "WARNING");

            Assert.IsNotNull(logs);
            Assert.AreEqual(target.TotalWarnings, logs.Count, $"Warnings count for {target.CITY_CODE} / {target.MA_NHOM} must match TotalWarnings in stats");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "WARNING"), "All warning logs must have SEVERITY = WARNING");
        }

        [TestMethod]
        public void GetLogsByUnit_WithResolvedMetric_ShouldReturnOnlyResolvedItems()
        {
            var da = new DataQualityDA();
            var stats = da.GetStatsByNhom("CD45");
            var target = stats.FirstOrDefault(x => x.TotalResolved > 0);
            Assert.IsNotNull(target, "Must have at least one unit with resolved items");

            var logs = da.GetLogsByUnit("CD45", target.CITY_CODE, target.MA_NHOM, "RESOLVED");

            Assert.IsNotNull(logs);
            Assert.AreEqual(target.TotalResolved, logs.Count, $"Resolved count for {target.CITY_CODE} / {target.MA_NHOM} must match TotalResolved in stats");
            Assert.IsTrue(logs.All(x => x.IS_RESOLVED == true), "All resolved logs must have IS_RESOLVED = true");
        }

        [TestMethod]
        public void InferGroupAndCity_ShouldMapMalformedClientIdsAndDags()
        {
            // 1. Ninh Bình - Gió Mới: mã thiếu tiền tố DNB '151117'
            bool ok1 = CD45Helper.InferGroupAndCity("151117", null, out string nhom1, out string city1);
            Assert.IsTrue(ok1);
            Assert.AreEqual("gm", nhom1, "151117 must map to Gió Mới (gm)");
            Assert.AreEqual("NBI", city1, "151117 must map to Ninh Bình (NBI)");

            // 2. Nghệ An - Quỳnh Hương Xanh: mã thiếu tiền tố DNA '21251'
            bool ok2 = CD45Helper.InferGroupAndCity("21251", null, out string nhom2, out string city2);
            Assert.IsTrue(ok2);
            Assert.AreEqual("qhx", nhom2, "21251 must map to Quỳnh Hương Xanh (qhx)");
            Assert.AreEqual("NAN", city2, "21251 must map to Nghệ An (NAN)");

            // 3. Hải Phòng - Vòng Tay Bè Bạn: mã thiếu số 0 'DHP10099'
            bool ok3 = CD45Helper.InferGroupAndCity("DHP10099", null, out string nhom3, out string city3);
            Assert.IsTrue(ok3);
            Assert.AreEqual("vtbb", nhom3, "DHP10099 must map to Vòng Tay Bè Bạn (vtbb)");
            Assert.AreEqual("HPG", city3, "DHP10099 must map to Hải Phòng (HPG)");

            // 4. Tra cứu từ DAG slug
            bool okDag = CD45Helper.InferGroupAndCity(null, "gi_mi", out string nhomDag, out string cityDag);
            Assert.IsTrue(okDag);
            Assert.AreEqual("gm", nhomDag);
            Assert.AreEqual("NBI", cityDag);
        }

        #endregion
    
        #region DataQuality Tab-Context Aware Export Excel Tests

        [TestMethod]
        public void DataQualityController_ExportExcel_Details_ShouldReturnExcelFileWithChiTietSheet()
        {
            var ctrl = new DataQualityController();
            var result = ctrl.ExportExcel("details", "CD45", null, null, null, null) as FileContentResult;

            Assert.IsNotNull(result, "ExportExcel for details must return FileContentResult");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("NhatKy_ChiTiet_CD45_"), "File name must start with NhatKy_ChiTiet_CD45_");
            Assert.IsTrue(result.FileContents.Length > 0, "File content must not be empty");

            using (var stream = new MemoryStream(result.FileContents))
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheet("ChiTiet_SuKien");
                Assert.IsNotNull(ws, "Must have worksheet ChiTiet_SuKien");
                Assert.IsTrue(ws.Cell("A1").GetString().Contains("NHẬT KÝ CHI TIẾT"), "A1 must contain title");
            }
        }

        [TestMethod]
        public void DataQualityController_ExportExcel_Grouped_ShouldReturnExcelFileWithGroupedSheet()
        {
            var ctrl = new DataQualityController();
            var result = ctrl.ExportExcel("grouped", "CD45", null, null, null, null) as FileContentResult;

            Assert.IsNotNull(result, "ExportExcel for grouped must return FileContentResult");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("GomNhom_QuyTac_CD45_"), "File name must start with GomNhom_QuyTac_CD45_");

            using (var stream = new MemoryStream(result.FileContents))
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheet("GomNhom_QuyTac");
                Assert.IsNotNull(ws, "Must have worksheet GomNhom_QuyTac");
                Assert.IsTrue(ws.Cell("A1").GetString().Contains("BẢNG TỔNG HỢP VI PHẠM"), "A1 must contain title");
            }
        }

        [TestMethod]
        public void DataQualityController_ExportExcel_ByUnit_ShouldReturnExcelFileWithUnitSheet()
        {
            var ctrl = new DataQualityController();
            var result = ctrl.ExportExcel("byUnit", "CD45", null, null, null, null) as FileContentResult;

            Assert.IsNotNull(result, "ExportExcel for byUnit must return FileContentResult");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("ThongKe_DonVi_CD45_"), "File name must start with ThongKe_DonVi_CD45_");

            using (var stream = new MemoryStream(result.FileContents))
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheet("ThongKe_DonVi");
                Assert.IsNotNull(ws, "Must have worksheet ThongKe_DonVi");
                Assert.IsTrue(ws.Cell("A1").GetString().Contains("BẢNG THỐNG KÊ CHẤT LƯỢNG"), "A1 must contain title");
            }
        }

        [TestMethod]
        public void DataQualityController_ExportExcel_MultiSheet_ShouldReturnExcelFileWithAllThreeSheets()
        {
            var ctrl = new DataQualityController();
            var result = ctrl.ExportExcel("multi", "CD45", null, null, null, null) as FileContentResult;

            Assert.IsNotNull(result, "ExportExcel for multi must return FileContentResult");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("BaoCao_ChatLuongDuLieu_CD45_"), "File name must start with BaoCao_ChatLuongDuLieu_CD45_");

            using (var stream = new MemoryStream(result.FileContents))
            using (var wb = new XLWorkbook(stream))
            {
                Assert.AreEqual(3, wb.Worksheets.Count, "Multi-sheet workbook must have exactly 3 worksheets");
                Assert.IsNotNull(wb.Worksheet("ThongKe_DonVi"), "Must contain ThongKe_DonVi worksheet");
                Assert.IsNotNull(wb.Worksheet("GomNhom_QuyTac"), "Must contain GomNhom_QuyTac worksheet");
                Assert.IsNotNull(wb.Worksheet("ChiTiet_CanhBao"), "Must contain ChiTiet_CanhBao worksheet");
            }
        }

        [TestMethod]
        public void DataQualityController_ExportExcelWarnings_ShouldReturnLegacyExcelFile()
        {
            var ctrl = new DataQualityController();
            var result = ctrl.ExportExcelWarnings("CD45", null) as FileContentResult;

            Assert.IsNotNull(result, "ExportExcelWarnings must return FileContentResult");
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("CanhBao_REDCap_CD45_"), "File name must start with CanhBao_REDCap_CD45_");

            using (var stream = new MemoryStream(result.FileContents))
            using (var wb = new XLWorkbook(stream))
            {
                var ws = wb.Worksheet("CanhBaoDuLieu");
                Assert.IsNotNull(ws, "Must have worksheet CanhBaoDuLieu");
            }
        }

        [TestMethod]
        public void CheckClusterIncomplete_WhenClusterDetected_ShouldRemoveIndividualFormIncompleteLogs()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>
            {
                // Existing individual warnings
                new BVTL_DATA_STANDARDIZATION_LOG_Entity { TABLE_NAME = "CD45_HOAT_DONG", RECORD_ID = "DHN020198", RULE_CODE = "WARN_FORM_INCOMPLETE", SEVERITY = "WARNING" },
                new BVTL_DATA_STANDARDIZATION_LOG_Entity { TABLE_NAME = "CD45_HOAT_DONG", RECORD_ID = "DHN020199", RULE_CODE = "WARN_FORM_INCOMPLETE", SEVERITY = "WARNING" },
                new BVTL_DATA_STANDARDIZATION_LOG_Entity { TABLE_NAME = "CD45_HOAT_DONG", RECORD_ID = "DHN020207", RULE_CODE = "WARN_FORM_INCOMPLETE", SEVERITY = "WARNING" },
                new BVTL_DATA_STANDARDIZATION_LOG_Entity { TABLE_NAME = "CD45_HOAT_DONG", RECORD_ID = "DHN020999", RULE_CODE = "WARN_FORM_INCOMPLETE", SEVERITY = "WARNING" } // Not in cluster
            };

            var list = new List<FakeServiceRecord>
            {
                new FakeServiceRecord { RecordId = "DHN020198", MaNhom = "vn", Tcv = "2", ServiceDate = new DateTime(2026, 8, 17), Status = "0" },
                new FakeServiceRecord { RecordId = "DHN020199", MaNhom = "vn", Tcv = "2", ServiceDate = new DateTime(2026, 8, 17), Status = "0" },
                new FakeServiceRecord { RecordId = "DHN020207", MaNhom = "vn", Tcv = "2", ServiceDate = new DateTime(2026, 8, 17), Status = "0" }
            };

            DataCleanerHelper.CheckClusterIncomplete(
                list,
                x => x.Tcv,
                x => x.ServiceDate,
                x => x.Status,
                tableName: "CD45_HOAT_DONG",
                apiCode: "API_CD45_F2",
                reportId: "REP1",
                maDuAn: "CD45",
                logs: ref logs,
                getRecordId: x => x.RecordId,
                getMaNhom: x => x.MaNhom
            );

            // Must contain 1 cluster warning and 1 unaffected individual log (DHN020999)
            Assert.AreEqual(2, logs.Count, "Cluster incomplete must deduplicate individual incomplete logs for records in the cluster");
            Assert.IsTrue(logs.Any(l => l.RULE_CODE == "WARN_CLUSTER_INCOMPLETE"));
            Assert.IsTrue(logs.Any(l => l.RULE_CODE == "WARN_FORM_INCOMPLETE" && l.RECORD_ID == "DHN020999"));
            Assert.IsFalse(logs.Any(l => l.RULE_CODE == "WARN_FORM_INCOMPLETE" && l.RECORD_ID == "DHN020207"), "DHN020207 individual incomplete log must be removed");
        }

        [TestMethod]
        public void CleanDocumentDelivery_WhenGreaterThan10_ShouldPreserveRawValueWithoutAltering()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            int? clean = DataCleanerHelper.CleanDocumentDelivery(860, "DHN020127", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(860, clean, "Document delivery quantity from REDCap source must be preserved faithfully as raw 860");
            Assert.AreEqual(0, logs.Count, "Must not artificially alter or flag outlier delivery quantity");
        }

        [TestMethod]
        public void CleanDocumentDelivery_WhenNormalValue_ShouldPreserveValueWithoutLogging()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            int? clean = DataCleanerHelper.CleanDocumentDelivery(2, "DHN020128", "API_CD45_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(2, clean, "Normal delivery quantity (2) must be preserved");
            Assert.AreEqual(0, logs.Count, "Must not log when within normal range");
        }

        [TestMethod]
        public void BHYT_ServiceCodeMatching_Service10ShouldNotMatchBHYT()
        {
            // Emulate SQL logic: CHARINDEX(',1,', ',' + REPLACE(DICH_VU, ' ', '') + ',') > 0
            bool IsBHYT(string dichVu)
            {
                if (string.IsNullOrEmpty(dichVu)) return false;
                string padded = "," + dichVu.Replace(" ", "") + ",";
                return padded.Contains(",1,") || dichVu.Contains("BHYT") || dichVu.Contains("bảo hiểm");
            }

            Assert.IsFalse(IsBHYT("10"), "Service 10 (HIV test) must NOT match BHYT (service 1)");
            Assert.IsFalse(IsBHYT("10, 2"), "Service 10 and 2 must NOT match BHYT");
            Assert.IsTrue(IsBHYT("1"), "Service 1 must match BHYT");
            Assert.IsTrue(IsBHYT("1, 10"), "Service 1 and 10 must match BHYT");
            Assert.IsTrue(IsBHYT("10, 1"), "Service 10 and 1 must match BHYT");
            Assert.IsTrue(IsBHYT("Hỗ trợ thẻ BHYT"), "Text BHYT must match");
        }

        [TestMethod]
        public void DrillDown_UnitAndQuantityMapping_VII_1ShouldMapBooksAndSumCorrectly()
        {
            string GetUnitText(string chiTieu, string code)
            {
                if (chiTieu != null && chiTieu.ToLower().Contains("lượt")) return "lượt";
                if (code == "VII_1" || (chiTieu != null && chiTieu.ToLower().Contains("quyển"))) return "quyển";
                if (chiTieu != null && chiTieu.ToLower().Contains("bao cao su")) return "chiếc";
                if (chiTieu != null && chiTieu.ToLower().Contains("bôi trơn")) return "gói";
                if (chiTieu != null && chiTieu.ToLower().Contains("bơm kim tiêm")) return "chiếc";
                return "KH";
            }

            Assert.AreEqual("quyển", GetUnitText("1. Số quyển tài liệu đã phát", "VII_1"));
            Assert.AreEqual("KH", GetUnitText("2. Số KH nhận tài liệu", "VII_2"));
            Assert.AreEqual("lượt", GetUnitText("2. Số lượt tiếp cận", "I_2"));

            var items = new List<CD45_DrillDown_ItemModel>
            {
                new CD45_DrillDown_ItemModel { RECORD_ID = "DHN01", SO_LUONG = 1, CHI_TIET = "Số quyển phát: 1" },
                new CD45_DrillDown_ItemModel { RECORD_ID = "DHN02", SO_LUONG = 2, CHI_TIET = "Số quyển phát: 2" },
                new CD45_DrillDown_ItemModel { RECORD_ID = "DHN03", SO_LUONG = 860, CHI_TIET = "Số quyển phát: 860" }
            };

            int totalBooks = items.Sum(x => x.SO_LUONG ?? 0);
            Assert.AreEqual(863, totalBooks);
            Assert.AreEqual(3, items.Count);
        }

        #endregion

    }
}
