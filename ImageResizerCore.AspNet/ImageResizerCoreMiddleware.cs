using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ImageResizerCore.AspNet
{
    public class ImageResizerCoreMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _environment;

        public ImageResizerCoreMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }
    }
}
