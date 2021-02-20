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

        internal static IEnumerable<string> GetFontFamilies()
        {
            return SKFontManager.Default.FontFamilies;
        }

        internal static SKPaint CreatePaint()
        {
            SKPaint paint = new SKPaint();
            paint.FilterQuality = SKFilterQuality.High;
            paint.IsAntialias = true;
            return paint;
        }

        internal static SKPaint CreatePaint(SKFont font)
        {
            SKPaint paint = CreatePaint();

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

            SKMatrix matrix = currentMatrix;

            SKMatrix translate1 = SKMatrix.CreateTranslation(halfWidth, halfHeight);
            SKMatrix rotate = SKMatrix.CreateRotation(rotationAngle);
            SKMatrix translate2 = SKMatrix.CreateTranslation(-halfWidth, -halfHeight);

            SKMatrix.Concat(ref matrix, ref matrix, ref translate1);
            SKMatrix.Concat(ref matrix, ref matrix, ref rotate);
            SKMatrix.Concat(ref matrix, ref matrix, ref translate2);

            return matrix;
        }
        
        internal static void DrawText(SKCanvas canvas, string text, float x, float y, SKPaint paint)
        {
            IEnumerable<string> lines = text.Split(lineSeparators, StringSplitOptions.None);
            y -= paint.FontMetrics.Ascent;
            float lineHeight = paint.FontMetrics.Descent - paint.FontMetrics.Ascent;

            foreach (string line in lines)
            {
                canvas.DrawText(line, x, y, paint);
                y += lineHeight;
            }
        }

        internal static SKFont GetOptimalFont(string text, Font font, float rotationAngle, float maxWidth, float maxHeight)
        {
            const double guardIntervalRatio = 0.95;

            var width = maxWidth * guardIntervalRatio;
            var height = maxHeight * guardIntervalRatio;

            var weight = font.IsBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
            var slant = font.IsItalic ? SKFontStyleSlant.Oblique : SKFontStyleSlant.Upright;

            SKTypeface typeface = SKTypeface.FromFamilyName(
                font.FontFamily,
                weight,
                SKFontStyleWidth.Normal,
                slant
                );

            int optimalSize = 1;

            while (true)
            {
                int currentSize = optimalSize + 1;
                SKFont currentFont = new SKFont(typeface, currentSize);

                SKSize size = GetTextSize(text, currentFont);

                SKPoint[] points = new[] {
                    new SKPoint(0, 0),
                    new SKPoint(size.Width, 0),
                    new SKPoint(size.Width, size.Height),
                    new SKPoint(0, size.Height)
                };

                SKPoint[] transformedPoints =
                    CreateWorldMatrixWithRotation(SKMatrix.Identity, rotationAngle, maxWidth, maxHeight).
                    MapPoints(points);

                double transformedWidth = transformedPoints.Max(p => p.X) - transformedPoints.Min(p => p.X);
                double transformedHeight = transformedPoints.Max(p => p.Y) - transformedPoints.Min(p => p.Y);

                if (width < transformedWidth || height < transformedHeight)
                {
                    break;
                }

                currentFont.Dispose();
                optimalSize = currentSize;
            }

            return new SKFont(typeface, optimalSize);
        }

        internal static SKSize GetTextSize(string text, SKFont font)
        {
            string[] lines = text.Split(lineSeparators, StringSplitOptions.None);

            SKRect bounds = new SKRect();
            SKFontMetrics fontMetrics = default(SKFontMetrics);
            float maxTextWidth = 0;
            int count = 0;

            using (var paint = new SKPaint(font))
            {
                int length = lines.Length;

                for (int i = 0; i < length; i++)
                {
                    float lineTextWidth = paint.MeasureText(lines[i], ref bounds);
                    if (maxTextWidth < lineTextWidth)
                    {
                        maxTextWidth = lineTextWidth;
                    }
                    count++;
                }

                fontMetrics = paint.FontMetrics;
            }

            return new SKSize(maxTextWidth, count * (fontMetrics.Descent - fontMetrics.Ascent));
        }

        internal static void SetColor(SKPaint paint, Color color)
        {
            paint.Color = new SKColor(color.R, color.G, color.B, color.A);
        }
    }
}