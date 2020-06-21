using System;
namespace NewCrmCore.Web.Controllers
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            IsSuccess = false;
            Message = "";
            Token = "";
        }

        public Boolean IsSuccess { get; set; }

        public String Message { get; set; }

        public String Token { get; set; }

        public String status
        {
            get
            {
                return IsSuccess ? "y" : "n";
            }
        }
    }

    public class ResponseModel<T> : ResponseBase
    {
        public T Model { get; set; }

        public ResponseModel() : base()
        {
            Model = default(T);
        }
    }

    public class ResponsePaging<T> : ResponseModel<T>
    {
        public Int32 TotalCount { get; set; }

        public ResponsePaging()
        {
            TotalCount = 0;
        }
    }


    public class ResponseSimple : ResponseModel<String>
    {

    }
}
