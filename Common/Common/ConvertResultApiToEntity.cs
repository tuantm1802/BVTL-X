using log4net;
using Model.Model;
using Model.ModelExtend.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ICommon;

namespace Common.Common
{
    public class ConvertResultApiToEntity: IConvertResultApiToEntity
    {
        private  readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private  BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int CreateCustomer(BVTL_KHACH_HANG customer)
        {
            var result = 0;
            db.BVTL_KHACH_HANG.Add(customer);
            db.SaveChanges();
            result = customer.khachhang_id;
            return result;
        }

        /// <summary>
        /// CHuyển đổi kết quả api có report_id = 1344 sang 2 entity BVTL_KQ_SL_SKTT, BVTL_KQ_SL_ASSIST
        /// </summary>
        /// <param name="resultApi1344s"></param>
        /// <param name="sktts"></param>
        /// <param name="assists"></param>
        public  void ConvertApi1344ToEntity(List<ResultApi1344Model> resultApi1344s, ref List<BVTL_KQ_SL_SKTT> sktts, ref List<BVTL_KQ_SL_ASSIST> assists)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api report_id = 1344 sang entity**************************************");
            try
            {
                var sktt = new BVTL_KQ_SL_SKTT();
                var assist = new BVTL_KQ_SL_ASSIST();
                var resultApi1344 = new ResultApi1344Model();
                var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = DateTime.Today;

                for (int i = 0; i < resultApi1344s.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = DateTime.Today;

                    resultApi1344 = resultApi1344s[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                    customer_code = resultApi1344.makh;
                    group_code = customer_code.Substring(0, 5);
                    // Lấy id tỉnh
                    if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    if (customer != null && customer.khachhang_id > 0)
                    {
                        customer_id = customer.khachhang_id;
                    }
                    else
                    {
                        customer = new BVTL_KHACH_HANG
                        {
                            makh = resultApi1344.makh,
                            hoten = resultApi1344.hoten,
                            gioitinh = resultApi1344.gioitinh == "Nam" ? "M" : (resultApi1344.gioitinh == "Nữ" ? "F" : "O"),
                            sodienthoai = resultApi1344.dienthoai,
                            diachi = resultApi1344.diachi
                        };
                        customer.city_code = cityCode;
                        if (!string.IsNullOrEmpty(resultApi1344.namsinh))
                            customer.namsinh = Convert.ToInt32(resultApi1344.namsinh);

                        if (!string.IsNullOrEmpty(resultApi1344.doituong))
                        {
                            customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApi1344.doituong).id;
                        }

                        if (!string.IsNullOrEmpty(resultApi1344.ngaytiepcan))
                            customer.ngaytiepcan = DateTime.ParseExact(resultApi1344.ngaytiepcan, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        customer_id = CreateCustomer(customer);
                    }

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null)
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi1344.tbh });
                        db.SaveChanges();
                    }

                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApi1344.ngaynhap))
                    {
                        ngaynhap = resultApi1344.ngaynhap.Split(' ')[0];
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    day = ngaynhapD.Day;
                    month = ngaynhapD.Month;
                    year = ngaynhapD.Year;

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_SKTT
                    sktt = new BVTL_KQ_SL_SKTT()
                    {
                        khachhang_id = customer_id,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode
                    };

                    sktt.ketqua_QST = 0;
                    if (!string.IsNullOrEmpty(resultApi1344.tongdiem))
                    {
                        sktt.diem_QST = Convert.ToInt32(resultApi1344.tongdiem);
                        if (sktt.diem_QST > 4)
                            sktt.ketqua_QST = 1;
                    }

                    sktts.Add(sktt);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_ASSIST
                    assist = new BVTL_KQ_SL_ASSIST()
                    {
                        khachhang_id = customer_id,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        // ghichu = resultApi1344.,
                        manhom_tbh = group_code,
                        city_code = cityCode
                    };

                    #region Tính điểm, nguy cơ của các loại chất kích thích

                    if (!string.IsNullOrEmpty(resultApi1344.diemthuocla))
                    {
                        assist.thuocla_diem = Convert.ToInt32(resultApi1344.diemthuocla);
                        if (assist.thuocla_diem < 4)
                            assist.thuocla_nguyco = "Thấp";
                        else if (assist.thuocla_diem >= 4 && assist.thuocla_diem < 27)
                            assist.thuocla_nguyco = "Trung bình";
                        else if (assist.thuocla_diem >= 27)
                            assist.thuocla_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemthucuong))
                    {
                        assist.conruou_diem = Convert.ToInt32(resultApi1344.diemthucuong);
                        if (assist.conruou_diem < 4)
                            assist.conruou_nguyco = "Thấp";
                        else if (assist.conruou_diem >= 4 && assist.conruou_diem < 27)
                            assist.conruou_nguyco = "Trung bình";
                        else if (assist.conruou_diem >= 27)
                            assist.conruou_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemcansa))
                    {
                        assist.cansa_diem = Convert.ToInt32(resultApi1344.diemcansa);
                        if (assist.cansa_diem < 4)
                            assist.cansa_nguyco = "Thấp";
                        else if (assist.cansa_diem >= 4 && assist.cansa_diem < 27)
                            assist.cansa_nguyco = "Trung bình";
                        else if (assist.cansa_diem >= 27)
                            assist.cansa_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemcoca))
                    {
                        assist.cocaine_diem = Convert.ToInt32(resultApi1344.diemcoca);
                        if (assist.cocaine_diem < 4)
                            assist.cocaine_nguyco = "Thấp";
                        else if (assist.cocaine_diem >= 4 && assist.cocaine_diem < 27)
                            assist.cocaine_nguyco = "Trung bình";
                        else if (assist.cocaine_diem >= 27)
                            assist.cocaine_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemchatkichthich))
                    {
                        assist.matuyda_diem = Convert.ToInt32(resultApi1344.diemchatkichthich);
                        if (assist.matuyda_diem < 4)
                            assist.matuyda_nguyco = "Thấp";
                        else if (assist.matuyda_diem >= 4 && assist.matuyda_diem < 27)
                            assist.matuyda_nguyco = "Trung bình";
                        else if (assist.matuyda_diem >= 27)
                            assist.matuyda_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemkhixong))
                    {
                        assist.khixonghit_diem = Convert.ToInt32(resultApi1344.diemkhixong);
                        if (assist.khixonghit_diem < 4)
                            assist.khixonghit_nguyco = "Thấp";
                        else if (assist.khixonghit_diem >= 4 && assist.khixonghit_diem < 27)
                            assist.khixonghit_nguyco = "Trung bình";
                        else if (assist.khixonghit_diem >= 27)
                            assist.khixonghit_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemchatanthan))
                    {
                        assist.thuocanthan_diem = Convert.ToInt32(resultApi1344.diemchatanthan);
                        if (assist.thuocanthan_diem < 4)
                            assist.thuocanthan_nguyco = "Thấp";
                        else if (assist.thuocanthan_diem >= 4 && assist.thuocanthan_diem < 27)
                            assist.thuocanthan_nguyco = "Trung bình";
                        else if (assist.thuocanthan_diem >= 27)
                            assist.thuocanthan_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemchatgayaogiac))
                    {
                        assist.chatgayaogiac_diem = Convert.ToInt32(resultApi1344.diemchatgayaogiac);
                        if (assist.chatgayaogiac_diem < 4)
                            assist.chatgayaogiac_nguyco = "Thấp";
                        else if (assist.chatgayaogiac_diem >= 4 && assist.chatgayaogiac_diem < 27)
                            assist.chatgayaogiac_nguyco = "Trung bình";
                        else if (assist.chatgayaogiac_diem >= 27)
                            assist.chatgayaogiac_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemchatthuocphien))
                    {
                        assist.thuocphien_diem = Convert.ToInt32(resultApi1344.diemchatthuocphien);
                        if (assist.thuocphien_diem < 4)
                            assist.thuocphien_nguyco = "Thấp";
                        else if (assist.thuocphien_diem >= 4 && assist.thuocphien_diem < 27)
                            assist.thuocphien_nguyco = "Trung bình";
                        else if (assist.thuocphien_diem >= 27)
                            assist.thuocphien_nguyco = "Cao";
                    }

                    if (!string.IsNullOrEmpty(resultApi1344.diemchatkhac))
                    {
                        assist.chatkhac_diem = Convert.ToInt32(resultApi1344.diemchatkhac);
                        if (assist.chatkhac_diem < 4)
                            assist.chatkhac_nguyco = "Thấp";
                        else if (assist.chatkhac_diem >= 4 && assist.chatkhac_diem < 27)
                            assist.chatkhac_nguyco = "Trung bình";
                        else if (assist.chatkhac_diem >= 27)
                            assist.chatkhac_nguyco = "Cao";
                    }
                    #endregion

                    assists.Add(assist);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api report_id = 1344 sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api report_id = 1344 sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api HIV sang 2 entity BVTL_KQ_XN_HIV
        /// </summary>
        /// <param name="resultApiHIVs"></param>
        /// <param name="hivs"></param>
        public  void ConvertApiHIVToEntity(List<ResultApiHIVModel> resultApiHIVs, ref List<BVTL_KQ_XN_HIV> hivs)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api hiv sang entity**************************************");
            try
            {
                var hiv = new BVTL_KQ_XN_HIV();
                var resultApiHIV = new ResultApiHIVModel();
                var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = DateTime.Today;

                for (int i = 0; i < resultApiHIVs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = DateTime.Today;

                    resultApiHIV = resultApiHIVs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                    customer_code = resultApiHIV.makh;
                    group_code = customer_code.Substring(0, 5);
                    // Lấy id tỉnh
                    if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    if (customer != null && customer.khachhang_id > 0)
                    {
                        customer_id = customer.khachhang_id;
                    }
                    else
                    {
                        customer = new BVTL_KHACH_HANG
                        {
                            makh = resultApiHIV.makh,
                            hoten = string.IsNullOrEmpty(resultApiHIV.hoten)? resultApiHIV.makh: resultApiHIV.hoten,
                            gioitinh = resultApiHIV.gioitinh == "Nam" ? "M" : (resultApiHIV.gioitinh == "Nữ" ? "F" : "O"),
                            sodienthoai = resultApiHIV.dienthoai,
                            diachi = resultApiHIV.diachi
                        };
                        customer.city_code = cityCode;
                        if (!string.IsNullOrEmpty(resultApiHIV.namsinh))
                            customer.namsinh = Convert.ToInt32(resultApiHIV.namsinh);

                        if (!string.IsNullOrEmpty(resultApiHIV.doituong))
                        {
                            customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiHIV.doituong).id;
                        }

                        if (!string.IsNullOrEmpty(resultApiHIV.ngaytiepcan))
                            customer.ngaytiepcan = DateTime.ParseExact(resultApiHIV.ngaytiepcan, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        customer_id = CreateCustomer(customer);
                    }

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null)
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiHIV.tbh });
                        db.SaveChanges();
                    }

                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApiHIV.ngayhoi))
                    {
                        ngaynhap = resultApiHIV.ngayhoi;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    day = ngaynhapD.Day;
                    month = ngaynhapD.Month;
                    year = ngaynhapD.Year;

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_SKTT
                    hiv = new BVTL_KQ_XN_HIV()
                    {
                        khachhang_id = customer_id,
                        ngayxn = ngaynhapD,
                        ngayxn_date = day,
                        ngayxn_month = month,
                        ngayxn_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode
                    };

                    hiv.ketqua = 0;
                    if (!string.IsNullOrEmpty(resultApiHIV.kqxn))
                    {
                        if (resultApiHIV.kqxn == "Âm tính")
                            hiv.ketqua = -1;
                        if (resultApiHIV.kqxn == "Dương tính")
                            hiv.ketqua = 1;
                    }

                    hiv.dangdieutri_hiv = 0;
                    //if (!string.IsNullOrEmpty(resultApiHIV.hiv))
                    //{
                    //    if (resultApiHIV.hiv == "Có")
                    //        hiv.dangdieutri_hiv = 1;
                    //}

                    hivs.Add(hiv);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api hiv sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api hiv sang entity**************************************");
        }
    }
}
