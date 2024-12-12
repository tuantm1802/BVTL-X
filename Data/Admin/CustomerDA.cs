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
    public class CustomerDA: ICustomerDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["BVTL_REPORTINGEntities"].ConnectionString;


        public BVTL_KHACH_HANG GetItemByCode(string code)
        {
            return db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == code);
        }

        public int GetTotalCustomersCount()
        {
            using (BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities())
            {
                return db.CD43_KHACH_HANG_THONG_TIN_CO_BAN
                    .Where(c => c.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete != null && c.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete == 2)
                    .Count();
            }
        }

        public DataTableResponse<Customer> GetAllCustomers(DataTableRequest request)
        {
            var searchValue = request.search?.value;

            var customers = db.CD43_KHACH_HANG_THONG_TIN_CO_BAN.AsQueryable()
                                .Where(c => c.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete != null
                            && c.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete == 2);
            if (!string.IsNullOrEmpty(searchValue))
            {
                customers = customers.Where(c =>
                                c.record_id.Contains(searchValue) ||
                                c.manhom_tbh.Contains(searchValue) ||
                                c.gioi_tinh.Contains(searchValue)
                            );
            }

            var filteredData = customers.OrderBy(c => c.id)
                                        .Skip(request.start)
                                        .Take(request.length)                    
                                        .Select(c => new Customer
                                        {
                                            Id = c.id,
                                            RecordId = c.record_id,
                                            MaNhomTbh = c.manhom_tbh,
                                            NgayThangNamSinh = c.ngay_thang_nam_sinh,
                                            GioiTinh = c.gioi_tinh,
                                            CapBacHocVan = c.cap_bac_hoc_van,
                                            NgheNghiep = c.nghe_nghiep,
                                            NgheNghiepKhac = c.nghe_nghiep_khac,
                                            NgayHoi = c.ngayhoi,
                                            NgayTuVan = c.ngaytuvan,
                                            PhiuTVanComplete = c.phiu_t_vn_complete,
                                            NgayBangHoiNgay = c.ngaybanghoi_ngay_3db24b,
                                            NgayShn = c.ngay_shn,
                                            NgayTheoDauTime = c.ngaytheodau_time,
                                            CityCodeMap = c.city_code_map
                                        })
                                        .ToList();
            // Tổng số bản ghi thỏa mãn điều kiện tìm kiếm
            var recordsFiltered = customers.Count();
            int totalRecords = this.GetTotalCustomersCount();

            return new DataTableResponse<Customer>
                        {
                            draw = request.draw,
                            recordsTotal = totalRecords,
                            recordsFiltered = recordsFiltered,
                            data = filteredData
             }; 
        }

        public Customer GetCustomerById(int id)
        {
            Customer customer = null;
            //string query = $@"  SELECT 
            //                    t.id as Id, 
            //                    t.record_id AS RecordId, 
            //                    t.manhom_tbh AS MaNhomTbh, 
            //                    t.ngay_thang_nam_sinh AS NgayThangNamSinh,
            //                    t.gioi_tinh AS GioiTinh,
            //                    t.cap_bac_hoc_van AS CapBacHocVan,
            //                    t.nghe_nghiep AS NgheNghiep,
            //                    t.ngayhoi AS NgayHoi, 
            //                    CASE WHEN hiv = 1 THEN CASE WHEN kqxn = 1 THEN N'Có phản ứng' WHEN kqxn = 2 THEN N'Âm tính'  ELSE 'Không xác định' END  WHEN hiv = 0 THEN N'Không tham gia xét nghiệm'  ELSE 'Không xác định' END AS [ketQuaXN]
            //                    FROM CD43_KHACH_HANG_THONG_TIN_CO_BAN t
            //                    INNER JOIN CD43_KHACH_HANG_SANG_LOC_HIV hiv ON t.record_id = hiv.record_id
            //                    WHERE t.Id = '{id}';
            //                ";
            string query = $@"  SELECT
	                                t.id AS Id,
	                                t.record_id AS RecordId,
	                                t.manhom_tbh AS MaNhomTbh,
	                                t.ngay_thang_nam_sinh AS NgayThangNamSinh,
	                                t.gioi_tinh AS GioiTinh,
	                                t.cap_bac_hoc_van AS CapBacHocVan,
	                                t.nghe_nghiep AS NgheNghiep,
	                                t.ngayhoi AS NgayHoi,
	                                CASE
		                                WHEN hiv = 1 THEN -- Có làm xét nghiệm
			                                CASE			
				                                WHEN kqxn = 1 THEN N'Có phản ứng' 
				                                WHEN kqxn = 2 THEN N'Âm tính' 
				                                ELSE 'Không xác định' 
			                                END 
		                                WHEN hiv = 0 THEN N'Không tham gia xét nghiệm' 
		                                ELSE 'Không xác định' 
	                                END AS [KetQuaXN],
	                                CASE
		                                WHEN hiv = 1 AND kqxn = 1 THEN -- Có làm xét nghiệm
			                                CASE 
				                                WHEN chuyengui___2 = 1 THEN N'Đã Xét nghiệm khẳng định HIV và điều trị ARV'
				                                ELSE 'Chưa Xét nghiệm khẳng định HIV và điều trị ARV'
			                                END
		                                ELSE 'N/A' 
	                                END AS [KetQuaChuyenGuiDieuTriARV],
	                                CASE
		                                WHEN hiv = 0 THEN -- không xét nghiệm nhanh HIV (tức là đã điều trị ARV)
			                                CASE 
				                                WHEN chuyengui___3 = 1 THEN 
					                                CONCAT(N'Đã Xét nghiệm tải lượng virus HIV vào ngày: ', FORMAT(f2_q_3_1_d, 'dd/MM/yyyy'), N'; Tải lượng VR: ',f2_q_3_1_1)
				                                ELSE 'Chưa Xét nghiệm tải lượng virus HIV'
			                                END
		                                ELSE 'N/A' 
	                                END AS [ketQuaChuyenGuiTaiLuongVR],
	                                FORMAT(f2_q_3_1_d, 'dd/MM/yyyy') AS NgayXNTLVR,
	                                f2_q_3_1_1 AS TaiLuongVR
                                FROM
	                                CD43_KHACH_HANG_THONG_TIN_CO_BAN t
	                                LEFT JOIN CD43_KHACH_HANG_SANG_LOC_HIV hiv ON t.record_id = hiv.record_id AND hiv.sng_lc_hiv_complete = 2
	                                LEFT JOIN CD43_KHACH_HANG_CHUYEN_GUI cg ON cg.record_id = t.record_id  AND cg.chuyn_gi_complete = 2                               
                                WHERE t.Id = '{id}'
                                    AND t.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = 2 ;	                               
                            ";

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {
                    Id = row.Field<int>("Id"),
                    RecordId = row.Field<string>("RecordId"),
                    NgayThangNamSinh = row.Field<DateTime?>("NgayThangNamSinh"),
                    GioiTinh = row.Field<string>("GioiTinh"),
                    CapBacHocVan = row.Field<string>("CapBacHocVan"),
                    NgheNghiep = row.Field<string>("NgheNghiep"),
                    KetQuaXNHiv = row.Field<string>("KetQuaXN"),
                    KetQuaChuyenGuiDieuTriARV = row.Field<string>("KetQuaChuyenGuiDieuTriARV"),
                    KetQuaChuyenGuiTaiLuongVR = row.Field<string>("KetQuaChuyenGuiTaiLuongVR"),
                    //NgayXNTLVR = row.Field<DateTime?>("NgayXNTLVR"),
                    
                };
            }
            return customer;
        }

        public Customer GetCustomerChuyenGuiById(int id)
        {
            Customer customer = null;
            string query = "SELECT top 1                                                                       "+
                           " t.id AS Id,                                                                       "+
                           " t.record_id AS RecordId,                                                          "+
                            " cg.f2_q_3_1_d AS NgayXNTLVR,                                                     " +
                           "                                                                                   "+
                           " CASE WHEN cg.chuyengui___3 = 1 THEN N'Được chuyển gửi'                            "+
                           "      ELSE N'Chưa được chuyển gửi'                                                 "+
                           " END AS ChuyenGuiStatus,                                                           "+
                           "                                                                                   "+
                           " ROW_NUMBER() OVER(PARTITION BY t.record_id ORDER BY cg.f2_q_3_1_d DESC) AS RowNum "+
                           "FROM                                                            "+
                           "    CD43_KHACH_HANG_THONG_TIN_CO_BAN t                          "+
                           "LEFT JOIN                                                       "+
                           "    CD43_KHACH_HANG_CHUYEN_GUI cg ON t.record_id = cg.record_id "+
                           "WHERE                                                           "+
                           " t.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = 2 "+
                           " AND t.Id = " + id;

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {
                    Id = row.Field<int>("Id"),
                    RecordId = row.Field<string>("RecordId"),
                    NgayXNTLVR = row.Field<DateTime?>("NgayXNTLVR"),
                    ChuyenGuiStatus = row.Field<string>("ChuyenGuiStatus"),

                };
            }
            return customer;
        }
        public Customer GetCustomerSuDungChatById(int id)
        {
            Customer customer = null;
            string query = @"SELECT                                                                          
                                t.id as Id,                                                                    
                                t.record_id AS RecordId,                                                        
                                STUFF((                                                                    
		                                SELECT ', ' + 
			                                CASE WHEN h.f1_q_b4___1 = 1 THEN N'Methamphetamin (Ma túy đá)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___2 = 1 THEN N', Ecstasy (kẹo, lắc)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___3 = 1 THEN N', Ketamin (ke)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___4 = 1 THEN N', Các chất hít hơi (Poppers)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___5 = 1 THEN N', Thuốc cương dương (Viagra)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___6 = 1 THEN N', Hồng phiến (ngựa, da)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___7 = 1 THEN N', Nước biển (nước G)' ELSE '' END +
			                                CASE WHEN h.f1_q_b4___8 = 1 THEN N', Khác' ELSE '' END
		                                FROM CD43_KHACH_HANG_HANH_VI_NGUY_CO h1                                  
		                                WHERE h1.record_id = h.record_id                                         
		                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') 
		                                AS ChatSuDungChemsex3Thang,                                                
                                CASE                                                                       
		                                WHEN h.f1_q_b5 = 1 THEN N'Methamphetamin (Ma túy đá)'                      
		                                WHEN h.f1_q_b5 = 2 THEN N'Ecstasy (kẹo, lắc)'                              
		                                WHEN h.f1_q_b5 = 3 THEN N'Ketamin (ke)'                                    
		                                WHEN h.f1_q_b5 = 4 THEN N'Các chất hít hơi (Poppers)'                      
		                                WHEN h.f1_q_b5 = 5 THEN N'Thuốc cương dương (Viagra)'                      
		                                WHEN h.f1_q_b5 = 6 THEN N'Hồng phiến (ngựa, da)'                           
		                                WHEN h.f1_q_b5 = 7 THEN N'Nước biển (nước G)'                              
		                                WHEN h.f1_q_b5 = 8 THEN N'Khác'                                            
		                                ELSE NULL                                                                  
                                END AS ChatSuDungThuongXuyenNhat,                                              
																																					 
                                CASE                                                                       
		                                WHEN h.f1_q_b6 = 1 THEN N'1 - 2 lần'                                       
		                                WHEN h.f1_q_b6 = 2 THEN N'Mỗi tháng (3 - 9 lần)'                           
		                                WHEN h.f1_q_b6 = 3 THEN N'Mỗi tuần (1 - 4 lần/tuần)'                       
		                                WHEN h.f1_q_b6 = 4 THEN N'Mỗi ngày hoặc gần như mỗi ngày (5 - 7 lần/tuần)' 
		                                ELSE NULL                                                                  
                                END AS TanSuatSuDungMTDChemsex3Thang,                                          
                                CASE                                                                       
				                                WHEN h.f1_q_b7 = 1 THEN N'Đã từng' 																		 
				                                WHEN h.f1_q_b7 = 2 THEN N'Chưa từng'   																	 
		                                END AS SuDungDaChat,                                                       
																																					 
                                STUFF((                                                                    
		                                SELECT ', '  +                                                          
				                                CASE WHEN h.f1_q_b8___1 = 1 THEN N'Methamphetamin (Ma túy đá)' ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___2 = 1 THEN N', Ecstasy (kẹo, lắc)'         ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___3 = 1 THEN N', Ketamin (ke)'               ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___4 = 1 THEN N', Các chất hít hơi (Poppers)' ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___5 = 1 THEN N', Thuốc cương dương (Viagra)' ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___6 = 1 THEN N', Hồng phiến (ngựa, da)'      ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___7 = 1 THEN N', Nước biển (nước G)'         ELSE '' END +             
				                                CASE WHEN h.f1_q_b8___8 = 1 THEN N', Khác'                       ELSE '' END              
	
		                                FROM CD43_KHACH_HANG_HANH_VI_NGUY_CO h2                                    
		                                WHERE h2.record_id = h.record_id                                           
		                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS SuDungChatDaChat,
		
                                CASE                                                                           
		                                    WHEN h.f1_q_b9 = 1 THEN N'1 - 2 lần'                                      
		                                    WHEN h.f1_q_b9 = 2 THEN N'Mỗi tháng (3 - 9 lần)'                          
		                                    WHEN h.f1_q_b9 = 3 THEN N'Mỗi tuần (1 - 4 lần/tuần)'                      
		                                    WHEN h.f1_q_b9 = 4 THEN N'Mỗi ngày hoặc gần như mỗi ngày (5 - 7 lần/tuần)'
		                                    WHEN h.f1_q_b9 = 5 THEN N'Không lần nào'                                  
		                                    ELSE NULL                                                                 
                                    END AS TanSuatSuDungDaChat3Thang                                              
                                FROM                                                                               
		                                CD43_KHACH_HANG_THONG_TIN_CO_BAN t                                         
		                                LEFT JOIN CD43_KHACH_HANG_HANH_VI_NGUY_CO h ON t.record_id = h.record_id   
                                WHERE(                                                                            
                                h.f1_q_b4___1 = 1                                                              
                                OR h.f1_q_b4___2 = 1                                                           
                                OR h.f1_q_b4___3 = 1                                                           
                                OR h.f1_q_b4___4 = 1                                                           
                                OR h.f1_q_b4___5 = 1                                                           
                                OR h.f1_q_b4___6 = 1                                                           
                                OR h.f1_q_b4___7 = 1                                                           
                                OR h.f1_q_b4___8 = 1)                                                          
                                AND t.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = 2   
                                AND t.Id =" + @id;

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {
                    Id = row.Field<int>("Id"),
                    RecordId = row.Field<string>("RecordId"),                    
                    ChatSuDungChemsex3Thang = row.Field<string>("ChatSuDungChemsex3Thang"),
                    ChatSuDungThuongXuyenNhat = row.Field<string>("ChatSuDungThuongXuyenNhat"),
                    TanSuatSuDungMTDChemsex3Thang = row.Field<string>("TanSuatSuDungMTDChemsex3Thang"),
                    SuDungDaChat = row.Field<string>("SuDungDaChat"),
                    SuDungChatDaChat = row.Field<string>("SuDungChatDaChat"),

                };
            }
            return customer;
        }

        public Customer GetCustomerQHTDById(int id)
        {
            Customer customer = null;
            string query = @"SELECT top 1                                                           
                            t.id AS Id,                                                           
                            t.record_id AS RecordId,                                              
                            CASE                                                                  
                                WHEN h.f1_q_a3 = 1 THEN N'Quan hệ tình dục đồng giới'             
                                WHEN h.f1_q_a3 = 2 THEN N'Quan hệ tình dục khác giới'             
                                WHEN h.f1_q_a3 = 3 THEN N'Quan hệ tình dục đồng giới và khác giới'
                                WHEN h.f1_q_a3 = 4 THEN N'Khác'                                   
                                ELSE NULL                                                         
                            END AS DoiTuongQuanHe,                                                  
		                        CASE                                                              
                                WHEN h.f1_q_a5___1 = 1 THEN N'Đang có bạn tình không thường xuyên'
                                WHEN h.f1_q_a5___2 = 1 THEN N'Đang có bạn tình nam thường xuyên'  
                                WHEN h.f1_q_a5___3 = 1 THEN N'Đang có bạn tình nữ thường xuyên'   
                                ELSE NULL                                                         
                            END AS TinhTrangMoiQHHT,                                              
		                        CASE                                                              
                                WHEN h.f1_q_b10 = 1 THEN N'Luôn luôn'                             
                                WHEN h.f1_q_b10 = 2 THEN N'Thường xuyên'                          
                                WHEN h.f1_q_b10 = 3 THEN N'Thỉnh thoảng'                          
                                WHEN h.f1_q_b10 = 4 THEN N'Hiếm khi'                              
                                WHEN h.f1_q_b10 = 5 THEN N'Không bao giờ'                         
                                ELSE NULL                                                         
                            END AS TanSuatSuDungBCSChemsex3Thang,                                 
		                    CASE                                                              
                                WHEN h.f1_q_b11 = 1 THEN N'Có'                                
                                WHEN h.f1_q_b11 = 2 THEN N'Không'                             
                                WHEN h.f1_q_b11 = 3 THEN N'Không biết/Không trả lời'          
                            END AS QHTDTT3Thang,                                              
		                    CASE                                                              
                                WHEN h.f1_q_b12 = 1 THEN N'Có'                                    
                                WHEN h.f1_q_b12 = 2 THEN N'Không'                                 
                                WHEN h.f1_q_b12 = 3 THEN N'Không biết/Không trả lời'              
                                ELSE NULL                                                         
                            END AS BanDam3Thang,                                                   
                            CASE                                                                   
                                 WHEN h.f1_q_b13 = 1 THEN N'Có'                                    
                                 WHEN h.f1_q_b13 = 2 THEN N'Không'                                 
                                 WHEN h.f1_q_b13 = 3 THEN N'Không biết/Không trả lời'              
                                 ELSE NULL                                                         
                             END AS STIs,		                                                    
															CASE     																																																											
                                 WHEN h.f1_q_b15 = 1 AND h.f1_q_b16 = 1 THEN N'Có biết - Không mắc viêm gan C'                  
                                 WHEN h.f1_q_b15 = 1 AND h.f1_q_b16 = 2 THEN N'Có biết - Đã từng mắc viêm gan C và đã điều trị' 
                                 WHEN h.f1_q_b15 = 1 AND h.f1_q_b16 = 3 THEN N'Có biết - Hiện tại mắc viêm gan C'               
                                 WHEN h.f1_q_b15 = 2 THEN N'Không'
                                 WHEN h.f1_q_b15 = 3 THEN N'Không biết/Không trả lời'																	
																 ELSE NULL                                                         
                             END AS TinhTrangViemGanC,                                             
		                         CASE                                                              
                                 WHEN h.diemthuocla >= 0 AND h.diemthuocla <= 3 THEN N'Mức nguy cơ: THẤP'                   
                                 WHEN h.diemthuocla >= 4 AND h.diemthuocla <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'            
                                 WHEN h.diemthuocla >= 27 THEN N'Mức nguy cơ: CAO'                                          
                                 ELSE NULL                                                                                  
                             END AS MucDoNguyCoThuocLa,                                                                     
		                         CASE                                                                                       
                                 WHEN h.diemthucuong >= 0 AND h.diemthucuong <= 10 THEN N'Mức nguy cơ: THẤP'                
                                 WHEN h.diemthucuong >= 11 AND h.diemthucuong <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'         
                                 WHEN h.diemthucuong >= 27 THEN N'Mức nguy cơ: CAO'                                         
                                 ELSE NULL                                                                                  
                             END AS MucDoNguyCoThucUong,                                                                    
		                         CASE                                                                                       
                                 WHEN h.diemchatkichthich >= 0 AND h.diemchatkichthich <= 3 THEN N'Mức nguy cơ: THẤP'       
                                 WHEN h.diemchatkichthich >= 4 AND h.diemchatkichthich <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
                                 WHEN h.diemchatkichthich >= 27 THEN N'Mức nguy cơ: CAO'                                    
                                 ELSE NULL                                                                                  
                             END AS MucDoNguyCoMaTuyDa,         
                             
                             CASE                                                                                       
				                     WHEN h.diemcansa >= 0 AND h.diemcansa <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemcansa >= 4 AND h.diemcansa <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemcansa >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoCanSa,            
		 
		                     CASE                                                                                       
				                     WHEN h.diemcoca >= 0 AND h.diemcoca <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemcoca >= 4 AND h.diemcoca <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemcoca >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoCoCain, 
		 
		                     CASE                                                                                       
				                     WHEN h.diemkhixong >= 0 AND h.diemkhixong <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemkhixong >= 4 AND h.diemkhixong <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemkhixong >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoKhiXong, 
		 
		                     CASE                                                                                       
				                     WHEN h.diemchatanthan >= 0 AND h.diemchatanthan <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemchatanthan >= 4 AND h.diemchatanthan <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemchatanthan >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoAnThan, 
		 
		                     CASE                                                                                       
				                     WHEN h.diemchatgayaogiac >= 0 AND h.diemchatgayaogiac <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemchatgayaogiac >= 4 AND h.diemchatgayaogiac <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemchatgayaogiac >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoGayAoGiac, 
		 
		                     CASE                                                                                       
				                     WHEN h.diemchatthuocphien >= 0 AND h.diemchatthuocphien <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemchatthuocphien >= 4 AND h.diemchatthuocphien <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemchatthuocphien >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoThuocPhien, 
		 
		                     CASE                                                                                       
				                     WHEN h.diemchatkhac >= 0 AND h.diemchatkhac <= 3 THEN N'Mức nguy cơ: THẤP'       
				                     WHEN h.diemchatkhac >= 4 AND h.diemchatkhac <= 26 THEN N'Mức nguy cơ: TRUNG BÌNH'
				                     WHEN h.diemchatkhac >= 27 THEN N'Mức nguy cơ: CAO'                                    
				                     ELSE NULL                                                                                  
		                     END AS MucDoNguyCoKhac, 
                                CASE                                                                                        
                                WHEN h.c_3 = 0 THEN N'Không'                                                                
                                WHEN h.c_3 = 1 THEN N'Có'                                                                   
                                WHEN h.c_3 = 2 THEN N'Không biết/Không trả lời'                                             
                                ELSE NULL                                                                                   
                            END AS QSTCoTuSat,                                                                              
		                        h.tongdiem AS QSTTongDiem,                                                                  
		                        h.diem AS ACESoLuong                                                                        
                            FROM                                                                          
                                 CD43_KHACH_HANG_THONG_TIN_CO_BAN t                                       
                                 LEFT JOIN CD43_KHACH_HANG_HANH_VI_NGUY_CO h ON t.record_id = h.record_id 
                           WHERE                                                                  
                            t.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = 2            
                            AND t.Id = " + id;

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {
                    Id = row.Field<int>("Id"),
                    RecordId = row.Field<string>("RecordId"),
                    DoiTuongQuanHe = row.Field<string>("DoiTuongQuanHe"),
                    TinhTrangMoiQHHT = row.Field<string>("TinhTrangMoiQHHT"),
                    TanSuatSuDungBCSChemsex3Thang = row.Field<string>("TanSuatSuDungBCSChemsex3Thang"),
                    QHTDTT3Thang = row.Field<string>("QHTDTT3Thang"),
                    BanDam3Thang = row.Field<string>("BanDam3Thang"),
                    STIs = row.Field<string>("STIs"),
                    TinhTrangViemGanC = row.Field<string>("TinhTrangViemGanC"),
                    MucDoNguyCoThuocLa = row.Field<string>("MucDoNguyCoThuocLa"),
                    MucDoNguyCoThucUong = row.Field<string>("MucDoNguyCoThucUong"),
                    MucDoNguyCoMaTuyDa = row.Field<string>("MucDoNguyCoMaTuyDa"),
                    MucDoNguyCoCanSa = row.Field<string>("MucDoNguyCoCanSa"),
                    MucDoNguyCoCoCain = row.Field<string>("MucDoNguyCoCoCain"),
                    MucDoNguyCoKhiXong = row.Field<string>("MucDoNguyCoKhiXong"),
                    MucDoNguyCoAnThan = row.Field<string>("MucDoNguyCoAnThan"),
                    MucDoNguyCoGayAoGiac = row.Field<string>("MucDoNguyCoGayAoGiac"),
                    MucDoNguyCoThuocPhien = row.Field<string>("MucDoNguyCoThuocPhien"),
                    MucDoNguyCoKhac = row.Field<string>("MucDoNguyCoKhac"),
                    QSTCoTuSat = row.Field<string>("QSTCoTuSat"),
                    QSTTongDiem = row.Field<string>("QSTTongDiem"),
                    ACESoLuong = row.Field<string>("ACESoLuong"),
                };
            }
            return customer;
        }

        public Customer GetDichVuChuyenGuiByCustomerId(int id, string recordid)
        {
            Customer customer = null;
            string query = @" SELECT 
                                cg.record_id AS RecordId,
                                CASE 
                                    WHEN cg.chuyengui___1 = '1' THEN N'Khám và điều trị sức khỏe tâm thần'
                                    WHEN cg.chuyengui___2 = '1' THEN N'Xét nghiệm khẳng định HIV và điều trị ARV'
                                    WHEN cg.chuyengui___3 = '1' THEN N'Xét nghiệm tải lượng virus HIV'
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___1 = '1' THEN 'STI'
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___2 = '1' THEN 'Viêm gan C'
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___3 = '1' THEN 'MMT'
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___4 = '1' THEN 'Lao'
                                    WHEN cg.chuyengui___5 = '1' THEN N'Hỗ trợ các dịch vụ y tế khác'
                                    WHEN cg.chuyengui___6 = '1' THEN N'Hỗ trợ các dịch vụ y tế khác'
                                END AS DichVuChuyenGui,
    
                                CASE 
                                    WHEN cg.chuyengui___1 = '1' THEN cg.f2_q_1_1
                                    WHEN cg.chuyengui___2 = '1' THEN cg.f2_q_2_1
                                    WHEN cg.chuyengui___3 = '1' THEN cg.f2_q_3_1_d
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___1 = '1' THEN cg.f2_q_4_1_1
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___2 = '1' THEN cg.f2_q_4_2_1
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___3 = '1' THEN cg.f2_q_4_3_1
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___4 = '1' THEN cg.f2_q_4_4_1
                                    WHEN cg.chuyengui___5 = '1' THEN cg.f2_q_5_d
                                    WHEN cg.chuyengui___6 = '1' THEN cg.f2_q_6_d
                                END AS NgayKham,
    
                                CASE 
                                    WHEN cg.chuyengui___1 = '1' THEN cg.f2_q_1_3
                                    WHEN cg.chuyengui___4 = '1' AND cg.loaihinh4___1 = '1' THEN cg.f2_q_4_1_3
                                    ELSE NULL
                                END AS LanKham
    
                            FROM 
                                CD43_KHACH_HANG_CHUYEN_GUI cg
                            WHERE 
                                cg.record_id = '" + @recordid + @"'
                            AND 
                                (
                                    cg.chuyengui___1 = '1' OR cg.chuyengui___2 = '1' OR cg.chuyengui___3 = '1' OR 
                                    cg.chuyengui___4 = '1' OR cg.chuyengui___5 = '1' OR cg.chuyengui___6 = '1'
                                );
 ";
            var result = _DatabaseSql.ExecuteTable(query);

            if (result.Rows.Count > 0)
            {
                customer = new Customer();
                customer.DichVuChuyenGuiList = new List<DichVuChuyenGui>();

                foreach (DataRow row in result.Rows)
                {
                    customer.DichVuChuyenGuiList.Add(new DichVuChuyenGui
                    {
                        DichVu = row.Field<string>("DichVuChuyenGui"),
                        NgayKham = row.Field<DateTime?>("NgayKham"),
                        LanKham = row.Field<string>("LanKham")
                    });
                }
            }
            return customer;
        }

        public Customer GetSinhHoatNhomByCustomerId(int id, string recordid)
        {
            Customer customer = null;
            string query = @"
                            SELECT 
                                record_id as RecordId,
                                CASE 
                                    WHEN COUNT(*) > 0 THEN 'Có'
                                    ELSE 'Không'
                                END AS ThamGiaSinhHoatNhom,
                                COUNT(*) AS SoLanThamGia
                            FROM 
                                CD43_KHACH_HANG_SINH_HOAT_NHOM
                            WHERE 
                                record_id = '" + recordid + @"'
                                AND ngay_shn IS NOT NULL
                                AND sinh_hot_nhm_complete = 2
                            GROUP BY record_id;
                             ";

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {   
                    RecordId = row.Field<string>("RecordId"),
                    ThamGiaSinhHoatNhom = row.Field<string>("ThamGiaSinhHoatNhom"),
                    SoLanThamGia = row.Field<int?>("SoLanThamGia"),

                };
            }
            return customer;
        }

        public Customer GetPhieuTuVanCustomerId(int id, string recordid)
        {
            Customer customer = null;
            string query = @"
                            SELECT 
	                            record_id as RecordId,
	                            COUNT(1) SoLanTuVan,
	                            tcv,
	                            v.hovaten AS TuVanVien
                            FROM CD43_KHACH_HANG_PHIEU_TU_VAN t
                            JOIN TCV_TU_VAN_VIEN v ON t.tcv = v.id
                            WHERE t.ngaytuvan is not null
                            AND t.phiu_t_vn_complete = 2
                            AND t.record_id = '"+@recordid+@"'
                            GROUP BY record_id, tcv, v.hovaten;
                             ";

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {   
                    RecordId = row.Field<string>("RecordId"),
                    TuVanVien = row.Field<string>("TuVanVien"),
                    SoLanTuVan = row.Field<int?>("SoLanTuVan"),
                };
            }
            return customer;
        }

        public List<PhieuTuVan> GetListPhieuTuVanCustomerId(int id, string recordid)
        {
            var listPhieuTuVan = new List<PhieuTuVan>();
            string query = @" SELECT 
                                    record_id as RecordId,
                                    NgayTuVan,		
                                    -- Nhóm I: Hành vi nguy cơ
                                    RTRIM(STUFF(CONCAT(
																				COALESCE (CASE WHEN cau2___1 = 1 THEN N'; Sử dụng ma túy không an toàn' ELSE NULL END, ''),
																						CASE WHEN cau2___2 = 1 THEN N'; Tiêm chích' ELSE NULL END,
																						CASE WHEN cau2___3 = 1 THEN N'; QHTD không an toàn' ELSE NULL END,
																						CASE WHEN cau2___4 = 1 THEN N'; QHTD và sử dụng ma túy' ELSE NULL END,
																						CASE WHEN cau2___5 = 1 THEN N'; Sử dụng đa chất' ELSE NULL END,
																						CASE WHEN cau2___6 = 1 THEN N'; Khác' ELSE NULL END,
																						CASE WHEN cau2___7 = 1 THEN N'; Chưa/không khai thác được hành vi nguy cơ gì' ELSE NULL END																						
																				),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS HanhViNguyCo,

                                    -- Nhóm II: Sức khỏe thể chất
                                    RTRIM(STUFF(CONCAT(
																				COALESCE(CASE WHEN cau3___1 = 1 THEN N'; Mệt mỏi' ELSE NULL END, ''),
																						CASE WHEN cau3___2 = 1 THEN N'; Sụt cân' ELSE NULL END,
																						CASE WHEN cau3___3 = 1 THEN N'; Nhiễm HIV' ELSE NULL END,
																						CASE WHEN cau3___4 = 1 THEN N'; VGC' ELSE NULL END,
																						CASE WHEN cau3___5 = 1 THEN N'; VGB' ELSE NULL END,
																						CASE WHEN cau3___6 = 1 THEN N'; Lao' ELSE NULL END,
																						CASE WHEN cau3___7 = 1 THEN N'; Mất ngủ' ELSE NULL END,
																						CASE WHEN cau3___8 = 1 THEN N'; STIs' ELSE NULL END,
																						CASE WHEN cau3___9 = 1 THEN N'; Khác' ELSE NULL END,
																						CASE WHEN cau3___10 = 1 THEN N'; Sức khỏe sinh sản' ELSE NULL END,
																						CASE WHEN cau3___11 = 1 THEN N'; Chưa/không khai thác được hành vi nguy cơ gì' ELSE NULL END,
																						CASE WHEN cau3___12 = 1 THEN N'; Không gặp vấn đề gì' ELSE NULL END
																			),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS SucKhoeTheChat,


                                    -- Nhóm III: Sức khỏe tâm thần
                                    RTRIM(STUFF(CONCAT(
																				COALESCE(CASE WHEN cau4___1 = 1 THEN N'; Ảo giác' ELSE NULL END, ''),
																						CASE WHEN cau4___2 = 1 THEN N'; Hoang tưởng' ELSE NULL END,
																						CASE WHEN cau4___3 = 1 THEN N'; Rối loạn suy nghĩ' ELSE NULL END,
																						CASE WHEN cau4___4 = 1 THEN N'; Trầm cảm' ELSE NULL END,
																						CASE WHEN cau4___5 = 1 THEN N'; Lo âu' ELSE NULL END,
																						CASE WHEN cau4___6 = 1 THEN N'; Ý định tự tử' ELSE NULL END,
																						CASE WHEN cau4___7 = 1 THEN N'; Cơn hoảng loạn' ELSE NULL END,
																						CASE WHEN cau4___8 = 1 THEN N'; Hành vi tự hại' ELSE NULL END,
																						CASE WHEN cau4___9 = 1 THEN N'; Hội chứng cai' ELSE NULL END,
																						CASE WHEN cau4___10 = 1 THEN N'; Khác' ELSE NULL END,
																						CASE WHEN cau4___11 = 1 THEN N'; Chưa/không khai thác được hành vi nguy cơ gì' ELSE NULL END,
																						CASE WHEN cau4___12 = 1 THEN N'; Không gặp vấn đề gì' ELSE NULL END
																				),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS SucKhoeTamThan,


		                                -- Nhóm IV: Sức khỏe tình dục
                                    RTRIM(STUFF(CONCAT(
																				COALESCE(CASE WHEN cau_5___1 = 1 THEN N'; Lệ thuộc vào chất để QHTD' ELSE NULL END, ''),
																						CASE WHEN cau_5___2 = 1 THEN N'; Hoang tưởng' ELSE NULL END,
																						CASE WHEN cau_5___3 = 1 THEN N'; Phá vỡ các giới hạn' ELSE NULL END,
																						CASE WHEN cau_5___4 = 1 THEN N'; Không có vấn đề' ELSE NULL END,
																						CASE WHEN cau_5___6 = 1 THEN N'; Chưa/không khai thác được vấn đề gì trong lần tư vấn này' ELSE NULL END,
																						CASE WHEN cau_5___5 = 1 THEN N'; Khác' ELSE NULL END
																			),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS SucKhoeTinhDuc,

		                                -- Nhóm V: Tư vấn giảm hại
																		RTRIM(
																				STUFF(CONCAT(
																						COALESCE(CASE WHEN cau6_1___1 = 1 THEN N'; Ăn, uống, ngủ, lặp lại' ELSE NULL END, ''),
																						CASE WHEN cau6_1___2 = 1 THEN N'; Giảm hại về cách thức sử dụng, đường sử dụng' ELSE NULL END,
																						CASE WHEN cau6_1___3 = 1 THEN N'; Tình dục an toàn (sử dụng bao cao su, gel bôi trơn, dung dịch vệ sinh đồ chơi tình dục ...)' ELSE NULL END,
																						CASE WHEN cau6_1___4 = 1 THEN N'; Vệ sinh cá nhân răng miệng' ELSE NULL END,
																						CASE WHEN cau6_1___5 = 1 THEN N'; Khác' ELSE NULL END,
																						CASE WHEN cau6_1___6 = 1 THEN N'; Không cung cấp can thiệp nào (trong buổi tư vấn này)' ELSE NULL END
																				),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS TuVanGiamHai,

		
		                                -- Nhóm VI: Hỗ trợ về SKTT
                                    RTRIM(STUFF(CONCAT(
                                        COALESCE (CASE WHEN cau6_2___6	 = 1 THEN N'; Can thiệp cơ bản về SKTT (giải thích về SKTT, giảm sự kỳ thị hoặc nhận diện các vấn đề SKTT ...)' ELSE NULL END, ''),
																						CASE WHEN cau6_2___7	 = 1 THEN N'; Tự làm dịu' ELSE NULL END,
						                                CASE WHEN cau6_2___8	 = 1 THEN N'; Lòng biết ơn' ELSE NULL END,
						                                CASE WHEN cau6_2___9	 = 1 THEN N'; Nhóm tự lực' ELSE NULL END,
						                                CASE WHEN cau6_2___10 = 1 THEN N'; Thiền' ELSE NULL END,
						                                CASE WHEN cau6_2___11 = 1 THEN N'; Tái định hình nhận thức' ELSE NULL END,
						                                CASE WHEN cau6_2___12 = 1 THEN N'; Thang đo mức độ tồi tệ' ELSE NULL END,
						                                CASE WHEN cau6_2___15 = 1 THEN N'; Nhu cầu dài hạn chung' ELSE NULL END,
						                                CASE WHEN cau6_2___13 = 1 THEN N'; Khác' ELSE NULL END,
						                                CASE WHEN cau6_2___14 = 1 THEN N'; Không cung cấp can thiệp nào (trong buổi tư vấn này)' ELSE NULL END                                        
																				) ,
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS HoTroSKTT,	
																		
		                                -- Nhóm VII: Hỗ trợ về SKTD
                                    RTRIM(STUFF(CONCAT(
                                        COALESCE (CASE WHEN cau6_3_bs___1 = 1 THEN	N'; Can thiệp cơ bản về SKTD (giải thích về Chemsex, các nguy cơ liên quan Chemsex,...)' ELSE NULL END, ''),
						                            CASE WHEN cau6_3_bs___2 = 1 THEN	N'; Can thiệp trước Chemsex' ELSE NULL END,
						                            CASE WHEN cau6_3_bs___3 = 1 THEN	N'; Can thiệp trong Chemsex' ELSE NULL END,
						                            CASE WHEN cau6_3_bs___4 = 1 THEN	N'; Can thiệp sau Chemsex' ELSE NULL END,
						                            CASE WHEN cau6_3_bs___5 = 1 THEN	N'; Khác' ELSE NULL END,
						                            CASE WHEN cau6_3_bs___6 = 1 THEN	N'; Không cung cấp can thiệp nào (trong buổi tư vấn này)' ELSE NULL END
                                       
																			),
																					1, 2, '' -- Xóa dấu phân cách ""; "" đầu tiên nếu nó tồn tại
																			)
																		) AS HoTroSKTD,		
		                            thoigian AS ThoiGianTuVanTiep		
                                FROM 
                                    CD43_KHACH_HANG_PHIEU_TU_VAN
                                WHERE record_id = '" + @recordid + @"'
                                
                                ORDER BY 
                                    record_id, ngaytuvan;
                                ";

            var result = _DatabaseSql.ExecuteTable(query);

            if (result.Rows.Count > 0)
            {
                
                foreach (DataRow row in result.Rows)
                {
                    listPhieuTuVan.Add(new PhieuTuVan
                    {
                        RecordId = row.Field<string>("RecordId"),
                        NgayTuVan = row.Field<DateTime?>("NgayTuVan"),
                        ThoiGianTuVanTiep = row.Field<DateTime?>("ThoiGianTuVanTiep"),
                        HanhViNguyCo = row.Field<string>("HanhViNguyCo"),
                        SucKhoeTheChat = row.Field<string>("SucKhoeTheChat"),
                        SucKhoeTamThan = row.Field<string>("SucKhoeTamThan"),
                        SucKhoeTinhDuc = row.Field<string>("SucKhoeTinhDuc"),
                        TuVanGiamHai = row.Field<string>("TuVanGiamHai"),
                        HoTroSKTT = row.Field<string>("HoTroSKTT"),
                        HoTroSKTD = row.Field<string>("HoTroSKTD"),
                    });
                }
            }
            return listPhieuTuVan;
        }

        public List<KhamDieuTriSKTT> GetListKhamVaDieuTriSKTTCustomerId(int id, string recordid)
        {
            var listKhamDieuTriSKTT = new List<KhamDieuTriSKTT>();
            string query = @"   SELECT 
                                    record_id as RecordId,
                                    f2_q_1_1 as NgayKham,		
                                    f2_q_1_3_1 AS TrieuChung,
		                            f2_q_1_4 AS ChanDoan,
		                            CASE WHEN f2_q_1_5 = 1 THEN N'Có'
				                            ELSE N'Không'
		                            END AS KeDon,		
		                            CASE WHEN f2_q_1_6 = 1 THEN N'Có'
				                            ELSE N'Không'
		                            END AS DungTheoDon,
		                            f2_q_1_8_1 AS NgayHenTaiKham
                                FROM CD43_KHACH_HANG_CHUYEN_GUI
                                WHERE 
                                chuyn_gi_complete = 2
                                AND chuyengui___1 = 1
                                AND  record_id = '"+@recordid+@"'
                                ";

            var result = _DatabaseSql.ExecuteTable(query);

            if (result.Rows.Count > 0)
            {

                foreach (DataRow row in result.Rows)
                {
                    listKhamDieuTriSKTT.Add(new KhamDieuTriSKTT
                    {
                        RecordId = row.Field<string>("RecordId"),
                        NgayKham = row.Field<DateTime?>("NgayKham"),
                        NgayHenTaiKham = row.Field<DateTime?>("NgayHenTaiKham"),
                        TrieuChung = row.Field<string>("TrieuChung"),
                        ChanDoan = row.Field<string>("ChanDoan"),
                        KeDon = row.Field<string>("KeDon"),
                        DungTheoDon = row.Field<string>("DungTheoDon"),
                        
                    });
                }
            }
            return listKhamDieuTriSKTT;
        }

        public Customer GetKhamVaDieuTriCustomerId(int id, string recordid)
        {
            Customer customer = null;
            string query = @"
                            SELECT 
                                record_id as RecordId,
                                COUNT(1) SoLanKham
                            FROM CD43_KHACH_HANG_CHUYEN_GUI t
                            WHERE chuyn_gi_complete = 2
                            AND chuyengui___1 = 1
                            AND  record_id = '"+@recordid+@"'
                            GROUP BY record_id;
                             ";

            var result = _DatabaseSql.ExecuteTable(query);
            if (result.Rows.Count > 0)
            {
                // Tạo đối tượng Customer và gán các giá trị từ DataTable
                DataRow row = result.Rows[0];
                customer = new Customer
                {
                    RecordId = row.Field<string>("RecordId"),
                    SoLanKham = row.Field<int?>("SoLanKham"),
                };
            }
            return customer;
        }

        public List<CustomerPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<CustomerPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("MaDuAn", string.IsNullOrEmpty(modelSearch.MaDuAn) ? DBNull.Value : (object)modelSearch.MaDuAn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<CustomerPageModel>(Constants.SP_Customer_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "CustomerDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách khách hàng theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<CustomerPageModel>();
            }
            return result;
        }

        public List<BVTL_KHACH_HANG> GetAll()
        {
            return db.BVTL_KHACH_HANG.ToList();
        }


        public ObjectMessage Add(BVTL_KHACH_HANG Customer)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_KHACH_HANG.Add(Customer);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        
        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var itemDelete = db.BVTL_KHACH_HANG.Find(Id);
                db.BVTL_KHACH_HANG.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
