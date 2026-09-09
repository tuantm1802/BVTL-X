using Model.ModelExtend;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface IDashboardCD45DA
    {
        DashboardCD45FullDataModel GetDashboardData(string cityCode, string maNhom, string fromDate, string toDate);
    }
}
