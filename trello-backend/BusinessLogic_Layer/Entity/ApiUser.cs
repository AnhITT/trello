
namespace BusinessLogic_Layer.Entity
{
    public class ApiUser
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool VerifyEmail { get; set; } 
    }
}
