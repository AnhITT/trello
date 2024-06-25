using BusinessLogic_Layer.Enums;

namespace BusinessLogic_Layer.Entity
{
    public class ResultObject
    {
        public ResultObject()
        {
            StatusCode = 0;
        }
        public string Message { get; set; }
        public object Data { get; set; }
        public EnumStatusCodesResult StatusCode { get; set; }
        public bool Success { get; set; }
        public object Error { get; set; }
    }
}
