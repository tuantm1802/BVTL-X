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
        public BaseResult InsertDataFromApi(DataTable dattableInsert, string tableName, string cityCode, string maDuAn)
        {

            BaseResult obj = new BaseResult();
            DataTable dataTable = new DataTable();

            string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection conn = new SqlConnection(stringConnect);
            SqlTransaction transaction;

            if (conn.State == ConnectionState.Closed) conn.Open();
            transaction = conn.BeginTransaction();
            SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);

            try
            {
                log.Info("!!!!!!!!!!!!!!!!!!!!!!Bắt đầu xóa dữ liệu bảng: " + tableName + " | CITY_CODE:" + cityCode + " | MADUAN:" + maDuAn);
                // Xóa dữ liệu bảng
                var data = _databaseSql.ExecuteNonQueryTran("DELETE FROM " + tableName + " WHERE CITY_CODE = '" + cityCode + "' AND MADUAN = '"  + maDuAn + "'", conn, transaction);
                log.Info("############!!!!!!!!!!Kết thúc xóa dữ liệu bảng: " + tableName + "############!!!!!!!!!!");


                log.Info("*******************Bắt đầu insert dữ liệu bảng: " + tableName + "*********************");
                //long record_id_max = db.BVTL_PHIEU_TU_VAN.Where(x => x != null).DefaultIfEmpty().Max(x => x == null ? 0 : x.record_id);

                //Bulk insert into table SKTTTest
                //using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                //{
                    bulkcopy.DestinationTableName = "dbo." + tableName;
                    bulkcopy.WriteToServer(dattableInsert);
                    bulkcopy.Close();
                //}
                transaction.Commit();
                conn.Close();

                obj.Success = true;
                obj.Message = "Tổng record đã thêm: " + dattableInsert.Rows.Count + " | TABLE: " + tableName + " | CITY_CODE: " + cityCode + " | MADUAN:" + maDuAn;
                log.Info("############*********KẾT THÚC insert bảng: " + tableName + " | Tổng record đã thêm:" + dattableInsert.Rows.Count + " | TABLE: " + tableName + " | CITY_CODE: " + cityCode + " | MADUAN:" + maDuAn);
            }

            catch (Exception ex)
            {
                transaction.Rollback();
                conn.Close();
                log.Error("Thêm dữ liệu bảng:" + tableName + " | CITY_CODE:"+ cityCode + " | MADUAN:" + maDuAn + " | Chi tiết lỗi: " + ex.Message);
                obj.Success = false;
                obj.Message = ex.Message;
                
                // loop through all inner exceptions to see if any relate to a constraint failure
                bool dataExceptionFound = false;
                Exception tmpException = ex;
                while (tmpException != null)
                {
                    if (tmpException is SqlException
                       && tmpException.Message.Contains("constraint"))
                    {
                        dataExceptionFound = true;
                        break;
                    }
                    tmpException = tmpException.InnerException;
                }

                if (dataExceptionFound)
                {
                    // call the helper method to document the errors and invalid data
                    string errorMessage = GetBulkCopyFailedData(
                       conn.ConnectionString,
                       bulkcopy.DestinationTableName,
                       dataTable.CreateDataReader());
                    throw new Exception(errorMessage, ex);
                    log.Error("Thêm dữ liệu bảng - ERROR:" + errorMessage);
                }
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
        public ObjectMessage UpdateTimeSync(string apiCode, bool isStartTime, string message)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_API.FirstOrDefault(x => x.Api_Code == apiCode);
                if (isStartTime) { 
                    data.Start_Time_Sync = DateTime.Now;
                    data.End_Time_Sync = null;
                    data.Message = message;
                }
                else
                {
                    data.End_Time_Sync = DateTime.Now;
                    data.Message = message;
                }
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
        /// Build an error message with the failed records and their related exceptions.
        /// </summary>
        /// <param name="connectionString">Connection string to the destination database</param>
        /// <param name="tableName">Table name into which the data will be bulk copied.</param>
        /// <param name="dataReader">DataReader to bulk copy</param>
        /// <returns>Error message with failed constraints and invalid data rows.</returns>
        public static string GetBulkCopyFailedData(
           string connectionString,
           string tableName,
           IDataReader dataReader)
        {
            StringBuilder errorMessage = new StringBuilder("Bulk copy failures:" + Environment.NewLine);
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            SqlBulkCopy bulkCopy = null;
            DataTable tmpDataTable = new DataTable();

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();
                bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.CheckConstraints, transaction);
                bulkCopy.DestinationTableName = tableName;

                // create a datatable with the layout of the data.
                DataTable dataSchema = dataReader.GetSchemaTable();
                foreach (DataRow row in dataSchema.Rows)
                {
                    tmpDataTable.Columns.Add(new DataColumn(
                       row["ColumnName"].ToString(),
                       (Type)row["DataType"]));
                }

                // create an object array to hold the data being transferred into tmpDataTable 
                //in the loop below.
                object[] values = new object[dataReader.FieldCount];

                // loop through the source data
                while (dataReader.Read())
                {
                    // clear the temp DataTable from which the single-record bulk copy will be done
                    tmpDataTable.Rows.Clear();

                    // get the data for the current source row
                    dataReader.GetValues(values);

                    // load the values into the temp DataTable
                    tmpDataTable.LoadDataRow(values, true);

                    // perform the bulk copy of the one row
                    try
                    {
                        bulkCopy.WriteToServer(tmpDataTable);
                    }
                    catch (Exception ex)
                    {
                        // an exception was raised with the bulk copy of the current row. 
                        // The row that caused the current exception is the only one in the temp 
                        // DataTable, so document it and add it to the error message.
                        DataRow faultyDataRow = tmpDataTable.Rows[0];
                        errorMessage.AppendFormat("Error: {0}{1}", ex.Message, Environment.NewLine);
                        errorMessage.AppendFormat("Row data: {0}", Environment.NewLine);
                        foreach (DataColumn column in tmpDataTable.Columns)
                        {
                            errorMessage.AppendFormat(
                               "\tColumn {0} - [{1}]{2}",
                               column.ColumnName,
                               faultyDataRow[column.ColumnName].ToString(),
                               Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                   "Unable to document SqlBulkCopy errors. See inner exceptions for details.",
                   ex);
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            return errorMessage.ToString();
        }

    }
}
