using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BVTL.Tests
{
    [TestClass]
    public class DashboardCD45Tests
    {
        [TestMethod]
        public void GetDashboardData_WithoutFilter_ShouldReturnValidOverview()
        {
            var da = new DashboardCD45DA();
            var result = da.GetDashboardData(null, null, null, null);

            Assert.IsNotNull(result, "Result must not be null");
            Assert.IsNotNull(result.Overview, "Overview must not be null");
            Assert.IsTrue(result.Overview.TongKhachHang > 0, "TongKhachHang should be greater than 0");
            Assert.IsTrue(result.Overview.TongSangLocQST > 0, "TongSangLocQST should be greater than 0");
            Assert.IsTrue(result.Overview.QSTNguyCoCao > 0, "QSTNguyCoCao should be greater than 0");

            Assert.IsNotNull(result.ByTargetGroup, "ByTargetGroup must not be null");
            Assert.IsTrue(result.ByTargetGroup.Count >= 5, "Should have at least 5 target groups");

            Assert.IsNotNull(result.CascadeFunnel, "CascadeFunnel must not be null");
            Assert.IsTrue(result.CascadeFunnel.Step1_TiepCanTruyenThong > 0, "Step1_TiepCanTruyenThong > 0");
            Assert.IsTrue(result.CascadeFunnel.Step2_SangLocQST > 0, "Step2_SangLocQST > 0");
        }

        [TestMethod]
        public void GetDashboardData_WithHanoiFilter_ShouldFilterProperly()
        {
            var da = new DashboardCD45DA();
            var result = da.GetDashboardData("HNO", null, null, null);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Overview.TongKhachHang > 0, "Hanoi should have customers");

            // ByProvince should only contain Hanoi when filtered by HNO or TongKH matches Hanoi
            var hno = result.ByProvince.FirstOrDefault(p => p.CityCode == "HNO");
            Assert.IsNotNull(hno, "Should have HNO province record");
            Assert.AreEqual(result.Overview.TongKhachHang, hno.TongKH, "Total KH should match Hanoi KH");
        }

        [TestMethod]
        public void GetDashboardData_WithNhomFilter_ShouldFilterProperly()
        {
            var da = new DashboardCD45DA();
            // Test with group standard code "HN_TT" (The Times)
            var result = da.GetDashboardData(null, "HN_TT", null, null);

            Assert.IsNotNull(result, "Result must not be null");
            Assert.IsTrue(result.Overview.TongKhachHang > 0, "The Times group must have customers");
            Assert.IsTrue(result.Overview.TongSangLocQST > 0, "The Times group must have QST screenings");
            Assert.IsTrue(result.Overview.QSTNguyCoCao > 0, "The Times group must have high risk QST");
            Assert.IsTrue(result.ByTargetGroup.Count > 0, "ByTargetGroup should have rows");
            Assert.IsTrue(result.CascadeFunnel.Step1_TiepCanTruyenThong > 0, "Step1 should have records");
        }

        [TestMethod]
        public void GetDashboardData_WithCityAndNhomFilter_ShouldFilterProperly()
        {
            var da = new DashboardCD45DA();
            // Test with City HPG and group standard code "HP_HD" (Hải Đăng - Hải Phòng)
            var result = da.GetDashboardData("HPG", "HP_HD", null, null);

            Assert.IsNotNull(result, "Result must not be null");
            Assert.IsTrue(result.Overview.TongKhachHang > 0, "Hải Đăng must have customers");
            Assert.IsTrue(result.Overview.TongSangLocQST > 0, "Hải Đăng must have QST screenings");
            Assert.IsTrue(result.Overview.QSTNguyCoCao > 0, "Hải Đăng must have high risk QST");
        }

        [TestMethod]
        public void CityDA_GetAllByPage_ShouldReturn63ProvincesAndProperPaging()
        {
            var da = new CityDA();
            var searchModel = new Model.ModelExtend.Base.ModelSearch
            {
                currentPage = 1,
                pageSize = 20,
                SortColumn = "Code",
                CityMode = "OLD63"
            };
            var result = da.GetAllByPage(searchModel);

            Assert.IsNotNull(result, "City list must not be null");
            Assert.AreEqual(20, result.Count, "First page should have exactly 20 provinces");
            Assert.IsTrue(result.First().TotalRow >= 63, "TotalRow must be at least 63 nationwide");

            // Verify Code_Map exists for key provinces
            var allProvinces = da.GetAll();
            var hno = allProvinces.FirstOrDefault(c => c.Code == "HNO");
            Assert.IsNotNull(hno);
            Assert.AreEqual("HN", hno.Code_Map, "HNO must have Code_Map HN");
        }

        [TestMethod]
        public void CityDA_GetAllByPage_WithKeyword_ShouldFilterAccurately()
        {
            var da = new CityDA();
            var searchModel = new Model.ModelExtend.Base.ModelSearch
            {
                KeyWord = "Hà Nội",
                currentPage = 1,
                pageSize = 20,
                SortColumn = "Code"
            };
            var result = da.GetAllByPage(searchModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(c => c.Code == "HNO"), "Filtered results must contain HNO");
        }

        [TestMethod]
        public void CityDA_GetAllByPage_WhenFilterIsKeyOnly_ShouldReturnOnlyKeyProvinces()
        {
            var da = new CityDA();
            var searchModel = new Model.ModelExtend.Base.ModelSearch
            {
                CityCodes = "KEY_ONLY",
                currentPage = 1,
                pageSize = 20,
                SortColumn = "KeyFirst"
            };
            var result = da.GetAllByPage(searchModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 6, "Should return at least 6 key provinces");
            Assert.IsTrue(result.All(c => !string.IsNullOrEmpty(c.Code_Map)), "All returned provinces must have Code_Map");
        }

        [TestMethod]
        public void CityDA_UpdateKeyProvince_ShouldValidateAndToggleProperly()
        {
            var da = new CityDA();

            // Test 1: Empty Code_Map when setting as key province should fail
            var invalidResult = da.UpdateKeyProvince("AGI", "", true);
            Assert.IsTrue(invalidResult.Error, "Setting key province without Code_Map should return error");

            // Test 2: Non-existent city should fail
            var notFound = da.UpdateKeyProvince("XYZ_NON_EXISTENT", "XY", true);
            Assert.IsTrue(notFound.Error, "Non-existent city should return error");

            // Test 3: Existing duplicate Code_Map (e.g. 'HN') should fail
            var dup = da.UpdateKeyProvince("AGI", "HN", true);
            Assert.IsTrue(dup.Error, "Duplicate Code_Map must return error");
        }
    }
}
