using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System.Collections.Generic;
using WebApp.Service;

namespace BVTL.Tests
{
    [TestClass]
    public class ExcelReportServiceTests
    {
        private IExcelReportService _excelService;

        [TestInitialize]
        public void Setup()
        {
            _excelService = new ExcelReportService();
        }

        [TestMethod]
        public void ExportReport_WithValidData_ShouldReturnNonEmptyByteArray()
        {
            // Arrange
            var listData = new List<BaoCaoModel>
            {
                new BaoCaoModel
                {
                    STT = "1",
                    ThongTinBC = "Số tiếp cận truyền thông",
                    Tong = 150,
                    MSM = 100,
                    PUD = 30,
                    SW = 20,
                    IsShow = "Y",
                    BoldText = "Y"
                },
                new BaoCaoModel
                {
                    STT = "2",
                    ThongTinBC = "Số khách hàng nhận vật phẩm",
                    Tong = 80,
                    MSM = 50,
                    PUD = 20,
                    SW = 10,
                    IsShow = "Y",
                    BoldText = "N"
                }
            };

            var user = new UserLogin
            {
                Name = "Người dùng thử nghiệm",
                UserName = "testuser"
            };

            string outFileName = string.Empty;

            // Act
            byte[] fileBytes = _excelService.ExportReport(
                listData,
                "BÁO CÁO THỰC NGHIỆM UNIT TEST",
                "Sheet1",
                user,
                "Tất cả nhóm",
                "BaoCaoTest",
                out outFileName
            );

            // Assert
            Assert.IsNotNull(fileBytes);
            Assert.IsTrue(fileBytes.Length > 0);
            Assert.IsFalse(string.IsNullOrEmpty(outFileName));
            // Verify PKZip magic header (0x50 0x4B) for openxml .xlsx format
            Assert.AreEqual((byte)0x50, fileBytes[0]);
            Assert.AreEqual((byte)0x4B, fileBytes[1]);
        }
    }
}
