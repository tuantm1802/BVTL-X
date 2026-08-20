using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.API
{
    public class SyncDataFromApi_SaveToDB: ISyncDataFromApi_SaveToDB
    {
        IInsertDataDA insertDataDA = new InsertDataDA();
        IGetDataFromAPI getDataFromAPI = new GetDataFromAPI();
        IConvertResultApiToEntity _convertResultApiToEntity = new ConvertResultApiToEntity();
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<BaseResult> GetDataFromApi_SaveToDB(string urlApi, string token, string reportId, string maDuAn, List<string> tableNames, string apiCode, string rawOrLabel)
        {
            var result = new BaseResult();
            var resultChiTiet = new BaseResult();

            // Cập nhật thời gian bắt đầu đồng bộ
            insertDataDA.UpdateTimeSync(apiCode, true, "");
            try
            {
                // Call api để lấy dữ liệu
                var resultApiString = await getDataFromAPI.PostDataFromApiReturnString(urlApi, token, reportId, rawOrLabel);
                if (!string.IsNullOrEmpty(resultApiString))
                {

                    #region Lưu dữ liệu từ api vào db
                    // Đầu api SKTT
                    if (tableNames.Contains("BVTL_KQ_SL_ASSIST"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApi1344Model>>(resultApiString);

                        var sktts = new List<BVTL_KQ_SL_SKTT>();
                        var assists = new List<BVTL_KQ_SL_ASSIST>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApi1344ToEntity(dataResultApi.Where(x=>!string.IsNullOrEmpty(x.makh)).ToList(), maDuAn, ref sktts, ref assists);
                        _convertResultApiToEntity.ConvertApi1344ToEntity(dataResultApi.Where(x=>x.sng_lc_assist_complete.Equals("Complete") 
                                                                                            || x.sng_lc_qst_complete.Equals("Complete")
                                                                                            || x.thng_tin_c_bn_v_hnh_vi_nguy_c_complete.Equals("Complete")
                                                                                            || x.bng_hi_nh_gi_kin_thc_complete.Equals("Complete")                                                                                            
                                                                                            ).ToList(), maDuAn, ref sktts, ref assists);

                        // Thêm dữ liệu bảng BVTL_KQ_SL_SKTT
                        if (sktts != null && sktts.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(sktts);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_SKTT", sktts.FirstOrDefault().city_code, maDuAn);
                        }

                        // Thêm dữ liệu bảng BVTL_KQ_SL_SKTT
                        if (assists != null && assists.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(assists);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_ASSIST", assists.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api HIV
                    if (tableNames.Contains("BVTL_KQ_XN_HIV"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiHIVModel>>(resultApiString);

                        var hivs = new List<BVTL_KQ_XN_HIV>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiHIVToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.makh.Length > 10).ToList(), maDuAn, ref hivs);

                        // Thêm dữ liệu bảng BVTL_KQ_XN_HIV
                        if (hivs != null && hivs.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(hivs);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_XN_HIV", hivs.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api ACE
                    if (tableNames.Contains("BVTL_KQ_SL_ACE"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiACEModel>>(resultApiString);

                        var aces = new List<BVTL_KQ_SL_ACE>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiACEToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh)).ToList(), maDuAn, ref aces);
                        _convertResultApiToEntity.ConvertApiACEToEntity(dataResultApi.Where(x => x.bng_hi_ace_complete.Equals("Complete")).ToList(), maDuAn, ref aces);

                        // Thêm dữ liệu bảng BVTL_KQ_SL_ACE
                        if (aces != null && aces.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(aces);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_ACE", aces.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api tổng hợp
                    if (tableNames.Contains("BVTL_BO_BIEU_MAU_KH_BAO_CAO"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTongHopModel>>(resultApiString);

                        var tongHops = new List<BVTL_BO_BIEU_MAU_KH_BAO_CAO>();
                        var khachHangs = new List<BVTL_KHACH_HANG>();
                        dataResultApi = dataResultApi.GroupBy(x => x.makh).Select(y => y.FirstOrDefault()).ToList();
                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiTongHopToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh)).ToList(), maDuAn, ref tongHops, ref khachHangs);

                        // Thêm dữ liệu bảng BVTL_KHACH_HANG
                        if (khachHangs != null && khachHangs.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(khachHangs);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KHACH_HANG", khachHangs.FirstOrDefault().city_code, maDuAn);
                        }

                        // Thêm dữ liệu bảng BVTL_BO_BIEU_MAU_KH_BAO_CAO
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_BO_BIEU_MAU_KH_BAO_CAO", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api phiếu tư vấn
                    if (tableNames.Contains("BVTL_PHIEU_TU_VAN"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiPhieuTuVanModel>>(resultApiString);

                        var tongHops = new List<BVTL_PHIEU_TU_VAN>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiPhieuTuVanToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh)).ToList(), maDuAn, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiPhieuTuVanToEntity(dataResultApi.ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiPhieuTuVanToEntity(dataResultApi.Where(x => x.phiu_t_vn_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);

                        // Thêm dữ liệu bảng BVTL_PHIEU_TU_VAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_PHIEU_TU_VAN", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api chuyển gửi dịch vụ
                    if (tableNames.Contains("BVTL_CHUYEN_GUI_DICH_VU"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiChuyenGuiDVModel>>(resultApiString);

                        var tongHops = new List<BVTL_CHUYEN_GUI_DICH_VU>();

                        //dataResultApi = dataResultApi.GroupBy(x => x.makh).Select(y => y.FirstOrDefault()).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);

                        // Thêm dữ liệu bảng BVTL_CHUYEN_GUI_DICH_VU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_CHUYEN_GUI_DICH_VU", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }
                    
                    // Đầu api Xét nghiệm Nước tiểu
                    if (tableNames.Contains("BVTL_KQ_XN_NUOC_TIEU"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiXnNuocTieuModel>>(resultApiString);

                        var tongHops = new List<BVTL_KQ_XN_NUOC_TIEU>();
                        

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiXNNuocTieuToEntity(dataResultApi.Where(x => x.sng_lc_nc_tiu_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);

                        // Thêm dữ liệu bảng BVTL_KQ_XN_NUOC_TIEU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_XN_NUOC_TIEU", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api BVTL_THONG_TIN_TRUYEN_THONG
                    if (tableNames.Contains("BVTL_THONG_TIN_TRUYEN_THONG"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiBVTLTTTTModel>>(resultApiString);

                        var tongHops = new List<BVTL_THONG_TIN_TRUYEN_THONG>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiBVTLTTTTToEntity(dataResultApi.Where(x => x.bvtl_thng_tin_truyn_thng_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_THONG_TIN_TRUYEN_THONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_THONG_TIN_TRUYEN_THONG", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api BVTL_THEO_DAU_KH
                    if (tableNames.Contains("BVTL_THEO_DAU_KH"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTheoDauKHModel>>(resultApiString);

                        var tongHops = new List<BVTL_THEO_DAU_KH>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiBVTLTHEODAUKHoEntity(dataResultApi.Where(x => x.theo_du_kh_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_THONG_TIN_TRUYEN_THONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_THEO_DAU_KH", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api VIIV_THONG_TIN_TRUYEN_THONG
                    if (tableNames.Contains("VIIV_THONG_TIN_TRUYEN_THONG"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTTTTModel>>(resultApiString);

                        var tongHops = new List<VIIV_THONG_TIN_TRUYEN_THONG>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiTTTTToEntity(dataResultApi.Where(x => x.viiv_thng_tin_truyn_thng_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_THONG_TIN_TRUYEN_THONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "VIIV_THONG_TIN_TRUYEN_THONG", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api VIIV_TRAINING_DATA_COLLECTION
                    if (tableNames.Contains("VIIV_TRAINING_DATA_COLLECTION"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTrainingDataCollModel>>(resultApiString);

                        var tongHops = new List<VIIV_TRAINING_DATA_COLLECTION>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiTrainingDataCollToEntity(dataResultApi.Where(x => x.training_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_TRAINING_DATA_COLLECTION
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "VIIV_TRAINING_DATA_COLLECTION", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }

                    // Đầu api VIIV_DANH_GIA_HAI_LONG
                    if (tableNames.Contains("VIIV_DANH_GIA_HAI_LONG"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiDGHLModel>>(resultApiString);

                        var tongHops = new List<VIIV_DANH_GIA_HAI_LONG>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiDGHLToEntity(dataResultApi.Where(x => x.nh_gi_mc_hi_lng_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_DANH_GIA_HAI_LONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "VIIV_DANH_GIA_HAI_LONG", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                    }
                    // Đầu api VIIV_TT_KH_MAT_DAU
                    if (tableNames.Contains("VIIV_TT_KH_MAT_DAU"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTTKHMaDaModel>>(resultApiString);

                        var tongHops = new List<VIIV_TT_KH_MAT_DAU>();


                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiTTKHMaDaToEntity(dataResultApi.Where(x => x.kh_mt_du_complete.Equals("Complete")).ToList(), maDuAn, apiCode, ref tongHops);

                        // Thêm dữ liệu bảng VIIV_TT_KH_MAT_DAU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "VIIV_TT_KH_MAT_DAU", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_THONG_TIN_CO_BAN - #01
                    if (tableNames.Contains("CD43_KHACH_HANG_THONG_TIN_CO_BAN"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangTTCBModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_THONG_TIN_CO_BAN>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiKhachHangTTCBEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_THONG_TIN_CO_BAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_THONG_TIN_CO_BAN", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU - #02
                    if (tableNames.Contains("CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangSangLocNuocTieuModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangSangLocNuocTieuEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.sng_lc_nc_tiu_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_SANG_LOC_HIV - #03
                    if (tableNames.Contains("CD43_KHACH_HANG_SANG_LOC_HIV"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangSangLocHIVModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_SANG_LOC_HIV>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangSangLocHIVEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.sng_lc_hiv_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_SANG_LOC_HIV
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_SANG_LOC_HIV", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_DANH_GIA_HAI_LONG - #04
                    if (tableNames.Contains("CD43_KHACH_HANG_DANH_GIA_HAI_LONG"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangDanhGiaHaiLongModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_DANH_GIA_HAI_LONG>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangDanhGiaHaiLongEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.nh_gi_mc_hi_lng_ca_kh_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiKhachHangDanhGiaHaiLongEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) ).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_DANH_GIA_HAI_LONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_DANH_GIA_HAI_LONG", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }

                    }

                    // Đầu api CD43_KHACH_HANG_THEO_DAU - #05
                    if (tableNames.Contains("CD43_KHACH_HANG_THEO_DAU"))
                    {
                       
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangTheoDauModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_THEO_DAU>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangTheoDauEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.theo_du_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiKhachHangTheoDauEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_THEO_DAU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_THEO_DAU", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_SINH_HOAT_NHOM - #06
                    if (tableNames.Contains("CD43_KHACH_HANG_SINH_HOAT_NHOM"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangSinhHoatNhomModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_SINH_HOAT_NHOM>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiKhachHangSinhHoatNhomEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.sinh_hot_nhm_complete.Equals("2")).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        _convertResultApiToEntity.ConvertApiKhachHangSinhHoatNhomEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) ).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_SINH_HOAT_NHOM
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_SINH_HOAT_NHOM", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_PHIEU_TU_VAN - #07
                    if (tableNames.Contains("CD43_KHACH_HANG_PHIEU_TU_VAN"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangPhieuTuVanModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_PHIEU_TU_VAN>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangPhieuTuVanEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.phiu_t_vn_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiKhachHangPhieuTuVanEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_PHIEU_TU_VAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_PHIEU_TU_VAN", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_CHUYEN_GUI - #08
                    if (tableNames.Contains("CD43_KHACH_HANG_CHUYEN_GUI"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangChuyenGuiModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_CHUYEN_GUI>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiKhachHangChuyenGuiEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.chuyn_gi_complete.Equals("2")).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        _convertResultApiToEntity.ConvertApiKhachHangChuyenGuiEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_CHUYEN_GUI
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_CHUYEN_GUI", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_HANH_VI_NGUY_CO - #09
                    if (tableNames.Contains("CD43_KHACH_HANG_HANH_VI_NGUY_CO"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangHanhViNguyCoModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_HANH_VI_NGUY_CO>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangHanhViNguyCoEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiKhachHangHanhViNguyCoEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_HANH_VI_NGUY_CO
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_HANH_VI_NGUY_CO", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_KHACH_HANG_DANH_GIA_TAC_DONG - #10
                    if (tableNames.Contains("CD43_KHACH_HANG_DANH_GIA_TAC_DONG"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangDanhGiaTacDongModel>>(resultApiString);

                        var tongHops = new List<CD43_KHACH_HANG_DANH_GIA_TAC_DONG>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangDanhGiaTacDongEntity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.phng_vn_nh_gi_tc_ng_complete == 2).ToList(), maDuAn, apiCode, cityCode, ref tongHops);
                        

                        // Thêm dữ liệu bảng CD43_KHACH_HANG_DANH_GIA_TAC_DONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CD43_KHACH_HANG_DANH_GIA_TAC_DONG", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }
                    #endregion

                    #region Lưu dữ liệu từ api vào db DỰ ÁN CH07

                    // Đầu api CH07_KHACH_HANG_THONG_TIN_CO_BAN - #10
                    if (tableNames.Contains("CH07_KHACH_HANG_THONG_TIN_CO_BAN"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangTTCBCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_THONG_TIN_CO_BAN>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangTTCBCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);
                        //_convertResultApiToEntity.ConvertApiKhachHangTTTCBCH07Entity(dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id)).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_THONG_TIN_CO_BAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_THONG_TIN_CO_BAN", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU - #11
                    if (tableNames.Contains("CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangSangLocNuocTieuCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.sng_lc_nc_tiu_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangSangLocNuocTieuCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_SANG_LOC_HIV - #12
                    if (tableNames.Contains("CH07_KHACH_HANG_SANG_LOC_HIV"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangSangLocHIVCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_SANG_LOC_HIV>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.sng_lc_hiv_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangSangLocHIVCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_SANG_LOC_HIV
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_SANG_LOC_HIV", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_PHIEU_TU_VAN - #13
                    if (tableNames.Contains("CH07_KHACH_HANG_PHIEU_TU_VAN"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangPhieuTuVanCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_PHIEU_TU_VAN>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.phiu_t_vn_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangPhieuTuVanCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_PHIEU_TU_VAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_PHIEU_TU_VAN", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_CHUYEN_GUI - #14
                    if (tableNames.Contains("CH07_KHACH_HANG_CHUYEN_GUI"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangChuyenGuiDichVuCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_CHUYEN_GUI>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.chuyn_gi_dch_v_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangChuyenGuiCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_CHUYEN_GUI
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_CHUYEN_GUI", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_THEO_DAU - #15
                    if (tableNames.Contains("CH07_KHACH_HANG_THEO_DAU"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangTheoDauCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_THEO_DAU>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.theo_du_kh_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangTheoDauCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_THEO_DAU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_THEO_DAU", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // =======================================================================
                    // DU AN CD45 (DREAMH) - 10 BANG + DATA QUALITY LOGGING
                    // =======================================================================
                    if (tableNames.Any(t => t.StartsWith("CD45_")))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<Model.ModelExtend.API.CD45.DreamhBaseApiModel>>(resultApiString);
                        var converter = new Common.Common.ConvertCD45ApiToEntity();
                        var stdLogs = new List<Model.ModelExtend.API.CD45.BVTL_DATA_STANDARDIZATION_LOG_Entity>();

                        if (tableNames.Contains("CD45_KH")) {
                            var listF1 = new List<Model.ModelExtend.API.CD45.CD45_KH_Entity>();
                            converter.ConvertF1(dataResultApi, maDuAn, apiCode, reportId, ref listF1, ref stdLogs);
                            if (listF1.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF1);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_KH", listF1.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_HOAT_DONG")) {
                            var listF2 = new List<Model.ModelExtend.API.CD45.CD45_HOAT_DONG_Entity>();
                            converter.ConvertF2(dataResultApi, maDuAn, apiCode, reportId, ref listF2, ref stdLogs);
                            if (listF2.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF2);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_HOAT_DONG", listF2.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_QST")) {
                            var listF3 = new List<Model.ModelExtend.API.CD45.CD45_QST_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_QST", "f3", "f3_date", "bng_c_c_sng_lc_sktt_qst_complete", ref listF3, ref stdLogs);
                            if (listF3.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF3);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_QST", listF3.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_HO_TRO_XH")) {
                            var listF4 = new List<Model.ModelExtend.API.CD45.CD45_HO_TRO_XH_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_HO_TRO_XH", "f4", "f4_time", "phiu_h_tr_x_hi_khc_complete", ref listF4, ref stdLogs);
                            if (listF4.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF4);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_HO_TRO_XH", listF4.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_TUAN_THU")) {
                            var listF5 = new List<Model.ModelExtend.API.CD45.CD45_TUAN_THU_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_TUAN_THU", "f5", "f5_date", "phiu_h_tr_tun_th_iu_tr_complete", ref listF5, ref stdLogs);
                            if (listF5.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF5);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_TUAN_THU", listF5.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_CHAN_DOAN")) {
                            var listF6 = new List<Model.ModelExtend.API.CD45.CD45_CHAN_DOAN_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_CHAN_DOAN", "f6", "f6_date", "thng_tin_chn_on_v_iu_tr_complete", ref listF6, ref stdLogs);
                            if (listF6.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF6);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_CHAN_DOAN", listF6.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_TU_VAN_L1")) {
                            var listF7 = new List<Model.ModelExtend.API.CD45.CD45_TU_VAN_L1_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_TU_VAN_L1", "f7", "f7_date", "phiu_t_vn_t_ln_1_complete", ref listF7, ref stdLogs);
                            if (listF7.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF7);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_TU_VAN_L1", listF7.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_TU_VAN_L2")) {
                            var listF8 = new List<Model.ModelExtend.API.CD45.CD45_TU_VAN_L2_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_TU_VAN_L2", "f8", "f8_date", "phiu_t_vn_t_ln_2_complete", ref listF8, ref stdLogs);
                            if (listF8.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF8);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_TU_VAN_L2", listF8.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }
                        else if (tableNames.Contains("CD45_VAN_TAY")) {
                            var listF10 = new List<Model.ModelExtend.API.CD45.CD45_VAN_TAY_Entity>();
                            converter.ConvertGeneric(dataResultApi, maDuAn, apiCode, reportId, "CD45_VAN_TAY", "f10", "f10_ngay", "vn_tay_khch_hng_complete", ref listF10, ref stdLogs);
                            if (listF10.Count > 0) {
                                var dt = insertDataDA.ConvertToDataTable(listF10);
                                result = insertDataDA.InsertDataFromApi(dt, "CD45_VAN_TAY", listF10.FirstOrDefault().CITY_CODE, maDuAn);
                            }
                        }

                        // Lưu nhật ký chuẩn hóa dữ liệu & cảnh báo (nếu có phát sinh)
                        if (stdLogs.Count > 0) {
                            try {
                                var dtLogs = insertDataDA.ConvertToDataTable(stdLogs);
                                insertDataDA.InsertDataFromApi(dtLogs, "BVTL_DATA_STANDARDIZATION_LOG", "ALL", maDuAn);
                                int warnCount = stdLogs.Count(x => x.SEVERITY == "WARNING" || x.SEVERITY == "ERROR");
                                if (warnCount > 0) {
                                    result.Message += $" [Chuẩn hóa: {stdLogs.Count} bản ghi, Cảnh báo: {warnCount}]";
                                }
                            } catch (Exception exLog) {
                                log.Warn("Lỗi lưu nhật ký chuẩn hóa: " + exLog.Message);
                            }
                        }
                    }
                    // Đầu api CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV - #15
                    if (tableNames.Contains("CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangPhieuXetNghiemLaiHIVCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.phiu_xt_nghim_li_hiv_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangPhieuXetNghiemLaiHIVCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_PHIEU_XET_NGHIEM_LAI_HIV", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_KHACH_HANG_BANG_HOI_ACE - #16
                    if (tableNames.Contains("CH07_KHACH_HANG_BANG_HOI_ACE"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangBangHoiACECH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_BANG_HOI_ACE>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.bng_hi_ace_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangBangHoiACECH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_BANG_HOI_ACE
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_BANG_HOI_ACE", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }
                    // Đầu api CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC - #16
                    if (tableNames.Contains("CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC"))
                    {

                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiKhachHangAssistQstKienThucCH07Model>>(resultApiString);

                        var tongHops = new List<CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        var listDataFiltered = dataResultApi.Where(x => !String.IsNullOrEmpty(x.record_id) && x.thng_tin_c_bn_assist_qst_kin_thc_complete == 2).ToList();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiKhachHangAssistQstKienThucCH07Entity(listDataFiltered, maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_KHACH_HANG_ASSIST_QST_KIEN_THUC", cityCode, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CH07_THONG_TIN_TRUYEN_THONG - #17
                    if (tableNames.Contains("CH07_THONG_TIN_TRUYEN_THONG"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiCH07ThongTinTruyenThongModel>>(resultApiString);

                        var tongHops = new List<CH07_THONG_TIN_TRUYEN_THONG>();
                        
                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        //_convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi.Where(x => !string.IsNullOrEmpty(x.makh) && x.chuyn_gi_dch_v_complete.Equals("Complete")).ToList(), maDuAn, ref tongHops);
                        _convertResultApiToEntity.ConvertApiCH07TTTTToEntity(dataResultApi.Where(x => x.ch_07_thng_tin_truyn_thng_complete.Equals("Complete")).ToList(), maDuAn, apiCode, cityCode, ref tongHops);

                        // Thêm dữ liệu bảng CH07_THONG_TIN_TRUYEN_THONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "CH07_THONG_TIN_TRUYEN_THONG", tongHops.FirstOrDefault().city_code, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    // Đầu api CD43_BIEN_BAN_GIAO_NHAN_VAT_PHAM - #18
                    if (tableNames.Contains("PhieuXuatNhap"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiBBGNVatPhamModel>>(resultApiString);

                        var tongHops = new List<PhieuXuatNhap>();
                        var chiTietXuatNhaps = new List<ChiTietPhieuXuatNhap>();

                        //Lay ma Code Tinh theo API CODE: API_VHNO_02/API_HNO_02
                        var cityCode = "";
                        string cityCodeTemp = apiCode.Split('_')[1];
                        if (!string.IsNullOrEmpty(cityCodeTemp))
                        {
                            cityCode = cityCodeTemp.Length == 3 ? cityCodeTemp.Substring(0, 3) : cityCodeTemp.Substring(1, 3);
                        }

                        // Chuyển đổi dữ liệu sang các bảng tương ứng                        
                        _convertResultApiToEntity.ConvertApiBBGNVatPhamCD43Entity(dataResultApi.Where(x => x.cd_43_bin_bn_giao_nhn_vt_phm_complete.Equals("2")).ToList(), maDuAn, apiCode, cityCode, ref tongHops, ref chiTietXuatNhaps);

                        // Thêm dữ liệu bảng CH07_THONG_TIN_TRUYEN_THONG
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "PhieuXuatNhap", tongHops.FirstOrDefault().city_code, maDuAn);

                            var dattableInsertChiTiet = insertDataDA.ConvertToDataTable(chiTietXuatNhaps);

                            resultChiTiet = insertDataDA.InsertDataFromApi(dattableInsertChiTiet, "ChiTietPhieuXuatNhap", chiTietXuatNhaps.FirstOrDefault().city_code, maDuAn);
                        }
                        else
                        {
                            result.Message = "Không có dữ liệu!";
                            result.Success = false;
                        }
                    }

                    #endregion


                }

            }
            catch (Exception ex)
            {
                log.Error("Lỗi đồng bộ dữ liệu api: "+ apiCode +" (report_id = "+reportId+"): "+ex.Message);
            }
            
            // Cập nhật thời gian kết thúc đồng bộ
            insertDataDA.UpdateTimeSync(apiCode, false, result.Message);
            return result;
        }
    }
}
