using Common;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SyncDataDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<Api_TableSaveData> GetAllByPage(ModelSearch modelSearch, ref int totalRow)
        {
            int skipRows = (modelSearch.currentPage - 1) * modelSearch.pageSize;
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.Api_TableSaveData.Where(x => x.IsActive == true).ToList();

                totalRow = queryResultPage.Count();
                queryResultPage = queryResultPage.Skip(skipRows) .Take(modelSearch.pageSize).ToList();
                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.Api_TableSaveData.Where(x => x.IsActive == true && x.NameSyncdata.Contains(modelSearch.KeyWord)).ToList();

                totalRow = queryResultPage.Count();
                queryResultPage = queryResultPage.Skip(skipRows) .Take(modelSearch.pageSize).ToList();
                return queryResultPage.ToList();
            }
        }

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Api_TableSaveData GetItemById(int Id)
        {
            return db.Api_TableSaveData.FirstOrDefault(x => x.Id == Id);
        }

        /// <summary>
        /// Thêm dữ liệu bảng SKTTTest
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
        public ObjectMessage InsertDataFromApi(DataTable dattableInsert, string tableName, string stringConnect)
        {
            ObjectMessage obj = new ObjectMessage();

            SqlConnection conn = new SqlConnection(stringConnect);
            SqlTransaction transaction;

            if (conn.State == ConnectionState.Closed) conn.Open();
            transaction = conn.BeginTransaction();

            try
            {
                //Bulk insert into table 
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkcopy.DestinationTableName = "dbo."+ tableName;
                    bulkcopy.WriteToServer(dattableInsert);
                    bulkcopy.Close();
                }
                transaction.Commit();
                conn.Close();

                obj.Error = false;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                conn.Close();
                log.Error("InsertDataDA - Thêm SKTTTest: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return obj;
        }

        /// <summary>
        ///Xóa dữ liệu
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
        public ObjectMessage DeleteData(string tableName)
        {
            ObjectMessage obj = new ObjectMessage();

            try
            {
                var sqlString = "DELETE FROM "+ tableName + "; ";
                var result = db.Database.ExecuteSqlCommand(sqlString);

                obj.Error = false;
            }
            catch (Exception ex)
            {
                log.Error("SyncDataDA - Xóa dữ liệu bảng "+tableName+" : " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
            }
            return obj;
        }


    }
}
