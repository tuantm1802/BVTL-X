using Common.Common;
using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BVTL.Tests
{
    [TestClass]
    public class HttpRetryHelperTests
    {
        [TestMethod]
        public async Task ExecuteWithRetryAsync_OnSuccess_ShouldReturnImmediately()
        {
            // Arrange
            int callCount = 0;

            // Act
            var response = await HttpRetryHelper.ExecuteWithRetryAsync(async () =>
            {
                callCount++;
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, maxRetries: 3, initialDelayMs: 10);

            // Assert
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ExecuteWithRetryAsync_OnTransientErrorThenSuccess_ShouldRetryAndSucceed()
        {
            // Arrange
            int callCount = 0;

            // Act
            var response = await HttpRetryHelper.ExecuteWithRetryAsync(async () =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }, maxRetries: 3, initialDelayMs: 10);

            // Assert
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
