using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ImageResizerCore.AspNet
{
    public static class Extensions
    {
        private static string[] Suffixes = new string[] {
            ".png",
            ".jpg",
            ".jpeg"
        };

        public static IApplicationBuilder UseImageResizerCoreMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageResizerCoreMiddleware>();
        }

        public static bool IsImagePath(this PathString path)
        {
            bool result = false;

            if (path != null && path.HasValue)
            {
                result = Suffixes.Any(x => x.EndsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }
    }
}
