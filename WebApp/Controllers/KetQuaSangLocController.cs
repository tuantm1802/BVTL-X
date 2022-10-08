using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KetQuaSangLocController : BaseController
    {
        IBaoCaoTongHopDA _KetQuaSangLocDA = new BaoCaoTongHopDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: KetQuaSangLoc
        [HasCredential(ControllerName = "KetQuaSangLoc")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAll(ReportSearchModel modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                List<KetQuaSangLocModel> DoiTuongKHs = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> GioiTinhs = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> Tuois = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> KetQuaHIVs = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> ChatGayNghien3Thangs = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> SoChatGayNghiens = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> ChatGayNghienSDThuongXuyens = new List<KetQuaSangLocModel>();
                List<DuongSuDungMaTuyDaModel> DuongSDMaTuyDas = new List<DuongSuDungMaTuyDaModel>();
                List<TanSuatSuDungMaTuyDaModel> TanSuatSDMaTuyDas = new List<TanSuatSuDungMaTuyDaModel>();
                List<LanDauSuDungMaTuyDaModel> LanDauSDMaTuyDas = new List<LanDauSuDungMaTuyDaModel>();
                List<LoaiMaTuyDaSuDungDTModel> LoaiMaTuyDaSDDauTiens = new List<LoaiMaTuyDaSuDungDTModel>();
                List<NguyCoKhiSDMTDModel> NguyCoSDMaTuyDas = new List<NguyCoKhiSDMTDModel>();
                List<ChungBKTModel> ChungBKTs = new List<ChungBKTModel>();
                List<NguyCoTinhDucModel> NguyCoTinhDucs = new List<NguyCoTinhDucModel>();
                List<DungBCSModel> DungBCSs = new List<DungBCSModel>();
                List<SuDungMTDKhiQHTDModel> SDMaTuyDaKhiQHTDs = new List<SuDungMTDKhiQHTDModel>();
                List<SuDungMTDKhiQHTDModel> QHTDTapThes = new List<SuDungMTDKhiQHTDModel>();
                List<SuDungMTDKhiQHTDModel> BanDams = new List<SuDungMTDKhiQHTDModel>();
                List<NhieuNguyCoTinhDucModel> NhieuNguycoTinhDucs = new List<NhieuNguyCoTinhDucModel>();
                List<BenhSTIModel> BenhSTIs = new List<BenhSTIModel>();
                List<BenhLao_VGCModel> BenhLaos = new List<BenhLao_VGCModel>();
                List<BenhLao_VGCModel> BenhVGCs = new List<BenhLao_VGCModel>();
                List<KetQuaSangLocModel> SocHeroins = new List<KetQuaSangLocModel>();
                List<KetQuaSangLocModel> SocMeths = new List<KetQuaSangLocModel>();
                List<CacChatGayNghienModel> CacLoaiChatGayNghiens = new List<CacChatGayNghienModel>();
                List<KetQuaSangLocModel> KetQuaQSTs = new List<KetQuaSangLocModel>();
                List<MucDoGapVanDeSKTTModel> MucDoGapVanDeSKTTs = new List<MucDoGapVanDeSKTTModel>();
                List<SuDungMTDKhiQHTDModel> TuLamHaiBanThans = new List<SuDungMTDKhiQHTDModel>();
                List<SuDungMTDKhiQHTDModel> CoTuSats = new List<SuDungMTDKhiQHTDModel>();
                List<LoanThanModel> LoanThans = new List<LoanThanModel>();
                _KetQuaSangLocDA.KetQuaSangLoc(modelSearch, ref DoiTuongKHs, ref GioiTinhs, ref Tuois
             , ref KetQuaHIVs, ref ChatGayNghien3Thangs, ref SoChatGayNghiens, ref ChatGayNghienSDThuongXuyens
             , ref DuongSDMaTuyDas, ref TanSuatSDMaTuyDas, ref LanDauSDMaTuyDas, ref LoaiMaTuyDaSDDauTiens
             , ref NguyCoSDMaTuyDas, ref ChungBKTs, ref NguyCoTinhDucs, ref DungBCSs
             , ref SDMaTuyDaKhiQHTDs, ref QHTDTapThes, ref BanDams, ref NhieuNguycoTinhDucs
             , ref BenhSTIs, ref BenhLaos, ref BenhVGCs, ref SocHeroins
             , ref SocMeths, ref CacLoaiChatGayNghiens, ref KetQuaQSTs, ref MucDoGapVanDeSKTTs
             , ref TuLamHaiBanThans, ref CoTuSats, ref LoanThans);
                AddLog("Lấy dữ liệu theo trang bảng kết quả sàng lọc thành công.");
                return Json(new
                {
                    DoiTuongKHs = DoiTuongKHs,
                    GioiTinhs = GioiTinhs,
                    Tuois = Tuois,
                    KetQuaHIVs = KetQuaHIVs,
                    ChatGayNghien3Thangs = ChatGayNghien3Thangs,
                    SoChatGayNghiens = SoChatGayNghiens,
                    ChatGayNghienSDThuongXuyens = ChatGayNghienSDThuongXuyens,
                    DuongSDMaTuyDas = DuongSDMaTuyDas,
                    TanSuatSDMaTuyDas = TanSuatSDMaTuyDas,
                    LanDauSDMaTuyDas = LanDauSDMaTuyDas,
                    LoaiMaTuyDaSDDauTiens = LoaiMaTuyDaSDDauTiens,
                    NguyCoSDMaTuyDas = NguyCoSDMaTuyDas,
                    ChungBKTs = ChungBKTs,
                    NguyCoTinhDucs = NguyCoTinhDucs,
                    DungBCSs = DungBCSs,
                    SDMaTuyDaKhiQHTDs = SDMaTuyDaKhiQHTDs,
                    QHTDTapThes = QHTDTapThes,
                    BanDams = BanDams,
                    NhieuNguycoTinhDucs = NhieuNguycoTinhDucs,
                    BenhSTIs = BenhSTIs,
                    BenhLaos = BenhLaos,
                    BenhVGCs = BenhVGCs,
                    SocHeroins = SocHeroins,
                    SocMeths = SocMeths,
                    CacLoaiChatGayNghiens = CacLoaiChatGayNghiens,
                    KetQuaQSTs = KetQuaQSTs,
                    MucDoGapVanDeSKTTs = MucDoGapVanDeSKTTs,
                    TuLamHaiBanThans = TuLamHaiBanThans,
                    CoTuSats = CoTuSats,
                    LoanThans = LoanThans
,
                    Error = false,
                    Title = "Lấy dữ liệu thành công."
                }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng kết quả sàng lọc lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả ACE thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả ACE lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "KetQuaSangLoc",
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
                var data = _KetQuaSangLocDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả ACE( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả ACE( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        #region Xuất dữ liệu ra excel
        //[HttpGet]
        //public ActionResult ExportData(string keyword)
        //{
        //    try
        //    {
        //        var user = Session["USER_SESSION"] as UserLogin;
        //        var modelSearch = new ModelSearch
        //        {
        //            KeyWord = keyword == "undefined" ? string.Empty : keyword,
        //            currentPage = 1,
        //            pageSize = int.MaxValue,
        //            SortColumn = "kqslace_id"
        //        };
        //        var data = _KetQuaSangLocDA.GetAllByPage(modelSearch);

        //        string file_name = "KetQuaSangLoc_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

        //        DataTable dt = new DataTable();
        //        dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Tỉnh"),
        //                new DataColumn("Mã nhóm TBH"),
        //                new DataColumn("Tên nhóm TBH"),
        //                new DataColumn("Mã KH"),
        //                 new DataColumn("Họ tên KH"),
        //                 new DataColumn("Ngày số liệu"),
        //                 new DataColumn("Tổng điểm"),
        //                 new DataColumn("Kết quả")

        //        });
        //        foreach (var item in data)
        //        {
        //            dt.Rows.Add(
        //                item.CityName,
        //                item.manhom_tbh,
        //                item.tennhom_tbh,
        //                item.makh,
        //                item.hoten,
        //                item.ngaysltext,
        //                item.tongdiem_ace,
        //                item.ketqua_ace
        //                );
        //        }
        //        using (XLWorkbook wb = new XLWorkbook())
        //        {
        //            var ws = wb.Worksheets.Add("Kết quả ACE");
        //            ws.Cell("A2").Value = "KẾT QUẢ ACE";
        //            ws.Range("A2:H2").Row(1).Merge();
        //            ws.Cell("A2").Style.Font.Bold = true;
        //            ws.Cell("A2").Style.Font.FontSize = 20;
        //            ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //            ws.Column("F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            ws.Column("H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        //            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //            ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //            ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        //            ws.Cell(3, 1).InsertTable(dt);
        //            AddLog("KẾT QUẢ ACE");
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                ws.Columns(1, 10).AdjustToContents();
        //                wb.SaveAs(stream);
        //                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AddLog("Export KẾT QUẢ ACE lỗi: " + ex.Message);
        //        return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion 
    }
}