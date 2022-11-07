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
    }
}
