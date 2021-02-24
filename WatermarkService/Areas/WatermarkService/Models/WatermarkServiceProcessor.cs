/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.IO;
using System.Collections.Generic;
using SkiaSharp;

namespace WatermarkService.Models
{
    public static class WatermarkServiceProcessor
    {
        public static IEnumerable<string> GetFontFamilies()
        {
            return SkiaSharpUtils.GetFontFamilies();
        }

        public static WatermarkServiceResponse Process(WatermarkServiceRequest request)
        {
            return new WatermarkServiceResponse() { Images = Process(request.Watermark, request.Images) };
        }

        private static IEnumerable<Image> Process(Watermark watermark, IEnumerable<Image> images)
        {
            foreach (var image in images)
            {
                if (TryAddWatermark(watermark, image, out Image imageWithWatermark))
                {
                    yield return imageWithWatermark;
                }
            }
        }

        private static bool TryAddWatermark(Watermark watermark, Image input, out Image output)
        {
            var parts = input.DataBase64.Split(',');
            if (parts.Length != 2)
            {
                output = null;
                return false;
            }

            byte[] imageData;
            try
            {
                imageData = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                output = null;
                return false;
            }

            SKBitmap inputBitmap = null;
            SKEncodedImageFormat imageFormat;

            using (var stream = new MemoryStream(imageData))
            {
                inputBitmap = SKBitmap.Decode(stream);
                if (inputBitmap == null)
                {
                    output = null;
                    return false;
                }
            }

            using (var stream = new MemoryStream(imageData))
            {
                using (var codec = SKCodec.Create(stream))
                {
                    imageFormat = codec.EncodedFormat;
                }
            }

            var width = inputBitmap.Width;
            var height = inputBitmap.Height;

            var watermarkWidth = width / watermark.RepeatCountX;
            var watermarkHeight = height / watermark.RepeatCountY;

            using var outputBitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);

            using (var canvas = new SKCanvas(outputBitmap))
            {
                using (var paint = SkiaSharpUtils.CreatePaint())
                {
                    canvas.DrawBitmap(inputBitmap, 0, 0);

                    using (var shader = CreateShader(watermark, watermarkWidth, watermarkHeight))
                    {
                        paint.Shader = CreateShader(watermark, watermarkWidth, watermarkHeight);
                        canvas.DrawRect(0, 0, (float)width, (float)height, paint);
                    }
                }
            }

            output = new Image()
            {
                Name = input.Name,
                DataBase64 = string.Concat(parts[0], ",", Convert.ToBase64String(outputBitmap.Encode(imageFormat, 100).ToArray()))
            };

            inputBitmap.Dispose();
            return true;
        }

        private static SKShader CreateShader(Watermark watermark, int watermarkWidth, int watermarkHeight)
        {
            using var watermarkBitmap = new SKBitmap(watermarkWidth, watermarkHeight);
            using var canvas = new SKCanvas(watermarkBitmap);

            var text = watermark.Text;
            var rotationAngle = (float)watermark.RotationAngle;

            using var font = SkiaSharpUtils.GetOptimalFont(watermark.Text, watermark.Font, rotationAngle, watermarkWidth, watermarkHeight);

            var size = SkiaSharpUtils.GetTextSize(text, font);
            var x = 0.5f * (watermarkWidth - size.Width);
            var y = 0.5f * (watermarkHeight - size.Height);
            var matrix = SkiaSharpUtils.CreateWorldMatrixWithRotation(canvas.TotalMatrix, rotationAngle, watermarkWidth, watermarkHeight);

            canvas.SetMatrix(matrix);

            using var paint = SkiaSharpUtils.CreatePaint(font);

            SkiaSharpUtils.SetColor(paint, watermark.BackgroundColor);
            canvas.DrawRect(x, y, size.Width, size.Height, paint);

            SkiaSharpUtils.SetColor(paint, watermark.ForegroundColor);
            SkiaSharpUtils.DrawText(canvas, text, x, y, paint);

            return SKShader.CreateBitmap(
                watermarkBitmap,
                SKShaderTileMode.Repeat,
                SKShaderTileMode.Repeat);
        }
    }
}