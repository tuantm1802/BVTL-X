using ClosedXML.Excel;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaoCaoChiSoController : Controller
    {
        readonly ICityDA _CityDA;
        readonly IDuAnDA _DuAnDA;
        readonly IBaoCaoTongHopDA _BaoCaoTongHopDA;
        readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        readonly ISysLogDA _sysLogDA;
        BaseController _helperController = new BaseController();

        public BaoCaoChiSoController(ICityDA cityDA, IDuAnDA duAnDA, IBaoCaoTongHopDA baoCaoTongHopDA, IBVTL_NHOM_TBHDA bVTL_NHOM_TBHDA, ISysLogDA sysLogDA)
        {
            _CityDA = cityDA;
            _DuAnDA = duAnDA;
            _BaoCaoTongHopDA = baoCaoTongHopDA;
            _BVTL_NHOM_TBHDA = bVTL_NHOM_TBHDA;
            _sysLogDA = sysLogDA;
        }

        // GET: BaoCaoChiSo
        [HasCredential(ControllerName = "BaoCaoChiSo")]
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra quyền 
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                if (user.IsAdmin || (duAn != null && user.MaDuAn.Contains(duAn.maduan)))
                    return View();
                else
                    return Redirect("/ErrorPage/Error404");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                return Redirect("/ErrorPage/Error404");
            }
        }
        // GET: BaoCaoChiSo6Thang
        [HasCredential(ControllerName = "BaoCaoChiSo")]
        public ActionResult ChiSo6Thang()
        {
            try
            {
                // Kiểm tra quyền 
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                if (user.IsAdmin || (duAn != null && user.MaDuAn.Contains(duAn.maduan)))
                    return View();
                else
                    return Redirect("/ErrorPage/Error404");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                return Redirect("/ErrorPage/Error404");
            }
        }

        [HttpPost]
        public ActionResult SearchData(ReportSearchModel modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";
                //modelSearch.TypeReport = 5; //-- 1: BC tháng, 2: BC quý, 3: BC 6 tháng, 4: BC năm, 5: Full theo DA
                var data = _BaoCaoTongHopDA.GetDataReportChiSo(modelSearch);
                AddLog("Lấy dữ liệu báo cáo tháng( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo tháng( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);

                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {

                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);

                var user = Session["USER_SESSION"] as UserLogin;
                var citys = _CityDA.GetCityReport((int)user.UserID);
                var duAns = _DuAnDA.GetDuAnReport((int)user.UserID);

                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Citys = citys, DuAns = duAns, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }
        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "BaoCaoThang",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(int Year, string Months, string CityCodes, string maNhomTBHs, string maDuAn)
        {
            try
            {

                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 5, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                var file_name = maDuAn;

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";
                var data = _BaoCaoTongHopDA.GetDataReportChiSo(modelSearch);
                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs)) {
                    //nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                }
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs);

                var tenDuAn = "";
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);

                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                }

                if (!string.IsNullOrEmpty(CityCodes))
                {
                    file_name += "_" + CityCodes;
                }

                string sNhomFilename = "";
                if (nhomTBHs != null && nhomTBHs.Count == 1)
                {
                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh));
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    //file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }

                var lsM = Months.Split(',');
                string sMFilenam = lsM[0];
                if (lsM.Count() > 1)
                {
                    sMFilenam = lsM.First() + "-" + lsM.Last();
                }
                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_SAU_THANG_" + Months.Replace(",", "-") + "-" + Year + ".xlsx";                
                file_name += "_BC_CHI_SO_DA_" + sMFilenam + "-" + Year + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo chỉ số dự án");
                    var titleReport = "Thời gian: từ đầu Dự án đến " + Months + "/" + Year;
                    CreateHeaderDA(ws, titleReport, user, tenNhomTBHs);

                    var columnName = "";
                    var columnNumber = 0;
                    var row = 6;
                    if (data.Any())
                    {

                        foreach (var rowReport in data)
                        {
                            if (rowReport.IsShow == "Y")
                            {
                                // Thêm dữ liệu cột STT
                                InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                                if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                                {
                                    // Thêm dữ liệu cột thông tin BC
                                    InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    if (rowReport.Colpan > 1)
                                    {
                                        columnNumber = ExcelColumnNameToNumber("B");
                                        columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                        ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                    }
                                    else
                                    {
                                        // Thêm dữ liệu cột thông tin BC - thêm
                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    }

                                    if (rowReport.Rowpan > 1)
                                    {
                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                    }
                                }
                                else
                                {
                                    // Thêm dữ liệu cột thông tin BC - thêm
                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                }


                                // Thêm dữ liệu cột Tổng
                                InsertDataCell(ws, "C", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                //// Thêm dữ liệu cột MSM
                                //InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                                //// Thêm dữ liệu cột PUD
                                //InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                                //// Thêm dữ liệu cột SW
                                //InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                                // Thêm dữ liệu cột Nam
                                InsertDataCell(ws, "D", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                // Thêm dữ liệu cột Nữ
                                InsertDataCell(ws, "E", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                // Thêm dữ liệu cột Chuyển giới
                                InsertDataCell(ws, "F", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);


                                row++;
                            }
                        }
                    }

                    CreateFooter(ws, titleReport, user, tenNhomTBHs);
                    ws.Range("A5:F" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A5:F" + row).Style.Font.FontSize = 13;
                    ws.Range("A5:F" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        //ws.Column("E").Width = 20;
                        //ws.Column("F").Width = 20;
                        //ws.Column("G").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        //ws.Column("G").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        //ws.Column("E").Style.Alignment.SetWrapText(true);
                        //ws.Column("F").Style.Alignment.SetWrapText(true);
                        //ws.Column("G").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        //ws.Column("G").Style.Alignment.SetWrapText(true);
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportDataChiSo6Thang(int Year, string Months, string CityCodes, string maNhomTBHs, string maDuAn)
        {
            try
            {

                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 3, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                var file_name = maDuAn;

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";
                var data = _BaoCaoTongHopDA.GetDataReportChiSo(modelSearch);
                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                {
                    //nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                }
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs);

                var tenDuAn = "";
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);

                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                }

                if (!string.IsNullOrEmpty(CityCodes))
                {
                    file_name += "_" + CityCodes;
                }
                
                string sNhomFilename = "";
                if (nhomTBHs != null && nhomTBHs.Count == 1)
                {
                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh));
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    //file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }

                var lsM = Months.Split(',');
                string sMFilenam = lsM[0];
                if (lsM.Count() > 1)
                {
                    sMFilenam = lsM.First() + "-" + lsM.Last();
                }
                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_SAU_THANG_" + Months.Replace(",", "-") + "-" + Year + ".xlsx";                
                file_name += "_BC_CHI_SO_6THANG_" + sMFilenam + "-" + Year + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo 6 tháng " + (Months.Contains("3") ? "đầu" : "cuối") + " năm " + Year);
                    var titleReport = "Kỳ báo cáo: Báo cáo 6 tháng " + (Months.Contains("3") ? "đầu" : "cuối") + " - " + Year;
                    CreateHeaderChiSo6Thang(ws, titleReport, user, tenNhomTBHs);

                    var columnName = "";
                    var columnNumber = 0;
                    var row = 6;
                    if (data.Any())
                    {

                        foreach (var rowReport in data)
                        {
                            if (rowReport.IsShow == "Y")
                            {
                                // Thêm dữ liệu cột STT
                                InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                                if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                                {
                                    // Thêm dữ liệu cột thông tin BC
                                    InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    if (rowReport.Colpan > 1)
                                    {
                                        columnNumber = ExcelColumnNameToNumber("B");
                                        columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                        ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                    }
                                    else
                                    {
                                        // Thêm dữ liệu cột thông tin BC - thêm
                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    }

                                    if (rowReport.Rowpan > 1)
                                    {
                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                    }
                                }
                                else
                                {
                                    // Thêm dữ liệu cột thông tin BC - thêm
                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                }


                                // Thêm dữ liệu cột Tổng
                                InsertDataCell(ws, "C", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                //// Thêm dữ liệu cột MSM
                                //InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                                //// Thêm dữ liệu cột PUD
                                //InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                                //// Thêm dữ liệu cột SW
                                //InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                                // Thêm dữ liệu cột Nam
                                InsertDataCell(ws, "D", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                // Thêm dữ liệu cột Nữ
                                InsertDataCell(ws, "E", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                // Thêm dữ liệu cột Chuyển giới
                                InsertDataCell(ws, "F", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);


                                row++;
                            }
                        }
                    }

                    CreateFooter(ws, titleReport, user, tenNhomTBHs);
                    ws.Range("A5:F" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A5:F" + row).Style.Font.FontSize = 13;
                    ws.Range("A5:F" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:F" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        //ws.Column("G").Width = 20;
                        //ws.Column("H").Width = 20;
                        //ws.Column("I").Width = 20;
                        //ws.Column("J").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        //ws.Column("G").Style.Alignment.SetWrapText(true);
                        //ws.Column("H").Style.Alignment.SetWrapText(true);
                        //ws.Column("I").Style.Alignment.SetWrapText(true);
                        //ws.Column("J").Style.Alignment.SetWrapText(true);
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public static int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }

        /// Gán dữ liệu cho cell
        /// </summary>
        /// <param name="ws"></param>
        private void InsertDataCell(IXLWorksheet ws, string cellName, int row, string value, bool bold,
            XLAlignmentHorizontalValues horizontal, XLAlignmentVerticalValues vertical, bool isNumber = true)
        {
            if (isNumber == true && string.IsNullOrEmpty(value))
                value = "0";

            ws.Cell(cellName + row).Value = isNumber == false ? (object)value : Convert.ToDecimal(value);
            ws.Cell(cellName + row).Style.Font.Bold = bold;
            ws.Cell(cellName + row).Style.Alignment.Horizontal = horizontal;
            ws.Cell(cellName + row).Style.Alignment.Vertical = vertical;
            ws.Cell(cellName + row).Style.Alignment.WrapText = true;
            if (isNumber)
                ws.Cell(cellName + row).Style.NumberFormat.Format = "#,##0";//"#,##0.00"
        }
        /// Gán dữ liệu cho cell có gộp cell
        /// </summary>
        /// <param name="ws"></param>
        private void InsertDataCell_Merge(IXLWorksheet ws, string cellName, int row, string value, bool bold,
            XLAlignmentHorizontalValues horizontal, XLAlignmentVerticalValues vertical, string cellStartMerge, string cellEndMerge)
        {
            ws.Cell(cellName + row).Value = value;
            ws.Cell(cellName + row).Style.Font.Bold = bold;
            ws.Cell(cellName + row).Style.Alignment.Horizontal = horizontal;
            ws.Cell(cellName + row).Style.Alignment.Vertical = vertical;
            ws.Cell(cellName + row).Style.Alignment.WrapText = true;
            //ws.Cell(cellName + row).Style.Alignment.h = true;
            ws.Range(cellStartMerge + ":" + cellEndMerge).Row(1).Merge();
        }

        /// Gán dữ liệu cho cell có gộp cell
        /// </summary>
        /// <param name="ws"></param>
        private void CreateHeaderDA(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "BÁO CÁO CHỈ SỐ DỰ ÁN";
            ws.Range("A1:F1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            // 
            ws.Cell("A2").Value = tileReport;//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            ws.Range("A2:F2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 13;

            // 
            ws.Cell("A3").Value = tenNhomTBHs;
            ws.Range("A3:F3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            var row = 5;

            //header table
            ws.Cell("A" + row).Value = "#";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "People / organizations that directly benefited from the project";
            //ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Number (cumulative since project start - up)";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("E" + row).Value = "MSM";
            //ws.Cell("E" + row).Style.Font.Bold = true;
            //ws.Cell("E" + row).Style.Alignment.WrapText = true;
            //ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("F" + row).Value = "PUD";
            //ws.Cell("F" + row).Style.Font.Bold = true;
            //ws.Cell("F" + row).Style.Alignment.WrapText = true;
            //ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("G" + row).Value = "SW";
            //ws.Cell("G" + row).Style.Font.Bold = true;
            //ws.Cell("G" + row).Style.Alignment.WrapText = true;
            //ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Disaggregated data 1 (Men)";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Disaggregated data 2 (Women)";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Disaggregated data 3 (TG: Transgender)";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion
        }
        private void CreateHeaderChiSo6Thang(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "BÁO CÁO CHỈ SỐ 6 THÁNG";
            ws.Range("A1:F1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            // 
            ws.Cell("A2").Value = tileReport;
            ws.Range("A2:F2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 13;

            // 
            ws.Cell("A3").Value = tenNhomTBHs;
            ws.Range("A3:F3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            var row = 5;

            //header table
            ws.Cell("A" + row).Value = "#";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "People / organizations that directly benefited from the project";
            //ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Number (cumulative since project start - up)";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("E" + row).Value = "MSM";
            //ws.Cell("E" + row).Style.Font.Bold = true;
            //ws.Cell("E" + row).Style.Alignment.WrapText = true;
            //ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("F" + row).Value = "PUD";
            //ws.Cell("F" + row).Style.Font.Bold = true;
            //ws.Cell("F" + row).Style.Alignment.WrapText = true;
            //ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //ws.Cell("G" + row).Value = "SW";
            //ws.Cell("G" + row).Style.Font.Bold = true;
            //ws.Cell("G" + row).Style.Alignment.WrapText = true;
            //ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Disaggregated data 1 (Men)";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Disaggregated data 2 (Women)";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Disaggregated data 3 (TG: Transgender)";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion
        }

        private void CreateFooter(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            ws.Cell("B20").Value = "Leader";
            ws.Cell("B20").Style.Font.Bold = true;
            ws.Cell("B20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B20").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B20").Style.Font.FontName = "Times New Roman";
            ws.Cell("B20").Style.Font.FontSize = 13;

            ws.Cell("C20").Value = "Project Officer";
            ws.Range("C20:D20").Row(1).Merge();
            ws.Cell("C20").Style.Font.Bold = true;
            ws.Cell("C20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C20").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("C20").Style.Font.FontName = "Times New Roman";
            ws.Cell("C20").Style.Font.FontSize = 13;

            ws.Cell("E20").Value = "Program management";
            ws.Range("E20:F20").Row(1).Merge();
            ws.Cell("E20").Style.Font.Bold = true;
            ws.Cell("E20").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E20").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("E20").Style.Font.FontName = "Times New Roman";
            ws.Cell("E20").Style.Font.FontSize = 13;

        }
        #endregion 

    }
}