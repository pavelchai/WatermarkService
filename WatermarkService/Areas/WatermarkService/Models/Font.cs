/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.ComponentModel.DataAnnotations;

namespace WatermarkService.Models
{
    /// <summary>
    /// Font.
    /// </summary>
    public sealed class Font
    {
        /// <summary>
        /// Family of the font.
        /// </summary>
        [Required(ErrorMessage = "Font family isn't specified")]
        public string FontFamily { get; set; }

        /// <summary>
        /// Indicates whether font is italic.
        /// </summary>
        [Required(ErrorMessage = "IsItalic isn't specified")]
        public bool IsItalic { get; set; }

        /// <summary>
        /// Indicates whether font is bold.
        /// </summary>
        [Required(ErrorMessage = "IsBold isn't specified")]
        public bool IsBold { get; set; }
    }
}
