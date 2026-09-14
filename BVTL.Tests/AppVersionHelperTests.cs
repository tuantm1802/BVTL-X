using Common.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BVTL.Tests
{
    [TestClass]
    public class AppVersionHelperTests
    {
        [TestMethod]
        public void AppVersionInfo_DefaultConstructor_ShouldHaveValidDefaults()
        {
            var info = new AppVersionInfo();
            Assert.AreEqual("v1.0.0", info.Version);
            Assert.AreEqual("unknown", info.Commit);
            Assert.AreEqual("main", info.Branch);
            Assert.AreEqual("Production", info.Environment);
            Assert.AreEqual("v1.0.0", info.DisplayVersion);
        }

        [TestMethod]
        public void AppVersionInfo_DisplayVersion_WithCommit_ShouldFormatNicely()
        {
            var info = new AppVersionInfo
            {
                Version = "v1.3.2",
                Commit = "fc71cf0"
            };
            Assert.AreEqual("v1.3.2 (fc71cf0)", info.DisplayVersion);
        }

        [TestMethod]
        public void GetVersionInfo_ShouldReturnNonNullInstance()
        {
            AppVersionHelper.ResetCache();
            var info = AppVersionHelper.GetVersionInfo();
            Assert.IsNotNull(info);
            Assert.IsFalse(string.IsNullOrWhiteSpace(info.Version));
            Assert.IsFalse(string.IsNullOrWhiteSpace(info.DisplayVersion));
        }

        [TestMethod]
        public void GetVersionInfo_WhenVersionJsonExists_ShouldReadValues()
        {
            var tempDir = AppDomain.CurrentDomain.BaseDirectory;
            var jsonFile = Path.Combine(tempDir, "version.json");
            var originalExisted = File.Exists(jsonFile);
            string originalContent = null;
            if (originalExisted)
            {
                originalContent = File.ReadAllText(jsonFile);
            }

            try
            {
                var testJson = "{\"Version\":\"v9.9.9\",\"Commit\":\"abcdef1\",\"Branch\":\"test-branch\",\"BuildDate\":\"2026-09-14 10:00:00\",\"Environment\":\"Testing\"}";
                File.WriteAllText(jsonFile, testJson);

                AppVersionHelper.ResetCache();
                var info = AppVersionHelper.GetVersionInfo();

                Assert.AreEqual("v9.9.9", info.Version);
                Assert.AreEqual("abcdef1", info.Commit);
                Assert.AreEqual("test-branch", info.Branch);
                Assert.AreEqual("2026-09-14 10:00:00", info.BuildDate);
                Assert.AreEqual("Testing", info.Environment);
                Assert.AreEqual("v9.9.9 (abcdef1)", info.DisplayVersion);
            }
            finally
            {
                if (originalExisted && originalContent != null)
                {
                    File.WriteAllText(jsonFile, originalContent);
                }
                else if (File.Exists(jsonFile))
                {
                    File.Delete(jsonFile);
                }
                AppVersionHelper.ResetCache();
            }
        }
    }
}
