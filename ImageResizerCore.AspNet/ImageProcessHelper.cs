using SkiaSharp;
using System.Drawing;

namespace ImageResizerCore.AspNet
{
    public class ImageProcessHelper
    {
        public static SKBitmap CropImage(SKBitmap original, PointF cropTopLeft, PointF cropBottomRight)
        {
            var cropRect = new SKRectI
            {
                Left = (int) cropTopLeft.X,
                Top = (int) cropTopLeft.Y,
                Right = (int) cropBottomRight.X,
                Bottom = (int) cropBottomRight.Y
            };

            SKBitmap bitmap = new SKBitmap(cropRect.Width, cropRect.Height);

            original.ExtractSubset(bitmap, cropRect);

            original.Dispose();

            return bitmap;
        }

        public static SKBitmap ResizeImage(SKBitmap original, int w, int h, string mode)
        {
            SKBitmap bitmap = null;

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
                float originalRatio = ((float) original.Height) / ((float) original.Width);
                float ratio = ((float) h) / ((float) w);

                switch (mode)
                {
                    case "pad":
                    {
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

                        var imageInfo = new SKImageInfo(drawRect.Width, drawRect.Height, SKImageInfo.PlatformColorType,
                            original.AlphaType);
                        original = original.Resize(imageInfo, SKFilterQuality.High);

                        canvas.DrawBitmap(original, drawRect, new SKPaint());

                        canvas.Flush();
                        canvas.Dispose();

                        break;
                    }
                    case "crop":
                    {
                        SKRectI drawRect;

                        if (originalRatio < ratio)
                        {
                            var newW = original.Width * h / original.Height;

                            var pad = (newW - w) / 2;

                            var imageInfo = new SKImageInfo(newW, h, SKImageInfo.PlatformColorType, original.AlphaType);

                            var resizedBitmap = original.Resize(imageInfo, SKFilterQuality.High);

                            drawRect = new SKRectI
                            {
                                Left = pad,
                                Top = 0,
                                Right = w + pad,
                                Bottom = h
                            };

                            bitmap = new SKBitmap(drawRect.Width, drawRect.Height, true);

                            resizedBitmap.ExtractSubset(bitmap, drawRect);
                        }
                        else
                        {
                            var newH = original.Height * w / original.Width;

                            var pad = (newH - h) / 2;

                            var imageInfo = new SKImageInfo(w, newH, SKImageInfo.PlatformColorType, original.AlphaType);

                            var resizedBitmap  = original.Resize(imageInfo, SKFilterQuality.High);

                            drawRect = new SKRectI
                            {
                                Left = 0,
                                Top = pad,
                                Right = w,
                                Bottom = h + pad
                            };

                            bitmap = new SKBitmap(drawRect.Width, drawRect.Height);

                            resizedBitmap.ExtractSubset(bitmap, drawRect);
                        }

                        break;
                    }
                    default:
                        break;
                }
            }
            else
            {
                bitmap = original.Copy();
            }

            original.Dispose();

            return bitmap;
        }
    }
}