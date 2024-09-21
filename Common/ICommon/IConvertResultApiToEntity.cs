using log4net;
using Model.Model;
using Model.ModelExtend.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ICommon
{
    public interface IConvertResultApiToEntity
    {
        /// <summary>
        /// Thêm mới khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int CreateCustomer(BVTL_KHACH_HANG customer);

        /// <summary>
        /// CHuyển đổi kết quả api có report_id = 1344 sang 2 entity BVTL_KQ_SL_SKTT, BVTL_KQ_SL_ASSIST
        /// </summary>
        /// <param name="resultApi1344s"></param>
        /// <param name="sktts"></param>
        /// <param name="assists"></param>
        void ConvertApi1344ToEntity(List<ResultApi1344Model> resultApi1344s, string maDuAn, ref List<BVTL_KQ_SL_SKTT> sktts, ref List<BVTL_KQ_SL_ASSIST> assists);

        /// <summary>
        /// CHuyển đổi kết quả api HIV sang 2 entity BVTL_KQ_XN_HIV
        /// </summary>
        /// <param name="resultApiHIVs"></param>
        /// <param name="hivs"></param>
        void ConvertApiHIVToEntity(List<ResultApiHIVModel> resultApiHIVs, string maDuAn, ref List<BVTL_KQ_XN_HIV> hivs);

        /// <summary>
        /// CHuyển đổi kết quả api ACE sang entity BVTL_KQ_SL_ACE
        /// </summary>
        /// <param name="resultApiACEs"></param>
        /// <param name="aces"></param>
        void ConvertApiACEToEntity(List<ResultApiACEModel> resultApiACEs, string maDuAn, ref List<BVTL_KQ_SL_ACE> aces);

        /// <summary>
        /// CHuyển đổi kết quả api tổng hợp sang entity BVTL_BO_BIEU_MAU_KH_BAO_CAO
        /// </summary>
        /// <param name="resultApiTHs"></param>
        /// <param name="tongHops"></param>
         void ConvertApiTongHopToEntity(List<ResultApiTongHopModel> resultApiTHs, string maDuAn, ref List<BVTL_BO_BIEU_MAU_KH_BAO_CAO> tongHops, ref List<BVTL_KHACH_HANG> khachHangs);

        /// <summary>
        /// CHuyển đổi kết quả api phiếu tư vấn sang entity BVTL_PHIEU_TU_VAN
        /// </summary>
        /// <param name="resultApiPTVs"></param>
        /// <param name="phieuTuVans"></param>
        void ConvertApiPhieuTuVanToEntity(List<ResultApiPhieuTuVanModel> resultApiPTVs, string maDuAn, ref List<BVTL_PHIEU_TU_VAN> phieuTuVans);

        /// <summary>
        /// CHuyển đổi kết quả api chuyển gửi dịch vụ sang entity BVTL_CHUYEN_GUI_DICH_VU
        /// </summary>
        /// <param name="resultApiCGDVs"></param>
        /// <param name="chuyenGuiDVs"></param>
        void ConvertApiChuyenGuiDichVuToEntity(List<ResultApiChuyenGuiDVModel> resultApiCGDVs, string maDuAn, ref List<BVTL_CHUYEN_GUI_DICH_VU> chuyenGuiDVs); 

        /// <summary>
        /// CHuyển đổi kết quả api chuyển gửi dịch vụ sang entity BVTL_CHUYEN_GUI_DICH_VU
        /// </summary>
        /// <param name="resultApiXnNuocTieu"></param>
        /// <param name="xnNuocTieu"></param>
        void ConvertApiXNNuocTieuToEntity(List<ResultApiXnNuocTieuModel> resultApiXnNuocTieu, string maDuAn, ref List<BVTL_KQ_XN_NUOC_TIEU> xnNuocTieu);
        void ConvertApiTTTTToEntity(List<ResultApiTTTTModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_THONG_TIN_TRUYEN_THONG> lsObjDB);
        void ConvertApiBVTLTTTTToEntity(List<ResultApiBVTLTTTTModel> resultApiModels, string maDuAn, string apiCode, ref List<BVTL_THONG_TIN_TRUYEN_THONG> lsObjDB);
        void ConvertApiTrainingDataCollToEntity(List<ResultApiTrainingDataCollModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_TRAINING_DATA_COLLECTION> lsObjDB);
        void ConvertApiDGHLToEntity(List<ResultApiDGHLModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_DANH_GIA_HAI_LONG> lsObjDB);
        void ConvertApiTTKHMaDaToEntity(List<ResultApiTTKHMaDaModel> resultApiModels, string maDuAn, string apiCode, ref List<VIIV_TT_KH_MAT_DAU> lsObjDB);
        void ConvertApiBVTLTHEODAUKHoEntity(List<ResultApiTheoDauKHModel> resultApiModels, string maDuAn, string apiCode, ref List<BVTL_THEO_DAU_KH> lsObjDB);
        void ConvertApiKhachHangTTCBEntity(List<ResultApiKhachHangTTCBModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_THONG_TIN_CO_BAN> lsObjDB);
        void ConvertApiKhachHangSangLocNuocTieuEntity(List<ResultApiKhachHangSangLocNuocTieuModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_SANG_LOC_NUOC_TIEU> lsObjDB);
        void ConvertApiKhachHangSangLocHIVEntity(List<ResultApiKhachHangSangLocHIVModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_SANG_LOC_HIV> lsObjDB);
        void ConvertApiKhachHangDanhGiaHaiLongEntity(List<ResultApiKhachHangDanhGiaHaiLongModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_DANH_GIA_HAI_LONG> lsObjDB);
        void ConvertApiKhachHangTheoDauEntity(List<ResultApiKhachHangTheoDauModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_THEO_DAU> lsObjDB);
        void ConvertApiKhachHangSinhHoatNhomEntity(List<ResultApiKhachHangSinhHoatNhomModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_SINH_HOAT_NHOM> lsObjDB);
        void ConvertApiKhachHangPhieuTuVanEntity(List<ResultApiKhachHangPhieuTuVanModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_PHIEU_TU_VAN> lsObjDB);
        void ConvertApiKhachHangChuyenGuiEntity(List<ResultApiKhachHangChuyenGuiModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_CHUYEN_GUI> lsObjDB);
        void ConvertApiKhachHangHanhViNguyCoEntity(List<ResultApiKhachHangHanhViNguyCoModel> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CD43_KHACH_HANG_HANH_VI_NGUY_CO> lsObjDB);
        void ConvertApiKhachHangTTCBCH07Entity(List<ResultApiKhachHangTTCBCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CH07_KHACH_HANG_THONG_TIN_CO_BAN> lsObjDB);
        void ConvertApiKhachHangSangLocNuocTieuCH07Entity(List<ResultApiKhachHangSangLocNuocTieuCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CH07_KHACH_HANG_SANG_LOC_NUOC_TIEU> lsObjDB);
        void ConvertApiKhachHangSangLocHIVCH07Entity(List<ResultApiKhachHangSangLocHIVCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CH07_KHACH_HANG_SANG_LOC_HIV> lsObjDB);
        void ConvertApiKhachHangPhieuTuVanCH07Entity(List<ResultApiKhachHangPhieuTuVanCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CH07_KHACH_HANG_PHIEU_TU_VAN> lsObjDB);
        void ConvertApiKhachHangChuyenGuiCH07Entity(List<ResultApiKhachHangChuyenGuiDichVuCH07Model> resultApiModels, string maDuAn, string apiCode, string cityCode, ref List<CH07_KHACH_HANG_CHUYEN_GUI> lsObjDB);

    }
}
