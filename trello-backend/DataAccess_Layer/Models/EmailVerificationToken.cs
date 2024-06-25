using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess_Layer.Models
{
    public class EmailVerificationToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string TokenEmail { get; set; }

        // n-1  EmailVerificationToken-User
        public Guid UserId { get; set; }
        public User? Users { get; set; }
    }
}
