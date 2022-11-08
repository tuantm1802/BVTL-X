using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.API
{
    public class InsertDataDA : IInsertDataDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _databaseSql = new DatabaseSql();

        /// <summary>
        /// Thêm dữ liệu bảng SKTTTest
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
        public BaseResult InsertDataFromApi(DataTable dattableInsert, string tableName, string cityCode)
        {

            BaseResult obj = new BaseResult();

            string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection conn = new SqlConnection(stringConnect);
            SqlTransaction transaction;

            if (conn.State == ConnectionState.Closed) conn.Open();
            transaction = conn.BeginTransaction();

            try
            {
                log.Info("!!!!!!!!!!!!!!!!!!!!!!Bắt đầu xóa dữ liệu bảng: " + tableName + "!!!!!!!!!!!!!!!!!!!!!!");
                // Xóa dữ liệu bảng
                var data = _databaseSql.ExecuteNonQueryTran("Delete from " + tableName + " where city_code = " + cityCode, conn, transaction);
                log.Info("############!!!!!!!!!!Kết thúc xóa dữ liệu bảng: " + tableName + "############!!!!!!!!!!");


                log.Info("*******************Bắt đầu insert dữ liệu bảng: " + tableName + "*********************");
                //Bulk insert into table SKTTTest
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkcopy.DestinationTableName = "dbo." + tableName;
                    bulkcopy.WriteToServer(dattableInsert);
                    bulkcopy.Close();
                }
                transaction.Commit();
                conn.Close();

                obj.Success = true;
                log.Info("Tổng số bản ghi thêm: " + dattableInsert.Rows.Count);
                log.Info("############*********Kết thúc insert dữ liệu bảng: " + tableName + "############**********");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                conn.Close();
                log.Error("Thêm dữ liệu bảng " + tableName + " lỗi: " + ex.Message);
                obj.Success = false;
                obj.Message = ex.Message;
            }
            finally
            {
                conn.Close();
            }

            return obj;
        }

        /// <summary>
        /// Chuyển đổi list model to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            var tableNames = GetAllTableName();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (!tableNames.Contains(prop.Name))
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    if (!tableNames.Contains(prop.Name))
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Lấy danh sách Api
        /// </summary>
        /// <returns></returns>
        public List<BVTL_API> GetAllApi()
        {
            var result = new List<BVTL_API>();
            try
            {
                result = _databaseSql.ExecuteCommanToList<BVTL_API>("select * from BVTL_API;").ToList();// where IsActive = 1
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "InsertDataDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách api lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BVTL_API>();
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách table theo đầu api Api
        /// </summary>
        /// <returns></returns>
        public List<BVTL_MASTER_TABLE> GetAllApi_Table()
        {
            var result = new List<BVTL_MASTER_TABLE>();
            try
            {
                result = _databaseSql.ExecuteCommanToList<BVTL_MASTER_TABLE>("select * from BVTL_MASTER_TABLE;").ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "InsertDataDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách table lưu dữ liệu theo đầu api lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BVTL_MASTER_TABLE>();
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách table trong db
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTableName()
        {
            var result = new List<string>();
            try
            {
                result = _databaseSql.ExecuteCommanToList<TableNameModel>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';").ToList().Select(x => x.TABLE_NAME).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "InsertDataDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách table lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<string>();
            }

            return result;
        }



        /// <summary>
        /// Lấy thông tin của 1 api theo id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public BVTL_API GetApiInfo(int id, ref List<string> tableNames)
        {
            var result = new BVTL_API();
            try
            {
                result = db.BVTL_API.FirstOrDefault(x => x.Api_Id == id);
                if (!string.IsNullOrEmpty(result.TableNameSaveData))
                    tableNames.Add(result.TableNameSaveData);
                else
                {
                    var tables = db.BVTL_MASTER_TABLE.Where(x => x.Api_Id == id).ToList();
                    if (tables != null && tables.Count > 0)
                        tableNames = tables.Select(x => x.table_name).ToList();
                }
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "InsertDataDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy thông tin 1 đầu api lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new BVTL_API();
            }

            return result;
        }

        /// <summary>
        /// Cập nhật job đồng bộ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ObjectMessage EditJobSync(BVTL_API model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_API.FirstOrDefault(x => x.Api_Code == model.Api_Code);
                data.IsActive = model.IsActive;
                data.TimeReCall = model.TimeReCall;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";

                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// Cập nhật thời gian bát đầu, kết thúc đồng bộ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ObjectMessage UpdateTimeSync(string apiCode, bool isStartTime)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_API.FirstOrDefault(x => x.Api_Code == apiCode);
                if (isStartTime)
                    data.Start_Time_Sync = DateTime.Now;
                else
                    data.End_Time_Syc = DateTime.Now;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";

                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

    }
}
