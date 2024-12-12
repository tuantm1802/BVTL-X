using Common.ICommon;
using log4net;
using Microsoft.SqlServer.Server;
using Model.Model;
using Model.ModelExtend.API;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                    sktt.record_id = resultApi1344.record_id;
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
                    assist.record_id = resultApi1344.record_id;
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
                    hiv.tinhtrang = resultApiHIV.tinhtrang;
                    hiv.arv = resultApiHIV.arv;
                    hiv.cs_dieutri = resultApiHIV.cs_dieutri;
                    hiv.record_id = resultApiHIV.record_id;
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
                    //customer_code = String.Concat(resultApiACE.makh, resultApiACE.makh_2, resultApiACE.makh_3, resultApiACE.makh_4, resultApiACE.makh_5, resultApiACE.makh_6, resultApiACE.makh_7, resultApiACE.makh_8, resultApiACE.makh_9, resultApiACE.makh_10
                    //    , resultApiACE.makh_11, resultApiACE.makh_12, resultApiACE.makh_13, resultApiACE.makh_14, resultApiACE.makh_15);

                    List<string> lsCustomerCode = new List<string> { resultApiACE.makh, resultApiACE.makh_2, resultApiACE.makh_3, resultApiACE.makh_4, resultApiACE.makh_5, resultApiACE.makh_6, resultApiACE.makh_7, resultApiACE.makh_8, resultApiACE.makh_9, resultApiACE.makh_10
                                                    , resultApiACE.makh_11, resultApiACE.makh_12, resultApiACE.makh_13, resultApiACE.makh_14, resultApiACE.makh_15};

                    customer_code = getValFromMultiFields(lsCustomerCode);
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
                        ace.record_id = resultApiACE.record_id;

                        aces.Add(ace);
                    }
                    

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
                            maduan = maDuAn,
                            manhom_tbh = group_code
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

                    tongHop.record_id_api = string.IsNullOrEmpty(resultApiTH.record_id) ? 0 : Convert.ToInt32(resultApiTH.record_id);
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
                //long record_id_max = db.BVTL_PHIEU_TU_VAN.Where(x => x != null).DefaultIfEmpty().Max(x => x == null ? 0 : x.record_id);

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
                    //customer_code = String.Concat(resultApiPTV.makh, resultApiPTV.makh_2, resultApiPTV.makh_3, resultApiPTV.makh_4, resultApiPTV.makh_5, resultApiPTV.makh_6, resultApiPTV.makh_7, resultApiPTV.makh_8, resultApiPTV.makh_9, resultApiPTV.makh_10
                    //                                , resultApiPTV.makh_11, resultApiPTV.makh_12, resultApiPTV.makh_13, resultApiPTV.makh_14, resultApiPTV.makh_15);

                    List<string> lsCustomerCode = new List<string> { resultApiPTV.makh, resultApiPTV.makh_2, resultApiPTV.makh_3, resultApiPTV.makh_4, resultApiPTV.makh_5, resultApiPTV.makh_6, resultApiPTV.makh_7, resultApiPTV.makh_8, resultApiPTV.makh_9, resultApiPTV.makh_10
                                                    , resultApiPTV.makh_11, resultApiPTV.makh_12, resultApiPTV.makh_13, resultApiPTV.makh_14, resultApiPTV.makh_15};

                    customer_code = getValFromMultiFields(lsCustomerCode);

                    if (!string.IsNullOrEmpty(customer_code))
                    {
                        phieuTuVan = new BVTL_PHIEU_TU_VAN()
                        {
                            makh = customer_code,
                            sottkh = customer_code.Substring(5),
                            maduan = maDuAn
                        };

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
                        //log.Info("##################LOI: " + i + " | SỐ Record_ID LỖI:" + resultApiPTV.record_id + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

                        #region Chuyển đổi dữ liệu sang bảng BVTL_PHIEU_TU_VAN

                        phieuTuVan.record_id_api = string.IsNullOrEmpty(resultApiPTV.record_id) ? 0 : Convert.ToInt32(resultApiPTV.record_id);

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
                        phieuTuVan.matcv = String.Concat(resultApiPTV.matcv, resultApiPTV.matcv_2, resultApiPTV.matcv_3, resultApiPTV.matcv_4, resultApiPTV.matcv_5, resultApiPTV.matcv_6, resultApiPTV.matcv_7, resultApiPTV.matcv_8, resultApiPTV.matcv_9, resultApiPTV.matcv_10
                                                        , resultApiPTV.matcv_11, resultApiPTV.matcv_12, resultApiPTV.matcv_13, resultApiPTV.matcv_14, resultApiPTV.matcv_15);
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
                    CultureInfo provider = CultureInfo.InvariantCulture;
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
                    sottkh = "";

                    resultApiCGDV = resultApiCGDVs[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    //customer_code = String.Concat(resultApiCGDV.makh, resultApiCGDV.makh_2, resultApiCGDV.makh_3, resultApiCGDV.makh_4, resultApiCGDV.makh_5, resultApiCGDV.makh_6, resultApiCGDV.makh_7, resultApiCGDV.makh_8, resultApiCGDV.makh_9, resultApiCGDV.makh_10
                    //                                , resultApiCGDV.makh_11, resultApiCGDV.makh_12, resultApiCGDV.makh_13, resultApiCGDV.makh_14, resultApiCGDV.makh_15);

                    List<string> lsCustomerCode = new List<string> { resultApiCGDV.makh, resultApiCGDV.makh_2, resultApiCGDV.makh_3, resultApiCGDV.makh_4, resultApiCGDV.makh_5, resultApiCGDV.makh_6, resultApiCGDV.makh_7, resultApiCGDV.makh_8, resultApiCGDV.makh_9, resultApiCGDV.makh_10
                                                    , resultApiCGDV.makh_11, resultApiCGDV.makh_12, resultApiCGDV.makh_13, resultApiCGDV.makh_14, resultApiCGDV.makh_15};

                    customer_code = getValFromMultiFields(lsCustomerCode);

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
                    if (!string.IsNullOrEmpty(sottkh))
                    {
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
                        
                        if (!string.IsNullOrEmpty(resultApiCGDV.ngay))
                        {
                            //provider = new CultureInfo("fr-FR");
                            //resultApiCGDV.ngay = resultApiCGDV.ngay.Replace("-", "/");
                            //chuyenGuiDV.ngay = DateTime.ParseExact(resultApiCGDV.ngay, "d/M/yyyy", provider);
                            chuyenGuiDV.ngay = DateTime.ParseExact(resultApiCGDV.ngay, "yyyy-mm-dd", provider);
                        }
                            
                        if (!string.IsNullOrEmpty(resultApiCGDV.ngay_ht))
                        {
                            //provider = new CultureInfo("fr-FR");
                            //resultApiCGDV.ngay_ht = resultApiCGDV.ngay_ht.Replace("-", "/");
                            //chuyenGuiDV.ngay_ht = DateTime.ParseExact(resultApiCGDV.ngay_ht, "d/M/yyyy", provider);
                            chuyenGuiDV.ngay_ht = DateTime.ParseExact(resultApiCGDV.ngay_ht, "yyyy-mm-dd", provider);

                        }

                        chuyenGuiDVs.Add(chuyenGuiDV);
                        }
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
                    //xnNuocTieu.kqxnnt_id = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    xnNuocTieu.kqxnnt_id = resultApi.record_id == null ? 0: (int)resultApi.record_id;

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
                    xnNuocTieu.record_id = resultApi.record_id;

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
        public void ConvertApiBVTLTTTTToEntity(List<ResultApiBVTLTTTTModel> resultApiModels, string maDuAn, string apiCode, ref List<BVTL_THONG_TIN_TRUYEN_THONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api BVTL_THONG_TIN_TRUYEN_THONG sang entity**************************************");
            var objDB = new BVTL_THONG_TIN_TRUYEN_THONG();
            var resultApi = new ResultApiBVTLTTTTModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                //var customer_code = "";

                var group_code = "";
                var customer_code = "";
                var cityCode = "";
                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp))
                {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }

                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();

                log.Info("*********-----TỔNG SỐ RECORD API BVTL_THONG_TIN_TRUYEN_THONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    resultApi = resultApiModels[i];

                    
                    //cityCode = "";
                    //nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    //customer = new BVTL_KHACH_HANG();
                    //customer_code = String.Concat(resultApi.makh, resultApi.makh_2, resultApi.makh_3, resultApi.makh_4, resultApi.makh_5, resultApi.makh_6, resultApi.makh_7, resultApi.makh_8, resultApi.makh_9, resultApi.makh_10
                    //                                , resultApi.makh_11, resultApi.makh_12, resultApi.makh_13, resultApi.makh_14, resultApi.makh_15);

                    List<string> listCodeMaKh = getListValFromMultiFields(resultApi);

                    //log.Info("Count: " + listCodeMaKh.Count + ":::::getListValFromMultiFields:::::" + listCodeMaKh.ToString());

                    for (int j = 0; j < listCodeMaKh.Count; j++)
                    {
                        customer_code = listCodeMaKh[j];
                        if (!string.IsNullOrEmpty(customer_code))
                        {
                            if (customer_code.Length > 11)
                            {
                                group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                                cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                            }
                            else if (!string.IsNullOrEmpty(customer_code))
                            {
                                cityCode = customer_code.Substring(0, 3);
                                group_code = customer_code.Substring(0, 5);
                            }
                            
                            #region Chuyển đổi dữ liệu sang bảng BVTL_THONG_TIN_TRUYEN_THONG
                            objDB = new BVTL_THONG_TIN_TRUYEN_THONG()
                            {
                                makh = customer_code,
                                manhom_tbh = group_code,
                                city_code = cityCode,
                                maduan = maDuAn
                            };
                            //objDB.city_code = cityCode;
                            //objDB.makh = customer_code;
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
                            objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
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
                            
                            if (!string.IsNullOrEmpty(resultApi.tailieu))
                            {
                                objDB.tailieu = Int32.Parse(resultApi.tailieu);
                            }

                            if (!string.IsNullOrEmpty(resultApi.ghichu))
                            {
                                objDB.ghichu = resultApi.ghichu;
                            }

                            if (!string.IsNullOrEmpty(resultApi.bvtl_thng_tin_truyn_thng_complete))
                            {
                                objDB.bvtl_thng_tin_truyn_thng_complete = resultApi.bvtl_thng_tin_truyn_thng_complete;
                            }

                            lsObjDB.Add(objDB);
                        }
                        
                    }
                    
                    #endregion

                }
                log.Info("*********-----SỐ Record BVTL - THONG TIN TRUYEN THONG ĐÃ CONVERT:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API BVTL - THONG TIN TRUYEN THONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api BVTL - THONG TIN TRUYEN THONG sang entity**************************************");
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
                    objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
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

                    if (!string.IsNullOrEmpty(resultApi.viiv_thng_tin_truyn_thng_complete))
                    {
                        objDB.viiv_thng_tin_truyn_thng_complete = resultApi.viiv_thng_tin_truyn_thng_complete;
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
                    objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
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
                    objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
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

                    objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    //objDB.record_id_api = resultApi.record_id_api;
                            
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

        public void ConvertApiBVTLTHEODAUKHoEntity(List<ResultApiTheoDauKHModel> resultApiModels, string maDuAn, string apiCode, ref List<BVTL_THEO_DAU_KH> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api BVTL_THEO_DAU_KH sang entity**************************************");
            var objDB = new BVTL_THEO_DAU_KH();
            var resultApi = new ResultApiTheoDauKHModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
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

                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();

                log.Info("*********-----TỔNG SỐ RECORD API BVTL_THEO_DAU_KH:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    customer_code = String.Concat(resultApi.makh, resultApi.makh_2, resultApi.makh_3, resultApi.makh_4, resultApi.makh_5, resultApi.makh_6, resultApi.makh_7, resultApi.makh_8, resultApi.makh_9, resultApi.makh_10);
                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Trim().Length > 11)
                    {
                        group_code = customer_code.Trim().Substring(1, 5); //Lấy mã nhóm TBH
                        cityCode = customer_code.Trim().Substring(1, 3); // Lấy id tỉnh
                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Trim().Substring(0, 3);
                        group_code = customer_code.Trim().Substring(0, 5);
                    }
                    #endregion
                    #region Chuyển đổi dữ liệu sang bảng BVTL_THEO_DAU_KH
                    objDB = new BVTL_THEO_DAU_KH()
                    {
                        makh = customer_code,
                        manhom_tbh = group_code,
                        city_code = cityCode,
                        maduan = maDuAn
                    };

                    if (!string.IsNullOrEmpty(resultApi.ngay))
                    {
                        ngaynhap = resultApi.ngay;
                        ngaynhapD = DateTime.ParseExact(ngaynhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        day = ngaynhapD.Day;
                        month = ngaynhapD.Month;
                        year = ngaynhapD.Year;
                    }

                    objDB.day = ngaynhapD;
                    objDB.ngayth_date = day;
                    objDB.ngayth_month = month;
                    objDB.ngayth_year = year;
                    objDB.record_id_api = string.IsNullOrEmpty(resultApi.record_id) ? 0 : Convert.ToInt32(resultApi.record_id);
                    if (!string.IsNullOrEmpty(resultApi.hinhthuc))
                    {
                        objDB.hinhthuc = resultApi.hinhthuc;
                    }

                    if (!string.IsNullOrEmpty(resultApi.hinhthuc_khac))
                    {
                        objDB.hinhthuc_khac = resultApi.hinhthuc_khac;
                    }

                    if (!string.IsNullOrEmpty(resultApi.kq))
                    {
                        objDB.kq = resultApi.kq;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.khac))
                    {
                        objDB.khac = resultApi.khac;
                    }

                    if (!string.IsNullOrEmpty(resultApi.ghichu))
                    {
                        objDB.ghichu = resultApi.ghichu;
                    }

                    if (!string.IsNullOrEmpty(resultApi.theo_du_kh_complete))
                    {
                        objDB.theo_du_kh_complete = resultApi.theo_du_kh_complete;
                    }

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record BVTL_THEO_DAU_KH ĐÃ CONVERT:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API BVTL_THEO_DAU_KH sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api BVTL_THEO_DAU_KH sang entity**************************************");
        }

        public void ConvertApiKhachHangTTCBEntity(List<ResultApiKhachHangTTCBModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_THONG_TIN_CO_BAN> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_THONG_TIN_CO_BAN sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_THONG_TIN_CO_BAN();
            var resultApi = new ResultApiKhachHangTTCBModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";
                

                log.Info("*********-----TỔNG SỐ RECORD API VIIV_TT_KH_MAT_DAU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_THONG_TIN_CO_BAN
                    objDB = new CD43_KHACH_HANG_THONG_TIN_CO_BAN()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,                        
                    };

                    if (!string.IsNullOrEmpty(resultApi.f1_q_a1))
                    {
                        var ngay_thang_nam_sinh = resultApi.f1_q_a1;
                        var ngaysinh = DateTime.ParseExact(ngay_thang_nam_sinh, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        objDB.ngay_thang_nam_sinh = ngaysinh;
                        var nam_sinh = ngaysinh.Year;
                        objDB.nam_sinh = nam_sinh;
                    }

                    if (!string.IsNullOrEmpty(resultApi.f1_q_a2))
                    {   
                        objDB.gioi_tinh = resultApi.f1_q_a2;
                    }
                   
                    if (!string.IsNullOrEmpty(resultApi.f1_q_a2_1))
                    {
                        objDB.gioi_tinh = resultApi.f1_q_a2_1;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.f1_q_a4))
                    {
                        objDB.cap_bac_hoc_van = resultApi.f1_q_a4;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.f1_q_a6))
                    {
                        objDB.nghe_nghiep = resultApi.f1_q_a6;
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.f1_q_a6_1))
                    {
                        objDB.nghe_nghiep_khac = resultApi.f1_q_a6_1;
                    }

                    objDB.thoi_gian_bat_dau = ValidateDateTimeRange(resultApi.time);
                    objDB.thoi_gian_ket_thuc = ValidateDateTimeRange(resultApi.end_time);
                    objDB.ngayhoi = ValidateDateTimeRange(resultApi.ngayhoi);
                    objDB.ngaychuyengui_f2_q_5_d = ValidateDateTimeRange(resultApi.f2_q_5_d);
                    objDB.ngaytuvan = ValidateDateTimeRange(resultApi.ngaytuvan);
                    objDB.ngay_shn = ValidateDateTimeRange(resultApi.ngay_shn);
                    objDB.ngaytheodau_time = ValidateDateTimeRange(resultApi.time_theodau);
                    objDB.ngaybanghoi_ngay_3db24b = ValidateDateTimeRange(resultApi.ngay_3db24b);

                    objDB.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = resultApi.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete;
                    objDB.sng_lc_nc_tiu_complete = resultApi.sng_lc_nc_tiu_complete;
                    objDB.sng_lc_hiv_complete = resultApi.sng_lc_hiv_complete;
                    objDB.chuyn_gi_complete = resultApi.chuyn_gi_complete;
                    objDB.phiu_t_vn_complete = resultApi.phiu_t_vn_complete;
                    objDB.nh_gi_mc_hi_lng_ca_kh_complete = resultApi.nh_gi_mc_hi_lng_ca_kh_complete;
                    objDB.sinh_hot_nhm_complete = resultApi.sinh_hot_nhm_complete;
                    objDB.theo_du_complete = resultApi.theo_du_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_THONG_TIN_CO_BAN:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_THONG_TIN_CO_BAN sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_THONG_TIN_CO_BAN sang entity**************************************");
        }

        public void ConvertApiKhachHangSangLocNuocTieuEntity(List<ResultApiKhachHangSangLocNuocTieuModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU();
            var resultApi = new ResultApiKhachHangSangLocNuocTieuModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";


                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 2); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 2);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_THONG_TIN_CO_BAN
                    objDB = new CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    objDB.ngayhoi = ValidateDateTimeRange(resultApi.ngayhoi);

                    if (!string.IsNullOrEmpty(resultApi.kqxnda))
                    {
                        objDB.kqxnda = resultApi.kqxnda;
                        if (resultApi.kqxnda.Equals("1"))
                        {
                            objDB.kqxnda_text = "Dương tính";
                        }
                        else
                        {
                            objDB.kqxnda_text = "Âm tính";
                        }
                    }

                    objDB.sng_lc_nc_tiu_complete = resultApi.sng_lc_nc_tiu_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU sang entity**************************************");
        }

        public void ConvertApiKhachHangSangLocHIVEntity(List<ResultApiKhachHangSangLocHIVModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_SANG_LOC_HIV> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_SANG_LOC_HIV sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_SANG_LOC_HIV();
            var resultApi = new ResultApiKhachHangSangLocHIVModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_SANG_LOC_HIV:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_SANG_LOC_HIV
                    objDB = new CD43_KHACH_HANG_SANG_LOC_HIV()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    objDB.ngayhoi = ValidateDateTimeRange(resultApi.ngayhoi);

                    if (!string.IsNullOrEmpty(resultApi.hiv))
                    {
                        objDB.hiv = resultApi.hiv;
                        if (resultApi.hiv.Equals("1"))
                        {
                            objDB.hiv_text = "Có";
                        }
                        else
                        {
                            objDB.hiv_text = "Không";
                        }
                    }

                    if (!string.IsNullOrEmpty(resultApi.dt_hiv))
                    {
                        objDB.dt_hiv = resultApi.dt_hiv;
                        if (resultApi.dt_hiv.Equals("1"))
                        {
                            objDB.dt_hiv_text = "Có";
                        }
                        else
                        {
                            objDB.dt_hiv_text = "Không";
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(resultApi.kqxn))
                    {
                        objDB.kqxn = resultApi.kqxn;
                        if (resultApi.kqxn.Equals("1"))
                        {
                            objDB.kqxn_text = "Có phản ứng";
                        }
                        else
                        {
                            objDB.kqxn_text = "Âm tính";
                        }
                    }
                    objDB.sng_lc_hiv_complete = resultApi.sng_lc_hiv_complete;


                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_SANG_LOC_HIV:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_SANG_LOC_HIV sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_SANG_LOC_HIV sang entity**************************************");
        }

        public void ConvertApiKhachHangDanhGiaHaiLongEntity(List<ResultApiKhachHangDanhGiaHaiLongModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_DANH_GIA_HAI_LONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_DANH_GIA_HAI_LONG sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_DANH_GIA_HAI_LONG();
            var resultApi = new ResultApiKhachHangDanhGiaHaiLongModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_DANH_GIA_HAI_LONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_DANH_GIA_HAI_LONG
                    objDB = new CD43_KHACH_HANG_DANH_GIA_HAI_LONG()
                    {
                        record_id = resultApi.record_id,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    if (resultApi.ngay_3db24b != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (true ) //DateTime.TryParseExact(resultApi.ngay_3db24b, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (resultApi.ngay_3db24b > new DateTime(1900, 1, 1) && resultApi.ngay_3db24b < new DateTime(5000, 1, 1))
                            {
                                objDB.ngay_3db24b = resultApi.ngay_3db24b; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngay_3db24b = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngay_3db24b = null; // Gán null nếu không chuyển đổi được
                        }

                    }
                    else
                    {
                        objDB.ngay_3db24b = null; // Gán null nếu resultApi.time là null
                    }

                    if (resultApi.dichvu != null)
                    {
                        objDB.dichvu = resultApi.dichvu;                    
                    }
                    if (resultApi.dv_khac != null)
                    {
                        objDB.dv_khac = resultApi.dv_khac;                    
                    }
                    if (resultApi.cau1 != null)
                    {
                        objDB.cau1 = resultApi.cau1;                    
                    }
                    if (resultApi.cau2_b127b4 != null)
                    {
                        objDB.cau2_b127b4 = resultApi.cau2_b127b4;                    
                    }
                    if (resultApi.cau3_eaf641 != null)
                    {
                        objDB.cau3_eaf641 = resultApi.cau3_eaf641;                    
                    }
                    if (resultApi.cau4_60e585 != null)
                    {
                        objDB.cau4_60e585 = resultApi.cau4_60e585;                    
                    }

                    objDB.cau5 = resultApi.cau5;
                    objDB.cau6 = resultApi.cau6;
                    objDB.kenh___1 = resultApi.kenh___1;
                    objDB.kenh___2 = resultApi.kenh___2;
                    objDB.kenh___3 = resultApi.kenh___3;
                    objDB.kenh___4 = resultApi.kenh___4;
                    objDB.khac_1 = resultApi.khac_1;
                    objDB.thongtin___1 = resultApi.thongtin___1;
                    objDB.thongtin___2 = resultApi.thongtin___2;
                    objDB.thongtin___3 = resultApi.thongtin___3;
                    objDB.thongtin___4 = resultApi.thongtin___4;
                    objDB.thongtin___5 = resultApi.thongtin___5;
                    objDB.thongtin___6 = resultApi.thongtin___6;
                    objDB.tt_khac = resultApi.tt_khac;
                    objDB.chiase = resultApi.chiase;

                    if (resultApi.nh_gi_mc_hi_lng_ca_kh_complete != null)
                    {
                        objDB.nh_gi_mc_hi_lng_ca_kh_complete = resultApi.nh_gi_mc_hi_lng_ca_kh_complete;
                    }

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_DANH_GIA_HAI_LONG:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_DANH_GIA_HAI_LONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_DANH_GIA_HAI_LONG sang entity**************************************");
        }

        public void ConvertApiKhachHangTheoDauEntity(List<ResultApiKhachHangTheoDauModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_THEO_DAU> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_THEO_DAU sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_THEO_DAU();
            var resultApi = new ResultApiKhachHangTheoDauModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_THEO_DAU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_THONG_TIN_CO_BAN
                    objDB = new CD43_KHACH_HANG_THEO_DAU()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    if (resultApi.time_theodau != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (true) //DateTime.TryParseExact(resultApi.ngay_3db24b, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (resultApi.time_theodau > new DateTime(1900, 1, 1) && resultApi.time_theodau < new DateTime(5000, 1, 1))
                            {
                                objDB.time_theodau = resultApi.time_theodau; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.time_theodau = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.time_theodau = null; // Gán null nếu không chuyển đổi được
                        }
                    }
                    else
                    {
                        objDB.time_theodau = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.kq_theodau = resultApi.kq_theodau;
                    objDB.lydo_matdau = resultApi.lydo_matdau;
                    objDB.matdau_khac = resultApi.matdau_khac;
                    objDB.theo_du_complete = resultApi.theo_du_complete;
                    
                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_THEO_DAU:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_THEO_DAU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_THEO_DAU sang entity**************************************");
        }

        public void ConvertApiKhachHangDanhGiaTacDongEntity(List<ResultApiKhachHangDanhGiaTacDongModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_DANH_GIA_TAC_DONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_DANH_GIA_TAC_DONG sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_DANH_GIA_TAC_DONG();
            var resultApi = new ResultApiKhachHangDanhGiaTacDongModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_DANH_GIA_TAC_DONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_THONG_TIN_CO_BAN
                    objDB = new CD43_KHACH_HANG_DANH_GIA_TAC_DONG()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };
                    
                    objDB.date_vi = ValidateDateTimeRange(resultApi.date_vi);
                    objDB.s2_vi = resultApi.s2_vi;                   
                    objDB.phng_vn_nh_gi_tc_ng_complete = resultApi.phng_vn_nh_gi_tc_ng_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_DANH_GIA_TAC_DONG:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_DANH_GIA_TAC_DONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_DANH_GIA_TAC_DONG sang entity**************************************");
        }

        public void ConvertApiKhachHangSinhHoatNhomEntity(List<ResultApiKhachHangSinhHoatNhomModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_SINH_HOAT_NHOM> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_SINH_HOAT_NHOM sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_SINH_HOAT_NHOM();
            var resultApi = new ResultApiKhachHangSinhHoatNhomModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_SINH_HOAT_NHOM:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_SINH_HOAT_NHOM
                    objDB = new CD43_KHACH_HANG_SINH_HOAT_NHOM()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    if (resultApi.ngay_shn != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (true) //DateTime.TryParseExact(resultApi.ngay_3db24b, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (resultApi.ngay_shn > new DateTime(1900, 1, 1) && resultApi.ngay_shn < new DateTime(5000, 1, 1))
                            {
                                objDB.ngay_shn = resultApi.ngay_shn; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngay_shn = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngay_shn = null; // Gán null nếu không chuyển đổi được
                        }
                    }
                    else
                    {
                        objDB.ngay_shn = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.chude = resultApi.chude;
                    objDB.phatvatpham = resultApi.phatvatpham;
                    objDB.bcs = resultApi.bcs;
                    objDB.gel = resultApi.gel;
                    objDB.sinh_hot_nhm_complete = resultApi.sinh_hot_nhm_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_SINH_HOAT_NHOM:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_SINH_HOAT_NHOM sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_SINH_HOAT_NHOM sang entity**************************************");
        }

        public void ConvertApiKhachHangPhieuTuVanEntity(List<ResultApiKhachHangPhieuTuVanModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_PHIEU_TU_VAN> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_PHIEU_TU_VAN sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_PHIEU_TU_VAN();
            var resultApi = new ResultApiKhachHangPhieuTuVanModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_PHIEU_TU_VAN:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_PHIEU_TU_VAN
                    objDB = new CD43_KHACH_HANG_PHIEU_TU_VAN()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    if (resultApi.ngaytuvan != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (true) //DateTime.TryParseExact(resultApi.ngay_3db24b, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (resultApi.ngaytuvan > new DateTime(1900, 1, 1) && resultApi.ngaytuvan < new DateTime(5000, 1, 1))
                            {
                                objDB.ngaytuvan = resultApi.ngaytuvan; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngaytuvan = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngaytuvan = null; // Gán null nếu không chuyển đổi được
                        }
                    }
                    else
                    {
                        objDB.ngaytuvan = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.diadiem = resultApi.diadiem;
                    objDB.tcv = resultApi.tcv;
                    objDB.lantuvan = resultApi.lantuvan;
                    objDB.cau1_1___1 = resultApi.cau1_1___1;
                    objDB.cau1_1___2 = resultApi.cau1_1___2;
                    objDB.cau1_1___3 = resultApi.cau1_1___3;
                    objDB.cau1_1___4 = resultApi.cau1_1___4;
                    objDB.cau1_1___5 = resultApi.cau1_1___5;
                    objDB.cau1_1___6 = resultApi.cau1_1___6;
                    objDB.cau1_1___7 = resultApi.cau1_1___7;
                    objDB.cau1_1k = resultApi.cau1_1k;
                    objDB.cau1_2 = resultApi.cau1_2;
                    objDB.cau1_3___8 = resultApi.cau1_3___8;
                    objDB.cau1_3___9 = resultApi.cau1_3___9;
                    objDB.cau1_3___10 = resultApi.cau1_3___10;
                    objDB.cau1_3___11 = resultApi.cau1_3___11;
                    objDB.cau_1_4_1___1 = resultApi.cau_1_4_1___1;
                    objDB.cau_1_4_1___2 = resultApi.cau_1_4_1___2;
                    objDB.cau_1_4_1___3 = resultApi.cau_1_4_1___3;
                    objDB.cau_1_4_1___4 = resultApi.cau_1_4_1___4;
                    objDB.cau_1_4_1___5 = resultApi.cau_1_4_1___5;
                    objDB.cau_1_4_1___6 = resultApi.cau_1_4_1___6;
                    objDB.cau_1_4_1___7 = resultApi.cau_1_4_1___7;
                    objDB.cau_1_4_2 = resultApi.cau_1_4_2;
                    objDB.cau_1_4_1_mt = resultApi.cau_1_4_1_mt;
                    objDB.cau_1_5_2 = resultApi.cau_1_5_2;
                    objDB.cau_1_5_3 = resultApi.cau_1_5_3;
                    objDB.cau_1_5_4 = resultApi.cau_1_5_4;
                    objDB.cau_1_5_5 = resultApi.cau_1_5_5;
                    objDB.cau_1_5_6 = resultApi.cau_1_5_6;
                    objDB.cau2___1 = resultApi.cau2___1;
                    objDB.cau2___2 = resultApi.cau2___2;
                    objDB.cau2___3 = resultApi.cau2___3;
                    objDB.cau2___4 = resultApi.cau2___4;
                    objDB.cau2___5 = resultApi.cau2___5;
                    objDB.cau2___6 = resultApi.cau2___6;
                    objDB.cau2___7 = resultApi.cau2___7;
                    objDB.cau2_1k = resultApi.cau2_1k;
                    objDB.cau2_2 = resultApi.cau2_2;
                    objDB.cau3___1 = resultApi.cau3___1;
                    objDB.cau3___2 = resultApi.cau3___2;
                    objDB.cau3___3 = resultApi.cau3___3;
                    objDB.cau3___4 = resultApi.cau3___4;
                    objDB.cau3___5 = resultApi.cau3___5;
                    objDB.cau3___6 = resultApi.cau3___6;
                    objDB.cau3___7 = resultApi.cau3___7;
                    objDB.cau3___8 = resultApi.cau3___8;
                    objDB.cau3___10 = resultApi.cau3___10;
                    objDB.cau3___9 = resultApi.cau3___9;
                    objDB.cau3___11 = resultApi.cau3___11;
                    objDB.cau3___12 = resultApi.cau3___12;
                    objDB.cau3_1k = resultApi.cau3_1k;
                    objDB.cau3_1k_2 = resultApi.cau3_1k_2;
                    objDB.cau4___1 = resultApi.cau4___1;
                    objDB.cau4___2 = resultApi.cau4___2;
                    objDB.cau4___3 = resultApi.cau4___3;
                    objDB.cau4___4 = resultApi.cau4___4;
                    objDB.cau4___5 = resultApi.cau4___5;
                    objDB.cau4___6 = resultApi.cau4___6;
                    objDB.cau4___7 = resultApi.cau4___7;
                    objDB.cau4___8 = resultApi.cau4___8;
                    objDB.cau4___9 = resultApi.cau4___9;
                    objDB.cau4___10 = resultApi.cau4___10;
                    objDB.cau4___11 = resultApi.cau4___11;
                    objDB.cau4___12 = resultApi.cau4___12;
                    objDB.cau4_1k = resultApi.cau4_1k;
                    objDB.cau4_1k_2 = resultApi.cau4_1k_2;
                    objDB.cau_5___1 = resultApi.cau_5___1;
                    objDB.cau_5___2 = resultApi.cau_5___2;
                    objDB.cau_5___3 = resultApi.cau_5___3;
                    objDB.cau_5___4 = resultApi.cau_5___4;
                    objDB.cau_5___6 = resultApi.cau_5___6;
                    objDB.cau_5___5 = resultApi.cau_5___5;
                    objDB.sktd_khac = resultApi.sktd_khac;
                    objDB.cau6_1___1 = resultApi.cau6_1___1;
                    objDB.cau6_1___2 = resultApi.cau6_1___2;
                    objDB.cau6_1___3 = resultApi.cau6_1___3;
                    objDB.cau6_1___4 = resultApi.cau6_1___4;
                    objDB.cau6_1___5 = resultApi.cau6_1___5;
                    objDB.cau6_1___6 = resultApi.cau6_1___6;
                    objDB.cau6_1k = resultApi.cau6_1k;
                    objDB.cau6_1_2 = resultApi.cau6_1_2;
                    objDB.cau6_2___6 = resultApi.cau6_2___6;
                    objDB.cau6_2___7 = resultApi.cau6_2___7;
                    objDB.cau6_2___8 = resultApi.cau6_2___8;
                    objDB.cau6_2___9 = resultApi.cau6_2___9;
                    objDB.cau6_2___10 = resultApi.cau6_2___10;
                    objDB.cau6_2___11 = resultApi.cau6_2___11;
                    objDB.cau6_2___12 = resultApi.cau6_2___12;
                    objDB.cau6_2___15 = resultApi.cau6_2___15;
                    objDB.cau6_2___13 = resultApi.cau6_2___13;
                    objDB.cau6_2___14 = resultApi.cau6_2___14;
                    objDB.cau6_2k = resultApi.cau6_2k;
                    objDB.cau6_2k_2 = resultApi.cau6_2k_2;
                    objDB.cau6_3_bs___1 = resultApi.cau6_3_bs___1;
                    objDB.cau6_3_bs___2 = resultApi.cau6_3_bs___2;
                    objDB.cau6_3_bs___3 = resultApi.cau6_3_bs___3;
                    objDB.cau6_3_bs___4 = resultApi.cau6_3_bs___4;
                    objDB.cau6_3_bs___5 = resultApi.cau6_3_bs___5;
                    objDB.cau6_3_bs___6 = resultApi.cau6_3_bs___6;
                    objDB.cau6_3_bs_1 = resultApi.cau6_3_bs_1;
                    
                    objDB.cau6_3 = resultApi.cau6_3;
                    objDB.cau6_3_1 = resultApi.cau6_3_1;
                    objDB.cau6_3_2 = resultApi.cau6_3_2;
                    objDB.cau6_4 = resultApi.cau6_4;
                    objDB.cau6_4_1 = resultApi.cau6_4_1;
                    objDB.cau6_5 = resultApi.cau6_5;
                    objDB.cau6_5_1 = resultApi.cau6_5_1;
                    
                    objDB.tuvantiep = resultApi.tuvantiep;
                    objDB.vande = resultApi.vande;
                    objDB.thoigian = ValidateDateTimeRange(resultApi.thoigian);

                    objDB.phiu_t_vn_complete = resultApi.phiu_t_vn_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_PHIEU_TU_VAN:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_PHIEU_TU_VAN sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_PHIEU_TU_VAN sang entity**************************************");
        }

        public void ConvertApiKhachHangChuyenGuiEntity(List<ResultApiKhachHangChuyenGuiModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_CHUYEN_GUI> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_CHUYEN_GUI sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_CHUYEN_GUI();
            var resultApi = new ResultApiKhachHangChuyenGuiModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_CHUYEN_GUI:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_CHUYEN_GUI
                    objDB = new CD43_KHACH_HANG_CHUYEN_GUI()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    objDB.f2_q_1_1 = ValidateDateTimeRange(resultApi.f2_q_1_1);

                    // Gán giá trị các thuộc tính
                    objDB.record_id = resultApi.record_id;
                    
                    objDB.chuyengui___1 = resultApi.chuyengui___1;
                    objDB.chuyengui___2 = resultApi.chuyengui___2;
                    objDB.chuyengui___3 = resultApi.chuyengui___3;
                    objDB.chuyengui___4 = resultApi.chuyengui___4;
                    objDB.chuyengui___5 = resultApi.chuyengui___5;
                    objDB.chuyengui___6 = resultApi.chuyengui___6;
                    objDB.p_1 = resultApi.p_1;
                    
                    objDB.f2_q_1_2 = resultApi.f2_q_1_2;
                    objDB.f2_q_1_3 = resultApi.f2_q_1_3;
                    objDB.f2_q_1_3_1 = resultApi.f2_q_1_3_1;
                    objDB.f2_q_1_4 = resultApi.f2_q_1_4;
                    objDB.f2_q_1_5 = resultApi.f2_q_1_5;
                    objDB.f2_q_1_5_1 = resultApi.f2_q_1_5_1;
                    objDB.f2_q_1_6 = resultApi.f2_q_1_6;
                    objDB.f2_q_1_8 = resultApi.f2_q_1_8;
                    objDB.f2_q_1_8_1 = ValidateDateTimeRange(resultApi.f2_q_1_8_1);
                    
                    objDB.p_2 = resultApi.p_2;
                    
                    objDB.f2_q_2_1 = ValidateDateTimeRange(resultApi.f2_q_2_1);
                    objDB.f2_q_2_2 = resultApi.f2_q_2_2;
                    objDB.f2_q_2_3 = resultApi.f2_q_2_3;
                    objDB.f2_q_2_4 = resultApi.f2_q_2_4;
                    objDB.f2_q_2_5 = resultApi.f2_q_2_5;
                    objDB.f2_q_2_6 = ValidateDateTimeRange(resultApi.f2_q_2_6);

                    objDB.p_3 = resultApi.p_3;
                    objDB.f2_q_3_1_d = ValidateDateTimeRange(resultApi.f2_q_3_1_d);
                    objDB.f2_q_3_1 = resultApi.f2_q_3_1;
                    objDB.f2_q_3_1_1 = resultApi.f2_q_3_1_1;
                    objDB.f2_q_3_1_1_1 = resultApi.f2_q_3_1_1_1;
                    objDB.f2_q_3_1_2 = resultApi.f2_q_3_1_2;
                    objDB.f2_q_3_1_1_2 = resultApi.f2_q_3_1_1_2;
                    objDB.f2_q_3_1_3 = resultApi.f2_q_3_1_3;
                    objDB.f2_q_3_1_3_1 = resultApi.f2_q_3_1_3_1;
                    objDB.f2_q_3_2 = resultApi.f2_q_3_2;
                    objDB.f2_q_3_3 = resultApi.f2_q_3_3;
                    objDB.p_4 = resultApi.p_4;
                    objDB.loaihinh4 = resultApi.loaihinh4;
                    objDB.loaihinh4___1 = resultApi.loaihinh4___1;
                    objDB.loaihinh4___2 = resultApi.loaihinh4___2;
                    objDB.loaihinh4___3 = resultApi.loaihinh4___3;
                    objDB.loaihinh4___4 = resultApi.loaihinh4___4;
                    objDB.f2_q_4_1 = resultApi.f2_q_4_1;
                    objDB.f2_q_4_1_1 = resultApi.f2_q_4_1_1;
                    objDB.f2_q_4_1_2 = resultApi.f2_q_4_1_2;
                    objDB.f2_q_4_1_3 = resultApi.f2_q_4_1_3;
                    objDB.f2_q_4_1_4 = resultApi.f2_q_4_1_4;
                    objDB.f2_q_4_1_4___1 = resultApi.f2_q_4_1_4___1;
                    objDB.f2_q_4_1_4___2 = resultApi.f2_q_4_1_4___2;
                    objDB.f2_q_4_1_4___3 = resultApi.f2_q_4_1_4___3;
                    objDB.f2_q_4_1_4___4 = resultApi.f2_q_4_1_4___4;
                    objDB.f2_q_4_1_4___5 = resultApi.f2_q_4_1_4___5;
                    objDB.f2_q_4_1_4___6 = resultApi.f2_q_4_1_4___6;
                    objDB.f2_q_4_1_4___7 = resultApi.f2_q_4_1_4___7;
                    objDB.f2_q_4_1_4_1 = resultApi.f2_q_4_1_4_1;
                    objDB.f2_q_4_1_5 = resultApi.f2_q_4_1_5;
                    if (resultApi.f2_q_4_1_5_1 != null && !string.IsNullOrEmpty(resultApi.f2_q_4_1_5_1))
                        objDB.f2_q_4_1_5_1 = "1";
                    objDB.f2_q_4_1_6 = resultApi.f2_q_4_1_6;
                    objDB.f2_q_4_1_7 = resultApi.f2_q_4_1_7;
                    objDB.f2_q_4_2 = resultApi.f2_q_4_2;

                    objDB.f2_q_4_2_1 = ValidateDateTimeRange(resultApi.f2_q_4_2_1);
                    objDB.f2_q_4_2_2 = resultApi.f2_q_4_2_2;
                    objDB.f2_q_4_2_3 = resultApi.f2_q_4_2_3;
                    objDB.f2_q_4_2_4 = resultApi.f2_q_4_2_4;
                    objDB.f2_q_4_2_5 = resultApi.f2_q_4_2_5;
                    objDB.f2_q_4_2_6 = resultApi.f2_q_4_2_6;
                    objDB.f2_q_4_3 = resultApi.f2_q_4_3;
                    
                    objDB.f2_q_4_3_1 = ValidateDateTimeRange(resultApi.f2_q_4_3_1);
                    objDB.f2_q_4_3_2 = resultApi.f2_q_4_3_2;
                    objDB.f2_q_4_3_3 = resultApi.f2_q_4_3_3;
                    objDB.f2_q_4_3_4 = resultApi.f2_q_4_3_4;
                    objDB.f2_q_4_4 = resultApi.f2_q_4_4;
                    
                    objDB.f2_q_4_4_1 = ValidateDateTimeRange(resultApi.f2_q_4_4_1);
                    objDB.f2_q_4_4_2 = resultApi.f2_q_4_4_2;
                    objDB.f2_q_4_4_2_1 = resultApi.f2_q_4_4_2_1;
                    objDB.f2_q_4_4_3 = resultApi.f2_q_4_4_3;
                    objDB.f2_q_4_4_3_1 = resultApi.f2_q_4_4_3_1;
                    objDB.f2_q_4_4_4 = resultApi.f2_q_4_4_4;
                    objDB.p_5 = resultApi.p_5;
                    objDB.f2_q_5 = resultApi.f2_q_5;                   
                    objDB.f2_q_5_d = ValidateDateTimeRange(resultApi.f2_q_5_d);

                    objDB.f2_q_51 = resultApi.f2_q_51;
                    objDB.f2_q_5_1 = resultApi.f2_q_5_1;
                    objDB.f2_q_53 = resultApi.f2_q_53;
                    objDB.f2_q_5_2 = resultApi.f2_q_5_2;
                    objDB.p_6 = resultApi.p_6;
                   
                    objDB.f2_q_6_d = ValidateDateTimeRange(resultApi.f2_q_6_d);
                    objDB.f2_q_6_1 = resultApi.f2_q_6_1;
                    objDB.f2_q_6_2 = resultApi.f2_q_6_2;
                    objDB.f2_q_6_3 = resultApi.f2_q_6_3;
                    objDB.f2_q_6_4 = resultApi.f2_q_6_4;
                    objDB.f2_q_6_5 = resultApi.f2_q_6_5;
                    objDB.f2_q_6_6 = resultApi.f2_q_6_6;
                    objDB.f2_q_6_7 = resultApi.f2_q_6_7;  
                    
                    objDB.chuyn_gi_complete = resultApi.chuyn_gi_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_CHUYEN_GUI:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_CHUYEN_GUI sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_CHUYEN_GUI sang entity**************************************");
        }

        public void ConvertApiKhachHangHanhViNguyCoEntity(List<ResultApiKhachHangHanhViNguyCoModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CD43_KHACH_HANG_HANH_VI_NGUY_CO> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CD43_KHACH_HANG_HANH_VI_NGUY_CO sang entity**************************************");
            var objDB = new CD43_KHACH_HANG_HANH_VI_NGUY_CO();
            var resultApi = new ResultApiKhachHangHanhViNguyCoModel();
            try
            {
                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";
                var city_code_map = "";

                log.Info("*********-----TỔNG SỐ RECORD API CD43_KHACH_HANG_HANH_VI_NGUY_CO:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

                    if (!string.IsNullOrEmpty(customer_code) && customer_code.Length > 11)
                    {
                        group_code = customer_code.Substring(1, 4); //Lấy mã nhóm TBH
                        cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh

                    }
                    else if (!string.IsNullOrEmpty(customer_code))
                    {
                        cityCode = customer_code.Substring(0, 3);
                        group_code = customer_code.Substring(0, 4);
                    }
                    city_code_map = customer_code.Substring(0, 2);

                    #endregion

                    #region Chuyển đổi dữ liệu sang bảng CD43_KHACH_HANG_HANH_VI_NGUY_CO
                    objDB = new CD43_KHACH_HANG_HANH_VI_NGUY_CO()
                    {
                        record_id = customer_code,
                        //city_code = cityCode,
                        city_code = cityCodeInput,
                        city_code_map = city_code_map,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    if (resultApi.ngayhoi != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (DateTime.TryParseExact(resultApi.ngayhoi, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                            {
                                objDB.ngayhoi = dateTime; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngayhoi = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngayhoi = null; // Gán null nếu không chuyển đổi được
                        }

                    }
                    else
                    {
                        objDB.ngayhoi = null; // Gán null nếu resultApi.time là null
                    }

                    if (resultApi.time != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (DateTime.TryParseExact(resultApi.time, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                            {
                                objDB.thoi_gian_bat_dau = dateTime; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.thoi_gian_bat_dau = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.thoi_gian_bat_dau = null; // Gán null nếu không chuyển đổi được
                        }

                    }
                    else
                    {
                        objDB.thoi_gian_bat_dau = null; // Gán null nếu resultApi.time là null
                    }

                    if (resultApi.end_time != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (DateTime.TryParseExact(resultApi.end_time, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                            {
                                objDB.thoi_gian_ket_thuc = dateTime; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.thoi_gian_ket_thuc = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.thoi_gian_ket_thuc = null; // Gán null nếu không chuyển đổi được
                        }

                    }
                    else
                    {
                        objDB.thoi_gian_ket_thuc = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.f1_q_a3 = resultApi.f1_q_a3;
                    objDB.f1_q_a3_1 = resultApi.f1_q_a3_1;
                    objDB.f1_q_a4 = resultApi.f1_q_a4;
                    objDB.f1_q_a5 = resultApi.f1_q_a5;
                    objDB.f1_q_a5___1 = resultApi.f1_q_a5___1;
                    objDB.f1_q_a5___2 = resultApi.f1_q_a5___2;
                    objDB.f1_q_a5___3 = resultApi.f1_q_a5___3;
                    objDB.f1_q_a6 = resultApi.f1_q_a6;
                    objDB.f1_q_a6_1 = resultApi.f1_q_a6_1;
                    objDB.f1_q_b1 = resultApi.f1_q_b1;
                    objDB.f1_q_b1_1 = resultApi.f1_q_b1_1;
                    objDB.f1_q_b2 = resultApi.f1_q_b2;
                    objDB.f1_q_b2_1 = resultApi.f1_q_b2_1;
                    objDB.f1_q_b3 = resultApi.f1_q_b3;
                    objDB.f1_q_b3_1 = resultApi.f1_q_b3_1;
                    objDB.f1_q_b4 = resultApi.f1_q_b4;
                    objDB.f1_q_b4_1 = resultApi.f1_q_b4_1;
                    objDB.f1_q_b5 = resultApi.f1_q_b5;
                    objDB.f1_q_b5_1 = resultApi.f1_q_b5_1;
                    objDB.f1_q_b6 = resultApi.f1_q_b6;
                    objDB.f1_q_b7 = resultApi.f1_q_b7;
                    objDB.f1_q_b8 = resultApi.f1_q_b8;
                    objDB.f1_q_b8_1 = resultApi.f1_q_b8_1;
                    objDB.f1_q_b9 = resultApi.f1_q_b9;
                    objDB.f1_q_b10 = resultApi.f1_q_b10;
                    objDB.f1_q_b11 = resultApi.f1_q_b11;
                    objDB.f1_q_b12 = resultApi.f1_q_b12;
                    objDB.f1_q_b13 = resultApi.f1_q_b13;
                    objDB.f1_q_b14 = resultApi.f1_q_b14;
                    objDB.f1_q_b14_1 = resultApi.f1_q_b14_1;
                    objDB.f1_q_b15 = resultApi.f1_q_b15;
                    objDB.f1_q_b16 = resultApi.f1_q_b16;
                    objDB.f1_q_b17 = resultApi.f1_q_b17;
                    objDB.assist_mota = resultApi.assist_mota;
                    objDB.thuocla = resultApi.thuocla;
                    objDB.thucuong = resultApi.thucuong;
                    objDB.cansa = resultApi.cansa;
                    objDB.cocain = resultApi.cocain;
                    objDB.chatkichthich = resultApi.chatkichthich;
                    objDB.khixong = resultApi.khixong;
                    objDB.thuocanthan = resultApi.thuocanthan;
                    objDB.chatgayaogiac = resultApi.chatgayaogiac;
                    objDB.thuocphien = resultApi.thuocphien;
                    objDB.chatkhac = resultApi.chatkhac;
                    objDB.cacchatkhac = resultApi.cacchatkhac;
                    objDB.thuocla1 = resultApi.thuocla1;
                    objDB.thucuong1 = resultApi.thucuong1;
                    objDB.cansa1 = resultApi.cansa1;
                    objDB.coca1 = resultApi.coca1;
                    objDB.chatkichthich1 = resultApi.chatkichthich1;
                    objDB.khixong1 = resultApi.khixong1;
                    objDB.thuocanthan1 = resultApi.thuocanthan1;
                    objDB.chatgayaogiac1 = resultApi.chatgayaogiac1;
                    objDB.chatthuocphien1 = resultApi.chatthuocphien1;
                    objDB.chatkhac1 = resultApi.chatkhac1;
                    objDB.thuocla2 = resultApi.thuocla2;
                    objDB.thucuong2 = resultApi.thucuong2;
                    objDB.cansa2 = resultApi.cansa2;
                    objDB.coca2 = resultApi.coca2;
                    objDB.chatkichthich2 = resultApi.chatkichthich2;
                    objDB.khixong2 = resultApi.khixong2;
                    objDB.thuocanthan2 = resultApi.thuocanthan2;
                    objDB.chatgayaogiac2 = resultApi.chatgayaogiac2;
                    objDB.chatthuocphien2 = resultApi.chatthuocphien2;
                    objDB.chatkhac2 = resultApi.chatkhac2;
                    objDB.thuocla3 = resultApi.thuocla3;
                    objDB.thucuong3 = resultApi.thucuong3;
                    objDB.cansa3 = resultApi.cansa3;
                    objDB.coca3 = resultApi.coca3;
                    objDB.chatkichthich3 = resultApi.chatkichthich3;
                    objDB.khixong3 = resultApi.khixong3;
                    objDB.thuocanthan3 = resultApi.thuocanthan3;
                    objDB.chatgayaogiac3 = resultApi.chatgayaogiac3;
                    objDB.chatthuocphien3 = resultApi.chatthuocphien3;
                    objDB.chatkhac3 = resultApi.chatkhac3;                    
                    objDB.thucuong4 = resultApi.thucuong4;
                    objDB.cansa4 = resultApi.cansa4;
                    objDB.coca4 = resultApi.coca4;
                    objDB.chatkichthich4 = resultApi.chatkichthich4;
                    objDB.khixong4 = resultApi.khixong4;
                    objDB.thuocanthan4 = resultApi.thuocanthan4;
                    objDB.chatgayaogiac4 = resultApi.chatgayaogiac4;
                    objDB.chatthuocphien4 = resultApi.chatthuocphien4;
                    objDB.chatkhac4 = resultApi.chatkhac4;
                    objDB.thuocla5 = resultApi.thuocla5;
                    objDB.thucuong5 = resultApi.thucuong5;
                    objDB.cansa5 = resultApi.cansa5;
                    objDB.coca5 = resultApi.coca5;
                    objDB.chatkichthich5 = resultApi.chatkichthich5;
                    objDB.khixong5 = resultApi.khixong5;
                    objDB.thuocanthan5 = resultApi.thuocanthan5;
                    objDB.chatgayaogiac5 = resultApi.chatgayaogiac5;
                    objDB.chatthuocphien5 = resultApi.chatthuocphien5;
                    objDB.chatkhac5 = resultApi.chatkhac5;
                    objDB.thuocla6 = resultApi.thuocla6;
                    objDB.thucuong6 = resultApi.thucuong6;
                    objDB.cansa6 = resultApi.cansa6;
                    objDB.coca6 = resultApi.coca6;
                    objDB.chatkichthich6 = resultApi.chatkichthich6;
                    objDB.khixong6 = resultApi.khixong6;
                    objDB.thuocanthan6 = resultApi.thuocanthan6;
                    objDB.chatgayaogiac6 = resultApi.chatgayaogiac6;
                    objDB.chatthuocphien6 = resultApi.chatthuocphien6;
                    objDB.chatkhac6 = resultApi.chatkhac6;
                    objDB.cau_8 = resultApi.cau_8;
                    objDB.diemthuocla = resultApi.diemthuocla;
                    objDB.diemthucuong = resultApi.diemthucuong;
                    objDB.diemcansa = resultApi.diemcansa;
                    objDB.diemcoca = resultApi.diemcoca;
                    objDB.diemchatkichthich = resultApi.diemchatkichthich;
                    objDB.diemkhixong = resultApi.diemkhixong;
                    objDB.diemchatanthan = resultApi.diemchatanthan;
                    objDB.diemchatgayaogiac = resultApi.diemchatgayaogiac;
                    objDB.diemchatthuocphien = resultApi.diemchatthuocphien;
                    objDB.diemchatkhac = resultApi.diemchatkhac;
                    objDB.c_1a = resultApi.c_1a;
                    objDB.c_1b = resultApi.c_1b;
                    objDB.c_1c = resultApi.c_1c;
                    objDB.c_1d = resultApi.c_1d;
                    objDB.c_2 = resultApi.c_2;
                    objDB.c_3 = resultApi.c_3;
                    objDB.c_4a = resultApi.c_4a;
                    objDB.c_4b = resultApi.c_4b;
                    objDB.c_4c = resultApi.c_4c;
                    objDB.tongdiem = resultApi.tongdiem;
                    objDB.duongtinh = resultApi.duongtinh;
                    objDB.amtinh = resultApi.amtinh;
                    objDB.c1 = resultApi.c1;
                    objDB.c2 = resultApi.c2;
                    objDB.c3 = resultApi.c3;
                    objDB.c4 = resultApi.c4;
                    objDB.c5 = resultApi.c5;
                    objDB.c6 = resultApi.c6;
                    objDB.c7 = resultApi.c7;
                    objDB.c8 = resultApi.c8;
                    objDB.c9 = resultApi.c9;
                    objDB.c10 = resultApi.c10;
                    objDB.diem = resultApi.diem;
                    objDB.f1_q_b1___1 = resultApi.f1_q_b1___1;
                    objDB.f1_q_b1___2 = resultApi.f1_q_b1___2;
                    objDB.f1_q_b1___3 = resultApi.f1_q_b1___3;
                    objDB.f1_q_b1___4 = resultApi.f1_q_b1___4;
                    objDB.f1_q_b1___5 = resultApi.f1_q_b1___5;
                    objDB.f1_q_b1___6 = resultApi.f1_q_b1___6;
                    objDB.f1_q_b1___7 = resultApi.f1_q_b1___7;
                    objDB.f1_q_b2___1 = resultApi.f1_q_b2___1;
                    objDB.f1_q_b2___2 = resultApi.f1_q_b2___2;
                    objDB.f1_q_b2___3 = resultApi.f1_q_b2___3;
                    objDB.f1_q_b2___4 = resultApi.f1_q_b2___4;
                    objDB.f1_q_b2___5 = resultApi.f1_q_b2___5;
                    objDB.f1_q_b2___6 = resultApi.f1_q_b2___6;
                    objDB.f1_q_b2___7 = resultApi.f1_q_b2___7;
                    objDB.f1_q_b3___1 = resultApi.f1_q_b3___1;
                    objDB.f1_q_b3___2 = resultApi.f1_q_b3___2;
                    objDB.f1_q_b3___3 = resultApi.f1_q_b3___3;
                    objDB.f1_q_b3___4 = resultApi.f1_q_b3___4;
                    objDB.f1_q_b3___5 = resultApi.f1_q_b3___5;
                    objDB.f1_q_b3___6 = resultApi.f1_q_b3___6;
                    objDB.f1_q_b3___7 = resultApi.f1_q_b3___7;
                    objDB.f1_q_b3___8 = resultApi.f1_q_b3___8;
                    objDB.f1_q_b3___9 = resultApi.f1_q_b3___9;
                    objDB.f1_q_b4___1 = resultApi.f1_q_b4___1;
                    objDB.f1_q_b4___2 = resultApi.f1_q_b4___2;
                    objDB.f1_q_b4___3 = resultApi.f1_q_b4___3;
                    objDB.f1_q_b4___4 = resultApi.f1_q_b4___4;
                    objDB.f1_q_b4___5 = resultApi.f1_q_b4___5;
                    objDB.f1_q_b4___6 = resultApi.f1_q_b4___6;
                    objDB.f1_q_b4___7 = resultApi.f1_q_b4___7;
                    objDB.f1_q_b4___8 = resultApi.f1_q_b4___8;
                    objDB.f1_q_b14___1 = resultApi.f1_q_b14___1;
                    objDB.f1_q_b14___2 = resultApi.f1_q_b14___2;
                    objDB.f1_q_b14___3 = resultApi.f1_q_b14___3;
                    objDB.f1_q_b14___4 = resultApi.f1_q_b14___4;
                    objDB.f1_q_b14___5 = resultApi.f1_q_b14___5;
                    objDB.f1_q_b8___1 = resultApi.f1_q_b8___1; 
                    objDB.f1_q_b8___2 = resultApi.f1_q_b8___2;
                    objDB.f1_q_b8___3 = resultApi.f1_q_b8___3;
                    objDB.f1_q_b8___4 = resultApi.f1_q_b8___4;
                    objDB.f1_q_b8___5 = resultApi.f1_q_b8___5;
                    objDB.f1_q_b8___6 = resultApi.f1_q_b8___6;
                    objDB.f1_q_b8___7 = resultApi.f1_q_b8___7;
                    objDB.f1_q_b8___8 = resultApi.f1_q_b8___8;

                    objDB.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete = resultApi.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CD43_KHACH_HANG_HANH_VI_NGUY_CO:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CD43_KHACH_HANG_HANH_VI_NGUY_CO sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CD43_KHACH_HANG_HANH_VI_NGUY_CO sang entity**************************************");
        }

        #region Chuyển Dữ liệu từ API vào DB DỰ ÁN CH07
        public void ConvertApiKhachHangTTCBCH07Entity(List<ResultApiKhachHangTTCBCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_THONG_TIN_CO_BAN> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_THONG_TIN_CO_BAN sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_THONG_TIN_CO_BAN();
            var resultApi = new ResultApiKhachHangTTCBCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_THONG_TIN_CO_BAN:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_THONG_TIN_CO_BAN
                    objDB = new CH07_KHACH_HANG_THONG_TIN_CO_BAN()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };
                    

                    objDB.ngaynhap = ValidateDateTimeRange(resultApi.ngaynhap);
                    objDB.ngay = ValidateDateTimeRange(resultApi.ngay);
                    objDB.doituong = resultApi.doituong;
                    objDB.gioitinh = resultApi.gioitinh;
                    objDB.namsinh = resultApi.namsinh;
                    objDB.tuoi = resultApi.tuoi;
                    objDB.thng_tin_c_bn_assist_qst_kin_thc_complete = resultApi.thng_tin_c_bn_assist_qst_kin_thc_complete;
                    objDB.ngaytuvan = ValidateDateTimeRange(resultApi.ngaytuvan);
                    objDB.ngaynhap_tuvan = ValidateDateTimeRange(resultApi.ngaynhap_tuvan);
                    objDB.ngay_29b439 = ValidateDateTimeRange(resultApi.ngay_29b439);
                    objDB.ngayxetnghiem = ValidateDateTimeRange(resultApi.ngayxetnghiem);
                    objDB.ngay_7794e9 = ValidateDateTimeRange(resultApi.ngay_7794e9);
                    objDB.ngay_ace = ValidateDateTimeRange(resultApi.ngay_ace);
                    objDB.ngayhoi = ValidateDateTimeRange(resultApi.ngayhoi);
                    objDB.ngayhoi_fdf6e6 = ValidateDateTimeRange(resultApi.ngayhoi_fdf6e6);
                    objDB.sng_lc_nc_tiu_complete = resultApi.sng_lc_nc_tiu_complete;
                    objDB.phiu_t_vn_complete = resultApi.phiu_t_vn_complete;
                    objDB.chuyn_gi_dch_v_complete = resultApi.chuyn_gi_dch_v_complete;
                    objDB.phiu_xt_nghim_li_hiv_complete = resultApi.phiu_xt_nghim_li_hiv_complete;
                    objDB.theo_du_kh_complete = resultApi.theo_du_kh_complete;
                    objDB.bng_hi_ace_complete = resultApi.bng_hi_ace_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_THONG_TIN_CO_BAN:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_THONG_TIN_CO_BAN sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_THONG_TIN_CO_BAN sang entity**************************************");
        }

        public void ConvertApiKhachHangSangLocNuocTieuCH07Entity(List<ResultApiKhachHangSangLocNuocTieuCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU();
            var resultApi = new ResultApiKhachHangSangLocNuocTieuCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU
                    objDB = new CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;
                    
                    if (resultApi.ngayhoi != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (DateTime.TryParseExact(resultApi.ngayhoi, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                            {
                                objDB.ngayhoi = dateTime; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngayhoi = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngayhoi = null; // Gán null nếu không chuyển đổi được
                        }
                    }
                    else
                    {
                        objDB.ngayhoi = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.kqxnda = resultApi.kqxnda;
                    objDB.kqxnheroin = resultApi.kqxnheroin;
                    objDB.sng_lc_nc_tiu_complete = resultApi.sng_lc_nc_tiu_complete;
                    
                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU sang entity**************************************");
        }

        public void ConvertApiKhachHangSangLocHIVCH07Entity(List<ResultApiKhachHangSangLocHIVCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_SANG_LOC_HIV> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_SANG_LOC_HIV sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_SANG_LOC_HIV();
            var resultApi = new ResultApiKhachHangSangLocHIVCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_SANG_LOC_HIV:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU
                    objDB = new CH07_KHACH_HANG_SANG_LOC_HIV()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    DateTime dateTime;

                    if (resultApi.ngayhoi_fdf6e6 != null)
                    {
                        // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                        if (DateTime.TryParseExact(resultApi.ngayhoi_fdf6e6, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                        {
                            // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                            if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                            {
                                objDB.ngayhoi_fdf6e6 = dateTime; // Gán giá trị nếu hợp lệ
                            }
                            else
                            {
                                objDB.ngayhoi_fdf6e6 = null; // Gán null nếu không nằm trong khoảng
                            }
                        }
                        else
                        {
                            objDB.ngayhoi_fdf6e6 = null; // Gán null nếu không chuyển đổi được
                        }
                    }
                    else
                    {
                        objDB.ngayhoi_fdf6e6 = null; // Gán null nếu resultApi.time là null
                    }

                    objDB.tinhtrang = resultApi.tinhtrang;
                    objDB.arv = resultApi.arv;
                    objDB.cs_dieutri = resultApi.cs_dieutri;
                    objDB.hiv = resultApi.hiv;
                    objDB.kqxn = resultApi.kqxn;
                    objDB.lydo = resultApi.lydo;
                    objDB.sng_lc_hiv_complete = resultApi.sng_lc_hiv_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_SANG_LOC_HIV:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_SANG_LOC_HIV sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_SANG_LOC_HIV sang entity**************************************");
        }
        
        public void ConvertApiKhachHangPhieuTuVanCH07Entity(List<ResultApiKhachHangPhieuTuVanCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_PHIEU_TU_VAN> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_PHIEU_TU_VAN sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_PHIEU_TU_VAN();
            var resultApi = new ResultApiKhachHangPhieuTuVanCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_PHIEU_TU_VAN:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_PHIEU_TU_VAN
                    objDB = new CH07_KHACH_HANG_PHIEU_TU_VAN()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    objDB.ngaynhap_tuvan = ValidateDateTimeRange(resultApi.ngaynhap_tuvan);
                    objDB.ngaytuvan = ValidateDateTimeRange(resultApi.ngaytuvan);

                    objDB.diadiem = resultApi.diadiem;
                    objDB.tcv = resultApi.tcv;
                    objDB.lantuvan = resultApi.lantuvan;

                    objDB.cau1_1___1 = resultApi.cau1_1___1;
                    objDB.cau1_1___2 = resultApi.cau1_1___2;
                    objDB.cau1_1___3 = resultApi.cau1_1___3;
                    objDB.cau1_1___4 = resultApi.cau1_1___4;
                    objDB.cau1_1___5 = resultApi.cau1_1___5;
                    objDB.cau1_1___6 = resultApi.cau1_1___6;
                    objDB.cau1_1___7 = resultApi.cau1_1___7;
                    objDB.cau1_1k = resultApi.cau1_1k;

                    objDB.cau1_2 = resultApi.cau1_2;

                    objDB.cau1_3___8 = resultApi.cau1_3___8;
                    objDB.cau1_3___9 = resultApi.cau1_3___9;
                    objDB.cau1_3___10 = resultApi.cau1_3___10;
                    objDB.cau1_3___11 = resultApi.cau1_3___11;

                    objDB.cau_1_4___1 = resultApi.cau_1_4___1;
                    objDB.cau_1_4___2 = resultApi.cau_1_4___2;
                    objDB.cau_1_4___3 = resultApi.cau_1_4___3;
                    objDB.cau_1_4___4 = resultApi.cau_1_4___4;
                    objDB.cau_1_4___5 = resultApi.cau_1_4___5;
                    objDB.cau_1_4___6 = resultApi.cau_1_4___6;
                    objDB.cau_1_4___7 = resultApi.cau_1_4___7;
                    objDB.cau_1_4_2 = resultApi.cau_1_4_2;

                    objDB.cau_1_4_1_mt = resultApi.cau_1_4_1_mt;
                    objDB.cau_1_5 = resultApi.cau_1_5;
                    objDB.cau_1_5_2 = resultApi.cau_1_5_2;
                    objDB.cau_1_5_3 = resultApi.cau_1_5_3;
                    objDB.cau_1_5_4 = resultApi.cau_1_5_4;
                    objDB.cau_1_5_5 = resultApi.cau_1_5_5;
                    objDB.cau_1_5_6 = resultApi.cau_1_5_6;

                    objDB.cau2_852460___1 = resultApi.cau2_852460___1;
                    objDB.cau2_852460___2 = resultApi.cau2_852460___2;
                    objDB.cau2_852460___3 = resultApi.cau2_852460___3;
                    objDB.cau2_852460___4 = resultApi.cau2_852460___4;
                    objDB.cau2_852460___5 = resultApi.cau2_852460___5;
                    objDB.cau2_852460___6 = resultApi.cau2_852460___6;
                    objDB.cau2_1k = resultApi.cau2_1k;
                    objDB.cau2_2 = resultApi.cau2_2;

                    objDB.cau3_6aaf33 = resultApi.cau3_6aaf33;
                    objDB.cau3_6aaf33___1 = resultApi.cau3_6aaf33___1;
                    objDB.cau3_6aaf33___2 = resultApi.cau3_6aaf33___2;
                    objDB.cau3_6aaf33___3 = resultApi.cau3_6aaf33___3;
                    objDB.cau3_6aaf33___4 = resultApi.cau3_6aaf33___4;
                    objDB.cau3_6aaf33___5 = resultApi.cau3_6aaf33___5;
                    objDB.cau3_6aaf33___6 = resultApi.cau3_6aaf33___6;
                    objDB.cau3_6aaf33___7 = resultApi.cau3_6aaf33___7;
                    objDB.cau3_6aaf33___8 = resultApi.cau3_6aaf33___8;
                    objDB.cau3_6aaf33___9 = resultApi.cau3_6aaf33___9;
                    objDB.cau3_6aaf33___10 = resultApi.cau3_6aaf33___10;
                    objDB.cau3_6aaf33___11 = resultApi.cau3_6aaf33___11;
                    objDB.cau3_6aaf33___12 = resultApi.cau3_6aaf33___12;
                    objDB.cau3_1k = resultApi.cau3_1k;
                    objDB.cau3_1k_2 = resultApi.cau3_1k_2;

                    objDB.cau_4 = resultApi.cau_4;
                    objDB.cau_4___1 = resultApi.cau_4___1;
                    objDB.cau_4___2 = resultApi.cau_4___2;
                    objDB.cau_4___3 = resultApi.cau_4___3;
                    objDB.cau_4___4 = resultApi.cau_4___4;
                    objDB.cau_4___5 = resultApi.cau_4___5;
                    objDB.cau_4___6 = resultApi.cau_4___6;
                    objDB.cau_4___7 = resultApi.cau_4___7;
                    objDB.cau_4___8 = resultApi.cau_4___8;
                    objDB.cau_4___9 = resultApi.cau_4___9;
                    objDB.cau_4___10 = resultApi.cau_4___10;
                    objDB.cau_4___11 = resultApi.cau_4___11;
                    objDB.cau_4___12 = resultApi.cau_4___12;
                    objDB.cau4_1k = resultApi.cau4_1k;
                    objDB.cau4_1k_2 = resultApi.cau4_1k_2;

                    objDB.cau_5 = resultApi.cau_5;
                    objDB.cau_5___1 = resultApi.cau_5___1;
                    objDB.cau_5___2 = resultApi.cau_5___2;
                    objDB.cau_5___3 = resultApi.cau_5___3;
                    objDB.cau_5___4 = resultApi.cau_5___4;
                    objDB.cau_5___5 = resultApi.cau_5___5;
                    objDB.cau_5___6 = resultApi.cau_5___6;
                    objDB.cau_5_1 = resultApi.cau_5_1;
                    objDB.cau_5_2 = resultApi.cau_5_2;

                    objDB.cau_6_1 = resultApi.cau_6_1;
                    objDB.cau_6_1___1 = resultApi.cau_6_1___1;
                    objDB.cau_6_1___2 = resultApi.cau_6_1___2;
                    objDB.cau_6_1___3 = resultApi.cau_6_1___3;

                    objDB.cau_6_1___4 = resultApi.cau_6_1___4;
                    objDB.cau_6_1___5 = resultApi.cau_6_1___5;
                    objDB.cau_6_1___6 = resultApi.cau_6_1___6;
                    objDB.cau6_1k = resultApi.cau6_1k;
                    objDB.cau6_1_2 = resultApi.cau6_1_2;

                    objDB.cau_6_2 = resultApi.cau_6_2;
                    objDB.cau_6_2___1 = resultApi.cau_6_2___1;
                    objDB.cau_6_2___2 = resultApi.cau_6_2___2;
                    objDB.cau_6_2___3 = resultApi.cau_6_2___3;
                    objDB.cau_6_2___4 = resultApi.cau_6_2___4;
                    objDB.cau_6_2___5 = resultApi.cau_6_2___5;
                    objDB.cau_6_2___6 = resultApi.cau_6_2___6;
                    objDB.cau_6_2___7 = resultApi.cau_6_2___7;
                    objDB.cau_6_2___8 = resultApi.cau_6_2___8;
                    objDB.cau_6_2___9 = resultApi.cau_6_2___9;
                    objDB.cau_6_2___10 = resultApi.cau_6_2___10;
                    objDB.cau_6_2_1 = resultApi.cau_6_2_1;
                    objDB.cau_6_2_2 = resultApi.cau_6_2_2;

                    objDB.cau_6_3 = resultApi.cau_6_3;
                    objDB.cau_6_3___1 = resultApi.cau_6_3___1;
                    objDB.cau_6_3___2 = resultApi.cau_6_3___2;
                    objDB.cau_6_3___3 = resultApi.cau_6_3___3;
                    objDB.cau_6_3___4 = resultApi.cau_6_3___4;
                    objDB.cau_6_3___5 = resultApi.cau_6_3___5;
                    objDB.cau_6_3___6 = resultApi.cau_6_3___6;
                    objDB.cau_6_3_1 = resultApi.cau_6_3_1;
                    objDB.cau_6_3_2 = resultApi.cau_6_3_2;

                    objDB.cau_6_4 = resultApi.cau_6_4;
                    objDB.cau_6_4_1 = resultApi.cau_6_4_1;

                    objDB.cau_6_5 = resultApi.cau_6_5;
                    objDB.cau_6_5_1 = resultApi.cau_6_5_1;

                    objDB.cau_6_6 = resultApi.cau_6_6;
                    objDB.cau_6_6_1 = resultApi.cau_6_6_1;

                    objDB.tongket = resultApi.tongket;
                    objDB.tuvantiep = resultApi.tuvantiep;
                    objDB.vande = resultApi.vande;
                    objDB.thoigian = resultApi.thoigian;
                    objDB.phiu_t_vn_complete = resultApi.phiu_t_vn_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_PHIEU_TU_VAN:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_PHIEU_TU_VAN sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_PHIEU_TU_VAN sang entity**************************************");
        }
        public void ConvertApiKhachHangChuyenGuiCH07Entity(List<ResultApiKhachHangChuyenGuiDichVuCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_CHUYEN_GUI> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_CHUYEN_GUI sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_CHUYEN_GUI();
            var resultApi = new ResultApiKhachHangChuyenGuiDichVuCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_CHUYEN_GUI:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_CHUYEN_GUI
                    objDB = new CH07_KHACH_HANG_CHUYEN_GUI()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    objDB.ngay_xn = ValidateDateTimeRange(resultApi.ngay_xn);
                    objDB.ngay_bddt = ValidateDateTimeRange(resultApi.ngay_bddt);
                    objDB.thoidiem = ValidateDateTimeRange(resultApi.thoidiem);
                    objDB.ngaykham = ValidateDateTimeRange(resultApi.ngaykham);
                    objDB.ngay_taikham = ValidateDateTimeRange(resultApi.ngay_taikham);
                    objDB.ngaykxn = ValidateDateTimeRange(resultApi.ngaykxn);
                    objDB.ngay_29b439 = ValidateDateTimeRange(resultApi.ngay_29b439);
                    
                    // Mapping các thuộc tính từ resultApi sang objDB
                    objDB.loaihinh = resultApi.loaihinh;
                    objDB.loaihinh___1 = resultApi.loaihinh___1;
                    objDB.loaihinh___2 = resultApi.loaihinh___2;
                    objDB.loaihinh___3 = resultApi.loaihinh___3;
                    objDB.loaihinh___4 = resultApi.loaihinh___4;
                    
                    objDB.diachi_xn = resultApi.diachi_xn;
                    objDB.kq_xn = resultApi.kq_xn;
                    objDB.dieutri = resultApi.dieutri;
                    objDB.diachi_cg = resultApi.diachi_cg;
                    
                    objDB.taiuong_bd = resultApi.taiuong_bd;
                    objDB.taiuong_bd_2 = resultApi.taiuong_bd_2;
                    objDB.dt_arv = resultApi.dt_arv;
                    objDB.lydo = resultApi.lydo;
                    objDB.diachikham = resultApi.diachikham;
                    objDB.lankham = resultApi.lankham;
                    objDB.chandoan = resultApi.chandoan;
                    objDB.khac = resultApi.khac;
                    objDB.kedon = resultApi.kedon;
                    objDB.dungthuoc = resultApi.dungthuoc;
                    objDB.taikham = resultApi.taikham;
                    objDB.diachi_kxn = resultApi.diachi_kxn;
                    objDB.lan_xnk = resultApi.lan_xnk;
                    objDB.chandoan1 = resultApi.chandoan1;
                    objDB.khac_sti = resultApi.khac_sti;
                    objDB.dieutri_sti = resultApi.dieutri_sti;
                    objDB.mua_bhyt = resultApi.mua_bhyt;
                    objDB.dt_prep = resultApi.dt_prep;
                    objDB.cs_prep = resultApi.cs_prep;
                    objDB.dt_pep = resultApi.dt_pep;
                    objDB.cs_pep = resultApi.cs_pep;
                    objDB.dt_vgc = resultApi.dt_vgc;
                    objDB.cs_vgc = resultApi.cs_vgc;
                    objDB.dt_lao = resultApi.dt_lao;
                    objDB.cs_lao = resultApi.cs_lao;
                    objDB.dt_met = resultApi.dt_met;
                    objDB.cs_met = resultApi.cs_met;
                    objDB.chuyn_gi_dch_v_complete = resultApi.chuyn_gi_dch_v_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_CHUYEN_GUI:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_CHUYEN_GUI sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_CHUYEN_GUI sang entity**************************************");
        }
        public void ConvertApiKhachHangTheoDauCH07Entity(List<ResultApiKhachHangTheoDauCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_THEO_DAU> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_THEO_DAU sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_THEO_DAU();
            var resultApi = new ResultApiKhachHangTheoDauCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_THEO_DAU:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_THEO_DAU
                    objDB = new CH07_KHACH_HANG_THEO_DAU()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    // Sử dụng hàm ValidateDateTimeRange đã viết để kiểm tra ngày tháng (nếu cần)
                    objDB.ngay_7794e9 = ValidateDateTimeRange(resultApi.ngay_7794e9);

                    // Mapping các thuộc tính còn lại
                    objDB.hinhthuc = resultApi.hinhthuc;
                    objDB.hinhthuc_khac = resultApi.hinhthuc_khac;
                    objDB.kq = resultApi.kq;
                    objDB.khac_a7b30e = resultApi.khac_a7b30e;
                    objDB.ghichu = resultApi.ghichu;
                    objDB.theo_du_kh_complete = resultApi.theo_du_kh_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_THEO_DAU:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_THEO_DAU sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_THEO_DAU sang entity**************************************");
        }
        public void ConvertApiKhachHangPhieuXetNghiemLaiHIVCH07Entity(List<ResultApiKhachHangPhieuXetNghiemLaiHIVCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV();
            var resultApi = new ResultApiKhachHangPhieuXetNghiemLaiHIVCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV
                    objDB = new CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    // Sử dụng hàm ValidateDateTimeRange đã viết để kiểm tra ngày tháng (nếu cần)
                    objDB.ngayxetnghiem = ValidateDateTimeRange(resultApi.ngayxetnghiem);

                    // Mapping các thuộc tính còn lại
                    objDB.solan = resultApi.solan;
                    objDB.kqxn_lai = resultApi.kqxn_lai;
                    objDB.phiu_xt_nghim_li_hiv_complete = resultApi.phiu_xt_nghim_li_hiv_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV sang entity**************************************");
        }
        public void ConvertApiKhachHangBangHoiACECH07Entity(List<ResultApiKhachHangBangHoiACECH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_BANG_HOI_ACE> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_BANG_HOI_ACE sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_BANG_HOI_ACE();
            var resultApi = new ResultApiKhachHangBangHoiACECH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_BANG_HOI_ACE:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                

                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_BANG_HOI_ACE
                    objDB = new CH07_KHACH_HANG_BANG_HOI_ACE()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    // Sử dụng hàm ValidateDateTimeRange đã viết để kiểm tra ngày tháng (nếu cần)
                    objDB.ngay_ace = ValidateDateTimeRange(resultApi.ngay_ace);

                    // Mapping các thuộc tính còn lại
                    objDB.c1 = resultApi.c1;
                    objDB.c2 = resultApi.c2;
                    objDB.c3 = resultApi.c3;
                    objDB.c4 = resultApi.c4;
                    objDB.c5 = resultApi.c5;
                    objDB.c6 = resultApi.c6;
                    objDB.c7 = resultApi.c7;
                    objDB.c8 = resultApi.c8;
                    objDB.c9 = resultApi.c9;
                    objDB.c10 = resultApi.c10;
                    objDB.diem = resultApi.diem;
                    objDB.bng_hi_ace_complete = resultApi.bng_hi_ace_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_BANG_HOI_ACE:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_BANG_HOI_ACE sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_BANG_HOI_ACE sang entity**************************************");
        }

        public void ConvertApiKhachHangAssistQstKienThucCH07Entity(List<ResultApiKhachHangAssistQstKienThucCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC sang entity**************************************");
            var objDB = new CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC();
            var resultApi = new ResultApiKhachHangAssistQstKienThucCH07Model();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var nhomTBH = new BVTL_NHOM_TBH();

                //var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                var customer_code = "";
                var group_code = "";
                var cityCode = "";

                log.Info("*********-----TỔNG SỐ RECORD API CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;


                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    //customer = new BVTL_KHACH_HANG();
                    customer_code = "";

                    group_code = "";
                    //cityCode = "";
                    nhomTBH = new BVTL_NHOM_TBH();

                    resultApi = resultApiModels[i];
                    #region Lấy thông tin khách hàng, nhóm thu thập dữ liệu

                    //customer_code = resultApiCGDV.makh;
                    customer_code = String.Concat(resultApi.record_id);

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

                    #region Chuyển đổi dữ liệu sang bảng CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC
                    objDB = new CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC()
                    {
                        record_id = customer_code,
                        ma_tinh = cityCode,
                        city_code = cityCodeInput,
                        manhom_tbh = group_code,
                        maduan = maDuAn,
                    };

                    // Sử dụng hàm ValidateDateTimeRange đã viết để kiểm tra ngày tháng (nếu cần)
                    objDB.ngaynhap = ValidateDateTimeRange(resultApi.ngaynhap);
                    objDB.ngay = ValidateDateTimeRange(resultApi.ngay);

                    objDB.doituong = resultApi.doituong;
                    objDB.gioitinh = resultApi.gioitinh;
                    objDB.namsinh = resultApi.namsinh;
                    objDB.chatgaynghien = resultApi.chatgaynghien;
                    objDB.khac1 = resultApi.khac1;
                    objDB.chatgaynghien_2 = resultApi.chatgaynghien_2;
                    objDB.khac2 = resultApi.khac2;
                    objDB.duongsd = resultApi.duongsd;
                    objDB.tansuatda = resultApi.tansuatda;
                    objDB.landau = resultApi.landau;
                    objDB.tuoi = resultApi.tuoi;
                    objDB.matuydautien = resultApi.matuydautien;
                    objDB.tiemchich = resultApi.tiemchich;
                    objDB.dungchung = resultApi.dungchung;
                    objDB.qhtd = resultApi.qhtd;
                    objDB.cau_7_1 = resultApi.cau_7_1;
                    objDB.qhtd_2 = resultApi.qhtd_2;
                    objDB.sdmatuy = resultApi.sdmatuy;
                    objDB.qhtdtt = resultApi.qhtdtt;
                    objDB.bandam = resultApi.bandam;
                    objDB.sti = resultApi.sti;
                    objDB.sti1 = resultApi.sti1;
                    objDB.khac3 = resultApi.khac3;
                    objDB.lao = resultApi.lao;
                    objDB.quakhu = resultApi.quakhu;
                    objDB.hientai = resultApi.hientai;
                    objDB.hientai_2 = resultApi.hientai_2;
                    objDB.ganc = resultApi.ganc;
                    objDB.quakhu_2 = resultApi.quakhu_2;
                    objDB.hientai_4 = resultApi.hientai_4;
                    objDB.hientai_3 = resultApi.hientai_3;
                    objDB.trieuchung_2 = resultApi.trieuchung_2;
                    objDB.khac_5 = resultApi.khac_5;
                    objDB.cau1 = resultApi.cau1;
                    objDB.cau2 = resultApi.cau2;
                    objDB.cau3 = resultApi.cau3;
                    objDB.cau4 = resultApi.cau4;
                    objDB.cau5 = resultApi.cau5;
                    objDB.cau6 = resultApi.cau6;
                    objDB.cau7 = resultApi.cau7;
                    objDB.cau8 = resultApi.cau8;
                    objDB.cau9 = resultApi.cau9;
                    objDB.cau10 = resultApi.cau10;
                    objDB.cau11 = resultApi.cau11;
                    objDB.cau12 = resultApi.cau12;
                    objDB.cau13 = resultApi.cau13;
                    objDB.cau14 = resultApi.cau14;
                    objDB.cau15 = resultApi.cau15;
                    objDB.cau16 = resultApi.cau16;
                    objDB.cau17 = resultApi.cau17;
                    objDB.cau18 = resultApi.cau18;
                    objDB.cau19 = resultApi.cau19;
                    objDB.cau20 = resultApi.cau20;
                    objDB.cau21 = resultApi.cau21;
                    objDB.cau22 = resultApi.cau22;
                    objDB.cau23 = resultApi.cau23;
                    objDB.cau24 = resultApi.cau24;
                    objDB.cau25 = resultApi.cau25;
                    objDB.cau26 = resultApi.cau26;
                    objDB.cau27 = resultApi.cau27;
                    objDB.assist = resultApi.assist;
                    objDB.thuocla = resultApi.thuocla;
                    objDB.thucuong = resultApi.thucuong;
                    objDB.cansa = resultApi.cansa;
                    objDB.cocain = resultApi.cocain;
                    objDB.chatkichthich = resultApi.chatkichthich;
                    objDB.khixong = resultApi.khixong;
                    objDB.thuocanthan = resultApi.thuocanthan;
                    objDB.chatgayaogiac = resultApi.chatgayaogiac;
                    objDB.thuocphien = resultApi.thuocphien;
                    objDB.chatkhac = resultApi.chatkhac;
                    objDB.cacchatkhac = resultApi.cacchatkhac;
                    objDB.lucdihoc = resultApi.lucdihoc;
                    objDB.thuocla1 = resultApi.thuocla1;
                    objDB.thucuong1 = resultApi.thucuong1;
                    objDB.cansa1 = resultApi.cansa1;
                    objDB.coca1 = resultApi.coca1;
                    objDB.chatkichthich1 = resultApi.chatkichthich1;
                    objDB.khixong1 = resultApi.khixong1;
                    objDB.thuocanthan1 = resultApi.thuocanthan1;
                    objDB.chatgayaogiac1 = resultApi.chatgayaogiac1;
                    objDB.chatthuocphien1 = resultApi.chatthuocphien1;
                    objDB.chatkhac1 = resultApi.chatkhac1;
                    objDB.thuocla2 = resultApi.thuocla2;
                    objDB.thucuong2 = resultApi.thucuong2;
                    objDB.cansa2 = resultApi.cansa2;
                    objDB.coca2 = resultApi.coca2;
                    objDB.chatkichthich2 = resultApi.chatkichthich2;
                    objDB.khixong2 = resultApi.khixong2;
                    objDB.thuocanthan2 = resultApi.thuocanthan2;
                    objDB.chatgayaogiac2 = resultApi.chatgayaogiac2;
                    objDB.chatthuocphien2 = resultApi.chatthuocphien2;
                    objDB.chatkhac2 = resultApi.chatkhac2;
                    objDB.thuocla3 = resultApi.thuocla3;
                    objDB.thucuong3 = resultApi.thucuong3;
                    objDB.cansa3 = resultApi.cansa3;
                    objDB.coca3 = resultApi.coca3;
                    objDB.chatkichthich3 = resultApi.chatkichthich3;
                    objDB.khixong3 = resultApi.khixong3;
                    objDB.thuocanthan3 = resultApi.thuocanthan3;
                    objDB.chatgayaogiac3 = resultApi.chatgayaogiac3;
                    objDB.chatthuocphien3 = resultApi.chatthuocphien3;
                    objDB.chatkhac3 = resultApi.chatkhac3;
                    objDB.thuocla4 = resultApi.thuocla4;
                    objDB.thucuong4 = resultApi.thucuong4;
                    objDB.cansa4 = resultApi.cansa4;
                    objDB.coca4 = resultApi.coca4;
                    objDB.chatkichthich4 = resultApi.chatkichthich4;
                    objDB.khixong4 = resultApi.khixong4;
                    objDB.thuocanthan4 = resultApi.thuocanthan4;
                    objDB.chatgayaogiac4 = resultApi.chatgayaogiac4;
                    objDB.chatthuocphien4 = resultApi.chatthuocphien4;
                    objDB.chatkhac4 = resultApi.chatkhac4;
                    objDB.thuocla5 = resultApi.thuocla5;
                    objDB.thucuong5 = resultApi.thucuong5;
                    objDB.cansa5 = resultApi.cansa5;
                    objDB.coca5 = resultApi.coca5;
                    objDB.chatkichthich5 = resultApi.chatkichthich5;
                    objDB.khixong5 = resultApi.khixong5;
                    objDB.thuocanthan5 = resultApi.thuocanthan5;
                    objDB.chatgayaogiac5 = resultApi.chatgayaogiac5;
                    objDB.chatthuocphien5 = resultApi.chatthuocphien5;
                    objDB.chatkhac5 = resultApi.chatkhac5;
                    objDB.thuocla6 = resultApi.thuocla6;
                    objDB.thucuong6 = resultApi.thucuong6;
                    objDB.cansa6 = resultApi.cansa6;
                    objDB.coca6 = resultApi.coca6;
                    objDB.chatkichthich6 = resultApi.chatkichthich6;
                    objDB.khixong6 = resultApi.khixong6;
                    objDB.thuocanthan6 = resultApi.thuocanthan6;
                    objDB.chatgayaogiac6 = resultApi.chatgayaogiac6;
                    objDB.chatthuocphien6 = resultApi.chatthuocphien6;
                    objDB.chatkhac6 = resultApi.chatkhac6;
                    objDB.cau_8 = resultApi.cau_8;
                    objDB.diemthuocla = resultApi.diemthuocla;
                    objDB.nguycothap = resultApi.nguycothap;
                    objDB.nguycotrungbinh = resultApi.nguycotrungbinh;
                    objDB.nguycocao = resultApi.nguycocao;
                    objDB.kocanthiep = resultApi.kocanthiep;
                    objDB.canthiepngan = resultApi.canthiepngan;
                    objDB.chuachuyensau = resultApi.chuachuyensau;
                    objDB.diemthucuong = resultApi.diemthucuong;
                    objDB.nguycothap_2 = resultApi.nguycothap_2;
                    objDB.nguycotrungbinh_2 = resultApi.nguycotrungbinh_2;
                    objDB.nguycocao_2 = resultApi.nguycocao_2;
                    objDB.kocanthiep_2 = resultApi.kocanthiep_2;
                    objDB.canthiepngan_2 = resultApi.canthiepngan_2;
                    objDB.chuachuyensau_2 = resultApi.chuachuyensau_2;
                    objDB.diemcansa = resultApi.diemcansa;
                    objDB.nguycothap_3 = resultApi.nguycothap_3;
                    objDB.nguycotrungbinh_3 = resultApi.nguycotrungbinh_3;
                    objDB.nguycocao_3 = resultApi.nguycocao_3;
                    objDB.kocanthiep_3 = resultApi.kocanthiep_3;
                    objDB.canthiepngan_3 = resultApi.canthiepngan_3;
                    objDB.chuachuyensau_3 = resultApi.chuachuyensau_3;
                    objDB.diemcoca = resultApi.diemcoca;
                    objDB.nguycothap_4 = resultApi.nguycothap_4;
                    objDB.nguycotrungbinh_4 = resultApi.nguycotrungbinh_4;
                    objDB.nguycocao_4 = resultApi.nguycocao_4;
                    objDB.kocanthiep_4 = resultApi.kocanthiep_4;
                    objDB.canthiepngan_4 = resultApi.canthiepngan_4;
                    objDB.chuachuyensau_4 = resultApi.chuachuyensau_4;
                    objDB.diemchatkichthich = resultApi.diemchatkichthich;
                    objDB.nguycothap_5 = resultApi.nguycothap_5;
                    objDB.nguycotrungbinh_5 = resultApi.nguycotrungbinh_5;
                    objDB.nguycocao_5 = resultApi.nguycocao_5;
                    objDB.kocanthiep_5 = resultApi.kocanthiep_5;
                    objDB.canthiepngan_5 = resultApi.canthiepngan_5;
                    objDB.chuachuyensau_5 = resultApi.chuachuyensau_5;
                    objDB.diemkhixong = resultApi.diemkhixong;
                    objDB.nguycothap_6 = resultApi.nguycothap_6;
                    objDB.nguycotrungbinh_6 = resultApi.nguycotrungbinh_6;
                    objDB.nguycocao_6 = resultApi.nguycocao_6;
                    objDB.kocanthiep_6 = resultApi.kocanthiep_6;
                    objDB.canthiepngan_6 = resultApi.canthiepngan_6;
                    objDB.chuachuyensau_6 = resultApi.chuachuyensau_6;
                    
                    objDB.nguycothap_7 = resultApi.nguycothap_7;
                    objDB.nguycotrungbinh_7 = resultApi.nguycotrungbinh_7;
                    objDB.nguycocao_7 = resultApi.nguycocao_7;
                    objDB.kocanthiep_7 = resultApi.kocanthiep_7;
                    objDB.canthiepngan_7 = resultApi.canthiepngan_7;
                    objDB.chuachuyensau_7 = resultApi.chuachuyensau_7;
                    objDB.diemchatgayaogiac = resultApi.diemchatgayaogiac;
                    objDB.nguycothap_8 = resultApi.nguycothap_8;
                    objDB.nguycotrungbinh_8 = resultApi.nguycotrungbinh_8;
                    objDB.nguycocao_8 = resultApi.nguycocao_8;
                    objDB.kocanthiep_8 = resultApi.kocanthiep_8;
                    objDB.canthiepngan_8 = resultApi.canthiepngan_8;
                    objDB.chuachuyensau_8 = resultApi.chuachuyensau_8;
                    objDB.diemchatthuocphien = resultApi.diemchatthuocphien;
                    objDB.nguycothap_9 = resultApi.nguycothap_9;
                    objDB.nguycotrungbinh_9 = resultApi.nguycotrungbinh_9;
                    objDB.nguycocao_9 = resultApi.nguycocao_9;
                    objDB.kocanthiep_9 = resultApi.kocanthiep_9;
                    objDB.canthiepngan_9 = resultApi.canthiepngan_9;
                    objDB.chuachuyensau_9 = resultApi.chuachuyensau_9;
                    objDB.diemchatkhac = resultApi.diemchatkhac;
                    objDB.nguycothap_10 = resultApi.nguycothap_10;
                    objDB.nguycotrungbinh_10 = resultApi.nguycotrungbinh_10;
                    objDB.nguycocao_10 = resultApi.nguycocao_10;
                    objDB.kocanthiep_10 = resultApi.kocanthiep_10;
                    objDB.canthiepngan_10 = resultApi.canthiepngan_10;
                    objDB.chuachuyensau_10 = resultApi.chuachuyensau_10;
                    
                    objDB.c_1a = resultApi.c_1a;
                    objDB.c_1b = resultApi.c_1b;
                    objDB.c_1c = resultApi.c_1c;
                    objDB.c_1d = resultApi.c_1d;
                    objDB.c_2 = resultApi.c_2;
                    objDB.c_3 = resultApi.c_3;
                    objDB.c_4a = resultApi.c_4a;
                    objDB.c_4b = resultApi.c_4b;
                    objDB.c_4c = resultApi.c_4c;
                    objDB.tongdiem = resultApi.tongdiem;
                    objDB.duongtinh = resultApi.duongtinh;
                    objDB.amtinh = resultApi.amtinh;
                    objDB.thng_tin_c_bn_assist_qst_kin_thc_complete = resultApi.thng_tin_c_bn_assist_qst_kin_thc_complete;

                    lsObjDB.Add(objDB);

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC sang entity**************************************");
        }

        public void ConvertApiCH07TTTTToEntity(List<ResultApiCH07ThongTinTruyenThongModel> resultApiModels, string maDuAn, string apiCode, string cityCodeInput, ref List<CH07_THONG_TIN_TRUYEN_THONG> lsObjDB)
        {

            log.Info("********************************Bắt đầu chuyển đổi kết quả api BVTL_THONG_TIN_TRUYEN_THONG sang entity**************************************");
            var objDB = new CH07_THONG_TIN_TRUYEN_THONG();
            var resultApi = new ResultApiCH07ThongTinTruyenThongModel();
            try
            {

                //var customers = db.BVTL_KHACH_HANG.ToList();
                var nhomTBHs = db.BVTL_NHOM_TBH.ToList();
                var loaiDoiTuongs = db.BVTL_LOAI_DOI_TUONG.ToList();
                //var customer = new BVTL_KHACH_HANG();
                //var customer_code = "";

                var group_code = "";
                var customer_code = "";
                var cityCode = "";

                //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                string cityCodeTemp = apiCode.Split('_')[1];
                if (!string.IsNullOrEmpty(cityCodeTemp))
                {
                    cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                }

                var nhomTBH = new BVTL_NHOM_TBH();
                var month = 0;
                var day = 0;
                var year = 0;
                var ngaynhap = "";
                var ngaynhapD = new DateTime();

                log.Info("*********-----TỔNG SỐ RECORD API CH07_THONG_TIN_TRUYEN_THONG:" + resultApiModels.Count + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);
                int errNo = 0;
                for (int i = 0; i < resultApiModels.Count; i++)
                {
                    resultApi = resultApiModels[i];


                    //cityCode = "";
                    //nhomTBH = new BVTL_NHOM_TBH();
                    month = 0;
                    day = 0;
                    year = 0;
                    ngaynhap = "";
                    ngaynhapD = new DateTime();

                    //customer = new BVTL_KHACH_HANG();
                    //customer_code = String.Concat(resultApi.makh, resultApi.makh_2, resultApi.makh_3, resultApi.makh_4, resultApi.makh_5, resultApi.makh_6, resultApi.makh_7, resultApi.makh_8, resultApi.makh_9, resultApi.makh_10
                    //                                , resultApi.makh_11, resultApi.makh_12, resultApi.makh_13, resultApi.makh_14, resultApi.makh_15);

                    List<string> listCodeMaKh = getListValFromMultiFieldsCH07(resultApi);

                    for (int j = 0; j < listCodeMaKh.Count; j++)
                    {
                        customer_code = listCodeMaKh[j];
                        if (!string.IsNullOrEmpty(customer_code))
                        {
                            if (customer_code.Length > 11)
                            {
                                group_code = customer_code.Substring(1, 5); //Lấy mã nhóm TBH
                                cityCode = customer_code.Substring(1, 3); // Lấy id tỉnh
                            }
                            else if (!string.IsNullOrEmpty(customer_code))
                            {
                                cityCode = customer_code.Substring(0, 3);
                                group_code = customer_code.Substring(0, 5);
                            }

                            #region Chuyển đổi dữ liệu sang bảng BVTL_THONG_TIN_TRUYEN_THONG
                            objDB = new CH07_THONG_TIN_TRUYEN_THONG()
                            {
                                makh = customer_code,
                                manhom_tbh = group_code,
                                ma_tinh = cityCode,
                                city_code = cityCodeInput,
                                maduan = maDuAn
                            };
                            //objDB.city_code = cityCode;
                            //objDB.makh = customer_code;
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
                            objDB.day = ValidateDateTimeRange(resultApi.day);
                            objDB.record_id = resultApi.record_id;
                            objDB.dichvu = resultApi.dichvu;

                            if (!string.IsNullOrEmpty(resultApi.diadiem))
                            {
                                objDB.diadiem = resultApi.diadiem;
                            }

                            if (!string.IsNullOrEmpty(resultApi.noidung))
                            {
                                objDB.noidung = resultApi.noidung;
                            }
                                                        
                            objDB.sokh = resultApi.sokh;
                           
                            if (!string.IsNullOrEmpty(resultApi.bcs))
                            {
                                objDB.bcs = Int32.Parse(resultApi.bcs);
                            }

                            if (!string.IsNullOrEmpty(resultApi.gel))
                            {
                                objDB.gel = Int32.Parse(resultApi.gel);
                            }

                            if (!string.IsNullOrEmpty(resultApi.tailieu))
                            {
                                objDB.tailieu = Int32.Parse(resultApi.tailieu);
                            }

                            if (!string.IsNullOrEmpty(resultApi.ghichu))
                            {
                                objDB.ghichu = resultApi.ghichu;
                            }

                            
                            objDB.ch_07_thng_tin_truyn_thng_complete = resultApi.ch_07_thng_tin_truyn_thng_complete;
                            

                            lsObjDB.Add(objDB);
                        }

                    }

                    #endregion

                }
                log.Info("*********-----SỐ Record CH07 - THONG TIN TRUYEN THONG ĐÃ CONVERT:" + lsObjDB.Count() + " | SỐ Record LỖI:" + errNo + " | CITY_CODE:" + cityCode + " | GROUP_CODE:" + group_code + " | MADUAN:" + maDuAn);

            }
            catch (Exception ex)
            {
                log.Error("Chuyển đổi kết quả API CH07 - THONG TIN TRUYEN THONG sang Entity lỗi: " + ex.Message + " | CITY_CODE:" + objDB.city_code + " | MADUAN:" + maDuAn);
            }
            log.Info("********************************Kết thúc chuyển đổi kết quả api CH07 - THONG TIN TRUYEN THONG sang entity**************************************");
        }


        #endregion

        public DateTime? ValidateDateTimeRange(object inputDate)
        {
            DateTime dateTime;
            string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "dd/MM/yyyy HH:mm" };
            // Trường hợp đầu vào là kiểu string
            if (inputDate is string inputString)
            {
                // Thử chuyển đổi từ chuỗi sang DateTime với các định dạng được cung cấp
                if (DateTime.TryParseExact(inputString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                    {
                        return dateTime; // Trả về DateTime hợp lệ
                    }
                }
            }
            // Trường hợp đầu vào là kiểu DateTime?
            else if (inputDate is DateTime inputDateTime)
            {
                // Kiểm tra nếu giá trị DateTime hợp lệ
                if (inputDateTime > new DateTime(1900, 1, 1) && inputDateTime < new DateTime(5000, 1, 1))
                {
                    return inputDateTime; // Trả về DateTime hợp lệ
                }
            }

            return null; // Trả về null nếu không hợp lệ hoặc không thể chuyển đổi
        }


        public DateTime? ConvertToDateTime(string inputDateString)
        {
            DateTime dateTime;
            string[] formats = { "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };
            // Kiểm tra nếu chuỗi không null
            if (!string.IsNullOrEmpty(inputDateString))
            {
                // Thử chuyển đổi chuỗi thành DateTime với các định dạng được chỉ định
                if (DateTime.TryParseExact(inputDateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    // Kiểm tra nếu dateTime nằm trong khoảng 1900-01-01 đến 5000-01-01
                    if (dateTime > new DateTime(1900, 1, 1) && dateTime < new DateTime(5000, 1, 1))
                    {
                        return dateTime; // Trả về giá trị nếu hợp lệ
                    }
                }
            }

            return null; // Trả về null nếu không chuyển đổi được hoặc không hợp lệ
        }


        private string getValFromMultiFields(List<string> lsVals)
        {
            string strVal = "";
            IEnumerable<string> distinctVals = lsVals.Where(s=>!string.IsNullOrWhiteSpace(s)).Distinct();
            strVal = distinctVals.FirstOrDefault();
            return strVal;
        }

        private List<string> getListValFromMultiFields(ResultApiBVTLTTTTModel resultApi)
        {
            string[] listCodeMaKh = { resultApi.ma1, resultApi.ma2, resultApi.ma3, resultApi.ma4, resultApi.ma5, resultApi.ma6, resultApi.ma7, resultApi.ma8, resultApi.ma9, resultApi.ma10
                                        , resultApi.ma11, resultApi.ma12, resultApi.ma13, resultApi.ma14, resultApi.ma15, resultApi.ma16, resultApi.ma17, resultApi.ma18, resultApi.ma19, resultApi.ma20
                                        , resultApi.ma21, resultApi.ma22, resultApi.ma23, resultApi.ma24, resultApi.ma25, resultApi.ma26, resultApi.ma27, resultApi.ma28, resultApi.ma29, resultApi.ma30
                                        , resultApi.ma1_1, resultApi.ma1_2, resultApi.ma1_3, resultApi.ma1_4, resultApi.ma1_5, resultApi.ma1_6, resultApi.ma1_7, resultApi.ma1_8, resultApi.ma1_9, resultApi.ma1_10, resultApi.ma1_11, resultApi.ma1_12, resultApi.ma1_13, resultApi.ma1_14, resultApi.ma1_15, resultApi.ma1_16, resultApi.ma1_17, resultApi.ma1_18, resultApi.ma1_19, resultApi.ma1_20, resultApi.ma1_21, resultApi.ma1_22, resultApi.ma1_23, resultApi.ma1_24, resultApi.ma1_25, resultApi.ma1_26, resultApi.ma1_27, resultApi.ma1_28, resultApi.ma1_29, resultApi.ma1_30
                                        , resultApi.ma2_1, resultApi.ma2_2, resultApi.ma2_3, resultApi.ma2_4, resultApi.ma2_5, resultApi.ma2_6, resultApi.ma2_7, resultApi.ma2_8, resultApi.ma2_9, resultApi.ma2_10, resultApi.ma2_11, resultApi.ma2_12, resultApi.ma2_13, resultApi.ma2_14, resultApi.ma2_15, resultApi.ma2_16, resultApi.ma2_17, resultApi.ma2_18, resultApi.ma2_19, resultApi.ma2_20, resultApi.ma2_21, resultApi.ma2_22, resultApi.ma2_23, resultApi.ma2_24, resultApi.ma2_25, resultApi.ma2_26, resultApi.ma2_27, resultApi.ma2_28, resultApi.ma2_29, resultApi.ma2_30
                                        , resultApi.ma3_1, resultApi.ma3_2, resultApi.ma3_3, resultApi.ma3_4, resultApi.ma3_5, resultApi.ma3_6, resultApi.ma3_7, resultApi.ma3_8, resultApi.ma3_9, resultApi.ma3_10, resultApi.ma3_11, resultApi.ma3_12, resultApi.ma3_13, resultApi.ma3_14, resultApi.ma3_15, resultApi.ma3_16, resultApi.ma3_17, resultApi.ma3_18, resultApi.ma3_19, resultApi.ma3_20, resultApi.ma3_21, resultApi.ma3_22, resultApi.ma3_23, resultApi.ma3_24, resultApi.ma3_25, resultApi.ma3_26, resultApi.ma3_27, resultApi.ma3_28, resultApi.ma3_29, resultApi.ma3_30
                                        , resultApi.ma4_1, resultApi.ma4_2, resultApi.ma4_3, resultApi.ma4_4, resultApi.ma4_5, resultApi.ma4_6, resultApi.ma4_7, resultApi.ma4_8, resultApi.ma4_9, resultApi.ma4_10, resultApi.ma4_11, resultApi.ma4_12, resultApi.ma4_13, resultApi.ma4_14, resultApi.ma4_15, resultApi.ma4_16, resultApi.ma4_17, resultApi.ma4_18, resultApi.ma4_19, resultApi.ma4_20, resultApi.ma4_21, resultApi.ma4_22, resultApi.ma4_23, resultApi.ma4_24, resultApi.ma4_25, resultApi.ma4_26, resultApi.ma4_27, resultApi.ma4_28, resultApi.ma4_29, resultApi.ma4_30
                                        , resultApi.ma5_1, resultApi.ma5_2, resultApi.ma5_3, resultApi.ma5_4, resultApi.ma5_5, resultApi.ma5_6, resultApi.ma5_7, resultApi.ma5_8, resultApi.ma5_9, resultApi.ma5_10, resultApi.ma5_11, resultApi.ma5_12, resultApi.ma5_13, resultApi.ma5_14, resultApi.ma5_15, resultApi.ma5_16, resultApi.ma5_17, resultApi.ma5_18, resultApi.ma5_19, resultApi.ma5_20, resultApi.ma5_21, resultApi.ma5_22, resultApi.ma5_23, resultApi.ma5_24, resultApi.ma5_25, resultApi.ma5_26, resultApi.ma5_27, resultApi.ma5_28, resultApi.ma5_29, resultApi.ma5_30
                                        , resultApi.ma6_1, resultApi.ma6_2, resultApi.ma6_3, resultApi.ma6_4, resultApi.ma6_5, resultApi.ma6_6, resultApi.ma6_7, resultApi.ma6_8, resultApi.ma6_9, resultApi.ma6_10, resultApi.ma6_11, resultApi.ma6_12, resultApi.ma6_13, resultApi.ma6_14, resultApi.ma6_15, resultApi.ma6_16, resultApi.ma6_17, resultApi.ma6_18, resultApi.ma6_19, resultApi.ma6_20, resultApi.ma6_21, resultApi.ma6_22, resultApi.ma6_23, resultApi.ma6_24, resultApi.ma6_25, resultApi.ma6_26, resultApi.ma6_27, resultApi.ma6_28, resultApi.ma6_29, resultApi.ma6_30
                                    };

            var ls = listCodeMaKh.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return ls;
        }
        private List<string> getListValFromMultiFieldsCH07(ResultApiCH07ThongTinTruyenThongModel resultApi)
        {
            string[] listCodeMaKh = {
                    resultApi.ma_hno34_1 ,
                    resultApi.ma_hno34_2 ,
                    resultApi.ma_hno34_3 ,
                    resultApi.ma_hno34_4 ,
                    resultApi.ma_hno34_5 ,
                    resultApi.ma_hno34_6 ,
                    resultApi.ma_hno34_7 ,
                    resultApi.ma_hno34_8 ,
                    resultApi.ma_hno34_9 ,
                    resultApi.ma_hno34_10,
                    resultApi.ma_hno34_11,
                    resultApi.ma_hno34_12,
                    resultApi.ma_hno34_13,
                    resultApi.ma_hno34_14,
                    resultApi.ma_hno34_15,
                    resultApi.ma_hno34_16,
                    resultApi.ma_hno34_17,
                    resultApi.ma_hno34_18,
                    resultApi.ma_hno34_19,
                    resultApi.ma_hno34_20,
                    resultApi.ma_hno34_21,
                    resultApi.ma_hno34_22,
                    resultApi.ma_hno34_23,
                    resultApi.ma_hno34_24,
                    resultApi.ma_hno34_25,
                    resultApi.ma_hno34_26,
                    resultApi.ma_hno34_27,
                    resultApi.ma_hno34_28,
                    resultApi.ma_hno34_29,
                    resultApi.ma_hno34_30,


                    resultApi.ma_hno18_1 ,
                    resultApi.ma_hno18_2 ,
                    resultApi.ma_hno18_3 ,
                    resultApi.ma_hno18_4 ,
                    resultApi.ma_hno18_5 ,
                    resultApi.ma_hno18_6 ,
                    resultApi.ma_hno18_7 ,
                    resultApi.ma_hno18_8 ,
                    resultApi.ma_hno18_9 ,
                    resultApi.ma_hno18_10,
                    resultApi.ma_hno18_11,
                    resultApi.ma_hno18_12,
                    resultApi.ma_hno18_13,
                    resultApi.ma_hno18_14,
                    resultApi.ma_hno18_15,
                    resultApi.ma_hno18_16,
                    resultApi.ma_hno18_17,
                    resultApi.ma_hno18_18,
                    resultApi.ma_hno18_19,
                    resultApi.ma_hno18_20,
                    resultApi.ma_hno18_21,
                    resultApi.ma_hno18_22,
                    resultApi.ma_hno18_23,
                    resultApi.ma_hno18_24,
                    resultApi.ma_hno18_25,
                    resultApi.ma_hno18_26,
                    resultApi.ma_hno18_27,
                    resultApi.ma_hno18_28,
                    resultApi.ma_hno18_29,
                    resultApi.ma_hno18_30,

                    resultApi.ma_nan08_1 ,
                    resultApi.ma_nan08_2 ,
                    resultApi.ma_nan08_3 ,
                    resultApi.ma_nan08_4 ,
                    resultApi.ma_nan08_5 ,
                    resultApi.ma_nan08_6 ,
                    resultApi.ma_nan08_7 ,
                    resultApi.ma_nan08_8 ,
                    resultApi.ma_nan08_9 ,
                    resultApi.ma_nan08_10,
                    resultApi.ma_nan08_11,
                    resultApi.ma_nan08_12,
                    resultApi.ma_nan08_13,
                    resultApi.ma_nan08_14,
                    resultApi.ma_nan08_15,
                    resultApi.ma_nan08_16,
                    resultApi.ma_nan08_17,
                    resultApi.ma_nan08_18,
                    resultApi.ma_nan08_19,
                    resultApi.ma_nan08_20,
                    resultApi.ma_nan08_21,
                    resultApi.ma_nan08_22,
                    resultApi.ma_nan08_23,
                    resultApi.ma_nan08_24,
                    resultApi.ma_nan08_25,
                    resultApi.ma_nan08_26,
                    resultApi.ma_nan08_27,
                    resultApi.ma_nan08_28,
                    resultApi.ma_nan08_29,
                    resultApi.ma_nan08_30,

                    resultApi.ma_tbh11_1 ,
                    resultApi.ma_tbh11_2 ,
                    resultApi.ma_tbh11_3 ,
                    resultApi.ma_tbh11_4 ,
                    resultApi.ma_tbh11_5 ,
                    resultApi.ma_tbh11_6 ,
                    resultApi.ma_tbh11_7 ,
                    resultApi.ma_tbh11_8 ,
                    resultApi.ma_tbh11_9 ,
                    resultApi.ma_tbh11_10,
                    resultApi.ma_tbh11_11,
                    resultApi.ma_tbh11_12,
                    resultApi.ma_tbh11_13,
                    resultApi.ma_tbh11_14,
                    resultApi.ma_tbh11_15,
                    resultApi.ma_tbh11_16,
                    resultApi.ma_tbh11_17,
                    resultApi.ma_tbh11_18,
                    resultApi.ma_tbh11_19,
                    resultApi.ma_tbh11_20,
                    resultApi.ma_tbh11_21,
                    resultApi.ma_tbh11_22,
                    resultApi.ma_tbh11_23,
                    resultApi.ma_tbh11_24,
                    resultApi.ma_tbh11_25,
                    resultApi.ma_tbh11_26,
                    resultApi.ma_tbh11_27,
                    resultApi.ma_tbh11_28,
                    resultApi.ma_tbh11_29,
                    resultApi.ma_tbh11_30,

                    resultApi.ma_nbi12_1,
                    resultApi.ma_nbi12_2,
                    resultApi.ma_nbi12_3,
                    resultApi.ma_nbi12_4,
                    resultApi.ma_nbi12_5,
                    resultApi.ma_nbi12_6,
                    resultApi.ma_nbi12_7,
                    resultApi.ma_nbi12_8,
                    resultApi.ma_nbi12_9,
                    resultApi.ma_nbi12_10,
                    resultApi.ma_nbi12_11,
                    resultApi.ma_nbi12_12,
                    resultApi.ma_nbi12_13,
                    resultApi.ma_nbi12_14,
                    resultApi.ma_nbi12_15,
                    resultApi.ma_nbi12_16,
                    resultApi.ma_nbi12_17,
                    resultApi.ma_nbi12_18,
                    resultApi.ma_nbi12_19,
                    resultApi.ma_nbi12_20,
                    resultApi.ma_nbi12_21,
                    resultApi.ma_nbi12_22,
                    resultApi.ma_nbi12_23,
                    resultApi.ma_nbi12_24,
                    resultApi.ma_nbi12_25,
                    resultApi.ma_nbi12_26,
                    resultApi.ma_nbi12_27,
                    resultApi.ma_nbi12_28,
                    resultApi.ma_nbi12_29,
                    resultApi.ma_nbi12_30,

                resultApi.ma_nbi16_1,
                resultApi.ma_nbi16_2,
                resultApi.ma_nbi16_3,
                resultApi.ma_nbi16_4,
                resultApi.ma_nbi16_5,
                resultApi.ma_nbi16_6,
                resultApi.ma_nbi16_7,
                resultApi.ma_nbi16_8,
                resultApi.ma_nbi16_9,
                resultApi.ma_nbi16_10,
                resultApi.ma_nbi16_11,
                resultApi.ma_nbi16_12,
                resultApi.ma_nbi16_13,
                resultApi.ma_nbi16_14,
                resultApi.ma_nbi16_15,
                resultApi.ma_nbi16_16,
                resultApi.ma_nbi16_17,
                resultApi.ma_nbi16_18,
                resultApi.ma_nbi16_19,
                resultApi.ma_nbi16_20,
                resultApi.ma_nbi16_21,
                resultApi.ma_nbi16_22,
                resultApi.ma_nbi16_23,
                resultApi.ma_nbi16_24,
                resultApi.ma_nbi16_25,
                resultApi.ma_nbi16_26,
                resultApi.ma_nbi16_27,
                resultApi.ma_nbi16_28,
                resultApi.ma_nbi16_29,
                resultApi.ma_nbi16_30,

                resultApi.ma_nbi17_1,
                resultApi.ma_nbi17_2,
                resultApi.ma_nbi17_3,
                resultApi.ma_nbi17_4,
                resultApi.ma_nbi17_5,
                resultApi.ma_nbi17_6,
                resultApi.ma_nbi17_7,
                resultApi.ma_nbi17_8,
                resultApi.ma_nbi17_9,
                resultApi.ma_nbi17_10,
                resultApi.ma_nbi17_11,
                resultApi.ma_nbi17_12,
                resultApi.ma_nbi17_13,
                resultApi.ma_nbi17_14,
                resultApi.ma_nbi17_15,
                resultApi.ma_nbi17_16,
                resultApi.ma_nbi17_17,
                resultApi.ma_nbi17_18,
                resultApi.ma_nbi17_19,
                resultApi.ma_nbi17_20,
                resultApi.ma_nbi17_21,
                resultApi.ma_nbi17_22,
                resultApi.ma_nbi17_23,
                resultApi.ma_nbi17_24,
                resultApi.ma_nbi17_25,
                resultApi.ma_nbi17_26,
                resultApi.ma_nbi17_27,
                resultApi.ma_nbi17_28,
                resultApi.ma_nbi17_29,
                resultApi.ma_nbi17_30,

                resultApi.ma_nan07_1,
                resultApi.ma_nan07_2,
                resultApi.ma_nan07_3,
                resultApi.ma_nan07_4,
                resultApi.ma_nan07_5,
                resultApi.ma_nan07_6,
                resultApi.ma_nan07_7,
                resultApi.ma_nan07_8,
                resultApi.ma_nan07_9,
                resultApi.ma_nan07_10,
                resultApi.ma_nan07_11,
                resultApi.ma_nan07_12,
                resultApi.ma_nan07_13,
                resultApi.ma_nan07_14,
                resultApi.ma_nan07_15,
                resultApi.ma_nan07_16,
                resultApi.ma_nan07_17,
                resultApi.ma_nan07_18,
                resultApi.ma_nan07_19,
                resultApi.ma_nan07_20,
                resultApi.ma_nan07_21,
                resultApi.ma_nan07_22,
                resultApi.ma_nan07_23,
                resultApi.ma_nan07_24,
                resultApi.ma_nan07_25,
                resultApi.ma_nan07_26,
                resultApi.ma_nan07_27,
                resultApi.ma_nan07_28,
                resultApi.ma_nan07_29,
                resultApi.ma_nan07_30,

            };

            var ls = listCodeMaKh.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return ls;
        }



    }
}
