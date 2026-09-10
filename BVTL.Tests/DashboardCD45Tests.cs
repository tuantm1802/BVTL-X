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
    }
}
