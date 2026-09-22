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
                log.Info("!!!!!!!!!!!!!!!!!!!!!!Bắt đầu đồng bộ dữ liệu bảng: " + tableName + " | CITY_CODE:" + cityCode + " | MADUAN:" + maDuAn);

                // Validate table name để tránh SQL Injection
                if (!System.Text.RegularExpressions.Regex.IsMatch(tableName, @"^[a-zA-Z0-9_]+$"))
                {
                    throw new ArgumentException("Invalid table name: " + tableName);
                }

                // Lấy tên cột IDENTITY của bảng (nếu có)
                string identityColumn = GetIdentityColumn(tableName, conn, transaction);
                log.Info("Identity column của bảng " + tableName + ": " + (identityColumn ?? "(không có)"));

                // Loại bỏ cột IDENTITY khỏi DataTable trước khi bulk copy
                // để tránh lỗi: Cannot insert explicit value for identity column when IDENTITY_INSERT is OFF
                DataTable dttInsert = dattableInsert.Copy();
                if (!string.IsNullOrEmpty(identityColumn) && dttInsert.Columns.Contains(identityColumn))
                {
                    dttInsert.Columns.Remove(identityColumn);
                    log.Info("Đã loại bỏ cột identity '" + identityColumn + "' khỏi DataTable trước khi bulk copy.");
                }

                // Xác định merge key & partition columns cho MERGE deduplication
                string mergeOnClause;
                string partitionCols;
                string pkColumn = GetPrimaryKeyColumn(tableName);

                if (tableName.Equals("BVTL_DATA_STANDARDIZATION_LOG", StringComparison.OrdinalIgnoreCase))
                {
                    // MERGE thông minh cho bảng Log (deduplicate theo MADUAN, TABLE_NAME, RECORD_ID, FIELD_NAME, RULE_CODE)
                    mergeOnClause = @"Target.[MADUAN] = Source.[MADUAN] 
                                  AND Target.[TABLE_NAME] = Source.[TABLE_NAME] 
                                  AND ISNULL(Target.[RECORD_ID], '') = ISNULL(Source.[RECORD_ID], '') 
                                  AND Target.[FIELD_NAME] = Source.[FIELD_NAME] 
                                  AND Target.[RULE_CODE] = Source.[RULE_CODE]";
                    partitionCols = "[MADUAN], [TABLE_NAME], [RECORD_ID], [FIELD_NAME], [RULE_CODE]";
                    log.Info("Sử dụng composite rule key làm merge key cho bảng log: " + tableName);
                }
                else if (dttInsert.Columns.Contains("record_id") && dttInsert.Columns.Contains("repeat_instance"))
                {
                    string recCol = dttInsert.Columns["record_id"].ColumnName;
                    string repCol = dttInsert.Columns["repeat_instance"].ColumnName;
                    mergeOnClause = $"Target.[{recCol}] = Source.[{recCol}] AND ISNULL(Target.[{repCol}], 1) = ISNULL(Source.[{repCol}], 1)";
                    partitionCols = $"[{recCol}], [{repCol}]";
                    log.Info($"Sử dụng composite key ({recCol} + {repCol}) làm merge key cho bảng: " + tableName);
                }
                else if (dttInsert.Columns.Contains("record_id"))
                {
                    string recCol = dttInsert.Columns["record_id"].ColumnName;
                    mergeOnClause = $"Target.[{recCol}] = Source.[{recCol}]";
                    partitionCols = $"[{recCol}]";
                    log.Info("Sử dụng " + recCol + " làm merge key cho bảng: " + tableName);
                }
                else if (tableName.Equals("ChiTietPhieuXuatNhap", StringComparison.OrdinalIgnoreCase))
                {
                    // Composite key cho ChiTietPhieuXuatNhap
                    mergeOnClause = "Target.[MaPhieu] = Source.[MaPhieu] AND Target.[MaSanPham] = Source.[MaSanPham]";
                    partitionCols = "[MaPhieu], [MaSanPham]";
                    log.Info("Sử dụng composite key (MaPhieu + MaSanPham) làm merge key cho bảng: " + tableName);
                }
                else if (!string.IsNullOrEmpty(pkColumn) && !pkColumn.Equals(identityColumn, StringComparison.OrdinalIgnoreCase))
                {
                    // PK không phải là identity column
                    mergeOnClause = $"Target.[{pkColumn}] = Source.[{pkColumn}]";
                    partitionCols = $"[{pkColumn}]";
                    log.Info("Sử dụng PK non-identity '" + pkColumn + "' làm merge key cho bảng: " + tableName);
                }
                else
                {
                    // Fallback: không merge được, chỉ insert (xóa rồi chèn lại theo city_code + maduan)
                    mergeOnClause = "1 = 0"; // Không bao giờ MATCHED → toàn bộ là INSERT mới
                    partitionCols = null;
                    log.Warn("Không tìm được merge key thích hợp cho bảng: " + tableName + " – sẽ thực hiện xóa và chèn lại.");
                }

                // Tạo bảng tạm (SELECT TOP 0 sẽ kế thừa cấu trúc bảng gốc, kể cả identity)
                // Bảng tạm không có ràng buộc IDENTITY nên có thể bulk copy tự do
                string tempTableName = "#Temp_" + tableName;
                string createTempTableSql = $"SELECT TOP 0 * INTO [{tempTableName}] FROM [{tableName}]";
                using (SqlCommand cmd = new SqlCommand(createTempTableSql, conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }

                // Nếu bảng gốc có IDENTITY, bảng tạm cũng kế thừa cột đó nhưng không có IDENTITY constraint
                // → Vẫn cần loại bỏ cột identity khỏi dttInsert khi copy vào bảng tạm
                // vì dttInsert không có giá trị hợp lệ cho cột này (mặc định = 0)

                log.Info("*******************Bắt đầu insert dữ liệu bảng tạm: " + tempTableName + "*********************");

                bulkcopy.DestinationTableName = tempTableName;
                // Thêm column mapping rõ ràng để tránh mismatch cột
                foreach (DataColumn col in dttInsert.Columns)
                {
                    bulkcopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }
                bulkcopy.WriteToServer(dttInsert);
                bulkcopy.Close();

                // Lấy danh sách cột (đã loại identity)
                List<string> columns = new List<string>();
                foreach (DataColumn column in dttInsert.Columns)
                {
                    columns.Add(column.ColumnName);
                }

                string columnsList = string.Join(", ", columns.Select(c => $"[{c}]"));
                string sourceColumnsList = string.Join(", ", columns.Select(c => $"Source.[{c}]"));

                // Tạo UPDATE SET: loại bỏ các cột trong merge key và cột identity
                var mergeKeyColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(identityColumn)) mergeKeyColumns.Add(identityColumn);
                if (dttInsert.Columns.Contains("record_id")) mergeKeyColumns.Add("record_id");
                if (dttInsert.Columns.Contains("repeat_instance")) mergeKeyColumns.Add("repeat_instance");
                if (tableName.Equals("ChiTietPhieuXuatNhap", StringComparison.OrdinalIgnoreCase))
                {
                    mergeKeyColumns.Add("MaPhieu");
                    mergeKeyColumns.Add("MaSanPham");
                }
                if (!string.IsNullOrEmpty(pkColumn)) mergeKeyColumns.Add(pkColumn);

                // Khởi tạo danh sách các cột bị bỏ qua (không cập nhật) dựa theo tên bảng
                var ignoredUpdateColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (tableName.Equals("CD43_KHACH_HANG_THONG_TIN_CO_BAN", StringComparison.OrdinalIgnoreCase))
                {
                    ignoredUpdateColumns.Add("sng_lc_nc_tiu_complete");
                    ignoredUpdateColumns.Add("sng_lc_hiv_complete");
                    ignoredUpdateColumns.Add("thng_tin_c_bn_v_hnh_vi_nguy_c_assist_qst_ace_complete");
                    ignoredUpdateColumns.Add("chuyn_gi_complete");
                    ignoredUpdateColumns.Add("phiu_t_vn_complete");
                    ignoredUpdateColumns.Add("sinh_hot_nhm_complete");
                    ignoredUpdateColumns.Add("nh_gi_mc_hi_lng_ca_kh_complete");
                }

                // Bổ sung các cột ignored này vào tập hợp mergeKeyColumns
                // để chúng KHÔNG được sinh ra trong câu UPDATE SET
                foreach (var col in ignoredUpdateColumns)
                {
                    mergeKeyColumns.Add(col);
                }

                string updateSetClause = string.Join(", ",
                    columns.Where(c => !mergeKeyColumns.Contains(c))
                           .Select(c => $"Target.[{c}] = Source.[{c}]"));

                // Xây dựng nguồn dữ liệu cho MERGE (Deduplicate source bằng ROW_NUMBER để tránh lỗi:
                // "The MERGE statement attempted to UPDATE or DELETE the same row more than once")
                string sourceTableExpression;
                if (!string.IsNullOrEmpty(partitionCols))
                {
                    sourceTableExpression = $@"(
                        SELECT * FROM (
                            SELECT *, ROW_NUMBER() OVER (PARTITION BY {partitionCols} ORDER BY (SELECT NULL)) AS _rn
                            FROM [{tempTableName}]
                        ) AS _srcTemp WHERE _srcTemp._rn = 1
                    )";
                }
                else
                {
                    sourceTableExpression = $"[{tempTableName}]";
                }

                if (tableName.Equals("BVTL_DATA_STANDARDIZATION_LOG", StringComparison.OrdinalIgnoreCase))
                {
                    // 1. Thực hiện MERGE UPSERT cho bảng Log: cập nhật trạng thái mới nhất, tránh nhân bản lặp dòng
                    string mergeLogSql = $@"
                        MERGE INTO [{tableName}] AS Target
                        USING {sourceTableExpression} AS Source
                        ON ({mergeOnClause})
                        WHEN MATCHED THEN
                            UPDATE SET 
                                Target.[OLD_VALUE] = Source.[OLD_VALUE],
                                Target.[NEW_VALUE] = Source.[NEW_VALUE],
                                Target.[REPORT_ID] = Source.[REPORT_ID],
                                Target.[API_CODE] = Source.[API_CODE],
                                Target.[SEVERITY] = Source.[SEVERITY],
                                Target.[ACTION_TAKEN] = Source.[ACTION_TAKEN],
                                Target.[MESSAGE] = Source.[MESSAGE],
                                Target.[CREATED_DATE] = Source.[CREATED_DATE],
                                Target.[MA_NHOM] = Source.[MA_NHOM],
                                Target.[CITY_CODE] = Source.[CITY_CODE]
                        WHEN NOT MATCHED BY TARGET THEN
                            INSERT ({columnsList}) VALUES ({sourceColumnsList});";

                    using (SqlCommand cmd = new SqlCommand(mergeLogSql, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Tự động giải quyết (Auto-Resolve) các CẢNH BÁO cũ đã được sửa/hợp lệ trên REDCap
                    string autoResolveSql = $@"
                        UPDATE [{tableName}]
                        SET IS_RESOLVED = 1,
                            RESOLVED_NOTE = N'Tự động đóng: Lỗi dữ liệu nguồn đã được sửa đổi / xử lý trên REDCap.'
                        WHERE MADUAN = @maDuAn
                          AND API_CODE IN (SELECT DISTINCT API_CODE FROM [{tempTableName}])
                          AND (SEVERITY = 'WARNING' OR SEVERITY = 'ERROR')
                          AND (IS_RESOLVED IS NULL OR IS_RESOLVED = 0)
                          AND NOT EXISTS (
                              SELECT 1 FROM [{tempTableName}] t 
                              WHERE t.MADUAN = [{tableName}].MADUAN
                                AND t.TABLE_NAME = [{tableName}].TABLE_NAME
                                AND ISNULL(t.RECORD_ID, '') = ISNULL([{tableName}].RECORD_ID, '')
                                AND t.FIELD_NAME = [{tableName}].FIELD_NAME
                                AND t.RULE_CODE = [{tableName}].RULE_CODE
                          )";

                    using (SqlCommand cmd = new SqlCommand(autoResolveSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@maDuAn", SqlDbType.VarChar).Value = (object)maDuAn ?? DBNull.Value;
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Tạo câu lệnh MERGE
                    string mergeSql = $@"
                        MERGE INTO [{tableName}] AS Target
                        USING {sourceTableExpression} AS Source
                        ON ({mergeOnClause})";

                    if (!string.IsNullOrEmpty(updateSetClause))
                    {
                        mergeSql += $@"
                        WHEN MATCHED THEN
                            UPDATE SET {updateSetClause}";
                    }

                    mergeSql += $@"
                        WHEN NOT MATCHED BY TARGET THEN
                            INSERT ({columnsList}) VALUES ({sourceColumnsList})
                        WHEN NOT MATCHED BY SOURCE AND (NULLIF(@cityCode, '') IS NULL OR Target.CITY_CODE = @cityCode) AND Target.MADUAN = @maDuAn THEN
                            DELETE;";

                    using (SqlCommand cmd = new SqlCommand(mergeSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@cityCode", SqlDbType.VarChar).Value = (object)cityCode ?? DBNull.Value;
                        cmd.Parameters.Add("@maDuAn", SqlDbType.VarChar).Value = (object)maDuAn ?? DBNull.Value;
                        cmd.ExecuteNonQuery();
                    }
                }

                log.Info("############!!!!!!!!!!Kết thúc đồng bộ dữ liệu bảng: " + tableName + "############!!!!!!!!!!");

                transaction.Commit();
                conn.Close();

                obj.Success = true;
                obj.Message = DateTime.Now.ToString() + ": Tổng record đã thêm: " + dttInsert.Rows.Count + " | TABLE: " + tableName + " | CITY_CODE: " + cityCode + " | MADUAN:" + maDuAn;
                log.Info("############*********KẾT THÚC insert bảng: " + tableName + " | Tổng record đã thêm:" + dttInsert.Rows.Count + " | TABLE: " + tableName + " | CITY_CODE: " + cityCode + " | MADUAN:" + maDuAn);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                conn.Close();
                log.Error("Thêm dữ liệu bảng:" + tableName + " | CITY_CODE:" + cityCode + " | MADUAN:" + maDuAn + " | Chi tiết lỗi: " + ex.Message);
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
                result = _databaseSql.ExecuteCommanToList<BVTL_API>("select * from BVTL_API WHERE IsActive = 1 ORDER BY Api_Code DESC;").ToList();// where IsActive = 1
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
        public string GetPrimaryKeyColumn(string tableName)
        {
            string pkColumn = "";
            string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection conn = new SqlConnection(stringConnect))
            {
                conn.Open();
                string query = @"
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1
                    AND TABLE_NAME = @TableName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        pkColumn = result.ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(pkColumn))
            {
                pkColumn = "record_id";
            }
            return pkColumn;
        }

        /// <summary>
        /// Lấy tên cột IDENTITY của bảng (nếu có) từ sys.columns.
        /// Sử dụng connection + transaction hiện tại để tránh tạo connection mới.
        /// </summary>
        public string GetIdentityColumn(string tableName, SqlConnection conn, SqlTransaction transaction)
        {
            string identityColumn = "";
            string query = @"
                SELECT c.name
                FROM sys.columns c
                INNER JOIN sys.tables t ON c.object_id = t.object_id
                WHERE t.name = @TableName AND c.is_identity = 1";
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    identityColumn = result.ToString();
                }
            }
            return identityColumn;
        }

        /// <summary>
        /// Overload: Lấy tên cột IDENTITY dùng connection string (mở connection mới).
        /// Dùng khi không có connection sẵn.
        /// </summary>
        public string GetIdentityColumn(string tableName)
        {
            string identityColumn = "";
            string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection conn = new SqlConnection(stringConnect))
            {
                conn.Open();
                string query = @"
                    SELECT c.name
                    FROM sys.columns c
                    INNER JOIN sys.tables t ON c.object_id = t.object_id
                    WHERE t.name = @TableName AND c.is_identity = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        identityColumn = result.ToString();
                    }
                }
            }
            return identityColumn;
        }

    }
}
