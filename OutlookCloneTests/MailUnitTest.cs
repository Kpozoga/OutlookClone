using System;
using System.Collections.Generic;
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
            var contact = new ContactModel
            {
                Id = 1,
                Guid = "john-doe-guid",
                FirstName = "John",
                LastName = "Doe",
                Groups = new List<GroupModel>(),
                Mails = new List<MailModel>()
            };
            
            var mail = new MailModel
            {
                Subject = "Test Subject 1",
                Body = "Test Body 1, Test Body 1, Test Body 1, Test Body 1, Test Body 1, ",
                FromId = contact.Id,
                Date = DateTime.Now,
            };
            
            Assert.AreEqual("Test Subject 1", mail.Subject);
        }
    }
}