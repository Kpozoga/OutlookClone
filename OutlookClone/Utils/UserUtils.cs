using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OutlookClone.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OutlookClone.Utils
{
    public class UserUtils
    {
        private static IConfiguration config;
        public static ContactModel GetCurrentUser(ClaimsPrincipal User, MyDbContext db)
        {
             var userGuid = ((ClaimsIdentity) User.Identity)
                .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return db.Contacts.First(c => c.Guid == userGuid);
        }
        public static async Task<Response> SendEmailNotification(ContactModel user, MailModel mail, string text = null, string action = null)
        {
            if (string.IsNullOrEmpty(user.Email))
                return null;
            if (text == null)
                text = "You have received a new message in our system\n\n" +
                "Message details: \n" +
                action + "/" + mail.Id;

            var To = new EmailAddress(user.Email, user.FullName);
            var From = new EmailAddress("01142204@pw.edu.pl", "Outlook Clone App");
            var Subject = $"You have a new message";
            var HtmlContent = $"<strong>{text}<strong>";
            var PlainTextContent = text;

            var client = new SendGridClient(config.GetValue<string>("SENDGRID_API_KEY"));         
            var msg = MailHelper.CreateSingleEmail(
                From, To, Subject, PlainTextContent, HtmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }

        public static async Task<MessageResource> SendSmsNotification(ContactModel user, MailModel mail, string text = null, string action = null)
        {
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return null;
            if(text==null)
                text = "\nYou have received a new message in our system\n" +
               "Message details: \n" +
               action + "/" + mail.Id;

            string accountSid = config.GetValue<string>("TWILIO_ACCOUNT_SID");
            string authToken = config.GetValue<string>("TWILIO_AUTH_TOKEN");

            TwilioClient.Init(accountSid, authToken);

            var message = await MessageResource.CreateAsync(
                body: text,
                from: new Twilio.Types.PhoneNumber(config.GetValue<string>("TWILIO_SENDER_PHONE")),
                to: new Twilio.Types.PhoneNumber(user.PhoneNumber)
            );
            return message;
        }
        public static void SetConfig(IConfiguration c)
        {
            config = c;
        }
    }
}
