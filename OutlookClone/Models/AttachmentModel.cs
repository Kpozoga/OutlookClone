using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace OutlookClone.Models
{
    public class AttachmentModel
    {
        public int Id { get; set; }
        
        public string FileName { get; set; }

        public string FileUri { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        [NotMapped]  
        public IFormFile File { get; set; }  

        public AttachmentModel()
        {
            CreatedAt = DateTime.Now;
        }
    }
}