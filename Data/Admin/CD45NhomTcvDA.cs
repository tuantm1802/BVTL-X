using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class CD45NhomTcvDA : ICD45NhomTcvDA
    {
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<CD45_NhomTcvViewModel> GetListNhomTcv(string cityCode, string maNhom, string keyword)
        {
            try
            {
                db.Database.ExecuteSqlCommand("EXEC dbo.SP_CD45_AutoProvisionNhom");
            }
            catch { }

            var sql = @"
                WITH AllNhomTcv AS (
                    SELECT 
                        tcv.ID,
                        RTRIM(tcv.MA_NHOM) AS MA_NHOM,
                        tcv.TEN_NHOM,
                        RTRIM(tcv.CITY_CODE) AS CITY_CODE,
                        ISNULL(c.Name, tcv.CITY_CODE) AS CityName,
                        RTRIM(tcv.MA_TCV) AS MA_TCV,
                        tcv.TEN_TCV,
                        tcv.MADUAN,
                        tcv.IsActive,
                        ISNULL(tcv.CreatedDate, GETDATE()) AS CreatedDate,
                        ISNULL(n.PREFIX, ISNULL(tcv.PREFIX, N'Nhóm')) AS PREFIX,
                        ISNULL(n.SHORT_PREFIX, ISNULL(tcv.SHORT_PREFIX, N'Nhóm')) AS SHORT_PREFIX,
                        ISNULL(n.CHUC_DANH, ISNULL(tcv.CHUC_DANH, N'Trưởng nhóm')) AS CHUC_DANH,
                        ISNULL(n.manhom_tbh, tcv.MA_NHOM) AS MANHOM_TBH,
                        ISNULL(n.manhom_tbh_map, tcv.MA_NHOM) AS MANHOM_MAP
                    FROM CD45_NHOM_TCV tcv
                    LEFT JOIN BVTL_CITES c ON tcv.CITY_CODE = c.Code
                    LEFT JOIN BVTL_NHOM_TBH n ON (n.maduan = 'CD45' AND (tcv.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND tcv.MA_NHOM = n.manhom_tbh_map)))
                    WHERE tcv.MADUAN = 'CD45'

                    UNION ALL

                    SELECT 
                        -CAST(ROW_NUMBER() OVER (ORDER BY n.manhom_tbh) AS INT) AS ID,
                        RTRIM(n.manhom_tbh) AS MA_NHOM,
                        n.tennhom_tbh AS TEN_NHOM,
                        RTRIM(n.city_code) AS CITY_CODE,
                        ISNULL(c.Name, n.city_code) AS CityName,
                        CAST(NULL AS VARCHAR(50)) AS MA_TCV,
                        CAST(NULL AS NVARCHAR(150)) AS TEN_TCV,
                        ISNULL(n.maduan, 'CD45') AS MADUAN,
                        CAST(0 AS BIT) AS IsActive,
                        CAST(GETDATE() AS DATETIME) AS CreatedDate,
                        ISNULL(n.PREFIX, N'Nhóm') AS PREFIX,
                        ISNULL(n.SHORT_PREFIX, N'Nhóm') AS SHORT_PREFIX,
                        ISNULL(n.CHUC_DANH, N'Trưởng nhóm') AS CHUC_DANH,
                        RTRIM(n.manhom_tbh) AS MANHOM_TBH,
                        ISNULL(n.manhom_tbh_map, RTRIM(n.manhom_tbh)) AS MANHOM_MAP
                    FROM BVTL_NHOM_TBH n
                    LEFT JOIN BVTL_CITES c ON n.city_code = c.Code
                    WHERE (n.maduan = 'CD45' OR n.maduan IS NULL)
                      AND (n.maduan IS NULL OR n.maduan NOT IN ('CH07'))
                      AND NOT EXISTS (
                          SELECT 1 FROM CD45_NHOM_TCV t 
                          WHERE t.MADUAN = 'CD45' 
                            AND (t.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND t.MA_NHOM = n.manhom_tbh_map))
                      )
                )
                SELECT 
                    CAST(ROW_NUMBER() OVER (ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, CASE WHEN tcv.MA_TCV IS NULL THEN 1 ELSE 0 END, TRY_CAST(tcv.MA_TCV AS INT), tcv.TEN_TCV) AS INT) AS RowIndex,
                    tcv.* 
                FROM AllNhomTcv tcv 
                WHERE 1=1";

            var pList = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(cityCode))
            {
                var cities = cityCode.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(c => c.Trim())
                                     .Where(c => !string.IsNullOrEmpty(c))
                                     .ToList();
                if (cities.Count == 1)
                {
                    sql += " AND tcv.CITY_CODE = @CityCode";
                    pList.Add(new SqlParameter("@CityCode", cities[0]));
                }
                else if (cities.Count > 1)
                {
                    var paramNames = new List<string>();
                    for (int i = 0; i < cities.Count; i++)
                    {
                        string pName = "@CityCode" + i;
                        paramNames.Add(pName);
                        pList.Add(new SqlParameter(pName, cities[i]));
                    }
                    sql += $" AND tcv.CITY_CODE IN ({string.Join(", ", paramNames)})";
                }
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                sql += " AND (tcv.MA_NHOM = @MaNhom OR tcv.MANHOM_TBH = @MaNhom OR tcv.MANHOM_MAP = @MaNhom)";
                pList.Add(new SqlParameter("@MaNhom", maNhom.Trim()));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (tcv.TEN_TCV LIKE @Keyword OR tcv.MA_TCV LIKE @Keyword OR tcv.TEN_NHOM LIKE @Keyword OR tcv.PREFIX LIKE @Keyword)";
                pList.Add(new SqlParameter("@Keyword", "%" + keyword.Trim() + "%"));
            }
            sql += " ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, CASE WHEN tcv.MA_TCV IS NULL THEN 1 ELSE 0 END, TRY_CAST(tcv.MA_TCV AS INT), tcv.TEN_TCV";

            return db.Database.SqlQuery<CD45_NhomTcvViewModel>(sql, pList.ToArray()).ToList();
        }

        public CD45_NhomTcvKpiModel GetKpiStats()
        {
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45') AS TongTCV,
                    (SELECT COUNT(DISTINCT manhom_tbh) FROM BVTL_NHOM_TBH WHERE (maduan = 'CD45' OR maduan IS NULL) AND (maduan IS NULL OR maduan NOT IN ('CH07'))) AS TongNhom,
                    (SELECT COUNT(DISTINCT city_code) FROM BVTL_NHOM_TBH WHERE (maduan = 'CD45' OR maduan IS NULL) AND (maduan IS NULL OR maduan NOT IN ('CH07'))) AS TongTinh,
                    (SELECT ISNULL(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END), 0) FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45') AS TcvActive";

            return db.Database.SqlQuery<CD45_NhomTcvKpiModel>(sql).FirstOrDefault() ?? new CD45_NhomTcvKpiModel();
        }

        public List<CD45_NhomTcvViewModel> GetAllForExport(string cityCode, string maNhom)
        {
            return GetListNhomTcv(cityCode, maNhom, null);
        }

        public CD45_NhomTcvViewModel GetTcvById(int id)
        {
            if (id <= 0) return null;
            var sql = @"
                SELECT 
                    tcv.ID,
                    RTRIM(tcv.MA_NHOM) AS MA_NHOM,
                    tcv.TEN_NHOM,
                    RTRIM(tcv.CITY_CODE) AS CITY_CODE,
                    ISNULL(c.Name, tcv.CITY_CODE) AS CityName,
                    RTRIM(tcv.MA_TCV) AS MA_TCV,
                    tcv.TEN_TCV,
                    tcv.MADUAN,
                    tcv.IsActive,
                    ISNULL(tcv.CreatedDate, GETDATE()) AS CreatedDate,
                    ISNULL(n.PREFIX, ISNULL(tcv.PREFIX, N'Nhóm')) AS PREFIX,
                    ISNULL(n.SHORT_PREFIX, ISNULL(tcv.SHORT_PREFIX, N'Nhóm')) AS SHORT_PREFIX,
                    ISNULL(n.CHUC_DANH, ISNULL(tcv.CHUC_DANH, N'Trưởng nhóm')) AS CHUC_DANH,
                    ISNULL(n.manhom_tbh, tcv.MA_NHOM) AS MANHOM_TBH,
                    ISNULL(n.manhom_tbh_map, tcv.MA_NHOM) AS MANHOM_MAP,
                    1 AS RowIndex
                FROM CD45_NHOM_TCV tcv
                LEFT JOIN BVTL_CITES c ON tcv.CITY_CODE = c.Code
                LEFT JOIN BVTL_NHOM_TBH n ON (n.maduan = 'CD45' AND (tcv.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND tcv.MA_NHOM = n.manhom_tbh_map)))
                WHERE tcv.ID = @p0 AND tcv.MADUAN = 'CD45'";
            return db.Database.SqlQuery<CD45_NhomTcvViewModel>(sql, id).FirstOrDefault();
        }

        public string GetNextTcvCode(string maNhom)
        {
            if (string.IsNullOrWhiteSpace(maNhom)) return "1";
            var cleanNhom = maNhom.Trim();
            var sql = @"
                SELECT ISNULL(MAX(TRY_CAST(MA_TCV AS INT)), 0) + 1
                FROM CD45_NHOM_TCV
                WHERE MADUAN = 'CD45' AND (MA_NHOM = @p0 OR MA_NHOM IN (
                    SELECT manhom_tbh_map FROM BVTL_NHOM_TBH WHERE manhom_tbh = @p0
                    UNION
                    SELECT manhom_tbh FROM BVTL_NHOM_TBH WHERE manhom_tbh_map = @p0
                ))";
            var nextVal = db.Database.SqlQuery<int>(sql, cleanNhom).FirstOrDefault();
            return (nextVal > 0 ? nextVal : 1).ToString();
        }

        public bool SaveTcv(CD45_TcvSaveModel model, string username, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                message = "Dữ liệu không hợp lệ.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(model.MA_NHOM))
            {
                message = "Vui lòng chọn Nhóm CBO.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(model.MA_TCV))
            {
                message = "Vui lòng nhập Mã TCV.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(model.TEN_TCV))
            {
                message = "Vui lòng nhập Họ và Tên TCV.";
                return false;
            }

            string maNhom = model.MA_NHOM.Trim();
            string maTcv = model.MA_TCV.Trim();
            if (int.TryParse(maTcv, out int numCode) && numCode > 0)
            {
                maTcv = numCode.ToString();
            }

            string tenTcv = model.TEN_TCV.Trim();

            // Lấy thông tin nhóm để điền tự động Tỉnh/Thành và tiền tố xưng danh
            var sqlNhom = @"
                SELECT TOP 1 
                    ISNULL(manhom_tbh_map, manhom_tbh) AS MA_NHOM,
                    tennhom_tbh AS TEN_NHOM,
                    city_code AS CITY_CODE,
                    ISNULL(PREFIX, N'Nhóm') AS PREFIX,
                    ISNULL(SHORT_PREFIX, N'Nhóm') AS SHORT_PREFIX,
                    ISNULL(CHUC_DANH, N'Trưởng nhóm') AS CHUC_DANH
                FROM BVTL_NHOM_TBH 
                WHERE (manhom_tbh = @p0 OR manhom_tbh_map = @p0) 
                  AND (maduan = 'CD45' OR maduan IS NULL)";
            var nhomInfo = db.Database.SqlQuery<CD45_NhomTcvViewModel>(sqlNhom, maNhom).FirstOrDefault();

            string finalMaNhom = nhomInfo?.MA_NHOM ?? maNhom;
            string finalTenNhom = !string.IsNullOrWhiteSpace(model.TEN_NHOM) ? model.TEN_NHOM.Trim() : (nhomInfo?.TEN_NHOM ?? maNhom);
            string finalCityCode = !string.IsNullOrWhiteSpace(model.CITY_CODE) ? model.CITY_CODE.Trim() : (nhomInfo?.CITY_CODE ?? "HNO");
            string prefix = nhomInfo?.PREFIX ?? "Nhóm";
            string shortPrefix = nhomInfo?.SHORT_PREFIX ?? "Nhóm";
            string chucDanh = nhomInfo?.CHUC_DANH ?? "Trưởng nhóm";

            try
            {
                if (model.ID > 0)
                {
                    // Chế độ Cập nhật: Kiểm tra trùng mã trong cùng nhóm
                    var checkDupSql = @"
                        SELECT COUNT(1) 
                        FROM CD45_NHOM_TCV 
                        WHERE MADUAN = 'CD45' AND MA_NHOM = @p0 AND MA_TCV = @p1 AND ID <> @p2";
                    int dupCount = db.Database.SqlQuery<int>(checkDupSql, finalMaNhom, maTcv, model.ID).FirstOrDefault();
                    if (dupCount > 0)
                    {
                        message = $"Mã TCV '{maTcv}' đã tồn tại trong nhóm '{finalTenNhom}'. Vui lòng chọn mã khác!";
                        return false;
                    }

                    var updateSql = @"
                        UPDATE CD45_NHOM_TCV 
                        SET MA_NHOM = @p0,
                            TEN_NHOM = @p1,
                            CITY_CODE = @p2,
                            MA_TCV = @p3,
                            TEN_TCV = @p4,
                            IsActive = @p5,
                            CreatedDate = GETDATE(),
                            PREFIX = @p6,
                            SHORT_PREFIX = @p7,
                            CHUC_DANH = @p8
                        WHERE ID = @p9 AND MADUAN = 'CD45'";
                    int affected = db.Database.ExecuteSqlCommand(updateSql, finalMaNhom, finalTenNhom, finalCityCode, maTcv, tenTcv, model.IsActive, prefix, shortPrefix, chucDanh, model.ID);
                    if (affected > 0)
                    {
                        LogAction("CD45_NHOM_TCV", username, $"Cập nhật TCV ID={model.ID}: Nhóm={finalMaNhom}, Mã={maTcv}, Tên={tenTcv}, Active={model.IsActive}");
                        message = $"Cập nhật thông tin TCV '{tenTcv}' (Mã {maTcv}) thành công!";
                        return true;
                    }
                    else
                    {
                        message = "Không tìm thấy TCV cần cập nhật.";
                        return false;
                    }
                }
                else
                {
                    // Chế độ Thêm mới: Kiểm tra trùng mã trong cùng nhóm
                    var checkDupSql = @"
                        SELECT COUNT(1) 
                        FROM CD45_NHOM_TCV 
                        WHERE MADUAN = 'CD45' AND MA_NHOM = @p0 AND MA_TCV = @p1";
                    int dupCount = db.Database.SqlQuery<int>(checkDupSql, finalMaNhom, maTcv).FirstOrDefault();
                    if (dupCount > 0)
                    {
                        message = $"Mã TCV '{maTcv}' đã tồn tại trong nhóm '{finalTenNhom}'. Vui lòng chọn mã khác!";
                        return false;
                    }

                    var insertSql = @"
                        INSERT INTO CD45_NHOM_TCV (MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV, MADUAN, IsActive, CreatedDate, PREFIX, SHORT_PREFIX, CHUC_DANH)
                        VALUES (@p0, @p1, @p2, @p3, @p4, 'CD45', @p5, GETDATE(), @p6, @p7, @p8)";
                    db.Database.ExecuteSqlCommand(insertSql, finalMaNhom, finalTenNhom, finalCityCode, maTcv, tenTcv, model.IsActive, prefix, shortPrefix, chucDanh);

                    LogAction("CD45_NHOM_TCV", username, $"Thêm mới TCV: Nhóm={finalMaNhom}, Mã={maTcv}, Tên={tenTcv}, Active={model.IsActive}");
                    message = $"Thêm mới TCV '{tenTcv}' (Mã {maTcv}) vào nhóm '{finalTenNhom}' thành công!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Lỗi khi lưu dữ liệu TCV: " + ex.Message;
                return false;
            }
        }

        public bool ToggleStatus(int id, string username, out string message)
        {
            message = string.Empty;
            if (id <= 0)
            {
                message = "ID không hợp lệ.";
                return false;
            }
            try
            {
                var item = db.Database.SqlQuery<CD45_NhomTcvViewModel>("SELECT * FROM CD45_NHOM_TCV WHERE ID = @p0 AND MADUAN = 'CD45'", id).FirstOrDefault();
                if (item == null)
                {
                    message = "Không tìm thấy thông tin TCV.";
                    return false;
                }

                bool newStatus = !item.IsActive;
                db.Database.ExecuteSqlCommand("UPDATE CD45_NHOM_TCV SET IsActive = @p0, CreatedDate = GETDATE() WHERE ID = @p1 AND MADUAN = 'CD45'", newStatus, id);

                string statusText = newStatus ? "Đang hoạt động" : "Ngừng hoạt động";
                LogAction("CD45_NHOM_TCV", username, $"Đổi trạng thái TCV ID={id} ({item.TEN_TCV}) sang '{statusText}'");
                message = $"Đã chuyển trạng thái TCV '{item.TEN_TCV}' sang '{statusText}'!";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi khi đổi trạng thái TCV: " + ex.Message;
                return false;
            }
        }

        public bool DeleteTcv(int id, string username, out string message)
        {
            message = string.Empty;
            if (id <= 0)
            {
                message = "ID không hợp lệ.";
                return false;
            }
            try
            {
                var item = db.Database.SqlQuery<CD45_NhomTcvViewModel>("SELECT * FROM CD45_NHOM_TCV WHERE ID = @p0 AND MADUAN = 'CD45'", id).FirstOrDefault();
                if (item == null)
                {
                    message = "Không tìm thấy thông tin TCV.";
                    return false;
                }

                // Kiểm tra xem TCV đã có dữ liệu hoạt động/tư vấn trong hệ thống hay chưa
                var countSql = @"
                    SELECT 
                        (SELECT COUNT(1) FROM CD45_HOAT_DONG WHERE MA_NHOM = @p0 AND MA_TCV = @p1) +
                        (SELECT COUNT(1) FROM CD45_QST WHERE MA_NHOM = @p0 AND MA_TCV = @p1) +
                        (SELECT COUNT(1) FROM CD45_TU_VAN_L1 WHERE MA_NHOM = @p0 AND MA_TCV = @p1) +
                        (SELECT COUNT(1) FROM CD45_TU_VAN_L2 WHERE MA_NHOM = @p0 AND MA_TCV = @p1)";
                int refCount = db.Database.SqlQuery<int>(countSql, item.MA_NHOM, item.MA_TCV).FirstOrDefault();
                if (refCount > 0)
                {
                    message = $"TCV '{item.TEN_TCV}' (Mã {item.MA_TCV} - Nhóm {item.TEN_NHOM}) đã có {refCount} bản ghi phát sinh (hoạt động/tư vấn/chuyển gửi) trong hệ thống. Để bảo toàn tính toàn vẹn dữ liệu và báo cáo lịch sử, không được phép xóa TCV này. Vui lòng chuyển trạng thái sang 'Ngừng hoạt động' thay vì xóa!";
                    return false;
                }

                db.Database.ExecuteSqlCommand("DELETE FROM CD45_NHOM_TCV WHERE ID = @p0 AND MADUAN = 'CD45'", id);
                LogAction("CD45_NHOM_TCV", username, $"Xóa TCV ID={id}: Mã={item.MA_TCV}, Tên={item.TEN_TCV}, Nhóm={item.MA_NHOM}");
                message = $"Đã xóa Tiếp cận viên '{item.TEN_TCV}' (Mã {item.MA_TCV}) thành công!";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi khi xóa TCV: " + ex.Message;
                return false;
            }
        }

        private void LogAction(string controller, string username, string content)
        {
            try
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = controller,
                    UserName = username ?? "System",
                    DateLog = DateTime.UtcNow,
                    Content = content
                };
                var logger = new SysLogDA();
                logger.Add(log);
            }
            catch { }
        }
    }
}
