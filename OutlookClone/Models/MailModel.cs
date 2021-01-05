using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OutlookClone.Models
{
    public class MailModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Mail has to have a subject.")]
        [StringLength(50, ErrorMessage = "Subject can't be longer than 50 characters")]
        public string Subject { get; set; }
        
        [StringLength(1000, ErrorMessage = "Email body can't be longer than 1000 characters for now")]
        public string Body { get; set; }
        
        [HiddenInput]
        public int FromId { get; set; }
        
        [Required]
        public ICollection<ContactModel> To { get; set; }
        
        [HiddenInput] 
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public bool Read { get; set; }
        
        public ICollection<AttachmentModel> Attachments { get; set; }

        public MailModel()
        {
            Date = DateTime.Now;
        }
    }
}
