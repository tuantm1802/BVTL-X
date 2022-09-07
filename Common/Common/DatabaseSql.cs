using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Common.ICommon;

namespace Common.Common
{
    public class DatabaseSql: IDatabaseSql
    {
        private  readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string stringConnect = ConfigurationManager.AppSettings["ConnectionString"];
        #region Create connection
        /// <summary>
        /// Lấy kết nối đến DB
        /// </summary>
        /// <param name="connectStrDB">
        /// lấy trong file Common.Constants
        /// </param>
        /// <returns></returns>
        public  SqlConnection GetConnect()
        {
            SqlConnection con = new SqlConnection(stringConnect);
            if (con.State == ConnectionState.Closed) con.Open();
            return con;
        }
        #endregion

        #region Execute Table for query
        public  DataTable ExecuteTable(string sql)
        {
            var con = GetConnect();
            SqlCommand cmd = new SqlCommand(sql, con)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 360
            };
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        #endregion

        #region Execute Non Query for query
        public  int ExecuteNonQuery(string sql)
        {
            var con = GetConnect();
            SqlCommand cmd = new SqlCommand(sql, con)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 360
            };
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        #endregion

        #region Execute Scalar for query
        public  int ExecuteScalar(string sql)
        {
            var con = GetConnect();
            SqlCommand cmd = new SqlCommand(sql, con)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 360
            };
            try
            {
                return (Int32)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        public  IList<T> ExecuteCommanToList<T>(string sql)
        {
            var dt = new DataTable();
            serializer.MaxJsonLength = Int32.MaxValue;
            IList<T> objectsList = new List<T>();
            try
            {
                //Execute procedure and get table result
                dt = ExecuteTable(sql);

                if (dt.Rows.Count == 0) return objectsList;
                //objectsList = ConvertToList<T>(dt);
                //Covert datatable to json string
                string jsonString = serializer.Serialize(ParseTableToDictionary(dt));

                //Convert jsonString to List<T>
                InvalidJsonElements = null;
                var array = JArray.Parse(jsonString);

                foreach (var item in array)
                {
                    try
                    {
                        //Map single json in array to object<T>.
                        var itemMapped = item.ToObject<T>();

                        objectsList.Add(itemMapped);
                    }
                    catch (Exception ex)
                    {
                        InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                        InvalidJsonElements.Add(item.ToString());
                    }
                }
                return objectsList;
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Execute Table for Store Procedure
        public  DataTable ExecuteProcTable(string procName, List<SqlParameter> lstParam)
        {
            var con = GetConnect();
            var dt = new DataTable();
            try
            {
                using (var cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    if (lstParam.Count > 0)
                        foreach (var param in lstParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                    var dtAdapter = new SqlDataAdapter(cmd);
                    dtAdapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return new DataTable();
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        #endregion

        #region Execute DataSet for Store Procedure
        public  DataSet ExecuteProcDataSet(string procName, List<SqlParameter> lstParam)
        {
            var con = GetConnect();
            var ds = new DataSet();
            try
            {
                using (var cmd = new SqlCommand(procName, con))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    if (lstParam.Count > 0)
                        foreach (var param in lstParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                    var dtAdapter = new SqlDataAdapter(cmd);
                    dtAdapter.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        public  int ExecuteNonQueryTran(string sql, SqlConnection con, SqlTransaction sqlTrans)
        {

            SqlCommand cmd = new SqlCommand(sql, con, sqlTrans)
            {
                CommandType = CommandType.Text
            };
            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
        }
        #endregion

        #region Parse Datatable To Dictionary
        public  List<Dictionary<string, object>> ParseTableToDictionary(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        #endregion

        #region Execute Non Query for Store Procedure
        public  int ExecuteProcNonQuery(string procName, List<SqlParameter> lstParam)
        {
            var con = GetConnect();
            try
            {
                using (var cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    if (lstParam.Count > 0)
                        foreach (var param in lstParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public async Task<int> ExecuteProcNonQueryAsync(string procName, List<SqlParameter> lstParam)
        {
            var con = GetConnect();
            try
            {
                using (var cmd = new SqlCommand(procName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    if (lstParam.Count > 0)
                        foreach (var param in lstParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        public  int ExecuteProcNonQueryTran(string procName, List<SqlParameter> lstParam, SqlConnection connect, SqlTransaction tran)
        {
            try
            {
                using (var cmd = new SqlCommand(procName, connect, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    if (lstParam.Count > 0)
                        foreach (var param in lstParam)
                        {
                            cmd.Parameters.Add(param);
                        }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return 0;
            }
        }
        #endregion

        #region Execute Store Procedure And Convert Result to List<T>

        public  List<string> InvalidJsonElements;
         JavaScriptSerializer serializer = new JavaScriptSerializer();

        public  IList<T> ExecuteProcToList<T>(string procName, List<SqlParameter> lstParam)
        {
            var dt = new DataTable();
            serializer.MaxJsonLength = Int32.MaxValue;
            IList<T> objectsList = new List<T>();
            try
            {
                //Execute procedure and get table result
                dt = ExecuteProcTable(procName, lstParam);

                if (dt.Rows.Count == 0) return objectsList;
                //objectsList = ConvertToList<T>(dt);
                //Covert datatable to json string
                string jsonString = serializer.Serialize(ParseTableToDictionary(dt));

                //Convert jsonString to List<T>
                InvalidJsonElements = null;
                var array = JArray.Parse(jsonString);

                foreach (var item in array)
                {
                    try
                    {
                        //Map single json in array to object<T>.
                        var itemMapped = item.ToObject<T>();

                        objectsList.Add(itemMapped);
                    }
                    catch (Exception ex)
                    {
                        InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                        InvalidJsonElements.Add(item.ToString());
                    }
                }
                return objectsList;
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }
        }

        public  IList<T> ExecuteProcToList2<T>(string procName, List<SqlParameter> lstParam)
        {
            var dt = new DataTable();
            serializer.MaxJsonLength = Int32.MaxValue;
            IList<T> objectsList = new List<T>();
            try
            {
                //Execute procedure and get table result
                dt = ExecuteProcTable(procName, lstParam);

                if (dt.Rows.Count == 0) return objectsList;
                objectsList = DataTableToList<T>(dt);

                return objectsList;
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Convert DataTable to Json
        public  string ConvertDataTabletoJson(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
        #endregion

        #region Convert DataTable to List<T>
        public  List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            var objectsList = new List<T>();

            if (dt.Rows.Count == 0) return objectsList;
            //objectsList = ConvertToList<T>(dt);
            //Covert datatable to json string
            string jsonString = serializer.Serialize(ParseTableToDictionary(dt));

            //Convert jsonString to List<T>
            InvalidJsonElements = null;
            var array = JArray.Parse(jsonString);

            foreach (var item in array)
            {
                try
                {
                    //Map single json in array to object<T>.
                    var itemMapped = item.ToObject<T>();

                    objectsList.Add(itemMapped);
                }
                catch (Exception ex)
                {
                    InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                    InvalidJsonElements.Add(item.ToString());
                }
            }

            return objectsList;
        }

        //  Converts DataTable To List
        //  DataTable dtTable = GetEmployeeDataTable();
        //  List<Employee> employeeList = dtTable.DataTableToList<Employee>();

        public  IList<T> DataTableToList<T>(DataTable table)
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = Activator.CreateInstance<T>();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            var value = row[prop.Name];
                            Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, safeValue, null);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Convert To List<T>
        public  IList<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var property in properties)
                {
                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    if (columnNames.Contains(property.Name))
                    {
                        object safeValue = (row[property.Name] == null) ? null : Convert.ChangeType(row[property.Name], t);
                        if (safeValue == null)
                        {
                            continue;
                        }
                        property.SetValue(objT, safeValue);
                    }
                }
                return objT;
            }).ToList();
        }
        #endregion
        #region Execute proc table and convert to Json
        public  string ExecuteProcToJson(string procName, List<SqlParameter> lstParam)
        {
            try
            {
                DataTable dt = ExecuteProcTable(procName, lstParam);
                var json = ConvertDataTabletoJson(dt);
                return json;
            }
            catch (Exception ex)
            {
                log.Error("ERROR: " + ex.Message);
                return null;
            }

        }
        #endregion

        #region Convert class to SqlParameter
        public  SqlParameter CreateSqlParameter(object value, string name)
        {
            var param = new SqlParameter
            {
                ParameterName = @"" + name + "",
                Value = value
            };
            return param;
        }

        public  List<SqlParameter> ConvertClassToSqlParameter<T>(T dt)
        {
            var listParam = new List<SqlParameter>();
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo pro in properties)
                {
                    Object value = pro.GetValue(dt, null);
                    if (value != null)
                    {
                        listParam.Add(new SqlParameter(@"" + pro.Name + "", value));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Convert T to List<SqlParameter> error: " + ex.Message);
            }

            return listParam;
        }


        public  List<SqlParameter> ConvertClassToSqlParameterNull<T>(T dt)
        {
            var listParam = new List<SqlParameter>();
            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (var pro in properties)
                {
                    Object value = pro.GetValue(dt, null);
                    if (value != null)
                        listParam.Add(new SqlParameter(@"" + pro.Name + "", value));
                    else
                        listParam.Add(new SqlParameter(@"" + pro.Name + "", DBNull.Value));
                }
            }
            catch (Exception ex)
            {
                log.Error("Convert T to List<SqlParameter> error: " + ex.Message);
            }

            return listParam;
        }

        public  DataTable ListToDataTable<T>(IList<T> data)
        {
            DataTable table = new DataTable();

            //special handling for value types and string
            if (typeof(T).IsValueType || typeof(T).Equals(typeof(string)))
            {

                DataColumn dc = new DataColumn("Value", typeof(T));
                table.Columns.Add(dc);
                foreach (T item in data)
                {
                    DataRow dr = table.NewRow();
                    dr[0] = item;
                    table.Rows.Add(dr);
                }
            }
            else
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                foreach (T item in data)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                    {
                        try
                        {
                            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        }
                        catch (Exception ex)
                        {
                            row[prop.Name] = DBNull.Value;
                        }
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }
        #endregion
        #region Convert List to DataTable
        public  DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public  List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
        #endregion
    }
}
