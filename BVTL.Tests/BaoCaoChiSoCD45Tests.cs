using Common.Common;
using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BVTL.Tests
{
    [TestClass]
    public class BaoCaoChiSoCD45Tests
    {
        private BaoCaoChiSoCD45DA _da;

        [TestInitialize]
        public void Setup()
        {
            _da = new BaoCaoChiSoCD45DA();
        }

        [TestMethod]
        public void GetBaoCaoChiSo_Cumulative_ShouldReturnAllIndicatorsAndPassArithmeticValidation()
        {
            var data = _da.GetBaoCaoChiSo(null, "31/12/2026", null, null, true, 2026, "ALL");

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count >= 40, "Expected full indicator hierarchy");

            string[] requiredCodes =
            {
                "MTCT1.1", "MTCT1.1_NUM", "MTCT1.1_DEN",
                "MTCT1.2", "MTCT1.2_NUM", "MTCT1.2_DEN",
                "MTCT1.3", "MTCT1.3_SUB",
                "MTCT1.4", "MTCT1.4_NUM", "MTCT1.4_DEN",
                "MTCT1.5", "MTCT1.5_NUM", "MTCT1.5_DEN",
                "MTCT-KQ1.1", "MTCT-KQ1.2",
                "MTCT-KQ2.1", "MTCT-KQ2.2",
                "MTCT-KQ3.1",
                "MTCT-KQ4.1", "MTCT-KQ4.2", "MTCT-KQ4.3", "MTCT-KQ4.4",
                "MTCT-KQ5.1", "MTCT-KQ5.2", "MTCT-KQ5.3"
            };

            foreach (var code in requiredCodes)
            {
                Assert.IsTrue(data.Any(x => x.Code == code), $"Missing indicator code '{code}'");
            }

            string warning;
            bool isValid = ReportValidatorHelper.ValidateChiSoReportArithmetic(data, out warning);
            Assert.IsTrue(isValid, "Arithmetic validation failed: " + warning);

            var m11 = data.First(x => x.Code == "MTCT1.1");
            Assert.IsTrue((m11.Tong ?? 0) > 0 && (m11.Denom_Tong ?? 0) > 0);
            Assert.IsTrue((m11.Tong ?? 0) <= (m11.Denom_Tong ?? 0), "MTCT1.1 numerator cannot exceed denominator");

            var m14 = data.First(x => x.Code == "MTCT1.4");
            Assert.IsTrue((m14.Tong ?? 0) <= (m14.Denom_Tong ?? 0), "MTCT1.4 numerator cannot exceed denominator");
        }

        [TestMethod]
        public void GetBaoCaoChiSo_InPeriodVsCumulative_ShouldBeConsistent()
        {
            var cum = _da.GetBaoCaoChiSo("01/01/2026", "31/12/2026", null, null, true, 2026, "12T");
            var inPeriod = _da.GetBaoCaoChiSo("01/01/2026", "31/12/2026", null, null, false, 2026, "12T");

            Assert.IsNotNull(cum);
            Assert.IsNotNull(inPeriod);

            var cumF1 = cum.First(x => x.Code == "MTCT1.1_DEN").Tong ?? 0;
            var perF1 = inPeriod.First(x => x.Code == "MTCT1.1_DEN").Tong ?? 0;
            Assert.IsTrue(perF1 <= cumF1, $"In-period F1 ({perF1}) must not exceed cumulative F1 ({cumF1})");

            var cumQst = cum.First(x => x.Code == "MTCT1.1_NUM").Tong ?? 0;
            var perQst = inPeriod.First(x => x.Code == "MTCT1.1_NUM").Tong ?? 0;
            Assert.IsTrue(perQst <= cumQst, $"In-period QST ({perQst}) must not exceed cumulative QST ({cumQst})");
        }

        [TestMethod]
        public void GetBaoCaoChiSo_WithCityAndGroupFilter_ShouldNarrowScope()
        {
            var allData = _da.GetBaoCaoChiSo(null, "31/12/2026", null, null, true, 2026, "12T");
            var hnoData = _da.GetBaoCaoChiSo(null, "31/12/2026", "HNO", null, true, 2026, "12T");
            var nhomData = _da.GetBaoCaoChiSo(null, "31/12/2026", "HNO", "HN_TT", true, 2026, "12T");

            int allF1 = allData.First(x => x.Code == "MTCT1.1_DEN").Tong ?? 0;
            int hnoF1 = hnoData.First(x => x.Code == "MTCT1.1_DEN").Tong ?? 0;
            int nhomF1 = nhomData.First(x => x.Code == "MTCT1.1_DEN").Tong ?? 0;

            Assert.IsTrue(hnoF1 > 0 && hnoF1 < allF1, "HNO F1 must be > 0 and < total F1");
            Assert.IsTrue(nhomF1 > 0 && nhomF1 <= hnoF1, "Group HN_TT F1 must be > 0 and <= HNO F1");

            string warning;
            Assert.IsTrue(ReportValidatorHelper.ValidateChiSoReportArithmetic(hnoData, out warning), warning);
            Assert.IsTrue(ReportValidatorHelper.ValidateChiSoReportArithmetic(nhomData, out warning), warning);
        }

        [TestMethod]
        public void GetDrillDown_KeyIndicatorsAndDimensionFilters_ShouldMatchSummary()
        {
            var summary = _da.GetBaoCaoChiSo(null, "31/12/2026", "HNO", null, true, 2026, "12T");
            int expectedMtct14 = summary.First(x => x.Code == "MTCT1.4_NUM").Tong ?? 0;
            int expectedMtct14Msm = summary.First(x => x.Code == "MTCT1.4_NUM").MSM ?? 0;

            var dd14 = _da.GetDrillDown("MTCT1.4_NUM", null, "31/12/2026", "HNO", null, true, null, null, null);
            Assert.IsNotNull(dd14);
            Assert.AreEqual(expectedMtct14, dd14.Count, "DrillDown count for MTCT1.4_NUM (HNO) must match summary");

            var dd14Msm = _da.GetDrillDown("MTCT1.4_NUM", null, "31/12/2026", "HNO", null, true, 4, null, null);
            Assert.AreEqual(expectedMtct14Msm, dd14Msm.Count, "DrillDown count for MTCT1.4_NUM (HNO, MSM) must match MSM column");

            // MTCT-KQ4.1 (Hospital enrichment check)
            var ddHospital = _da.GetDrillDown("MTCT-KQ4.1", null, "31/12/2026", null, null, true, null, null, null);
            Assert.IsNotNull(ddHospital);
            Assert.IsTrue(ddHospital.Count > 0, "Expected MTCT-KQ4.1 hospital records");
            Assert.IsTrue(ddHospital.All(x => !string.IsNullOrEmpty(x.CHI_TIET)), "Hospital details must be enriched");

            // MTCT-KQ2.1 (TCV directory)
            int expectedTcv = summary.First(x => x.Code == "MTCT-KQ2.1").Tong ?? 0;
            var ddTcv = _da.GetDrillDown("MTCT-KQ2.1", null, "31/12/2026", "HNO", null, true, null, null, null);
            Assert.AreEqual(expectedTcv, ddTcv.Count, "MTCT-KQ2.1 drilldown count must match TCV active count");
        }

        [TestMethod]
        public void Config_GetAndSave_RoundTrip_ShouldPersistAndRestoreCleanly()
        {
            var original = _da.GetConfig(2026, "12T");
            Assert.IsNotNull(original);
            Assert.IsTrue(original.Count > 0, "Expected seeded config items for (2026, 12T)");

            var target = original.First();
            int oldVal = target.GIA_TRI ?? 0;
            int? originalNullableVal = target.GIA_TRI;
            string oldNote = target.GHI_CHU;

            try
            {
                var updatedList = original.Select(x => new CD45_ChiSoDuAnConfigModel
                {
                    MA_THAM_SO = x.MA_THAM_SO,
                    TEN_THAM_SO = x.TEN_THAM_SO,
                    CHI_SO_CODE = x.CHI_SO_CODE,
                    GIA_TRI = x.MA_THAM_SO == target.MA_THAM_SO ? oldVal + 7 : x.GIA_TRI,
                    GHI_CHU = x.MA_THAM_SO == target.MA_THAM_SO ? "UnitTest Verify" : x.GHI_CHU
                }).ToList();

                string msg;
                bool ok = _da.SaveConfig(2026, "12T", updatedList, "UnitTest", out msg);
                Assert.IsTrue(ok, "SaveConfig failed: " + msg);

                var reloaded = _da.GetConfig(2026, "12T");
                var reloadedTarget = reloaded.First(x => x.MA_THAM_SO == target.MA_THAM_SO);
                Assert.AreEqual(oldVal + 7, reloadedTarget.GIA_TRI);
            }
            finally
            {
                var restoreList = original.Select(x => new CD45_ChiSoDuAnConfigModel
                {
                    MA_THAM_SO = x.MA_THAM_SO,
                    TEN_THAM_SO = x.TEN_THAM_SO,
                    CHI_SO_CODE = x.CHI_SO_CODE,
                    GIA_TRI = x.MA_THAM_SO == target.MA_THAM_SO ? originalNullableVal : x.GIA_TRI,
                    GHI_CHU = x.MA_THAM_SO == target.MA_THAM_SO ? oldNote : x.GHI_CHU
                }).ToList();
                _da.SaveConfig(2026, "12T", restoreList, "UnitTestRestore", out _);
            }
        }

        [TestMethod]
        public void ExistingBaoCaoCD45DA_ShouldRemainUnaffected()
        {
            var legacyDa = new BaoCaoCD45DA();
            var rows = legacyDa.GetBaoCao(null, null, null, null, null);
            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Count > 0, "Existing BaoCaoCD45DA.GetBaoCao must continue returning rows without regressions");
        }
    }
}
