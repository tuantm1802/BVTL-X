using ClosedXML.Excel;
using Data.Admin;
using Data.InterfaceDA;
using Data.InterfaceDA.Admin;
using log4net;
using Model.ModelExtend;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApp.Service;

namespace WebApp.Services
{
    public class ReportExportService : IReportExportService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IScheduledReportDA _scheduledReportDA;
        private readonly IBaoCaoCD45DA _baoCaoCD45DA;
        private readonly IBaoCaoTongHopDA _baoCaoTongHopDA;
        private readonly IExcelReportService _excelReportService;
        private readonly ICityDA _cityDA;
        private readonly IBVTL_NHOM_TBHDA _nhomTBHDA;

        public ReportExportService(
            IScheduledReportDA scheduledReportDA = null,
            IBaoCaoCD45DA baoCaoCD45DA = null,
            IBaoCaoTongHopDA baoCaoTongHopDA = null,
            IExcelReportService excelReportService = null,
            ICityDA cityDA = null,
            IBVTL_NHOM_TBHDA nhomTBHDA = null)
        {
            _scheduledReportDA = scheduledReportDA ?? new ScheduledReportDA();
            _baoCaoCD45DA = baoCaoCD45DA ?? new BaoCaoCD45DA();
            _baoCaoTongHopDA = baoCaoTongHopDA ?? new BaoCaoTongHopDA();
            _excelReportService = excelReportService ?? new ExcelReportService();
            _cityDA = cityDA ?? new CityDA();
            _nhomTBHDA = nhomTBHDA ?? new BVTL_NHOM_TBHDA();
        }

        private string GetStorageDirectory(int year, int month)
        {
            string baseDir;
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                baseDir = HttpContext.Current.Server.MapPath("~/App_Data/ExportedReports/");
            }
            else
            {
                baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "ExportedReports");
            }

            string targetDir = Path.Combine(baseDir, year.ToString(), month.ToString("D2"));
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            return targetDir;
        }

        public async Task<List<ReportExportResult>> ExecuteAllMonthlyReportsAsync(int year, int month, string triggerType = "AutoSchedule", string createdBy = "QuartzScheduler")
        {
            var results = new List<ReportExportResult>();
            var settings = _scheduledReportDA.GetSettings();
            var telegram = new TelegramNotifier(chatId: settings.TelegramChatId);

            log.Info($"[PeriodicReportExportJob] Bắt đầu xử lý xuất báo cáo tự động tháng {month}/{year} ({triggerType})...");

            // 1. Pre-flight Check: Kiểm tra dữ liệu tháng
            bool hasCd45Data = _scheduledReportDA.CheckDataAvailability("TCV_CD45", year, month);
            bool hasBvtlData = _scheduledReportDA.CheckDataAvailability("TONGHOP_BVTL", year, month);

            if (!hasCd45Data && !hasBvtlData)
            {
                string warnMsg = $"⚠️ [HỆ THỐNG BVTL] Tạm hoãn xuất tự động báo cáo tháng {month:D2}/{year}: Chưa phát hiện dữ liệu nhập từ các tỉnh trong CSDL. Hệ thống sẽ giữ nguyên và thử lại vào chu kỳ tiếp theo.";
                log.Warn(warnMsg);
                await telegram.SendMessageAsync(warnMsg);

                // Ghi nhận log trạng thái tạm hoãn
                _scheduledReportDA.AddExportLog(new ExportedReportLogModel
                {
                    ReportType = "ALL",
                    ReportName = $"Tự động xuất báo cáo tháng {month:D2}/{year}",
                    PeriodType = "Month",
                    PeriodValue = $"Tháng {month:D2}/{year}",
                    Year = year,
                    Month = month,
                    FileName = "NONE",
                    FilePath = "NONE",
                    Status = "Pending_NoData",
                    ErrorMessage = "Chưa có dữ liệu từ các tỉnh trong CSDL",
                    TriggerType = triggerType,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                });

                results.Add(new ReportExportResult { Success = false, Message = warnMsg });
                return results;
            }

            // 2. Xuất Báo cáo TCV CD45 (Gói nén ZIP)
            if (hasCd45Data)
            {
                var resTCV = await ExportTCVCD45ZipAsync(year, month, null, null, triggerType, createdBy);
                results.Add(resTCV);

                var resHD = await ExportHoatDongCD45ExcelAsync(year, month, null, null, triggerType, createdBy);
                results.Add(resHD);
            }
            else
            {
                log.Warn($"[CD45] Chưa có dữ liệu hoạt động tháng {month}/{year}, bỏ qua xuất CD45.");
            }

            // 3. Xuất Báo cáo Tổng Hợp Số Liệu BVTL
            if (hasBvtlData)
            {
                var resBVTL = await ExportTongHopBVTLExcelAsync(year, month, triggerType, createdBy);
                results.Add(resBVTL);
            }
            else
            {
                log.Warn($"[BVTL] Chưa có dữ liệu tháng {month}/{year}, bỏ qua xuất BVTL.");
            }

            // 4. Bắn thông báo Telegram tổng kết
            int successCount = results.Count(r => r.Success);
            string summaryMsg = $"🎉 [HỆ THỐNG BVTL] Đã hoàn tất xuất báo cáo định kỳ tháng {month:D2}/{year} ({triggerType}):\n" +
                                string.Join("\n", results.Select(r => (r.Success ? "✅ " : "❌ ") + r.Message)) +
                                $"\n👉 Quản trị viên và cán bộ dự án có thể tra cứu và tải trực tiếp tại menu: Báo Cáo Định Kỳ.";

            log.Info(summaryMsg);
            await telegram.SendMessageAsync(summaryMsg);

            return results;
        }

        public async Task<ReportExportResult> ExportTCVCD45ZipAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User")
        {
            var sw = Stopwatch.StartNew();
            var result = new ReportExportResult();

            try
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                string fromDate = $"01/{month:D2}/{year}";
                string toDate = $"{daysInMonth:D2}/{month:D2}/{year}";

                var listTCV = _baoCaoCD45DA.GetListTCV(cityCode, maNhom);
                if (listTCV == null || listTCV.Count == 0)
                {
                    result.Success = false;
                    result.Message = $"Không tìm thấy danh sách Tiếp cận viên CD45 ({cityCode ?? "Tất cả"} - {maNhom ?? "Tất cả"}).";
                    return result;
                }

                string storageDir = GetStorageDirectory(year, month);
                string suffix = !string.IsNullOrEmpty(cityCode) ? $"_{cityCode}" : "_ALL";
                if (!string.IsNullOrEmpty(maNhom)) suffix += $"_{maNhom}";

                string zipFileName = $"BaoCao_TCV_CD45_Thang{month:D2}_{year}{suffix}.zip";
                string zipFilePath = Path.Combine(storageDir, zipFileName);

                if (File.Exists(zipFilePath))
                {
                    try { File.Delete(zipFilePath); } catch { }
                }

                int totalTcvExported = 0;

                using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var tcv in listTCV)
                        {
                            var data = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, null, tcv.MA_NHOM, tcv.MA_TCV);

                            using (var wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("BaoCao");
                                BuildTCVWorksheet(ws, data, fromDate, toDate, tcv.TEN_NHOM ?? tcv.MA_NHOM, tcv.TEN_TCV ?? tcv.MA_TCV);

                                var cleanName = (tcv.TEN_TCV ?? ("TCV_" + tcv.MA_TCV)).Replace("/", "_").Replace("\\", "_");
                                var zipEntry = archive.CreateEntry($"BaoCao_{(tcv.MA_NHOM ?? "CD45")}_{cleanName}.xlsx", CompressionLevel.Fastest);
                                using (var zipStream = zipEntry.Open())
                                {
                                    wb.SaveAs(zipStream);
                                }
                                totalTcvExported++;
                            }
                        }
                    }
                }

                sw.Stop();
                var fileInfo = new FileInfo(zipFilePath);
                long sizeKb = fileInfo.Exists ? fileInfo.Length / 1024 : 0;

                result.Success = true;
                result.FileName = zipFileName;
                result.FilePath = zipFilePath;
                result.FileSizeBytes = fileInfo.Length;
                result.TotalItems = totalTcvExported;
                result.ExecutionTimeMs = (int)sw.ElapsedMilliseconds;
                result.Message = $"Báo cáo TCV CD45: Đóng gói thành công {totalTcvExported} TCV vào file ZIP ({sizeKb} KB).";

                // Ghi log CSDL
                _scheduledReportDA.AddExportLog(new ExportedReportLogModel
                {
                    ReportType = "TCV_CD45",
                    ReportName = "Báo cáo Tiếp cận viên (TCV) Dự án CD45",
                    PeriodType = "Month",
                    PeriodValue = $"Tháng {month:D2}/{year}",
                    Year = year,
                    Month = month,
                    MaDuAn = "CD45",
                    CityCode = cityCode,
                    MaNhom = maNhom,
                    FileName = zipFileName,
                    FilePath = zipFilePath,
                    FileSizeKb = sizeKb,
                    TotalRecords = totalTcvExported,
                    Status = "Success",
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    TelegramSent = true,
                    TriggerType = triggerType,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                log.Error("Lỗi ExportTCVCD45ZipAsync: " + ex.Message, ex);
                result.Success = false;
                result.Message = "Lỗi xuất file ZIP TCV CD45: " + ex.Message;

                _scheduledReportDA.AddExportLog(new ExportedReportLogModel
                {
                    ReportType = "TCV_CD45",
                    ReportName = "Báo cáo Tiếp cận viên (TCV) Dự án CD45",
                    PeriodType = "Month",
                    PeriodValue = $"Tháng {month:D2}/{year}",
                    Year = year,
                    Month = month,
                    MaDuAn = "CD45",
                    FileName = "ERROR",
                    FilePath = "ERROR",
                    Status = "Failed",
                    ErrorMessage = ex.Message,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    TriggerType = triggerType,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                });
            }

            return await Task.FromResult(result);
        }

        public async Task<ReportExportResult> ExportHoatDongCD45ExcelAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User")
        {
            var sw = Stopwatch.StartNew();
            var result = new ReportExportResult();

            try
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                string fromDate = $"01/{month:D2}/{year}";
                string toDate = $"{daysInMonth:D2}/{month:D2}/{year}";

                var data = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, cityCode, maNhom, null);
                string storageDir = GetStorageDirectory(year, month);
                string suffix = !string.IsNullOrEmpty(cityCode) ? $"_{cityCode}" : "_TOANQUOC";

                string fileName = $"BaoCao_HoatDong_CD45_Thang{month:D2}_{year}{suffix}.xlsx";
                string filePath = Path.Combine(storageDir, fileName);

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add($"CD45_T{month}_{year}");
                    BuildHoatDongCD45Worksheet(ws, data, fromDate, toDate, cityCode ?? "Toàn quốc", maNhom ?? "Tất cả nhóm");
                    wb.SaveAs(filePath);
                }

                sw.Stop();
                var fileInfo = new FileInfo(filePath);
                long sizeKb = fileInfo.Exists ? fileInfo.Length / 1024 : 0;

                result.Success = true;
                result.FileName = fileName;
                result.FilePath = filePath;
                result.FileSizeBytes = fileInfo.Length;
                result.TotalItems = data != null ? data.Count : 0;
                result.ExecutionTimeMs = (int)sw.ElapsedMilliseconds;
                result.Message = $"Báo cáo Hoạt động CD45: Xuất thành công file Excel ({sizeKb} KB).";

                _scheduledReportDA.AddExportLog(new ExportedReportLogModel
                {
                    ReportType = "HOATDONG_CD45",
                    ReportName = "Báo cáo Hoạt động Tổng hợp Dự án CD45",
                    PeriodType = "Month",
                    PeriodValue = $"Tháng {month:D2}/{year}",
                    Year = year,
                    Month = month,
                    MaDuAn = "CD45",
                    CityCode = cityCode,
                    MaNhom = maNhom,
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeKb = sizeKb,
                    TotalRecords = result.TotalItems,
                    Status = "Success",
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    TelegramSent = true,
                    TriggerType = triggerType,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                log.Error("Lỗi ExportHoatDongCD45ExcelAsync: " + ex.Message, ex);
                result.Success = false;
                result.Message = "Lỗi xuất Báo cáo Hoạt động CD45: " + ex.Message;
            }

            return await Task.FromResult(result);
        }

        public async Task<ReportExportResult> ExportTongHopBVTLExcelAsync(int year, int month, string triggerType = "Manual", string createdBy = "User")
        {
            var sw = Stopwatch.StartNew();
            var result = new ReportExportResult();

            try
            {
                var modelSearch = new ReportSearchModel
                {
                    Year = year,
                    Months = month.ToString(),
                    TypeReport = 1,
                    MaDuAn = "BVTL"
                };

                var data = _baoCaoTongHopDA.GetDataReport(modelSearch);
                string storageDir = GetStorageDirectory(year, month);
                string fileName = $"BaoCao_TongHop_BVTL_Thang{month:D2}_{year}.xlsx";
                string filePath = Path.Combine(storageDir, fileName);

                string titleReport = $"Kỳ báo cáo: Báo cáo Tháng {month:D2} - {year}";
                string sheetName = $"Báo cáo tháng {month:D2} năm {year}";
                string finalName;

                byte[] fileBytes = _excelReportService.ExportReport(
                    data,
                    titleReport,
                    sheetName,
                    null,
                    "Toàn hệ thống",
                    Path.GetFileNameWithoutExtension(fileName),
                    out finalName
                );

                File.WriteAllBytes(filePath, fileBytes);

                sw.Stop();
                var fileInfo = new FileInfo(filePath);
                long sizeKb = fileInfo.Exists ? fileInfo.Length / 1024 : 0;

                result.Success = true;
                result.FileName = fileName;
                result.FilePath = filePath;
                result.FileSizeBytes = fileBytes.Length;
                result.TotalItems = data != null ? data.Count : 0;
                result.ExecutionTimeMs = (int)sw.ElapsedMilliseconds;
                result.Message = $"Báo cáo Tổng hợp BVTL: Xuất thành công file Excel ({sizeKb} KB).";

                _scheduledReportDA.AddExportLog(new ExportedReportLogModel
                {
                    ReportType = "TONGHOP_BVTL",
                    ReportName = "Báo cáo Tổng hợp Số liệu Bảo vệ Tương lai (BVTL)",
                    PeriodType = "Month",
                    PeriodValue = $"Tháng {month:D2}/{year}",
                    Year = year,
                    Month = month,
                    MaDuAn = "BVTL",
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeKb = sizeKb,
                    TotalRecords = result.TotalItems,
                    Status = "Success",
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    TelegramSent = true,
                    TriggerType = triggerType,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                log.Error("Lỗi ExportTongHopBVTLExcelAsync: " + ex.Message, ex);
                result.Success = false;
                result.Message = "Lỗi xuất Báo cáo Tổng hợp BVTL: " + ex.Message;
            }

            return await Task.FromResult(result);
        }

        private void BuildTCVWorksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenNhom, string tenTCV)
        {
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG - DỰ ÁN CD45";
            ws.Range("A1:H1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate}";
            ws.Range("A2:H2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A3").Value = $"Nhóm: {tenNhom} | Tiếp cận viên: {tenTCV}";
            ws.Range("A3:H3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A5").Value = "#";
            ws.Cell("B5").Value = "Thông tin báo cáo";
            ws.Cell("C5").Value = "Tổng";
            ws.Cell("D5").Value = "PUD";
            ws.Cell("E5").Value = "PLHIV";
            ws.Cell("F5").Value = "TG";
            ws.Cell("G5").Value = "SW";
            ws.Cell("H5").Value = "MSM";

            var headerRange = ws.Range("A5:H5");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8ECEF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 6;
            if (data != null)
            {
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var prefix = item.IndentLevel == 1 ? "      " : "";
                    ws.Cell(row, 2).Value = prefix + item.ChiTieu;

                    ws.Cell(row, 3).Value = item.Tong ?? 0;
                    ws.Cell(row, 4).Value = item.PUD ?? 0;
                    ws.Cell(row, 5).Value = item.PLHIV ?? 0;
                    ws.Cell(row, 6).Value = item.TG ?? 0;
                    ws.Cell(row, 7).Value = item.SW ?? 0;
                    ws.Cell(row, 8).Value = item.MSM ?? 0;

                    for (int c = 3; c <= 8; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    if (item.IsBold)
                    {
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
                    }
                    row++;
                }
            }

            var dataTableRange = ws.Range(5, 1, Math.Max(row - 1, 6), 8);
            dataTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            row += 2;
            ws.Cell(row, 2).Value = "Tiếp cận viên";
            ws.Cell(row, 2).Style.Font.Bold = true;
            ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 5).Value = "Cán bộ dự án";
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 8).Value = "MnE";
            ws.Cell(row, 8).Style.Font.Bold = true;
            ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Columns().AdjustToContents();
        }

        private void BuildHoatDongCD45Worksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenTinh, string tenNhom)
        {
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG TỔNG HỢP - DỰ ÁN CD45 (DREAMH)";
            ws.Range("A1:H1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate} | Tỉnh/Thành: {tenTinh} | Nhóm: {tenNhom}";
            ws.Range("A2:H2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Italic = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A4").Value = "#";
            ws.Cell("B4").Value = "Chỉ tiêu báo cáo";
            ws.Cell("C4").Value = "Tổng";
            ws.Cell("D4").Value = "PUD";
            ws.Cell("E4").Value = "PLHIV";
            ws.Cell("F4").Value = "TG";
            ws.Cell("G4").Value = "SW";
            ws.Cell("H4").Value = "MSM";

            var headerRange = ws.Range("A4:H4");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#CCE5FF");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 5;
            if (data != null)
            {
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var prefix = item.IndentLevel == 1 ? "      " : "";
                    ws.Cell(row, 2).Value = prefix + item.ChiTieu;

                    ws.Cell(row, 3).Value = item.Tong ?? 0;
                    ws.Cell(row, 4).Value = item.PUD ?? 0;
                    ws.Cell(row, 5).Value = item.PLHIV ?? 0;
                    ws.Cell(row, 6).Value = item.TG ?? 0;
                    ws.Cell(row, 7).Value = item.SW ?? 0;
                    ws.Cell(row, 8).Value = item.MSM ?? 0;

                    for (int c = 3; c <= 8; c++)
                    {
                        ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }

                    if (item.IsBold)
                    {
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
                    }
                    row++;
                }
            }

            var dataTableRange = ws.Range(4, 1, Math.Max(row - 1, 5), 8);
            dataTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();
        }
    }
}
