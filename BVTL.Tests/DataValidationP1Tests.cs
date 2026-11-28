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

        [TestMethod]
        public void ConvertF6_WhenInstancesOutOfOrderButRetryOrderMatchesChronology_ShouldNotLogInversion()
        {
            var converter = new ConvertCD45ApiToEntity();
            // Mô phỏng dữ liệu thực tế của DHN040002: Instance 5 là Tái khám lần 5 (15/10), Instance 6 là Tái khám lần 4 (05/10)
            string json = @"[
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""1"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-07-01"", ""f6_examination"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""2"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-07-15"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""3"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-08-14"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""2"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""4"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-09-21"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""3"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""5"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-10-15"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""5"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""6"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-10-05"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""4"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""7"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-11-03"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""6"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040002"", ""redcap_repeat_instance"": ""8"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-11-18"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""7"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" }
            ]";
            var apiData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json);

            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN040002", "2", 1, new DateTime(2026, 6, 15));

            var entities = new List<CD45_CHAN_DOAN_Entity>();
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            converter.ConvertF6(apiData, "CD45", "API_CD45_F6", "REP1", ref entities, ref logs, context);

            Assert.AreEqual(8, entities.Count);
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "ERR_CHRONOLOGICAL_INVERSION"), "Should not report chronological inversion when retry order matches dates");
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "ERR_DUPLICATE_VISIT_ORDER"));
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "WARN_SKIPPED_VISIT_ORDER"));
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "WARN_F6_VISIT_TYPE_MISMATCH"));
        }

        [TestMethod]
        public void ConvertF6_WhenRetryOrderHasRealChronologicalInversion_ShouldLogErrorWithRetryLabels()
        {
            var converter = new ConvertCD45ApiToEntity();
            string json = @"[
              { ""record_id"": ""DHN040099"", ""redcap_repeat_instance"": ""1"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-07-01"", ""f6_examination"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040099"", ""redcap_repeat_instance"": ""2"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-10-05"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""4"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040099"", ""redcap_repeat_instance"": ""3"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-10-01"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""5"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" }
            ]";
            var apiData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json);

            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN040099", "2", 1, new DateTime(2026, 6, 15));

            var entities = new List<CD45_CHAN_DOAN_Entity>();
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();

            converter.ConvertF6(apiData, "CD45", "API_CD45_F6", "REP1", ref entities, ref logs, context);

            var inv = logs.Find(l => l.RULE_CODE == "ERR_CHRONOLOGICAL_INVERSION");
            Assert.IsNotNull(inv);
            StringAssert.Contains(inv.MESSAGE, "Tái khám lần 5");
            StringAssert.Contains(inv.MESSAGE, "Tái khám lần 4");

            var skipped = logs.Find(l => l.RULE_CODE == "WARN_SKIPPED_VISIT_ORDER");
            Assert.IsNotNull(skipped);
            Assert.AreEqual("f6_examination_retry", skipped.FIELD_NAME);
        }

        [TestMethod]
        public void ConvertF6_WhenOnlyRetryWithoutFirstVisit_ShouldWarnMismatch_AndBackfilledFirstVisitShouldNotWarn()
        {
            var converter = new ConvertCD45ApiToEntity();
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN040100", "2", 1, new DateTime(2026, 6, 15));
            context.RegisterClientF1("DHN040101", "2", 1, new DateTime(2026, 6, 15));

            // Case 1: Chỉ có Tái khám (Instance #1), không có Khám lần đầu -> phải cảnh báo
            string jsonNoFirst = @"[{ ""record_id"": ""DHN040100"", ""redcap_repeat_instance"": ""1"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-08-01"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" }]";
            var entities1 = new List<CD45_CHAN_DOAN_Entity>();
            var logs1 = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            converter.ConvertF6(Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonNoFirst), "CD45", "API_CD45_F6", "REP1", ref entities1, ref logs1, context);
            Assert.IsTrue(logs1.Exists(l => l.RULE_CODE == "WARN_F6_VISIT_TYPE_MISMATCH"));

            // Case 2: Nhập bù Khám lần đầu ở Instance #2 nhưng đúng ngày sớm hơn -> không cảnh báo giả
            string jsonBackfilled = @"[
              { ""record_id"": ""DHN040101"", ""redcap_repeat_instance"": ""1"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-08-01"", ""f6_examination"": ""2"", ""f6_examination_retry"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" },
              { ""record_id"": ""DHN040101"", ""redcap_repeat_instance"": ""2"", ""redcap_data_access_group"": ""its_t_time"", ""f6_date"": ""2026-07-01"", ""f6_examination"": ""1"", ""f6_thng_tin_chn_on_v_iu_tr_complete"": ""2"" }
            ]";
            var entities2 = new List<CD45_CHAN_DOAN_Entity>();
            var logs2 = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            converter.ConvertF6(Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonBackfilled), "CD45", "API_CD45_F6", "REP1", ref entities2, ref logs2, context);
            Assert.IsFalse(logs2.Exists(l => l.RULE_CODE == "WARN_F6_VISIT_TYPE_MISMATCH"));
            Assert.IsFalse(logs2.Exists(l => l.RULE_CODE == "ERR_CHRONOLOGICAL_INVERSION"));
        }

        [TestMethod]
        public void ConvertF2_WhenSkippedOrInvertedOrDuplicateInstance_ShouldIgnoreVisitOrderErrors_ButKeepSameDayDuplicateCheck()
        {
            var converter = new ConvertCD45ApiToEntity();
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DHN010352", "2", 1, new DateTime(2026, 1, 1));

            // F2 không có số thứ tự: bỏ qua nhảy cóc (1 -> 3), nghịch đảo ngày theo instance (4 trước 3), trùng instance,
            // nhưng vẫn giữ kiểm tra trùng sự kiện cùng ngày + cùng loại dịch vụ.
            string json = @"[
              { ""record_id"": ""DHN010352"", ""redcap_repeat_instance"": ""1"", ""redcap_data_access_group"": ""hn_vut"", ""f2_date"": ""2026-08-01"", ""f2_services"": ""1"", ""f2_truyn_thng_sinh_hot_nhm_vng_trn_chia_s_tr_liu_n_complete"": ""2"" },
              { ""record_id"": ""DHN010352"", ""redcap_repeat_instance"": ""3"", ""redcap_data_access_group"": ""hn_vut"", ""f2_date"": ""2026-09-19"", ""f2_services"": ""1"", ""f2_truyn_thng_sinh_hot_nhm_vng_trn_chia_s_tr_liu_n_complete"": ""2"" },
              { ""record_id"": ""DHN010352"", ""redcap_repeat_instance"": ""4"", ""redcap_data_access_group"": ""hn_vut"", ""f2_date"": ""2026-08-28"", ""f2_services"": ""2"", ""f2_truyn_thng_sinh_hot_nhm_vng_trn_chia_s_tr_liu_n_complete"": ""2"" },
              { ""record_id"": ""DHN010352"", ""redcap_repeat_instance"": ""4"", ""redcap_data_access_group"": ""hn_vut"", ""f2_date"": ""2026-08-28"", ""f2_services"": ""2"", ""f2_truyn_thng_sinh_hot_nhm_vng_trn_chia_s_tr_liu_n_complete"": ""2"" }
            ]";
            var apiData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json);

            var entities = new List<CD45_HOAT_DONG_Entity>();
            var logs = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            converter.ConvertF2(apiData, "CD45", "API_CD45_F2", "REP1", ref entities, ref logs, context);

            Assert.AreEqual(4, entities.Count);
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "WARN_SKIPPED_VISIT_ORDER"), "F2 must not log WARN_SKIPPED_VISIT_ORDER");
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "ERR_CHRONOLOGICAL_INVERSION"), "F2 must not log ERR_CHRONOLOGICAL_INVERSION");
            Assert.IsFalse(logs.Exists(l => l.RULE_CODE == "ERR_DUPLICATE_VISIT_ORDER"), "F2 must not log ERR_DUPLICATE_VISIT_ORDER");
            Assert.IsTrue(logs.Exists(l => l.RULE_CODE == "ERR_DUPLICATE_EVENT_SAME_DAY"), "F2 should still detect exact duplicate same-day same-service events");
        }

        [TestMethod]
        public void ExtractNhomTCV_F7_And_F8_MissingFields_ShouldMapAllSuffixesAndStripLeadingZeros()
        {
            var f7Cases = new (string Field, string RawVal, string ExpectedNhom, string ExpectedTcv)[]
            {
                ("f7_person_ad", "3", "ad", "3"),
                ("f7_person_tg", "2", "tg", "2"),
                ("f7_person_g3", "5", "g3vn", "5"),
                ("f7_person_ac", " 04 ", "alo", "4")
            };

            foreach (var tc in f7Cases)
            {
                string json = $@"[{{ ""record_id"": ""DHC070001"", ""{tc.Field}"": ""{tc.RawVal}"" }}]";
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json)[0];
                var res = CD45Helper.ExtractNhomTCV(model, "f7");
                Assert.AreEqual(tc.ExpectedNhom, res.maNhom, $"Failed group mapping for {tc.Field}");
                Assert.AreEqual(tc.ExpectedTcv, res.maTcv, $"Failed TCV mapping for {tc.Field}");
            }

            var f8Cases = new (string Field, string ExpectedNhom)[]
            {
                ("f8_person_qhx", "qhx"), ("f8_person_tt", "tt"), ("f8_person_gm", "gm"),
                ("f8_person_tk", "tk"), ("f8_person_hv", "hv"), ("f8_person_vtbb", "vtbb"),
                ("f8_person_bm", "bm"), ("f8_person_ct", "ct"), ("f8_person_hd", "hd"),
                ("f8_person_htn", "htn"), ("f8_person_hn", "hn"), ("f8_person_sv", "sv"),
                ("f8_person_hs", "hs"), ("f8_person_tg", "tg"), ("f8_person_g3", "g3vn"),
                ("f8_person_ac", "alo"), ("f8_person_lmmsm", "lmm")
            };

            foreach (var tc in f8Cases)
            {
                string json = $@"[{{ ""record_id"": ""DHN010001"", ""{tc.Field}"": ""7"" }}]";
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json)[0];
                var res = CD45Helper.ExtractNhomTCV(model, "f8");
                Assert.AreEqual(tc.ExpectedNhom, res.maNhom, $"Failed group mapping for {tc.Field}");
                Assert.AreEqual("7", res.maTcv, $"Failed TCV mapping for {tc.Field}");
            }
        }

        [TestMethod]
        public void ExtractNhomTCV_WhenMultiplePersonFieldsExist_ShouldPrioritizeFieldMatchingDag()
        {
            string json = @"[{
                ""record_id"": ""DHC070099"",
                ""redcap_data_access_group"": ""alocare"",
                ""f7_person_ad"": ""1"",
                ""f7_person_ac"": ""5""
            }]";
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(json)[0];
            var res = CD45Helper.ExtractNhomTCV(model, "f7");

            Assert.AreEqual("alo", res.maNhom);
            Assert.AreEqual("5", res.maTcv);
        }

        [TestMethod]
        public void ConvertF7_WhenGroupHas100PercentMissingTcv_ShouldLogWarnMissingTcvByGroup()
        {
            var converter = new ConvertCD45ApiToEntity();
            var context = new CD45ValidationContext();
            context.RegisterClientF1("DNA190001", "2", 1, new DateTime(2026, 5, 1));
            context.RegisterClientF1("DNA190002", "2", 1, new DateTime(2026, 5, 1));
            context.RegisterClientF1("DHC070001", "2", 1, new DateTime(2026, 5, 1));
            context.RegisterClientF1("DHC070002", "2", 1, new DateTime(2026, 5, 1));
            context.RegisterClientF1("DHC070003", "2", 1, new DateTime(2026, 5, 1));

            // Nhóm nh_dng (ad): 2/2 thiếu f7_person_ad -> phải cảnh báo
            // Nhóm alocare (alo): 2/2 thiếu f7_person_ac -> phải cảnh báo
            string jsonMissing = @"[
              { ""record_id"": ""DNA190001"", ""redcap_data_access_group"": ""nh_dng"", ""f7_date"": ""2026-06-01"", ""f7_phiu_t_vn_ln_1_complete"": ""2"" },
              { ""record_id"": ""DNA190002"", ""redcap_data_access_group"": ""nh_dng"", ""f7_date"": ""2026-06-02"", ""f7_phiu_t_vn_ln_1_complete"": ""2"" },
              { ""record_id"": ""DHC070001"", ""redcap_data_access_group"": ""alocare"", ""f7_date"": ""2026-06-01"", ""f7_phiu_t_vn_ln_1_complete"": ""2"" },
              { ""record_id"": ""DHC070002"", ""redcap_data_access_group"": ""alocare"", ""f7_date"": ""2026-06-02"", ""f7_phiu_t_vn_ln_1_complete"": ""2"" }
            ]";
            var entities1 = new List<CD45_TU_VAN_L1_Entity>();
            var logs1 = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            converter.ConvertF7(Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonMissing), "CD45", "API_CD45_F7", "2506", ref entities1, ref logs1, context);

            var warnAd = logs1.Find(l => l.RULE_CODE == "WARN_MISSING_TCV_BY_GROUP" && l.MA_NHOM == "ad");
            var warnAlo = logs1.Find(l => l.RULE_CODE == "WARN_MISSING_TCV_BY_GROUP" && l.MA_NHOM == "alo");
            Assert.IsNotNull(warnAd);
            Assert.AreEqual("WARNING", warnAd.SEVERITY);
            Assert.AreEqual("f7_person_ad", warnAd.FIELD_NAME);
            Assert.IsNotNull(warnAlo);
            Assert.AreEqual("f7_person_ac", warnAlo.FIELD_NAME);

            // Nhóm alocare có f7_person_ac = "3" -> không cảnh báo
            string jsonComplete = @"[{ ""record_id"": ""DHC070003"", ""redcap_data_access_group"": ""alocare"", ""f7_date"": ""2026-06-03"", ""f7_person_ac"": ""3"", ""f7_phiu_t_vn_ln_1_complete"": ""2"" }]";
            var entities2 = new List<CD45_TU_VAN_L1_Entity>();
            var logs2 = new List<BVTL_DATA_STANDARDIZATION_LOG_Entity>();
            converter.ConvertF7(Newtonsoft.Json.JsonConvert.DeserializeObject<List<DreamhBaseApiModel>>(jsonComplete), "CD45", "API_CD45_F7", "2506", ref entities2, ref logs2, context);

            Assert.IsFalse(logs2.Exists(l => l.RULE_CODE == "WARN_MISSING_TCV_BY_GROUP"));
        }
    }
}
