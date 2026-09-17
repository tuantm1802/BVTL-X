using System;
using System.Collections.Generic;
using System.Linq;
using Common.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend;
using Model.ModelExtend.API.CD45;

namespace BVTL.Tests
{
    [TestClass]
    public class DataValidationP0Tests
    {
        #region VR-01: Report Arithmetic Validation Tests

        [TestMethod]
        public void ValidateReportArithmetic_WhenAllRowsMatch_ShouldReturnTrue()
        {
            var rows = new List<BaoCaoCD45Model>
            {
                new BaoCaoCD45Model
                {
                    STT = "I",
                    ChiTieu = "THÔNG TIN CHUNG",
                    IsBold = true,
                    Code = "SEC_I",
                    Tong = 0, PUD = 0, PLHIV = 0, TG = 0, SW = 0, MSM = 0
                },
                new BaoCaoCD45Model
                {
                    STT = "1",
                    ChiTieu = "Tổng số KH được chăm sóc",
                    IsBold = false,
                    Tong = 100,
                    PUD = 20,
                    PLHIV = 30,
                    TG = 10,
                    SW = 15,
                    MSM = 25
                },
                new BaoCaoCD45Model
                {
                    STT = "2",
                    ChiTieu = "Tổng số KH mới",
                    IsBold = false,
                    Tong = 50,
                    PUD = 10,
                    PLHIV = 10,
                    TG = 5,
                    SW = 5,
                    MSM = 20
                }
            };

            bool isValid = ReportValidatorHelper.ValidateReportArithmetic(rows, out string error);

            Assert.IsTrue(isValid);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void ValidateReportArithmetic_WhenRowHasMismatch_ShouldReturnFalseAndProvideDetailedMessage()
        {
            var rows = new List<BaoCaoCD45Model>
            {
                new BaoCaoCD45Model
                {
                    STT = "1",
                    ChiTieu = "Tổng số KH được chăm sóc",
                    IsBold = false,
                    Tong = 100, // Cố ý để 100 trong khi tổng 5 nhóm là 20+30+10+15+15 = 90
                    PUD = 20,
                    PLHIV = 30,
                    TG = 10,
                    SW = 15,
                    MSM = 15
                }
            };

            bool isValid = ReportValidatorHelper.ValidateReportArithmetic(rows, out string error);

            Assert.IsFalse(isValid);
            Assert.IsNotNull(error);
            StringAssert.Contains(error, "VR-01 [BLOCKING]");
            StringAssert.Contains(error, "100");
            StringAssert.Contains(error, "90");
        }

        [TestMethod]
        public void ValidateReportArithmetic_WhenDataIsNull_ShouldReturnTrue()
        {
            bool isValid = ReportValidatorHelper.ValidateReportArithmetic(null, out string error);
            Assert.IsTrue(isValid);
            Assert.IsNull(error);
        }

        #endregion

        #region VR-02: CleanRecordId & DAG Match Tests

        [TestMethod]
        public void CleanRecordId_WithValidCodeAndMatchingDag_ShouldSucceed()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string clean = DataCleanerHelper.CleanRecordId("  dhn010001  ", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs, "the_times");

            Assert.AreEqual("DHN010001", clean);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "R1_RECORD_ID_AUTO_UPPER"));
            Assert.IsFalse(logs.Any(x => x.SEVERITY == "ERROR"));
        }

        [TestMethod]
        public void CleanRecordId_WithDNTCodeAndQuynhHuongXanh_ShouldSucceed()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string clean = DataCleanerHelper.CleanRecordId("DNT210242", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs, "qunh_hng_xanh");

            Assert.AreEqual("DNT210242", clean);
            Assert.IsFalse(logs.Any(x => x.SEVERITY == "ERROR"));
            Assert.IsFalse(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_FORMAT"));
            Assert.IsFalse(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_DAG_MISMATCH"));
        }

        [TestMethod]
        public void CleanRecordId_WithDNA21271_EightChars_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string clean = DataCleanerHelper.CleanRecordId("DNA21271", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs, "qunh_hng_xanh");

            Assert.IsNull(clean);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_FORMAT" && x.SEVERITY == "ERROR"));
        }

        [TestMethod]
        public void CleanRecordId_WithInvalidFormat_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            // Quá ngắn
            string r1 = DataCleanerHelper.CleanRecordId("W", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);
            Assert.IsNull(r1);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_FORMAT" && x.SEVERITY == "ERROR" && x.ACTION_TAKEN == "QUARANTINED"));

            // Thuần số thiếu D và mã tỉnh
            logs.Clear();
            string r2 = DataCleanerHelper.CleanRecordId("10124", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);
            Assert.IsNull(r2);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_FORMAT" && x.SEVERITY == "ERROR"));

            // Thừa khoảng trắng ở giữa không đúng định dạng 9 ký tự
            logs.Clear();
            string r3 = DataCleanerHelper.CleanRecordId("D NA 12345", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);
            Assert.IsNull(r3);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_FORMAT" && x.SEVERITY == "ERROR"));
        }

        [TestMethod]
        public void CleanRecordId_WithDagMismatch_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            // Record ID là Hà Nội (DHN...) nhưng DAG là Hải Phòng (bnh_minh)
            string clean = DataCleanerHelper.CleanRecordId("DHN010001", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs, "bnh_minh");

            Assert.IsNull(clean);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_DAG_MISMATCH" && x.SEVERITY == "ERROR" && x.ACTION_TAKEN == "QUARANTINED"));
        }

        [TestMethod]
        public void CleanRecordId_WithEmptyCode_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string clean = DataCleanerHelper.CleanRecordId("   ", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);

            Assert.IsNull(clean);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_RECORD_ID_EMPTY" && x.SEVERITY == "ERROR"));
        }

        #endregion

        #region VR-03: CleanDate & Appointment Logic Tests

        [TestMethod]
        public void CleanDate_WithValidDate_ShouldParseCorrectly()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            var d = DataCleanerHelper.CleanDate("2026-05-15", "f1_date", "DHN010001", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);

            Assert.IsNotNull(d);
            Assert.AreEqual(new DateTime(2026, 5, 15), d.Value);
            Assert.AreEqual(0, logs.Count(x => x.SEVERITY == "ERROR"));
        }

        [TestMethod]
        public void CleanDate_WithDateBeforeProjectStart_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            // Trước ngày khởi động 2026-01-01
            var d1 = DataCleanerHelper.CleanDate("2025-12-31", "f1_date", "DHN010001", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);
            Assert.IsNull(d1);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_DATE_BEFORE_PROJECT" && x.SEVERITY == "ERROR" && x.ACTION_TAKEN == "QUARANTINED"));

            // Năm gõ nhầm 2005
            logs.Clear();
            var d2 = DataCleanerHelper.CleanDate("2005-06-15", "f1_date", "DHN010001", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);
            Assert.IsNull(d2);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_DATE_BEFORE_PROJECT" && x.SEVERITY == "ERROR"));
        }

        [TestMethod]
        public void CleanDate_WithFutureDate_ShouldBlockAndReturnNull()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string futureStr = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd");

            var d = DataCleanerHelper.CleanDate(futureStr, "f1_date", "DHN010001", "API_CD45_F1", "CD45_KH", "1", "CD45", ref logs);

            Assert.IsNull(d);
            Assert.IsTrue(logs.Any(x => x.RULE_CODE == "ERR_DATE_FUTURE" && x.SEVERITY == "ERROR" && x.ACTION_TAKEN == "QUARANTINED"));
        }

        [TestMethod]
        public void CleanDate_WithFutureAppointmentDate_ShouldAllowWhenWhitelisted()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            string futureStr = DateTime.Today.AddDays(15).ToString("yyyy-MM-dd");

            // Ngày hẹn tái khám F6 được phép ở tương lai
            var d = DataCleanerHelper.CleanDate(futureStr, "f6_followup_visit", "DHN010001", "API_CD45_F6", "CD45_CHAN_DOAN", "1", "CD45", ref logs);

            Assert.IsNotNull(d);
            Assert.IsFalse(logs.Any(x => x.RULE_CODE == "ERR_DATE_FUTURE"));
        }

        #endregion

        #region DAG Batch Summary Collector Tests

        [TestMethod]
        public void DagSummaryCollector_ShouldAggregateCountsPerGroup()
        {
            var collector = new DagSummaryCollector();
            collector.Record("hi_vng", "NB_HV", "hv");
            collector.Record("hi_vng", "NB_HV", "hv");
            collector.Record("hi_vng", "NB_HV", "hv");
            collector.Record("the_times", "HN_TT", "tt");
            collector.Record("the_times", "HN_TT", "tt");

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            collector.FlushToLogs("CD45", "API_CD45_F1", "CD45_KH", "1", ref logs);

            // Phải sinh đúng 2 dòng log tóm tắt (thay vì 5 dòng riêng lẻ)
            Assert.AreEqual(2, logs.Count);

            var logHv = logs.FirstOrDefault(x => x.OLD_VALUE == "hi_vng");
            Assert.IsNotNull(logHv);
            Assert.AreEqual("BATCH (N=3)", logHv.RECORD_ID);
            StringAssert.Contains(logHv.MESSAGE, "3 bản ghi");

            var logTt = logs.FirstOrDefault(x => x.OLD_VALUE == "the_times");
            Assert.IsNotNull(logTt);
            Assert.AreEqual("BATCH (N=2)", logTt.RECORD_ID);
            StringAssert.Contains(logTt.MESSAGE, "2 bản ghi");
        }

        #endregion
    }
}
