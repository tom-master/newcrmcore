using System;
namespace NewCrmCore.Web.Controllers.ControllerHelper
{
    public class ResponseModel<T>
    {
        public ResponseModel()
        {
            IsSuccess = false;
            Message = "";
            Model = default(T);
        }

        public Boolean IsSuccess { get; set; }

        public String Message { get; set; }

        public T Model { get; set; }

        public String status
        {
            get
            {
                return IsSuccess ? "y" : "n";
            }
        }
    }

    public class ResponseModels<T> : ResponseModel<T>
    {
        public Int32 TotalCount { get; set; }
    }

    public class ResponseModel : ResponseModel<String>
    {

    }
}
