
namespace BusinessLogic_Layer.Model
{
    public class ApiPageModel
    {
        private int _pageSize;

        public int CurrentPage { get;  set; }
        public int TotalPages { get;  set; }
        public int PageSize { get;  set; }
        public string? Fillter { get;  set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool? OrderByDateEnd { get; set; }
        public bool? OrderEmail { get; set; }

        public int Skip
        {
            get
            {
                return (CurrentPage - 1) * PageSize;
            }
        }
        public int Take
        {
            get
            {
                return PageSize;
            }
        }

    }
}
