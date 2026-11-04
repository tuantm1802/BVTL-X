using Data.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelExtend;
using System;
using System.Linq;

namespace BVTL.Tests
{
    [TestClass]
    public class NhomTcvCD45Tests
    {
        private CD45NhomTcvDA _da;

        [TestInitialize]
        public void Setup()
        {
            _da = new CD45NhomTcvDA();
        }

        [TestMethod]
        public void GetListNhomTcv_ShouldReturnActiveRecords()
        {
            var list = _da.GetListNhomTcv(null, null, null);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count > 0, "List must contain groups/TCVs");
        }

        [TestMethod]
        public void GetNextTcvCode_WithExistingGroup_ShouldReturnPositiveInteger()
        {
            string nextCode = _da.GetNextTcvCode("vn");
            Assert.IsFalse(string.IsNullOrWhiteSpace(nextCode));
            bool isNum = int.TryParse(nextCode, out int val);
            Assert.IsTrue(isNum && val > 0, $"Suggested code '{nextCode}' must be a positive integer");
        }

        [TestMethod]
        public void SaveTcv_EmptyFields_ShouldReturnValidationFailure()
        {
            string msg;
            // Null model
            bool r1 = _da.SaveTcv(null, "TestUser", out msg);
            Assert.IsFalse(r1);

            // Empty group
            bool r2 = _da.SaveTcv(new CD45_TcvSaveModel { MA_NHOM = "", MA_TCV = "99", TEN_TCV = "Test" }, "TestUser", out msg);
            Assert.IsFalse(r2);
            StringAssert.Contains(msg, "Nhóm CBO");

            // Empty code
            bool r3 = _da.SaveTcv(new CD45_TcvSaveModel { MA_NHOM = "vn", MA_TCV = "", TEN_TCV = "Test" }, "TestUser", out msg);
            Assert.IsFalse(r3);
            StringAssert.Contains(msg, "Mã TCV");

            // Empty name
            bool r4 = _da.SaveTcv(new CD45_TcvSaveModel { MA_NHOM = "vn", MA_TCV = "99", TEN_TCV = "" }, "TestUser", out msg);
            Assert.IsFalse(r4);
            StringAssert.Contains(msg, "Họ và Tên TCV");
        }

        [TestMethod]
        public void Tcv_FullLifecycle_Insert_Update_Toggle_Delete_ShouldSucceed()
        {
            string testNhom = "vn";
            string testCode = "999";
            string testName = "TCV Test UnitTest";
            string msg;

            // 1. Cleanup any pre-existing test record
            var existing = _da.GetListNhomTcv(null, testNhom, testName);
            foreach (var item in existing.Where(x => x.MA_TCV == testCode))
            {
                _da.DeleteTcv(item.ID, "UnitTest", out _);
            }

            // 2. Insert test TCV
            var saveModel = new CD45_TcvSaveModel
            {
                ID = 0,
                MA_NHOM = testNhom,
                MA_TCV = testCode,
                TEN_TCV = testName,
                IsActive = true
            };
            bool insertOk = _da.SaveTcv(saveModel, "UnitTest", out msg);
            Assert.IsTrue(insertOk, "Insert test TCV failed: " + msg);

            // 3. Verify inserted
            var list = _da.GetListNhomTcv(null, testNhom, testName);
            var inserted = list.FirstOrDefault(x => x.MA_TCV == testCode && x.TEN_TCV == testName);
            Assert.IsNotNull(inserted, "Inserted TCV not found in query");
            int insertedId = inserted.ID;
            Assert.IsTrue(inserted.IsActive, "Newly inserted TCV should be Active");

            // 4. Update test TCV name
            string updatedName = "TCV Test UnitTest Updated";
            saveModel.ID = insertedId;
            saveModel.TEN_TCV = updatedName;
            bool updateOk = _da.SaveTcv(saveModel, "UnitTest", out msg);
            Assert.IsTrue(updateOk, "Update test TCV failed: " + msg);

            // 5. Toggle status
            bool toggleOk = _da.ToggleStatus(insertedId, "UnitTest", out msg);
            Assert.IsTrue(toggleOk, "Toggle status failed: " + msg);
            var toggled = _da.GetTcvById(insertedId);
            Assert.IsNotNull(toggled);
            Assert.IsFalse(toggled.IsActive, "Status should now be inactive");

            // 6. Delete test TCV (no activity reference, should succeed)
            bool deleteOk = _da.DeleteTcv(insertedId, "UnitTest", out msg);
            Assert.IsTrue(deleteOk, "Delete test TCV failed: " + msg);

            // Verify deleted
            var afterDelete = _da.GetTcvById(insertedId);
            Assert.IsNull(afterDelete, "TCV should be deleted from DB");
        }

        [TestMethod]
        public void DeleteTcv_WithActivityReferences_ShouldBeBlockedFromDeletion()
        {
            // Pick an active TCV that has real activity data (e.g. MA_NHOM='vn', MA_TCV='1')
            var list = _da.GetListNhomTcv("HNO", "vn", null);
            var activeWorker = list.FirstOrDefault(x => x.MA_TCV == "1" && x.ID > 0);
            if (activeWorker != null)
            {
                string msg;
                bool deleteResult = _da.DeleteTcv(activeWorker.ID, "UnitTest", out msg);
                Assert.IsFalse(deleteResult, "TCV with data records should NOT be deleted!");
                StringAssert.Contains(msg, "Ngừng hoạt động", "Should advise user to toggle to Ngừng hoạt động");
            }
        }
    }
}
