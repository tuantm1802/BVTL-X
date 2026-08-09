using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Model.ModelExtend;

namespace BVTL.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        // Mock logic trích xuất từ HasCredentialAttribute.AuthorizeCore
        // để có thể Unit Test mà không phụ thuộc vào HttpContext.Current
        public bool AuthorizeCoreLogic(string controllerName, List<MenuModel> sessionMenus)
        {
            if (sessionMenus == null)
                return false;

            List<string> privilegeLevels = new List<string>();
            foreach (var menu in sessionMenus)
            {
                if (!string.IsNullOrEmpty(menu.HREF_URL))
                {
                    privilegeLevels.Add(menu.HREF_URL);
                }
            }

            return privilegeLevels.Contains(controllerName);
        }

        [TestMethod]
        public void AuthorizeCoreLogic_WithNullSession_ShouldReturnFalse()
        {
            // Act
            bool result = AuthorizeCoreLogic("BaoCaoTuan", null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthorizeCoreLogic_WithValidMenu_ShouldReturnTrue()
        {
            // Arrange
            var menus = new List<MenuModel>
            {
                new MenuModel { HREF_URL = "BaoCaoTuan" },
                new MenuModel { HREF_URL = "BaoCaoThang" }
            };

            // Act
            bool result = AuthorizeCoreLogic("BaoCaoTuan", menus);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AuthorizeCoreLogic_WithInvalidMenu_ShouldReturnFalse()
        {
            // Arrange
            var menus = new List<MenuModel>
            {
                new MenuModel { HREF_URL = "BaoCaoThang" }
            };

            // Act
            bool result = AuthorizeCoreLogic("BaoCaoTuan", menus);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
