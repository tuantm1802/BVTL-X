using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using Data.Admin;
using Data.InterfaceDA;
using Model.ModelExtend.API.CD45;

namespace WebApp.Controllers
{
    public class DataQualityController : BaseController
    {
        private readonly IDataQualityDA _dataQualityDA = new DataQualityDA();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLogs(string maDuAn, string apiCode, string severity, string keyword, bool? isResolved, int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                int totalRows = 0;
                var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, severity, keyword, isResolved, pageIndex, pageSize, out totalRows);
                return Json(new { Success = true, Data = logs, TotalRows = totalRows, PageIndex = pageIndex, PageSize = pageSize });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetStats(string maDuAn)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                var stats = _dataQualityDA.GetStats(maDuAn);
                return Json(new { Success = true, Stats = stats });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ResolveLog(long id, string note)
        {
            try
            {
                bool ok = _dataQualityDA.MarkResolved(id, note);
                return Json(new { Success = ok });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportExcelWarnings(string maDuAn, string apiCode)
        {
            try
            {
                if (string.IsNullOrEmpty(maDuAn)) maDuAn = "CD45";
                int totalRows = 0;
                var logs = _dataQualityDA.GetLogs(maDuAn, apiCode, "WARNING", null, false, 1, 5000, out totalRows);

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("CanhBaoDuLieu");
                    ws.Cell("A1").Value = "DANH SÁCH CẢNH BÁO BẤT THƯỜNG DỮ LIỆU NGUỒN REDCAP (DỰ ÁN CD45)";
                    ws.Range("A1:H1").Merge().Style.Font.Bold = true;
                    ws.Range("A1:H1").Style.Font.FontSize = 14;
                    ws.Range("A1:H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell("A2").Value = $"Thời gian xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Tổng số cảnh báo chưa xử lý: {logs.Count}";
                    ws.Range("A2:H2").Merge().Style.Font.Italic = true;

                    string[] headers = { "#", "Thời gian", "API / Form", "Mã KH (record_id)", "Trường lỗi", "Giá trị từ REDCap", "Mã quy tắc", "Nội dung cảnh báo" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        ws.Cell(4, c + 1).Value = headers[c];
                        ws.Cell(4, c + 1).Style.Font.Bold = true;
                        ws.Cell(4, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4E73DF");
                        ws.Cell(4, c + 1).Style.Font.FontColor = XLColor.White;
                    }

                    int row = 5;
                    for (int i = 0; i < logs.Count; i++)
                    {
                        var item = logs[i];
                        ws.Cell(row, 1).Value = i + 1;
                        ws.Cell(row, 2).Value = item.CREATED_DATE.ToString("dd/MM/yyyy HH:mm");
                        ws.Cell(row, 3).Value = item.API_CODE;
                        ws.Cell(row, 4).Value = item.RECORD_ID;
                        ws.Cell(row, 5).Value = item.FIELD_NAME;
                        ws.Cell(row, 6).Value = item.OLD_VALUE;
                        ws.Cell(row, 7).Value = item.RULE_CODE;
                        ws.Cell(row, 8).Value = item.MESSAGE;
                        row++;
                    }

                    ws.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileName = $"CanhBao_REDCap_CD45_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi xuất file cảnh báo: " + ex.Message);
            }
        }
    }
}
