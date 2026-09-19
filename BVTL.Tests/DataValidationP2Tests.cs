using System;
using System.Collections.Generic;
using System.Linq;
using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Common;
using Model.ModelExtend.API.CD45;

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
            var logs = da.GetLogsByUnit("CD45", "NA", "UNKNOWN", "PENDING");

            Assert.IsNotNull(logs);
            Assert.AreEqual(26, logs.Count, "Pending count for NA / UNKNOWN should be exactly 26");
            Assert.IsTrue(logs.All(x => x.IS_RESOLVED == false), "All pending logs must have IS_RESOLVED = false");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "WARNING" || x.SEVERITY == "ERROR"), "All pending logs must be WARNING or ERROR");
        }

        [TestMethod]
        public void GetLogsByUnit_WithErrorsMetric_ShouldReturnOnlyErrors()
        {
            var da = new DataQualityDA();
            var logs = da.GetLogsByUnit("CD45", "NA", "UNKNOWN", "ERROR");

            Assert.IsNotNull(logs);
            Assert.AreEqual(27, logs.Count, "Errors count for NA / UNKNOWN should be exactly 27");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "ERROR"), "All error logs must have SEVERITY = ERROR");
        }

        [TestMethod]
        public void GetLogsByUnit_WithWarningsMetric_ShouldReturnOnlyWarnings()
        {
            var da = new DataQualityDA();
            var logs = da.GetLogsByUnit("CD45", "NA", "UNKNOWN", "WARNING");

            Assert.IsNotNull(logs);
            Assert.AreEqual(16, logs.Count, "Warnings count for NA / UNKNOWN should be exactly 16");
            Assert.IsTrue(logs.All(x => x.SEVERITY == "WARNING"), "All warning logs must have SEVERITY = WARNING");
        }

        [TestMethod]
        public void GetLogsByUnit_WithResolvedMetric_ShouldReturnOnlyResolvedItems()
        {
            var da = new DataQualityDA();
            var logs = da.GetLogsByUnit("CD45", "NA", "UNKNOWN", "RESOLVED");

            Assert.IsNotNull(logs);
            Assert.AreEqual(17, logs.Count, "Resolved count for NA / UNKNOWN should be exactly 17");
            Assert.IsTrue(logs.All(x => x.IS_RESOLVED == true), "All resolved logs must have IS_RESOLVED = true");
        }

        #endregion
    }
}
