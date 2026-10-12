using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Common;
using Model.ModelExtend.API.CD45;

namespace BVTL.Tests
{
    [TestClass]
    public class DataValidationP1Tests
    {
        [TestMethod]
        public void ValidateClientF1_WhenClientNotFound_ShouldReturnFalseAndQuarantine()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateClientF1("DHN010002", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_F1_MISSING", logs[0].RULE_CODE);
            Assert.AreEqual("ERROR", logs[0].SEVERITY);
        }

        [TestMethod]
        public void ValidateClientF1_WhenClientIncomplete_ShouldReturnFalseAndQuarantine()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "0", 1, new DateTime(2026, 2, 1)); // Status = 0 (Incomplete)

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateClientF1("DHN010001", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_F1_NOT_COMPLETE", logs[0].RULE_CODE);
            Assert.AreEqual("ERROR", logs[0].SEVERITY);
        }

        [TestMethod]
        public void ValidateClientF1_WhenInvalidTargetGroup_ShouldReturnFalseAndQuarantine()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", null, new DateTime(2026, 2, 1)); // Missing DoiTuong

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateClientF1("DHN010001", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_F1_INVALID_TARGET_GROUP", logs[0].RULE_CODE);
        }

        [TestMethod]
        public void ValidateClientF1_WhenValidAndComplete_ShouldReturnTrue()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateClientF1("DHN010001", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsTrue(valid);
            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void ValidateServiceDateAgainstF1_WhenDateBeforeF1_ShouldReturnTrueAndWarn()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 10));

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            // Dịch vụ ngày 2026-02-05 xảy ra trước ngày tham gia 2026-02-10
            bool valid = DataCleanerHelper.ValidateServiceDateAgainstF1("DHN010001", new DateTime(2026, 2, 5), "f2_date", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsTrue(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_SERVICE_DATE_BEFORE_F1", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void ValidateServiceDateAgainstF1_WhenDateOnOrAfterF1_ShouldReturnTrue()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 10));

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateServiceDateAgainstF1("DHN010001", new DateTime(2026, 2, 15), "f2_date", context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.IsTrue(valid);
            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void ValidateF8Dependency_WhenF7NotCompleted_ShouldReturnFalseAndQuarantine()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));
            // KH chưa có F7 completed

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateF8Dependency("DHN010001", context, "API_F8", "REP1", "CD45", ref logs);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_F8_WITHOUT_F7_COMPLETE", logs[0].RULE_CODE);
            Assert.AreEqual("ERROR", logs[0].SEVERITY);
        }

        [TestMethod]
        public void ValidateF8Dependency_WhenF7Completed_ShouldReturnTrue()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));
            context.RegisterF7Complete("DHN010001");

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateF8Dependency("DHN010001", context, "API_F8", "REP1", "CD45", ref logs);

            Assert.IsTrue(valid);
            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void ValidateF5Dependency_WhenF6NotVisited_ShouldReturnFalseAndQuarantine()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));
            // KH chưa từng có F6

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateF5Dependency("DHN010001", context, "API_F5", "REP1", "CD45", ref logs);

            Assert.IsFalse(valid);
            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("ERR_F5_WITHOUT_F6", logs[0].RULE_CODE);
            Assert.AreEqual("ERROR", logs[0].SEVERITY);
        }

        [TestMethod]
        public void ValidateF5Dependency_WhenF6Visited_ShouldReturnTrue()
        {
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));
            context.RegisterF6Visit("DHN010001");

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            bool valid = DataCleanerHelper.ValidateF5Dependency("DHN010001", context, "API_F5", "REP1", "CD45", ref logs);

            Assert.IsTrue(valid);
            Assert.AreEqual(0, logs.Count);
        }

        [TestMethod]
        public void ValidateServiceAfterLostToFollowUp_WhenAfterLostDate_ShouldLogWarning()
        {
            var context = new CD45ValidationContext();
            context.RegisterF9Lost("DHN010001", new DateTime(2026, 3, 1));

            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            // Dịch vụ ngày 2026-03-10 diễn ra sau ngày mất dấu 2026-03-01
            DataCleanerHelper.ValidateServiceAfterLostToFollowUp("DHN010001", new DateTime(2026, 3, 10), context, "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_SERVICE_AFTER_LOST_TO_FOLLOWUP", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void CheckFormCompletionStatus_WhenIncompleteOrUnverified_ShouldLogWarning()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.CheckFormCompletionStatus("DHN010001", "0", "F1", "API_F1", "CD45_KH", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_FORM_INCOMPLETE", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
        }

        [TestMethod]
        public void LogEmptyInstance_ShouldLogWarningAndSkip()
        {
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            DataCleanerHelper.LogEmptyInstance("DHN010001", 2, "F2", "API_F2", "CD45_HOAT_DONG", "REP1", "CD45", ref logs);

            Assert.AreEqual(1, logs.Count);
            Assert.AreEqual("WARN_EMPTY_INSTANCE", logs[0].RULE_CODE);
            Assert.AreEqual("WARNING", logs[0].SEVERITY);
            Assert.AreEqual("SKIPPED", logs[0].ACTION_TAKEN);
        }

        [TestMethod]
        public void ConvertF3_WhenScoreMismatch_ShouldAutoStandardizeAndEnforceLevel1OnSelfHarm()
        {
            var converter = new ConvertCD45ApiToEntity();
            string json = @"[
              {
                ""record_id"": ""DHN010001"",
                ""redcap_data_access_group"": ""v_nh"",
                ""f3_date"": ""2026-02-15"",
                ""f3_q1a"": 1,
                ""f3_q1b"": 1,
                ""f3_q1c"": 1,
                ""f3_q1d"": 1,
                ""f3_q1e"": 0,
                ""f3_q1f"": 0,
                ""f3_q2"": 2,
                ""f3_q3"": 0,
                ""f3_score"": 3,
                ""f3_level"": 3
              }
            ]";
            var apiData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json);

            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010001", "2", 1, new DateTime(2026, 2, 1));

            var entities = new List<CD45_QST_Entity>();
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            converter.ConvertF3(apiData, "CD45", "API_F3", "REP1", ref entities, ref logs, context);

            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(6, entities[0].DIEM_QST); // Đã chuẩn hóa về 6
            Assert.AreEqual((byte)1, entities[0].MUC_QST); // Bắt buộc Mức 1 vì có tự hại
            Assert.IsTrue(logs.Exists(l => l.RULE_CODE == "R_QST_SCORE_STANDARDIZED"));
            Assert.IsTrue(logs.Exists(l => l.RULE_CODE == "R_QST_MUC_STANDARDIZED"));
        }

        [TestMethod]
        public void ExtractNhomTCV_HCM_Fields_f2_person_tg_g3_ac_ShouldMapCorrectly()
        {
            // Test 1: The Gate (f2_person_tg)
            string jsonTG = @"[{ ""record_id"": ""DHC060001"", ""redcap_data_access_group"": ""the_gate"", ""f2_person_tg"": ""2"" }]";
            var modelTG = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonTG)[0];
            var resTG = CD45Helper.ExtractNhomTCV(modelTG, "f2");
            Assert.AreEqual("tg", resTG.maNhom);
            Assert.AreEqual("2", resTG.maTcv);

            // Test 2: G3VN (f2_person_g3) có và không có DAG
            string jsonG3WithDag = @"[{ ""record_id"": ""DHC080001"", ""redcap_data_access_group"": ""g3vn"", ""f2_person_g3"": ""4"" }]";
            var modelG3WithDag = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonG3WithDag)[0];
            var resG3WithDag = CD45Helper.ExtractNhomTCV(modelG3WithDag, "f2");
            Assert.AreEqual("g3vn", resG3WithDag.maNhom);
            Assert.AreEqual("4", resG3WithDag.maTcv);

            string jsonG3NoDag = @"[{ ""record_id"": ""DHC080002"", ""f2_person_g3"": ""5"" }]";
            var modelG3NoDag = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonG3NoDag)[0];
            var resG3NoDag = CD45Helper.ExtractNhomTCV(modelG3NoDag, "f2");
            Assert.AreEqual("g3vn", resG3NoDag.maNhom); // g3 tự động map về g3vn
            Assert.AreEqual("5", resG3NoDag.maTcv);

            // Test 3: Alocare (f2_person_ac) có và không có DAG
            string jsonAcWithDag = @"[{ ""record_id"": ""DHC070001"", ""redcap_data_access_group"": ""alocare"", ""f2_person_ac"": ""1"" }]";
            var modelAcWithDag = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonAcWithDag)[0];
            var resAcWithDag = CD45Helper.ExtractNhomTCV(modelAcWithDag, "f2");
            Assert.AreEqual("alo", resAcWithDag.maNhom);
            Assert.AreEqual("1", resAcWithDag.maTcv);

            string jsonAcNoDag = @"[{ ""record_id"": ""DHC070002"", ""f2_person_ac"": ""8"" }]";
            var modelAcNoDag = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonAcNoDag)[0];
            var resAcNoDag = CD45Helper.ExtractNhomTCV(modelAcNoDag, "f2");
            Assert.AreEqual("alo", resAcNoDag.maNhom); // ac tự động map về alo
            Assert.AreEqual("8", resAcNoDag.maTcv);
        }
    }
}
