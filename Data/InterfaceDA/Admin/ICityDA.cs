using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend;
using Model.ModelExtend.Base;

namespace Data.InterfaceDA.Admin
{
    public interface ICityDA
    {

        /// <summary>
        /// Lấy thông tin tỉnh theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        BVTL_CITES GetItemByCode(string code);

        /// <summary>
        /// Lấy danh sách tỉnh theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<CityPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy danh sách tỉnh
        /// </summary>
        /// <returns></returns>
        List<BVTL_CITES> GetAll();
        List<BVTL_CITES> GetAllByCodeMap();

        /// <summary>
        /// Lấy danh sách tỉnh theo người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<BVTL_CITES> GetCityReport(int userId);

        /// <summary>
        /// Cập nhật trạng thái tỉnh trọng điểm CD45 và mã viết tắt
        /// </summary>
        ObjectMessage UpdateKeyProvince(string code, string codeMap, bool isKey);

        /// <summary>
        /// Lấy toàn bộ danh mục 34 tỉnh mới (NQ 202/2025/QH15)
        /// </summary>
        List<CityNewModel> GetAllNewCities(bool keyOnly = false);

        /// <summary>
        /// Lấy toàn bộ danh sách ánh xạ 63 tỉnh cũ sang 34 tỉnh mới
        /// </summary>
        List<CityMappingModel> GetCityMappings();

        /// <summary>
        /// Lấy danh sách các mã tỉnh cũ thuộc về 1 tỉnh mới
        /// </summary>
        List<string> GetMappedOldCityCodes(string newCityCode);
    }
}
