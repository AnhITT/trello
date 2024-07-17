
namespace BusinessLogic_Layer.Entity
{
    public class ApiMessage
    {
        public string ChatId { get; set; }
        public string Sender { get; set; }
        public string Type { get; set; } 
        public string Text { get; set; }
        public string? File { get; set; }
    }
}
