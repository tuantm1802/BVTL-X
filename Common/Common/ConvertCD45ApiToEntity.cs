using System;
using System.Collections.Generic;
using System.Linq;
using Model.ModelExtend.API.CD45;

namespace Common.Common
{
    public class ConvertCD45ApiToEntity
    {
        public void ConvertF1(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, 
            ref List<CD45_KH_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_KH", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                var e = new CD45_KH_Entity();
                e.RECORD_ID = cleanRecordId;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                // Xử lý redcap_data_access_group
                e.REDCAP_DAG = item.redcap_data_access_group;
                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_KH", reportId, maDuAn, 
                        ref logs, out string std, out string map, out string city, dagCollector);
                    e.MA_NHOM = map ?? std;
                }

                // Xử lý ngày tham gia
                string rawDate = item.GetString("f1_date");
                e.NGAY_THAM_GIA = DataCleanerHelper.CleanDate(rawDate, "f1_date", cleanRecordId, apiCode, "CD45_KH", reportId, maDuAn, ref logs);
                
                // Tham gia nghiên cứu & Mã KH nghiên cứu
                e.THAM_GIA_NGHIEN_CUU = item.GetBool("f1_pre_research");
                e.MA_KH_NGHIEN_CUU = item.GetString("f1_cus_id");

                // f1_q_a4 = doi tuong (1=PUD,2=PLHIV,3=TG,4=MSM,5=SW)
                e.DOI_TUONG = item.GetByte("f1_q_a4");

                // Mapping gioi tinh, nam sinh
                e.GIOI_TINH_TU_XD = item.GetByte("f1_q_a1");
                e.GIOI_TINH_KHAI_SINH = item.GetByte("f1_q_a2");
                e.NAM_SINH = DataCleanerHelper.CleanYearOfBirth(item.GetInt("f1_q_a3"), cleanRecordId, apiCode, "CD45_KH", reportId, maDuAn, ref logs);
                
                // Checkbox "Đối tượng khác" f1_q_a5___0...5 (0=Không, 1=PUD, 2=PLHIV, 3=TG, 4=MSM, 5=SW)
                List<string> dtKhac = new List<string>();
                for (int i = 0; i <= 5; i++) {
                    if (item.GetBool($"f1_q_a5___{i}") == true) dtKhac.Add(i.ToString());
                }
                e.DOI_TUONG_KHAC = string.Join(",", dtKhac);

                // A6..A10 (1=Có, 2=Không,...)
                e.CO_CCCD = item.GetBool("f1_q_a6");
                e.CO_THUONG_TRU = item.GetBool("f1_q_a7");
                e.CO_BHYT = item.GetBool("f1_q_a8");
                e.VO_GIA_CU_6T = item.GetBool("f1_q_a9");
                e.DANG_VO_GIA_CU = item.GetBool("f1_q_a9_1");
                e.BI_TAM_GIU_6T = item.GetBool("f1_q_a10");

                // Hôn nhân & Số con
                e.HON_NHAN = item.GetByte("f1_q_a11");
                e.SO_CON = item.GetByte("f1_q_a12");
                
                e.COMPLETE_STATUS = item.GetString("thng_tin_khch_hng_complete") ?? item.GetString("f1_thng_tin_khch_hng_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F1", apiCode, "CD45_KH", reportId, maDuAn, ref logs);

                // VR-07(c): Kiểm tra mâu thuẫn nghiên cứu
                DataCleanerHelper.CheckContradictoryRules(cleanRecordId, e.THAM_GIA_NGHIEN_CUU, e.MA_KH_NGHIEN_CUU, e.DOI_TUONG, null, apiCode, "CD45_KH", reportId, maDuAn, ref logs);

                // Đăng ký vào context
                if (context != null)
                {
                    context.RegisterClientF1(cleanRecordId, e.COMPLETE_STATUS, e.DOI_TUONG, e.NGAY_THAM_GIA, e.MA_NHOM, e.CITY_CODE);
                }
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_KH", reportId, ref logs);
        }

        public void ConvertF2(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, 
            ref List<CD45_HOAT_DONG_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                // Lọc bỏ bản ghi cơ sở rỗng
                string rawDate = item.GetString("f2_date");
                byte? loaiDv = item.GetByte("f2_services");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate) && !loaiDv.HasValue)
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F2 (Truyền thông / Sinh hoạt)", apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                    continue;
                }

                if (string.IsNullOrEmpty(rawDate) && !loaiDv.HasValue && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_HOAT_DONG_Entity();
                e.RECORD_ID = cleanRecordId;
                
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;

                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;
                
                // Xử lý DAG & TCV
                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f2");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, 
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.LOAI_DV = loaiDv;
                e.NGAY_HOAT_DONG = DataCleanerHelper.CleanDate(rawDate, "f2_date", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_HOAT_DONG, "f2_date", context, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_HOAT_DONG, context, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);

                e.DIA_DIEM = item.GetByte("f2_location");
                e.DIA_DIEM_NGOAI = DataCleanerHelper.CleanString(item.GetString("f2_location_outside"), "f2_location_outside", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                e.DIA_DIEM_KHAC = DataCleanerHelper.CleanString(item.GetString("f2_location_other"), "f2_location_other", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                
                // Chủ đề truyền thông / sinh hoạt
                e.CHU_DE = DataCleanerHelper.CleanString(item.GetString("f2_topic"), "f2_topic", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);

                e.SO_TAI_LIEU = item.GetInt("f2_document_delivery");
                e.DONG_Y_QST = item.GetBool("f2_join");
                e.SO_BAO_CAO_SU = item.GetInt("f2_condom_delivery");
                e.SO_CHAT_BOI_TRAN = item.GetInt("f2_gel_delivery");
                e.SO_BOM_KIM = item.GetInt("f2_syringe_delivery");
                e.GHI_CHU = DataCleanerHelper.CleanString(item.GetString("f2_note"), "f2_note", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                
                e.COMPLETE_STATUS = item.GetString("f2_truyn_thng_sinh_hot_nhm_vng_trn_chia_s_tr_liu_n_complete") ?? item.GetString("truyn_thng_v_sinh_hot_nhm_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F2", apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_HOAT_DONG", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_HOAT_DONG, x => x.REPEAT_INSTANCE, x => x.LOAI_DV, "CD45_HOAT_DONG", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_HOAT_DONG, x => x.COMPLETE_STATUS, "CD45_HOAT_DONG", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF3(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_QST_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_QST", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f3_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");
                byte? q1a = item.GetByte("f3_q1a");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate) && !q1a.HasValue)
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F3 (QST)", apiCode, "CD45_QST", reportId, maDuAn, ref logs);
                    continue;
                }

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument) && !q1a.HasValue)
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_QST", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_QST_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f3");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_QST", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.LY_DO_DANH_GIA_LAI = item.GetByte("f3_repeat_assessment");
                e.NGAY_SANG_LOC = DataCleanerHelper.CleanDate(rawDate, "f3_date", cleanRecordId, apiCode, "CD45_QST", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_SANG_LOC, "f3_date", context, apiCode, "CD45_QST", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_SANG_LOC, context, apiCode, "CD45_QST", reportId, maDuAn, ref logs);

                e.Q1A = q1a;
                e.Q1B = item.GetByte("f3_q1b");
                e.Q1C = item.GetByte("f3_q1c");
                e.Q1D = item.GetByte("f3_q1d");
                e.Q1E = item.GetByte("f3_q1e");
                e.Q1F = item.GetByte("f3_q1f");
                e.Q2_TU_HARM = item.GetByte("f3_q2");
                e.Q3_NGHE = item.GetByte("f3_q3");

                // VR-06(c) [BLOCKING]: Tính điểm và chuẩn hóa tổng điểm khớp 10 câu hỏi thành phần
                int? score = item.GetInt("f3_score");
                int calculatedScore = (e.Q1A ?? 0) + (e.Q1B ?? 0) + (e.Q1C ?? 0) + (e.Q1D ?? 0) + (e.Q1E ?? 0) + (e.Q1F ?? 0) + (e.Q2_TU_HARM == 2 ? 2 : 0) + (e.Q3_NGHE == 2 ? 2 : 0);

                if (score.HasValue && score.Value != calculatedScore)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_QST",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f3_score",
                        OLD_VALUE = score.Value.ToString(),
                        NEW_VALUE = calculatedScore.ToString(),
                        RULE_CODE = "R_QST_SCORE_STANDARDIZED",
                        SEVERITY = "INFO",
                        ACTION_TAKEN = "AUTO_NORMALIZED",
                        MESSAGE = $"Tổng điểm QST từ REDCap ({score.Value}) không khớp 10 câu thành phần ({calculatedScore}). Đã tự động chuẩn hóa về {calculatedScore}.",
                        CREATED_DATE = DateTime.Now
                    });
                    score = calculatedScore;
                }
                else if (!score.HasValue && (e.Q1A.HasValue || e.Q1B.HasValue || e.Q1C.HasValue || e.Q1D.HasValue || e.Q1E.HasValue || e.Q1F.HasValue))
                {
                    score = calculatedScore;
                }
                e.DIEM_QST = score;

                // VR-06(c) [BLOCKING]: Phân loại MUC_QST. Nếu có nguy cơ tự làm hại bản thân (Q2 = 2) -> BẮT BUỘC Mức 1
                byte? standardMuc = null;
                if (e.Q2_TU_HARM == 2)
                {
                    standardMuc = 1; // Khách hàng cần chuyển gửi gấp
                }
                else if (score.HasValue)
                {
                    if (score.Value >= 8) standardMuc = 1;
                    else if (score.Value >= 6) standardMuc = 2;
                    else if (score.Value >= 4) standardMuc = 3;
                    else standardMuc = 4;
                }

                byte? rawMuc = item.GetByte("f3_level");
                if (rawMuc.HasValue && standardMuc.HasValue && rawMuc.Value != standardMuc.Value)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_QST",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f3_level",
                        OLD_VALUE = rawMuc.Value.ToString(),
                        NEW_VALUE = standardMuc.Value.ToString(),
                        RULE_CODE = "R_QST_MUC_STANDARDIZED",
                        SEVERITY = "INFO",
                        ACTION_TAKEN = "AUTO_NORMALIZED",
                        MESSAGE = $"Phân loại Mức QST từ REDCap (Mức {rawMuc.Value}) chưa khớp điểm/nguy cơ tự hại. Đã chuẩn hóa về Mức {standardMuc.Value}.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                e.MUC_QST = standardMuc ?? rawMuc;

                e.COMPLETE_STATUS = item.GetString("f3_bng_hi_sng_lc_sc_kho_tm_thn_qst_complete") ?? item.GetString("bng_c_c_sng_lc_sktt_qst_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F3", apiCode, "CD45_QST", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_QST", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_SANG_LOC, x => x.REPEAT_INSTANCE, null, "CD45_QST", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_SANG_LOC, x => x.COMPLETE_STATUS, "CD45_QST", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF4(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_HO_TRO_XH_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f4_time");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F4 (Hỗ trợ xã hội)", apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);
                    continue;
                }

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_HO_TRO_XH_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f4");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_HO_TRO = DataCleanerHelper.CleanDate(rawDate, "f4_time", cleanRecordId, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_HO_TRO, "f4_time", context, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_HO_TRO, context, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);

                // Checkbox dịch vụ hỗ trợ (1..10)
                var services = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    if (item.GetString($"f4_services___{i}") == "1")
                    {
                        services.Add(i.ToString());
                    }
                }
                e.DICH_VU = services.Count > 0 ? string.Join(",", services) : null;

                e.KET_QUA_HIV = item.GetByte("f4_hiv_result");

                // VR-07(c): Cảnh báo nếu khách hàng là PLHIV nhưng vẫn làm test mới
                if (context != null && context.KhachHangLookup.TryGetValue(cleanRecordId, out var f1Client))
                {
                    DataCleanerHelper.CheckContradictoryRules(cleanRecordId, null, null, f1Client.DoiTuong, e.KET_QUA_HIV, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);
                }

                e.COMPLETE_STATUS = item.GetString("f4_phiu_h_tr_x_hi_khc_complete") ?? item.GetString("phiu_h_tr_x_hi_khc_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F4", apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_HO_TRO_XH", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_HO_TRO, x => x.REPEAT_INSTANCE, null, "CD45_HO_TRO_XH", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_HO_TRO, x => x.COMPLETE_STATUS, "CD45_HO_TRO_XH", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF5(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TUAN_THU_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f5_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F5 (Tuân thủ điều trị)", apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);
                    continue;
                }

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra điều kiện tiên quyết: Bắt buộc đã có bản ghi khám SKTT (F6)
                if (!DataCleanerHelper.ValidateF5Dependency(cleanRecordId, context, apiCode, reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_TUAN_THU_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f5");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_TUAN_THU", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_HO_TRO = DataCleanerHelper.CleanDate(rawDate, "f5_date", cleanRecordId, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_HO_TRO, "f5_date", context, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_HO_TRO, context, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);

                e.HINH_THUC_DIEU_TRI = item.GetByte("f5_indication");
                e.CO_KE_DON_THUOC = item.GetBool("f5_prescribe");
                e.TUAN_THU = item.GetByte("f5_adherence");

                e.COMPLETE_STATUS = item.GetString("f5_phiu_h_tr_tun_th_iu_tr_complete") ?? item.GetString("phiu_h_tr_tun_th_iu_tr_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F5", apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_TUAN_THU", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_HO_TRO, x => x.REPEAT_INSTANCE, null, "CD45_TUAN_THU", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_HO_TRO, x => x.COMPLETE_STATUS, "CD45_TUAN_THU", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF6(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_CHAN_DOAN_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f6_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F6 (Khám SKTT)", apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                    continue;
                }

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_CHAN_DOAN_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f6");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_KHAM = DataCleanerHelper.CleanDate(rawDate, "f6_date", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_KHAM, "f6_date", context, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_KHAM, context, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);

                e.CO_SO_Y_TE = DataCleanerHelper.CleanString(item.GetString("f6_hospital"), "f6_hospital", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.BAC_SI = DataCleanerHelper.CleanString(item.GetString("f6_doctor"), "f6_doctor", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.LAN_KHAM = item.GetByte("f6_examination");

                // VR-06(b) [WARNING]: Kiểm tra lần khám đầu tiên phải là Khám đầu (1), các lần sau phải là Tái khám (2)
                if (e.LAN_KHAM == 1 && e.REPEAT_INSTANCE > 1)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_CHAN_DOAN",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f6_examination",
                        OLD_VALUE = "1 (Khám lần đầu)",
                        NEW_VALUE = null,
                        RULE_CODE = "WARN_F6_VISIT_TYPE_MISMATCH",
                        SEVERITY = "WARNING",
                        ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                        MESSAGE = $"Khách hàng {cleanRecordId} tại Instance #{e.REPEAT_INSTANCE} vẫn ghi nhận 'Khám lần đầu'. Theo logic cần là 'Tái khám'.",
                        CREATED_DATE = DateTime.Now
                    });
                }
                else if (e.LAN_KHAM == 2 && e.REPEAT_INSTANCE == 1)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_CHAN_DOAN",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f6_examination",
                        OLD_VALUE = "2 (Tái khám)",
                        NEW_VALUE = null,
                        RULE_CODE = "WARN_F6_VISIT_TYPE_MISMATCH",
                        SEVERITY = "WARNING",
                        ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                        MESSAGE = $"Khách hàng {cleanRecordId} tại Instance #1 lại ghi nhận 'Tái khám'. Theo logic cần là 'Khám lần đầu'.",
                        CREATED_DATE = DateTime.Now
                    });
                }

                e.TRIEU_CHUNG = DataCleanerHelper.CleanString(item.GetString("f6_symptom") ?? item.GetString("f6_symptom_other"), "f6_symptom", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.CHAN_DOAN_CHINH = DataCleanerHelper.CleanString(item.GetString("f6_diagnose_pri"), "f6_diagnose_pri", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                
                // Nguy cơ
                string nguyCo = null;
                if (item.GetString("f6_evaluate_suicide") == "1") nguyCo = "Nguy cơ tự sát";
                if (item.GetString("f6_harmful") == "1") nguyCo = (nguyCo != null ? nguyCo + ", " : "") + "Nguy cơ gây hại cho người khác";
                e.NGUY_CO = nguyCo;

                // Hình thức điều trị (checkbox 1..4)
                var treatments = new List<string>();
                for (int i = 1; i <= 4; i++)
                {
                    if (item.GetString($"f6_treatment___{i}") == "1")
                    {
                        treatments.Add(i.ToString());
                    }
                }
                e.HINH_THUC_DIEU_TRI = treatments.Count > 0 ? string.Join(",", treatments) : null;

                e.NGAY_HEN_TAI_KHAM = DataCleanerHelper.CleanDate(item.GetString("f6_followup_visit"), "f6_followup_visit", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs, allowFutureDate: true);

                // VR-03(c) [BLOCKING]: Chặn ngày hẹn tái khám trước ngày khám
                if (e.NGAY_HEN_TAI_KHAM.HasValue && e.NGAY_KHAM.HasValue && e.NGAY_HEN_TAI_KHAM.Value < e.NGAY_KHAM.Value)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_CHAN_DOAN",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f6_followup_visit",
                        OLD_VALUE = e.NGAY_HEN_TAI_KHAM.Value.ToString("yyyy-MM-dd"),
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F6_APPOINTMENT_BEFORE_VISIT",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Ngày hẹn tái khám ({e.NGAY_HEN_TAI_KHAM.Value:dd/MM/yyyy}) xảy ra trước ngày khám ({e.NGAY_KHAM.Value:dd/MM/yyyy}). Bắt buộc cách ly trường ngày hẹn.",
                        CREATED_DATE = DateTime.Now
                    });
                    e.NGAY_HEN_TAI_KHAM = null;
                }

                e.COMPLETE_STATUS = item.GetString("f6_thng_tin_chn_on_v_iu_tr_complete") ?? item.GetString("thng_tin_chn_on_v_iu_tr_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // Đăng ký vào context để làm tiền đề cho F5
                if (context != null)
                {
                    context.RegisterF6Visit(cleanRecordId);
                }

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F6", apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_CHAN_DOAN", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_KHAM, x => (int?)x.LAN_KHAM ?? x.REPEAT_INSTANCE, null, "CD45_CHAN_DOAN", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_KHAM, x => x.COMPLETE_STATUS, "CD45_CHAN_DOAN", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF7(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TU_VAN_L1_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f7_date");
                string completeStatus = item.GetString("f7_phiu_t_vn_ln_1_complete") ?? item.GetString("phiu_t_vn_t_ln_1_complete");

                // F7 là form đơn (non-repeating). Lọc bỏ bản ghi rỗng
                if (string.IsNullOrEmpty(rawDate) && (string.IsNullOrEmpty(completeStatus) || completeStatus == "0"))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_TU_VAN_L1_Entity();
                e.RECORD_ID = cleanRecordId;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f7");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_TU_VAN = DataCleanerHelper.CleanDate(rawDate, "f7_date", cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_TU_VAN, "f7_date", context, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_TU_VAN, context, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);

                // Địa điểm
                byte? diaDiem = item.GetByte("f7_location");
                if (!diaDiem.HasValue)
                {
                    string locStr = item.GetString("f7_location");
                    if (!string.IsNullOrEmpty(locStr))
                    {
                        if (locStr.IndexOf("văn phòng", StringComparison.OrdinalIgnoreCase) >= 0 || locStr.IndexOf("nhóm", StringComparison.OrdinalIgnoreCase) >= 0)
                            diaDiem = 1;
                        else
                            diaDiem = 2;
                    }
                }
                e.DIA_DIEM = diaDiem;

                // AUDIT-C score (f7_qa1_1 + f7_qa1_2 + f7_qa1_3)
                int? qa1_1 = item.GetInt("f7_qa1_1");
                int? qa1_2 = item.GetInt("f7_qa1_2");
                int? qa1_3 = item.GetInt("f7_qa1_3");
                if (qa1_1.HasValue || qa1_2.HasValue || qa1_3.HasValue)
                {
                    e.AUDIT_C_SCORE = (qa1_1 ?? 0) + (qa1_2 ?? 0) + (qa1_3 ?? 0);
                }

                // PCL-5 score & positive
                int? pclScore = item.GetInt("f7_score");
                if (!pclScore.HasValue && item.GetString("f7_qc1") == "1")
                {
                    pclScore = (item.GetInt("f7_qc2") ?? 0) + (item.GetInt("f7_qc3") ?? 0) + (item.GetInt("f7_qc4") ?? 0) + (item.GetInt("f7_qc5") ?? 0) + (item.GetInt("f7_qc6") ?? 0);
                }
                e.PCL5_SCORE = pclScore;
                if (pclScore.HasValue)
                {
                    e.PCL5_POSITIVE = pclScore.Value >= 3;
                }

                // STIGMA score (f7qd_1 .. f7qd_5)
                int? qd1 = item.GetInt("f7qd_1");
                int? qd2 = item.GetInt("f7qd_2");
                int? qd3 = item.GetInt("f7qd_3");
                int? qd4 = item.GetInt("f7qd_4");
                int? qd5 = item.GetInt("f7qd_5");
                if (qd1.HasValue || qd2.HasValue || qd3.HasValue || qd4.HasValue || qd5.HasValue)
                {
                    e.STIGMA_SCORE = (qd1 ?? 0) + (qd2 ?? 0) + (qd3 ?? 0) + (qd4 ?? 0) + (qd5 ?? 0);
                }

                // Tình trạng SKTT (f7_q3a checkbox 1..5)
                for (int i = 1; i <= 5; i++)
                {
                    if (item.GetString($"f7_q3a___{i}") == "1")
                    {
                        e.TINH_TRANG_SKTT = (byte)i;
                        break;
                    }
                }

                e.NHU_CAU_HO_TRO = DataCleanerHelper.CleanString(item.GetString("f7_q3c"), "f7_q3c", cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);
                e.LICH_HEN_TIEP = DataCleanerHelper.CleanDate(item.GetString("f7_q61"), "f7_q61", cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs, allowFutureDate: true);

                // VR-03(c) [BLOCKING]: Chặn ngày hẹn tiếp theo trước ngày tư vấn
                if (e.LICH_HEN_TIEP.HasValue && e.NGAY_TU_VAN.HasValue && e.LICH_HEN_TIEP.Value < e.NGAY_TU_VAN.Value)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_TU_VAN_L1",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f7_q61",
                        OLD_VALUE = e.LICH_HEN_TIEP.Value.ToString("yyyy-MM-dd"),
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F7_APPOINTMENT_BEFORE_VISIT",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Lịch hẹn tư vấn tiếp theo ({e.LICH_HEN_TIEP.Value:dd/MM/yyyy}) xảy ra trước ngày tư vấn ({e.NGAY_TU_VAN.Value:dd/MM/yyyy}). Bắt buộc cách ly trường ngày hẹn.",
                        CREATED_DATE = DateTime.Now
                    });
                    e.LICH_HEN_TIEP = null;
                }

                e.COMPLETE_STATUS = completeStatus;
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // Đăng ký F7 Complete vào context làm tiền đề cho F8
                if (context != null && e.COMPLETE_STATUS == "2")
                {
                    context.RegisterF7Complete(cleanRecordId);
                }

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F7", apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_TU_VAN_L1", reportId, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_TU_VAN, x => x.COMPLETE_STATUS, "CD45_TU_VAN_L1", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF8(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TU_VAN_L2_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f8_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F8 (Tư vấn lần 2)", apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);
                    continue;
                }

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra điều kiện tiên quyết: Bắt buộc đã có F7 Complete
                if (!DataCleanerHelper.ValidateF8Dependency(cleanRecordId, context, apiCode, reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_TU_VAN_L2_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, "f8");
                e.MA_NHOM = tcvInfo.maNhom;
                e.MA_TCV = tcvInfo.maTcv;

                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_TU_VAN = DataCleanerHelper.CleanDate(rawDate, "f8_date", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_TU_VAN, "f8_date", context, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_TU_VAN, context, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);

                byte? diaDiem = item.GetByte("f8_location");
                if (!diaDiem.HasValue)
                {
                    string locStr = item.GetString("f8_location");
                    if (!string.IsNullOrEmpty(locStr))
                    {
                        if (locStr.IndexOf("văn phòng", StringComparison.OrdinalIgnoreCase) >= 0 || locStr.IndexOf("nhóm", StringComparison.OrdinalIgnoreCase) >= 0)
                            diaDiem = 1;
                        else
                            diaDiem = 2;
                    }
                }
                e.DIA_DIEM = diaDiem;

                // Đánh giá hiện tại
                e.DANH_GIA_HIEN_TAI = DataCleanerHelper.CleanString(item.GetString("f8_q2_1_a_1") ?? item.GetString("f8_q2_1_a_2") ?? item.GetString("f8_q2_2"), "f8_q2", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);

                // Can thiệp áp dụng
                e.CAN_THIEP_AP_DUNG = DataCleanerHelper.CleanString(item.GetString("f8_q32") ?? item.GetString("f8_q41"), "f8_q32", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);

                e.NGAY_HEN_TIEP = DataCleanerHelper.CleanDate(item.GetString("f8_q61"), "f8_q61", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs, allowFutureDate: true);

                // VR-03(c) [BLOCKING]: Chặn ngày hẹn tư vấn tiếp theo trước ngày tư vấn
                if (e.NGAY_HEN_TIEP.HasValue && e.NGAY_TU_VAN.HasValue && e.NGAY_HEN_TIEP.Value < e.NGAY_TU_VAN.Value)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = "CD45_TU_VAN_L2",
                        RECORD_ID = cleanRecordId,
                        FIELD_NAME = "f8_q61",
                        OLD_VALUE = e.NGAY_HEN_TIEP.Value.ToString("yyyy-MM-dd"),
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_F8_APPOINTMENT_BEFORE_VISIT",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "QUARANTINED",
                        MESSAGE = $"Ngày hẹn tư vấn tiếp theo ({e.NGAY_HEN_TIEP.Value:dd/MM/yyyy}) xảy ra trước ngày tư vấn ({e.NGAY_TU_VAN.Value:dd/MM/yyyy}). Bắt buộc cách ly trường ngày hẹn.",
                        CREATED_DATE = DateTime.Now
                    });
                    e.NGAY_HEN_TIEP = null;
                }

                e.COMPLETE_STATUS = item.GetString("f8_phiu_t_vn_t_ln_2_complete") ?? item.GetString("phiu_t_vn_t_ln_2_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F8", apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_TU_VAN_L2", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_TU_VAN, x => x.REPEAT_INSTANCE, null, "CD45_TU_VAN_L2", apiCode, reportId, maDuAn, ref logs);

            // VR-07(b): Kiểm tra cụm incomplete
            CheckClusterIncomplete(entities, x => x.MA_TCV, x => x.NGAY_TU_VAN, x => x.COMPLETE_STATUS, "CD45_TU_VAN_L2", apiCode, reportId, maDuAn, ref logs, x => x.RECORD_ID, x => x.MA_NHOM, context);
        }

        public void ConvertF9(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_THEO_DAU_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f9_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F9 (Theo dõi mất dấu)", apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs);
                    continue;
                }

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_THEO_DAU_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_THEO_DAU", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    e.MA_NHOM = map ?? std;
                }

                e.NGAY_THEO_DAU = DataCleanerHelper.CleanDate(rawDate, "f9_date", cleanRecordId, apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_THEO_DAU, "f9_date", context, apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                e.HINH_THUC_LIEN_HE = item.GetByte("f9_reach");
                e.KET_QUA = item.GetByte("f9_result");
                e.MAT_DAU = item.GetBool("f9_loss");

                // Đăng ký ngày mất dấu vào context để kiểm tra các dịch vụ sau mất dấu (VR-06d)
                if (e.MAT_DAU == true && e.NGAY_THEO_DAU.HasValue && context != null)
                {
                    context.RegisterF9Lost(cleanRecordId, e.NGAY_THEO_DAU.Value);
                }

                e.COMPLETE_STATUS = item.GetString("f9_theo_di_khch_hng_complete") ?? item.GetString("theo_di_khch_hng_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F9", apiCode, "CD45_THEO_DAU", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_THEO_DAU", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_THEO_DAU, x => x.REPEAT_INSTANCE, null, "CD45_THEO_DAU", apiCode, reportId, maDuAn, ref logs);
        }

        public void ConvertF10(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_VAN_TAY_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null)
        {
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f10_ngay");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // VR-07(a): Cảnh báo Repeat Instance rỗng
                if (repeatInstance.HasValue && repeatInstance > 1 && string.IsNullOrEmpty(rawDate))
                {
                    DataCleanerHelper.LogEmptyInstance(cleanRecordId, repeatInstance, "F10 (Vân tay)", apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);
                    continue;
                }

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
                {
                    continue;
                }

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                var e = new CD45_VAN_TAY_Entity();
                e.RECORD_ID = cleanRecordId;
                if (!repeatInstance.HasValue) repeatInstance = 1;
                e.REPEAT_INSTANCE = repeatInstance;
                e.CITY_CODE = CD45Helper.ExtractCityCode(cleanRecordId);
                e.MADUAN = maDuAn;

                e.REDCAP_DAG = item.redcap_data_access_group;
                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, "CD45_VAN_TAY", reportId, maDuAn,
                        ref logs, out string std, out string map, out string city, dagCollector);
                    e.MA_NHOM = map ?? std;
                }

                e.NGAY_GHI_NHAN = DataCleanerHelper.CleanDate(rawDate, "f10_ngay", cleanRecordId, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);

                // VR-03(b): Kiểm tra ngày dịch vụ >= ngày tham gia F1
                if (!DataCleanerHelper.ValidateServiceDateAgainstF1(cleanRecordId, e.NGAY_GHI_NHAN, "f10_ngay", context, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs))
                {
                    continue;
                }

                // VR-06(d): Kiểm tra sau ngày mất dấu
                DataCleanerHelper.ValidateServiceAfterLostToFollowUp(cleanRecordId, e.NGAY_GHI_NHAN, context, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);

                e.DICH_VU = item.GetByte("f10_dichvu");
                e.VAN_DE = item.GetByte("f10_vande") ?? item.GetByte("f10_vande_2");
                e.PHUONG_AN = item.GetByte("f10_phuongan");
                e.COMPLETE_STATUS = item.GetString("f10_vn_tay_khch_hng_complete") ?? item.GetString("vn_tay_khch_hng_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);

                // VR-04(b): Cảnh báo form incomplete
                DataCleanerHelper.CheckFormCompletionStatus(cleanRecordId, e.COMPLETE_STATUS, "F10", apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, "CD45_VAN_TAY", reportId, ref logs);

            // VR-05(a, b, c): Kiểm tra chuỗi tiến trình và lần thứ
            ValidateServiceProgress(entities, x => x.RECORD_ID, x => x.NGAY_GHI_NHAN, x => x.REPEAT_INSTANCE, null, "CD45_VAN_TAY", apiCode, reportId, maDuAn, ref logs);
        }
        
        public void ConvertGeneric<T>(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, string tableName,
            string prefix, string dateField, string completeField, ref List<T> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs, CD45ValidationContext context = null) where T : new()
        {
            var props = typeof(T).GetProperties();
            var dagCollector = new DagSummaryCollector();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, tableName, reportId, maDuAn, ref logs, item.redcap_data_access_group);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                // VR-04(a): Kiểm tra ràng buộc hồ sơ gốc F1
                if (!tableName.Equals("CD45_KH", StringComparison.OrdinalIgnoreCase))
                {
                    if (!DataCleanerHelper.ValidateClientF1(cleanRecordId, context, apiCode, tableName, reportId, maDuAn, ref logs))
                    {
                        continue;
                    }
                }

                var e = new T();
                
                // Base
                SetProp(e, props, "RECORD_ID", cleanRecordId);
                int? repeat = item.GetInt("redcap_repeat_instance");
                if (repeat == null) repeat = 1;
                SetProp(e, props, "REPEAT_INSTANCE", repeat);
                SetProp(e, props, "CITY_CODE", CD45Helper.ExtractCityCode(cleanRecordId));
                SetProp(e, props, "MADUAN", maDuAn);
                SetProp(e, props, "NGAY_SYNC", DateTime.Now);
                SetProp(e, props, "REDCAP_DAG", item.redcap_data_access_group);
                
                // TCV & Nhom
                var tcvInfo = CD45Helper.ExtractNhomTCV(item, prefix);
                string maNhom = tcvInfo.maNhom;
                if (!string.IsNullOrEmpty(item.redcap_data_access_group))
                {
                    DataCleanerHelper.ProcessDag(item.redcap_data_access_group, cleanRecordId, apiCode, tableName, reportId, maDuAn, 
                        ref logs, out string std, out string map, out string city, dagCollector);
                    if (string.IsNullOrEmpty(maNhom)) maNhom = map ?? std;
                }
                SetProp(e, props, "MA_NHOM", maNhom);
                SetProp(e, props, "MA_TCV", tcvInfo.maTcv);

                // Auto parse other properties
                foreach (var prop in props)
                {
                    if (prop.Name == "RECORD_ID" || prop.Name == "REPEAT_INSTANCE" || prop.Name == "CITY_CODE" || 
                        prop.Name == "MADUAN" || prop.Name == "NGAY_SYNC" || prop.Name == "MA_NHOM" || prop.Name == "MA_TCV" || prop.Name == "REDCAP_DAG")
                        continue;
                        
                    string expectedKey = prefix + "_" + prop.Name.ToLower();
                    
                    // Special fields mapping override
                    if (prop.Name == "COMPLETE_STATUS") expectedKey = completeField;
                    if (prop.Name.StartsWith("NGAY_")) expectedKey = dateField;

                    // If API JSON has the key
                    if (item.AdditionalData != null && item.AdditionalData.ContainsKey(expectedKey))
                    {
                        var val = item.GetString(expectedKey);
                        if (!string.IsNullOrEmpty(val))
                        {
                            try {
                                if (prop.PropertyType == typeof(byte?) || prop.PropertyType == typeof(byte))
                                    prop.SetValue(e, byte.Parse(val));
                                else if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(int))
                                    prop.SetValue(e, int.Parse(val));
                                else if (prop.PropertyType == typeof(bool?) || prop.PropertyType == typeof(bool))
                                    prop.SetValue(e, val == "1" || val.ToLower() == "true");
                                else if (prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime))
                                {
                                    var cleanedDate = DataCleanerHelper.CleanDate(val, expectedKey, cleanRecordId, apiCode, tableName, reportId, maDuAn, ref logs);
                                    prop.SetValue(e, cleanedDate);
                                }
                                else if (prop.PropertyType == typeof(string))
                                {
                                    var cleanedStr = DataCleanerHelper.CleanString(val, expectedKey, cleanRecordId, apiCode, tableName, reportId, maDuAn, ref logs);
                                    prop.SetValue(e, cleanedStr);
                                }
                            } catch { }
                        }
                    }
                }
                
                entities.Add(e);
            }

            dagCollector.FlushToLogs(maDuAn, apiCode, tableName, reportId, ref logs);
        }

        #region Helper Kiểm Soát Chuỗi Tiến Trình (VR-05) & Cụm Incomplete (VR-07b)

        /// <summary>
        /// VR-05(a, b, c). Kiểm tra chuỗi tiến trình dịch vụ: không trùng lặp sự kiện cùng ngày,
        /// không trùng số lần, đảm bảo ngày lần sau >= ngày lần trước, cảnh báo nhảy cóc.
        /// </summary>
        private void ValidateServiceProgress<T>(
            List<T> entities,
            Func<T, string> getRecordId,
            Func<T, DateTime?> getDate,
            Func<T, int?> getOrder,
            Func<T, byte?> getServiceType,
            string tableName,
            string apiCode,
            string reportId,
            string maDuAn,
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            if (entities == null || entities.Count == 0 || logs == null) return;

            var groupedByClient = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in entities)
            {
                string rid = getRecordId(item);
                if (string.IsNullOrEmpty(rid)) continue;
                if (!groupedByClient.TryGetValue(rid, out var list))
                {
                    list = new List<T>();
                    groupedByClient[rid] = list;
                }
                list.Add(item);
            }

            foreach (var kvp in groupedByClient)
            {
                string rid = kvp.Key;
                var clientEvents = kvp.Value;
                if (clientEvents.Count <= 1) continue;

                // VR-05(a): Không trùng sự kiện chính xác (cùng ngày + cùng loại dịch vụ)
                var dateServiceGroups = new Dictionary<string, int>();
                foreach (var ev in clientEvents)
                {
                    var d = getDate(ev);
                    var st = getServiceType?.Invoke(ev) ?? 0;
                    if (d.HasValue)
                    {
                        string key = $"{d.Value:yyyy-MM-dd}_{st}";
                        dateServiceGroups[key] = dateServiceGroups.TryGetValue(key, out int count) ? count + 1 : 1;
                    }
                }
                foreach (var ds in dateServiceGroups)
                {
                    if (ds.Value > 1)
                    {
                        logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                        {
                            MADUAN = maDuAn,
                            REPORT_ID = reportId,
                            API_CODE = apiCode,
                            TABLE_NAME = tableName,
                            RECORD_ID = rid,
                            FIELD_NAME = "event_date",
                            OLD_VALUE = ds.Key,
                            NEW_VALUE = ds.Key,
                            RULE_CODE = "ERR_DUPLICATE_EVENT_SAME_DAY",
                            SEVERITY = "ERROR",
                            ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                            MESSAGE = $"Khách hàng {rid} có {ds.Value} sự kiện cùng loại dịch vụ trong cùng một ngày ({ds.Key.Split('_')[0]}). Cần kiểm tra trùng lặp.",
                            CREATED_DATE = DateTime.Now
                        });
                    }
                }

                // VR-05(a, b, c): Kiểm tra số lần thứ và đơn điệu thời gian
                var orderedEvents = new List<(int Order, DateTime Date)>();
                var seenOrders = new HashSet<int>();
                bool hasDuplicateOrder = false;

                foreach (var ev in clientEvents)
                {
                    var ord = getOrder(ev);
                    var d = getDate(ev);
                    if (ord.HasValue && d.HasValue)
                    {
                        if (!seenOrders.Add(ord.Value))
                        {
                            hasDuplicateOrder = true;
                        }
                        orderedEvents.Add((ord.Value, d.Value));
                    }
                }

                // VR-05(a): Trùng số lần thứ
                if (hasDuplicateOrder)
                {
                    logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                    {
                        MADUAN = maDuAn,
                        REPORT_ID = reportId,
                        API_CODE = apiCode,
                        TABLE_NAME = tableName,
                        RECORD_ID = rid,
                        FIELD_NAME = "visit_order",
                        OLD_VALUE = rid,
                        NEW_VALUE = null,
                        RULE_CODE = "ERR_DUPLICATE_VISIT_ORDER",
                        SEVERITY = "ERROR",
                        ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                        MESSAGE = $"Khách hàng {rid} có các buổi dịch vụ {tableName} bị trùng số lần thứ.",
                        CREATED_DATE = DateTime.Now
                    });
                }

                if (orderedEvents.Count >= 2)
                {
                    orderedEvents.Sort((a, b) => a.Order.CompareTo(b.Order));

                    // VR-05(b): Nghịch đảo thời gian (Ngày lần N+1 < Ngày lần N)
                    for (int i = 0; i < orderedEvents.Count - 1; i++)
                    {
                        if (orderedEvents[i + 1].Date < orderedEvents[i].Date)
                        {
                            logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                            {
                                MADUAN = maDuAn,
                                REPORT_ID = reportId,
                                API_CODE = apiCode,
                                TABLE_NAME = tableName,
                                RECORD_ID = rid,
                                FIELD_NAME = "event_date",
                                OLD_VALUE = orderedEvents[i + 1].Date.ToString("yyyy-MM-dd"),
                                NEW_VALUE = null,
                                RULE_CODE = "ERR_CHRONOLOGICAL_INVERSION",
                                SEVERITY = "ERROR",
                                ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                                MESSAGE = $"Nghịch đảo thời gian: Buổi lần {orderedEvents[i + 1].Order} ({orderedEvents[i + 1].Date:dd/MM/yyyy}) lại diễn ra trước buổi lần {orderedEvents[i].Order} ({orderedEvents[i].Date:dd/MM/yyyy}).",
                                CREATED_DATE = DateTime.Now
                            });
                        }
                    }

                    // VR-05(c): Nhảy cóc số lần
                    for (int i = 0; i < orderedEvents.Count - 1; i++)
                    {
                        if (orderedEvents[i + 1].Order - orderedEvents[i].Order > 1)
                        {
                            logs.Add(new BVTL_DATA_STANDARDIZATION_LOG_Entity
                            {
                                MADUAN = maDuAn,
                                REPORT_ID = reportId,
                                API_CODE = apiCode,
                                TABLE_NAME = tableName,
                                RECORD_ID = rid,
                                FIELD_NAME = "visit_order",
                                OLD_VALUE = $"{orderedEvents[i].Order} -> {orderedEvents[i + 1].Order}",
                                NEW_VALUE = null,
                                RULE_CODE = "WARN_SKIPPED_VISIT_ORDER",
                                SEVERITY = "WARNING",
                                ACTION_TAKEN = "FLAGGED_FOR_ADMIN",
                                MESSAGE = $"Khách hàng {rid} nhảy cóc số lần dịch vụ {tableName} (có Lần {orderedEvents[i].Order} và Lần {orderedEvents[i + 1].Order} nhưng thiếu các lần ở giữa).",
                                CREATED_DATE = DateTime.Now
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// VR-07(b) [WARNING]. Cảnh báo cụm khi phát hiện chuỗi liên tiếp >= 3 khách hàng của cùng 1 TCV bị Incomplete trong cùng 1 ngày.
        /// </summary>
        private void CheckClusterIncomplete<T>(
            List<T> entities,
            Func<T, string> getTcv,
            Func<T, DateTime?> getDate,
            Func<T, string> getStatus,
            string tableName,
            string apiCode,
            string reportId,
            string maDuAn,
            ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs,
            Func<T, string> getRecordId = null,
            Func<T, string> getMaNhom = null,
            CD45ValidationContext context = null)
        {
            Func<string, string, string> resolveTcvName = null;
            if (context != null)
            {
                resolveTcvName = (nhom, tcv) => context.GetTcvName(nhom, tcv);
            }
            DataCleanerHelper.CheckClusterIncomplete(entities, getTcv, getDate, getStatus, tableName, apiCode, reportId, maDuAn, ref logs, getRecordId, getMaNhom, resolveTcvName);
        }

        #endregion
        
        private void SetProp(object e, System.Reflection.PropertyInfo[] props, string name, object value) {
            var p = Array.Find(props, x => x.Name == name);
            if (p != null && value != null) p.SetValue(e, value);
        }
    }
}
