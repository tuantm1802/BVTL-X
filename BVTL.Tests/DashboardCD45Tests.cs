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

        [TestMethod]
        public void GetDashboardData_Mode2_Gender_ShouldReturnMaleFemaleOtherAndBalance()
        {
            var da = new DashboardCD45DA();
            var result = da.GetDashboardData(null, null, null, null, null, "NEW34", null, null, null, 2);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ByTargetGroup);
            Assert.AreEqual(3, result.ByTargetGroup.Count, "Mode 2 must return exactly 3 genders (Nam, Nữ, Khác)");

            var names = result.ByTargetGroup.Select(x => x.TenDoiTuong).ToList();
            CollectionAssert.Contains(names, "Nam");
            CollectionAssert.Contains(names, "Nữ");
            CollectionAssert.Contains(names, "Khác");

            int sumKH = result.ByTargetGroup.Sum(x => x.TongKH);
            Assert.AreEqual(result.Overview.TongKhachHang, sumKH, "Sum of gender KH must match Overview.TongKhachHang");

            Assert.IsNotNull(result.MentalHealth);
            Assert.AreEqual(3, result.MentalHealth.Count, "MentalHealth must return 3 gender rows");
        }

        [TestMethod]
        public void GetDashboardData_Mode3_AgeGroup_ShouldReturnAllAgeBucketsAndBalance()
        {
            var da = new DashboardCD45DA();
            var result = da.GetDashboardData(null, null, null, null, null, "NEW34", null, null, null, 3);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ByTargetGroup);
            Assert.AreEqual(5, result.ByTargetGroup.Count, "Mode 3 must return 5 age groups (<18, 18-25, 26-35, >=36, Chưa xác định)");

            int sumKH = result.ByTargetGroup.Sum(x => x.TongKH);
            Assert.AreEqual(result.Overview.TongKhachHang, sumKH, "Sum of age group KH must match Overview.TongKhachHang");
        }

        [TestMethod]
        public void GetDashboardData_WithGenderFilter_Female_ShouldFilterEntireDashboard()
        {
            var da = new DashboardCD45DA();
            // Filter: Female (gioiTinhFilter = 2)
            var result = da.GetDashboardData(null, null, null, null, null, "NEW34", 2, null, null, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(918, result.Overview.TongKhachHang, "Female filter should yield 918 clients");

            int sumTargetGroupKH = result.ByTargetGroup.Sum(x => x.TongKH);
            Assert.AreEqual(918, sumTargetGroupKH, "Sum across target groups for females must equal 918");
        }

        [TestMethod]
        public void GetDashboardData_CombinedGenderFilterAndAgeDimension_ShouldBalance()
        {
            var da = new DashboardCD45DA();
            // Filter: Female (2) AND DimensionMode: Age Group (3)
            var result = da.GetDashboardData(null, null, null, null, null, "NEW34", 2, null, null, 3);

            Assert.IsNotNull(result);
            Assert.AreEqual(918, result.Overview.TongKhachHang);

            int sumKH = result.ByTargetGroup.Sum(x => x.TongKH);
            Assert.AreEqual(918, sumKH, "Sum across age groups for female segment must equal 918");

            int sumSangLoc = result.ByTargetGroup.Sum(x => x.SoKHSangLoc);
            Assert.AreEqual(result.Overview.TongSangLocQST, sumSangLoc, "Sum of QST screened across age groups must match Overview.TongSangLocQST");
        }

        [TestMethod]
        public void GetListTCV_WithNullFilters_ShouldReturnAll112TCVs()
        {
            var da = new BaoCaoCD45DA();
            var list = da.GetListTCV(null, null);

            Assert.IsNotNull(list);
            Assert.AreEqual(112, list.Count, "Total TCV count in CD45_NHOM_TCV must be 112");
            Assert.IsTrue(list.All(t => !string.IsNullOrEmpty(t.TEN_TCV)), "Every TCV must have a name");
            Assert.IsTrue(list.All(t => !string.IsNullOrEmpty(t.MA_TCV)), "Every TCV must have an ID code");
        }

        [TestMethod]
        public void GetListTCV_WithMultiCityString_ShouldReturnTCVsFromBothProvinces()
        {
            var da = new BaoCaoCD45DA();
            // Test non-admin multi-city permission scope "HNO,HCM"
            var list = da.GetListTCV("HNO,HCM", null);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Multi-city filter 'HNO,HCM' must return TCVs");
            Assert.IsTrue(list.All(t => t.CITY_CODE == "HNO" || t.CITY_CODE == "HCM"), "Results must only contain HNO or HCM");
            Assert.IsTrue(list.Any(t => t.CITY_CODE == "HNO"), "Must contain at least one HNO TCV");
        }

        [TestMethod]
        public void GetListTCV_WithSingleCityString_ShouldReturnOnlyMatchingProvince()
        {
            var da = new BaoCaoCD45DA();
            var list = da.GetListTCV("NAN", null);

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Nghe An (NAN) must have TCVs");
            Assert.IsTrue(list.All(t => t.CITY_CODE == "NAN"), "All returned TCVs must be in NAN province");
        }

        [TestMethod]
        public void GetListTCV_WithGroupStandardCode_ShouldReturnGroupTCVs()
        {
            var da = new BaoCaoCD45DA();
            // "NA_AD" (standard system code for Ánh Dương group, mapped to REDCap code 'ad')
            var list = da.GetListTCV(null, "NA_AD");

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Group NA_AD must return TCVs");
            Assert.IsTrue(list.All(t => t.TEN_NHOM.Contains("Ánh Dương") || t.MA_NHOM == "ad"), "Returned TCVs must belong to Ánh Dương group");
        }

        [TestMethod]
        public void GetListTCV_WithGroupMapCode_ShouldReturnGroupTCVs()
        {
            var da = new BaoCaoCD45DA();
            // "ad" (REDCap code in CD45_NHOM_TCV.MA_NHOM)
            var list = da.GetListTCV(null, "ad");

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "Group 'ad' must return TCVs");
            Assert.IsTrue(list.All(t => t.MA_NHOM == "ad"), "Returned TCVs must have MA_NHOM = 'ad'");
        }
    }
}
