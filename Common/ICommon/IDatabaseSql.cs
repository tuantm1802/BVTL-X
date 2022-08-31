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

namespace Common.ICommon
{
    public interface IDatabaseSql
    {
        #region Create connection
        /// <summary>
        /// Lấy kết nối đến DB
        /// </summary>
        /// <param name="connectStrDB">
        /// lấy trong file Common.Constants
        /// </param>
        /// <returns></returns>
        SqlConnection GetConnect(string connectStrDB);
        #endregion

        #region Execute Table for query
        DataTable ExecuteTable(string sql, string connectStrDB);
        #endregion

        #region Execute Non Query for query
        int ExecuteNonQuery(string sql, string connectStrDB);
        #endregion

        #region Execute Scalar for query
        int ExecuteScalar(string sql, string connectStrDB);
        IList<T> ExecuteCommanToList<T>(string sql, string connectStrDB);
        #endregion

        #region Execute Table for Store Procedure
        DataTable ExecuteProcTable(string procName, List<SqlParameter> lstParam, string connectStrDB);
        #endregion

        #region Execute DataSet for Store Procedure
        DataSet ExecuteProcDataSet(string procName, List<SqlParameter> lstParam, string connectStrDB);
        int ExecuteNonQueryTran(string sql, SqlConnection con, SqlTransaction sqlTrans);
        #endregion

        #region Parse Datatable To Dictionary
        List<Dictionary<string, object>> ParseTableToDictionary(DataTable dt);

        #endregion

        #region Execute Non Query for Store Procedure
        int ExecuteProcNonQuery(string procName, List<SqlParameter> lstParam, string connectStrDB);

        Task<int> ExecuteProcNonQueryAsync(string procName, List<SqlParameter> lstParam, string connectStrDB);

        int ExecuteProcNonQueryTran(string procName, List<SqlParameter> lstParam, SqlConnection connect, SqlTransaction tran);
        #endregion

        #region Execute Store Procedure And Convert Result to List<T>

        IList<T> ExecuteProcToList<T>(string procName, List<SqlParameter> lstParam, string connectStrDB);

        IList<T> ExecuteProcToList2<T>(string procName, List<SqlParameter> lstParam, string connectStrDB);

        #endregion

        #region Convert DataTable to Json
        string ConvertDataTabletoJson(DataTable dt);
        #endregion

        #region Convert DataTable to List<T>
        List<T> ConvertDataTableToList<T>(DataTable dt);

        //  Converts DataTable To List
        //  DataTable dtTable = GetEmployeeDataTable();
        //  List<Employee> employeeList = dtTable.DataTableToList<Employee>();

        IList<T> DataTableToList<T>(DataTable table);

        #endregion

        #region Convert To List<T>
        IList<T> ConvertToList<T>(DataTable dt);
        #endregion
        #region Execute proc table and convert to Json
        string ExecuteProcToJson(string procName, List<SqlParameter> lstParam, string connectStrDB);
        #endregion

        #region Convert class to SqlParameter
        SqlParameter CreateSqlParameter(object value, string name);

        List<SqlParameter> ConvertClassToSqlParameter<T>(T dt);


        List<SqlParameter> ConvertClassToSqlParameterNull<T>(T dt);

        DataTable ListToDataTable<T>(IList<T> data);
        #endregion
        #region Convert List to DataTable
        DataTable ToDataTable<T>(IList<T> data);

        List<T> DataReaderMapToList<T>(IDataReader dr);
        #endregion
    }
}
