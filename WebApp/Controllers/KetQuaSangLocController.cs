using ClosedXML.Excel;
using Common;
using Common.Common;
using Common.ICommon;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KetQuaSangLocController : BaseController
    {
        IBaoCaoTongHopDA _BaoCaoTongHopDA = new BaoCaoTongHopDA();
        ICityDA _CityDA = new CityDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        IBVTL_NHOM_TBHDA _NhomTBHDA = new BVTL_NHOM_TBHDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();
        IUltil _Ultil = new Ultil();

        // GET: KetQuaSangLoc
        [HasCredential(ControllerName = "KetQuaSangLoc")]
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

        [HttpPost]
        public ActionResult GetAll(ReportSearchModel modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                //if (modelSearch._FromDate != null)
                //    modelSearch.FromDate = Convert.ToDateTime(modelSearch._FromDate).ToString("yyyyMMdd");
                //if (modelSearch._ToDate != null)
                //    modelSearch.ToDate = Convert.ToDateTime(modelSearch._ToDate).ToString("yyyyMMdd");

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

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

               

                var citys = _CityDA.GetCityReport((int)user.UserID);
                if (citys != null && citys.Count > 0)
                    modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));

                _BaoCaoTongHopDA.KetQuaSangLoc(modelSearch, ref DoiTuongKHs, ref GioiTinhs, ref Tuois
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
                    LoanThans = LoanThans,
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

                // Lấy danh sách tỉnh, tp phố
                var citis = _CityDA.GetAll();

                // Lấy danh sách nhóm tbh
                var nhomTBHs = _NhomTBHDA.GetAll();

                return Json(new { Buttoms = bottoms, Citis = citis, NhomTBHs = nhomTBHs, Error = false, Title = "Lấy dữ liệu thành công." }); ;
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
                var data = _BaoCaoTongHopDA.GetItemById(Id);
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
        [HttpGet]
        public ActionResult ExportData(string FromDate, string ToDate, string CityCodes, string maNhomTBHs)
        {
            try
            {
                
                var modelSearch = new ReportSearchModel() { FromDate = FromDate, ToDate = ToDate, CityCodes = CityCodes, MaNhomTBH = maNhomTBHs };

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

               
                var citys = _CityDA.GetCityReport((int)user.UserID);
                if (citys != null && citys.Count > 0)
                    modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));

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
                _BaoCaoTongHopDA.KetQuaSangLoc(modelSearch, ref DoiTuongKHs, ref GioiTinhs, ref Tuois
             , ref KetQuaHIVs, ref ChatGayNghien3Thangs, ref SoChatGayNghiens, ref ChatGayNghienSDThuongXuyens
             , ref DuongSDMaTuyDas, ref TanSuatSDMaTuyDas, ref LanDauSDMaTuyDas, ref LoaiMaTuyDaSDDauTiens
             , ref NguyCoSDMaTuyDas, ref ChungBKTs, ref NguyCoTinhDucs, ref DungBCSs
             , ref SDMaTuyDaKhiQHTDs, ref QHTDTapThes, ref BanDams, ref NhieuNguycoTinhDucs
             , ref BenhSTIs, ref BenhLaos, ref BenhVGCs, ref SocHeroins
             , ref SocMeths, ref CacLoaiChatGayNghiens, ref KetQuaQSTs, ref MucDoGapVanDeSKTTs
             , ref TuLamHaiBanThans, ref CoTuSats, ref LoanThans);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _NhomTBHDA.GetItemByCityCodes(CityCodes);
                else
                    nhomTBHs = _NhomTBHDA.GetItemByMaNhoms(maNhomTBHs);
                var tenDuAn = "";
                if (!string.IsNullOrEmpty(modelSearch.MaDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(modelSearch.MaDuAn);
                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                }

                var file_name = modelSearch.MaDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_KET_QUA_SANG_LOC_TU_NGAY_" + FromDate + "-" + ToDate + ".xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Từ ngày " + _Ultil.LoadDate(FromDate).Replace("/","-") + "-" + _Ultil.LoadDate(ToDate).Replace("/", "-"));
                    var thoiGianTuyen = "Thời gian tuyển: Từ ngày " + _Ultil.LoadDate(FromDate) + " - " + _Ultil.LoadDate(ToDate);
                    CreateHeader(ws, thoiGianTuyen, tenNhomTBHs);

                    var row = 8;

                    // Thêm dữ liệu DoiTuongKHs
                    row = CreateTableBC_DoiTuongKH( ws, row, DoiTuongKHs);
                    row++;

                    // Thêm dữ liệu GioiTinhs
                    row = CreateTableBC_GioiTinh(ws, row, GioiTinhs);
                    row++;

                    // Thêm dữ liệu Tuois
                    row = CreateTableBC_Tuoi(ws, row, Tuois);
                    row++;

                    // Thêm dữ liệu KetQuaHIVs
                    row = CreateTableBC_KQ_HIV(ws, row, KetQuaHIVs);
                    row++;

                    // Thêm dữ liệu ChatGayNghien3Thangs
                    row = CreateTableBC_ChatGayNghien3Thang(ws, row, ChatGayNghien3Thangs);
                    row++;

                    // Thêm dữ liệu SoChatGayNghiens
                    row = CreateTableBC_SoChatGayNghien(ws, row, SoChatGayNghiens);
                    row++;

                    // Thêm dữ liệu ChatGayNghienSDThuongXuyens
                    row = CreateTableBC_ChatGayNghienSDThuongXuyen(ws, row, ChatGayNghienSDThuongXuyens);
                    row++;

                    // Thêm dữ liệu DuongSDMaTuyDas
                    row = CreateTableBC_DuongSDMaTuyDa(ws, row, DuongSDMaTuyDas);
                    row++;

                    // Thêm dữ liệu TanSuatSDMaTuyDas
                    row = CreateTableBC_TanSuatSDMaTuyDa(ws, row, TanSuatSDMaTuyDas);
                    row++;

                    // Thêm dữ liệu LanDauSDMaTuyDas
                    row = CreateTableBC_LanDauSDMaTuyDa(ws, row, LanDauSDMaTuyDas);
                    row++;

                    // Thêm dữ liệu LoaiMaTuyDaSDDauTiens
                    row = CreateTableBC_LoaiMaTuyDaSDDauTien(ws, row, LoaiMaTuyDaSDDauTiens);
                    row++;

                    // Thêm dữ liệu NguyCoSDMaTuyDas
                    row = CreateTableBC_NguyCoSDMaTuyDa(ws, row, NguyCoSDMaTuyDas);
                    row++;

                    // Thêm dữ liệu ChungBKTs
                    row = CreateTableBC_ChungBKT(ws, row, ChungBKTs);
                    row++;

                    // Thêm dữ liệu NguyCoTinhDucs
                    row = CreateTableBC_NguyCoTinhDuc(ws, row, NguyCoTinhDucs);
                    row++;

                    // Thêm dữ liệu DungBCSs
                    row = CreateTableBC_DungBCS(ws, row, DungBCSs);
                    row++;

                    // Thêm dữ liệu SDMaTuyDaKhiQHTDs
                    row = CreateTableBC_SDMaTuyDaKhiQHTD(ws, row, SDMaTuyDaKhiQHTDs);
                    row++;

                    // Thêm dữ liệu QHTDTapThes
                    row = CreateTableBC_QHTDTapThe(ws, row, QHTDTapThes);
                    row++;

                    // Thêm dữ liệu BanDams
                    row = CreateTableBC_BanDam(ws, row, BanDams);
                    row++;

                    // Thêm dữ liệu NhieuNguycoTinhDucs
                    row = CreateTableBC_NhieuNguycoTinhDuc(ws, row, NhieuNguycoTinhDucs);
                    row++;

                    // Thêm dữ liệu BenhSTIs
                    row = CreateTableBC_BenhSTI(ws, row, BenhSTIs);
                    row++;

                    // Thêm dữ liệu BenhLaos
                    row = CreateTableBC_BenhLao(ws, row, BenhLaos);
                    row++;

                    // Thêm dữ liệu BenhVGCs
                    row = CreateTableBC_BenhVGC(ws, row, BenhVGCs);
                    row++;

                    // Thêm dữ liệu SocHeroins
                    row = CreateTableBC_SocHeroin(ws, row, SocHeroins);
                    row++;

                    // Thêm dữ liệu SocMeths
                    row = CreateTableBC_SocMeth(ws, row, SocMeths);
                    row++;

                    // Thêm dữ liệu CacLoaiChatGayNghiens
                    row = CreateTableBC_CacLoaiChatGayNghien(ws, row, CacLoaiChatGayNghiens);
                    row++;

                    // Thêm dữ liệu KetQuaQSTs
                    row = CreateTableBC_KetQuaQST(ws, row, KetQuaQSTs);
                    row++;

                    // Thêm dữ liệu MucDoGapVanDeSKTTs
                    row = CreateTableBC_MucDoGapVanDeSKTT(ws, row, MucDoGapVanDeSKTTs);
                    row++;

                    // Thêm dữ liệu TuLamHaiBanThans
                    row = CreateTableBC_TuLamHaiBanThan(ws, row, TuLamHaiBanThans);
                    row++;

                    // Thêm dữ liệu CoTuSats
                    row = CreateTableBC_CoTuSat(ws, row, CoTuSats);
                    row++;

                    // Thêm dữ liệu LoanThans
                    row = CreateTableBC_LoanThan(ws, row, LoanThans);
                    row++;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        ws.Column("H").Width = 20;
                        ws.Column("I").Width = 20;
                        ws.Column("J").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        ws.Column("H").Style.Alignment.SetWrapText(true);
                        ws.Column("I").Style.Alignment.SetWrapText(true);
                        ws.Column("J").Style.Alignment.SetWrapText(true);
                        ws.Column("K").Style.Alignment.SetWrapText(true);
                        ws.Column("L").Style.Alignment.SetWrapText(true);
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
        private void CreateHeader(IXLWorksheet ws, string thoiGianTuyen, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "THÔNG TIN SÀNG LỌC KHÁCH HÀNG TUYỂN VÀO DỰ ÁN";
            ws.Range("A1:J1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            // 
            ws.Cell("A3").Value = thoiGianTuyen;
            ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            // 
            ws.Cell("A4").Value = tenNhomTBHs;
            ws.Range("A4:J4").Row(1).Merge();
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A4").Style.Font.FontName = "Times New Roman";
            ws.Cell("A4").Style.Font.FontSize = 13;

            //// 
            //ws.Cell("A5").Value = tenNhomTBHs;
            //ws.Range("A5:J5").Row(1).Merge();
            //ws.Cell("A5").Style.Font.Bold = true;
            //ws.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell("A5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //ws.Cell("A5").Style.Font.FontName = "Times New Roman";
            //ws.Cell("A5").Style.Font.FontSize = 13;

            #endregion
        }

        /// <summary>
        /// Tạo dữ liệu table Đối tượng khách hàng
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_DoiTuongKH(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Đối tượng KH";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Đối tượng KH";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A"+(row - data.Count() -2) +":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table giới tính
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_GioiTinh(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Giới tính";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Giới tính";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Tuổi
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_Tuoi(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Tuổi";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Tuổi";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Kết quả sàng lọc HIV
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_KQ_HIV(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Kết quả sàng lọc HIV";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Kết quả";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Chất gây nghiện sử dụng trong 3 tháng gần đây
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_ChatGayNghien3Thang(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Chất gây nghiện sử dụng trong 3 tháng gần đây";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Chất gây nghiện sử dụng trong 3 tháng gần đây";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Tỉ lệ % người dùng từng loại";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Số Chất gây nghiện sử dụng trong 3 tháng gần đây
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_SoChatGayNghien(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Số Chất gây nghiện sử dụng trong 3 tháng gần đây";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Tỉ lệ % người dùng từng loại";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Loại chất gây nghiện sử dụng thường xuyên nhất trong 3 tháng gần đây
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_ChatGayNghienSDThuongXuyen(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Loại chất gây nghiện sử dụng thường xuyên nhất trong 3 tháng gần đây";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Loại chất gây nghiện sử dụng thường xuyên nhất trong 3 tháng gần đây";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Tỉ lệ % người dùng từng loại";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Đường sử dụng ma túy đá
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_DuongSDMaTuyDa(IXLWorksheet ws, int row, List<DuongSuDungMaTuyDaModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Đường sử dụng ma túy đá";
            ws.Range("A" + row + ":F" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "1, Hút, hít (khói, mùi) bằng mũi/miệng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "2, Hít ở dạng bột bằng mũi";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "3, Uống, nuốt";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "4, Tiêm chích";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "5, KB/KTL";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.HutHit/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.DangBot/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.UongNuot/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport.TiemChich/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.HutHit.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.DangBot.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.UongNuot.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport.TiemChich.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);
                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Tần suất sử dụng ma túy đá trong 3 tháng gần đây
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_TanSuatSDMaTuyDa(IXLWorksheet ws, int row, List<TanSuatSuDungMaTuyDaModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Tần suất sử dụng ma túy đá trong 3 tháng gần đây";
            ws.Range("A" + row + ":I" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Tần suất";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "1, Vài lần 1 ngày";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "2, Hàng ngày";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "3, Vài lần 1 tuần";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "4, Theo đợt vài ngày liên tục rồi tạm nghỉ";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "5, Chỉ dùng cuối tuần/ dịp đặc biệt";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "6, Vài lần trong 1 tháng";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "7, Ít hơn 1 lần/tháng";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.WrapText = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I" + row).Value = "8, KB/KTL";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Style.Alignment.WrapText = true;
            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.VaiLan1Ngay/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.HangNgay/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.VaiLan1Tuan/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport.VaiNgayRoiTamNghi/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, (rowReport.DungCuoiTuan/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, (rowReport.VaiLan1Thang/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, (rowReport.ItHon1Lan1Thang/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "I", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.VaiLan1Ngay.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.HangNgay.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.VaiLan1Tuan.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport.VaiNgayRoiTamNghi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, rowReport.DungCuoiTuan.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, rowReport.VaiLan1Thang.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, rowReport.ItHon1Lan1Thang.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "I", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);
                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":I" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Lần đầu SD ma tuý
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_LanDauSDMaTuyDa(IXLWorksheet ws, int row, List<LanDauSuDungMaTuyDaModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Lần đầu SD ma tuý";
            ws.Range("A" + row + ":L" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Tuổi";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "13";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "14";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "15";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "16";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "17";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "18";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "19";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.WrapText = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I" + row).Value = "20";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Style.Alignment.WrapText = true;
            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("J" + row).Value = "21";
            ws.Cell("J" + row).Style.Font.Bold = true;
            ws.Cell("J" + row).Style.Alignment.WrapText = true;
            ws.Cell("J" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("K" + row).Value = "22";
            ws.Cell("K" + row).Style.Font.Bold = true;
            ws.Cell("K" + row).Style.Alignment.WrapText = true;
            ws.Cell("K" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("K" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("L" + row).Value = "23";
            ws.Cell("L" + row).Style.Font.Bold = true;
            ws.Cell("L" + row).Style.Alignment.WrapText = true;
            ws.Cell("L" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("L" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport._13/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport._14/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport._15/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport._16/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, (rowReport._17/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, (rowReport._18/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, (rowReport._19/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "I", row, (rowReport._20/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "J", row, (rowReport._21/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "K", row, (rowReport._22/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "L", row, (rowReport._23/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport._13.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport._14.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport._15.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport._16.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, rowReport._17.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, rowReport._18.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, rowReport._19.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "I", row, rowReport._20.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "J", row, rowReport._21.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "K", row, rowReport._22.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "L", row, rowReport._23.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);
                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":L" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Loại ma túy SD đầu tiên
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_LoaiMaTuyDaSDDauTien(IXLWorksheet ws, int row, List<LoaiMaTuyDaSuDungDTModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Loại ma túy SD đầu tiên";
            ws.Range("A" + row + ":H" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Loại ma túy SD đầu tiên";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Đá";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Keo";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Cần, cỏ";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Ketamin";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Bóng cười";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "Heroin";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Các chất hít";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.WrapText = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Da/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Keo/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.CanCo/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport.Ketamin/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, (rowReport.BongCuoi/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, (rowReport.Heroin/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, (rowReport.CacChatHit/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Da.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Keo.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.CanCo.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport.Ketamin.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, rowReport.BongCuoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "G", row, rowReport.Heroin.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "H", row, rowReport.CacChatHit.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":H" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Nguy cơ khi SD ma túy
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_NguyCoSDMaTuyDa(IXLWorksheet ws, int row, List<NguyCoKhiSDMTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Nguy cơ khi SD ma túy";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Tiêm chích";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Chưa bao giờ";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Đã từng tiêm chích";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Vẫn đang tiêm chích";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.ChuaBaoGio/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.DaTungTiemChich/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.VanDangTiemChich/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);


                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.ChuaBaoGio.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.DaTungTiemChich.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.VanDangTiemChich.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);


                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Chung BKT
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_ChungBKT(IXLWorksheet ws, int row, List<ChungBKTModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Chung BKT";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Chung BKT";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Đã từng dùng chung";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Chưa bao giờ";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.DaTungDungChung/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.ChuaBaoGio/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.DaTungDungChung.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.ChuaBaoGio.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Nguy cơ tình dục
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_NguyCoTinhDuc(IXLWorksheet ws, int row, List<NguyCoTinhDucModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Loại ma túy SD đầu tiên";
            ws.Range("A" + row + ":E" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "QHTD";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Chưa bao giờ";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Đồng giới";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Khác giới";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Cả hai";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.ChuaBaoGio/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.DongGioi/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KhacGioi/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport.CaHai/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);


                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.ChuaBaoGio.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.DongGioi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KhacGioi.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport.CaHai.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);


                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Dùng BCS
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_DungBCS(IXLWorksheet ws, int row, List<DungBCSModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Dùng BCS";
            ws.Range("A" + row + ":F" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Dùng BCS";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Luôn luôn";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Thường xuyên";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Thỉnh thoảng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Hiếm khi";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Không bao giờ";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.LuonLuon/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.ThuongXuyen/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.ThiThoang/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, (rowReport.HiemKhi/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, (rowReport.KhongBaoGio/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.LuonLuon.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.ThuongXuyen.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.ThiThoang.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "E", row, rowReport.HiemKhi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "F", row, rowReport.KhongBaoGio.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":F" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table SD ma túy khi QHTD
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_SDMaTuyDaKhiQHTD(IXLWorksheet ws, int row, List<SuDungMTDKhiQHTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "SD ma túy khi QHTD";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "SD ma túy khi QHTD";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Có";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Không";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "KB/KTL";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Co/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Khong/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Co.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Khong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table QHTD tập thể
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_QHTDTapThe(IXLWorksheet ws, int row, List<SuDungMTDKhiQHTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "QHTD tập thể";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "QHTD tập thể";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Có";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Không";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "KB/KTL";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Co/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Khong/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Co.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Khong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Bán dâm
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_BanDam(IXLWorksheet ws, int row, List<SuDungMTDKhiQHTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Bán dâm";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Bán dâm";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Có";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Không";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "KB/KTL";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Co/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Khong/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Co.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Khong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Nhiều nguy cơ tình dục (Qhđồng giới, ko thường xuyên sử dụng BCS, QH tập thể, QH khi sd ma túy, bán dâm)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_NhieuNguycoTinhDuc(IXLWorksheet ws, int row, List<NhieuNguyCoTinhDucModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Nhiều nguy cơ tình dục (Qhđồng giới, ko thường xuyên sử dụng BCS, QH tập thể, QH khi sd ma túy, bán dâm)";
            ws.Range("A" + row + ":E" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Nhiều nguy cơ tình dục";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "1 nguy cơ";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "2 nguy cơ";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "3 nguy cơ";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "4-5 nguy cơ";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Mot/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Hai/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.Ba/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        InsertDataCell(ws, "E", row, (rowReport.BonNam/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Mot.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Hai.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.Ba.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        InsertDataCell(ws, "E", row, rowReport.BonNam.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":E" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Bệnh tật STI
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_BenhSTI(IXLWorksheet ws, int row, List<BenhSTIModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Bệnh tật STI";
            ws.Range("A" + row + ":G" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "STI";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Lậu";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Sùi mào gà";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Không mắc";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Khác";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "KB/KTL";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "Giang mai";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Lau/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.SuiMaoGa/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KhongMac/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        InsertDataCell(ws, "E", row, (rowReport.Khac/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        InsertDataCell(ws, "F", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                        InsertDataCell(ws, "G", row, (rowReport.GiangMai/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Lau.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.SuiMaoGa.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KhongMac.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        InsertDataCell(ws, "E", row, rowReport.Khac.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        InsertDataCell(ws, "F", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                        InsertDataCell(ws, "G", row, rowReport.GiangMai.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Bệnh tật Lao
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_BenhLao(IXLWorksheet ws, int row, List<BenhLao_VGCModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Bệnh tật Lao";
            ws.Range("A" + row + ":E" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Lao";
            ws.Range("A" + row + ":A" + row + 1).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Hiện tại";
            ws.Range("B" + row + ":C" + row).Row(1).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            ws.Cell("D" + row).Value = "Qúa khứ";
            ws.Range("D" + row + ":E" + row).Row(1).Merge();
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            row++;
            ws.Cell("B" + row).Value = "Số lượng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Số lượng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "%";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.HienTai_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.HienTai_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "D", row, rowReport.QuaKhu_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "E", row, (rowReport.QuaKhu_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Bệnh tật VGC
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_BenhVGC(IXLWorksheet ws, int row, List<BenhLao_VGCModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Bệnh tật VGC";
            ws.Range("A" + row + ":E" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "VGC";
            ws.Range("A" + row + ":A" + row + 1).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Hiện tại";
            ws.Range("B" + row + ":C" + row).Row(1).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
           
            ws.Cell("D" + row).Value = "Qúa khứ";
            ws.Range("D" + row + ":E" + row).Row(1).Merge();
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
           
            row++;
            ws.Cell("B" + row).Value = "Số lượng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Số lượng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "%";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.HienTai_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.HienTai_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "D", row, rowReport.QuaKhu_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "E", row, (rowReport.QuaKhu_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":E" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Sốc thuốc Heroin
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_SocHeroin(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Sốc thuốc Heroin";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Heroin";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Sốc thuốc Meth
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_SocMeth(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Sốc thuốc Meth";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Meth";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }



        /// <summary>
        /// Tạo dữ liệu table Các loại chất gân nghiện/kết quả ASSIST
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_CacLoaiChatGayNghien(IXLWorksheet ws, int row, List<CacChatGayNghienModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Các loại chất gân nghiện/kết quả ASSIST";
            ws.Range("A" + row + ":H" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "";
            ws.Range("A" + row + ":A" + row + 1).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Nguy cơ thấp (0-3)";
            ws.Range("B" + row + ":C" + row).Row(1).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Nguy cơ trung bình (4-27)";
            ws.Range("D" + row + ":E" + row).Row(1).Merge();
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Nguy cơ cao (27+)";
            ws.Range("F" + row + ":G" + row).Row(1).Merge();
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Tổng";
            ws.Range("H" + row + ":H" + row + 1).Row(1).Merge();
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            row++;
            ws.Cell("B" + row).Value = "Số lượng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Số lượng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "%";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Số lượng";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "%";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.NguyCoThap_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.NguyCoThap_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "D", row, rowReport.NguyCoTrungBinh_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "E", row, (rowReport.NguyCoTrungBinh_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    InsertDataCell(ws, "F", row, rowReport.NguyCoCao_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "G", row, (rowReport.NguyCoCao_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    InsertDataCell(ws, "H", row, rowReport.Tong_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":H" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Kết quả QST (sức khỏe tâm thần)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_KetQuaQST(IXLWorksheet ws, int row, List<KetQuaSangLocModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Kết quả QST (sức khỏe tâm thần)";
            ws.Range("A" + row + ":C" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Kết quả QST";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Số người";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;

                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.SoNguoi.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        /// <summary>
        /// Tạo dữ liệu table Sàng lọc SKTT Mức độ gặp các vấn đề SKTT trong 2 tuần qua (câu 1)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_MucDoGapVanDeSKTT(IXLWorksheet ws, int row, List<MucDoGapVanDeSKTTModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Sàng lọc SKTT Mức độ gặp các vấn đề SKTT trong 2 tuần qua (câu 1)";
            ws.Range("A" + row + ":I" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "";
            ws.Range("A" + row + ":A" + row + 1).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Không chút nào";
            ws.Range("B" + row + ":C" + row).Row(1).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Từ 1 - 7 ngày";
            ws.Range("D" + row + ":E" + row).Row(1).Merge();
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Từ 8 ngày trở lên";
            ws.Range("F" + row + ":G" + row).Row(1).Merge();
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Gần như hàng ngày";
            ws.Range("H" + row + ":I" + row + 1).Row(1).Merge();
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            row++;
            ws.Cell("B" + row).Value = "Số lượng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Số lượng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "%";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Số lượng";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "%";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Số lượng";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I" + row).Value = "%";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.KhongChutNao_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.KhongChutNao_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "D", row, rowReport.Tu1Den7Ngay_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "E", row, (rowReport.Tu1Den7Ngay_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    InsertDataCell(ws, "F", row, rowReport.Tu8NgayTroLen_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "G", row, (rowReport.Tu8NgayTroLen_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    InsertDataCell(ws, "H", row, rowReport.GanNhuHangNgay_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "I", row, (rowReport.GanNhuHangNgay_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":I" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Sàng lọc SKTT Ý nghĩ tự làm hại bản thân trong 2 tuần qua (câu 2)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_TuLamHaiBanThan(IXLWorksheet ws, int row, List<SuDungMTDKhiQHTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Sàng lọc SKTT Ý nghĩ tự làm hại bản thân trong 2 tuần qua (câu 2)";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Ý nghĩ tự làm hại bản thân trong 2 tuần qua (câu 2)";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Có";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Không";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "KB/KTL";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Co/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Khong/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Co.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Khong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Sàng lọc SKTT Cố tự sát từ trước đến nay(câu 3)
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_CoTuSat(IXLWorksheet ws, int row, List<SuDungMTDKhiQHTDModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Sàng lọc SKTT Cố tự sát từ trước đến nay(câu 3)";
            ws.Range("A" + row + ":D" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "Cố tự sát từ trước đến nay (câu 3)";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Có";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "Không";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "KB/KTL";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                var count = 0;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    if (count == 1)
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, (rowReport.Co/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, (rowReport.Khong/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, (rowReport.KBKTL/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                    }
                    else
                    {
                        // Thêm dữ liệu cột Số người
                        InsertDataCell(ws, "B", row, rowReport.Co.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột %
                        InsertDataCell(ws, "C", row, rowReport.Khong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                        // Thêm dữ liệu cột Đối tượng KH
                        InsertDataCell(ws, "D", row, rowReport.KBKTL.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    }

                    count++;
                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 2) + ":D" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }


        /// <summary>
        /// Tạo dữ liệu table Loạn thần
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int CreateTableBC_LoanThan(IXLWorksheet ws, int row, List<LoanThanModel> data)
        {
            #region create header table

            ws.Cell("A" + row).Value = "Loạn thần";
            ws.Range("A" + row + ":I" + row).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A" + row).Style.Font.FontName = "Times New Roman";
            ws.Cell("A" + row).Style.Font.FontSize = 13;
            row++;

            //header table
            ws.Cell("A" + row).Value = "";
            ws.Range("A" + row + ":A" + row + 1).Row(1).Merge();
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Tin rằng có ai đó đang theo dõi, rình rập hoặc làm hại  mình (từ trc đến nay)";
            ws.Range("B" + row + ":C" + row).Row(1).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Tin rằng có ai đó đọc được suy nghĩ hoặc nghe thấy ý nghĩ hoặc có thể đọc hoặc nghe được ý nghĩ của người khác (từ trc đến nay)";
            ws.Range("D" + row + ":E" + row).Row(1).Merge();
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Nghe thấy những thứ mà người khác không nghe thấy (từ trc đến nay)";
            ws.Range("F" + row + ":G" + row).Row(1).Merge();
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            row++;
            ws.Cell("B" + row).Value = "Số lượng";
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "%";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.WrapText = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Số lượng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "%";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Số lượng";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "%";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


            #endregion

            #region Create data
            if (data.Any())
            {
                row++;
                foreach (var rowReport in data)
                {
                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "A", row, rowReport.NoiDung, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Số người
                    InsertDataCell(ws, "B", row, rowReport.TheoDoiRinhRap_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, true);

                    // Thêm dữ liệu cột %
                    InsertDataCell(ws, "C", row, (rowReport.TheoDoiRinhRap_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);

                    // Thêm dữ liệu cột Đối tượng KH
                    InsertDataCell(ws, "D", row, rowReport.YNghi_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "E", row, (rowReport.YNghi_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    InsertDataCell(ws, "F", row, rowReport.NgheThuMaNKKNT_SoLuong.ToString(), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, true);

                    InsertDataCell(ws, "G", row, (rowReport.NgheThuMaNKKNT_PhanTram/100).ToString("P", CultureInfo.InvariantCulture), false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                    row++;
                }
            }

            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Font.FontName = "Times New Roman";
            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Font.FontSize = 13;
            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A" + (row - data.Count() - 3) + ":G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            #endregion
            row++;

            return row;
        }

        #endregion 
    }
}