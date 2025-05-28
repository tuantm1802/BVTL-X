using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using OfficeOpenXml.Style;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaoCaoCD43Controller : BaseController
    {
        ICityDA _CityDA = new CityDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        IBaoCaoTongHopDA _BaoCaoTongHopDA = new BaoCaoTongHopDA();
        IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA = new BVTL_NHOM_TBHDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: BaoCaoCD43
        [HasCredential(ControllerName = "BaoCaoCD43")]
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

        public ActionResult BaoCaoQuy()
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

        public ActionResult BaoCaoNam()
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

        public ActionResult BaoCaoHoatDong()
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
        public ActionResult BaoCaoHoatDongNhom()
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
                if (modelSearch.TypeReport == null) {
                    modelSearch.TypeReport = 2;
                };
               
                var data = _BaoCaoTongHopDA.GetDataReportCD43(modelSearch);
                AddLog("Lấy dữ liệu báo cáo quý( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo quý(tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);

                return Json(obj);
            }
        }

        public ActionResult SearchDataBaoCaoHoatDong(ReportSearchModel modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                modelSearch.TypeReport = 2;
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "CD43";
                var data = _BaoCaoTongHopDA.LayDuLieuBaoCaoHoatDongCD43(modelSearch);
                AddLog("Lấy dữ liệu báo cáo tổng hợp quý CD43( từ tháng: " + modelSearch.TuThang +
                                                               ", từ năm: " + modelSearch.TuNam +
                                                                ", đến tháng: " + modelSearch.DenThang +
                                                                 ", đến năm: " + modelSearch.DenNam +
                                                               ", tỉnh: " + modelSearch.CityCodes +
                                                               ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("ERROR - SearchDataBaoCaoHoatDong::SearchData: Lấy dữ liệu báo cáo tổng hợp quý CD43(từ tháng: " + modelSearch.TuThang +
                                                               ", từ năm: " + modelSearch.TuNam +
                                                                ", đến tháng: " + modelSearch.DenThang +
                                                                 ", đến năm: " + modelSearch.DenNam +
                                                               ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);

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
                //var citys = _CityDA.GetCityReport((int)user.UserID);
                var citys = _CityDA.GetAllByCodeMap();
                var duAns = _DuAnDA.GetDuAnReport((int)user.UserID);
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Citys = citys, DuAns = duAns, Error = false, Title = "Lấy dữ liệu thành công." }); ;
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
                        ControllerName = "BaoCaoCD43",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpPost]
        public ActionResult GetNhomTBHByCityCodes(string CityCodes)
        {
            // Lấy danh sách nhóm TBH theo tỉnh
            var nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
            return Json(new { NhomTBHs = nhomTBHs, Error = false, Title = "Lấy dữ liệu thành công." }); ;
        }

        [HttpPost]
        public ActionResult GetNhomTBHByMaNhomMap(string CityCodes)
        {
            // Lấy danh sách nhóm TBH theo tỉnh
            //var nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhomMap(maNhom);
            var nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodesMaDuAn(CityCodes, "CD43");
            return Json(new { NhomTBHs = nhomTBHs, Error = false, Title = "Lấy dữ liệu thành công." }); ;
        }

        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(int Year, string Months, string CityCodes, string quy, string maNhomTBHs, string maDuAn)
        {
            try
            {
                

                var modelSearch = new ReportSearchModel() { 
                    Year = Year, 
                    Months = Months, 
                    CityCodes = CityCodes, 
                    TypeReport = 2, 
                    MaNhomTBH = maNhomTBHs,
                    MaDuAn = maDuAn 
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "CD43";

                var data = _BaoCaoTongHopDA.GetDataReportCD43(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhomMap(maNhomTBHs); 

                var tenDuAn = "";
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);
                var tenNhomTBHs = "";
                //if (nhomTBHs != null && nhomTBHs.Count > 0)
                //{
                //    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                //}
                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_QUY_" + quy+"-"+Year+".xlsx";
                var file_name = maDuAn + "_" + CityCodes + "_BAO_CAO_QUY_" + quy+"-"+Year+".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo quý "+quy+" năm "+Year);
                    var titleReport = "Kỳ báo cáo: Quý " + quy+ " - " + Year;
                    CreateHeader(ws, titleReport, user, tenNhomTBHs);

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
                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    }

                                    if (rowReport.Rowpan > 1)
                                    {
                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                    }
                                }
                                else
                                {
                                    // Thêm dữ liệu cột thông tin BC - thêm
                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                }
                                                                                                
                                // Thêm dữ liệu cột Nam
                                InsertDataCell(ws, "D", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                // Thêm dữ liệu cột Nữ
                                InsertDataCell(ws, "E", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);


                                // Thêm dữ liệu cột Chuyển giới
                                InsertDataCell(ws, "F", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);

                                // Thêm dữ liệu cột Tổng
                                InsertDataCell(ws, "G", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                row++;
                            }
                        }
                    }

                    CreateFooter(ws, titleReport, user, tenNhomTBHs);

                    ws.Range("A5:G" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A5:G" + row).Style.Font.FontSize = 13;
                    ws.Range("A5:G" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        
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

        public ActionResult ExportDataNam(int Year, string Months, string CityCodes, string quy, string maNhomTBHs, string maDuAn)
        {
            try
            {
                

                var modelSearch = new ReportSearchModel() { 
                    Year = Year, 
                    Months = Months, 
                    CityCodes = CityCodes, 
                    TypeReport = 4, // 1: BC tháng, 2: BC quý, 3: BC 6 tháng, 4: BC năm
                    MaNhomTBH = maNhomTBHs,
                    MaDuAn = maDuAn 
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "CD43";

                var data = _BaoCaoTongHopDA.GetDataReportCD43(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhomMap(maNhomTBHs); 

                var tenDuAn = "";
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);
                var tenNhomTBHs = "";
                //if (nhomTBHs != null && nhomTBHs.Count > 0)
                //{
                //    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                //}
                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_QUY_" + quy+"-"+Year+".xlsx";
                
                var file_name = maDuAn + "_BC_NAM_" + Year + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo năm " + Year);
                    var titleReport = "Kỳ báo cáo: Năm " + Year;
                    CreateHeader(ws, titleReport, user, tenNhomTBHs);

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
                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                    }

                                    if (rowReport.Rowpan > 1)
                                    {
                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                    }
                                }
                                else
                                {
                                    // Thêm dữ liệu cột thông tin BC - thêm
                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                }


                                
                                // Thêm dữ liệu cột Nam
                                InsertDataCell(ws, "D", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                // Thêm dữ liệu cột Nữ
                                InsertDataCell(ws, "E", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                // Thêm dữ liệu cột Chuyển giới
                                InsertDataCell(ws, "F", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);

                                // Thêm dữ liệu cột Tổng
                                InsertDataCell(ws, "G", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                
                                row++;
                            }
                        }
                    }

                    CreateFooter(ws, titleReport, user, tenNhomTBHs);

                    ws.Range("A5:G" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A5:G" + row).Style.Font.FontSize = 13;
                    ws.Range("A5:G" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        
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

        public ActionResult ExportDataTongHopQuy(int TuThang, int TuNam, int DenThang, int DenNam, string CityCodes, string maNhomTBHs)
        {
            try
            {
                var maDuAn = "CD43";
                var file_name = maDuAn;

                var modelSearch = new ReportSearchModel() { TuThang = TuThang, TuNam = TuNam, DenThang = DenThang, DenNam = DenNam, CityCodes = CityCodes, TypeReport = 2, MaNhomTBH = maNhomTBHs };

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "CD43";


                var data = _BaoCaoTongHopDA.LayDuLieuBaoCaoHoatDongCD43(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                {
                    //nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                }
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhomMap(maNhomTBHs);

                var tenDuAn = "";
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);
                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                    file_name += "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh));
                }

                var tuQuy = "I";
                switch (TuThang)
                {
                    case 1:
                        tuQuy = "I";
                        break;
                    case 4:
                        tuQuy = "II";
                        break;
                    case 7:
                        tuQuy = "III";
                        break;
                    case 10:
                        tuQuy = "IV";
                        break;
                }
                var denQuy = "I";
                switch (DenThang)
                {
                    case 3:
                        denQuy = "I";
                        break;
                    case 6:
                        denQuy = "II";
                        break;
                    case 9:
                        denQuy = "III";
                        break;
                    case 12:
                        denQuy = "IV";
                        break;
                }


                file_name += "_BC_TH_QUY_TU-" + tuQuy + "-" + TuNam + "_DEN-" + denQuy + "-" + DenNam + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Tổng hợp BC theo HĐ");
                    var titleReport = "Kỳ báo cáo: Từ quý " + tuQuy + " - " + TuNam + " đến quý " + denQuy + " - " + DenNam;

                    List<ListQuyModel> listQuy = new List<ListQuyModel>();

                    if (data != null && data.Count > 0)
                        listQuy = data.FirstOrDefault().ListQuy;

                    CreateHeaderTongHopQuy(ws, titleReport, tenNhomTBHs, listQuy);

                    var row = 6;
                    if (data.Any())
                    {
                        var startColumn = ExcelColumnNameToNumber("E");
                        var boldText = false;
                        foreach (var rowReport in data)
                        {
                            boldText = rowReport.BoldText;
                            // Thêm dữ liệu cột STT
                            InsertDataCell(ws, "A", row, rowReport.STT, boldText, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                            // Thêm dữ liệu cột Hoạt động
                            InsertDataCell(ws, "B", row, rowReport.HoatDong, boldText, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                            // Thêm dữ liệu cột chỉ tiêu
                            InsertDataCell(ws, "C", row, rowReport.ChiTieu, boldText, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                            // Thêm dữ liệu cột đơn vị
                            InsertDataCell(ws, "D", row, rowReport.DonVi, boldText, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                            // Thêm dữ liệu cột quý
                            if (rowReport.ListQuy != null && rowReport.ListQuy.Count > 0)
                            {
                                startColumn = ExcelColumnNameToNumber("E");
                                foreach (var quy in rowReport.ListQuy)
                                {
                                    InsertDataCell(ws, GetExcelColumnName(startColumn), row, quy.SoLuong > 0 ? quy.SoLuong.ToString() : "", boldText, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, quy.SoLuong > 0 ? true : false);
                                    startColumn++;
                                }
                                InsertDataCell(ws, GetExcelColumnName(startColumn), row, rowReport.TyLe > 0 ? rowReport.TyLe.ToString() : "", boldText, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.TyLe > 0 ? true : false);
                            }
                            else
                            {
                                InsertDataCell(ws, "E", row, rowReport.TyLe > 0 ? rowReport.TyLe.ToString() : "", boldText, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.TyLe > 0 ? true : false);
                            }

                            row++;
                        }
                    }

                    CreateFooterTongHopQuy(ws, titleReport, user, tenNhomTBHs);
                    
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Font.FontSize = 13;
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A5:" + GetExcelColumnName(listQuy.Count + 5) + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(2, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        ws.Column("H").Width = 20;
                        ws.Column("I").Width = 20;
                        ws.Column("J").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(false);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        ws.Column("H").Style.Alignment.SetWrapText(true);
                        ws.Column("I").Style.Alignment.SetWrapText(true);
                        ws.Column("J").Style.Alignment.SetWrapText(true);
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

        private void CreateHeaderTongHopQuy(IXLWorksheet ws, string tileReport, string tenNhomTBHs, List<ListQuyModel> listQuy)
        {
            #region header
            // 
            ws.Cell("A1").Value = "TỔNG HỢP BÁO CÁO THEO CHỈ TIÊU";//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            //ws.Range("A1:J1").Row(1).Merge();
            ws.Row(1).Height = 25;
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            // 
            ws.Cell("A2").Value = tileReport;//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            ws.Row(2).Height = 25;
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 13;

            // 
            if (!string.IsNullOrEmpty(tenNhomTBHs))
                ws.Cell("A3").Value = tenNhomTBHs;
            ws.Row(3).Height = 25;
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            var row = 5;

            //header table
            ws.Cell("A" + row).Value = "STT";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Hoạt động";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Chỉ tiêu";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Đơn vị";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (listQuy != null && listQuy.Count > 0)
            {
                var startColumn = ExcelColumnNameToNumber("E");
                foreach (var quy in listQuy)
                {
                    ws.Cell(GetExcelColumnName(startColumn) + row).Value = quy.Quy;//"Quý " + quy.Quy + "/" + quy.Nam;
                    ws.Cell(GetExcelColumnName(startColumn) + row).Style.Font.Bold = true;
                    ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.WrapText = true;
                    ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    startColumn++;
                }
                ws.Cell(GetExcelColumnName(startColumn) + row).Value = "Tỷ lệ";
                ws.Cell(GetExcelColumnName(startColumn) + row).Style.Font.Bold = true;
                ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.WrapText = true;
                ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(GetExcelColumnName(startColumn) + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            else
            {
                ws.Cell("E" + row).Value = "Tỷ lệ";
                ws.Cell("E" + row).Style.Font.Bold = true;
                ws.Cell("E" + row).Style.Alignment.WrapText = true;
                ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            #endregion
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
                ws.Cell(cellName + row).Style.NumberFormat.Format = "#,##0";//#,##0.00
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
        private void CreateHeader(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "BÁO CÁO QUÝ DỰ ÁN CD43";
            ws.Range("A1:F1").Row(1).Merge();
            ws.Row(1).Height = 30;
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
            ws.Row(1).Height = 40;
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

            ws.Cell("B" + row).Value = "Thông tin báo cáo";
            ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Nam";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Nữ";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Chuyển giới";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            ws.Cell("G" + row).Value = "Tổng";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion
        }
        private void CreateFooter(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            ws.Cell("B63").Value = "Trưởng nhóm";
            ws.Cell("B63").Style.Font.Bold = true;
            ws.Cell("B63").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B63").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B63").Style.Font.FontName = "Times New Roman";
            ws.Cell("B63").Style.Font.FontSize = 13;

            ws.Cell("C63").Value = "Cán bộ dự án";
            ws.Range("C63:D63").Row(1).Merge();
            ws.Cell("C63").Style.Font.Bold = true;
            ws.Cell("C63").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C63").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("C63").Style.Font.FontName = "Times New Roman";
            ws.Cell("C63").Style.Font.FontSize = 13;

            ws.Cell("F63").Value = "Quản lý chương trình/Điều phối";
            ws.Range("F63:F63").Row(1).Merge();
            ws.Cell("F63").Style.Font.Bold = true;
            ws.Cell("F63").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F63").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("F63").Style.Font.FontName = "Times New Roman";
            ws.Cell("F63").Style.Font.FontSize = 13;

        }

        private void CreateFooterTongHopQuy(IXLWorksheet ws, string tileReport, UserLogin user, string tenNhomTBHs)
        {
            ws.Cell("B35").Value = "Trưởng nhóm";
            ws.Cell("B35").Style.Font.Bold = true;
            ws.Cell("B35").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B35").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B35").Style.Font.FontName = "Times New Roman";
            ws.Cell("B35").Style.Font.FontSize = 13;

            ws.Cell("C35").Value = "Cán bộ dự án";
            ws.Range("C35:D35").Row(1).Merge();
            ws.Cell("C35").Style.Font.Bold = true;
            ws.Cell("C35").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C35").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("C35").Style.Font.FontName = "Times New Roman";
            ws.Cell("C35").Style.Font.FontSize = 13;

            ws.Cell("E35").Value = "Quản lý chương trình/Điều phối";
            ws.Range("E35:F35").Row(1).Merge();
            ws.Cell("E35").Style.Font.Bold = true;
            ws.Cell("E35").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E35").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("E35").Style.Font.FontName = "Times New Roman";
            ws.Cell("E35").Style.Font.FontSize = 13;

        }
        #endregion 

    }
}