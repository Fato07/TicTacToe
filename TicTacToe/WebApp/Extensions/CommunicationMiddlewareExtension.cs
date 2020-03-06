using Microsoft.AspNetCore.Builder;
using WebApp.MiddleWare;

namespace WebApp.Extensions
{
    public static class CommunicationMiddlewareExtension
    {
        public static IApplicationBuilder UseCommunicationMiddleware(this IApplicationBuilder app) 
        { 
            return app.UseMiddleware<CommunicationMiddleware>(); 
        } 
    }
}