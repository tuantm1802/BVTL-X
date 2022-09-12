using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KetQuaASSISTController : BaseController
    {
        IKetQuaASSISTDA _KetQuaASSISTDA = new KetQuaASSISTDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: KetQuaASSIST
        [HasCredential(ControllerName = "KetQuaASSIST")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAll(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                var data = _KetQuaASSISTDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng kết quả ASSIST( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng kết quả ASSIST( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả ASSIST thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả ASSIST lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "KetQuaASSIST",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }


        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _KetQuaASSISTDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả ASSIST( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả ASSIST( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(string keyword)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                var modelSearch = new ModelSearch
                {
                    KeyWord = keyword,
                    currentPage = 1,
                    pageSize = int.MaxValue
                };
                var data = _KetQuaASSISTDA.GetAllByPage(modelSearch);

                string file_name = "KetQuaASSIST_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[26] { 
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn(""),
                        new DataColumn("")
                    });
                    foreach (var item in data)
                    {
                        dt.Rows.Add(
                            item.CityName,
                            item.manhom_tbh,
                            item.tennhom_tbh,
                            item.makh,
                            item.hoten,
                            item.ngaysltext,
                            item.thuocla_diem,
                            item.thuocla_nguyco,
                            item.conruou_diem,
                            item.conruou_nguyco,
                            item.cansa_diem,
                            item.cansa_nguyco,
                            item.cocaine_diem,
                            item.cocaine_nguyco,
                            item.matuyda_diem,
                            item.matuyda_nguyco,
                            item.khixonghit_diem,
                            item.khixonghit_nguyco,
                            item.thuocanthan_diem,
                            item.thuocanthan_nguyco,
                            item.chatgayaogiac_diem,
                            item.chatgayaogiac_nguyco,
                            item.thuocphien_diem,
                            item.thuocphien_nguyco,
                            item.chatkhac_diem,
                            item.chatkhac_nguyco
                            );
                    }

                    string _SheetName = "Kết quả ASSIST";
                    string title = "KẾT QUẢ ASSIST";
                    var ck = ExportToExcelTemplate(stream, dt, Server.MapPath("/TemplateExcel/KetQuaASSIST.xlsx"), _SheetName, title);
                    if (ck == 0)
                        return RedirectToAction("Index", "Login");
                    bytes = stream.ToArray();
                }
                AddLog("Export kết quả ASSIST thành công.");
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
            }
            catch (Exception ex)
            {
                AddLog("Export kết quả ASSIST lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        protected virtual int ExportToExcelTemplate(Stream stream, DataTable datatable, string templateFile,
           string sheetName, string title)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            try
            {
                // Mở tệp tin mẫu báo cáo (chứa sẵn phần tiêu đề, chân trang)
                Stream template = new FileStream(templateFile, FileMode.Open);
                using (var xlPackage = new ExcelPackage(stream, template))
                {
                    template.Close();

                    ExcelWorksheet worksheet;
                    if (xlPackage.Workbook.Worksheets.Count > 0)
                    {
                        worksheet = xlPackage.Workbook.Worksheets.First();
                        worksheet.Name = sheetName;
                    }
                    else
                        worksheet = xlPackage.Workbook.Worksheets.Add(sheetName);

                    worksheet.Cells["A2"].Value = title;
                    worksheet.Cells["A5"].LoadFromDataTable(datatable, false);

                    // Lưu file Excel
                    xlPackage.Save();
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }

        }

    }
    #endregion
}
}