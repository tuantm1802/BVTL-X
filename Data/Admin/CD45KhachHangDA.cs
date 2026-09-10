using Common.Common;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class CD45KhachHangDA : ICD45KhachHangDA
    {
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        private static string GetTenHonNhan(byte? hn)
        {
            switch (hn)
            {
                case 1: return "Độc thân";
                case 2: return "Đã kết hôn / Sống chung";
                case 3: return "Ly hôn / Ly thân";
                case 4: return "Góa";
                default: return "Chưa rõ";
            }
        }

        private void BuildFilterCriteria(CD45KhachHangFilterModel filter, out string baseWhere, out List<SqlParameter> pList)
        {
            baseWhere = " WHERE kh.MADUAN = 'CD45' ";
            pList = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                baseWhere += " AND (kh.RECORD_ID LIKE @Keyword OR kh.MA_KH_NGHIEN_CUU LIKE @Keyword) ";
                pList.Add(new SqlParameter("@Keyword", "%" + filter.Keyword.Trim() + "%"));
            }

            if (!string.IsNullOrEmpty(filter.CityCode))
            {
                baseWhere += " AND kh.CITY_CODE = @CityCode ";
                pList.Add(new SqlParameter("@CityCode", filter.CityCode));
            }

            if (!string.IsNullOrEmpty(filter.MaNhom))
            {
                var nhom = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == filter.MaNhom || x.manhom_tbh_map == filter.MaNhom);
                string map = nhom != null && !string.IsNullOrEmpty(nhom.manhom_tbh_map) ? nhom.manhom_tbh_map : filter.MaNhom;
                string std = nhom != null && !string.IsNullOrEmpty(nhom.manhom_tbh) ? nhom.manhom_tbh : filter.MaNhom;
                baseWhere += " AND (kh.MA_NHOM IN (@MaNhom, @MaNhomMap, @MaNhomStd) OR kh.REDCAP_DAG = @MaNhom) ";
                pList.Add(new SqlParameter("@MaNhom", filter.MaNhom));
                pList.Add(new SqlParameter("@MaNhomMap", map));
                pList.Add(new SqlParameter("@MaNhomStd", std));
            }

            if (filter.DoiTuong.HasValue)
            {
                baseWhere += " AND kh.DOI_TUONG = @DoiTuong ";
                pList.Add(new SqlParameter("@DoiTuong", filter.DoiTuong.Value));
            }

            if (filter.CoBHYT.HasValue)
            {
                baseWhere += " AND kh.CO_BHYT = @CoBHYT ";
                pList.Add(new SqlParameter("@CoBHYT", filter.CoBHYT.Value));
            }

            if (filter.CoCCCD.HasValue)
            {
                baseWhere += " AND kh.CO_CCCD = @CoCCCD ";
                pList.Add(new SqlParameter("@CoCCCD", filter.CoCCCD.Value));
            }

            if (!string.IsNullOrEmpty(filter.FromDate))
            {
                DateTime dtFrom;
                if (DateTime.TryParseExact(filter.FromDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFrom))
                {
                    baseWhere += " AND kh.NGAY_THAM_GIA >= @FromDate ";
                    pList.Add(new SqlParameter("@FromDate", dtFrom));
                }
            }

            if (!string.IsNullOrEmpty(filter.ToDate))
            {
                DateTime dtTo;
                if (DateTime.TryParseExact(filter.ToDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtTo))
                {
                    baseWhere += " AND kh.NGAY_THAM_GIA <= @ToDate ";
                    pList.Add(new SqlParameter("@ToDate", dtTo));
                }
            }
        }

        public DataTableResponse<CD45KhachHangViewModel> GetPagingCustomers(CD45KhachHangFilterModel filter)
        {
            if (filter == null) filter = new CD45KhachHangFilterModel();
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 20;

            string baseWhere;
            List<SqlParameter> pCountList;
            BuildFilterCriteria(filter, out baseWhere, out pCountList);

            // 1. Đếm tổng số bản ghi
            var countSql = "SELECT COUNT(*) FROM CD45_KH kh " + baseWhere;
            int totalRecords = db.Database.SqlQuery<int>(countSql, pCountList.ToArray()).FirstOrDefault();

            // 2. Truy vấn phân trang
            List<SqlParameter> pDataList;
            BuildFilterCriteria(filter, out baseWhere, out pDataList);

            int offset = (filter.PageIndex - 1) * filter.PageSize;
            pDataList.Add(new SqlParameter("@Offset", offset));
            pDataList.Add(new SqlParameter("@PageSize", filter.PageSize));

            var dataSql = @"
                SELECT 
                    kh.ID,
                    kh.RECORD_ID,
                    kh.MA_KH_NGHIEN_CUU,
                    kh.CITY_CODE,
                    ISNULL(c.Name, kh.CITY_CODE) AS CityName,
                    kh.MA_NHOM,
                    ISNULL(n.tennhom_tbh, ISNULL(kh.REDCAP_DAG, kh.MA_NHOM)) AS TenNhom,
                    kh.NGAY_THAM_GIA,
                    kh.DOI_TUONG,
                    CASE kh.DOI_TUONG
                        WHEN 1 THEN N'PUD (Sử dụng ma túy)'
                        WHEN 2 THEN N'PLHIV (Nhiễm HIV)'
                        WHEN 3 THEN N'TG (Chuyển giới)'
                        WHEN 4 THEN N'MSM (Quan hệ đồng giới nam)'
                        WHEN 5 THEN N'SW (Bán dâm)'
                        ELSE N'Khác'
                    END AS TenDoiTuong,
                    kh.GIOI_TINH_TU_XD,
                    CASE kh.GIOI_TINH_TU_XD
                        WHEN 1 THEN N'Nam'
                        WHEN 2 THEN N'Nữ'
                        WHEN 3 THEN N'Chuyển giới (Nữ sang Nam)'
                        WHEN 4 THEN N'Chuyển giới (Nam sang Nữ)'
                        ELSE N'Không xác định'
                    END AS TenGioiTinh,
                    kh.NAM_SINH,
                    CASE WHEN kh.NAM_SINH IS NOT NULL THEN (YEAR(GETDATE()) - kh.NAM_SINH) ELSE NULL END AS Tuoi,
                    kh.CO_CCCD,
                    kh.CO_BHYT,
                    kh.CO_THUONG_TRU,
                    kh.DANG_VO_GIA_CU,
                    kh.BI_TAM_GIU_6T,
                    kh.COMPLETE_STATUS,
                    kh.NGAY_SYNC
                FROM CD45_KH kh
                LEFT JOIN BVTL_CITES c ON kh.CITY_CODE = c.Code
                LEFT JOIN (
                    SELECT ISNULL(manhom_tbh_map, manhom_tbh) AS manhom_key, MAX(tennhom_tbh) AS tennhom_tbh
                    FROM BVTL_NHOM_TBH
                    WHERE maduan = 'CD45'
                    GROUP BY ISNULL(manhom_tbh_map, manhom_tbh)
                ) n ON kh.MA_NHOM = n.manhom_key
                " + baseWhere + @"
                ORDER BY kh.NGAY_THAM_GIA DESC, kh.RECORD_ID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var dataList = db.Database.SqlQuery<CD45KhachHangViewModel>(dataSql, pDataList.ToArray()).ToList();

            return new DataTableResponse<CD45KhachHangViewModel>
            {
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = dataList
            };
        }

        public CD45KhachHangDetailModel GetCustomerDetail(string recordId)
        {
            if (string.IsNullOrEmpty(recordId)) return null;

            var sqlInfo = @"
                SELECT 
                    kh.ID,
                    kh.RECORD_ID,
                    kh.MA_KH_NGHIEN_CUU,
                    kh.CITY_CODE,
                    ISNULL(c.Name, kh.CITY_CODE) AS CityName,
                    kh.MA_NHOM,
                    ISNULL(n.tennhom_tbh, ISNULL(kh.REDCAP_DAG, kh.MA_NHOM)) AS TenNhom,
                    kh.NGAY_THAM_GIA,
                    kh.DOI_TUONG,
                    CASE kh.DOI_TUONG
                        WHEN 1 THEN N'PUD (Sử dụng ma túy)'
                        WHEN 2 THEN N'PLHIV (Nhiễm HIV)'
                        WHEN 3 THEN N'TG (Chuyển giới)'
                        WHEN 4 THEN N'MSM (Quan hệ đồng giới nam)'
                        WHEN 5 THEN N'SW (Bán dâm)'
                        ELSE N'Khác'
                    END AS TenDoiTuong,
                    kh.GIOI_TINH_TU_XD,
                    CASE kh.GIOI_TINH_TU_XD
                        WHEN 1 THEN N'Nam'
                        WHEN 2 THEN N'Nữ'
                        WHEN 3 THEN N'Chuyển giới (Nữ sang Nam)'
                        WHEN 4 THEN N'Chuyển giới (Nam sang Nữ)'
                        ELSE N'Không xác định'
                    END AS TenGioiTinh,
                    kh.NAM_SINH,
                    CASE WHEN kh.NAM_SINH IS NOT NULL THEN (YEAR(GETDATE()) - kh.NAM_SINH) ELSE NULL END AS Tuoi,
                    kh.CO_CCCD,
                    kh.CO_BHYT,
                    kh.CO_THUONG_TRU,
                    kh.DANG_VO_GIA_CU,
                    kh.BI_TAM_GIU_6T,
                    kh.COMPLETE_STATUS,
                    kh.NGAY_SYNC
                FROM CD45_KH kh
                LEFT JOIN BVTL_CITES c ON kh.CITY_CODE = c.Code
                LEFT JOIN (
                    SELECT ISNULL(manhom_tbh_map, manhom_tbh) AS manhom_key, MAX(tennhom_tbh) AS tennhom_tbh
                    FROM BVTL_NHOM_TBH
                    WHERE maduan = 'CD45'
                    GROUP BY ISNULL(manhom_tbh_map, manhom_tbh)
                ) n ON kh.MA_NHOM = n.manhom_key
                WHERE kh.RECORD_ID = @RecordId";

            var vm = db.Database.SqlQuery<CD45KhachHangViewModel>(sqlInfo, new SqlParameter("@RecordId", recordId)).FirstOrDefault();
            if (vm == null) return null;

            // Extra fields from CD45_KH
            var sqlExtra = @"
                SELECT HON_NHAN, SO_CON, VO_GIA_CU_6T, THAM_GIA_NGHIEN_CUU, DOI_TUONG_KHAC
                FROM CD45_KH WHERE RECORD_ID = @RecordId";
            var extraRow = db.Database.SqlQuery<CD45ExtraKhModel>(sqlExtra, new SqlParameter("@RecordId", recordId)).FirstOrDefault();

            var detail = new CD45KhachHangDetailModel
            {
                ThongTinChung = vm,
                HON_NHAN = extraRow?.HON_NHAN,
                TenHonNhan = GetTenHonNhan(extraRow?.HON_NHAN),
                SO_CON = extraRow?.SO_CON,
                VO_GIA_CU_6T = extraRow?.VO_GIA_CU_6T,
                THAM_GIA_NGHIEN_CUU = extraRow?.THAM_GIA_NGHIEN_CUU,
                DOI_TUONG_KHAC = extraRow?.DOI_TUONG_KHAC
            };

            // 1. Hoạt động
            var sqlHd = @"
                SELECT 
                    hd.REPEAT_INSTANCE,
                    hd.NGAY_HOAT_DONG,
                    hd.LOAI_DV,
                    CASE hd.LOAI_DV
                        WHEN 1 THEN N'Truyền thông 1-1'
                        WHEN 2 THEN N'Sinh hoạt nhóm'
                        WHEN 3 THEN N'Can thiệp chữa lành'
                        ELSE N'Dịch vụ khác'
                    END AS TenLoaiDv,
                    hd.MA_TCV,
                    ISNULL(tcv.TEN_TCV, hd.MA_TCV) AS TenTCV,
                    hd.DIA_DIEM,
                    hd.CHU_DE,
                    hd.SO_TAI_LIEU,
                    hd.SO_BAO_CAO_SU,
                    hd.SO_CHAT_BOI_TRAN,
                    hd.SO_BOM_KIM,
                    hd.GHI_CHU
                FROM CD45_HOAT_DONG hd
                LEFT JOIN CD45_NHOM_TCV tcv ON hd.MA_NHOM = tcv.MA_NHOM AND hd.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE hd.RECORD_ID = @RecordId
                ORDER BY hd.NGAY_HOAT_DONG";
            detail.ListHoatDong = db.Database.SqlQuery<CD45_HoatDongItemModel>(sqlHd, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongHoatDong = detail.ListHoatDong.Count;

            // 2. Sàng lọc QST
            var sqlQst = @"
                SELECT 
                    qst.REPEAT_INSTANCE,
                    qst.NGAY_SANG_LOC,
                    qst.MA_TCV,
                    ISNULL(tcv.TEN_TCV, qst.MA_TCV) AS TenTCV,
                    qst.LY_DO_DANH_GIA_LAI,
                    qst.DIEM_QST,
                    qst.MUC_QST,
                    CASE qst.MUC_QST
                        WHEN 1 THEN N'Mức 1 (Điểm >= 8 - Rất cao)'
                        WHEN 2 THEN N'Mức 2 (Điểm 6-7 - Cao)'
                        WHEN 3 THEN N'Mức 3 (Điểm 4-5 - Trung bình)'
                        WHEN 4 THEN N'Mức 4 (Điểm < 4 - Thấp)'
                        ELSE N'Chưa xác định'
                    END AS TenMucQst,
                    qst.Q1A, qst.Q1B, qst.Q1C, qst.Q1D, qst.Q1E, qst.Q1F,
                    qst.Q2_TU_HARM, qst.Q3_NGHE
                FROM CD45_QST qst
                LEFT JOIN CD45_NHOM_TCV tcv ON qst.MA_NHOM = tcv.MA_NHOM AND qst.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE qst.RECORD_ID = @RecordId
                ORDER BY qst.NGAY_SANG_LOC";
            detail.ListQst = db.Database.SqlQuery<CD45_QstItemModel>(sqlQst, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongQst = detail.ListQst.Count;

            // 3. Chẩn đoán SKTT
            var sqlCd = @"
                SELECT 
                    cd.REPEAT_INSTANCE,
                    cd.NGAY_KHAM,
                    cd.CO_SO_Y_TE,
                    cd.BAC_SI,
                    cd.LAN_KHAM,
                    cd.TRIEU_CHUNG,
                    cd.CHAN_DOAN_CHINH,
                    cd.HINH_THUC_DIEU_TRI,
                    cd.NGAY_HEN_TAI_KHAM
                FROM CD45_CHAN_DOAN cd
                WHERE cd.RECORD_ID = @RecordId
                ORDER BY cd.NGAY_KHAM";
            detail.ListChanDoan = db.Database.SqlQuery<CD45_ChanDoanItemModel>(sqlCd, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongChanDoan = detail.ListChanDoan.Count;

            // 4. Hỗ trợ xã hội
            var sqlHt = @"
                SELECT 
                    ht.REPEAT_INSTANCE,
                    ht.NGAY_HO_TRO,
                    ht.MA_TCV,
                    ISNULL(tcv.TEN_TCV, ht.MA_TCV) AS TenTCV,
                    ht.DICH_VU,
                    ht.KET_QUA_HIV
                FROM CD45_HO_TRO_XH ht
                LEFT JOIN CD45_NHOM_TCV tcv ON ht.MA_NHOM = tcv.MA_NHOM AND ht.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE ht.RECORD_ID = @RecordId
                ORDER BY ht.NGAY_HO_TRO";
            detail.ListHoTroXh = db.Database.SqlQuery<CD45_HoTroXhItemModel>(sqlHt, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongHoTroXh = detail.ListHoTroXh.Count;

            // 5. Tuân thủ điều trị
            var sqlTt = @"
                SELECT 
                    tt.REPEAT_INSTANCE,
                    tt.NGAY_HO_TRO,
                    tt.MA_TCV,
                    ISNULL(tcv.TEN_TCV, tt.MA_TCV) AS TenTCV,
                    tt.HINH_THUC_DIEU_TRI,
                    tt.CO_KE_DON_THUOC,
                    tt.TUAN_THU
                FROM CD45_TUAN_THU tt
                LEFT JOIN CD45_NHOM_TCV tcv ON tt.MA_NHOM = tcv.MA_NHOM AND tt.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE tt.RECORD_ID = @RecordId
                ORDER BY tt.NGAY_HO_TRO";
            detail.ListTuanThu = db.Database.SqlQuery<CD45_TuanThuItemModel>(sqlTt, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongTuanThu = detail.ListTuanThu.Count;

            // 6. Tư vấn cá nhân (L1 & L2)
            var sqlTv = @"
                SELECT 
                    1 AS LanTuVan,
                    1 AS REPEAT_INSTANCE,
                    tv.NGAY_TU_VAN,
                    tv.MA_TCV,
                    ISNULL(tcv.TEN_TCV, tv.MA_TCV) AS TenTCV,
                    tv.DIA_DIEM,
                    tv.AUDIT_C_SCORE,
                    tv.PCL5_SCORE,
                    tv.PCL5_POSITIVE,
                    tv.STIGMA_SCORE,
                    tv.TINH_TRANG_SKTT,
                    tv.NHU_CAU_HO_TRO,
                    CAST(NULL AS NVARCHAR(MAX)) AS DANH_GIA_HIEN_TAI,
                    CAST(NULL AS NVARCHAR(MAX)) AS CAN_THIEP_AP_DUNG,
                    tv.LICH_HEN_TIEP AS NGAY_HEN_TIEP
                FROM CD45_TU_VAN_L1 tv
                LEFT JOIN CD45_NHOM_TCV tcv ON tv.MA_NHOM = tcv.MA_NHOM AND tv.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE tv.RECORD_ID = @RecordId

                UNION ALL

                SELECT 
                    2 AS LanTuVan,
                    tv2.REPEAT_INSTANCE,
                    tv2.NGAY_TU_VAN,
                    tv2.MA_TCV,
                    ISNULL(tcv.TEN_TCV, tv2.MA_TCV) AS TenTCV,
                    tv2.DIA_DIEM,
                    NULL AS AUDIT_C_SCORE,
                    NULL AS PCL5_SCORE,
                    NULL AS PCL5_POSITIVE,
                    NULL AS STIGMA_SCORE,
                    NULL AS TINH_TRANG_SKTT,
                    NULL AS NHU_CAU_HO_TRO,
                    tv2.DANH_GIA_HIEN_TAI,
                    tv2.CAN_THIEP_AP_DUNG,
                    tv2.NGAY_HEN_TIEP
                FROM CD45_TU_VAN_L2 tv2
                LEFT JOIN CD45_NHOM_TCV tcv ON tv2.MA_NHOM = tcv.MA_NHOM AND tv2.MA_TCV = tcv.MA_TCV AND tcv.MADUAN = 'CD45'
                WHERE tv2.RECORD_ID = @RecordId
                ORDER BY NGAY_TU_VAN";
            detail.ListTuVan = db.Database.SqlQuery<CD45_TuVanItemModel>(sqlTv, new SqlParameter("@RecordId", recordId)).ToList();
            detail.TongTuVan = detail.ListTuVan.Count;

            // 7. Vân tay
            var sqlVt = "SELECT COUNT(*) FROM CD45_VAN_TAY WHERE RECORD_ID = @RecordId";
            detail.CoVanTay = db.Database.SqlQuery<int>(sqlVt, new SqlParameter("@RecordId", recordId)).FirstOrDefault() > 0;

            return detail;
        }

        public List<CD45KhachHangViewModel> GetAllForExport(CD45KhachHangFilterModel filter)
        {
            if (filter == null) filter = new CD45KhachHangFilterModel();

            string baseWhere;
            List<SqlParameter> pList;
            BuildFilterCriteria(filter, out baseWhere, out pList);

            var sql = @"
                SELECT TOP 10000
                    kh.ID,
                    kh.RECORD_ID,
                    kh.MA_KH_NGHIEN_CUU,
                    kh.CITY_CODE,
                    ISNULL(c.Name, kh.CITY_CODE) AS CityName,
                    kh.MA_NHOM,
                    ISNULL(n.tennhom_tbh, ISNULL(kh.REDCAP_DAG, kh.MA_NHOM)) AS TenNhom,
                    kh.NGAY_THAM_GIA,
                    kh.DOI_TUONG,
                    CASE kh.DOI_TUONG
                        WHEN 1 THEN N'PUD (Sử dụng ma túy)'
                        WHEN 2 THEN N'PLHIV (Nhiễm HIV)'
                        WHEN 3 THEN N'TG (Chuyển giới)'
                        WHEN 4 THEN N'MSM (Quan hệ đồng giới nam)'
                        WHEN 5 THEN N'SW (Bán dâm)'
                        ELSE N'Khác'
                    END AS TenDoiTuong,
                    kh.GIOI_TINH_TU_XD,
                    CASE kh.GIOI_TINH_TU_XD
                        WHEN 1 THEN N'Nam'
                        WHEN 2 THEN N'Nữ'
                        WHEN 3 THEN N'Chuyển giới (Nữ sang Nam)'
                        WHEN 4 THEN N'Chuyển giới (Nam sang Nữ)'
                        ELSE N'Không xác định'
                    END AS TenGioiTinh,
                    kh.NAM_SINH,
                    CASE WHEN kh.NAM_SINH IS NOT NULL THEN (YEAR(GETDATE()) - kh.NAM_SINH) ELSE NULL END AS Tuoi,
                    kh.CO_CCCD,
                    kh.CO_BHYT,
                    kh.CO_THUONG_TRU,
                    kh.DANG_VO_GIA_CU,
                    kh.BI_TAM_GIU_6T,
                    kh.COMPLETE_STATUS,
                    kh.NGAY_SYNC
                FROM CD45_KH kh
                LEFT JOIN BVTL_CITES c ON kh.CITY_CODE = c.Code
                LEFT JOIN (
                    SELECT ISNULL(manhom_tbh_map, manhom_tbh) AS manhom_key, MAX(tennhom_tbh) AS tennhom_tbh
                    FROM BVTL_NHOM_TBH
                    WHERE maduan = 'CD45'
                    GROUP BY ISNULL(manhom_tbh_map, manhom_tbh)
                ) n ON kh.MA_NHOM = n.manhom_key
                " + baseWhere + @"
                ORDER BY kh.NGAY_THAM_GIA DESC, kh.RECORD_ID";

            return db.Database.SqlQuery<CD45KhachHangViewModel>(sql, pList.ToArray()).ToList();
        }
    }

    public class CD45ExtraKhModel
    {
        public byte? HON_NHAN { get; set; }
        public byte? SO_CON { get; set; }
        public bool? VO_GIA_CU_6T { get; set; }
        public bool? THAM_GIA_NGHIEN_CUU { get; set; }
        public string DOI_TUONG_KHAC { get; set; }
    }
}
