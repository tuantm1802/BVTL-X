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
                
                // f1_q_a4 = doi tuong (1=PUD,2=PLHIV,3=TG,4=MSM,5=SW)
                e.DOI_TUONG = item.GetByte("f1_q_a4");

                // Mapping gioi tinh, nam sinh
                e.GIOI_TINH_TU_XD = item.GetByte("f1_q_a1");
                e.GIOI_TINH_KHAI_SINH = item.GetByte("f1_q_a2");
                e.NAM_SINH = DataCleanerHelper.CleanYearOfBirth(item.GetInt("f1_q_a3"), cleanRecordId, apiCode, "CD45_KH", reportId, maDuAn, ref logs);
                
                // Checkbox "Đối tượng khác" f1_q_a5___0...4.
                List<string> dtKhac = new List<string>();
                for (int i = 0; i <= 4; i++) {
                    if (item.GetBool($"f1_q_a5___{i}") == true) dtKhac.Add(i.ToString());
                }
                e.DOI_TUONG_KHAC = string.Join(",", dtKhac);

                e.CO_CCCD = item.GetBool("f1_q_a7");
                e.CO_THUONG_TRU = item.GetBool("f1_q_a8");
                e.CO_BHYT = item.GetBool("f1_q_a9");
                e.VO_GIA_CU_6T = item.GetBool("f1_q_a10");
                e.DANG_VO_GIA_CU = item.GetBool("f1_q_a11");
                e.BI_TAM_GIU_6T = item.GetBool("f1_q_a12");
                
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

                var e = new CD45_HOAT_DONG_Entity();
                e.RECORD_ID = cleanRecordId;
                
                int? repeatInstance = item.GetInt("redcap_repeat_instance");
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

                e.LOAI_DV = item.GetByte("f2_services");
                
                string rawDate = item.GetString("f2_date");
                e.NGAY_HOAT_DONG = DataCleanerHelper.CleanDate(rawDate, "f2_date", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);

                e.DIA_DIEM = item.GetByte("f2_location");
                e.DIA_DIEM_NGOAI = DataCleanerHelper.CleanString(item.GetString("f2_location_other"), "f2_location_other", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                
                e.SO_TAI_LIEU = item.GetInt("f2_document_delivery");
                e.DONG_Y_QST = item.GetBool("f2_join");
                e.SO_BAO_CAO_SU = item.GetInt("f2_condom_delivery");
                e.SO_CHAT_BOI_TRAN = item.GetInt("f2_gel_delivery");
                e.SO_BOM_KIM = item.GetInt("f2_syringe_delivery");
                e.GHI_CHU = DataCleanerHelper.CleanString(item.GetString("f2_note"), "f2_note", cleanRecordId, apiCode, "CD45_HOAT_DONG", reportId, maDuAn, ref logs);
                
                e.COMPLETE_STATUS = item.GetString("truyn_thng_v_sinh_hot_nhm_complete") ?? item.GetString("f2_truyn_thng_v_sinh_hot_nhm_complete");
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
