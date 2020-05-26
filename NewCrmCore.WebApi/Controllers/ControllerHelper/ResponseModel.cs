using System;
namespace NewCrmCore.WebApi.ApiHelper
{
    /// <summary>
    /// 响应基类
    /// </summary>
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
    }

    /// <summary>
    /// 返回一个或多个模型响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T> : ResponseBase
    {
        public T Model { get; set; }

        public ResponseModel() : base()
        {
            Model = default(T);
        }
    }

    /// <summary>
    /// 分页响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponsePaging<T> : ResponseModel<T>
    {
        public Int32 TotalCount { get; set; }

        public ResponsePaging()
        {
            TotalCount = 0;
        }
    }

    /// <summary>
    /// 返回简单的响应 只返回是否成功和信息
    /// </summary>
    public class ResponseSimple : ResponseModel<String>
    {

    }
}
