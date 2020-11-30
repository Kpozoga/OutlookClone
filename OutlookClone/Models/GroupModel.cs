using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace OutlookClone.Models
{
    public class GroupModel
    {
        public int Id { get; set; }

        public int GroupOwnerID { get; set; }

        [Required(ErrorMessage = "Group name is required and must not be empty.")]
        [StringLength(200, ErrorMessage = "Group name should not exceed 200 characters.")]
        public string GroupName { get; set; }
        public ICollection<ContactModel> GroupMembers { get; set; }

        public GroupModel() { GroupMembers = new List<ContactModel>(); }
    }


}
