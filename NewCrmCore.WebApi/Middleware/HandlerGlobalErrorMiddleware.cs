using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NewCrmCore.WebApi.Middleware
{
    public class HandlerGlobalErrorMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public HandlerGlobalErrorMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {

        }

    }
}