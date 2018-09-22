using System;
using System.Threading.Tasks;
using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ZestMonitor.Api.Middleware
{
    public static class ExceptionHandler
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler("/error").WithConventions(x =>
            {
                x.ContentType = "application/json";
                x.MessageFormatter(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred whilst processing your request"
                }));

                x.ForException<DbUpdateException>().ReturnStatusCode(StatusCodes.Status500InternalServerError)
                .UsingMessageFormatter((ex, context) => JsonConvert.SerializeObject(new
                {
                    Message = "Sorry, we had an unexpected error, please contact support."
                }));

                x.ForException<ArgumentNullException>().ReturnStatusCode(StatusCodes.Status400BadRequest);

                x.OnError((exception, httpContext) =>
                {
                    logger.LogError(exception.Message);
                    return Task.CompletedTask;
                });
            });

        }
    }
}