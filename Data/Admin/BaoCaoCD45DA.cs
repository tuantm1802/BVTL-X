using Common.Common;
using Data.InterfaceDA;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

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
            var sql = "SELECT ID, RTRIM(MA_NHOM) AS MA_NHOM, TEN_NHOM, RTRIM(CITY_CODE) AS CITY_CODE, RTRIM(MA_TCV) AS MA_TCV, TEN_TCV FROM CD45_NHOM_TCV WHERE 1=1";
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND (CITY_CODE = '" + cityCode.Replace("'", "''") + "')";
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                sql += " AND (MA_NHOM = '" + maNhom.Replace("'", "''") + "')";
            }
            sql += " ORDER BY CITY_CODE, MA_NHOM, TRY_CAST(MA_TCV AS INT), TEN_TCV";
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
                    var cities = db.Database.SqlQuery<CityLookupRow>("SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM BVTL_CITES").ToList();
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
