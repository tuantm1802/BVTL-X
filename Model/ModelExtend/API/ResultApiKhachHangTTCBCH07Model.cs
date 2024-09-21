using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.API
{
    public class ResultApiKhachHangTTCBCH07Model
    {
        public int id { get; set; } // ID auto increment

        public string record_id { get; set; } // Mã định danh khách hàng

        public string redcap_repeat_instrument { get; set; } // Nhạc cụ lặp lại (nullable)

        public int? redcap_repeat_instance { get; set; } // Số lần lặp lại (nullable)

        public DateTime? ngaynhap { get; set; } // Ngày nhập thông tin (nullable)

        public string ngay { get; set; } // Ngày khác liên quan đến đối tượng (nullable)

        public int? doituong { get; set; } // Đối tượng (nullable)

        public int? gioitinh { get; set; } // Giới tính (nullable)

        public int? namsinh { get; set; } // Năm sinh (nullable)

        public int? tuoi { get; set; } // Tuổi (nullable)

        public int? thng_tin_c_bn_assist_qst_kin_thc_complete { get; set; } // Trạng thái hoàn thành thông tin cơ bản

        public DateTime? sync_date { get; set; } = DateTime.Now; // Ngày đồng bộ (nullable, default là ngày hiện tại)

        public string maduan { get; set; } // Mã dự án (nullable)

        public string manhom_tbh { get; set; } // Mã nhóm TBH (nullable)

        public string city_code { get; set; } // Mã thành phố (nullable)
    }


    public class ResultApiKhachHangSangLocNuocTieuCH07Model
    {
        public int id { get; set; } // Khoá chính tự động tăng
        public string record_id { get; set; } // Mã định danh khách hàng
        public string redcap_repeat_instrument { get; set; } // Nhạc cụ lặp lại
        public int? redcap_repeat_instance { get; set; } // Số lần lặp lại (nullable)
        public string ngayhoi { get; set; } // Ngày hỏi (nullable)
        public int? kqxnda { get; set; } // Kết quả xét nghiệm DNA (nullable)
        public int? kqxnheroin { get; set; } // Kết quả xét nghiệm Heroin (nullable)
        public int? sng_lc_nc_tiu_complete { get; set; } // Trạng thái hoàn thành xét nghiệm sàng lọc nước tiểu (nullable)
        public string maduan { get; set; } // Mã dự án
        public string manhom_tbh { get; set; } // Mã nhóm TBH
        public string city_code { get; set; } // Mã thành phố
        public DateTime? sync_date { get; set; } // Ngày đồng bộ (nullable, mặc định là ngày hiện tại)
    }


    public class ResultApiKhachHangSangLocHIVCH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string redcap_repeat_instrument { get; set; }
        public int? redcap_repeat_instance { get; set; }
        public int? tinhtrang { get; set; }
        public int? arv { get; set; }
        public string cs_dieutri { get; set; }
        public int? hiv { get; set; }
        public string ngayhoi_fdf6e6 { get; set; }
        public int? kqxn { get; set; }
        public int? lydo { get; set; }
        public int? sng_lc_hiv_complete { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
    }


    public class ResultApiKhachHangPhieuTuVanCH07Model
    {
        public int? id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }

        public string ngaynhap_tuvan { get; set; }
        public string ngaytuvan { get; set; }
        public string diadiem { get; set; }
        public string tcv { get; set; }
        public int? lantuvan { get; set; }

        public int? cau1_1___1 { get; set; }
        public int? cau1_1___2 { get; set; }
        public int? cau1_1___3 { get; set; }
        public int? cau1_1___4 { get; set; }
        public int? cau1_1___5 { get; set; }
        public int? cau1_1___6 { get; set; }
        public int? cau1_1___7 { get; set; }
        public string cau1_1k { get; set; }

        public string cau1_2 { get; set; }

        public int? cau1_3___8 { get; set; }
        public int? cau1_3___9 { get; set; }
        public int? cau1_3___10 { get; set; }
        public int? cau1_3___11 { get; set; }

        public int? cau_1_4___1 { get; set; }
        public int? cau_1_4___2 { get; set; }
        public int? cau_1_4___3 { get; set; }
        public int? cau_1_4___4 { get; set; }
        public int? cau_1_4___5 { get; set; }
        public int? cau_1_4___6 { get; set; }
        public int? cau_1_4___7 { get; set; }
        public string cau_1_4_2 { get; set; }

        public string cau_1_4_1_mt { get; set; }
        public string cau_1_5 { get; set; }
        public string cau_1_5_2 { get; set; }
        public string cau_1_5_3 { get; set; }
        public string cau_1_5_4 { get; set; }
        public string cau_1_5_5 { get; set; }
        public string cau_1_5_6 { get; set; }

        public int? cau2_852460___1 { get; set; }
        public int? cau2_852460___2 { get; set; }
        public int? cau2_852460___3 { get; set; }
        public int? cau2_852460___4 { get; set; }
        public int? cau2_852460___5 { get; set; }
        public int? cau2_852460___6 { get; set; }
        public string cau2_1k { get; set; }
        public string cau2_2 { get; set; }

        public int? cau3_6aaf33 { get; set; }
        public int? cau3_6aaf33___1 { get; set; }
        public int? cau3_6aaf33___2 { get; set; }
        public int? cau3_6aaf33___3 { get; set; }
        public int? cau3_6aaf33___4 { get; set; }
        public int? cau3_6aaf33___5 { get; set; }
        public int? cau3_6aaf33___6 { get; set; }
        public int? cau3_6aaf33___7 { get; set; }
        public int? cau3_6aaf33___8 { get; set; }
        public int? cau3_6aaf33___9 { get; set; }
        public int? cau3_6aaf33___10 { get; set; }
        public int? cau3_6aaf33___11 { get; set; }
        public int? cau3_6aaf33___12 { get; set; }
        public string cau3_1k { get; set; }
        public string cau3_1k_2 { get; set; }

        public int? cau_4 { get; set; }
        public int? cau_4___1 { get; set; }
        public int? cau_4___2 { get; set; }
        public int? cau_4___3 { get; set; }
        public int? cau_4___4 { get; set; }
        public int? cau_4___5 { get; set; }
        public int? cau_4___6 { get; set; }
        public int? cau_4___7 { get; set; }
        public int? cau_4___8 { get; set; }
        public int? cau_4___9 { get; set; }
        public int? cau_4___10 { get; set; }
        public int? cau_4___11 { get; set; }
        public int? cau_4___12 { get; set; }
        public string cau4_1k { get; set; }
        public string cau4_1k_2 { get; set; }

        public int? cau_5 { get; set; }
        public int? cau_5___1 { get; set; }
        public int? cau_5___2 { get; set; }
        public int? cau_5___3 { get; set; }
        public int? cau_5___4 { get; set; }
        public int? cau_5___5 { get; set; }
        public int? cau_5___6 { get; set; }
        public string cau_5_1 { get; set; }
        public string cau_5_2 { get; set; }

        public int? cau_6_1 { get; set; }
        public int? cau_6_1___1 { get; set; }
        public int? cau_6_1___2 { get; set; }
        public int? cau_6_1___3 { get; set; }
        public int? cau_6_1___4 { get; set; }
        public int? cau_6_1___5 { get; set; }
        public int? cau_6_1___6 { get; set; }
        public string cau6_1k { get; set; }
        public string cau6_1_2 { get; set; }

        public int? cau_6_2 { get; set; }
        public int? cau_6_2___1 { get; set; }
        public int? cau_6_2___2 { get; set; }
        public int? cau_6_2___3 { get; set; }
        public int? cau_6_2___4 { get; set; }
        public int? cau_6_2___5 { get; set; }
        public int? cau_6_2___6 { get; set; }
        public int? cau_6_2___7 { get; set; }
        public int? cau_6_2___8 { get; set; }
        public int? cau_6_2___9 { get; set; }
        public int? cau_6_2___10 { get; set; }
        public string cau_6_2_1 { get; set; }
        public string cau_6_2_2 { get; set; }

        public int? cau_6_3 { get; set; }
        public int? cau_6_3___1 { get; set; }
        public int? cau_6_3___2 { get; set; }
        public int? cau_6_3___3 { get; set; }
        public int? cau_6_3___4 { get; set; }
        public int? cau_6_3___5 { get; set; }
        public int? cau_6_3___6 { get; set; }
        public string cau_6_3_1 { get; set; }
        public string cau_6_3_2 { get; set; }

        public int? cau_6_4 { get; set; }
        public string cau_6_4_1 { get; set; }

        public int? cau_6_5 { get; set; }
        public int? cau_6_5_1 { get; set; }

        public int? cau_6_6 { get; set; }
        public int? cau_6_6_1 { get; set; }

        public int? tongket { get; set; }
        public int? tuvantiep { get; set; }
        public string vande { get; set; }
        public string thoigian { get; set; }
        public int? phiu_t_vn_complete { get; set; }
    }


    public class ResultApiKhachHangPhieuXetNghiemLaiHIVCH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
        public int solan { get; set; }
        public DateTime ngayxetnghiem { get; set; }
        public int kqxn_lai { get; set; }
        public int phiu_xt_nghim_li_hiv_complete { get; set; }
    }

    public class ResultApiKhachHangTheoDauCH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }

        public DateTime ngay_7794e9 { get; set; }
        public int hinhthuc { get; set; }
        public string hinhthuc_khac { get; set; }
        public int kq { get; set; }
        public string khac_a7b30e { get; set; }
        public string ghichu { get; set; }
        public int theo_du_kh_complete { get; set; }
    }

    public class ResultApiKhachHangChuyenGuiDichVuCH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
        public int? loaihinh { get; set; }
        public string ngay_xn { get; set; }
        public string diachi_xn { get; set; }
        public int? kq_xn { get; set; }
        public int? dieutri { get; set; }
        public string diachi_cg { get; set; }
        public string ngay_bddt { get; set; }
        public int? taiuong_bd { get; set; }
        public int? taiuong_bd_2 { get; set; }
        public int? dt_arv { get; set; }
        public string thoidiem { get; set; }
        public string lydo { get; set; }
        public string ngaykham { get; set; }
        public string diachikham { get; set; }
        public int? lankham { get; set; }
        public int? chandoan { get; set; }
        public string khac { get; set; }
        public int? kedon { get; set; }
        public int? dungthuoc { get; set; }
        public int? taikham { get; set; }
        public string ngay_taikham { get; set; }
        public string ngaykxn { get; set; }
        public string diachi_kxn { get; set; }
        public int? lan_xnk { get; set; }
        public int? chandoan1 { get; set; }
        public string khac_sti { get; set; }
        public int? dieutri_sti { get; set; }
        public string ngay_29b439 { get; set; }
        public int? mua_bhyt { get; set; }
        public int? dt_prep { get; set; }
        public string cs_prep { get; set; }
        public int? dt_pep { get; set; }
        public string cs_pep { get; set; }
        public int? dt_vgc { get; set; }
        public string cs_vgc { get; set; }
        public int? dt_lao { get; set; }
        public string cs_lao { get; set; }
        public int? dt_met { get; set; }
        public string cs_met { get; set; }
        public int? chuyn_gi_dch_v_complete { get; set; }
    }

    public class ResultApiKhachHangBangHoiACECH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }

        public DateTime? ngay_ace { get; set; }
        public int? c1 { get; set; }
        public int? c2 { get; set; }
        public int? c3 { get; set; }
        public int? c4 { get; set; }
        public int? c5 { get; set; }
        public int? c6 { get; set; }
        public int? c7 { get; set; }
        public int? c8 { get; set; }
        public int? c9 { get; set; }
        public int? c10 { get; set; }
        public int? diem { get; set; }
        public int? bng_hi_ace_complete { get; set; }
    }

    //CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC 
    public class ResultApiKhachHangAssistQstKienThucCH07Model
    {
        public int id { get; set; }
        public string record_id { get; set; }
        public string maduan { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
        public string ngaynhap { get; set; }
        public string ngay { get; set; }
        public int doituong { get; set; }
        public int gioitinh { get; set; }
        public int namsinh { get; set; }
        public int chatgaynghien { get; set; }
        public string khac1 { get; set; }
        public int chatgaynghien_2 { get; set; }
        public string khac2 { get; set; }
        public int duongsd { get; set; }
        public int tansuatda { get; set; }
        public string landau { get; set; }
        public int tuoi { get; set; }
        public string matuydautien { get; set; }
        public int tiemchich { get; set; }
        public int dungchung { get; set; }
        public int qhtd { get; set; }
        public int cau_7_1 { get; set; }
        public int qhtd_2 { get; set; }
        public int sdmatuy { get; set; }
        public int qhtdtt { get; set; }
        public int bandam { get; set; }
        public int sti { get; set; }
        public int sti1 { get; set; }
        public string khac3 { get; set; }
        public string lao { get; set; }
        public int quakhu { get; set; }
        public int hientai { get; set; }
        public int hientai_2 { get; set; }
        public string ganc { get; set; }
        public int quakhu_2 { get; set; }
        public int hientai_4 { get; set; }
        public int hientai_3 { get; set; }
        public int trieuchung_2 { get; set; }
        public string khac_5 { get; set; }
        public int cau1 { get; set; }
        public int cau2 { get; set; }
        public int cau3 { get; set; }
        public int cau4 { get; set; }
        public int cau5 { get; set; }
        public int cau6 { get; set; }
        public int cau7 { get; set; }
        public int cau8 { get; set; }
        public int cau9 { get; set; }
        public int cau10 { get; set; }
        public int cau11 { get; set; }
        public int cau12 { get; set; }
        public int cau13 { get; set; }
        public int cau14 { get; set; }
        public int cau15 { get; set; }
        public int cau16 { get; set; }
        public int cau17 { get; set; }
        public int cau18 { get; set; }
        public int cau19 { get; set; }
        public int cau20 { get; set; }
        public int cau21 { get; set; }
        public int cau22 { get; set; }
        public int cau23 { get; set; }
        public int cau24 { get; set; }
        public int cau25 { get; set; }
        public int cau26 { get; set; }
        public int cau27 { get; set; }
        public string assist { get; set; }
        public int thuocla { get; set; }
        public int thucuong { get; set; }
        public int cansa { get; set; }
        public int cocain { get; set; }
        public int chatkichthich { get; set; }
        public int khixong { get; set; }
        public int thuocanthan { get; set; }
        public int chatgayaogiac { get; set; }
        public int thuocphien { get; set; }
        public int chatkhac { get; set; }
        public string cacchatkhac { get; set; }

        // Ma trận
        public int lucdihoc { get; set; }
        public int thuocla1 { get; set; }
        public int thucuong1 { get; set; }
        public int cansa1 { get; set; }
        public int coca1 { get; set; }
        public int chatkichthich1 { get; set; }
        public int khixong1 { get; set; }
        public int thuocanthan1 { get; set; }
        public int chatgayaogiac1 { get; set; }
        public int chatthuocphien1 { get; set; }
        public int chatkhac1 { get; set; }
        public int thuocla2 { get; set; }
        public int thucuong2 { get; set; }
        public int cansa2 { get; set; }
        public int coca2 { get; set; }
        public int chatkichthich2 { get; set; }
        public int khixong2 { get; set; }
        public int thuocanthan2 { get; set; }
        public int chatgayaogiac2 { get; set; }
        public int chatthuocphien2 { get; set; }
        public int chatkhac2 { get; set; }
        public int thuocla3 { get; set; }
        public int thucuong3 { get; set; }
        public int cansa3 { get; set; }
        public int coca3 { get; set; }
        public int chatkichthich3 { get; set; }
        public int khixong3 { get; set; }
        public int thuocanthan3 { get; set; }
        public int chatgayaogiac3 { get; set; }
        public int chatthuocphien3 { get; set; }
        public int chatkhac3 { get; set; }
        public int thuocla4 { get; set; }
        public int thucuong4 { get; set; }
        public int cansa4 { get; set; }
        public int coca4 { get; set; }
        public int chatkichthich4 { get; set; }
        public int khixong4 { get; set; }
        public int thuocanthan4 { get; set; }
        public int chatgayaogiac4 { get; set; }
        public int chatthuocphien4 { get; set; }
        public int chatkhac4 { get; set; }
        public int thuocla5 { get; set; }
        public int thucuong5 { get; set; }
        public int cansa5 { get; set; }
        public int coca5 { get; set; }
        public int chatkichthich5 { get; set; }
        public int khixong5 { get; set; }
        public int thuocanthan5 { get; set; }
        public int chatgayaogiac5 { get; set; }
        public int chatthuocphien5 { get; set; }
        public int chatkhac5 { get; set; }
        public int thuocla6 { get; set; }
        public int thucuong6 { get; set; }
        public int cansa6 { get; set; }
        public int coca6 { get; set; }
        public int chatkichthich6 { get; set; }
        public int khixong6 { get; set; }
        public int thuocanthan6 { get; set; }
        public int chatgayaogiac6 { get; set; }
        public int chatthuocphien6 { get; set; }
        public int chatkhac6 { get; set; }

        // Điểm và nguy cơ
        public int? cau_8 { get; set; }
        public int? diemthuocla { get; set; }
        public string nguycothap { get; set; }
        public string nguycotrungbinh { get; set; }
        public string nguycocao { get; set; }
        public string kocanthiep { get; set; }
        public string canthiepngan { get; set; }
        public string chuachuyensau { get; set; }
        public int? diemthucuong { get; set; }
        public string nguycothap_2 { get; set; }
        public string nguycotrungbinh_2 { get; set; }
        public string nguycocao_2 { get; set; }
        public string kocanthiep_2 { get; set; }
        public string canthiepngan_2 { get; set; }
        public string chuachuyensau_2 { get; set; }
        public int? diemcansa { get; set; }
        public string nguycothap_3 { get; set; }
        public string nguycotrungbinh_3 { get; set; }
        public string nguycocao_3 { get; set; }
        public string kocanthiep_3 { get; set; }
        public string canthiepngan_3 { get; set; }
        public string chuachuyensau_3 { get; set; }
        public int? diemcoca { get; set; }
        public string nguycothap_4 { get; set; }
        public string nguycotrungbinh_4 { get; set; }
        public string nguycocao_4 { get; set; }
        public string kocanthiep_4 { get; set; }
        public string canthiepngan_4 { get; set; }
        public string chuachuyensau_4 { get; set; }
        public int? diemchatkichthich { get; set; }
        public string nguycothap_5 { get; set; }
        public string nguycotrungbinh_5 { get; set; }
        public string nguycocao_5 { get; set; }
        public string kocanthiep_5 { get; set; }
        public string canthiepngan_5 { get; set; }
        public string chuachuyensau_5 { get; set; }
        public int? diemkhixong { get; set; }
        public string nguycothap_6 { get; set; }
        public string nguycotrungbinh_6 { get; set; }
        public string nguycocao_6 { get; set; }
        public string kocanthiep_6 { get; set; }
        public string canthiepngan_6 { get; set; }
        public string chuachuyensau_6 { get; set; }
        public int? diemthuocanthan { get; set; }
        public string nguycothap_7 { get; set; }
        public string nguycotrungbinh_7 { get; set; }
        public string nguycocao_7 { get; set; }
        public string kocanthiep_7 { get; set; }
        public string canthiepngan_7 { get; set; }
        public string chuachuyensau_7 { get; set; }
        public int? diemchatgayaogiac { get; set; }
        public string nguycothap_8 { get; set; }
        public string nguycotrungbinh_8 { get; set; }
        public string nguycocao_8 { get; set; }
        public string kocanthiep_8 { get; set; }
        public string canthiepngan_8 { get; set; }
        public string chuachuyensau_8 { get; set; }
        public int? diemchatthuocphien { get; set; }
        public string nguycothap_9 { get; set; }
        public string nguycotrungbinh_9 { get; set; }
        public string nguycocao_9 { get; set; }
        public string kocanthiep_9 { get; set; }
        public string canthiepngan_9 { get; set; }
        public string chuachuyensau_9 { get; set; }
        public int? diemchatkhac { get; set; }
        public string nguycothap_10 { get; set; }
        public string nguycotrungbinh_10 { get; set; }
        public string nguycocao_10 { get; set; }
        public string kocanthiep_10 { get; set; }
        public string canthiepngan_10 { get; set; }
        public string chuachuyensau_10 { get; set; }
        public int? diemcacchatkhac { get; set; }
        public int c_1a { get; set; }
        public int c_1b { get; set; }
        public int c_1c { get; set; }
        public int c_1d { get; set; }
        public int c_2 { get; set; }
        public int c_3 { get; set; }
        public int c_4a { get; set; }
        public int c_4b { get; set; }
        public int c_4c { get; set; }
        public int tongdiem { get; set; }
        public string duongtinh { get; set; }
        public string amtinh { get; set; }
        public int thng_tin_c_bn_assist_qst_kin_thc_complete { get; set; }
    }

}
