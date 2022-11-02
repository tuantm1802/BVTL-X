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
            insertDataDA.UpdateTimeSync(apiCode, true);
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
                        _convertResultApiToEntity.ConvertApi1344ToEntity(dataResultApi, maDuAn, ref sktts, ref assists);

                        // Thêm dữ liệu bảng BVTL_KQ_SL_SKTT
                        if (sktts != null && sktts.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(sktts);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_SKTT");
                        }

                        // Thêm dữ liệu bảng BVTL_KQ_SL_SKTT
                        if (assists != null && assists.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(assists);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_ASSIST");
                        }
                    }

                    // Đầu api HIV
                    if (tableNames.Contains("BVTL_KQ_XN_HIV"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiHIVModel>>(resultApiString);

                        var hivs = new List<BVTL_KQ_XN_HIV>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiHIVToEntity(dataResultApi, maDuAn, ref hivs);

                        // Thêm dữ liệu bảng BVTL_KQ_XN_HIV
                        if (hivs != null && hivs.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(hivs);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_XN_HIV");
                        }
                    }

                    // Đầu api ACE
                    if (tableNames.Contains("BVTL_KQ_SL_ACE"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiACEModel>>(resultApiString);

                        var aces = new List<BVTL_KQ_SL_ACE>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiACEToEntity(dataResultApi, maDuAn, ref aces);

                        // Thêm dữ liệu bảng BVTL_KQ_SL_ACE
                        if (aces != null && aces.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(aces);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_SL_ACE");
                        }
                    }

                    // Đầu api tổng hợp
                    if (tableNames.Contains("BVTL_BO_BIEU_MAU_KH_BAO_CAO"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiTongHopModel>>(resultApiString);

                        var tongHops = new List<BVTL_BO_BIEU_MAU_KH_BAO_CAO>();
                        var khachHangs = new List<BVTL_KHACH_HANG>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiTongHopToEntity(dataResultApi, maDuAn, ref tongHops, ref khachHangs);

                        // Thêm dữ liệu bảng BVTL_KHACH_HANG
                        if (khachHangs != null && khachHangs.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(khachHangs);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KHACH_HANG");
                        }

                        // Thêm dữ liệu bảng BVTL_BO_BIEU_MAU_KH_BAO_CAO
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_BO_BIEU_MAU_KH_BAO_CAO");
                        }
                    }

                    // Đầu api phiếu tư vấn
                    if (tableNames.Contains("BVTL_PHIEU_TU_VAN"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiPhieuTuVanModel>>(resultApiString);

                        var tongHops = new List<BVTL_PHIEU_TU_VAN>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiPhieuTuVanToEntity(dataResultApi, maDuAn, ref tongHops);

                        // Thêm dữ liệu bảng BVTL_PHIEU_TU_VAN
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_PHIEU_TU_VAN");
                        }
                    }

                    // Đầu api chuyển gửi dịch vụ
                    if (tableNames.Contains("BVTL_CHUYEN_GUI_DICH_VU"))
                    {
                        var dataResultApi = JsonConvert.DeserializeObject<List<ResultApiChuyenGuiDVModel>>(resultApiString);

                        var tongHops = new List<BVTL_CHUYEN_GUI_DICH_VU>();

                        // Chuyển đổi dữ liệu sang các bảng tương ứng
                        _convertResultApiToEntity.ConvertApiChuyenGuiDichVuToEntity(dataResultApi, maDuAn, ref tongHops);

                        // Thêm dữ liệu bảng BVTL_CHUYEN_GUI_DICH_VU
                        if (tongHops != null && tongHops.Count > 0)
                        {
                            var dattableInsert = insertDataDA.ConvertToDataTable(tongHops);

                            result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_CHUYEN_GUI_DICH_VU");
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
            insertDataDA.UpdateTimeSync(apiCode, false);
            return result;
        }
    }
}
