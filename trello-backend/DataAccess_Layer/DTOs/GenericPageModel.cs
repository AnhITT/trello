
namespace DataAccess_Layer.DTOs
{
    public class GenericPageModel<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
