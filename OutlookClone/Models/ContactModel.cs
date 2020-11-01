using System.ComponentModel.DataAnnotations;

namespace OutlookClone.Models
{
    public class ContactModel
    {    
        [Key]
        public int Guid { get; set; }
        
        [Required(ErrorMessage = "First name is required and must not be empty.")]
        [StringLength(200, ErrorMessage = "First name should not exceed 200 characters.")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required and must not be empty.")]
        [StringLength(200, ErrorMessage = "Last name should not exceed 200 characters.")]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}