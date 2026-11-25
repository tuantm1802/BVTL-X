using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Data.Admin
{
    public class BaoCaoChiSoCD45DA : IBaoCaoChiSoCD45DA
    {
        private readonly BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<BaoCaoChiSoCD45Model> GetBaoCaoChiSo(string fromDate, string toDate, string cityCode, string maNhom, bool isCumulative, int? nam, string maKy)
        {
            var pFromDate = string.IsNullOrWhiteSpace(fromDate)
                ? new SqlParameter("@FromDate", DBNull.Value)
                : new SqlParameter("@FromDate", DateTime.ParseExact(fromDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));

            var pToDate = string.IsNullOrWhiteSpace(toDate)
                ? new SqlParameter("@ToDate", DBNull.Value)
                : new SqlParameter("@ToDate", DateTime.ParseExact(toDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));

            var pCityCode = string.IsNullOrWhiteSpace(cityCode)
                ? new SqlParameter("@CityCode", DBNull.Value)
                : new SqlParameter("@CityCode", cityCode.Trim());

            var pMaNhom = string.IsNullOrWhiteSpace(maNhom)
                ? new SqlParameter("@MaNhom", DBNull.Value)
                : new SqlParameter("@MaNhom", maNhom.Trim());

            var pIsCumulative = new SqlParameter("@IsCumulative", isCumulative);
            var pNam = nam.HasValue ? new SqlParameter("@Nam", nam.Value) : new SqlParameter("@Nam", DBNull.Value);
            var pMaKy = string.IsNullOrWhiteSpace(maKy) ? new SqlParameter("@MaKy", DBNull.Value) : new SqlParameter("@MaKy", maKy.Trim());

            return db.Database.SqlQuery<BaoCaoChiSoCD45Model>(
                "EXEC dbo.SP_CD45_GetBaoCaoChiSo @FromDate, @ToDate, @CityCode, @MaNhom, @IsCumulative, @Nam, @MaKy",
                pFromDate, pToDate, pCityCode, pMaNhom, pIsCumulative, pNam, pMaKy
            ).ToList();
        }

        public List<CD45_DrillDown_ItemModel> GetDrillDown(string chiTieuCode, string fromDate, string toDate, string cityCode, string maNhom, bool isCumulative, int? doiTuong, int? gioiTinh, int? nhomTuoi)
        {
            var pChiTieu = new SqlParameter("@ChiTieuCode", chiTieuCode ?? "");
            var pFromDate = string.IsNullOrWhiteSpace(fromDate)
                ? new SqlParameter("@FromDate", DBNull.Value)
                : new SqlParameter("@FromDate", DateTime.ParseExact(fromDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
            var pToDate = string.IsNullOrWhiteSpace(toDate)
                ? new SqlParameter("@ToDate", DBNull.Value)
                : new SqlParameter("@ToDate", DateTime.ParseExact(toDate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
            var pCityCode = string.IsNullOrWhiteSpace(cityCode)
                ? new SqlParameter("@CityCode", DBNull.Value)
                : new SqlParameter("@CityCode", cityCode.Trim());
            var pMaNhom = string.IsNullOrWhiteSpace(maNhom)
                ? new SqlParameter("@MaNhom", DBNull.Value)
                : new SqlParameter("@MaNhom", maNhom.Trim());
            var pIsCumulative = new SqlParameter("@IsCumulative", isCumulative);
            var pDoiTuong = doiTuong.HasValue ? new SqlParameter("@DoiTuong", doiTuong.Value) : new SqlParameter("@DoiTuong", DBNull.Value);
            var pGioiTinh = gioiTinh.HasValue ? new SqlParameter("@GioiTinh", gioiTinh.Value) : new SqlParameter("@GioiTinh", DBNull.Value);
            var pNhomTuoi = nhomTuoi.HasValue ? new SqlParameter("@NhomTuoi", nhomTuoi.Value) : new SqlParameter("@NhomTuoi", DBNull.Value);

            var list = db.Database.SqlQuery<CD45_DrillDown_ItemModel>(
                "EXEC dbo.SP_CD45_GetBaoCaoChiSo_DrillDown @ChiTieuCode, @FromDate, @ToDate, @CityCode, @MaNhom, @IsCumulative, @DoiTuong, @GioiTinh, @NhomTuoi",
                pChiTieu, pFromDate, pToDate, pCityCode, pMaNhom, pIsCumulative, pDoiTuong, pGioiTinh, pNhomTuoi
            ).ToList();

            EnrichDrillDown(list);
            return list;
        }

        public List<CD45_ChiSoDuAnConfigModel> GetConfig(int nam, string maKy)
        {
            string ky = NormalizeKy(maKy);
            int targetYear = nam > 2000 ? nam : DateTime.Now.Year;

            EnsureConfigRowsForPeriod(targetYear, ky);

            return db.Database.SqlQuery<CD45_ChiSoDuAnConfigModel>(
                @"SELECT ID, NAM, MA_KY, MA_THAM_SO, CHI_SO_CODE, TEN_THAM_SO, GIA_TRI, GHI_CHU, SORT_ORDER, UPDATED_AT, UPDATED_BY
                  FROM dbo.CD45_CHI_SO_DU_AN_CONFIG
                  WHERE NAM = @p0 AND MA_KY = @p1
                  ORDER BY SORT_ORDER, ID",
                targetYear, ky
            ).ToList();
        }

        public bool SaveConfig(int nam, string maKy, List<CD45_ChiSoDuAnConfigModel> items, string updatedBy, out string message)
        {
            message = string.Empty;
            if (items == null || items.Count == 0)
            {
                message = "Không có dữ liệu cấu hình để lưu.";
                return false;
            }

            string ky = NormalizeKy(maKy);
            int targetYear = nam > 2000 ? nam : DateTime.Now.Year;
            EnsureConfigRowsForPeriod(targetYear, ky);

            try
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.MA_THAM_SO)) continue;
                    int val = item.GIA_TRI ?? 0;
                    if (val < 0)
                    {
                        message = $"Giá trị của tham số '{item.MA_THAM_SO}' không được nhỏ hơn 0.";
                        return false;
                    }

                    db.Database.ExecuteSqlCommand(
                        @"UPDATE dbo.CD45_CHI_SO_DU_AN_CONFIG
                          SET GIA_TRI = @p0,
                              GHI_CHU = ISNULL(@p1, GHI_CHU),
                              UPDATED_AT = GETDATE(),
                              UPDATED_BY = @p2
                          WHERE NAM = @p3 AND MA_KY = @p4 AND MA_THAM_SO = @p5",
                        val,
                        (object)item.GHI_CHU ?? DBNull.Value,
                        updatedBy ?? "System",
                        targetYear,
                        ky,
                        item.MA_THAM_SO.Trim()
                    );
                }

                message = $"Đã lưu cấu hình Chỉ số Dự án (Năm {targetYear} - Kỳ {ky}) thành công!";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi khi lưu cấu hình: " + ex.Message;
                return false;
            }
        }

        private string NormalizeKy(string maKy)
        {
            if (string.Equals(maKy, "6T", StringComparison.OrdinalIgnoreCase) || string.Equals(maKy, "6Thang", StringComparison.OrdinalIgnoreCase))
                return "6T";
            if (string.Equals(maKy, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(maKy, "LuyKe", StringComparison.OrdinalIgnoreCase))
                return "ALL";
            return "12T";
        }

        private void EnsureConfigRowsForPeriod(int nam, string maKy)
        {
            int count = db.Database.SqlQuery<int>(
                "SELECT COUNT(1) FROM dbo.CD45_CHI_SO_DU_AN_CONFIG WHERE NAM = @p0 AND MA_KY = @p1",
                nam, maKy
            ).FirstOrDefault();

            if (count == 0)
            {
                // Copy template từ kỳ gần nhất hoặc bản seed 2026
                db.Database.ExecuteSqlCommand(
                    @"INSERT INTO dbo.CD45_CHI_SO_DU_AN_CONFIG (NAM, MA_KY, MA_THAM_SO, CHI_SO_CODE, TEN_THAM_SO, GIA_TRI, GHI_CHU, SORT_ORDER, UPDATED_AT, UPDATED_BY)
                      SELECT @p0, @p1, MA_THAM_SO, CHI_SO_CODE, TEN_THAM_SO, GIA_TRI, GHI_CHU, SORT_ORDER, GETDATE(), 'AUTO_INIT'
                      FROM dbo.CD45_CHI_SO_DU_AN_CONFIG t
                      WHERE t.NAM = (SELECT TOP 1 NAM FROM dbo.CD45_CHI_SO_DU_AN_CONFIG WHERE MA_KY = @p1 ORDER BY NAM DESC)
                        AND t.MA_KY = @p1
                        AND NOT EXISTS (
                            SELECT 1 FROM dbo.CD45_CHI_SO_DU_AN_CONFIG ex
                            WHERE ex.NAM = @p0 AND ex.MA_KY = @p1 AND ex.MA_THAM_SO = t.MA_THAM_SO
                        )",
                    nam, maKy
                );
            }
        }

        private void EnrichDrillDown(List<CD45_DrillDown_ItemModel> list)
        {
            if (list == null || list.Count == 0) return;

            var dictCity = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dictNhom = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dictTcv = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var cities = db.Database.SqlQuery<CityRow>(
                    "SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM dbo.BVTL_CITES UNION SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM dbo.BVTL_DM_TINH_MOI"
                ).ToList();
                foreach (var c in cities)
                {
                    if (!string.IsNullOrEmpty(c.Code) && !string.IsNullOrEmpty(c.Name))
                        dictCity[c.Code.Trim()] = c.Name.Trim();
                }

                var groups = db.Database.SqlQuery<NhomRow>(
                    "SELECT RTRIM(manhom_tbh) AS manhom_tbh, RTRIM(manhom_tbh_map) AS manhom_tbh_map, RTRIM(tennhom_tbh) AS tennhom_tbh FROM dbo.BVTL_NHOM_TBH WHERE maduan = 'CD45'"
                ).ToList();
                foreach (var g in groups)
                {
                    if (!string.IsNullOrEmpty(g.tennhom_tbh))
                    {
                        if (!string.IsNullOrEmpty(g.manhom_tbh)) dictNhom[g.manhom_tbh.Trim()] = g.tennhom_tbh.Trim();
                        if (!string.IsNullOrEmpty(g.manhom_tbh_map)) dictNhom[g.manhom_tbh_map.Trim()] = g.tennhom_tbh.Trim();
                    }
                }

                var tcvs = db.Database.SqlQuery<CD45_TCV_ItemModel>(
                    "SELECT ID, RTRIM(MA_NHOM) AS MA_NHOM, RTRIM(TEN_NHOM) AS TEN_NHOM, RTRIM(CITY_CODE) AS CITY_CODE, RTRIM(MA_TCV) AS MA_TCV, RTRIM(TEN_TCV) AS TEN_TCV FROM dbo.CD45_NHOM_TCV WHERE TEN_TCV IS NOT NULL AND TEN_TCV <> ''"
                ).ToList();
                foreach (var t in tcvs)
                {
                    var city = (t.CITY_CODE ?? "").Trim();
                    var nhom = (t.MA_NHOM ?? "").Trim();
                    var tcv = (t.MA_TCV ?? "").Trim();
                    if (!string.IsNullOrEmpty(tcv) && !string.IsNullOrEmpty(t.TEN_TCV))
                    {
                        if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(nhom))
                            dictTcv[$"{city}_{nhom}_{tcv}"] = t.TEN_TCV.Trim();
                        if (!string.IsNullOrEmpty(nhom) && !dictTcv.ContainsKey($"{nhom}_{tcv}"))
                            dictTcv[$"{nhom}_{tcv}"] = t.TEN_TCV.Trim();
                        if (int.TryParse(tcv, out int tcvInt))
                        {
                            if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(nhom))
                                dictTcv[$"{city}_{nhom}_{tcvInt}"] = t.TEN_TCV.Trim();
                            if (!string.IsNullOrEmpty(nhom) && !dictTcv.ContainsKey($"{nhom}_{tcvInt}"))
                                dictTcv[$"{nhom}_{tcvInt}"] = t.TEN_TCV.Trim();
                        }
                    }
                }
            }
            catch { }

            foreach (var item in list)
            {
                var city = (item.CITY_CODE ?? "").Trim();
                var nhom = (item.MA_NHOM ?? "").Trim();
                var tcv = (item.MA_TCV ?? "").Trim();

                item.TEN_TINH = (!string.IsNullOrEmpty(city) && dictCity.TryGetValue(city, out var cName)) ? cName : item.CITY_CODE;
                item.TEN_NHOM = (!string.IsNullOrEmpty(nhom) && dictNhom.TryGetValue(nhom, out var nName)) ? nName : item.MA_NHOM;

                if (string.IsNullOrEmpty(tcv))
                {
                    item.TEN_TCV = "-";
                }
                else
                {
                    string tName = null;
                    if (dictTcv.TryGetValue($"{city}_{nhom}_{tcv}", out tName) || dictTcv.TryGetValue($"{nhom}_{tcv}", out tName))
                        item.TEN_TCV = tName;
                    else if (int.TryParse(tcv, out int tcvInt) && (dictTcv.TryGetValue($"{city}_{nhom}_{tcvInt}", out tName) || dictTcv.TryGetValue($"{nhom}_{tcvInt}", out tName)))
                        item.TEN_TCV = tName;
                    else
                        item.TEN_TCV = item.MA_TCV;
                }

                // Làm giàu thông tin cơ sở y tế & chẩn đoán nếu có
                BaoCaoCD45DA.FormatKhamSKTTChiTiet(item);

                // Nếu là drill-down Cơ sở KCB (RECORD_ID là mã CSYT '1'..'9')
                if (item.DOI_TUONG_TEXT == "Cơ sở KCB" && !string.IsNullOrEmpty(item.RECORD_ID) && BaoCaoCD45DA.DictHospital.TryGetValue(item.RECORD_ID.Trim(), out var hospName))
                {
                    item.CHI_TIET = hospName + (item.CHI_TIET != null && item.CHI_TIET.Contains("(") ? " " + item.CHI_TIET.Substring(item.CHI_TIET.IndexOf("(")) : "");
                }
            }
        }

        private class CityRow
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private class NhomRow
        {
            public string manhom_tbh { get; set; }
            public string manhom_tbh_map { get; set; }
            public string tennhom_tbh { get; set; }
        }
    }
}
