using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OutlookClone.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string Guid { get; set; }

        [Required(ErrorMessage = "First name is required and must not be empty.")]
        [StringLength(200, ErrorMessage = "First name should not exceed 200 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required and must not be empty.")]
        [StringLength(200, ErrorMessage = "Last name should not exceed 200 characters.")]
        public string LastName { get; set; }

        [HiddenInput]
        [DataType(DataType.DateTime)]
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public ICollection<MailModel> Mails { get; set; }

        public ICollection<GroupModel> Groups { get; set; }

        public static explicit operator ContactModel(Microsoft.Graph.User usr) => new ContactModel
        {
            FirstName = usr.GivenName,
            LastName = usr.Surname,
            Guid = usr.Id,
            Mails = new List<MailModel>(),
            Groups = new List<GroupModel>(),
            JoinDate = DateTime.Now
        };

        public ContactModel()
        {
            Mails = new List<MailModel>();
            Groups = new List<GroupModel>();
        }
        
        public string FullName => $"{FirstName} {LastName}";
    }
}