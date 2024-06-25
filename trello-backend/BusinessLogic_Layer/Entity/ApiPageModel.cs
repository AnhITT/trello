using BusinessLogic_Layer.Enums;

namespace BusinessLogic_Layer.Model
{
    public class ApiPageList<T>
    {
        public int CurrentPage { get;  set; }
        public int TotalPages { get;  set; }
        public int PageSize { get;  set; }
        public int TotalCount { get;  set; }
        public List<T> Items { get;  set; }
        public string Message { get; set; }
        public EnumStatusCodesResult StatusCode { get; set; }
        public bool Success { get; set; }
        public object Error { get; set; }

    }
}
