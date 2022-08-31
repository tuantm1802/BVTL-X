using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.ICommon
{
    public interface IExportExcel
    {
        Stream CreateExcelFile<T>(string linkFileTemplate, List<T> lsData, int rowStart, int columnStart);
    }
}
