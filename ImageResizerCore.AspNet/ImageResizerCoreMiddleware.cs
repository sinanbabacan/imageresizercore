using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Query.Count == 0 || !context.Request.Path.IsImagePath())
            {
                await _next.Invoke(context);

                return;
            }

            var queryParams = GetQueryParams(context.Request.Path, context.Request.Query);

            if (!queryParams.hasParams)
            {
                await _next.Invoke(context);

                return;
            }

            string path = context.Request.Path;

            var imagePath = Path.Combine(_environment.WebRootPath, path.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            FileStream fileStream = File.OpenRead(imagePath);

            SKBitmap sKBitmap = LoadBitmap(fileStream, out SKEncodedOrigin origin);

            SKData sKData = ImageProcess(sKBitmap, queryParams, origin);

            context.Response.ContentType = queryParams.format == "png" ? "image/png" : "image/jpeg";

            context.Response.ContentLength = sKData.Size;
           
            await context.Response.Body.WriteAsync(sKData.ToArray(), 0, (int)sKData.Size);

            sKBitmap.Dispose();
            sKData.Dispose();
        }

        private QueryParams GetQueryParams(PathString path, IQueryCollection query)
        {
            var resizeParams = new QueryParams();

            resizeParams.hasParams = resizeParams.GetType().GetTypeInfo().GetFields()
                .Where(f => f.Name != "hasParams")
                .Any(f => query.ContainsKey(f.Name));

            if (!resizeParams.hasParams)
            {
                return resizeParams;
            }

            if (query.ContainsKey("w"))
            {
                int.TryParse(query["w"], out int w);
                resizeParams.w = w;
            }

            if (query.ContainsKey("h"))
            {
                int.TryParse(query["h"], out int h);
                resizeParams.h = h;
            }

            if (resizeParams.h != 0 && resizeParams.w != 0)
            {
                resizeParams.mode = "pad";

                if (query.ContainsKey("mode") && QueryParams.modes.Any(q => query["mode"] == q))
                {
                    resizeParams.mode = query["mode"];
                }
            }

            if (query.ContainsKey("crop"))
            {
                resizeParams.crop = query["crop"];
            }

            resizeParams.format = path.Value.Substring(path.Value.LastIndexOf('.') + 1);

            int quality = 100;

            if (query.ContainsKey("quality"))
            {
                int.TryParse(query["quality"], out quality);
            }

            resizeParams.quality = quality;

            return resizeParams;
        }

        private SKBitmap LoadBitmap(Stream stream, out SKEncodedOrigin origin)
        {
            using ( var sKManagedStream = new SKManagedStream(stream))
            {
                using (var codec = SKCodec.Create(sKManagedStream))
                {
                    origin = codec.EncodedOrigin;
                    var info = codec.Info;
                    var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                    IntPtr length;
                    var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out length));

                    if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                    {
                        return bitmap;
                    }
                    else
                    {
                        throw new ArgumentException("Bitmap was not loaded with provided stream data.");
                    }
                }
            }
        }

        private SKData ImageProcess(SKBitmap bitmap, QueryParams queryParams, SKEncodedOrigin origin)
        {
            bitmap = ImageProcessHelper.ResizeImage(bitmap, queryParams.w, queryParams.h, queryParams.mode);

            var encodeFormat = queryParams.format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
            
            var image = SKImage.FromBitmap(bitmap);
            
            return image.Encode(encodeFormat, queryParams.quality);
        }
    }
}
