using Microsoft.AspNetCore.Builder;

namespace ImageResizerCore.AspNet
{
    public static class Extensions
    {
        public static IApplicationBuilder ImageResizerCore(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageResizerCoreMiddleware>();
        }
    }
}
