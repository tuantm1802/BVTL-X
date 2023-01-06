using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Newtonsoft.Json;
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

        public async Task<BaseResult> GetDataFromApi_SaveToDB(string urlApi, string token, string reportId, string maDuAn, List<string> tableNames, string apiCode)
        {
            var result = new BaseResult();

            // Cập nhật thời gian bắt đầu đồng bộ
            insertDataDA.UpdateTimeSync(apiCode, true, "");
            try
            {
                // Call api để lấy dữ liệu
                var resultApiString = await getDataFromAPI.PostDataFromApiReturnString(urlApi, token, reportId);
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
