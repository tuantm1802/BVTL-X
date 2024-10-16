using System.Collections.Generic;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;

namespace Data.InterfaceDA.Admin
{
    public interface IBaoCaoTongHopDA
    {

        
        /// <summary>
        /// Lấy kết quả ACE theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<BaoCaoModel> GetDataReport(ReportSearchModel modelSearch);
        List<BaoCaoModel> GetDataReportVIIV(ReportSearchModel modelSearch);
        List<BaoCaoModel> GetDataYearReportVIIV(ReportSearchModel modelSearch);
        List<BaoCaoModel> GetDataReportChiSo(ReportSearchModel modelSearch);

        List<BaoCaoModel> GetDataReportCD43(ReportSearchModel modelSearch);
        List<BaoCaoModel> GetDataReportCH07(ReportSearchModel modelSearch);
        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<BaoCaoTongHopPageModel> GetBaoCaoTongHopByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         BaoCaoTongHopPageModel GetItemById(int id);

        /// <summary>
        /// Lấy dữ liệu kết quả sáng lọc
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         void KetQuaSangLoc(ReportSearchModel modelSearch, ref List<KetQuaSangLocModel> DoiTuongKHs, ref List<KetQuaSangLocModel> GioiTinhs, ref List<KetQuaSangLocModel> Tuois
            , ref List<KetQuaSangLocModel> KetQuaHIVs, ref List<KetQuaSangLocModel> ChatGayNghien3Thangs, ref List<KetQuaSangLocModel> SoChatGayNghiens, ref List<KetQuaSangLocModel> ChatGayNghienSDThuongXuyens
            , ref List<DuongSuDungMaTuyDaModel> DuongSDMaTuyDas, ref List<TanSuatSuDungMaTuyDaModel> TanSuatSDMaTuyDas, ref List<LanDauSuDungMaTuyDaModel> LanDauSDMaTuyDas, ref List<LoaiMaTuyDaSuDungDTModel> LoaiMaTuyDaSDDauTiens
            , ref List<NguyCoKhiSDMTDModel> NguyCoSDMaTuyDas, ref List<ChungBKTModel> ChungBKTs, ref List<NguyCoTinhDucModel> NguyCoTinhDucs, ref List<DungBCSModel> DungBCSs
            , ref List<SuDungMTDKhiQHTDModel> SDMaTuyDaKhiQHTDs, ref List<SuDungMTDKhiQHTDModel> QHTDTapThes, ref List<SuDungMTDKhiQHTDModel> BanDams, ref List<NhieuNguyCoTinhDucModel> NhieuNguycoTinhDucs
            , ref List<BenhSTIModel> BenhSTIs, ref List<BenhLao_VGCModel> BenhLaos, ref List<BenhLao_VGCModel> BenhVGCs, ref List<KetQuaSangLocModel> SocHeroins
            , ref List<KetQuaSangLocModel> SocMeths, ref List<CacChatGayNghienModel> CacLoaiChatGayNghiens, ref List<KetQuaSangLocModel> KetQuaQSTs, ref List<MucDoGapVanDeSKTTModel> MucDoGapVanDeSKTTs
            , ref List<SuDungMTDKhiQHTDModel> TuLamHaiBanThans, ref List<SuDungMTDKhiQHTDModel> CoTuSats, ref List<LoanThanModel> LoanThans);


        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp quý VIIV
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
         List<BaoCaoTongHopQuyVIIVModel> LayDuLieuBaoCaoTongHopQuyVIIV(ReportSearchModel modelSearch);
    }
}
