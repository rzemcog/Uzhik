using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uzhik.Services;

namespace Uzhik.CustomMiddleware
{
    public class NotificationMiddleware
    {

        private readonly RequestDelegate _next;

        public NotificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, INotificationSender sender)
        {
            var message = "message"; 

            return sender.Send(message);
        }

    }
}
