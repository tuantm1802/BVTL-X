using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Base;
using System;
using System.Linq;

namespace BVTL.Tests
{
    [TestClass]
    public class CityMappingTests
    {
        [TestMethod]
        public void CityDA_GetAllNewCities_ShouldReturn34NewProvincesWithKeyCD45Marked()
        {
            var cityDA = new CityDA();
            var newCities = cityDA.GetAllNewCities();

            Assert.IsNotNull(newCities, "Danh sách 34 tỉnh mới không được rỗng");
            Assert.AreEqual(34, newCities.Count, "Tổng số tỉnh mới theo NQ 202/2025/QH15 phải là 34");

            // Kiểm tra 6 tỉnh trọng điểm CD45
            var keyProvinces = newCities.Where(x => x.IsKeyProvince).ToList();
            Assert.AreEqual(6, keyProvinces.Count, "Phải có đúng 6 tỉnh trọng điểm CD45");

            var expectedKeyCodes = new[] { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" };
            foreach (var code in expectedKeyCodes)
            {
                var found = keyProvinces.FirstOrDefault(x => x.Code == code);
                Assert.IsNotNull(found, $"Tỉnh trọng điểm {code} phải tồn tại trong danh mục 34 tỉnh mới");
                Assert.IsFalse(string.IsNullOrWhiteSpace(found.Code_Map), $"Mã viết tắt Code_Map của {code} không được để trống");
            }

            // Tổng số tỉnh cũ gộp lại phải đúng bằng 63
            int totalOldCount = newCities.Sum(x => x.OldCount);
            Assert.AreEqual(63, totalOldCount, "Tổng số tỉnh cũ từ 34 tỉnh mới phải bằng 63");
        }

        [TestMethod]
        public void CityDA_GetCityMappings_ShouldMapAll63HistoricalProvincesAccurately()
        {
            var cityDA = new CityDA();
            var mappings = cityDA.GetCityMappings();

            Assert.IsNotNull(mappings, "Bảng ánh xạ không được null");
            Assert.AreEqual(63, mappings.Count, "Phải có đúng 63 dòng ánh xạ cho 63 tỉnh lịch sử");

            // Kiểm tra tính đơn nhất của OldCityCode
            var distinctOld = mappings.Select(x => x.OldCityCode).Distinct().Count();
            Assert.AreEqual(63, distinctOld, "Tất cả 63 mã tỉnh cũ phải là duy nhất trong bảng ánh xạ");

            // Kiểm tra các trường hợp sáp nhập điển hình
            var hpgOld = cityDA.GetMappedOldCityCodes("HPG");
            CollectionAssert.Contains(hpgOld, "HPG", "Hải Phòng mới phải chứa mã HPG cũ");
            CollectionAssert.Contains(hpgOld, "HDU", "Hải Phòng mới phải chứa mã HDU (Hải Dương) cũ");
            Assert.AreEqual(2, hpgOld.Count, "Hải Phòng mới sáp nhập 2 tỉnh");

            var hcmOld = cityDA.GetMappedOldCityCodes("HCM");
            CollectionAssert.Contains(hcmOld, "HCM", "TP.HCM mới phải chứa mã HCM cũ");
            CollectionAssert.Contains(hcmOld, "BDU", "TP.HCM mới phải chứa mã BDU (Bình Dương) cũ");
            CollectionAssert.Contains(hcmOld, "VTB", "TP.HCM mới phải chứa mã VTB (Bà Rịa Vũng Tàu) cũ");
            Assert.AreEqual(3, hcmOld.Count, "TP.HCM mới sáp nhập 3 đơn vị");

            var nbiOld = cityDA.GetMappedOldCityCodes("NBI");
            CollectionAssert.Contains(nbiOld, "NBI", "Ninh Bình mới phải chứa NBI");
            CollectionAssert.Contains(nbiOld, "HNA", "Ninh Bình mới phải chứa HNA (Hà Nam)");
            CollectionAssert.Contains(nbiOld, "NDH", "Ninh Bình mới phải chứa NDH (Nam Định)");
            Assert.AreEqual(3, nbiOld.Count, "Ninh Bình mới sáp nhập 3 tỉnh");

            var hyeOld = cityDA.GetMappedOldCityCodes("HYE");
            CollectionAssert.Contains(hyeOld, "HYE", "Hưng Yên mới phải chứa HYE");
            CollectionAssert.Contains(hyeOld, "TBH", "Hưng Yên mới phải chứa TBH (Thái Bình)");
            Assert.AreEqual(2, hyeOld.Count, "Hưng Yên mới sáp nhập 2 tỉnh");
        }

        [TestMethod]
        public void CityDA_GetAllByPage_ModeNew34_ShouldReturn34ProvincesAndMergedSummaries()
        {
            var cityDA = new CityDA();
            var search = new ModelSearch
            {
                currentPage = 1,
                pageSize = 50,
                CityMode = "NEW34",
                SortColumn = "KeyFirst"
            };

            var list = cityDA.GetAllByPage(search);
            Assert.IsNotNull(list, "Danh sách không được null");
            Assert.IsTrue(list.Count > 0, "Phải có dữ liệu trả về");
            Assert.AreEqual(34, list.First().TotalRow, "TotalRow ở chế độ NEW34 phải là 34");

            // Kiểm tra thông tin sáp nhập
            var hpg = list.FirstOrDefault(x => x.Code == "HPG");
            Assert.IsNotNull(hpg, "Phải tìm thấy HPG trong danh sách 34 tỉnh");
            Assert.AreEqual(2, hpg.OldCount, "HPG phải có OldCount = 2");
            StringAssert.Contains(hpg.OldNamesSummary, "Hải Dương", "OldNamesSummary của HPG phải có Hải Dương");
        }

        [TestMethod]
        public void CityDA_GetAllByPage_ModeOld63_ShouldReturn63HistoricalProvinces()
        {
            var cityDA = new CityDA();
            var search = new ModelSearch
            {
                currentPage = 1,
                pageSize = 20,
                CityMode = "OLD63",
                SortColumn = "KeyFirst"
            };

            var list = cityDA.GetAllByPage(search);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0);
            Assert.AreEqual(63, list.First().TotalRow, "TotalRow ở chế độ OLD63 phải là 63");
        }

        [TestMethod]
        public void DashboardCD45DA_GetDashboardData_ModeNew34_WithHaiPhongFilter_ShouldAggregateHaiDuongData()
        {
            var da = new DashboardCD45DA();
            // Lọc theo HPG ở chế độ 34 tỉnh mới
            var result = da.GetDashboardData("HPG", null, null, null, null, "NEW34");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Overview);
            Assert.IsTrue(result.Overview.TongKhachHang > 0, "Hải Phòng mới phải có khách hàng");

            // Kiểm tra phân bố tỉnh
            var hpgRow = result.ByProvince.FirstOrDefault(x => x.CityCode == "HPG");
            Assert.IsNotNull(hpgRow, "Phải có dòng dữ liệu Hải Phòng");
            Assert.AreEqual("Hải Phòng", hpgRow.CityName, "Tên tỉnh phải là Hải Phòng");
            Assert.AreEqual(result.Overview.TongKhachHang, hpgRow.TongKH, "Tổng KH phải khớp với dòng Hải Phòng");
        }

        [TestMethod]
        public void BaoCaoCD45DA_GetBaoCao_WithNewProvinceFilter_ShouldExecuteSuccessfully()
        {
            var bcDA = new BaoCaoCD45DA();
            // Lấy báo cáo theo HCM
            var data = bcDA.GetBaoCao(null, null, "HCM", null, null);

            Assert.IsNotNull(data, "Dữ liệu báo cáo không được null");
            Assert.IsTrue(data.Count > 0, "Báo cáo phải có danh sách chỉ tiêu");

            var row1 = data.FirstOrDefault(x => x.Code == "I_1");
            Assert.IsNotNull(row1, "Phải có chỉ tiêu I_1");
            Assert.IsTrue(row1.Tong >= 0, "Tổng KH phải >= 0");
        }
    }
}
