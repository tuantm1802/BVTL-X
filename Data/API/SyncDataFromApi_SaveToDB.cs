using Common;
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
    public class SyncDataFromApi_SaveToDB
    {
        InsertDataDA insertDataDA = new InsertDataDA();
        GetDataFromAPI getDataFromAPI = new GetDataFromAPI();

        public async Task<BaseResult> GetDataFromApi_SaveToDB(string urlApi, string token, string reportId, List<string> tableNames)
        {
            var result = new BaseResult();
            // Call api để lấy dữ liệu
            var resultApiString = await getDataFromAPI.PostDataFromApiReturnString(urlApi, token, reportId);
            if (!string.IsNullOrEmpty(resultApiString))
            {

                #region Lưu dữ liệu từ api vào db
                // Đầu api 1344
                if (tableNames.Contains("BVTL_KQ_SL_ASSIST"))
                {
                    var dataResultApi = JsonConvert.DeserializeObject<List<ResultApi1344Model>>(resultApiString);

                    var sktts = new List<BVTL_KQ_SL_SKTT>();
                    var assists = new List<BVTL_KQ_SL_ASSIST>();

                    // Chuyển đổi dữ liệu sang các bảng tương ứng
                    ConvertResultApiToEntity.ConvertApi1344ToEntity(dataResultApi, ref sktts, ref assists);

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

                #endregion
            }
            return result;
        }
    }
}
