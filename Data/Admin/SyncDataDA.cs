using Common;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
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
    public class SyncDataDA: ISyncDataDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<BVTL_API> GetAllByPage(ModelSearch modelSearch, ref int totalRow)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int skipRows = (modelSearch.currentPage - 1) * modelSearch.pageSize;
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.BVTL_API.Where(x => x.IsActive == true).ToList();

                totalRow = queryResultPage.Count();
                queryResultPage = queryResultPage.Skip(skipRows) .Take(modelSearch.pageSize).ToList();
                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.BVTL_API.Where(x => x.IsActive == true && x.NameSyncdata.Contains(modelSearch.KeyWord)).ToList();

                totalRow = queryResultPage.Count();
                queryResultPage = queryResultPage.Skip(skipRows) .Take(modelSearch.pageSize).ToList();
                return queryResultPage.ToList();
            }
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
                log.Error("InsertDataDA - Thêm "+ tableName + ": " + ex.Message);
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
