using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENETCare_IMS_WebApp;
using ENETCare_IMS_WebApp.Controllers;

namespace ENETCare_IMS_WebApp.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        /// <summary>
        /// Checks that unauthenticated users are redirected to the login page
        /// </summary>
        [TestMethod]
        public void Index_Redirects_To_Login_For_Unauthorized_User()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            // Check that result is a redirection
            Assert.IsNotNull(result);

            // Check that the redirection is to the login page
            Assert.IsTrue((string)result.RouteValues["controller"] == "Account");
            Assert.IsTrue((string)result.RouteValues["action"] == "Login");
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
