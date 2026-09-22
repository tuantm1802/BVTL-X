using Common.Common;
using Data.InterfaceDA;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.Admin
{
    public class BaoCaoCD45DA : IBaoCaoCD45DA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<BaoCaoCD45Model> GetBaoCao(string fromDate, string toDate, string cityCode, string maNhom, string maTCV, string loaiBaoCao = null)
        {
            var pFromDate = string.IsNullOrEmpty(fromDate) ? new SqlParameter("@FromDate", System.DBNull.Value) : new SqlParameter("@FromDate", System.DateTime.ParseExact(fromDate, "dd/MM/yyyy", null));
            var pToDate = string.IsNullOrEmpty(toDate) ? new SqlParameter("@ToDate", System.DBNull.Value) : new SqlParameter("@ToDate", System.DateTime.ParseExact(toDate, "dd/MM/yyyy", null));
            var pCityCode = string.IsNullOrEmpty(cityCode) ? new SqlParameter("@CityCode", System.DBNull.Value) : new SqlParameter("@CityCode", cityCode);
            var pMaNhom = string.IsNullOrEmpty(maNhom) ? new SqlParameter("@MaNhom", System.DBNull.Value) : new SqlParameter("@MaNhom", maNhom);
            var pMaTCV = string.IsNullOrEmpty(maTCV) ? new SqlParameter("@MaTCV", System.DBNull.Value) : new SqlParameter("@MaTCV", maTCV);
            var pLoaiBaoCao = string.IsNullOrEmpty(loaiBaoCao) ? new SqlParameter("@LoaiBaoCao", System.DBNull.Value) : new SqlParameter("@LoaiBaoCao", loaiBaoCao);

            var result = db.Database.SqlQuery<BaoCaoCD45Model>(
                "EXEC SP_CD45_GetBaoCao @FromDate, @ToDate, @CityCode, @MaNhom, @MaTCV, @LoaiBaoCao",
                pFromDate, pToDate, pCityCode, pMaNhom, pMaTCV, pLoaiBaoCao
            ).ToList();

            // Khi có filter theo kỳ, một số chỉ tiêu bị ẩn → STT không còn tuần tự.
            // Đánh lại STT tuần tự trong từng section, chỉ cho các chỉ tiêu cấp 0 có STT số.
            if (!string.IsNullOrEmpty(loaiBaoCao) && loaiBaoCao != "TuyChon")
                RenumberSTT(result);

            return result;
        }

        /// <summary>
        /// Đánh lại số thứ tự (STT) tuần tự trong từng section sau khi filter theo kỳ báo cáo.
        /// Chỉ áp dụng cho các dòng chỉ tiêu chính (IsBold=false, IndentLevel=0, STT là số nguyên).
        /// Sub-rows (IndentLevel=1, STT rỗng) và section headers (IsBold=true) giữ nguyên.
        /// </summary>
        private static void RenumberSTT(List<BaoCaoCD45Model> items)
        {
            int counter = 0;
            foreach (var item in items)
            {
                if (item.IsBold) // Section header: reset counter
                {
                    counter = 0;
                }
                else if (item.IndentLevel.GetValueOrDefault(0) == 0 && !string.IsNullOrEmpty(item.STT) && int.TryParse(item.STT, out _))
                {
                    // Chỉ tiêu cấp 0 có STT số → đánh lại tuần tự
                    counter++;
                    item.STT = counter.ToString();
                }
                // IndentLevel > 0 hoặc STT rỗng: giữ nguyên (sub-rows, không đánh số)
            }
        }

        public List<CD45_TCV_ItemModel> GetListTCV(string cityCode, string maNhom)
        {
            var sql = @"
                SELECT 
                    tcv.ID, 
                    RTRIM(tcv.MA_NHOM) AS MA_NHOM, 
                    tcv.TEN_NHOM, 
                    RTRIM(tcv.CITY_CODE) AS CITY_CODE, 
                    RTRIM(tcv.MA_TCV) AS MA_TCV, 
                    tcv.TEN_TCV, 
                    ISNULL(n.PREFIX, ISNULL(tcv.PREFIX, N'Nhóm')) AS PREFIX, 
                    ISNULL(n.SHORT_PREFIX, ISNULL(tcv.SHORT_PREFIX, N'Nhóm')) AS SHORT_PREFIX 
                FROM CD45_NHOM_TCV tcv
                LEFT JOIN BVTL_NHOM_TBH n ON (tcv.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND tcv.MA_NHOM = n.manhom_tbh_map))
                WHERE 1=1";
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND (tcv.CITY_CODE = '" + cityCode.Replace("'", "''") + "')";
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                sql += " AND (tcv.MA_NHOM = '" + maNhom.Replace("'", "''") + "')";
            }
            sql += " ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, TRY_CAST(tcv.MA_TCV AS INT), tcv.TEN_TCV";
            return db.Database.SqlQuery<CD45_TCV_ItemModel>(sql).ToList();
        }

        public List<CD45_DrillDown_ItemModel> GetDrillDown(string chiTieuCode, string fromDate, string toDate, string cityCode, string maNhom, string maTCV, int? doiTuong)
        {
            var pChiTieu = new SqlParameter("@ChiTieuCode", chiTieuCode ?? "");
            var pFromDate = string.IsNullOrEmpty(fromDate) ? new SqlParameter("@FromDate", System.DBNull.Value) : new SqlParameter("@FromDate", System.DateTime.ParseExact(fromDate, "dd/MM/yyyy", null));
            var pToDate = string.IsNullOrEmpty(toDate) ? new SqlParameter("@ToDate", System.DBNull.Value) : new SqlParameter("@ToDate", System.DateTime.ParseExact(toDate, "dd/MM/yyyy", null));
            var pCityCode = string.IsNullOrEmpty(cityCode) ? new SqlParameter("@CityCode", System.DBNull.Value) : new SqlParameter("@CityCode", cityCode);
            var pMaNhom = string.IsNullOrEmpty(maNhom) ? new SqlParameter("@MaNhom", System.DBNull.Value) : new SqlParameter("@MaNhom", maNhom);
            var pMaTCV = string.IsNullOrEmpty(maTCV) ? new SqlParameter("@MaTCV", System.DBNull.Value) : new SqlParameter("@MaTCV", maTCV);
            var pDoiTuong = !doiTuong.HasValue ? new SqlParameter("@DoiTuong", System.DBNull.Value) : new SqlParameter("@DoiTuong", doiTuong.Value);

            var list = db.Database.SqlQuery<CD45_DrillDown_ItemModel>(
                "EXEC SP_CD45_GetDrillDown @ChiTieuCode, @FromDate, @ToDate, @CityCode, @MaNhom, @MaTCV, @DoiTuong",
                pChiTieu, pFromDate, pToDate, pCityCode, pMaNhom, pMaTCV, pDoiTuong
            ).ToList();

            EnrichDrillDownData(list);

            return list;
        }

        public List<CD45_BcTieuCauHinhModel> GetCauHinhChiTieu()
        {
            return db.Database.SqlQuery<CD45_BcTieuCauHinhModel>(
                @"SELECT ID, ChiTieuCode, ChiTieuName, SectionCode, IsSection,
                         HienThi_Thang, HienThi_Quy, HienThi_6T, HienThi_12T,
                         Default_Thang, Default_Quy, Default_6T, Default_12T,
                         SortOrder, IsActive
                  FROM dbo.CD45_BCTIEU_CAU_HINH
                  ORDER BY SortOrder"
            ).ToList();
        }

        public bool SaveCauHinhChiTieu(List<CD45_BcTieuCauHinhModel> items, string updatedBy)
        {
            if (items == null || items.Count == 0) return false;
            var connStr = db.Database.Connection.ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                foreach (var item in items)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE dbo.CD45_BCTIEU_CAU_HINH
                            SET HienThi_Thang = @Thang,
                                HienThi_Quy   = @Quy,
                                HienThi_6T    = @6T,
                                HienThi_12T   = @12T,
                                UpdatedAt      = GETDATE(),
                                UpdatedBy      = @UpdatedBy
                            WHERE ID = @ID";
                        cmd.Parameters.AddWithValue("@ID", item.ID);
                        cmd.Parameters.AddWithValue("@Thang", item.HienThi_Thang ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Quy", item.HienThi_Quy ? 1 : 0);
                        cmd.Parameters.AddWithValue("@6T", item.HienThi_6T ? 1 : 0);
                        cmd.Parameters.AddWithValue("@12T", item.HienThi_12T ? 1 : 0);
                        cmd.Parameters.AddWithValue("@UpdatedBy", (object)updatedBy ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public bool ResetCauHinhMacDinh(string updatedBy)
        {
            var pUpdatedBy = string.IsNullOrEmpty(updatedBy) ? new SqlParameter("@UpdatedBy", DBNull.Value) : new SqlParameter("@UpdatedBy", updatedBy);
            db.Database.ExecuteSqlCommand("EXEC dbo.SP_CD45_ResetCauHinhChiTieuMacDinh @UpdatedBy", pUpdatedBy);
            return true;
        }

        private static readonly object _lockLookup = new object();
        private static Dictionary<string, string> _dictCity;
        private static Dictionary<string, string> _dictNhom;
        private static Dictionary<string, string> _dictTcv;
        private static System.DateTime _lastLookupLoaded = System.DateTime.MinValue;

        private void EnsureLookupDictionaries()
        {
            if (_dictCity != null && _dictNhom != null && _dictTcv != null && (System.DateTime.Now - _lastLookupLoaded).TotalMinutes < 15)
            {
                return;
            }

            lock (_lockLookup)
            {
                if (_dictCity != null && _dictNhom != null && _dictTcv != null && (System.DateTime.Now - _lastLookupLoaded).TotalMinutes < 15)
                {
                    return;
                }

                var dictCity = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                var dictNhom = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                var dictTcv = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);

                try
                {
                    // 1. Tỉnh/Thành
                    var cities = db.Database.SqlQuery<CityLookupRow>("SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM BVTL_CITES UNION SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM BVTL_DM_TINH_MOI").ToList();
                    foreach (var c in cities)
                    {
                        if (!string.IsNullOrEmpty(c.Code) && !string.IsNullOrEmpty(c.Name))
                        {
                            dictCity[c.Code.Trim()] = c.Name.Trim();
                        }
                    }

                    // 2. Nhóm TBH
                    var groups = db.Database.SqlQuery<NhomLookupRow>(
                        "SELECT RTRIM(manhom_tbh) AS manhom_tbh, RTRIM(manhom_tbh_map) AS manhom_tbh_map, RTRIM(tennhom_tbh) AS tennhom_tbh FROM BVTL_NHOM_TBH WHERE maduan = 'CD45'"
                    ).ToList();
                    foreach (var g in groups)
                    {
                        if (!string.IsNullOrEmpty(g.tennhom_tbh))
                        {
                            if (!string.IsNullOrEmpty(g.manhom_tbh)) dictNhom[g.manhom_tbh.Trim()] = g.tennhom_tbh.Trim();
                            if (!string.IsNullOrEmpty(g.manhom_tbh_map)) dictNhom[g.manhom_tbh_map.Trim()] = g.tennhom_tbh.Trim();
                        }
                    }

                    // 3. TCV & Nhóm từ CD45_NHOM_TCV
                    var tcvs = db.Database.SqlQuery<CD45_TCV_ItemModel>(
                        "SELECT ID, RTRIM(MA_NHOM) AS MA_NHOM, RTRIM(TEN_NHOM) AS TEN_NHOM, RTRIM(CITY_CODE) AS CITY_CODE, RTRIM(MA_TCV) AS MA_TCV, RTRIM(TEN_TCV) AS TEN_TCV FROM CD45_NHOM_TCV WHERE TEN_TCV IS NOT NULL AND TEN_TCV <> ''"
                    ).ToList();
                    foreach (var t in tcvs)
                    {
                        if (!string.IsNullOrEmpty(t.MA_NHOM) && !string.IsNullOrEmpty(t.TEN_NHOM) && !dictNhom.ContainsKey(t.MA_NHOM.Trim()))
                        {
                            dictNhom[t.MA_NHOM.Trim()] = t.TEN_NHOM.Trim();
                        }

                        if (!string.IsNullOrEmpty(t.MA_TCV) && !string.IsNullOrEmpty(t.TEN_TCV))
                        {
                            var city = (t.CITY_CODE ?? "").Trim();
                            var nhom = (t.MA_NHOM ?? "").Trim();
                            var tcv = t.MA_TCV.Trim();

                            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(nhom))
                            {
                                dictTcv[$"{city}_{nhom}_{tcv}"] = t.TEN_TCV.Trim();
                            }
                            if (!string.IsNullOrEmpty(nhom) && !dictTcv.ContainsKey($"{nhom}_{tcv}"))
                            {
                                dictTcv[$"{nhom}_{tcv}"] = t.TEN_TCV.Trim();
                            }

                            if (int.TryParse(tcv, out int tcvInt))
                            {
                                if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(nhom))
                                {
                                    dictTcv[$"{city}_{nhom}_{tcvInt}"] = t.TEN_TCV.Trim();
                                }
                                if (!string.IsNullOrEmpty(nhom) && !dictTcv.ContainsKey($"{nhom}_{tcvInt}"))
                                {
                                    dictTcv[$"{nhom}_{tcvInt}"] = t.TEN_TCV.Trim();
                                }
                            }
                        }
                    }

                    _dictCity = dictCity;
                    _dictNhom = dictNhom;
                    _dictTcv = dictTcv;
                    _lastLookupLoaded = System.DateTime.Now;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading lookup dictionaries: " + ex.Message);
                }
            }
        }

        private void EnrichDrillDownData(List<CD45_DrillDown_ItemModel> list)
        {
            if (list == null || list.Count == 0) return;

            EnsureLookupDictionaries();

            var dictCity = _dictCity;
            var dictNhom = _dictNhom;
            var dictTcv = _dictTcv;

            foreach (var item in list)
            {
                var city = (item.CITY_CODE ?? "").Trim();
                var nhom = (item.MA_NHOM ?? "").Trim();
                var tcv = (item.MA_TCV ?? "").Trim();

                // 1. Tên Tỉnh
                if (!string.IsNullOrEmpty(city) && dictCity != null && dictCity.TryGetValue(city, out var cityName))
                {
                    item.TEN_TINH = cityName;
                }
                else
                {
                    item.TEN_TINH = item.CITY_CODE;
                }

                // 2. Tên Nhóm
                if (!string.IsNullOrEmpty(nhom) && dictNhom != null && dictNhom.TryGetValue(nhom, out var nhomName))
                {
                    item.TEN_NHOM = nhomName;
                }
                else
                {
                    item.TEN_NHOM = item.MA_NHOM;
                }

                // 3. Tên TCV
                if (string.IsNullOrEmpty(tcv))
                {
                    item.TEN_TCV = "-";
                }
                else if (dictTcv != null)
                {
                    string tcvName = null;
                    string keyFull = $"{city}_{nhom}_{tcv}";
                    string keyNhom = $"{nhom}_{tcv}";

                    if (dictTcv.TryGetValue(keyFull, out tcvName) || dictTcv.TryGetValue(keyNhom, out tcvName))
                    {
                        item.TEN_TCV = tcvName;
                    }
                    else if (int.TryParse(tcv, out int tcvInt) &&
                            (dictTcv.TryGetValue($"{city}_{nhom}_{tcvInt}", out tcvName) ||
                             dictTcv.TryGetValue($"{nhom}_{tcvInt}", out tcvName)))
                    {
                        item.TEN_TCV = tcvName;
                    }
                    else
                    {
                        item.TEN_TCV = item.MA_TCV;
                    }
                }
                else
                {
                    item.TEN_TCV = item.MA_TCV;
                }

                // 4. Làm giàu Thông tin chi tiết cho Khám SKTT (Cơ sở khám & Chẩn đoán chính)
                FormatKhamSKTTChiTiet(item);
            }
        }


        private static readonly Dictionary<string, string> DictHospital = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", "Hà Nội - Bệnh viện Lão khoa" },
            { "2", "Hưng Yên - BV SKTT Thái Bình" },
            { "3", "Hưng Yên - PK Meheal" },
            { "4", "Hà Nội - Phòng khám Dr Phi" },
            { "5", "Ninh Bình - BV SKTT Ninh Bình" },
            { "6", "Bệnh viện tâm thần Nghệ An" },
            { "7", "Bệnh viện SKTT Hải Phòng" },
            { "8", "Bệnh viện tâm thần TP.HCM" },
            { "9", "Bệnh viện Thủ Đức" }
        };

﻿        private static readonly Dictionary<string, string> DictDiagnose = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", "F00- Sa sút trí tuệ (bệnh Alzheimer, sa sút trí tuệ do mạch máu, sa sút trí tuệ trong các bệnh lý khác)" },
            { "2", "F10- Rối loạn tâm thần và hành vi do sử dụng rượu" },
            { "3", "F10.0- Rối loạn tâm thần và hành vi do sử dụng rượu (Nhiễm độc cấp)" },
            { "4", "F10.1- Rối loạn tâm thần và hành vi do sử dụng rượu (Sử dụng gây hại)" },
            { "5", "F10.2- Rối loạn tâm thần và hành vi do sử dụng rượu (Hội chứng nghiện)" },
            { "6", "F10.3- Rối loạn tâm thần và hành vi do sử dụng rượu (Trạng thái cai)" },
            { "7", "F10.4- Rối loạn tâm thần và hành vi do sử dụng rượu (Trạng thái cai với mê sảng)" },
            { "8", "F10.5- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần)" },
            { "9", "F10.6- Rối loạn tâm thần và hành vi do sử dụng rượu (Hội chứng quên)" },
            { "10", "F10.7- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "11", "F10.8- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần và hành vi khác)" },
            { "12", "F10.9- Rối loạn tâm thần và hành vi do sử dụng rượu (Rối loạn tâm thần và hành vi không biệt định)" },
            { "13", "F11- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện" },
            { "14", "F11.0- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Nhiễm độc cấp)" },
            { "15", "F11.1- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Sử dụng gây hại)" },
            { "16", "F11.2- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Hội chứng nghiện)" },
            { "17", "F11.3- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Trạng thái cai)" },
            { "18", "F11.4- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Trạng thái cai với mê sảng)" },
            { "19", "F11.5- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần)" },
            { "20", "F11.6- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Hội chứng quên)" },
            { "21", "F11.7- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "22", "F11.8- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần và hành vi khác)" },
            { "23", "F11.9- Rối loạn tâm thần và hành vi do sử dụng các dạng thuốc phiện (Rối loạn tâm thần và hành vi không biệt định)" },
            { "24", "F12- Rối loạn tâm thần và hành vi do sử dụng cần sa" },
            { "25", "F12.0- Rối loạn tâm thần và hành vi do sử dụng cần sa (Nhiễm độc cấp)" },
            { "26", "F12.1- Rối loạn tâm thần và hành vi do sử dụng cần sa (Sử dụng gây hại)" },
            { "27", "F12.2- Rối loạn tâm thần và hành vi do sử dụng cần sa (Hội chứng nghiện)" },
            { "28", "F12.3- Rối loạn tâm thần và hành vi do sử dụng cần sa (Trạng thái cai)" },
            { "29", "F12.4- Rối loạn tâm thần và hành vi do sử dụng cần sa (Trạng thái cai với mê sảng)" },
            { "30", "F12.5- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần)" },
            { "31", "F12.6- Rối loạn tâm thần và hành vi do sử dụng cần sa (Hội chứng quên)" },
            { "32", "F12.7- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "33", "F12.8- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần và hành vi khác)" },
            { "34", "F12.9- Rối loạn tâm thần và hành vi do sử dụng cần sa (Rối loạn tâm thần và hành vi không biệt định)" },
            { "35", "F13- Rối loạn tâm thần và hành vi do sử dụng các chất an dịu hoặc các thuốc ngủ" },
            { "36", "F13.0- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Nhiễm độc cấp)" },
            { "37", "F13.1- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Sử dụng gây hại)" },
            { "38", "F13.2- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Hội chứng nghiện)" },
            { "39", "F13.3- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Trạng thái cai)" },
            { "40", "F13.4- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Trạng thái cai với mê sảng)" },
            { "41", "F13.5- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần)" },
            { "42", "F13.6- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Hội chứng quên)" },
            { "43", "F13.7- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "44", "F13.8- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần và hành vi khác)" },
            { "45", "F13.9- Rối loạn tâm thần và hành vi do sử dụng các chất an thần hoặc các thuốc ngủ (Rối loạn tâm thần và hành vi không biệt định)" },
            { "46", "F14- Rối loạn tâm thần và hành vi do sử dụng cocain" },
            { "47", "F14.0- Rối loạn tâm thần và hành vi do sử dụng cocain (Nhiễm độc cấp)" },
            { "48", "F14.1- Rối loạn tâm thần và hành vi do sử dụng cocain (Sử dụng gây hại)" },
            { "49", "F14.2- Rối loạn tâm thần và hành vi do sử dụng cocain (Hội chứng nghiện)" },
            { "50", "F14.3- Rối loạn tâm thần và hành vi do sử dụng cocain (Trạng thái cai)" },
            { "51", "F14.4- Rối loạn tâm thần và hành vi do sử dụng cocain (Trạng thái cai với mê sảng)" },
            { "52", "F14.5- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần)" },
            { "53", "F14.6- Rối loạn tâm thần và hành vi do sử dụng cocain (Hội chứng quên)" },
            { "54", "F14.7- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "55", "F14.8- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần và hành vi khác)" },
            { "56", "F14.9- Rối loạn tâm thần và hành vi do sử dụng cocain (Rối loạn tâm thần và hành vi không biệt định)" },
            { "57", "F15- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein" },
            { "58", "F15.0- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Nhiễm độc cấp)" },
            { "59", "F15.1- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Sử dụng gây hại)" },
            { "60", "F15.2- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Hội chứng nghiện)" },
            { "61", "F15.3- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Trạng thái cai)" },
            { "62", "F15.4- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Trạng thái cai với mê sảng)" },
            { "63", "F15.5- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần)" },
            { "64", "F15.6- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Hội chứng quên)" },
            { "65", "F15.7- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "66", "F15.8- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần và hành vi khác)" },
            { "67", "F15.9- Rối loạn tâm thần và hành vi do sử dụng chất kích thích khác, bao gồm cả caffein (Rối loạn tâm thần và hành vi không biệt định)" },
            { "68", "F16- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác" },
            { "69", "F16.0- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Nhiễm độc cấp)" },
            { "70", "F16.1- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Sử dụng gây hại)" },
            { "71", "F16.2- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Hội chứng nghiện)" },
            { "72", "F16.3- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Trạng thái cai)" },
            { "73", "F16.4- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Trạng thái cai với mê sảng)" },
            { "74", "F16.5- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần)" },
            { "75", "F16.6- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Hội chứng quên)" },
            { "76", "F16.7- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "77", "F16.8- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần và hành vi khác)" },
            { "78", "F16.9- Rối loạn tâm thần và hành vi do sử dụng các chất gây ảo giác (Rối loạn tâm thần và hành vi không biệt định)" },
            { "79", "F18- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi" },
            { "80", "F18.0- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Nhiễm độc cấp)" },
            { "81", "F18.1- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Sử dụng gây hại)" },
            { "82", "F18.2- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Hội chứng nghiện)" },
            { "83", "F18.3- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Trạng thái cai)" },
            { "84", "F18.4- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Trạng thái cai với mê sảng)" },
            { "85", "F18.5- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần)" },
            { "86", "F18.6- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Hội chứng quên)" },
            { "87", "F18.7- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "88", "F18.8- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần và hành vi khác)" },
            { "89", "F18.9- Rối loạn tâm thần và hành vi do sử dụng dung môi dễ bay hơi (Rối loạn tâm thần và hành vi không biệt định)" },
            { "90", "F19- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác" },
            { "91", "F19.0- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Nhiễm độc cấp)" },
            { "92", "F19.1- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Sử dụng gây hại)" },
            { "93", "F19.2- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Hội chứng nghiện)" },
            { "94", "F19.3- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Trạng thái cai)" },
            { "95", "F19.4- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Trạng thái cai với mê sảng)" },
            { "96", "F19.5- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần)" },
            { "97", "F19.6- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Hội chứng quên)" },
            { "98", "F19.7- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn loạn thần di chứng và khởi phát muộn)" },
            { "99", "F19.8- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần và hành vi khác)" },
            { "100", "F19.9- Rối loạn tâm thần và hành vi do sử dụng nhiều loại ma túy và chất tác động tâm thần khác (Rối loạn tâm thần và hành vi không biệt định)" },
            { "101", "F20- Tâm thần phân liệt" },
            { "102", "F21- Rối loạn loại phân liệt" },
            { "103", "F22- Rối loạn hoang tưởng dai dẳng" },
            { "104", "F23- Rối loạn loạn thần cấp và nhất thời" },
            { "105", "F24- Rối loạn hoang tưởng cảm ứng" },
            { "106", "F25- Rối loạn phân liệt cảm xúc" },
            { "107", "F29- Loạn thần không thực tổn không biệt định" },
            { "108", "F30- Giai đoạn hưng cảm" },
            { "109", "F30.0- Hưng cảm nhẹ" },
            { "110", "F30.1- Hưng cảm không có các triệu chứng loạn thần" },
            { "111", "F30.2- Hưng cảm với các triệu chứng loạn thần" },
            { "112", "F31- Rối loạn cảm xúc lưỡng cực" },
            { "113", "F32- Giai đoạn trầm cảm" },
            { "114", "F33- Rối loạn trầm cảm tái diễn" },
            { "115", "F34- Rối loạn khí sắc [cảm xúc] dai dẳng" },
            { "116", "F38- Rối loạn khí sắc [cảm xúc] khác" },
            { "117", "F39- Rối loạn khí sắc (cảm xúc) biệt định" },
            { "118", "F40- Rối loạn lo âu ám ảnh sợ hãi" },
            { "119", "F41- Các rối loạn lo âu khác" },
            { "120", "F42- Rối loạn ám ảnh nghi thức" },
            { "121", "F43- Phản ứng với stress trầm trọng và rối loạn sự thích ứng" },
            { "122", "F43.1- Rối loạn stress sau sang chấn" },
            { "123", "F44- Các rối loạn phân ly [chuyển di]" },
            { "124", "F45- Rối loạn dạng cơ thể" },
            { "125", "F50- Các rối loạn ăn uống" },
            { "126", "F51- Rối loạn giấc ngủ không thực tổn" },
            { "127", "F52- Loạn chức năng tình dục, không do rối loạn hoặc bệnh thực tổn" },
            { "128", "F60- Rối loạn nhân cách đặc hiệu" },
            { "129", "F70- Chậm phát triển tâm thần nhẹ" },
            { "130", "F90- Các rối loạn tăng động" },
            { "131", "F41.2- Rối loạn hỗn hợp lo âu và trầm cảm" },
            { "132", "Khác" },
        };


        public static void FormatKhamSKTTChiTiet(CD45_DrillDown_ItemModel item)
        {
            if (string.IsNullOrEmpty(item?.CHI_TIET)) return;

            if (!item.CHI_TIET.StartsWith("Lần khám:", StringComparison.OrdinalIgnoreCase)) return;

            // Nếu đã được làm giàu hoàn chỉnh: có "Chẩn đoán:" và phần sau "Chẩn đoán:" không phải chỉ là mã số thuần túy
            var chanDoanIdx = item.CHI_TIET.IndexOf("Chẩn đoán:", StringComparison.OrdinalIgnoreCase);
            if (chanDoanIdx >= 0)
            {
                string afterCd = item.CHI_TIET.Substring(chanDoanIdx + "Chẩn đoán:".Length).Trim();
                if (!int.TryParse(afterCd, out _))
                {
                    return;
                }
            }
            else if (DictHospital.Values.Any(v => item.CHI_TIET.Contains(v)))
            {
                // Đã chứa tên cơ sở y tế
                return;
            }

            // Nhận diện: Lần khám: {lan} - Cơ sở: {coSo} [ - [Chẩn đoán:]? {cd} ]
            var match = Regex.Match(
                item.CHI_TIET,
                @"^Lần khám:\s*(?<lan>\d+)\s*-\s*Cơ sở:\s*(?<cs>[^-]*?)(?:\s*-\s*(?:Chẩn đoán:\s*)?(?<cd>.*))?$",
                RegexOptions.IgnoreCase
            );

            if (!match.Success) return;

            string lan = match.Groups["lan"].Value.Trim();
            string cs = match.Groups["cs"].Value.Trim();
            string cd = match.Groups["cd"].Value.Trim();

            // Map tên Cơ sở
            string tenCoSo = cs;
            if (!string.IsNullOrEmpty(cs))
            {
                if (DictHospital.TryGetValue(cs, out var hName))
                {
                    tenCoSo = hName;
                }
            }
            else
            {
                tenCoSo = "-";
            }

            // Map tên Chẩn đoán chính
            string tenCd = cd;
            if (!string.IsNullOrEmpty(cd))
            {
                if (DictDiagnose.TryGetValue(cd, out var dName))
                {
                    tenCd = dName;
                }
            }

            if (!string.IsNullOrEmpty(tenCd))
            {
                item.CHI_TIET = $"Lần khám: {lan} - Cơ sở: {tenCoSo} - Chẩn đoán: {tenCd}";
            }
            else
            {
                item.CHI_TIET = $"Lần khám: {lan} - Cơ sở: {tenCoSo}";
            }
        }

        private class CityLookupRow
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private class NhomLookupRow
        {
            public string manhom_tbh { get; set; }
            public string manhom_tbh_map { get; set; }
            public string tennhom_tbh { get; set; }
        }
    }
}
