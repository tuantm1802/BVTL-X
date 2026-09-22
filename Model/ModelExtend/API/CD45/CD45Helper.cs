using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ModelExtend.API.CD45
{
    public static class CD45Helper
    {
        // Dictionary quy chuẩn: Key là DAG raw hoặc Label thường -> (manhom_tbh, manhom_tbh_map, city_code)
        private static readonly Dictionary<string, (string maNhomStd, string maNhomMap, string cityCode)> DagMapping 
            = new Dictionary<string, (string, string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            // Hà Nội (HNO)
            { "v_nh", ("HN_VN", "vn", "HNO") },
            { "về nhà", ("HN_VN", "vn", "HNO") },
            { "ve nha", ("HN_VN", "vn", "HNO") },
            { "its_t_time", ("HN_ITT", "itt", "HNO") },
            { "its t time", ("HN_ITT", "itt", "HNO") },
            { "it's t time", ("HN_ITT", "itt", "HNO") },
            { "the_times", ("HN_TT", "tt", "HNO") },
            { "the times", ("HN_TT", "tt", "HNO") },

            // Hải Phòng (HPG)
            { "bnh_minh", ("HP_BM", "bm", "HPG") },
            { "bình minh", ("HP_BM", "bm", "HPG") },
            { "binh minh", ("HP_BM", "bm", "HPG") },
            { "ct_trng", ("HP_CT", "ct", "HPG") },
            { "cát trắng", ("HP_CT", "ct", "HPG") },
            { "cat trang", ("HP_CT", "ct", "HPG") },
            { "hi_ng", ("HP_HD", "hd", "HPG") },
            { "hải đăng", ("HP_HD", "hd", "HPG") },
            { "hai dang", ("HP_HD", "hd", "HPG") },
            { "hoa_sen", ("HP_HS", "hs", "HPG") },
            { "hoa sen", ("HP_HS", "hs", "HPG") },
            { "hoa_trinh_n", ("HP_HT", "htn", "HPG") },
            { "hoa trinh nữ", ("HP_HT", "htn", "HPG") },
            { "hoa trinh nu", ("HP_HT", "htn", "HPG") },
            { "vng_tay_b_bn", ("HP_VT", "vtbb", "HPG") },
            { "vòng tay bè bạn", ("HP_VT", "vtbb", "HPG") },
            { "vong tay be ban", ("HP_VT", "vtbb", "HPG") },

            // Hưng Yên (HYE)
            { "lin_minh_hnh_phc", ("HY_LM", "lmhp", "HYE") },
            { "liên minh hạnh phúc", ("HY_LM", "lmhp", "HYE") },
            { "lien minh hanh phuc", ("HY_LM", "lmhp", "HYE") },

            // Nghệ An (NAN)
            { "nh_dng", ("NA_AD", "ad", "NAN") },
            { "anh_duong", ("NA_AD", "ad", "NAN") },
            { "ánh dương", ("NA_AD", "ad", "NAN") },
            { "anh duong", ("NA_AD", "ad", "NAN") },
            { "hoa_nng", ("NA_HN", "hn", "NAN") },
            { "hoa nắng", ("NA_HN", "hn", "NAN") },
            { "hoa nang", ("NA_HN", "hn", "NAN") },
            { "lin_minh_msm", ("NA_MM", "lmm", "NAN") },
            { "liên minh msm", ("NA_MM", "lmm", "NAN") },
            { "lien minh msm", ("NA_MM", "lmm", "NAN") },
            { "qunh_hng_xanh", ("NA_QH", "qhx", "NAN") },
            { "quỳnh hương xanh", ("NA_QH", "qhx", "NAN") },
            { "quynh huong xanh", ("NA_QH", "qhx", "NAN") },
            { "sao_va", ("NA_SV", "sv", "NAN") },
            { "sao va", ("NA_SV", "sv", "NAN") },

            // Ninh Bình (NBI)
            { "gi_mi", ("NB_GM", "gm", "NBI") },
            { "gió mới", ("NB_GM", "gm", "NBI") },
            { "gio moi", ("NB_GM", "gm", "NBI") },
            { "hi_vng", ("NB_HV", "hv", "NBI") },
            { "hi vọng", ("NB_HV", "hv", "NBI") },
            { "hy vọng 35", ("NB_HV", "hv", "NBI") },
            { "hy vong", ("NB_HV", "hv", "NBI") },
            { "trng_khuyt", ("NB_TK", "tk", "NBI") },
            { "trăng khuyết", ("NB_TK", "tk", "NBI") },
            { "trang khuyet 35", ("NB_TK", "tk", "NBI") },
            { "trang khuyet", ("NB_TK", "tk", "NBI") },

            // TP. Hồ Chí Minh (HCM)
            { "alocare", ("HC_ALO", "alo", "HCM") },
            { "g3vn", ("HC_G3V", "g3vn", "HCM") },
            { "myhands", ("HC_MYH", "myh", "HCM") },
            { "the_gate", ("HC_TGA", "tg", "HCM") },
            { "the gate", ("HC_TGA", "tg", "HCM") }
        };

        // 1. Trich xuat ma tinh tu record_id (vd DHN01 -> HN -> HNO)
        public static string ExtractCityCode(string recordId)
        {
            if (string.IsNullOrEmpty(recordId) || recordId.Length < 3) return null;
            string prefix = recordId.Substring(1, 2).ToUpper();
            
            switch (prefix)
            {
                case "HC": return "HCM";
                case "HN": return "HNO";
                case "HP": return "HPG";
                case "HY": return "HYE";
                case "NA": return "NAN";
                case "NB": return "NBI";
                case "NT": return "NT";
                default: return prefix;
            }
        }

        // 2. Chuan hoa redcap_data_access_group
        public static bool NormalizeDagToGroupCode(string dag, out string maNhomStd, out string maNhomMap, out string cityCode)
        {
            maNhomStd = null;
            maNhomMap = null;
            cityCode = null;

            if (string.IsNullOrWhiteSpace(dag)) return false;

            string key = dag.Trim();
            if (DagMapping.TryGetValue(key, out var mapping))
            {
                maNhomStd = mapping.maNhomStd;
                maNhomMap = mapping.maNhomMap;
                cityCode = mapping.cityCode;
                return true;
            }

            return false;
        }

        // 3. Trich xuat Nhóm và TCV tu JSON
        public static (string maNhom, string maTcv) ExtractNhomTCV(DreamhBaseApiModel model, string formPrefix)
        {
            string detectedGroup = null;
            string detectedTcv = null;

            // Uu tien 1: Lay ma nhom tu redcap_data_access_group neu co
            if (!string.IsNullOrEmpty(model.redcap_data_access_group))
            {
                if (NormalizeDagToGroupCode(model.redcap_data_access_group, out string std, out string map, out string city))
                {
                    detectedGroup = map; // Luu ma ngan 'vn', 'lmhp' de match CD45_NHOM_TCV
                }
            }

            if (model.AdditionalData != null)
            {
                // Tim ma TCV tu "{formPrefix}_person_"
                string searchPrefix = formPrefix + "_person_";
                foreach (var kvp in model.AdditionalData)
                {
                    if (kvp.Key.StartsWith(searchPrefix))
                    {
                        string val = kvp.Value?.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (string.IsNullOrEmpty(detectedGroup))
                            {
                                detectedGroup = kvp.Key.Substring(searchPrefix.Length);
                            }
                            detectedTcv = val;
                            break;
                        }
                    }
                }

                // Fallback F4, F5, F6
                if (string.IsNullOrEmpty(detectedTcv))
                {
                    string searchPrefix2 = formPrefix + "_tcv_";
                    string searchPrefix3 = formPrefix + "_refer_";

                    foreach (var kvp in model.AdditionalData)
                    {
                        if (kvp.Key.StartsWith(searchPrefix2))
                        {
                            string val = kvp.Value?.ToString();
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (string.IsNullOrEmpty(detectedGroup)) detectedGroup = kvp.Key.Substring(searchPrefix2.Length);
                                detectedTcv = val;
                                break;
                            }
                        }
                        if (kvp.Key.StartsWith(searchPrefix3))
                        {
                            string val = kvp.Value?.ToString();
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (string.IsNullOrEmpty(detectedGroup)) detectedGroup = kvp.Key.Substring(searchPrefix3.Length);
                                detectedTcv = val;
                                break;
                            }
                        }
                    }
                }
            }

            return (detectedGroup, detectedTcv);
        }

        // 4. Dictionary quy hoạch mã số nhóm theo tài liệu chuẩn -> (Mã nhóm map, Mã tỉnh)
        private static readonly Dictionary<string, (string maNhomMap, string cityCode)> GroupNumberMapping
            = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            // Hà Nội (HNO)
            { "01", ("tt", "HNO") },   // The Times
            { "02", ("vn", "HNO") },   // Về Nhà
            { "04", ("itt", "HNO") },  // It's T Time

            // Hải Phòng (HPG)
            { "09", ("hd", "HPG") },   // Hải Đăng
            { "10", ("vtbb", "HPG") }, // Vòng Tay Bè Bạn
            { "11", ("hs", "HPG") },   // Hoa Sen
            { "12", ("ct", "HPG") },   // Cát Trắng
            { "13", ("bm", "HPG") },   // Bình Minh
            { "14", ("htn", "HPG") },  // Hoa Trinh Nữ

            // Ninh Bình (NBI)
            { "15", ("gm", "NBI") },   // Gió Mới
            { "16", ("tk", "NBI") },   // Trăng Khuyết 35
            { "17", ("hv", "NBI") },   // Hy Vọng 35

            // Nghệ An (NAN)
            { "18", ("sv", "NAN") },   // Sao Va
            { "19", ("ad", "NAN") },   // Ánh Dương
            { "20", ("lmm", "NAN") },  // Liên Minh MSM
            { "21", ("qhx", "NAN") },  // Quỳnh Hương Xanh
            { "22", ("hn", "NAN") },   // Hoa Nắng

            // Hưng Yên (HYE)
            { "23", ("lmhp", "HYE") }, // Liên Minh Hạnh Phúc

            // TP. Hồ Chí Minh (HCM)
            { "05", ("myh", "HCM") },  // Myhands
            { "06", ("tg", "HCM") },   // The Gate
            { "07", ("alo", "HCM") },  // Alocare
            { "08", ("g3vn", "HCM") }  // G3VN
        };

        /// <summary>
        /// Suy luận Nhóm CBO và Tỉnh/Thành phố từ DAG, Record ID hoặc Ngữ cảnh kiểm thực
        /// </summary>
        public static bool InferGroupAndCity(string recordId, string rawDag, out string maNhom, out string cityCode)
        {
            maNhom = null;
            cityCode = null;

            // 1. Ưu tiên 1: redcap_data_access_group (DAG)
            if (!string.IsNullOrWhiteSpace(rawDag))
            {
                if (NormalizeDagToGroupCode(rawDag.Trim(), out _, out string map, out string c))
                {
                    maNhom = map;
                    cityCode = c;
                    return true;
                }
            }

            if (string.IsNullOrWhiteSpace(recordId)) return false;
            string clean = recordId.Trim().ToUpper();

            // 2. Tra cứu theo quy hoạch số nhóm (2 chữ số sau mã tỉnh hoặc ở đầu chuỗi)
            // Trường hợp chuẩn: D + 2 chữ cái tỉnh + 2 chữ số nhóm (VD: DHN020001 -> 02 -> vn / HNO)
            if (clean.Length >= 5 && clean.StartsWith("D") && char.IsLetter(clean[1]) && char.IsLetter(clean[2]) && char.IsDigit(clean[3]) && char.IsDigit(clean[4]))
            {
                string groupNum = clean.Substring(3, 2);
                if (GroupNumberMapping.TryGetValue(groupNum, out var gMap))
                {
                    maNhom = gMap.maNhomMap;
                    cityCode = (clean.Substring(1, 2) == "NT") ? "NT" : gMap.cityCode;
                    return true;
                }
                cityCode = ExtractCityCode(clean);
                return true;
            }

            // Trường hợp gõ sai thiếu tiền tố D (VD: 151117... -> 15 -> gm / NBI, 21251... -> 21 -> qhx / NAN)
            if (clean.Length >= 2 && char.IsDigit(clean[0]) && char.IsDigit(clean[1]))
            {
                string groupNum = clean.Substring(0, 2);
                if (GroupNumberMapping.TryGetValue(groupNum, out var gMap))
                {
                    maNhom = gMap.maNhomMap;
                    cityCode = gMap.cityCode;
                    return true;
                }
            }

            // Trường hợp gõ thiếu số (VD: DHP10099 -> length 8 -> D + HP + 10)
            if (clean.StartsWith("D") && clean.Length >= 5 && char.IsLetter(clean[1]) && char.IsLetter(clean[2]) && char.IsDigit(clean[3]) && char.IsDigit(clean[4]))
            {
                string groupNum = clean.Substring(3, 2);
                if (GroupNumberMapping.TryGetValue(groupNum, out var gMap))
                {
                    maNhom = gMap.maNhomMap;
                    cityCode = gMap.cityCode;
                    return true;
                }
            }

            // Trường hợp có khoảng trắng "D NA..."
            if (clean.StartsWith("D NA") || clean == "DNA")
            {
                cityCode = "NAN";
                if (clean.Length >= 6 && char.IsDigit(clean[4]) && char.IsDigit(clean[5]))
                {
                    string groupNum = clean.Substring(4, 2);
                    if (GroupNumberMapping.TryGetValue(groupNum, out var gMap))
                    {
                        maNhom = gMap.maNhomMap;
                    }
                }
                return true;
            }

            cityCode = ExtractCityCode(clean);
            return !string.IsNullOrEmpty(cityCode);
        }
    }
}
