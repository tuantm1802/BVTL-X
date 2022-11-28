using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.API
{
    public interface IInsertDataDA
    {
        /// <summary>
        /// Thêm dữ liệu bảng SKTTTest
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
        BaseResult InsertDataFromApi(DataTable dattableInsert, string tableName, string cityCode, string maDuAn);

        /// <summary>
        /// Chuyển đổi list model to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        DataTable ConvertToDataTable<T>(IList<T> data);

        /// <summary>
        /// Lấy danh sách Api
        /// </summary>
        /// <returns></returns>
        List<BVTL_API> GetAllApi();

        /// <summary>
        /// Lấy danh sách table theo đầu api Api
        /// </summary>
        /// <returns></returns>
        List<BVTL_MASTER_TABLE> GetAllApi_Table();

        /// <summary>
        /// Lấy danh sách table trong db
        /// </summary>
        /// <returns></returns>
        List<string> GetAllTableName();

        /// <summary>
        /// Lấy thông tin của 1 api theo id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        BVTL_API GetApiInfo(int id, ref List<string> tableNames);

        /// <summary>
        /// Cập nhật job đồng bộ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ObjectMessage EditJobSync(BVTL_API model);

        /// <summary>
        /// Cập nhật thời gian bát đầu, kết thúc đồng bộ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ObjectMessage UpdateTimeSync(string apiCode, bool isStartTime, string message);
    }
}
