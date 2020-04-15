using SkiaSharp;

namespace ImageResizerCore.AspNet
{
    public class ImageProcessHelper
    {
        public static SKBitmap ResizeImage(SKBitmap original, int w, int h, string mode)
        {
            var bitmap = new SKBitmap();

            if (w == 0 && h != 0)
            {
                w = original.Width * h / original.Height;

                var imageInfo = new SKImageInfo(w, h, SKImageInfo.PlatformColorType, original.AlphaType);

                bitmap = original.Resize(imageInfo, SKFilterQuality.High);
            }
            else if (h == 0 && w != 0)
            {
                h = original.Height * w / original.Width;

                var imageInfo = new SKImageInfo(w, h, SKImageInfo.PlatformColorType, original.AlphaType);

                bitmap = original.Resize(imageInfo, SKFilterQuality.High);
            }
            else if (w != 0 && h != 0)
            {
                switch (mode)
                {
                    case "pad":
                        {
                            float originalRatio = ((float)original.Height) / ((float)original.Width);
                            float ratio = ((float)h) / ((float)w);

                            SKRectI drawRect;
                            if (originalRatio < ratio)
                            {
                                var newW = w;
                                var newH = original.Height * w / original.Width;
                                var pad = (h - newH) / 2;

                                drawRect = new SKRectI
                                {
                                    Left = 0,
                                    Top = pad,
                                    Right = newW,
                                    Bottom = newH + pad
                                };
                            }
                            else
                            {
                                var newW = original.Width * h / original.Height;
                                var newH = h;
                                var pad = (w - newW) / 2;

                                drawRect = new SKRectI
                                {
                                    Left = pad,
                                    Top = 0,
                                    Right = newW + pad,
                                    Bottom = newH
                                };
                            }

                            bitmap = new SKBitmap(w, h, true);
                            var canvas = new SKCanvas(bitmap);
                            canvas.Clear(new SKColor(255, 255, 255));

                            var imageInfo = new SKImageInfo(drawRect.Width, drawRect.Height, SKImageInfo.PlatformColorType, original.AlphaType);
                            original = original.Resize(imageInfo, SKFilterQuality.High);

                            canvas.DrawBitmap(original, drawRect, new SKPaint());

                            canvas.Flush();
                            canvas.Dispose();

                            break;
                        }
                    default:
                        break;
                }
            }

            original.Dispose();

            return bitmap;
        }
    }
}
