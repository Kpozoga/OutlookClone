using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OutlookClone.Models;

namespace OutlookCloneTests
{
    [TestClass]
    public class MailUnitTest
    {
        [TestMethod]
        public void TestCreateMail()
        {
            var mail = new MailModel
            {
                Subject = "Test Subject 1",
                Body = "Test Body 1, Test Body 1, Test Body 1, Test Body 1, Test Body 1, ",
                From = "test1@example.com",
                Date = DateTime.Now,
            };
            Assert.AreEqual("Test Subject 1", mail.Subject);
        }
    }
}