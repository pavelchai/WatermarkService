/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WatermarkService.Models
{
    internal static class SkiaSharpUtils
    {
        private readonly static string[] lineSeparators = { "\n", "\r", "\r\n" };

        private readonly static IReadOnlyList<string> fontFamilies = SKFontManager.Default.FontFamilies.ToArray();

        internal static IEnumerable<string> GetFontFamilies()
        {
            return fontFamilies;
        }

        internal static SKPaint CreatePaint()
        {
            var paint = new SKPaint();

            paint.FilterQuality = SKFilterQuality.High;
            paint.IsAntialias = true;

            return paint;
        }

        internal static SKPaint CreatePaint(SKFont font)
        {
            var paint = CreatePaint();

            paint.Typeface = font.Typeface;
            paint.TextSize = font.Size;
            paint.TextEncoding = SKTextEncoding.Utf16;
            paint.IsStroke = false;
            paint.IsAntialias = true;
            paint.SubpixelText = true;
            paint.IsLinearText = true;
            paint.TextAlign = 0.0f;
            paint.TextScaleX = 1.0f;
            paint.TextSkewX = 0.0f;

            return paint;
        }

        internal static SKMatrix CreateWorldMatrixWithRotation(SKMatrix currentMatrix, float rotationAngle, float areaWidth, float areaHeight)
        {
            var halfWidth = 0.5f * areaWidth;
            var halfHeight = 0.5f * areaHeight;

            var matrix = currentMatrix;

            var translate1 = SKMatrix.CreateTranslation(halfWidth, halfHeight);
            var rotate = SKMatrix.CreateRotation(rotationAngle);
            var translate2 = SKMatrix.CreateTranslation(-halfWidth, -halfHeight);

            SKMatrix.Concat(ref matrix, ref matrix, ref translate1);
            SKMatrix.Concat(ref matrix, ref matrix, ref rotate);
            SKMatrix.Concat(ref matrix, ref matrix, ref translate2);

            return matrix;
        }
        
        internal static void DrawText(SKCanvas canvas, string text, float x, float y, SKPaint paint)
        {
            var lines = text.Split(lineSeparators, StringSplitOptions.None);
            var lineHeight = paint.FontMetrics.Descent - paint.FontMetrics.Ascent;

            y -= paint.FontMetrics.Ascent;
            foreach (var line in lines)
            {
                canvas.DrawText(line, x, y, paint);
                y += lineHeight;
            }
        }

        internal static SKFont GetOptimalFont(string text, Font font, float rotationAngle, float maxWidth, float maxHeight)
        {
            const int sizeStep = 16;
            const double guardIntervalRatio = 0.95;

            var width = maxWidth * guardIntervalRatio;
            var height = maxHeight * guardIntervalRatio;

            var weight = font.IsBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            var slant = font.IsItalic ? SKFontStyleSlant.Oblique : SKFontStyleSlant.Upright;

            using var typeface = SKTypeface.FromFamilyName(
                font.FontFamily,
                weight,
                SKFontStyleWidth.Normal,
                slant
                );
            
            bool IsValidSize(int currentSize)
            {
                using var currentFont = new SKFont(typeface, currentSize);

                var size = GetTextSize(text, currentFont);

                var points = new[] {
                    new SKPoint(0, 0),
                    new SKPoint(size.Width, 0),
                    new SKPoint(size.Width, size.Height),
                    new SKPoint(0, size.Height)
                };

                var transformedPoints =
                    CreateWorldMatrixWithRotation(SKMatrix.Identity, rotationAngle, maxWidth, maxHeight).
                    MapPoints(points);

                var transformedWidth = transformedPoints.Max(p => p.X) - transformedPoints.Min(p => p.X);
                var transformedHeight = transformedPoints.Max(p => p.Y) - transformedPoints.Min(p => p.Y);

                return width >= transformedWidth && height >= transformedHeight;
            }

            int size = 0, minSize = 0, maxSize = 0;

            while (true)
            {
                minSize = size;
                size += sizeStep;

                if (!IsValidSize(size))
                {
                    maxSize = size;
                    break;
                }
            }

            int averageSize = 0, optimalSize = minSize;

            while (true)
            {
                averageSize = (minSize + maxSize) / 2;

                if (averageSize == minSize || averageSize == maxSize)
                {
                    break;
                }

                if (IsValidSize(averageSize))
                {
                    minSize = averageSize;
                    optimalSize = averageSize;
                }
                else
                {
                    maxSize = averageSize;
                }
            }

            return new SKFont(typeface, optimalSize);
        }

        internal static SKSize GetTextSize(string text, SKFont font)
        {
            var lines = text.Split(lineSeparators, StringSplitOptions.None);
            var length = lines.Length;
            var bounds = new SKRect();
            float maxTextWidth = 0;
            int count = 0;

            using var paint = new SKPaint(font);
            var fontMetrics = paint.FontMetrics;

            for (int i = 0; i < length; i++)
            {
                var lineTextWidth = paint.MeasureText(lines[i], ref bounds);
                if (maxTextWidth < lineTextWidth)
                {
                    maxTextWidth = lineTextWidth;
                }
                count++;
            }

            return new SKSize(maxTextWidth, count * (fontMetrics.Descent - fontMetrics.Ascent));
        }

        internal static void SetColor(SKPaint paint, Color color)
        {
            paint.Color = new SKColor(color.R, color.G, color.B, color.A);
        }
    }
}