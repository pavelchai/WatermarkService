/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.ComponentModel.DataAnnotations;

namespace WatermarkService.Models
{
    /// <summary>
    /// Watermark.
    /// </summary>
    public sealed class Watermark
    {
        /// <summary>
        /// Text of the watermark.
        /// </summary>
        [Required(ErrorMessage = "Text isn't specified")]
        public string Text { get; set; }
        
        /// <summary>
        /// Font for the watermark.
        /// </summary>
        [Required(ErrorMessage = "Font isn't specified")]
        public Font Font { get; set; }

        /// <summary>
        /// Background color of the text.
        /// </summary>
        [Required(ErrorMessage = "Background color isn't specified")]
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Foreground color of the text.
        /// </summary>
        [Required(ErrorMessage = "Foreground color isn't specified")]
        public Color ForegroundColor { get; set; }

        /// <summary>
        /// Count of repeating by X.
        /// </summary>
        [Required(ErrorMessage = "Repeat count (X) isn't specified"), Range(1, int.MaxValue, ErrorMessage = "Repeat count (X) should be in [1; 2147483647]")]
        public int RepeatCountX { get; set; }

        /// <summary>
        /// Count of repeating by Y.
        /// </summary>
        [Required(ErrorMessage = "Repeat count (Y) isn't specified"), Range(1, int.MaxValue, ErrorMessage = "Repeat count (Y) should be in [1; 2147483647]")]
        public int RepeatCountY { get; set; }

        /// <summary>
        /// Rotation angle for the text (in radians).
        /// </summary>
        [Required(ErrorMessage = "Rotation angle isn't specified")]
        public double RotationAngle { get; set; }
    }
}