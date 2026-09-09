using System;
using System.Collections.Generic;
using Model.ModelExtend.API.CD45;

namespace Common.Common
{
    public class ConvertCD45ApiToEntity
    {
        public void ConvertF1(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, 
            ref List<CD45_KH_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_KH", reportId, maDuAn, ref logs);
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
                        ref logs, out string std, out string map, out string city);
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
            }
        }

        public void ConvertF2(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, 
            ref List<CD45_HOAT_DONG_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                // Lọc bỏ bản ghi cơ sở rỗng (chỉ xử lý các bản ghi thực sự có hoạt động F2)
                string rawDate = item.GetString("f2_date");
                byte? loaiDv = item.GetByte("f2_services");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                if (string.IsNullOrEmpty(rawDate) && !loaiDv.HasValue && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.LOAI_DV = loaiDv;
                e.NGAY_HOAT_DONG = DataCleanerHelper.CleanDate(rawDate, "f2_date", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);

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
            }
        }

        public void ConvertF3(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_QST_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_QST", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f3_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");
                byte? q1a = item.GetByte("f3_q1a");

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument) && !q1a.HasValue)
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.LY_DO_DANH_GIA_LAI = item.GetByte("f3_repeat_assessment");
                e.NGAY_SANG_LOC = DataCleanerHelper.CleanDate(rawDate, "f3_date", cleanRecordId, apiCode, "CD45_QST", reportId, maDuAn, ref logs);

                e.Q1A = q1a;
                e.Q1B = item.GetByte("f3_q1b");
                e.Q1C = item.GetByte("f3_q1c");
                e.Q1D = item.GetByte("f3_q1d");
                e.Q1E = item.GetByte("f3_q1e");
                e.Q1F = item.GetByte("f3_q1f");
                e.Q2_TU_HARM = item.GetByte("f3_q2");
                e.Q3_NGHE = item.GetByte("f3_q3");

                int? score = item.GetInt("f3_score");
                if (!score.HasValue && (e.Q1A.HasValue || e.Q1B.HasValue || e.Q1C.HasValue || e.Q1D.HasValue || e.Q1E.HasValue || e.Q1F.HasValue))
                {
                    score = (e.Q1A ?? 0) + (e.Q1B ?? 0) + (e.Q1C ?? 0) + (e.Q1D ?? 0) + (e.Q1E ?? 0) + (e.Q1F ?? 0) + (e.Q2_TU_HARM == 2 ? 2 : 0) + (e.Q3_NGHE == 2 ? 2 : 0);
                }
                e.DIEM_QST = score;

                // Tính MUC_QST theo branching logic chuẩn của REDCap
                if (score.HasValue)
                {
                    if (score.Value >= 8 || e.Q2_TU_HARM == 2)
                    {
                        e.MUC_QST = 1; // Khách hàng cần chuyển gửi gấp
                    }
                    else if (score.Value >= 6 && score.Value <= 7 && (e.Q2_TU_HARM == 0 || !e.Q2_TU_HARM.HasValue))
                    {
                        e.MUC_QST = 2; // Khách hàng cần chuyển gửi điều trị
                    }
                    else if (score.Value >= 4 && score.Value <= 5 && (e.Q2_TU_HARM == 0 || !e.Q2_TU_HARM.HasValue))
                    {
                        e.MUC_QST = 3; // Khách hàng cần hỗ trợ, theo dõi tại cộng đồng
                    }
                    else if (score.Value < 4 && (e.Q2_TU_HARM == 0 || !e.Q2_TU_HARM.HasValue))
                    {
                        e.MUC_QST = 4; // Chỉ cần theo dõi nếu KH có nhu cầu
                    }
                }

                e.COMPLETE_STATUS = item.GetString("f3_bng_hi_sng_lc_sc_kho_tm_thn_qst_complete") ?? item.GetString("bng_c_c_sng_lc_sktt_qst_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF4(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_HO_TRO_XH_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f4_time");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_HO_TRO = DataCleanerHelper.CleanDate(rawDate, "f4_time", cleanRecordId, apiCode, "CD45_HO_TRO_XH", reportId, maDuAn, ref logs);

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
                e.COMPLETE_STATUS = item.GetString("f4_phiu_h_tr_x_hi_khc_complete") ?? item.GetString("phiu_h_tr_x_hi_khc_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF5(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TUAN_THU_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f5_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                // Lọc bỏ bản ghi cơ sở rỗng
                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_HO_TRO = DataCleanerHelper.CleanDate(rawDate, "f5_date", cleanRecordId, apiCode, "CD45_TUAN_THU", reportId, maDuAn, ref logs);
                e.HINH_THUC_DIEU_TRI = item.GetByte("f5_indication");
                e.CO_KE_DON_THUOC = item.GetBool("f5_prescribe");
                e.TUAN_THU = item.GetByte("f5_adherence");

                e.COMPLETE_STATUS = item.GetString("f5_phiu_h_tr_tun_th_iu_tr_complete") ?? item.GetString("phiu_h_tr_tun_th_iu_tr_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF6(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_CHAN_DOAN_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f6_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_KHAM = DataCleanerHelper.CleanDate(rawDate, "f6_date", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.CO_SO_Y_TE = DataCleanerHelper.CleanString(item.GetString("f6_hospital"), "f6_hospital", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.BAC_SI = DataCleanerHelper.CleanString(item.GetString("f6_doctor"), "f6_doctor", cleanRecordId, apiCode, "CD45_CHAN_DOAN", reportId, maDuAn, ref logs);
                e.LAN_KHAM = item.GetByte("f6_examination");
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
                e.COMPLETE_STATUS = item.GetString("f6_thng_tin_chn_on_v_iu_tr_complete") ?? item.GetString("thng_tin_chn_on_v_iu_tr_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF7(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TU_VAN_L1_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f7_date");
                string completeStatus = item.GetString("f7_phiu_t_vn_ln_1_complete") ?? item.GetString("phiu_t_vn_t_ln_1_complete");

                // F7 là form đơn (non-repeating). Lọc bỏ bản ghi rỗng
                if (string.IsNullOrEmpty(rawDate) && (string.IsNullOrEmpty(completeStatus) || completeStatus == "0"))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_TU_VAN = DataCleanerHelper.CleanDate(rawDate, "f7_date", cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);
                
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
                e.LICH_HEN_TIEP = DataCleanerHelper.CleanDate(item.GetString("f7_q61"), "f7_q61", cleanRecordId, apiCode, "CD45_TU_VAN_L1", reportId, maDuAn, ref logs);
                e.COMPLETE_STATUS = completeStatus;
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF8(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_TU_VAN_L2_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f8_date");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    if (string.IsNullOrEmpty(e.MA_NHOM)) e.MA_NHOM = map ?? std;
                }

                e.NGAY_TU_VAN = DataCleanerHelper.CleanDate(rawDate, "f8_date", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);

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

                e.NGAY_HEN_TIEP = DataCleanerHelper.CleanDate(item.GetString("f8_q61"), "f8_q61", cleanRecordId, apiCode, "CD45_TU_VAN_L2", reportId, maDuAn, ref logs);
                e.COMPLETE_STATUS = item.GetString("f8_phiu_t_vn_t_ln_2_complete") ?? item.GetString("phiu_t_vn_t_ln_2_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }

        public void ConvertF10(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId,
            ref List<CD45_VAN_TAY_Entity> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs)
        {
            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

                string rawDate = item.GetString("f10_ngay");
                int? repeatInstance = item.RepeatInstance ?? item.GetInt("redcap_repeat_instance");
                string repeatInstrument = item.GetString("redcap_repeat_instrument");

                if (string.IsNullOrEmpty(rawDate) && !repeatInstance.HasValue && string.IsNullOrEmpty(repeatInstrument))
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
                        ref logs, out string std, out string map, out string city);
                    e.MA_NHOM = map ?? std;
                }

                e.NGAY_GHI_NHAN = DataCleanerHelper.CleanDate(rawDate, "f10_ngay", cleanRecordId, apiCode, "CD45_VAN_TAY", reportId, maDuAn, ref logs);
                e.DICH_VU = item.GetByte("f10_dichvu");
                e.VAN_DE = item.GetByte("f10_vande") ?? item.GetByte("f10_vande_2");
                e.PHUONG_AN = item.GetByte("f10_phuongan");
                e.COMPLETE_STATUS = item.GetString("f10_vn_tay_khch_hng_complete") ?? item.GetString("vn_tay_khch_hng_complete");
                e.NGAY_SYNC = DateTime.Now;

                entities.Add(e);
            }
        }
        
        public void ConvertGeneric<T>(List<DreamhBaseApiModel> apiData, string maDuAn, string apiCode, string reportId, string tableName,
            string prefix, string dateField, string completeField, ref List<T> entities, ref List<BVTL_DATA_STANDARDIZATION_LOG_Entity> logs) where T : new()
        {
            var props = typeof(T).GetProperties();

            foreach (var item in apiData)
            {
                string cleanRecordId = DataCleanerHelper.CleanRecordId(item.record_id, apiCode, tableName, reportId, maDuAn, ref logs);
                if (string.IsNullOrEmpty(cleanRecordId)) continue;

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
                        ref logs, out string std, out string map, out string city);
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
        }
        
        private void SetProp(object e, System.Reflection.PropertyInfo[] props, string name, object value) {
            var p = Array.Find(props, x => x.Name == name);
            if (p != null && value != null) p.SetValue(e, value);
        }
    }
}
