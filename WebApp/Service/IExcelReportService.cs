using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System.Collections.Generic;

namespace WebApp.Service
{
    public interface IExcelReportService
    {
        byte[] ExportReport(
            List<BaoCaoModel> data, 
            string titleReport, 
            string sheetName, 
            UserLogin user, 
            string tenNhomTBHs, 
            string baseFileName,
            out string fileName
        );
    }
}
