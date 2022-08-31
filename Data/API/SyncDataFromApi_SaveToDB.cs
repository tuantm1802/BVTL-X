using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.API;
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

        public async Task<BaseResult> GetDataFromApi_SaveToDB(string urlApi, string token, string reportId, List<string> tableNames)
        {
            var result = new BaseResult();
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
                    _convertResultApiToEntity.ConvertApi1344ToEntity(dataResultApi, ref sktts, ref assists);

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
                    _convertResultApiToEntity.ConvertApiHIVToEntity(dataResultApi, ref hivs);

                    // Thêm dữ liệu bảng BVTL_KQ_XN_HIV
                    if (hivs != null && hivs.Count > 0)
                    {
                        var dattableInsert = insertDataDA.ConvertToDataTable(hivs);

                        result = insertDataDA.InsertDataFromApi(dattableInsert, "BVTL_KQ_XN_HIV");
                    }
                }

                #endregion
            }
            return result;
        }
    }
}
