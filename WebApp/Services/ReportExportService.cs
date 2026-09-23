using ClosedXML.Excel;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
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
            if (System.Web.Hosting.HostingEnvironment.IsHosted)
            {
                baseDir = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/ExportedReports/");
            }
            else if (HttpContext.Current != null && HttpContext.Current.Server != null)
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

        /// <summary>
        /// Map periodType sang giá trị @LoaiBaoCao để lọc chỉ tiêu trong SP_CD45_GetBaoCao.
        /// Khi xuất báo cáo tự động, cần đảm bảo đúng chỉ tiêu cho từng kỳ.
        /// </summary>
        private static string MapPeriodTypeToLoaiBaoCao(string periodType)
        {
            if (periodType == "Quarter" || periodType == "Quy") return "Quy";
            if (periodType == "Year" || periodType == "Nam" || periodType == "12Thang") return "12T";
            if (periodType == "6Thang" || periodType == "HalfYear") return "6T";
            return "Thang"; // Default: Month
        }

        public static void CalculatePeriodDateRange(string periodType, int year, int periodNumber, out string fromDate, out string toDate, out string periodValue)
        {
            if (periodType == "Quarter" || periodType == "Quy")
            {
                switch (periodNumber)
                {
                    case 1:
                        fromDate = $"26/12/{year - 1}";
                        toDate = $"25/03/{year}";
                        periodValue = $"Quý I/{year}";
                        break;
                    case 2:
                        fromDate = $"26/03/{year}";
                        toDate = $"25/06/{year}";
                        periodValue = $"Quý II/{year}";
                        break;
                    case 3:
                        fromDate = $"26/06/{year}";
                        toDate = $"25/09/{year}";
                        periodValue = $"Quý III/{year}";
                        break;
                    case 4:
                    default:
                        fromDate = $"26/09/{year}";
                        toDate = $"25/12/{year}";
                        periodValue = $"Quý IV/{year}";
                        break;
                }
            }
            else if (periodType == "Year" || periodType == "Nam" || periodType == "12Thang")
            {
                fromDate = $"26/12/{year - 1}";
                toDate = $"25/12/{year}";
                periodValue = $"Năm {year}";
            }
            else // Default: Month (Tháng: từ 26 tháng trước đến 25 tháng này)
            {
                int prevMonth = periodNumber == 1 ? 12 : periodNumber - 1;
                int prevYear = periodNumber == 1 ? year - 1 : year;
                fromDate = $"26/{prevMonth:D2}/{prevYear}";
                toDate = $"25/{periodNumber:D2}/{year}";
                periodValue = $"Tháng {periodNumber:D2}/{year}";
            }
        }

        public async Task<List<ReportExportResult>> ExecuteAllMonthlyReportsAsync(int year, int month, string triggerType = "AutoSchedule", string createdBy = "QuartzScheduler")
        {
            var results = new List<ReportExportResult>();
            var settings = _scheduledReportDA.GetSettings();
            var telegram = new TelegramNotifier(chatId: settings.TelegramChatId);

            string fromDate, toDate, pValue;
            CalculatePeriodDateRange("Month", year, month, out fromDate, out toDate, out pValue);

            log.Info($"[PeriodicReportExportJob] Bắt đầu xử lý xuất báo cáo tự động {pValue} (Từ {fromDate} đến {toDate}) ({triggerType})...");

            // 1. Pre-flight Check: Kiểm tra dữ liệu tháng theo chu kỳ 26-25
            bool hasCd45Data = _scheduledReportDA.CheckDataAvailability("TCV_CD45", year, month, "Month");

            if (!hasCd45Data)
            {
                string warnMsg = $"⚠️ [HỆ THỐNG BVTL] Tạm hoãn xuất tự động báo cáo {pValue} (Từ {fromDate} đến {toDate}): Chưa phát hiện dữ liệu nhập từ các tỉnh trong CSDL. Hệ thống sẽ giữ nguyên và thử lại vào chu kỳ tiếp theo.";
                log.Warn(warnMsg);
                await telegram.SendMessageAsync(warnMsg);

                // Ghi nhận log trạng thái tạm hoãn
                _scheduledReportDA.SaveOrUpdateExportLog(new ExportedReportLogModel
                {
                    ReportType = "ALL",
                    ReportName = $"Tự động xuất báo cáo {pValue}",
                    PeriodType = "Month",
                    PeriodValue = pValue,
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

            // 2. Xuất Báo cáo TCV CD45 & Báo cáo Hoạt động CD45
            var resTCV = await ExportTCVCD45ZipAsync(year, month, null, null, triggerType, createdBy, "Month", fromDate, toDate, pValue);
            results.Add(resTCV);

            var resHD = await ExportHoatDongCD45ExcelAsync(year, month, null, null, triggerType, createdBy, "Month", fromDate, toDate, pValue);
            results.Add(resHD);

            // 3. Bắn thông báo Telegram tổng kết
            int successCount = results.Count(r => r.Success);
            string summaryMsg = $"🎉 [HỆ THỐNG BVTL] Đã hoàn tất xuất báo cáo định kỳ {pValue} (Từ {fromDate} đến {toDate}) ({triggerType}):\n" +
                                string.Join("\n", results.Select(r => (r.Success ? "✅ " : "❌ ") + r.Message)) +
                                $"\n👉 Quản trị viên và cán bộ dự án có thể tra cứu và tải trực tiếp tại menu: Báo Cáo Định Kỳ.";

            log.Info(summaryMsg);
            await telegram.SendMessageAsync(summaryMsg);

            return results;
        }

        private class GroupProcessItem
        {
            public string QueryCode { get; set; }
            public string TenNhom { get; set; }
            public string XungDanh { get; set; }
            public string ShortPrefix { get; set; }
            public List<CD45_TCV_ItemModel> TcvList { get; set; }
        }

        public async Task<ReportExportResult> ExportTCVCD45ZipAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null)
        {
            var sw = Stopwatch.StartNew();
            var result = new ReportExportResult();

            try
            {
                string fromDate = customFromDate;
                string toDate = customToDate;
                string pValue = periodValue;

                if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                {
                    CalculatePeriodDateRange(periodType ?? "Month", year, month, out fromDate, out toDate, out pValue);
                }

                string filterCityCode = string.IsNullOrWhiteSpace(cityCode) ? null : cityCode.Trim();
                string filterMaNhom = string.IsNullOrWhiteSpace(maNhom) ? null : maNhom.Trim();

                var listTCV = _baoCaoCD45DA.GetListTCV(filterCityCode, filterMaNhom) ?? new List<CD45_TCV_ItemModel>();
                var allNhoms = _nhomTBHDA.GetAll() ?? new List<BVTL_NHOM_TBH>();

                var cd45Nhoms = allNhoms
                    .Where(x => string.Equals(x.maduan, "CD45", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!string.IsNullOrEmpty(filterCityCode))
                {
                    cd45Nhoms = cd45Nhoms
                        .Where(x => string.Equals((x.city_code ?? "").Trim(), filterCityCode, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(filterMaNhom))
                {
                    cd45Nhoms = cd45Nhoms
                        .Where(x => string.Equals((x.manhom_tbh ?? "").Trim(), filterMaNhom, StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals((x.manhom_tbh_map ?? "").Trim(), filterMaNhom, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if ((listTCV == null || listTCV.Count == 0) && (cd45Nhoms == null || cd45Nhoms.Count == 0))
                {
                    result.Success = false;
                    result.Message = $"Không tìm thấy dữ liệu Tiếp cận viên hoặc Nhóm CBO thuộc dự án CD45 ({filterCityCode ?? "Tất cả"} - {filterMaNhom ?? "Tất cả"}).";
                    return result;
                }

                string storageDir = GetStorageDirectory(year, month);
                string suffix = !string.IsNullOrEmpty(filterCityCode) ? $"_{filterCityCode}" : "_ALL";
                if (!string.IsNullOrEmpty(filterMaNhom)) suffix += $"_{filterMaNhom}";

                string periodTag;
                if (periodType == "Quarter" || periodType == "Quy")
                    periodTag = $"Quy{month}_{year}";
                else if (periodType == "Year" || periodType == "Nam" || periodType == "12Thang")
                    periodTag = $"Nam{year}";
                else
                    periodTag = $"Thang{month:D2}_{year}";

                string zipFileName = $"BaoCao_TCV_CD45_{periodTag}{suffix}.zip";
                string zipFilePath = Path.Combine(storageDir, zipFileName);

                if (File.Exists(zipFilePath))
                {
                    try { File.Delete(zipFilePath); } catch { }
                }

                var cities = _cityDA.GetAll();
                var cityDict = (cities != null && cities.Count > 0)
                    ? cities.ToDictionary(x => (x.Code ?? "").Trim(), x => (x.Name ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                Func<string, string> getCityName = (cCode) =>
                {
                    if (string.IsNullOrEmpty(cCode)) return "Toàn dự án";
                    if (cityDict.TryGetValue(cCode, out var cName) && !string.IsNullOrEmpty(cName)) return cName;
                    switch (cCode.ToUpper())
                    {
                        case "HNO": return "Hà Nội";
                        case "HPG": return "Hải Phòng";
                        case "HYE": return "Hưng Yên";
                        case "NAN": return "Nghệ An";
                        case "NBI": return "Ninh Bình";
                        case "HCM": return "TP. Hồ Chí Minh";
                        default: return cCode;
                    }
                };

                Func<string, string> sanitize = (input) =>
                {
                    if (string.IsNullOrWhiteSpace(input)) return "Unknown";
                    var invalid = Path.GetInvalidFileNameChars();
                    var clean = new string(input.Where(ch => !invalid.Contains(ch) && ch != '/' && ch != '\\').ToArray()).Trim();
                    return string.IsNullOrWhiteSpace(clean) ? "Unknown" : clean;
                };

                int totalTcvExported = 0;
                int totalSummaries = 0;
                string loaiBaoCaoFilter = MapPeriodTypeToLoaiBaoCao(periodType);

                Func<string, BVTL_NHOM_TBH> findNhom = (mNhom) =>
                {
                    if (string.IsNullOrEmpty(mNhom) || allNhoms == null) return null;
                    return allNhoms.FirstOrDefault(x => string.Equals((x.manhom_tbh ?? "").Trim(), mNhom.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                        string.Equals((x.manhom_tbh_map ?? "").Trim(), mNhom.Trim(), StringComparison.OrdinalIgnoreCase));
                };

                List<string> targetCityCodes;
                if (!string.IsNullOrEmpty(filterCityCode))
                {
                    targetCityCodes = new List<string> { filterCityCode.ToUpper() };
                }
                else
                {
                    targetCityCodes = cd45Nhoms
                        .Where(x => !string.IsNullOrWhiteSpace(x.city_code))
                        .Select(x => x.city_code.Trim().ToUpper())
                        .Union(listTCV.Where(x => !string.IsNullOrWhiteSpace(x.CITY_CODE)).Select(x => x.CITY_CODE.Trim().ToUpper()))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(c => c)
                        .ToList();
                }

                using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var currentCityCode in targetCityCodes)
                        {
                            string currentCityName = getCityName(currentCityCode);
                            string cleanCityFolder = sanitize(currentCityName);

                            // 1. Báo cáo tổng hợp cấp Tỉnh (nếu có dữ liệu)
                            try
                            {
                                var cityData = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, currentCityCode, null, null, loaiBaoCaoFilter);
                                if (cityData != null && cityData.Count > 0)
                                {
                                    using (var wbCity = new XLWorkbook())
                                    {
                                        var wsCity = wbCity.Worksheets.Add("TongHop_" + currentCityCode);
                                        BuildHoatDongCD45Worksheet(wsCity, cityData, fromDate, toDate, currentCityName, "Toàn tỉnh (" + currentCityCode + ")", "Nhóm");
                                        var cityEntry = archive.CreateEntry($"{cleanCityFolder}/BaoCao_TongHop_{sanitize(currentCityCode)}.xlsx", CompressionLevel.Fastest);
                                        using (var zipStream = cityEntry.Open())
                                        {
                                            wbCity.SaveAs(zipStream);
                                        }
                                        totalSummaries++;
                                    }
                                }
                            }
                            catch (Exception exCity)
                            {
                                log.Warn($"Không thể tạo file tổng hợp tỉnh {currentCityCode}: {exCity.Message}");
                            }

                            // 2. Gom nhóm theo từng Nhóm CBO trong Tỉnh
                            var nhomsInCity = cd45Nhoms
                                .Where(x => string.Equals((x.city_code ?? "").Trim(), currentCityCode, StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            var tcvsInCity = listTCV
                                .Where(x => string.Equals((x.CITY_CODE ?? "").Trim(), currentCityCode, StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            var groupItems = new List<GroupProcessItem>();
                            var processedGroupCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            // 2.1 Ưu tiên duyệt từ danh mục nhóm CBO CD45
                            foreach (var nhom in nhomsInCity)
                            {
                                string stdCode = (nhom.manhom_tbh ?? "").Trim();
                                string mapCode = (nhom.manhom_tbh_map ?? "").Trim();
                                string queryCode = !string.IsNullOrEmpty(mapCode) ? mapCode : stdCode;
                                if (string.IsNullOrEmpty(queryCode)) continue;

                                var matchedTcvs = tcvsInCity.Where(t =>
                                    string.Equals((t.MA_NHOM ?? "").Trim(), stdCode, StringComparison.OrdinalIgnoreCase) ||
                                    (!string.IsNullOrEmpty(mapCode) && string.Equals((t.MA_NHOM ?? "").Trim(), mapCode, StringComparison.OrdinalIgnoreCase))
                                ).ToList();

                                string tenNhom = !string.IsNullOrWhiteSpace(nhom.tennhom_tbh)
                                    ? nhom.tennhom_tbh.Trim()
                                    : (matchedTcvs.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.TEN_NHOM))?.TEN_NHOM?.Trim() ?? queryCode);

                                string xungDanh = nhom.GetXungDanh();
                                string shortPrefix = nhom.GetShortXungDanh();

                                groupItems.Add(new GroupProcessItem
                                {
                                    QueryCode = queryCode,
                                    TenNhom = tenNhom,
                                    XungDanh = xungDanh,
                                    ShortPrefix = shortPrefix,
                                    TcvList = matchedTcvs
                                });

                                if (!string.IsNullOrEmpty(stdCode)) processedGroupCodes.Add(stdCode);
                                if (!string.IsNullOrEmpty(mapCode)) processedGroupCodes.Add(mapCode);
                            }

                            // 2.2 Gom tiếp các nhóm có TCV trong CSDL nhưng không có trong danh mục nhóm CD45
                            var remainingTcvGroups = tcvsInCity
                                .Where(t => !processedGroupCodes.Contains((t.MA_NHOM ?? "").Trim()))
                                .GroupBy(t => (t.MA_NHOM ?? "OTHER").Trim(), StringComparer.OrdinalIgnoreCase);

                            foreach (var rGroup in remainingTcvGroups)
                            {
                                string rMaNhom = rGroup.Key;
                                var firstTcv = rGroup.First();
                                var nhomObj = findNhom(rMaNhom);

                                string tenNhom = (nhomObj != null && !string.IsNullOrWhiteSpace(nhomObj.tennhom_tbh))
                                    ? nhomObj.tennhom_tbh.Trim()
                                    : (!string.IsNullOrWhiteSpace(firstTcv.TEN_NHOM) ? firstTcv.TEN_NHOM.Trim() : rMaNhom);

                                string xungDanh = (nhomObj != null && !string.IsNullOrWhiteSpace(nhomObj.PREFIX) && nhomObj.PREFIX != "Nhóm")
                                    ? nhomObj.GetXungDanh()
                                    : (!string.IsNullOrWhiteSpace(firstTcv.PREFIX) ? firstTcv.PREFIX.Trim() : (nhomObj != null ? nhomObj.GetXungDanh() : "Nhóm"));

                                string shortPrefix = (nhomObj != null && !string.IsNullOrWhiteSpace(nhomObj.SHORT_PREFIX) && nhomObj.SHORT_PREFIX != "Nhóm")
                                    ? nhomObj.GetShortXungDanh()
                                    : (!string.IsNullOrWhiteSpace(firstTcv.SHORT_PREFIX) ? firstTcv.SHORT_PREFIX.Trim() : (nhomObj != null ? nhomObj.GetShortXungDanh() : "Nhóm"));

                                groupItems.Add(new GroupProcessItem
                                {
                                    QueryCode = rMaNhom,
                                    TenNhom = tenNhom,
                                    XungDanh = xungDanh,
                                    ShortPrefix = shortPrefix,
                                    TcvList = rGroup.ToList()
                                });
                            }

                            // Tiến hành xuất báo cáo cho từng Nhóm CBO
                            foreach (var groupItem in groupItems)
                            {
                                string cleanGroupFolder = sanitize($"{groupItem.ShortPrefix} {groupItem.TenNhom}");

                                // Báo cáo tổng hợp cấp Nhóm CBO
                                try
                                {
                                    var nhomData = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, null, groupItem.QueryCode, null, loaiBaoCaoFilter);
                                    if (nhomData != null && nhomData.Count > 0)
                                    {
                                        using (var wbNhom = new XLWorkbook())
                                        {
                                            var wsNhom = wbNhom.Worksheets.Add("TongHop_Nhom");
                                            BuildHoatDongCD45Worksheet(wsNhom, nhomData, fromDate, toDate, currentCityName, groupItem.TenNhom, groupItem.XungDanh);
                                            var nhomEntry = archive.CreateEntry($"{cleanCityFolder}/{cleanGroupFolder}/BaoCao_TongHop_{sanitize(groupItem.ShortPrefix)}_{sanitize(groupItem.TenNhom)}.xlsx", CompressionLevel.Fastest);
                                            using (var zipStream = nhomEntry.Open())
                                            {
                                                wbNhom.SaveAs(zipStream);
                                            }
                                            totalSummaries++;
                                        }
                                    }
                                }
                                catch (Exception exNhom)
                                {
                                    log.Warn($"Không thể tạo file tổng hợp nhóm {groupItem.QueryCode}: {exNhom.Message}");
                                }

                                // Các Báo cáo TCV cá nhân trong Nhóm CBO (nếu nhóm có TCV)
                                if (groupItem.TcvList != null && groupItem.TcvList.Count > 0)
                                {
                                    foreach (var tcv in groupItem.TcvList)
                                    {
                                        var data = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, null, tcv.MA_NHOM, tcv.MA_TCV, loaiBaoCaoFilter);

                                        using (var wb = new XLWorkbook())
                                        {
                                            var ws = wb.Worksheets.Add("BaoCao");
                                            BuildTCVWorksheet(ws, data, fromDate, toDate, tcv.TEN_NHOM ?? groupItem.TenNhom, tcv.TEN_TCV ?? tcv.MA_TCV, groupItem.XungDanh);

                                            var cleanTcvName = sanitize(tcv.TEN_TCV ?? ("TCV_" + tcv.MA_TCV));
                                            var zipEntry = archive.CreateEntry($"{cleanCityFolder}/{cleanGroupFolder}/BaoCao_TCV_{cleanTcvName}.xlsx", CompressionLevel.Fastest);
                                            using (var zipStream = zipEntry.Open())
                                            {
                                                wb.SaveAs(zipStream);
                                            }
                                            totalTcvExported++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                sw.Stop();
                var fileInfo = new FileInfo(zipFilePath);
                long sizeKb = fileInfo.Exists ? fileInfo.Length / 1024 : 0;

                if (totalTcvExported == 0 && totalSummaries == 0)
                {
                    result.Success = false;
                    result.Message = $"Không tìm thấy dữ liệu báo cáo nào được tạo ({filterCityCode ?? "Tất cả"} - {filterMaNhom ?? "Tất cả"}).";
                    return result;
                }

                result.Success = true;
                result.FileName = zipFileName;
                result.FilePath = zipFilePath;
                result.FileSizeBytes = fileInfo.Length;
                result.TotalItems = totalTcvExported > 0 ? totalTcvExported : totalSummaries;
                result.ExecutionTimeMs = (int)sw.ElapsedMilliseconds;
                result.Message = totalTcvExported > 0
                    ? $"Báo cáo TCV CD45 theo Tỉnh/Nhóm: Đóng gói thành công {totalTcvExported} TCV và {totalSummaries} báo cáo tổng hợp Tỉnh/Nhóm vào file ZIP ({sizeKb} KB)."
                    : $"Báo cáo TCV CD45 theo Tỉnh/Nhóm: Đóng gói thành công {totalSummaries} báo cáo tổng hợp Tỉnh/Nhóm vào file ZIP ({sizeKb} KB).";

                // Ghi log CSDL (bỏ qua khi chạy UnitTest để không làm bẩn dữ liệu thật)
                if (triggerType != "UnitTest")
                {
                    _scheduledReportDA.SaveOrUpdateExportLog(new ExportedReportLogModel
                    {
                        ReportType = "TCV_CD45",
                        ReportName = "Báo cáo TCV CD45 (.ZIP theo Tỉnh/Nhóm)",
                        PeriodType = periodType ?? "Month",
                        PeriodValue = pValue ?? $"Tháng {month:D2}/{year}",
                        Year = year,
                        Month = month,
                        MaDuAn = "CD45",
                        CityCode = filterCityCode,
                        MaNhom = filterMaNhom,
                        FileName = zipFileName,
                        FilePath = zipFilePath,
                        FileSizeKb = sizeKb,
                        TotalRecords = totalTcvExported > 0 ? totalTcvExported : totalSummaries,
                        Status = "Success",
                        ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                        TelegramSent = true,
                        TriggerType = triggerType,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                log.Error("Lỗi ExportTCVCD45ZipAsync: " + ex.Message, ex);
                result.Success = false;
                result.Message = "Lỗi xuất file ZIP TCV CD45: " + ex.Message;

                if (triggerType != "UnitTest")
                {
                    _scheduledReportDA.SaveOrUpdateExportLog(new ExportedReportLogModel
                    {
                        ReportType = "TCV_CD45",
                        ReportName = "Báo cáo TCV CD45 (.ZIP theo Tỉnh/Nhóm)",
                        PeriodType = periodType ?? "Month",
                        PeriodValue = periodValue ?? $"Tháng {month:D2}/{year}",
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
            }

            return await Task.FromResult(result);
        }

        public async Task<ReportExportResult> ExportHoatDongCD45ExcelAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null)
        {
            var sw = Stopwatch.StartNew();
            var result = new ReportExportResult();

            try
            {
                string fromDate = customFromDate;
                string toDate = customToDate;
                string pValue = periodValue;

                if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                {
                    CalculatePeriodDateRange(periodType ?? "Month", year, month, out fromDate, out toDate, out pValue);
                }

                var data = _baoCaoCD45DA.GetBaoCao(fromDate, toDate, cityCode, maNhom, null, MapPeriodTypeToLoaiBaoCao(periodType));

                // VR-01 [BLOCKING]: Kiểm toán cấu trúc số học (Tổng = 5 nhóm đích) trước khi tạo file
                if (!ReportValidatorHelper.ValidateReportArithmetic(data, out var arithmeticError))
                {
                    sw.Stop();
                    log.Error(arithmeticError);
                    result.Success = false;
                    result.Message = arithmeticError;
                    return await Task.FromResult(result);
                }

                var cities = _cityDA.GetAll();
                var cityDict = (cities != null && cities.Count > 0)
                    ? cities.ToDictionary(x => (x.Code ?? "").Trim(), x => (x.Name ?? "").Trim(), StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                Func<string, string> getCityName = (cCode) =>
                {
                    if (string.IsNullOrEmpty(cCode)) return "Toàn dự án";
                    if (cityDict.TryGetValue(cCode, out var cName) && !string.IsNullOrEmpty(cName)) return cName;
                    switch (cCode.ToUpper())
                    {
                        case "HNO": return "Hà Nội";
                        case "HPG": return "Hải Phòng";
                        case "HYE": return "Hưng Yên";
                        case "NAN": return "Nghệ An";
                        case "NBI": return "Ninh Bình";
                        case "HCM": return "TP. Hồ Chí Minh";
                        default: return cCode;
                    }
                };

                Func<string, string> sanitize = (input) =>
                {
                    if (string.IsNullOrWhiteSpace(input)) return "Unknown";
                    var invalid = Path.GetInvalidFileNameChars();
                    var clean = new string(input.Where(ch => !invalid.Contains(ch) && ch != '/' && ch != '\\').ToArray()).Trim();
                    return string.IsNullOrWhiteSpace(clean) ? "Unknown" : clean;
                };

                string resolvedCityName = !string.IsNullOrEmpty(cityCode) ? getCityName(cityCode) : "Toàn quốc";
                string resolvedTenNhom = "Tất cả nhóm";
                string resolvedXungDanh = "Nhóm";

                if (!string.IsNullOrEmpty(maNhom))
                {
                    var nhoms = _nhomTBHDA.GetAll();
                    var nhomItem = nhoms?.FirstOrDefault(x => string.Equals(x.manhom_tbh, maNhom, StringComparison.OrdinalIgnoreCase) ||
                                                              string.Equals(x.manhom_tbh_map, maNhom, StringComparison.OrdinalIgnoreCase));
                    if (nhomItem != null && !string.IsNullOrEmpty(nhomItem.tennhom_tbh))
                    {
                        resolvedTenNhom = nhomItem.tennhom_tbh.Trim();
                        resolvedXungDanh = nhomItem.GetXungDanh();
                        if (resolvedXungDanh == "Nhóm" && _baoCaoCD45DA != null)
                        {
                            var tcvList = _baoCaoCD45DA.GetListTCV(null, maNhom);
                            var tcvFirst = tcvList?.FirstOrDefault();
                            if (tcvFirst != null && !string.IsNullOrWhiteSpace(tcvFirst.PREFIX))
                            {
                                resolvedXungDanh = tcvFirst.PREFIX.Trim();
                            }
                        }
                        if (string.IsNullOrEmpty(cityCode) && !string.IsNullOrEmpty(nhomItem.city_code))
                        {
                            cityCode = nhomItem.city_code.Trim();
                            resolvedCityName = getCityName(cityCode);
                        }
                    }
                    else
                    {
                        resolvedTenNhom = maNhom;
                    }
                }

                string suffix;
                string reportName;
                string scopeDesc;

                if (!string.IsNullOrEmpty(maNhom))
                {
                    suffix = $"_{sanitize(cityCode ?? "CD45")}_{sanitize(maNhom)}";
                    reportName = $"Báo cáo Hoạt động CD45 - {resolvedXungDanh} {resolvedTenNhom} ({resolvedCityName})";
                    scopeDesc = $"{resolvedXungDanh} {resolvedTenNhom} - {resolvedCityName}";
                }
                else if (!string.IsNullOrEmpty(cityCode))
                {
                    suffix = $"_{sanitize(cityCode)}";
                    reportName = $"Báo cáo Hoạt động CD45 - Tỉnh {resolvedCityName}";
                    scopeDesc = $"Tỉnh {resolvedCityName}";
                }
                else
                {
                    suffix = "_TOANQUOC";
                    reportName = "Báo cáo Hoạt động Tổng hợp Dự án CD45 (Toàn quốc)";
                    scopeDesc = "Toàn quốc";
                }

                string storageDir = GetStorageDirectory(year, month);

                string periodTag;
                if (periodType == "Quarter" || periodType == "Quy")
                    periodTag = $"Quy{month}_{year}";
                else if (periodType == "Year" || periodType == "Nam" || periodType == "12Thang")
                    periodTag = $"Nam{year}";
                else
                    periodTag = $"Thang{month:D2}_{year}";

                string fileName = $"BaoCao_HoatDong_CD45_{periodTag}{suffix}.xlsx";
                string filePath = Path.Combine(storageDir, fileName);

                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add($"CD45_{periodTag}");
                    BuildHoatDongCD45Worksheet(ws, data, fromDate, toDate, resolvedCityName, resolvedTenNhom, resolvedXungDanh);
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
                result.Message = $"Báo cáo Hoạt động CD45 ({scopeDesc}): Xuất thành công file Excel ({sizeKb} KB).";

                if (triggerType != "UnitTest")
                {
                    _scheduledReportDA.SaveOrUpdateExportLog(new ExportedReportLogModel
                    {
                        ReportType = "HOATDONG_CD45",
                        ReportName = reportName,
                        PeriodType = periodType ?? "Month",
                        PeriodValue = pValue ?? $"Tháng {month:D2}/{year}",
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

                if (triggerType != "UnitTest")
                {
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

        private void BuildTCVWorksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenNhom, string tenTCV, string xungDanh = "Nhóm")
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

            string prefixText = !string.IsNullOrWhiteSpace(xungDanh) ? xungDanh.Trim() : "Nhóm";
            ws.Cell("A3").Value = $"{prefixText}: {tenNhom} | Tiếp cận viên: {tenTCV}";
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

            Action<IXLCell, int?> setVal = (c, val) =>
            {
                if (val.HasValue && val.Value > 0)
                {
                    c.Value = val.Value;
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    c.Value = "-";
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            };

            int row = 6;
            if (data != null)
            {
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var prefix = item.IndentLevel == 1 ? "      " : "";
                    ws.Cell(row, 2).Value = prefix + item.ChiTieu;

                    if (item.IsBold)
                    {
                        for (int c = 3; c <= 8; c++) ws.Cell(row, c).Value = "";
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
                    }
                    else
                    {
                        setVal(ws.Cell(row, 3), item.Tong);
                        setVal(ws.Cell(row, 4), item.PUD);
                        setVal(ws.Cell(row, 5), item.PLHIV);
                        setVal(ws.Cell(row, 6), item.TG);
                        setVal(ws.Cell(row, 7), item.SW);
                        setVal(ws.Cell(row, 8), item.MSM);
                    }
                    row++;
                }
            }

            var dataTableRange = ws.Range(5, 1, Math.Max(row - 1, 6), 8);
            dataTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Signature Footer
            row += 2;
            int signTitleRow = row;
            int signNoteRow = row + 1;
            int signNameRow = row + 5;

            // 1. Khối chữ ký "Tiếp cận viên" (Merge cột A:B)
            ws.Range(signTitleRow, 1, signTitleRow, 2).Merge();
            ws.Cell(signTitleRow, 1).Value = "Tiếp cận viên";
            ws.Cell(signTitleRow, 1).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 1, signNoteRow, 2).Merge();
            ws.Cell(signNoteRow, 1).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 1).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 1).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (!string.IsNullOrEmpty(tenTCV))
            {
                ws.Range(signNameRow, 1, signNameRow, 2).Merge();
                ws.Cell(signNameRow, 1).Value = tenTCV;
                ws.Cell(signNameRow, 1).Style.Font.Bold = true;
                ws.Cell(signNameRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // 2. Khối chữ ký "Cán bộ dự án" (Merge cột C:E)
            ws.Range(signTitleRow, 3, signTitleRow, 5).Merge();
            ws.Cell(signTitleRow, 3).Value = "Cán bộ dự án";
            ws.Cell(signTitleRow, 3).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 3, signNoteRow, 5).Merge();
            ws.Cell(signNoteRow, 3).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 3).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 3).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // 3. Khối chữ ký "MnE" (Merge cột F:H)
            ws.Range(signTitleRow, 6, signTitleRow, 8).Merge();
            ws.Cell(signTitleRow, 6).Value = "MnE";
            ws.Cell(signTitleRow, 6).Style.Font.Bold = true;
            ws.Cell(signTitleRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(signNoteRow, 6, signNoteRow, 8).Merge();
            ws.Cell(signNoteRow, 6).Value = "(Ký, ghi rõ họ tên)";
            ws.Cell(signNoteRow, 6).Style.Font.Italic = true;
            ws.Cell(signNoteRow, 6).Style.Font.FontSize = 9;
            ws.Cell(signNoteRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Thiết lập độ rộng cột (Column Widths)
            ws.Column(1).Width = 5.5;  // Cột # (STT)
            ws.Column(2).Width = 44.0; // Cột Thông tin báo cáo
            ws.Column(2).Style.Alignment.WrapText = true;

            // Các cột số liệu từ C đến H (Tổng, PUD, PLHIV, TG, SW, MSM) có khoảng cách bằng nhau tuyệt đối
            for (int c = 3; c <= 8; c++)
            {
                ws.Column(c).Width = 10.0;
            }

            // Cấu hình trang in chuẩn A4 dọc vừa vặn trong 1 trang ngang
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.Margins.Left = 0.4;
            ws.PageSetup.Margins.Right = 0.4;
            ws.PageSetup.Margins.Top = 0.6;
            ws.PageSetup.Margins.Bottom = 0.6;
        }

        private void BuildHoatDongCD45Worksheet(IXLWorksheet ws, List<BaoCaoCD45Model> data, string fromDate, string toDate, string tenTinh, string tenNhom, string xungDanh = "Nhóm")
        {
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG TỔNG HỢP - DỰ ÁN CD45 (DREAMH)";
            ws.Range("A1:H1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 14;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string prefixText = !string.IsNullOrWhiteSpace(xungDanh) ? xungDanh.Trim() : "Nhóm";
            ws.Cell("A2").Value = $"Kỳ báo cáo: Từ {fromDate} đến {toDate} | Tỉnh/Thành: {tenTinh} | {prefixText}: {tenNhom}";
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

            Action<IXLCell, int?> setVal = (c, val) =>
            {
                if (val.HasValue && val.Value > 0)
                {
                    c.Value = val.Value;
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    c.Value = "-";
                    c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            };

            int row = 5;
            if (data != null)
            {
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.STT;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var prefix = item.IndentLevel == 1 ? "      " : "";
                    ws.Cell(row, 2).Value = prefix + item.ChiTieu;

                    if (item.IsBold)
                    {
                        for (int c = 3; c <= 8; c++) ws.Cell(row, c).Value = "";
                        ws.Range(row, 1, row, 8).Style.Font.Bold = true;
                        ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
                    }
                    else
                    {
                        setVal(ws.Cell(row, 3), item.Tong);
                        setVal(ws.Cell(row, 4), item.PUD);
                        setVal(ws.Cell(row, 5), item.PLHIV);
                        setVal(ws.Cell(row, 6), item.TG);
                        setVal(ws.Cell(row, 7), item.SW);
                        setVal(ws.Cell(row, 8), item.MSM);
                    }
                    row++;
                }
            }

            var dataTableRange = ws.Range(4, 1, Math.Max(row - 1, 5), 8);
            dataTableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataTableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Thiết lập độ rộng cột (Column Widths)
            ws.Column(1).Width = 5.5;  // Cột # (STT)
            ws.Column(2).Width = 44.0; // Cột Chỉ tiêu báo cáo
            ws.Column(2).Style.Alignment.WrapText = true;

            // Các cột số liệu từ C đến H (Tổng, PUD, PLHIV, TG, SW, MSM) có khoảng cách bằng nhau tuyệt đối
            for (int c = 3; c <= 8; c++)
            {
                ws.Column(c).Width = 10.0;
            }

            // Cấu hình trang in chuẩn A4 dọc vừa vặn trong 1 trang ngang
            ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.Margins.Left = 0.4;
            ws.PageSetup.Margins.Right = 0.4;
            ws.PageSetup.Margins.Top = 0.6;
            ws.PageSetup.Margins.Bottom = 0.6;
        }
    }
}
