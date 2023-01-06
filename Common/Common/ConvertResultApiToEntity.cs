using Common.ICommon;
using log4net;
using Model.Model;
using Model.ModelExtend.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Common
{
    public class ConvertResultApiToEntity : IConvertResultApiToEntity
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int CreateCustomer(BVTL_KHACH_HANG customer)
        {
            var result = 0;
            try
            {
                customer.sync_date = DateTime.Now;
                db.BVTL_KHACH_HANG.Add(customer);
                db.SaveChanges();
                result = customer.khachhang_id;
            }catch(Exception ex)
            {
                log.Error("Lỗi thêm khách hàng: "+ ex.Message);
                // Lấy thông tin khách hàng
                //var customers = db.BVTL_KHACH_HANG.FirstOrDefault(x=>x.makh == customer.makh);
                //if (customers != null && customers.khachhang_id > 0)
                //    result = customers.khachhang_id;
            }
            
            return result;
        }

        /// <summary>
        /// CHuyển đổi kết quả api có report_id = 1344 sang 2 entity BVTL_KQ_SL_SKTT, BVTL_KQ_SL_ASSIST
        /// </summary>
        /// <param name="resultApi1344s"></param>
        /// <param name="sktts"></param>
        /// <param name="assists"></param>
        public void ConvertApi1344ToEntity(List<ResultApi1344Model> resultApi1344s, string maDuAn, ref List<BVTL_KQ_SL_SKTT> sktts, ref List<BVTL_KQ_SL_ASSIST> assists)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api report_id = 1344 ASSIST/SKTT sang entity**************************************");
            log.Info("*********-----TỔNG SỐ RECORD API ASSIST/SKTT:" + resultApi1344s.Count + " | MADUAN:" + maDuAn);

            var sktt = new BVTL_KQ_SL_SKTT();
            var assist = new BVTL_KQ_SL_ASSIST();
            var resultApi1344 = new ResultApi1344Model();
            var customer_code = "";
            try
            {
                
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                var sottkh = "";

                //for (int i = 0; i < resultApi1344s.Where(x=> !string.IsNullOrEmpty( x.makh)).ToList().Count; i++)
                for (int i = 0; i < resultApi1344s.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi1344 = resultApi1344s[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                    //customer_code = resultApi1344.makh;
                    customer_code = String.Concat(resultApi1344.makh, resultApi1344.makh_2, resultApi1344.makh_3, resultApi1344.makh_4, resultApi1344.makh_5, resultApi1344.makh_6, resultApi1344.makh_7, resultApi1344.makh_8, resultApi1344.makh_9, resultApi1344.makh_10);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        sottkh = customer_code.Substring(5);
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                        sottkh = customer_code.Substring(5);
                    }
                    
                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    //customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    //if (customer != null && customer.khachhang_id > 0)
                    //{
                    //    customer_id = customer.khachhang_id;
                    //}
                    //else
                    //{
                    //    customer = new BVTL_KHACH_HANG
                    //    {
                    //        makh = resultApi1344.makh,
                    //        hoten = resultApi1344.hoten,
                    //        gioitinh = resultApi1344.gioitinh == "Nam" ? "M" : (resultApi1344.gioitinh == "Nữ" ? "F" : "O"),
                    //        sodienthoai = resultApi1344.dienthoai,
                    //        diachi = resultApi1344.diachi,
                    //        sottkh = customer_code.Substring(5)
                    //    };
                    //    customer.city_code = cityCode;
                    //    if (!string.IsNullOrEmpty(resultApi1344.namsinh))
                    //        customer.namsinh = Convert.ToInt32(resultApi1344.namsinh);

                    //    if (!string.IsNullOrEmpty(resultApi1344.doituong))
                    //    {
                    //        customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApi1344.doituong).id;
                    //    }

                    //    if (!string.IsNullOrEmpty(resultApi1344.ngay))
                    //        customer.ngaytiepcan = DateTime.ParseExact(resultApi1344.ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    //    customer_id = CreateCustomer(customer);
                    //}

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi1344.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    //else
                    //{
                    //    // Cập nhật thêm tỉnh khai thác dữ liệu nếu chưa có
                    //    if (!string.IsNullOrEmpty(nhomTBH.city_codes))
                    //    {
                    //        if(!nhomTBH.city_codes.Contains(cityCode))
                    //            nhomTBH.city_codes += ','+ cityCode;
                    //    }
                    //    else
                    //        nhomTBH.city_codes = cityCode;
                    //    db.SaveChanges();
                    //}

                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApi1344.ngay))
                    {
                        ngaynhap = resultApi1344.ngay.Split(' ')[0];
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_SKTT
                    sktt = new BVTL_KQ_SL_SKTT()
                    {
                        makh = customer_code,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn,
                        
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
                        makh = customer_code,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        // ghichu = resultApi1344.,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn
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
                
                log.Info("*********-----SỐ BẢN GHI ASSIST/SKTT ĐÃ CONVERT:" + assists.Count() + "/" + sktts.Count() + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api report_id = 1344 sang entity lỗi: " + ex.Message + " | MADUAN:" + maDuAn + " | MAKH:" + customer_code);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api report_id = 1344 sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api HIV sang entity BVTL_KQ_XN_HIV
        /// </summary>
        /// <param name="resultApiHIVs"></param>
        /// <param name="hivs"></param>
        public void ConvertApiHIVToEntity(List<ResultApiHIVModel> resultApiHIVs, string maDuAn, ref List<BVTL_KQ_XN_HIV> hivs)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api hiv sang entity**************************************");
            log.Info("*********-----TỔNG SỐ RECORD API HIV:" + resultApiHIVs.Count + " | MADUAN:" + maDuAn);

            try
            {
                var hiv = new BVTL_KQ_XN_HIV();
                var resultApiHIV = new ResultApiHIVModel();
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                //var ngaynhapD = DateTime.Today;
                var ngaynhapD = new DateTime();

                int errNo = 0;
                for (int i = 0; i < resultApiHIVs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApiHIV = resultApiHIVs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                    //customer_code = resultApiHIV.makh;
                    customer_code = String.Concat(resultApiHIV.makh, resultApiHIV.makh_2, resultApiHIV.makh_3, resultApiHIV.makh_4, resultApiHIV.makh_5, resultApiHIV.makh_6, resultApiHIV.makh_7, resultApiHIV.makh_8, resultApiHIV.makh_9, resultApiHIV.makh_10);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    /*
                    customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    if (customer != null && customer.khachhang_id > 0)
                    {
                        customer_id = customer.khachhang_id;
                    }
                    else
                    {
                        //customer = new BVTL_KHACH_HANG
                        //{
                        //    makh = resultApiHIV.makh,
                        //    hoten = string.IsNullOrEmpty(resultApiHIV.hoten) ? resultApiHIV.makh : resultApiHIV.hoten,
                        //    gioitinh = resultApiHIV.gioitinh == "Nam" ? "M" : (resultApiHIV.gioitinh == "Nữ" ? "F" : "O"),
                        //    sodienthoai = resultApiHIV.dienthoai,
                        //    diachi = resultApiHIV.diachi,
                        //    sottkh = customer_code.Substring(5)
                        //};
                        //customer.city_code = cityCode;
                        //if (!string.IsNullOrEmpty(resultApiHIV.namsinh))
                        //    customer.namsinh = Convert.ToInt32(resultApiHIV.namsinh);

                        //if (!string.IsNullOrEmpty(resultApiHIV.doituong))
                        //{
                        //    customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiHIV.doituong).id;
                        //}

                        //if (!string.IsNullOrEmpty(resultApiHIV.ngaytiepcan))
                        //    customer.ngaytiepcan = DateTime.ParseExact(resultApiHIV.ngaytiepcan, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        //customer_id = CreateCustomer(customer);


                        // Lấy thông tin khách hàng
                        var customerCK = db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == customer_code);
                        if (customerCK != null && customerCK.khachhang_id > 0)
                            customer_id = customerCK.khachhang_id;
                    }
                    */

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiHIV.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    //else
                    //{
                    //    // Cập nhật thêm tỉnh khai thác dữ liệu nếu chưa có
                    //    if (!string.IsNullOrEmpty(nhomTBH.city_codes))
                    //    {
                    //        if (!nhomTBH.city_codes.Contains(cityCode))
                    //            nhomTBH.city_codes += ',' + cityCode;
                    //    }
                    //    else
                    //        nhomTBH.city_codes = cityCode;
                    //    db.SaveChanges();
                    //}
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApiHIV.ngayhoi))
                    {
                        ngaynhap = resultApiHIV.ngayhoi;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    else if(!string.IsNullOrEmpty(resultApiHIV.sng_lc_hiv_timestamp))
                    {
                        ngaynhap = resultApiHIV.sng_lc_hiv_timestamp.Split(' ')[0];
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_XN_HIV
                    hiv = new BVTL_KQ_XN_HIV()
                    {
                        makh = customer_code,
                        ngayxn = ngaynhapD,
                        ngayxn_date = day,
                        ngayxn_month = month,
                        ngayxn_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn,
                        sottkh = customer_code.Substring(5)
                    };

                    hiv.ketqua = 0;
                    if (!string.IsNullOrEmpty(resultApiHIV.kqxn))
                    {
                        if (resultApiHIV.kqxn == "Âm tính")
                            hiv.ketqua = -1;
                        if (resultApiHIV.kqxn == "Dương tính")
                            hiv.ketqua = 1;
                    }
                    hiv.lydo = resultApiHIV.lydo;
                    hiv.dangdieutri_hiv = 0;
                    if (!string.IsNullOrEmpty(resultApiHIV.hiv))
                    {
                        if (resultApiHIV.hiv == "Có")
                            hiv.dangdieutri_hiv = 1;
                    }

                    hivs.Add(hiv);

                    #endregion
                    
                }

                log.Info("*********-----SỐ BẢN GHI HIV ĐÃ CONVERT:" + hivs.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api hiv sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api HIV sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api ACE sang entity BVTL_KQ_SL_ACE
        /// </summary>
        /// <param name="resultApiACEs"></param>
        /// <param name="aces"></param>
        public void ConvertApiACEToEntity(List<ResultApiACEModel> resultApiACEs, string maDuAn, ref List<BVTL_KQ_SL_ACE> aces)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api ace sang entity**************************************");
            log.Info("*********-----TỔNG SỐ RECORD API ACE:" + resultApiACEs.Count + " | MADUAN:" + maDuAn);
            try
            {
                var ace = new BVTL_KQ_SL_ACE();
                var resultApiACE = new ResultApiACEModel();
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = DateTime.Today;

                int errNo = 0;
                for (int i = 0; i < resultApiACEs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = DateTime.Today;

                    resultApiACE = resultApiACEs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiACE.makh;
                    customer_code = String.Concat(resultApiACE.makh, resultApiACE.makh_2, resultApiACE.makh_3, resultApiACE.makh_4, resultApiACE.makh_5, resultApiACE.makh_6, resultApiACE.makh_7, resultApiACE.makh_8, resultApiACE.makh_9, resultApiACE.makh_10);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    //customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    //if (customer != null && customer.khachhang_id > 0)
                    //{
                    //    customer_id = customer.khachhang_id;
                    //}
                    //else
                    //{
                    //    //customer = new BVTL_KHACH_HANG
                    //    //{
                    //    //    makh = resultApiACE.makh,
                    //    //    hoten = string.IsNullOrEmpty(resultApiACE.hoten) ? resultApiACE.makh : resultApiACE.hoten,
                    //    //    gioitinh = resultApiACE.gioitinh == "Nam" ? "M" : (resultApiACE.gioitinh == "Nữ" ? "F" : "O"),
                    //    //    sodienthoai = resultApiACE.dienthoai,
                    //    //    diachi = resultApiACE.diachi,
                    //    //    sottkh = customer_code.Substring(5)
                    //    //};
                    //    //customer.city_code = cityCode;
                    //    //if (!string.IsNullOrEmpty(resultApiACE.namsinh))
                    //    //    customer.namsinh = Convert.ToInt32(resultApiACE.namsinh);

                    //    //if (!string.IsNullOrEmpty(resultApiACE.doituong))
                    //    //{
                    //    //    customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiACE.doituong).id;
                    //    //}

                    //    //if (!string.IsNullOrEmpty(resultApiACE.ngay))
                    //    //    customer.ngaytiepcan = DateTime.ParseExact(resultApiACE.ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    //    //customer_id = CreateCustomer(customer);

                    //    // Lấy thông tin khách hàng
                    //    var customerCK = db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == customer_code);
                    //    if (customerCK != null && customerCK.khachhang_id > 0)
                    //        customer_id = customerCK.khachhang_id;
                    //}

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiACE.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApiACE.ngay))
                    {
                        ngaynhap = resultApiACE.ngay;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    day = ngaynhapD.Day;
                    month = ngaynhapD.Month;
                    year = ngaynhapD.Year;

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_ACE
                    ace = new BVTL_KQ_SL_ACE()
                    {
                        makh = customer_code,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn,
                        sottkh = customer_code.Substring(5)
                    };

                    ace.ketqua_ace = 0;
                    ace.tongdiem_ace = 0;
                    if (!string.IsNullOrEmpty(resultApiACE.diem))
                    {
                        ace.tongdiem_ace = Convert.ToInt32(resultApiACE.diem);
                        if (ace.tongdiem_ace >= 4)
                            ace.ketqua_ace = 1;
                    }
                    aces.Add(ace);

                    #endregion
                    
                }

                log.Info("*********-----SỐ BẢN GHI ACE ĐÃ CONVERT:" + aces.Count() + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api ace sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api ace sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api tổng hợp sang entity BVTL_BO_BIEU_MAU_KH_BAO_CAO
        /// </summary>
        /// <param name="resultApiTHs"></param>
        /// <param name="tongHops"></param>
        public void ConvertApiTongHopToEntity(List<ResultApiTongHopModel> resultApiTHs, string maDuAn, ref List<BVTL_BO_BIEU_MAU_KH_BAO_CAO> tongHops, ref List<BVTL_KHACH_HANG> khachHangs)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api tổng hợp sang entity**************************************");
            log.Info("*********-----TỔNG SỐ RECORD API TONG HOP BIEU_MAU_KH_BAO_CAO:" + resultApiTHs.Count + " | MADUAN:" + maDuAn);

            try
            {

                var tongHop = new BVTL_BO_BIEU_MAU_KH_BAO_CAO();
                var resultApiTH = new ResultApiTongHopModel();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                //var ngaynhapD = DateTime.Today;
                var ngaynhapD = new DateTime();

                int errNo = 0;
                for (int i = 0; i < resultApiTHs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApiTH = resultApiTHs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                    //customer_code = resultApiTH.makh;
                    customer_code = String.Concat(resultApiTH.makh, resultApiTH.makh_2, resultApiTH.makh_3, resultApiTH.makh_4, resultApiTH.makh_5, resultApiTH.makh_6, resultApiTH.makh_7, resultApiTH.makh_8, resultApiTH.makh_9, resultApiTH.makh_10);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    customer = khachHangs.FirstOrDefault(x => x.makh == customer_code);
                    if (!(customer != null && customer.khachhang_id > 0))
                    {
                        customer = new BVTL_KHACH_HANG
                        {
                            makh = resultApiTH.makh,
                            hoten = string.IsNullOrEmpty(resultApiTH.hoten) ? resultApiTH.makh : resultApiTH.hoten,
                            gioitinh = resultApiTH.gioitinh == "Nam" ? "M" : (resultApiTH.gioitinh == "Nữ" ? "F" : "O"),
                            sodienthoai = resultApiTH.dienthoai,
                            diachi = resultApiTH.diachi,
                            sottkh = customer_code.Substring(5),
                            maduan = maDuAn
                        };
                        customer.city_code = cityCode;
                        if (!string.IsNullOrEmpty(resultApiTH.namsinh))
                            customer.namsinh = Convert.ToInt32(resultApiTH.namsinh);

                        if (!string.IsNullOrEmpty(resultApiTH.doituong))
                        {
                            customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiTH.doituong).id;
                        }

                        if (!string.IsNullOrEmpty(resultApiTH.ngay))
                            customer.ngaytiepcan = DateTime.ParseExact(resultApiTH.ngay, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        khachHangs.Add(customer);
                        //CreateCustomer(customer);
                    }

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiTH.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    if (!string.IsNullOrEmpty(resultApiTH.ngay))
                    {
                        ngaynhap = resultApiTH.ngay;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    day = ngaynhapD.Day;
                    month = ngaynhapD.Month;
                    year = ngaynhapD.Year;

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_SL_ACE
                    tongHop = new BVTL_BO_BIEU_MAU_KH_BAO_CAO()
                    {
                        makh = customer_code,
                        ngaysl = ngaynhapD,
                        ngaysl_date = day,
                        ngaysl_month = month,
                        ngaysl_year = year,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn,
                        sottkh = customer_code.Substring(5)
                    };

                    tongHop.record_id = string.IsNullOrEmpty(resultApiTH.record_id) ? 0 : Convert.ToInt32(resultApiTH.record_id);
                    tongHop.hanhvinguyco_timestamp = ngaynhapD;
                    tongHop.chatgaynghien = resultApiTH.chatgaynghien;
                    tongHop.loaikhac = resultApiTH.khac1;
                    tongHop.chatgaynghien_2 = resultApiTH.chatgaynghien_2;
                    tongHop.loaikhac_2 = resultApiTH.khac2;
                    tongHop.duongsd = resultApiTH.duongsd;
                    tongHop.sdheroin = resultApiTH.sdheroin;
                    tongHop.tansuatda = resultApiTH.tansuatda;
                    tongHop.tansuat_heroin = resultApiTH.tansuat_heroin;
                    tongHop.tuoi = string.IsNullOrEmpty(resultApiTH.tuoi) ? 0 : Convert.ToInt32(resultApiTH.tuoi);
                    tongHop.matuydautien = resultApiTH.matuydautien;
                    tongHop.tiemchich = resultApiTH.tiemchich;
                    tongHop.dungchung = resultApiTH.dungchung;
                    tongHop.qhtd = resultApiTH.qhtd;
                    tongHop.qhtd_2 = resultApiTH.qhtd_2;
                    tongHop.sdmatuy = resultApiTH.sdmatuy;
                    tongHop.qhtdtt = resultApiTH.qhtdtt;
                    tongHop.bandam = resultApiTH.bandam;
                    tongHop.sti = resultApiTH.sti;
                    tongHop.sti1 = resultApiTH.sti1;
                    tongHop.loaikhac_3 = resultApiTH.khac3;
                    tongHop.quakhu = resultApiTH.quakhu;
                    tongHop.hientai = resultApiTH.hientai;
                    tongHop.hientai_2 = resultApiTH.hientai_2;
                    tongHop.quakhu2 = resultApiTH.quakhu_2;
                    tongHop.hientai_4 = resultApiTH.hientai_4;
                    tongHop.hientai_3 = resultApiTH.hientai_3;
                    tongHop.quakhu_3 = resultApiTH.quakhu_3;
                    tongHop.hientai_5 = resultApiTH.hientai_5;
                    tongHop.hientai_6 = resultApiTH.hientai_6;
                    tongHop.trieuchung = resultApiTH.trieuchung;
                    tongHop.trieuchung_2 = resultApiTH.trieuchung_2;
                    tongHop.hiv_timestamp = ngaynhapD;
                    tongHop.cau1 = resultApiTH.cau1;
                    tongHop.cau2 = resultApiTH.cau2;
                    tongHop.cau3 = resultApiTH.cau3;
                    tongHop.cau4 = resultApiTH.cau4;
                    tongHop.cau5 = resultApiTH.cau5;
                    tongHop.cau6 = resultApiTH.cau6;
                    tongHop.cau7 = resultApiTH.cau7;
                    tongHop.cau8 = resultApiTH.cau8;
                    tongHop.cau9 = resultApiTH.cau9;
                    tongHop.cau10 = resultApiTH.cau10;
                    tongHop.cau11 = resultApiTH.cau11;
                    tongHop.cau12 = resultApiTH.cau12;
                    tongHop.cau13 = resultApiTH.cau13;
                    tongHop.cau14 = resultApiTH.cau14;
                    tongHop.cau15 = resultApiTH.cau15;
                    tongHop.cau16 = resultApiTH.cau16;
                    tongHop.cau17 = resultApiTH.cau17;
                    tongHop.cau18 = resultApiTH.cau18;
                    tongHop.cau19 = resultApiTH.cau19;
                    tongHop.cau20 = resultApiTH.cau20;
                    tongHop.cau21 = resultApiTH.cau21;
                    tongHop.cau22 = resultApiTH.cau22;
                    tongHop.cau23 = resultApiTH.cau23;
                    tongHop.cau24 = resultApiTH.cau24;
                    tongHop.cau25 = resultApiTH.cau25;
                    tongHop.cau26 = resultApiTH.cau26;
                    tongHop.cau27 = resultApiTH.cau27;
                    tongHop.assist_timestamp = ngaynhapD;
                    tongHop.thuocla = resultApiTH.thuocla;
                    tongHop.thucuong = resultApiTH.thucuong;
                    tongHop.cansa = resultApiTH.cansa;
                    tongHop.cocain = resultApiTH.cocain;
                    tongHop.chatkichthich = resultApiTH.chatkichthich;
                    tongHop.khixong = resultApiTH.khixong;
                    tongHop.thuocanthan = resultApiTH.thuocanthan;
                    tongHop.chatgayaogiac = resultApiTH.chatgayaogiac;
                    tongHop.thuocphien = resultApiTH.thuocphien;
                    tongHop.chatkhac = resultApiTH.chatkhac;
                    tongHop.cacchatkhac = resultApiTH.cacchatkhac;
                    tongHop.lucdihoc = resultApiTH.lucdihoc;
                    tongHop.thuocla1 = resultApiTH.thuocla1;
                    tongHop.thucuong1 = resultApiTH.thucuong1;
                    tongHop.cansa1 = resultApiTH.cansa1;
                    tongHop.coca1 = resultApiTH.coca1;
                    tongHop.chatkichthich1 = resultApiTH.chatkichthich1;
                    tongHop.khixong1 = resultApiTH.khixong1;
                    tongHop.thuocanthan1 = resultApiTH.thuocanthan1;
                    tongHop.chatgayaogiac1 = resultApiTH.chatgayaogiac1;
                    tongHop.chatthuocphien1 = resultApiTH.chatthuocphien1;
                    tongHop.chatkhac1 = resultApiTH.chatkhac1;
                    tongHop.thuocla2 = resultApiTH.thuocla2;
                    tongHop.thucuong2 = resultApiTH.thucuong2;
                    tongHop.cansa2 = resultApiTH.cansa2;
                    tongHop.coca2 = resultApiTH.coca2;
                    tongHop.chatkichthich2 = resultApiTH.chatkichthich2;
                    tongHop.khixong2 = resultApiTH.khixong2;
                    tongHop.thuocanthan2 = resultApiTH.thuocanthan2;
                    tongHop.chatgayaogiac2 = resultApiTH.chatgayaogiac2;
                    tongHop.chatthuocphien2 = resultApiTH.chatthuocphien2;
                    tongHop.chatkhac2 = resultApiTH.chatkhac2;
                    tongHop.thuocla3 = resultApiTH.thuocla3;
                    tongHop.thucuong3 = resultApiTH.thucuong3;
                    tongHop.cansa3 = resultApiTH.cansa3;
                    tongHop.coca3 = resultApiTH.coca3;
                    tongHop.chatkichthich3 = resultApiTH.chatkichthich3;
                    tongHop.khixong3 = resultApiTH.khixong3;
                    tongHop.thuocanthan3 = resultApiTH.thuocanthan3;
                    tongHop.chatgayaogiac3 = resultApiTH.chatgayaogiac3;
                    tongHop.chatthuocphien3 = resultApiTH.chatthuocphien3;
                    tongHop.chatkhac3 = resultApiTH.chatkhac3;
                    tongHop.thucuong4 = resultApiTH.thucuong4;
                    tongHop.cansa4 = resultApiTH.cansa4;
                    tongHop.coca4 = resultApiTH.coca4;
                    tongHop.chatkichthich4 = resultApiTH.chatkichthich4;
                    tongHop.khixong4 = resultApiTH.khixong4;
                    tongHop.thuocanthan4 = resultApiTH.thuocanthan4;
                    tongHop.chatgayaogiac4 = resultApiTH.chatgayaogiac4;
                    tongHop.chatthuocphien4 = resultApiTH.chatthuocphien4;
                    tongHop.chatkhac4 = resultApiTH.chatkhac4;
                    tongHop.thuocla5 = resultApiTH.thuocla5;
                    tongHop.thucuong5 = resultApiTH.thucuong5;
                    tongHop.cansa5 = resultApiTH.cansa5;
                    tongHop.coca5 = resultApiTH.coca5;
                    tongHop.chatkichthich5 = resultApiTH.chatkichthich5;
                    tongHop.khixong5 = resultApiTH.khixong5;
                    tongHop.thuocanthan5 = resultApiTH.thuocanthan5;
                    tongHop.chatgayaogiac5 = resultApiTH.chatgayaogiac5;
                    tongHop.chatthuocphien5 = resultApiTH.chatthuocphien5;
                    tongHop.chatkhac5 = resultApiTH.chatkhac5;
                    tongHop.thuocla6 = resultApiTH.thuocla6;
                    tongHop.thucuong6 = resultApiTH.thucuong6;
                    tongHop.cansa6 = resultApiTH.cansa6;
                    tongHop.coca6 = resultApiTH.coca6;
                    tongHop.chatkichthich6 = resultApiTH.chatkichthich6;
                    tongHop.khixong6 = resultApiTH.khixong6;
                    tongHop.thuocanthan6 = resultApiTH.thuocanthan6;
                    tongHop.chatgayaogiac6 = resultApiTH.chatgayaogiac6;
                    tongHop.chatthuocphien6 = resultApiTH.chatthuocphien6;
                    tongHop.chatkhac6 = resultApiTH.chatkhac6;
                    tongHop.cau_8 = resultApiTH.cau_8;
                    tongHop.diemthuocla = resultApiTH.diemthuocla;
                    tongHop.diemthucuong = resultApiTH.diemthucuong;
                    tongHop.diemcansa = resultApiTH.diemcansa;
                    tongHop.diemcoca = resultApiTH.diemcoca;
                    tongHop.diemchatkichthich = resultApiTH.diemchatkichthich;
                    tongHop.diemkhixong = resultApiTH.diemkhixong;
                    tongHop.diemchatanthan = resultApiTH.diemchatanthan;
                    tongHop.diemchatgayaogiac = resultApiTH.diemchatgayaogiac;
                    tongHop.diemchatthuocphien = resultApiTH.diemchatthuocphien;
                    tongHop.qst_timestamp = ngaynhapD;
                    tongHop.diemchatkhac = resultApiTH.diemchatkhac;
                    tongHop.c_1a = resultApiTH.c_1a;
                    tongHop.c_1b = resultApiTH.c_1b;
                    tongHop.c_1c = resultApiTH.c_1c;
                    tongHop.c_1d = resultApiTH.c_1d;
                    tongHop.c_2 = resultApiTH.c_2;
                    tongHop.c_3 = resultApiTH.c_3;
                    tongHop.c_4a = resultApiTH.c_4a;
                    tongHop.c_4b = resultApiTH.c_4b;
                    tongHop.c_4c = resultApiTH.c_4c;
                    tongHop.tongdiem = string.IsNullOrEmpty(resultApiTH.tongdiem) ? 0 : Convert.ToInt32(resultApiTH.tongdiem);

                    tongHops.Add(tongHop);

                    #endregion
                    
                }
                log.Info("*********-----SỐ BẢN GHI TONG HOP BIEU_MAU_KH_BAO_CAO ĐÃ CONVERT:" + tongHops.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);


            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api tổng hợp sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api tổng hợp sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api phiếu tư vấn sang entity BVTL_PHIEU_TU_VAN
        /// </summary>
        /// <param name="resultApiPTVs"></param>
        /// <param name="phieuTuVans"></param>
        public void ConvertApiPhieuTuVanToEntity(List<ResultApiPhieuTuVanModel> resultApiPTVs, string maDuAn, ref List<BVTL_PHIEU_TU_VAN> phieuTuVans)
        {

            log.Info("*********Bắt đầu chuyển đổi kết quả api phiếu tư vấn sang entity**************************************");
            log.Info("*********-----TỔNG SỐ RECORD API PHIEUTUVAN:" + resultApiPTVs.Count + " | MADUAN:" + maDuAn);
            try
            {
                var phieuTuVan = new BVTL_PHIEU_TU_VAN();
                var resultApiPTV = new ResultApiPhieuTuVanModel();
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                long record_id_max = db.BVTL_PHIEU_TU_VAN.Where(x => x != null).DefaultIfEmpty().Max(x => x == null ? 0 : x.record_id);

                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();

                int errNo = 0;
                for (int i = 0; i < resultApiPTVs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApiPTV = resultApiPTVs[i];


                    // Updated 07/11/2022: API Phiếu tư vấn thêm các trường makh_2,makh_3... và các trường matcv_2, matcv_3... tương ứng
                    // makh trong bảng dữ liệu giữ nguyên, makh = makh_2 -> makh_10 nếu 1 trong 10 makh đó có giá trị (trong 10 trường sẽ tồn tại 1 trường có dữ liệu)
                    //customer_code = resultApiPTV.makh;
                    customer_code = String.Concat(resultApiPTV.makh, resultApiPTV.makh_2, resultApiPTV.makh_3, resultApiPTV.makh_4, resultApiPTV.makh_5, resultApiPTV.makh_6, resultApiPTV.makh_7, resultApiPTV.makh_8, resultApiPTV.makh_9, resultApiPTV.makh_10);
                    phieuTuVan = new BVTL_PHIEU_TU_VAN()
                    {
                        makh = customer_code,
                        sottkh = customer_code.Substring(5),
                        maduan = maDuAn
                    };

                    if (!string.IsNullOrEmpty(customer_code))
                    {
                        if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                        {
                            group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                            cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        }
                        else if (!string.IsNullOrEmpty(customer_code))
                        {
                            cityCode = customer_code.Substring(0, 3);
                            group_code = customer_code.Substring(0, 5);
                        }

                        #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu
                        // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                        /*
                        customer = customers.FirstOrDefault(x => x.makh == customer_code);
                        if (customer != null && customer.khachhang_id > 0)
                        {
                            customer_id = customer.khachhang_id;
                        }
                        else
                        {
                            //customer = new BVTL_KHACH_HANG
                            //{
                            //    makh = resultApiPTV.makh,
                            //    hoten = string.IsNullOrEmpty(resultApiPTV.hoten) ? resultApiPTV.makh : resultApiPTV.hoten,
                            //    gioitinh = resultApiPTV.gioitinh == "Nam" ? "M" : (resultApiPTV.gioitinh == "Nữ" ? "F" : "O"),
                            //    sodienthoai = resultApiPTV.dienthoai,
                            //    diachi = resultApiPTV.diachi,
                            //    sottkh = customer_code.Substring(5)
                            //};
                            //customer.city_code = cityCode;
                            //if (!string.IsNullOrEmpty(resultApiPTV.namsinh))
                            //    customer.namsinh = Convert.ToInt32(resultApiPTV.namsinh);

                            //if (!string.IsNullOrEmpty(resultApiPTV.doituong))
                            //{
                            //    customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiPTV.doituong).id;
                            //}

                            //if (!string.IsNullOrEmpty(resultApiPTV.ngaytuvan))
                            //    customer.ngaytiepcan = DateTime.ParseExact(resultApiPTV.ngaytuvan, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                            //customer_id = CreateCustomer(customer);

                            // Lấy thông tin khách hàng
                            var customerCK = db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == customer_code);
                            if (customerCK != null && customerCK.khachhang_id > 0)
                                customer_id = customerCK.khachhang_id;
                        }
                        */

                        // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                        nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                        if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                        {
                            db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiPTV.tbh, city_code = cityCode });
                            db.SaveChanges();
                        }
                        // Lấy ngay, tháng, năm nhập dữ liệu
                        // EDIT: LẤY NGÀY TƯ VẤN, NẾU NGÀY TƯ VẤN KHÔNG CÓ THÌ LẤY NGÀY NHẬP
                        if (!string.IsNullOrEmpty(resultApiPTV.ngaytuvan))
                        {
                            ngaynhap = resultApiPTV.ngaytuvan.Split(' ')[0];
                            ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                            day = ngaynhapD.Day;
                            month = ngaynhapD.Month;
                            year = ngaynhapD.Year;
                        }
                        else
                        {
                            ngaynhap = resultApiPTV.ngaynhap.Split(' ')[0];
                            ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                            day = ngaynhapD.Day;
                            month = ngaynhapD.Month;
                            year = ngaynhapD.Year;
                        }

                        #endregion

                        #region Chuyển đổi dữ liệu sang bảng BVTL_PHIEU_TU_VAN

                        //phieuTuVan.record_id = string.IsNullOrEmpty(resultApiPTV.record_id) ? 0 : Convert.ToInt32(resultApiPTV.record_id);

                        phieuTuVan.ngaytuvan = ngaynhapD;
                        phieuTuVan.ngaytuvan_date = day;
                        phieuTuVan.ngaytuvan_month = month;
                        phieuTuVan.ngaytuvan_year = year;
                        phieuTuVan.ngaynhap = ngaynhapD;
                        phieuTuVan.city_code = cityCode;
                        phieuTuVan.manhom_tbh = group_code;

                        if (!string.IsNullOrEmpty(resultApiPTV.ngaytuvan))
                            phieuTuVan.ngaytuvan = DateTime.ParseExact(resultApiPTV.ngaytuvan, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                        phieuTuVan.diadiem = resultApiPTV.diadiem;
                        phieuTuVan.matcv = String.Concat(resultApiPTV.matcv, resultApiPTV.matcv_2, resultApiPTV.matcv_3, resultApiPTV.matcv_4, resultApiPTV.matcv_5, resultApiPTV.matcv_6, resultApiPTV.matcv_7, resultApiPTV.matcv_8, resultApiPTV.matcv_9, resultApiPTV.matcv_10);
                        phieuTuVan.lantuvan = resultApiPTV.lantuvan;
                        phieuTuVan.cau1_1 = resultApiPTV.cau1_1;
                        phieuTuVan.cau1_1k = resultApiPTV.cau1_1k;
                        phieuTuVan.cau1_2 = resultApiPTV.cau1_2;
                        phieuTuVan.cau1_3 = resultApiPTV.cau1_3;
                        phieuTuVan.cau1_4 = resultApiPTV.cau1_4;
                        phieuTuVan.cau1_5 = resultApiPTV.cau1_5;
                        phieuTuVan.cau2 = resultApiPTV.cau2;
                        phieuTuVan.cau2_1k = resultApiPTV.cau2_1k;
                        phieuTuVan.cau2_2 = resultApiPTV.cau2_2;
                        phieuTuVan.cau3 = resultApiPTV.cau3;
                        phieuTuVan.cau3_1k = resultApiPTV.cau3_1k;
                        phieuTuVan.cau3_1k_2 = resultApiPTV.cau3_1k_2;
                        phieuTuVan.cau4 = resultApiPTV.cau4;
                        phieuTuVan.cau4_1k = resultApiPTV.cau4_1k;
                        phieuTuVan.cau4_1k_2 = resultApiPTV.cau4_1k_2;
                        phieuTuVan.cau5_1 = resultApiPTV.cau5_1;
                        phieuTuVan.cau5_1k = resultApiPTV.cau5_1k;
                        phieuTuVan.cau5_1_2 = resultApiPTV.cau5_1_2;
                        phieuTuVan.cau5_2 = resultApiPTV.cau5_2;
                        phieuTuVan.cau5_2k = resultApiPTV.cau5_2k;
                        phieuTuVan.cau5_2k_2 = resultApiPTV.cau5_2k_2;
                        phieuTuVan.cau5_3 = resultApiPTV.cau5_3;
                        phieuTuVan.cau5_3_1 = resultApiPTV.cau5_3_1;
                        phieuTuVan.cau5_3_2 = resultApiPTV.cau5_3_2;
                        phieuTuVan.cau5_4 = resultApiPTV.cau5_4;
                        phieuTuVan.cau5_4_1 = resultApiPTV.cau5_4_1;
                        phieuTuVan.cau5_5 = resultApiPTV.cau5_5;
                        phieuTuVan.cau5_4_2 = resultApiPTV.cau5_4_2;
                        phieuTuVan.cau5_6 = resultApiPTV.cau5_6;
                        phieuTuVan.cau5_6k = resultApiPTV.cau5_6k;
                        phieuTuVan.cau5_6k_2 = resultApiPTV.cau5_6k_2;
                        phieuTuVan.cau5_7 = resultApiPTV.cau5_7;
                        phieuTuVan.cau5_7k = resultApiPTV.cau5_7k;
                        phieuTuVan.cau5_7k_3 = resultApiPTV.cau5_7k_3;
                        phieuTuVan.cau5_8 = resultApiPTV.cau5_8;
                        phieuTuVan.cau5_7k_2 = resultApiPTV.cau5_7k_2;
                        phieuTuVan.tongket = resultApiPTV.tongket;
                        phieuTuVan.tuvantiep = resultApiPTV.tuvantiep;
                        phieuTuVan.vande = resultApiPTV.vande;
                        if (!string.IsNullOrEmpty(resultApiPTV.thoigian))
                            phieuTuVan.thoigian = DateTime.ParseExact(resultApiPTV.thoigian, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);


                        phieuTuVans.Add(phieuTuVan);

                    }

                    #endregion
                    
                   
                }

                log.Info("*********-----SỐ Record PHIEUTUVAN ĐÃ CONVERT:" + phieuTuVans.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                
            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả api phiếu tư vấn sang entity lỗi: " + ex.Message);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api phiếu tư vấn sang entity**************************************");
        }

        /// <summary>
        /// CHuyển đổi kết quả api chuyển gửi dịch vụ sang entity BVTL_CHUYEN_GUI_DICH_VU
        /// </summary>
        /// <param name="resultApiCGDVs"></param>
        /// <param name="chuyenGuiDVs"></param>
        public void ConvertApiChuyenGuiDichVuToEntity(List<ResultApiChuyenGuiDVModel> resultApiCGDVs, string maDuAn, ref List<BVTL_CHUYEN_GUI_DICH_VU> chuyenGuiDVs)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api chuyển gửi dịch vụ sang entity**************************************");
            var chuyenGuiDV = new BVTL_CHUYEN_GUI_DICH_VU();
            var resultApiCGDV = new ResultApiChuyenGuiDVModel();
            try
            {
                
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                //var customer_id = 0;
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                var sottkh = "";

                log.Info("*********-----TỔNG SỐ RECORD API CHUYEN_GUI_DICH_VU:" + resultApiCGDVs.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiCGDVs.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApiCGDV = resultApiCGDVs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApiCGDV.makh, resultApiCGDV.makh_2, resultApiCGDV.makh_3, resultApiCGDV.makh_4, resultApiCGDV.makh_5, resultApiCGDV.makh_6, resultApiCGDV.makh_7, resultApiCGDV.makh_8, resultApiCGDV.makh_9, resultApiCGDV.makh_10);
                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        sottkh = customer_code.Substring(5);
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                        sottkh = customer_code.Substring(5);
                    }

                    // Kiểm tra xem đã tồn tại khách hàng chưa, nếu chưa thì thêm mới
                    //customer = customers.FirstOrDefault(x => x.makh == customer_code);
                    //if (customer != null && customer.khachhang_id > 0)
                    //{
                    //    customer_id = customer.khachhang_id;
                    //}
                    //else
                    //{
                    //    //customer = new BVTL_KHACH_HANG
                    //    //{
                    //    //    makh = resultApiCGDV.makh,
                    //    //    hoten = string.IsNullOrEmpty(resultApiCGDV.hoten) ? resultApiCGDV.makh : resultApiCGDV.hoten,
                    //    //    gioitinh = resultApiCGDV.gioitinh == "Nam" ? "M" : (resultApiCGDV.gioitinh == "Nữ" ? "F" : "O"),
                    //    //    sodienthoai = resultApiCGDV.dienthoai,
                    //    //    diachi = resultApiCGDV.diachi,
                    //    //    sottkh = customer_code.Substring(5)
                    //    //};
                    //    //customer.city_code = cityCode;
                    //    //if (!string.IsNullOrEmpty(resultApiCGDV.namsinh))
                    //    //    customer.namsinh = Convert.ToInt32(resultApiCGDV.namsinh);

                    //    //if (!string.IsNullOrEmpty(resultApiCGDV.doituong))
                    //    //{
                    //    //    customer.loai_doi_tuong_id = loaiDoiTuongs.FirstOrDefault(x => x.code == resultApiCGDV.doituong).id;
                    //    //}

                    //    //if (!string.IsNullOrEmpty(resultApiCGDV.ngay_xn))
                    //    //    customer.ngaytiepcan = DateTime.ParseExact(resultApiCGDV.ngay_xn, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    //    //customer_id = CreateCustomer(customer);

                    //    // Lấy thông tin khách hàng
                    //    var customerCK = db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == customer_code);
                    //    if (customerCK != null && customerCK.khachhang_id > 0)
                    //        customer_id = customerCK.khachhang_id;
                    //}

                    #endregion
                    #region Chuyển đổi dữ liệu sang bảng BVTL_CHUYEN_GUI_DICH_VU
                    chuyenGuiDV = new BVTL_CHUYEN_GUI_DICH_VU()
                    {
                        makh = customer_code,
                        manhom_tbh = group_code,
                        sottkh = sottkh,
                        city_code = cityCode,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApiCGDV.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApiCGDV.ngay_bddt))
                    {
                        ngaynhap = resultApiCGDV.ngay_bddt;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    else if(!string.IsNullOrEmpty(resultApiCGDV.ngaykxn))
                    {
                        ngaynhap = resultApiCGDV.ngaykxn;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }else if(!string.IsNullOrEmpty(resultApiCGDV.ngay_xn))
                    {
                        ngaynhap = resultApiCGDV.ngay_xn;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }else if(!string.IsNullOrEmpty(resultApiCGDV.ngaykham))
                    {
                        ngaynhap = resultApiCGDV.ngaykham;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    else if(!string.IsNullOrEmpty(resultApiCGDV.ngayhotro_2))
                    {
                        ngaynhap = resultApiCGDV.ngayhotro_2;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }


                    chuyenGuiDV.ngay_xn = ngaynhapD;
                    chuyenGuiDV.ngay_xn_date = day;
                    chuyenGuiDV.ngay_xn_month = month;
                    chuyenGuiDV.ngay_xn_year = year;
                    chuyenGuiDV.record_id = string.IsNullOrEmpty(resultApiCGDV.record_id) ? 0 : Convert.ToInt32(resultApiCGDV.record_id);
                    chuyenGuiDV.loaihinh = resultApiCGDV.loaihinh;
                    chuyenGuiDV.diachi_xn = resultApiCGDV.diachi_xn;
                    chuyenGuiDV.kq_xn = resultApiCGDV.kq_xn;
                    chuyenGuiDV.dieutri = resultApiCGDV.dieutri;
                    chuyenGuiDV.diachi_cg = resultApiCGDV.diachi_cg;

                    if (!string.IsNullOrEmpty(resultApiCGDV.ngay_bddt))
                        chuyenGuiDV.ngay_bddt = DateTime.ParseExact(resultApiCGDV.ngay_bddt, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    chuyenGuiDV.taiuong_bd = resultApiCGDV.taiuong_bd;
                    chuyenGuiDV.anh1 = resultApiCGDV.anh1;
                    chuyenGuiDV.taiuong_bd_2 = resultApiCGDV.taiuong_bd_2;
                    chuyenGuiDV.anh2 = resultApiCGDV.anh2;
                    chuyenGuiDV.taiuong_bd_21 = resultApiCGDV.taiuong_bd_21;
                    chuyenGuiDV.anh3 = resultApiCGDV.anh3;
                    chuyenGuiDV.s3t = resultApiCGDV.s3t;
                    chuyenGuiDV.s6t = resultApiCGDV.s6t;
                    chuyenGuiDV.s9t = resultApiCGDV.s9t;
                    chuyenGuiDV.s12t = resultApiCGDV.s12t;

                    if (!string.IsNullOrEmpty(resultApiCGDV.ngaykham))
                        chuyenGuiDV.ngaykham = DateTime.ParseExact(resultApiCGDV.ngaykham, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    chuyenGuiDV.diachikham = resultApiCGDV.diachikham;
                    chuyenGuiDV.lankham = resultApiCGDV.lankham;
                    chuyenGuiDV.chandoan = resultApiCGDV.chandoan;
                    chuyenGuiDV.khac = resultApiCGDV.khac;
                    chuyenGuiDV.kedon = resultApiCGDV.kedon;
                    chuyenGuiDV.dungthuoc = resultApiCGDV.dungthuoc;
                    chuyenGuiDV.hotro = resultApiCGDV.hotro;

                    if (!string.IsNullOrEmpty(resultApiCGDV.ngaykxn))
                        chuyenGuiDV.ngaykxn = DateTime.ParseExact(resultApiCGDV.ngaykxn, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    chuyenGuiDV.diachi_kxn = resultApiCGDV.diachi_kxn;
                    chuyenGuiDV.lan_xnk = resultApiCGDV.lan_xnk;
                    chuyenGuiDV.chandoan1 = resultApiCGDV.chandoan1;
                    chuyenGuiDV.khac_sti = resultApiCGDV.khac_sti;
                    chuyenGuiDV.dieutri_sti = resultApiCGDV.dieutri_sti;
                    chuyenGuiDV.hotro_sti = resultApiCGDV.hotro_sti;
                    chuyenGuiDV.hotro_bhyt = resultApiCGDV.hotro_bhyt;

                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_gttt))
                        chuyenGuiDV.hotro_gttt = resultApiCGDV.hotro_gttt;
                    if (!string.IsNullOrEmpty(resultApiCGDV.ngayhotro_2))
                        chuyenGuiDV.ngayhotro_2 = DateTime.ParseExact(resultApiCGDV.ngayhotro_2, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_prep))
                        chuyenGuiDV.hotro_prep = resultApiCGDV.hotro_prep;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_pep))
                        chuyenGuiDV.hotro_pep = resultApiCGDV.hotro_pep;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_c))
                        chuyenGuiDV.hotro_c = resultApiCGDV.hotro_c;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_c_2))
                        chuyenGuiDV.hotro_c_2 = resultApiCGDV.hotro_c_2;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_lao))
                        chuyenGuiDV.hotro_lao = resultApiCGDV.hotro_lao;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_lao_2))
                        chuyenGuiDV.hotro_lao_2 = resultApiCGDV.hotro_lao_2;
                    if (!string.IsNullOrEmpty(resultApiCGDV.hotro_mtd))
                        chuyenGuiDV.hotro_mtd = resultApiCGDV.hotro_mtd;

                    chuyenGuiDVs.Add(chuyenGuiDV);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record CHUYEN_GUI_DICH_VU ĐÃ CONVERT:" + chuyenGuiDVs.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CHUYEN_GUI_DICH_VU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + chuyenGuiDV.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api chuyển gửi dịch vụ sang entity**************************************");
        }
        public void ConvertApiXNNuocTieuToEntity(List<ResultApiXnNuocTieuModel> resultApiXnNuocTieus, string maDuAn, ref List<BVTL_KQ_XN_NUOC_TIEU> xnNuocTieus)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api XNNuocTieu sang entity**************************************");
            var xnNuocTieu = new BVTL_KQ_XN_NUOC_TIEU();
            var resultApi = new ResultApiXnNuocTieuModel();
            try
            {
                
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                
                var group_code = "";
                var cityCode = "";
                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                var sottkh = "";

                log.Info("*********-----TỔNG SỐ RECORD API BVTL_KQ_XN_NUOC_TIEU:" + resultApiXnNuocTieus.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiXnNuocTieus.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    //customer_id = 0;
                    group_code = "";
                    cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiXnNuocTieus[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.makh, resultApi.makh_2, resultApi.makh_3, resultApi.makh_4, resultApi.makh_5, resultApi.makh_6, resultApi.makh_7, resultApi.makh_8, resultApi.makh_9, resultApi.makh_10);
                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        sottkh = customer_code.Substring(5);
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                        sottkh = customer_code.Substring(5);
                    }

                    #endregion
                    #region Chuyển đổi dữ liệu sang bảng BVTL_KQ_XN_NUOC_TIEU
                    xnNuocTieu = new BVTL_KQ_XN_NUOC_TIEU()
                    {
                        makh = customer_code,
                        manhom_tbh = group_code,
                        sottkh = sottkh,
                        city_code = cityCode,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    {
                        db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi.tbh, city_code = cityCode });
                        db.SaveChanges();
                    }
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApi.ngayhoi))
                    {
                        ngaynhap = resultApi.ngayhoi;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    
                    xnNuocTieu.ngayhoi = ngaynhapD;
                    xnNuocTieu.ngayhoi_date = day;
                    xnNuocTieu.ngayhoi_month = month;
                    xnNuocTieu.ngayhoi_year = year;
                    xnNuocTieu.kqxnnt_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    if (!string.IsNullOrEmpty(resultApi.kqxnda))
                    {
                        if (resultApi.kqxnda?.Trim() == "Âm tính")
                            xnNuocTieu.kqxnda = 2;
                        else if (resultApi.kqxnda?.Trim() == "Dương tính")
                            xnNuocTieu.kqxnda = 1;
                        else
                            xnNuocTieu.kqxnda = 3;
                    }

                    if (!string.IsNullOrEmpty(resultApi.kqxnheroin))
                    {
                        if (resultApi.kqxnheroin?.Trim() == "Âm tính")
                            xnNuocTieu.kqxnheroin = 2;
                        else if (resultApi.kqxnheroin?.Trim() == "Dương tính")
                            xnNuocTieu.kqxnheroin = 1;
                        else
                            xnNuocTieu.kqxnheroin = 3;
                    }

                    if (!string.IsNullOrEmpty(resultApi.mop))
                    {
                        if (resultApi.mop?.Trim() == "Âm tính")
                            xnNuocTieu.kqxnheroin = 2;
                        else if (resultApi.mop?.Trim() == "Dương tính")
                            xnNuocTieu.kqxnheroin = 1;
                        else
                            xnNuocTieu.kqxnheroin = 3;
                    }

                    if (!string.IsNullOrEmpty(resultApi.thc))
                    {
                        if (resultApi.thc?.Trim() == "Âm tính")
                            xnNuocTieu.kqxnthc = 2;
                        else if (resultApi.thc?.Trim() == "Dương tính")
                            xnNuocTieu.kqxnthc = 1;
                        else
                            xnNuocTieu.kqxnthc = 3;
                    }

                    if (!string.IsNullOrEmpty(resultApi.mdma))
                    {
                        if (resultApi.mdma?.Trim() == "Âm tính")
                            xnNuocTieu.kqxnmdma = 2;
                        else if (resultApi.mdma?.Trim() == "Dương tính")
                            xnNuocTieu.kqxnmdma = 1;
                        else
                            xnNuocTieu.kqxnmdma = 3;
                    }
                    
                    xnNuocTieus.Add(xnNuocTieu);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record XN_NUOC_TIEU ĐÃ CONVERT:" + xnNuocTieus.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API XN_NUOC_TIEU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + xnNuocTieu.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api XN_NUOC_TIEU sang entity**************************************");
        }
        
        public void ConvertApiTTTTToEntity(List<ResultApiTTTTModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_THONG_TIN_TRUYEN_THONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api VIIV_THONG_TIN_TRUYEN_THONG sang entity**************************************");
            var objDB = new VIIV_THONG_TIN_TRUYEN_THONG();
            var resultApi = new ResultApiTTTTModel();
            try
            {
                
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                //var customer_code = "";
                
                var group_code = "HNO34";
                var cityCode = "";
                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp)) {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }

                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();                

                log.Info("*********-----TỔNG SỐ RECORD API VIIV_THONG_TIN_TRUYEN_THONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    //customer_code = "";
                    //customer_id = 0;
                    //group_code = "";
                    //cityCode = "";
                    //nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    //customer_code = String.Concat(resultApi.makh);
                    //if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    //{
                    //    group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                    //    cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                    //    sottkh = customer_code.Substring(5);
                    //}
                    //else if (!string.IsNullOrEmpty(customer_code))
                    //{
                    //    cityCode = customer_code.Substring(0, 3);
                    //    group_code = customer_code.Substring(0, 5);
                    //    sottkh = customer_code.Substring(5);
                    //}

                    #endregion
                    #region Chuyển đổi dữ liệu sang bảng VIIV_THONG_TIN_TRUYEN_THONG
                    objDB = new VIIV_THONG_TIN_TRUYEN_THONG()
                    {
                        //makh = customer_code,
                        //manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    //nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    //if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    //{
                    //    db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi.tbh, city_code = cityCode });
                    //    db.SaveChanges();
                    //}
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApi.day))
                    {
                        ngaynhap = resultApi.day;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }

                    objDB.day = ngaynhapD;
                    objDB.ngayth_date = day;
                    objDB.ngayth_month = month;
                    objDB.ngayth_year = year;
                    objDB.record_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    if (!string.IsNullOrEmpty(resultApi.diadiem))
                    {
                        objDB.diadiem = resultApi.diadiem;
                    }

                    if (!string.IsNullOrEmpty(resultApi.noidung))
                    {
                        objDB.noidung = resultApi.noidung; 
                    }

                    if (!string.IsNullOrEmpty(resultApi.sokh))
                    {
                        objDB.sokh = Int32.Parse(resultApi.sokh);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.bcs))
                    {
                        objDB.bcs = Int32.Parse(resultApi.bcs);
                    }

                    if (!string.IsNullOrEmpty(resultApi.gel))
                    {
                        objDB.gel = Int32.Parse(resultApi.gel);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.bomkt))
                    {
                        objDB.bomkt = Int32.Parse(resultApi.bomkt);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.nuoccat))
                    {
                        objDB.nuoccat = Int32.Parse(resultApi.nuoccat);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.hopchiathuoc))
                    {
                        objDB.hopchiathuoc = Int32.Parse(resultApi.hopchiathuoc);
                    }

                    if (!string.IsNullOrEmpty(resultApi.ghichu))
                    {
                        objDB.ghichu = resultApi.ghichu;
                    }

                    lsObjDB.Add(objDB);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record VIIV - THONG TIN TRUYEN THONG ĐÃ CONVERT:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API VIIV - THONG TIN TRUYEN THONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api VIIV - THONG TIN TRUYEN THONG sang entity**************************************");
        }
        
        public void ConvertApiTrainingDataCollToEntity(List<ResultApiTrainingDataCollModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_TRAINING_DATA_COLLECTION> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api VIIV_TRAINING_DATA_COLLECTION sang entity**************************************");
            var objDB = new VIIV_TRAINING_DATA_COLLECTION();
            var resultApi = new ResultApiTrainingDataCollModel();
            try
            {
                
                //var customers = db.BVTL_KHACH_HANG.ToList();
                //var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                //var customer_code = "";
                
                var group_code = "HNO34";
                var cityCode = "";
                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp))
                {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }

                //var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                //var sottkh = "";

                log.Info("*********-----TỔNG SỐ RECORD API VIIV_TRAINING_DATA_COLLECTION:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    //customer_code = "";
                    //customer_id = 0;
                    //group_code = "";
                    //cityCode = "";
                    //nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    //customer_code = String.Concat(resultApi.makh);
                    //if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    //{
                    //    group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                    //    cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                    //    sottkh = customer_code.Substring(5);
                    //}
                    //else if (!string.IsNullOrEmpty(customer_code))
                    //{
                    //    cityCode = customer_code.Substring(0, 3);
                    //    group_code = customer_code.Substring(0, 5);
                    //    sottkh = customer_code.Substring(5);
                    //}

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng VIIV_TRAINING_DATA_COLLECTION
                    objDB = new VIIV_TRAINING_DATA_COLLECTION()
                    {                        
                        city_code = cityCode,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    //nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    //if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    //{
                    //    db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi.tbh, city_code = cityCode });
                    //    db.SaveChanges();
                    //}
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApi.thoigian))
                    {
                        ngaynhap = resultApi.thoigian;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }

                    objDB.thoigian = ngaynhapD;
                    objDB.ngayth_date = day;
                    objDB.ngayth_month = month;
                    objDB.ngayth_year = year;
                    objDB.record_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    if (!string.IsNullOrEmpty(resultApi.doituong))
                    {
                        objDB.doituong = resultApi.doituong;
                    }

                    if (!string.IsNullOrEmpty(resultApi.taphuan))
                    {
                        objDB.taphuan = resultApi.taphuan;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.khac))
                    {
                        objDB.khac = resultApi.khac;
                    }

                    if (!string.IsNullOrEmpty(resultApi.noidung))
                    {
                        objDB.noidung = resultApi.noidung;
                    }

                    if (!string.IsNullOrEmpty(resultApi.nhataitro))
                    {
                        objDB.nhataitro = Int32.Parse(resultApi.nhataitro);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.nvngo))
                    {
                        objDB.nvngo = Int32.Parse(resultApi.nvngo);
                    }

                    if (!string.IsNullOrEmpty(resultApi.nvtccd))
                    {
                        objDB.nvtccd = Int32.Parse(resultApi.nvtccd);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.cbcqnn))
                    {
                        objDB.cbcqnn = Int32.Parse(resultApi.cbcqnn);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.nvtv))
                    {
                        objDB.nvtv = Int32.Parse(resultApi.nvtv);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.cbyt))
                    {
                        objDB.cbyt = Int32.Parse(resultApi.cbyt);
                    }

                    if (!string.IsNullOrEmpty(resultApi.scdi))
                    {
                        objDB.scdi = Int32.Parse(resultApi.scdi);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.khac1))
                    {
                        objDB.khac1 = Int32.Parse(resultApi.khac1);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.nhataitro_2))
                    {
                        objDB.nhataitro_2 = Int32.Parse(resultApi.nhataitro_2);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.ngo))
                    {
                        objDB.ngo = Int32.Parse(resultApi.ngo);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.tochuc1))
                    {
                        objDB.tochuc1 = Int32.Parse(resultApi.tochuc1);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.qlnn))
                    {
                        objDB.qlnn = Int32.Parse(resultApi.qlnn);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.ccdv))
                    {
                        objDB.ccdv = Int32.Parse(resultApi.ccdv);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.ttcn))
                    {
                        objDB.ttcn = Int32.Parse(resultApi.ttcn);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.bc))
                    {
                        objDB.bc = Int32.Parse(resultApi.bc);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.scdi1))
                    {
                        objDB.scdi1 = Int32.Parse(resultApi.scdi1);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.scdi2))
                    {
                        objDB.scdi2 = Int32.Parse(resultApi.scdi2);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.tinh))
                    {
                        objDB.tinh = resultApi.tinh;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.tinh_2))
                    {
                        objDB.tinh_2 = resultApi.tinh_2;
                    }

                    lsObjDB.Add(objDB);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record VIIV_TRAINING_DATA_COLLECTION:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API VIIV_TRAINING_DATA_COLLECTION sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api VIIV_TRAINING_DATA_COLLECTION sang entity**************************************");
        }
        
        public void ConvertApiDGHLToEntity(List<ResultApiDGHLModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_DANH_GIA_HAI_LONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api VIIV_DANH_GIA_HAI_LONG sang entity**************************************");
            var objDB = new VIIV_DANH_GIA_HAI_LONG();
            var resultApi = new ResultApiDGHLModel();
            try
            {
                
                var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                
                var group_code = "";
                var cityCode = "";
                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp))
                {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }
                
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                //var sottkh = "";

                log.Info("*********-----TỔNG SỐ RECORD API VIIV_DANH_GIA_HAI_LONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    
                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.makh);
                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                    }

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng VIIV_DANH_GIA_HAI_LONG
                    objDB = new VIIV_DANH_GIA_HAI_LONG()
                    {
                        makh = customer_code,
                        city_code = cityCode,
                        manhom_tbh = group_code,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    //nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    //if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    //{
                    //    db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi.tbh, city_code = cityCode });
                    //    db.SaveChanges();
                    //}
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApi.ngay))
                    {
                        ngaynhap = resultApi.ngay;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }

                    objDB.ngay = ngaynhapD;
                    objDB.ngayth_date = day;
                    objDB.ngayth_month = month;
                    objDB.ngayth_year = year;
                    objDB.record_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    if (!string.IsNullOrEmpty(resultApi.lantuvan))
                    {
                        objDB.lantuvan = Int32.Parse(resultApi.lantuvan);
                    }

                    if (!string.IsNullOrEmpty(resultApi.cau1))
                    {
                        objDB.cau1 = Int32.Parse(resultApi.cau1);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.cau2))
                    {
                        objDB.cau2 = Int32.Parse(resultApi.cau2);
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.cau3))
                    {
                        objDB.cau3 = Int32.Parse(resultApi.cau3);
                    }
                                        
                    if (!string.IsNullOrEmpty(resultApi.cau4))
                    {
                        objDB.cau4 = Int32.Parse(resultApi.cau4);
                    }
                                       
                    if (!string.IsNullOrEmpty(resultApi.cau5))
                    {
                        objDB.cau5 = Int32.Parse(resultApi.cau5);
                    }
                                       
                    if (!string.IsNullOrEmpty(resultApi.cau6))
                    {
                        objDB.cau6 = resultApi.cau6;
                    }
                            
                    lsObjDB.Add(objDB);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record VIIV_DANH_GIA_HAI_LONG:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API VIIV_DANH_GIA_HAI_LONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api VIIV_DANH_GIA_HAI_LONG sang entity**************************************");
        }
        
        public void ConvertApiTTKHMaDaToEntity(List<ResultApiTTKHMaDaModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_TT_KH_MAT_DAU> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api VIIV_TT_KH_MAT_DAU sang entity**************************************");
            var objDB = new VIIV_TT_KH_MAT_DAU();
            var resultApi = new ResultApiTTKHMaDaModel();
            try
            {
                
                var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                
                var group_code = "";
                var cityCode = "";
                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp))
                {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }
                
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();
                //var sottkh = "";

                log.Info("*********-----TỔNG SỐ RECORD API VIIV_TT_KH_MAT_DAU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    customer = new BVTL_KHACH_HANG();
                    customer_code = "";
                    
                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.makh);
                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                        
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 5);
                    }

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng VIIV_TT_KH_MAT_DAU
                    objDB = new VIIV_TT_KH_MAT_DAU()
                    {
                        makh = customer_code,
                        city_code = cityCode,
                        manhom_tbh = group_code,
                        maduan = maDuAn
                    };

                    // Kiểm tra xem có nhóm tbh chưa nếu chua có thì thêm
                    //nhomTBH = nhomTBHs.FirstOrDefault(x => x.manhom_tbh == group_code);
                    //if (nhomTBH == null && !string.IsNullOrEmpty(group_code))
                    //{
                    //    db.BVTL_NHOM_TBH.Add(new BVTL_NHOM_TBH() { manhom_tbh = group_code, tennhom_tbh = resultApi.tbh, city_code = cityCode });
                    //    db.SaveChanges();
                    //}
                    // Lấy ngay, tháng, năm nhập dữ liệu
                    // Edit: Nếu ngày xét nghiệm không có thì lấy Ngày Khám
                    if (!string.IsNullOrEmpty(resultApi.ngaynl))
                    {
                        ngaynhap = resultApi.ngaynl;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.ngay_md))
                    {
                        var ngay_md = resultApi.ngay_md;
                        objDB.ngay_md = DateTime.ParseExact(ngay_md, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    }

                    objDB.ngaynl = ngaynhapD;
                    objDB.ngayth_date = day;
                    objDB.ngayth_month = month;
                    objDB.ngayth_year = year;

                    objDB.record_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                            
                    lsObjDB.Add(objDB);

                    #endregion
                    
                }
                log.Info("*********-----SỐ Record VIIV_TT_KH_MAT_DAU:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API VIIV_TT_KH_MAT_DAU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api VIIV_TT_KH_MAT_DAU sang entity**************************************");
        }





    }
}
