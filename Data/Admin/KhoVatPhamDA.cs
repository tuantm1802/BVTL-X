using log4net;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;
using Common.Common;
using Common.ICommon;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq.Dynamic.Core;

namespace Data.Admin
{
    public class KhoVatPhamDA: IKhoVatPhamDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["BVTL_REPORTINGEntities"].ConnectionString;


        public DataTableResponse<PhieuXuatNhap> GetAllPhieuXuatNhaps(DataTableRequest request)
        {
            var searchValue = request.search?.value;

            var phieuXuatNhap = db.PhieuXuatNhap.AsQueryable();
            if (!string.IsNullOrEmpty(searchValue))
            {
                phieuXuatNhap = phieuXuatNhap.Where(c =>
                                c.record_id.Contains(searchValue) ||
                                c.manhom_tbh.Contains(searchValue) 
                            );
            }

            int totalRecords = phieuXuatNhap.Count();

            var filteredData = phieuXuatNhap.OrderBy(c => c.Id)
                                        .Skip(request.start)
                                        .Take(request.length)                    
                                        .Select(c => new PhieuXuatNhap
                                        {
                                            Id = c.Id,
                                            record_id = c.record_id,
                                            manhom_tbh = c.manhom_tbh,
                                            MaPhieu  = c.MaPhieu,
                                            LoaiPhieu = c.LoaiPhieu,
                                            NgayLap = c.NgayLap,
                                            NguoiLap = c.NguoiLap,
                                            GhiChu = c.GhiChu,
                                            sync_date = c.sync_date,
                                            city_code = c.city_code
                                        })
                                        .ToList();
            // Tổng số bản ghi thỏa mãn điều kiện tìm kiếm
            var recordsFiltered = phieuXuatNhap.Count();

            return new DataTableResponse<PhieuXuatNhap>
                        {
                            draw = request.draw,
                            recordsTotal = totalRecords,
                            recordsFiltered = recordsFiltered,
                            data = filteredData
             }; 
        }

        public DataTableResponse<ChiTietPhieuXuatNhapModel> GetAllChiTietPhieuXuatNhaps(DataTableRequest request)
        {
            var searchValue = request.search?.value;

            var chiTietPhieuXuatNhap = db.ChiTietPhieuXuatNhap.AsQueryable();
            if (!string.IsNullOrEmpty(searchValue))
            {
                chiTietPhieuXuatNhap = chiTietPhieuXuatNhap.Where(c =>
                                //c.record_id.Contains(searchValue) ||
                                c.manhom_tbh.Contains(searchValue)
                            );
            }

            int totalRecords = chiTietPhieuXuatNhap.Count();

            var filteredData = chiTietPhieuXuatNhap
                                    .Join(db.SanPham,
                                          c => c.MaSanPham,
                                          sp => sp.MaSanPham,
                                          (c, sp) => new { c, sp }) // Join bảng ChiTietPhieuXuatNhap với SanPham
                                    .OrderBy(c => c.c.Id)
                                    .Skip(request.start)
                                    .Take(request.length)
                                    .Select(c => new ChiTietPhieuXuatNhapModel
                                    {
                                        Id = c.c.Id,
                                        SoLuong = c.c.SoLuong,
                                        MaPhieu = c.c.MaPhieu,
                                        MaSanPham = c.c.MaSanPham,
                                        LoaiPhieu = c.c.LoaiPhieu,
                                        NgayLap = c.c.NgayLap,
                                        GhiChu = c.c.GhiChu,
                                        sync_date = c.c.sync_date,
                                        manhom_tbh = c.c.manhom_tbh,
                                        city_code = c.c.city_code,
                                        TenSanPham = c.sp.TenSanPham // Lấy tên sản phẩm từ bảng SanPham
                                    })
                                    .ToList();
            // Tổng số bản ghi thỏa mãn điều kiện tìm kiếm
            var recordsFiltered = chiTietPhieuXuatNhap.Count();

            return new DataTableResponse<ChiTietPhieuXuatNhapModel>
            {
                draw = request.draw,
                recordsTotal = totalRecords,
                recordsFiltered = recordsFiltered,
                data = filteredData
            };
        }


        public bool CapNhatPhieuXuatNhap(ChiTietPhieuXuatNhapModel model)
        {
            
                if (model.LoaiPhieu == 1) // Nhập
                {
                    // Logic xử lý phiếu nhập
                }
                else if (model.LoaiPhieu == 2) // Xuất
                {
                    // Bước 1: Xóa các bản ghi trùng trong bảng PhieuXuatNhap
                    var deletePhieuXuatNhapQuery = @"
                DELETE FROM PhieuXuatNhap
                WHERE EXISTS (
                    SELECT 1
                    FROM (
                        SELECT 
                            'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                            2 AS LoaiPhieu, -- Phiếu Xuất
                            NgayPhat AS NgayLap
                        FROM (
                            -- Dữ liệu từ CD43_KHACH_HANG_PHIEU_TU_VAN
                            SELECT ptv.ngaytuvan AS NgayPhat
                            FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                            WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                            AND ptv.phiu_t_vn_complete = 2

                            UNION ALL

                            -- Dữ liệu từ CD43_KHACH_HANG_SINH_HOAT_NHOM
                            SELECT shn.ngay_shn AS NgayPhat
                            FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                            WHERE shn.phatvatpham = 1
                        ) AS Data
                        GROUP BY NgayPhat
                    ) AS RecordsToInsert
                    WHERE 
                        RecordsToInsert.MaPhieu = PhieuXuatNhap.MaPhieu
                        AND RecordsToInsert.LoaiPhieu = PhieuXuatNhap.LoaiPhieu
                        AND RecordsToInsert.NgayLap = PhieuXuatNhap.NgayLap
                );
            ";

                    //db.Database.ExecuteSqlRaw(deletePhieuXuatNhapQuery);
                    var result1= _DatabaseSql.ExecuteNonQuery(deletePhieuXuatNhapQuery);

                    // Bước 2: Thêm dữ liệu mới vào bảng PhieuXuatNhap
                    var insertPhieuXuatNhapQuery = @"
                    INSERT INTO PhieuXuatNhap (
                        MaPhieu,
                        LoaiPhieu,
                        NgayLap,
                        GhiChu,
                        sync_date
                    )
                    SELECT 
                        'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                        2 AS LoaiPhieu, -- Phiếu Xuất
                        NgayPhat AS NgayLap,
                        N'Phiếu xuất từ dữ liệu Phiếu tư vấn và Sinh hoạt nhóm' AS GhiChu,
                        GETDATE() AS sync_date
                    FROM (
                        -- Dữ liệu từ CD43_KHACH_HANG_PHIEU_TU_VAN
                        SELECT ptv.ngaytuvan AS NgayPhat
                        FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                        WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                        AND ptv.phiu_t_vn_complete = 2

                        UNION ALL

                        -- Dữ liệu từ CD43_KHACH_HANG_SINH_HOAT_NHOM
                        SELECT shn.ngay_shn AS NgayPhat
                        FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                        WHERE shn.phatvatpham = 1
                    ) AS Data
                    GROUP BY NgayPhat;
                ";

                    //db.Database.ExecuteSqlRaw(insertPhieuXuatNhapQuery);
                    var result2 = _DatabaseSql.ExecuteNonQuery(insertPhieuXuatNhapQuery);

                    // Bước 3: Xóa các bản ghi trùng trong bảng ChiTietPhieuXuatNhap
                    var deleteChiTietPhieuXuatNhapQuery = @"
                    DELETE FROM ChiTietPhieuXuatNhap
                    WHERE EXISTS (
                        SELECT 1
                        FROM (
                            -- Chuẩn bị các bản ghi sẽ được chèn vào
                            SELECT 
                                'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                                'vat_pham___1' AS MaSanPham,
                                2 AS LoaiPhieu,
                                NgayPhat AS NgayLap
                            FROM (
                                -- Dữ liệu từ CD43_KHACH_HANG_PHIEU_TU_VAN
                                SELECT ptv.ngaytuvan AS NgayPhat
                                FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                                WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                                AND ptv.phiu_t_vn_complete = 2

                                UNION ALL

                                -- Dữ liệu từ CD43_KHACH_HANG_SINH_HOAT_NHOM
                                SELECT shn.ngay_shn AS NgayPhat
                                FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                                WHERE shn.phatvatpham = 1
                            ) AS Data
                            GROUP BY NgayPhat

                            UNION ALL

                            SELECT 
                                'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                                'vat_pham___2' AS MaSanPham,
                                2 AS LoaiPhieu,
                                NgayPhat AS NgayLap
                            FROM (
                                -- Dữ liệu từ CD43_KHACH_HANG_PHIEU_TU_VAN
                                SELECT ptv.ngaytuvan AS NgayPhat
                                FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                                WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                                AND ptv.phiu_t_vn_complete = 2

                                UNION ALL

                                -- Dữ liệu từ CD43_KHACH_HANG_SINH_HOAT_NHOM
                                SELECT shn.ngay_shn AS NgayPhat
                                FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                                WHERE shn.phatvatpham = 1
                            ) AS Data
                            GROUP BY NgayPhat
                        ) AS RecordsToInsert
                        WHERE 
                            RecordsToInsert.MaPhieu = ChiTietPhieuXuatNhap.MaPhieu
                            AND RecordsToInsert.MaSanPham = ChiTietPhieuXuatNhap.MaSanPham
                            AND RecordsToInsert.LoaiPhieu = ChiTietPhieuXuatNhap.LoaiPhieu
                            AND RecordsToInsert.NgayLap = ChiTietPhieuXuatNhap.NgayLap
                    );
                ";

                    //db.Database.ExecuteSqlRaw(deleteChiTietPhieuXuatNhapQuery);
                    var result3 = _DatabaseSql.ExecuteNonQuery(deleteChiTietPhieuXuatNhapQuery);


                    // Bước 4: Chèn dữ liệu mới vào bảng ChiTietPhieuXuatNhap
                    var insertChiTietPhieuXuatNhapQuery = @"
                    INSERT INTO ChiTietPhieuXuatNhap (
                        NgayLap,
                        LoaiPhieu,
                        MaPhieu,
                        MaSanPham,
                        SoLuong,
                        maduan,
                        sync_date,
                        GhiChu
                    )
                    SELECT 
                        NgayPhat AS NgayLap,
                        2 AS LoaiPhieu,
                        'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                        'vat_pham___1' AS MaSanPham,
                        SUM(TRY_CAST(SoLuongBCS AS INT)) AS SoLuong,
                        'CD43' AS maduan,
                        GETDATE() AS sync_date,
                        N'Phiếu xuất từ dữ liệu Phiếu tư vấn và Sinh hoạt nhóm' AS GhiChu
                    FROM (
                        SELECT 
                            ptv.ngaytuvan AS NgayPhat,
                            ptv.cau6_4_1 AS SoLuongBCS,
                            NULL AS SoLuongGel
                        FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                        WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                        AND ptv.phiu_t_vn_complete = 2

                        UNION ALL

                        SELECT 
                            shn.ngay_shn AS NgayPhat,
                            shn.bcs AS SoLuongBCS,
                            NULL AS SoLuongGel
                        FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                        WHERE shn.phatvatpham = 1
                    ) AS DataBCS
                    GROUP BY NgayPhat

                    UNION ALL

                    SELECT 
                        NgayPhat AS NgayLap,
                        2 AS LoaiPhieu,
                        'PX' + FORMAT(NgayPhat, 'yyyyMMdd') AS MaPhieu,
                        'vat_pham___2' AS MaSanPham,
                        SUM(TRY_CAST(SoLuongGel AS INT)) AS SoLuong,
                        'CD43' AS maduan,
                        GETDATE() AS sync_date,
                        N'Phiếu xuất từ dữ liệu Phiếu tư vấn và Sinh hoạt nhóm' AS GhiChu
                    FROM (
                        SELECT 
                            ptv.ngaytuvan AS NgayPhat,
                            NULL AS SoLuongBCS,
                            ptv.cau6_5_1 AS SoLuongGel
                        FROM CD43_KHACH_HANG_PHIEU_TU_VAN ptv
                        WHERE (ptv.cau6_4 = 1 OR ptv.cau6_5 = 1)
                        AND ptv.phiu_t_vn_complete = 2

                        UNION ALL

                        SELECT 
                            shn.ngay_shn AS NgayPhat,
                            NULL AS SoLuongBCS,
                            shn.gel AS SoLuongGel
                        FROM CD43_KHACH_HANG_SINH_HOAT_NHOM shn
                        WHERE shn.phatvatpham = 1
                    ) AS DataGel
                    GROUP BY NgayPhat;
                ";

                    //db.Database.ExecuteSqlRaw(insertChiTietPhieuXuatNhapQuery);
                    var result4 = _DatabaseSql.ExecuteNonQuery(insertChiTietPhieuXuatNhapQuery);
                }

                //db.SaveChanges();
                return true;
            
        }

        public List<TonKhoModel> GetTonKhoData(DateTime fromDate, DateTime toDate)
        {
            var listTonKho = new List<TonKhoModel>();

            // Sử dụng tham số để tránh SQL Injection và đảm bảo định dạng ngày chính xác
            string query = @"
                    -- Tồn kho đầu kỳ
                    WITH TonKhoDauKy AS (
                        SELECT 
                            SP.MaSanPham,
                            SP.TenSanPham, SP.DonViTinh,
                            ISNULL(SUM(CASE 
                                WHEN PXN.NgayLap < @FromDate AND PXN.LoaiPhieu = 1 THEN CT.SoLuong 
                                WHEN PXN.NgayLap < @FromDate AND PXN.LoaiPhieu = 2 THEN -CT.SoLuong 
                                ELSE 0 END), 0) AS TonDauKy
                        FROM 
                            SanPham SP
                        LEFT JOIN ChiTietPhieuXuatNhap CT ON SP.MaSanPham = CT.MaSanPham
                        LEFT JOIN PhieuXuatNhap PXN ON CT.MaPhieu = PXN.MaPhieu
                        GROUP BY SP.MaSanPham, SP.TenSanPham, SP.DonViTinh
                    ),
                    -- Nhập kho và xuất kho trong khoảng thời gian
                    NhapXuatTrongKy AS (
                        SELECT 
                            SP.MaSanPham,
                            SUM(CASE WHEN PXN.LoaiPhieu = 1 THEN CT.SoLuong ELSE 0 END) AS NhapKho,
                            SUM(CASE WHEN PXN.LoaiPhieu = 2 THEN CT.SoLuong ELSE 0 END) AS XuatKho
                        FROM 
                            SanPham SP
                        LEFT JOIN ChiTietPhieuXuatNhap CT ON SP.MaSanPham = CT.MaSanPham
                        LEFT JOIN PhieuXuatNhap PXN ON CT.MaPhieu = PXN.MaPhieu
                        WHERE PXN.NgayLap BETWEEN @FromDate AND @ToDate
                        GROUP BY SP.MaSanPham
                    )
                    -- Tổng hợp báo cáo
                    SELECT 
                        TKD.MaSanPham,
                        TKD.TenSanPham, TKD.DonViTinh,
                        TKD.TonDauKy,
                        NX.NhapKho,
                        NX.XuatKho,
                        (TKD.TonDauKy + NX.NhapKho - NX.XuatKho) AS TonCuoiKy
                    FROM TonKhoDauKy TKD
                    LEFT JOIN NhapXuatTrongKy NX ON TKD.MaSanPham = NX.MaSanPham
                    ORDER BY TKD.MaSanPham";

            // Khai báo các tham số
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate },
                new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate }
            };

            // Thực thi câu truy vấn và lấy kết quả
            var result = _DatabaseSql.ExecuteTable(query, parameters);

            if (result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    listTonKho.Add(new TonKhoModel
                    {
                        MaSanPham = row.Field<string>("MaSanPham"),
                        TenSanPham = row.Field<string>("TenSanPham"),
                        DVT = row.Field<string>("DonViTinh"),
                        TonDauKy = row.Field<int?>("TonDauKy"),
                        TonCuoiKy = row.Field<int?>("TonCuoiKy"),
                        NhapKho = row.Field<int?>("NhapKho"),
                        XuatKho = row.Field<int?>("XuatKho"),
                    });
                }
            }

            return listTonKho;
        }


        //public Customer GetCustomerById(int id)
        //{
        //    Customer customer = null;
        //    //string query = $@"  SELECT 
        //    //                    t.id as Id, 
        //    //                    t.record_id AS RecordId, 
        //    //                    t.manhom_tbh AS MaNhomTbh, 
        //    //                    t.ngay_thang_nam_sinh AS NgayThangNamSinh,
        //    //                    t.gioi_tinh AS GioiTinh,
        //    //                    t.cap_bac_hoc_van AS CapBacHocVan,
        //    //                    t.nghe_nghiep AS NgheNghiep,
        //    //                    t.ngayhoi AS NgayHoi, 
        //    //                    CASE WHEN hiv = 1 THEN CASE WHEN kqxn = 1 THEN N'Có phản ứng' WHEN kqxn = 2 THEN N'Âm tính'  ELSE 'Không xác định' END  WHEN hiv = 0 THEN N'Không tham gia xét nghiệm'  ELSE 'Không xác định' END AS [ketQuaXN]
        //    //                    FROM CD43_KHACH_HANG_THONG_TIN_CO_BAN t
        //    //                    INNER JOIN CD43_KHACH_HANG_SANG_LOC_HIV hiv ON t.record_id = hiv.record_id
        //    //                    WHERE t.Id = '{id}';
        //    //                ";
        //    string query = $@"  SELECT
        //                         t.id AS Id,
        //                         t.record_id AS RecordId,
        //                         t.manhom_tbh AS MaNhomTbh,
        //                         t.ngay_thang_nam_sinh AS NgayThangNamSinh,
        //                         t.gioi_tinh AS GioiTinh,
        //                         t.cap_bac_hoc_van AS CapBacHocVan,
        //                         t.nghe_nghiep AS NgheNghiep,
        //                         t.ngayhoi AS NgayHoi,
        //                         CASE
        //                          WHEN hiv = 1 THEN -- Có làm xét nghiệm
        //                           CASE			
        //                            WHEN kqxn = 1 THEN N'Có phản ứng' 
        //                            WHEN kqxn = 2 THEN N'Âm tính' 
        //                            ELSE 'Không xác định' 
        //                           END 
        //                          WHEN hiv = 0 THEN N'Không tham gia xét nghiệm' 
        //                          ELSE 'Không xác định' 
        //                         END AS [KetQuaXN],
        //                         CASE
        //                          WHEN hiv = 1 AND kqxn = 1 THEN -- Có làm xét nghiệm
        //                           CASE 
        //                            WHEN chuyengui___2 = 1 THEN N'Đã Xét nghiệm khẳng định HIV và điều trị ARV'
        //                            ELSE 'Chưa Xét nghiệm khẳng định HIV và điều trị ARV'
        //                           END
        //                          ELSE 'N/A' 
        //                         END AS [KetQuaChuyenGuiDieuTriARV],
        //                         CASE
        //                          WHEN hiv = 0 THEN -- không xét nghiệm nhanh HIV (tức là đã điều trị ARV)
        //                           CASE 
        //                            WHEN chuyengui___3 = 1 THEN 
        //                             CONCAT(N'Đã Xét nghiệm tải lượng virus HIV vào ngày: ', FORMAT(f2_q_3_1_d, 'dd/MM/yyyy'), N'; Tải lượng VR: ',f2_q_3_1_1)
        //                            ELSE N'Chưa Xét nghiệm tải lượng virus HIV'
        //                           END
        //                          ELSE 'N/A' 
        //                         END AS [ketQuaChuyenGuiTaiLuongVR],
        //                         FORMAT(f2_q_3_1_d, 'dd/MM/yyyy') AS NgayXNTLVR,
        //                         f2_q_3_1_1 AS TaiLuongVR
        //                        FROM
        //                         CD43_KHACH_HANG_THONG_TIN_CO_BAN t
        //                         LEFT JOIN CD43_KHACH_HANG_SANG_LOC_HIV hiv ON t.record_id = hiv.record_id AND hiv.sng_lc_hiv_complete = 2
        //                         LEFT JOIN CD43_KHACH_HANG_CHUYEN_GUI cg ON cg.record_id = t.record_id  AND cg.chuyn_gi_complete = 2                               
        //                        WHERE t.Id = '{id}'
        //                            AND t.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = 2 ;	                               
        //                    ";

        //    var result = _DatabaseSql.ExecuteTable(query);
        //    if (result.Rows.Count > 0)
        //    {
        //        // Tạo đối tượng Customer và gán các giá trị từ DataTable
        //        DataRow row = result.Rows[0];
        //        customer = new Customer
        //        {
        //            Id = row.Field<int>("Id"),
        //            RecordId = row.Field<string>("RecordId"),
        //            NgayThangNamSinh = row.Field<DateTime?>("NgayThangNamSinh"),
        //            GioiTinh = row.Field<string>("GioiTinh"),
        //            CapBacHocVan = row.Field<string>("CapBacHocVan"),
        //            NgheNghiep = row.Field<string>("NgheNghiep"),
        //            KetQuaXNHiv = row.Field<string>("KetQuaXN"),
        //            KetQuaChuyenGuiDieuTriARV = row.Field<string>("KetQuaChuyenGuiDieuTriARV"),
        //            KetQuaChuyenGuiTaiLuongVR = row.Field<string>("KetQuaChuyenGuiTaiLuongVR"),
        //            //NgayXNTLVR = row.Field<DateTime?>("NgayXNTLVR"),

        //        };
        //    }
        //    return customer;
        //}
    }
}
