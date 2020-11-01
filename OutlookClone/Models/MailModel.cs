using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OutlookClone.Models
{
    public class MailModel
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Mail has to have a subject.")]
        [StringLength(50, ErrorMessage = "Subject can't be longer than 50 characters")]
        public string Subject { get; set; }
        
        [StringLength(1000, ErrorMessage = "Email body can't be longer than 1000 characters for now")]
        public string Body { get; set; }
        
        [HiddenInput]
        public string From { get; set; }
        
        [Required]
        public List<ContactModel> To { get; set; }
        
        [HiddenInput] 
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        
        public List<string> Attachments;
    }
}
