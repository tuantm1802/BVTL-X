using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.API
{
    public class ResultApiKhachHangTTCBModel
    {
        public string record_id { get; set; }        
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string f1_q_a1 { get; set; } //ngay thang nam sinh
        public int nam_sinh { get; set; }           
        public string f1_q_a2 { get; set; } //gioi tinh
        public string f1_q_a2_1 { get; set; } //gioi_tinh_khac
        public string f1_q_a4 { get; set; } //cap_bac_hoc_van
        public string f1_q_a6{ get; set; } //nghe_nghiep 
        public string f1_q_a6_1 { get; set; } //nghe_nghiep_khac 
        public string time { get; set; } //thoi_gian_bat_dau
        public string end_time  { get; set; } //thoi_gian_ket_thuc
        public string city_code { get; set; }
        public string thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete { get; set; }


    }

    public class ResultApiKhachHangSangLocNuocTieuModel
    {
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string ngayhoi { get; set; } //ngay hoi
        public string kqxnda { get; set; }
        public string city_code { get; set; }
        public string sng_lc_nc_tiu_complete { get; set; }


    }

    public class ResultApiKhachHangSangLocHIVModel
    {
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string ngayhoi { get; set; } 
        public string hiv { get; set; } 
        public string dt_hiv { get; set; }
        public string kqxn { get; set; }
        public string city_code { get; set; }
        public string sng_lc_hiv_complete { get; set; }


    }

    public class ResultApiKhachHangDanhGiaHaiLongModel
    {
        public string record_id { get; set; }
        public string redcap_repeat_instrument { get; set; }
        public int? redcap_repeat_instance { get; set; }
        public DateTime? ngay_3db24b { get; set; }
        public int? dichvu { get; set; }
        public string dv_khac { get; set; }
        public int? cau1 { get; set; }
        public int? cau2_b127b4 { get; set; }
        public int? cau3_eaf641 { get; set; }
        public int? cau4_60e585 { get; set; }
        public int? cau5 { get; set; }
        public string cau6 { get; set; }
        public string kenh___1 { get; set; }
        public string kenh___2 { get; set; }
        public string kenh___3 { get; set; }
        public string kenh___4 { get; set; }
        public string khac_1 { get; set; }
        public string thongtin___1 { get; set; }
        public string thongtin___2 { get; set; }
        public string thongtin___3 { get; set; }
        public string thongtin___4 { get; set; }
        public string thongtin___5 { get; set; }
        public string thongtin___6 { get; set; }
        public string tt_khac { get; set; }
        public string chiase { get; set; }
        public int? nh_gi_mc_hi_lng_ca_kh_complete { get; set; }


    }

    public class ResultApiKhachHangTheoDauModel
    {
        public long id { get; set; } // Trường id là kiểu bigint và tự tăng

        public string record_id { get; set; } // record_id là kiểu nvarchar(20)

        public string redcap_repeat_instrument { get; set; } // redcap_repeat_instrument là kiểu nvarchar(100)

        public int? redcap_repeat_instance { get; set; } // redcap_repeat_instance là kiểu int

        public DateTime? time_theodau { get; set; } // time_theodau là kiểu datetime

        public string kq_theodau { get; set; } // kq_theodau là kiểu nvarchar(255)

        public string lydo_matdau { get; set; } // lydo_matdau là kiểu nvarchar(255)

        public string matdau_khac { get; set; } // matdau_khac là kiểu nvarchar(255)

        public int? theo_du_complete { get; set; } // theo_du_complete là kiểu int

        public DateTime? sync_date { get; set; } // sync_date là kiểu datetime với giá trị mặc định là GETDATE()

        public string maduan { get; set; } // maduan là kiểu varchar(50)

        public string manhom_tbh { get; set; } // manhom_tbh là kiểu varchar(5)

        public string city_code { get; set; } // city_code là kiểu varchar(10)
    }

    public class ResultApiKhachHangSinhHoatNhomModel
    {
        public long id { get; set; } // Primary key, auto-incrementing

        public string record_id { get; set; } // record_id is nvarchar(20)

        public string redcap_repeat_instrument { get; set; } // redcap_repeat_instrument is nvarchar(100)

        public int? redcap_repeat_instance { get; set; } // redcap_repeat_instance is int

        public DateTime? ngay_shn { get; set; } // ngay_shn is date

        public string chude { get; set; } // chude is nvarchar(255)

        public string phatvatpham { get; set; } // phatvatpham is nvarchar(255)

        public string bcs { get; set; } // bcs is nvarchar(50)

        public string gel { get; set; } // gel is nvarchar(50)

        public int? sinh_hot_nhm_complete { get; set; } // sinh_hot_nhm_complete is int

        public DateTime? sync_date { get; set; } // sync_date is datetime, default is GETDATE()

        public string maduan { get; set; } // maduan is varchar(50)

        public string manhom_tbh { get; set; } // manhom_tbh is varchar(5)

        public string city_code { get; set; } // city_code is varchar(10)
    }

    public class ResultApiKhachHangPhieuTuVanModel
    {
        public long id { get; set; } // Primary key, auto-incrementing
        public string record_id { get; set; }
        public string redcap_repeat_instrument { get; set; }
        public string redcap_repeat_instance { get; set; }
        public DateTime? ngaytuvan { get; set; }
        public string diadiem { get; set; }
        public string tcv { get; set; }
        public string lantuvan { get; set; }
        public string cau1_1___1 { get; set; }
        public string cau1_1___2 { get; set; }
        public string cau1_1___3 { get; set; }
        public string cau1_1___4 { get; set; }
        public string cau1_1___5 { get; set; }
        public string cau1_1___6 { get; set; }
        public string cau1_1___7 { get; set; }
        public string cau1_1k { get; set; }
        public string cau1_2 { get; set; }
        public string cau1_3___8 { get; set; }
        public string cau1_3___9 { get; set; }
        public string cau1_3___10 { get; set; }
        public string cau1_3___11 { get; set; }
        public string cau_1_4_1___1 { get; set; }
        public string cau_1_4_1___2 { get; set; }
        public string cau_1_4_1___3 { get; set; }
        public string cau_1_4_1___4 { get; set; }
        public string cau_1_4_1___5 { get; set; }
        public string cau_1_4_1___6 { get; set; }
        public string cau_1_4_1___7 { get; set; }
        public string cau_1_4_2 { get; set; }
        public string cau_1_4_1_mt { get; set; }
        public string cau_1_5_2 { get; set; }
        public string cau_1_5_3 { get; set; }
        public string cau_1_5_4 { get; set; }
        public string cau_1_5_5 { get; set; }
        public string cau_1_5_6 { get; set; }
        public string cau2___1 { get; set; }
        public string cau2___2 { get; set; }
        public string cau2___3 { get; set; }
        public string cau2___4 { get; set; }
        public string cau2___5 { get; set; }
        public string cau2___6 { get; set; }
        public string cau2___7 { get; set; }
        public string cau2_1k { get; set; }
        public string cau2_2 { get; set; }
        public string cau3___1 { get; set; }
        public string cau3___2 { get; set; }
        public string cau3___3 { get; set; }
        public string cau3___4 { get; set; }
        public string cau3___5 { get; set; }
        public string cau3___6 { get; set; }
        public string cau3___7 { get; set; }
        public string cau3___8 { get; set; }
        public string cau3___10 { get; set; }
        public string cau3___9 { get; set; }
        public string cau3___11 { get; set; }
        public string cau3___12 { get; set; }
        public string cau3_1k { get; set; }
        public string cau3_1k_2 { get; set; }
        public string cau4___1 { get; set; }
        public string cau4___2 { get; set; }
        public string cau4___3 { get; set; }
        public string cau4___4 { get; set; }
        public string cau4___5 { get; set; }
        public string cau4___6 { get; set; }
        public string cau4___7 { get; set; }
        public string cau4___8 { get; set; }
        public string cau4___9 { get; set; }
        public string cau4___10 { get; set; }
        public string cau4___11 { get; set; }
        public string cau4___12 { get; set; }
        public string cau4_1k { get; set; }
        public string cau4_1k_2 { get; set; }
        public string cau_5___1 { get; set; }
        public string cau_5___2 { get; set; }
        public string cau_5___3 { get; set; }
        public string cau_5___4 { get; set; }
        public string cau_5___6 { get; set; }
        public string cau_5___5 { get; set; }
        public string sktd_khac { get; set; }
        public string cau6_1___1 { get; set; }
        public string cau6_1___2 { get; set; }
        public string cau6_1___3 { get; set; }
        public string cau6_1___4 { get; set; }
        public string cau6_1___5 { get; set; }
        public string cau6_1___6 { get; set; }
        public string cau6_1k { get; set; }
        public string cau6_1_2 { get; set; }
        public string cau6_2___6 { get; set; }
        public string cau6_2___7 { get; set; }
        public string cau6_2___8 { get; set; }
        public string cau6_2___9 { get; set; }
        public string cau6_2___10 { get; set; }
        public string cau6_2___11 { get; set; }
        public string cau6_2___12 { get; set; }
        public string cau6_2___15 { get; set; }
        public string cau6_2___13 { get; set; }
        public string cau6_2___14 { get; set; }
        public string cau6_2k { get; set; }
        public string cau6_2k_2 { get; set; }
        public string cau6_3_bs___1 { get; set; }
        public string cau6_3_bs___2 { get; set; }
        public string cau6_3_bs___3 { get; set; }
        public string cau6_3_bs___4 { get; set; }
        public string cau6_3_bs___5 { get; set; }
        public string cau6_3_bs___6 { get; set; }
        public string cau6_3_bs_1 { get; set; }
        public string cau6_3_bs_2 { get; set; }
        public string cau6_3 { get; set; }
        public string cau6_3_1 { get; set; }
        public string cau6_3_2 { get; set; }
        public string cau6_4 { get; set; }
        public string cau6_4_1 { get; set; }
        public string cau6_5 { get; set; }
        public string cau6_5_1 { get; set; }
        public string tuvantiep { get; set; }
        public string vande { get; set; }
        public string thoigian { get; set; }
        public DateTime? sync_date { get; set; } // sync_date is datetime, default is GETDATE()

        public string maduan { get; set; } // maduan is varchar(50)

        public string manhom_tbh { get; set; } // manhom_tbh is varchar(5)

        public string city_code { get; set; } // city_code is varchar(10)
        public string phiu_t_vn_complete { get; set; }
    }

    public class ResultApiKhachHangChuyenGuiModel
    {
        public string record_id { get; set; }
        public DateTime? sync_date { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
        public string chuyengui___1 { get; set; }
        public string chuyengui___2 { get; set; }
        public string chuyengui___3 { get; set; }
        public string chuyengui___4 { get; set; }
        public string chuyengui___5 { get; set; }
        public string chuyengui___6 { get; set; }
        public string p_1 { get; set; }
        public DateTime? f2_q_1_1 { get; set; }
        public string f2_q_1_2 { get; set; }
        public string f2_q_1_3 { get; set; }
        public string f2_q_1_3_1 { get; set; }
        public string f2_q_1_4 { get; set; }
        public string f2_q_1_5 { get; set; }
        public string f2_q_1_5_1 { get; set; }
        public string f2_q_1_6 { get; set; }
        public string f2_q_1_8 { get; set; }
        public DateTime? f2_q_1_8_1 { get; set; }
        public string p_2 { get; set; }
        public DateTime? f2_q_2_1 { get; set; }
        public string f2_q_2_2 { get; set; }
        public string f2_q_2_3 { get; set; }
        public string f2_q_2_4 { get; set; }
        public string f2_q_2_5 { get; set; }
        public DateTime? f2_q_2_6 { get; set; }
        public string p_3 { get; set; }
        public string f2_q_3_1 { get; set; }
        public string f2_q_3_1_1 { get; set; }
        public string f2_q_3_1_1_1 { get; set; }
        public string f2_q_3_1_2 { get; set; }
        public string f2_q_3_1_1_2 { get; set; }
        public string f2_q_3_1_3 { get; set; }
        public string f2_q_3_1_3_1 { get; set; }
        public string f2_q_3_2 { get; set; }
        public string f2_q_3_3 { get; set; }
        public string p_4 { get; set; }
        public string loaihinh4 { get; set; }
        public string loaihinh4___1 { get; set; }
        public string loaihinh4___2 { get; set; }
        public string loaihinh4___3 { get; set; }
        public string loaihinh4___4 { get; set; }
        public string f2_q_4_1 { get; set; }
        public string f2_q_4_1_1 { get; set; }
        public string f2_q_4_1_2 { get; set; }
        public string f2_q_4_1_3 { get; set; }
        public string f2_q_4_1_4 { get; set; }
        public string f2_q_4_1_4___1 { get; set; }
        public string f2_q_4_1_4___2 { get; set; }
        public string f2_q_4_1_4___3 { get; set; }
        public string f2_q_4_1_4___4 { get; set; }
        public string f2_q_4_1_4___5 { get; set; }
        public string f2_q_4_1_4___6 { get; set; }
        public string f2_q_4_1_4___7 { get; set; }
        public string f2_q_4_1_4_1 { get; set; }
        public string f2_q_4_1_5 { get; set; }
        public string f2_q_4_1_6 { get; set; }
        public string f2_q_4_1_7 { get; set; }
        public string f2_q_4_2 { get; set; }
        public DateTime? f2_q_4_2_1 { get; set; }
        public string f2_q_4_2_2 { get; set; }
        public string f2_q_4_2_3 { get; set; }
        public string f2_q_4_2_4 { get; set; }
        public string f2_q_4_2_5 { get; set; }
        public string f2_q_4_2_6 { get; set; }
        public string f2_q_4_3 { get; set; }
        public DateTime? f2_q_4_3_1 { get; set; }
        public string f2_q_4_3_2 { get; set; }
        public string f2_q_4_3_3 { get; set; }
        public string f2_q_4_3_4 { get; set; }
        public string f2_q_4_4 { get; set; }
        public DateTime? f2_q_4_4_1 { get; set; }
        public string f2_q_4_4_2 { get; set; }
        public string f2_q_4_4_2_1 { get; set; }
        public string f2_q_4_4_3 { get; set; }
        public string f2_q_4_4_3_1 { get; set; }
        public string f2_q_4_4_4 { get; set; }
        public string p_5 { get; set; }
        public string f2_q_5 { get; set; }
        public DateTime? f2_q_5_d { get; set; }
        public string f2_q_51 { get; set; }
        public string f2_q_5_1 { get; set; }
        public string f2_q_53 { get; set; }
        public string f2_q_5_2 { get; set; }
        public string p_6 { get; set; }
        public DateTime? f2_q_6_d { get; set; }
        public string f2_q_6_1 { get; set; }
        public string f2_q_6_2 { get; set; }
        public string f2_q_6_3 { get; set; }
        public string f2_q_6_4 { get; set; }
        public string f2_q_6_5 { get; set; }
        public string f2_q_6_6 { get; set; }
        public string f2_q_6_7 { get; set; }
        public string chuyn_gi_complete { get; set; }
    }

    public class ResultApiKhachHangHanhViNguyCoModel
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public DateTime? sync_date { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string ngayhoi { get; set; }
        public string time { get; set; }
        public string end_time { get; set; }
        public string city_code { get; set; }
        public string f1_q_a3 { get; set; }
        public string f1_q_a3_1 { get; set; }
        public string f1_q_a4 { get; set; }
        public string f1_q_a5 { get; set; }
        public string f1_q_a5___1 { get; set; }
        public string f1_q_a5___2 { get; set; }
        public string f1_q_a5___3 { get; set; }
        public string f1_q_a6 { get; set; }
        public string f1_q_a6_1 { get; set; }
        public string f1_q_b1 { get; set; }
        public string f1_q_b1_1 { get; set; }
        public string f1_q_b2 { get; set; }
        public string f1_q_b2_1 { get; set; }
        public string f1_q_b3 { get; set; }
        public string f1_q_b3_1 { get; set; }
        public string f1_q_b4 { get; set; }
        public string f1_q_b4_1 { get; set; }
        public string f1_q_b5 { get; set; }
        public string f1_q_b5_1 { get; set; }
        public string f1_q_b6 { get; set; }
        public string f1_q_b7 { get; set; }
        public string f1_q_b8 { get; set; }
        public string f1_q_b8_1 { get; set; }
        public string f1_q_b9 { get; set; }
        public string f1_q_b10 { get; set; }
        public string f1_q_b11 { get; set; }
        public string f1_q_b12 { get; set; }
        public string f1_q_b13 { get; set; }
        public string f1_q_b14 { get; set; }
        public string f1_q_b14_1 { get; set; }
        public string f1_q_b15 { get; set; }
        public string f1_q_b16 { get; set; }
        public string f1_q_b17 { get; set; }
        public string assist_mota { get; set; }
        public string thuocla { get; set; }
        public string thucuong { get; set; }
        public string cansa { get; set; }
        public string cocain { get; set; }
        public string chatkichthich { get; set; }
        public string khixong { get; set; }
        public string thuocanthan { get; set; }
        public string chatgayaogiac { get; set; }
        public string thuocphien { get; set; }
        public string chatkhac { get; set; }
        public string cacchatkhac { get; set; }
        public string thuocla1 { get; set; }
        public string thucuong1 { get; set; }
        public string cansa1 { get; set; }
        public string coca1 { get; set; }
        public string chatkichthich1 { get; set; }
        public string khixong1 { get; set; }
        public string thuocanthan1 { get; set; }
        public string chatgayaogiac1 { get; set; }
        public string chatthuocphien1 { get; set; }
        public string chatkhac1 { get; set; }
        public string thuocla2 { get; set; }
        public string thucuong2 { get; set; }
        public string cansa2 { get; set; }
        public string coca2 { get; set; }
        public string chatkichthich2 { get; set; }
        public string khixong2 { get; set; }
        public string thuocanthan2 { get; set; }
        public string chatgayaogiac2 { get; set; }
        public string chatthuocphien2 { get; set; }
        public string chatkhac2 { get; set; }
        public string thuocla3 { get; set; }
        public string thucuong3 { get; set; }
        public string cansa3 { get; set; }
        public string coca3 { get; set; }
        public string chatkichthich3 { get; set; }
        public string khixong3 { get; set; }
        public string thuocanthan3 { get; set; }
        public string chatgayaogiac3 { get; set; }
        public string chatthuocphien3 { get; set; }
        public string chatkhac3 { get; set; }
        public string thucuong4 { get; set; }
        public string cansa4 { get; set; }
        public string coca4 { get; set; }
        public string chatkichthich4 { get; set; }
        public string khixong4 { get; set; }
        public string thuocanthan4 { get; set; }
        public string chatgayaogiac4 { get; set; }
        public string chatthuocphien4 { get; set; }
        public string chatkhac4 { get; set; }
        public string thuocla5 { get; set; }
        public string thucuong5 { get; set; }
        public string cansa5 { get; set; }
        public string coca5 { get; set; }
        public string chatkichthich5 { get; set; }
        public string khixong5 { get; set; }
        public string thuocanthan5 { get; set; }
        public string chatgayaogiac5 { get; set; }
        public string chatthuocphien5 { get; set; }
        public string chatkhac5 { get; set; }
        public string thuocla6 { get; set; }
        public string thucuong6 { get; set; }
        public string cansa6 { get; set; }
        public string coca6 { get; set; }
        public string chatkichthich6 { get; set; }
        public string khixong6 { get; set; }
        public string thuocanthan6 { get; set; }
        public string chatgayaogiac6 { get; set; }
        public string chatthuocphien6 { get; set; }
        public string chatkhac6 { get; set; }
        public string cau_8 { get; set; }
        public string diemthuocla { get; set; }
        public string diemthucuong { get; set; }
        public string diemcansa { get; set; }
        public string diemcoca { get; set; }
        public string diemchatkichthich { get; set; }
        public string diemkhixong { get; set; }
        public string diemchatanthan { get; set; }
        public string diemchatgayaogiac { get; set; }
        public string diemchatthuocphien { get; set; }
        public string diemchatkhac { get; set; }
        public string c_1a { get; set; }
        public string c_1b { get; set; }
        public string c_1c { get; set; }
        public string c_1d { get; set; }
        public string c_2 { get; set; }
        public string c_3 { get; set; }
        public string c_4a { get; set; }
        public string c_4b { get; set; }
        public string c_4c { get; set; }
        public string tongdiem { get; set; }
        public string duongtinh { get; set; }
        public string amtinh { get; set; }
        public string c1 { get; set; }
        public string c2 { get; set; }
        public string c3 { get; set; }
        public string c4 { get; set; }
        public string c5 { get; set; }
        public string c6 { get; set; }
        public string c7 { get; set; }
        public string c8 { get; set; }
        public string c9 { get; set; }
        public string c10 { get; set; }
        public string diem { get; set; }
        public string thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete { get; set; }
    }

}
