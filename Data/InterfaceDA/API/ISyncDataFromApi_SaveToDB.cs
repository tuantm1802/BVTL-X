using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.API
{
    public interface ISyncDataFromApi_SaveToDB
    {
        Task<BaseResult> GetDataFromApi_SaveToDB(string urlApi, string token, string reportId, List<string> tableNames);
    }
}
