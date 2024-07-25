using DataAccess_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic_Layer.Entity
{
    public class ChatWithUserDetails
    {
        public string Id { get; set; }
        public string NameGroup { get; set; }
        public string AvatarGroup { get; set; }
        public bool IsGroup { get; set; }
        public List<MemberWithUserDetails> Members { get; set; }
        public List<Message> Messages { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class MemberWithUserDetails
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool VerifyEmail { get; set; }
    }

}
