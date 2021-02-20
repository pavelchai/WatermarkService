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
    /// Color.
    /// </summary>
    public sealed class Color
    {
        /// <summary>
        /// Alpha component of the color.
        /// </summary>
        [Required(ErrorMessage = "A isn't specified"), Range(0, 255, ErrorMessage = "A should be in [0; 255]")]
        public byte A { get; set; }

        /// <summary>
        /// Red component of the color.
        /// </summary>
        [Required(ErrorMessage = "R isn't specified"), Range(0, 255, ErrorMessage = "R should be in [0; 255]")]
        public byte R { get; set; }

        /// <summary>
        /// Green component of the color.
        /// </summary>
        [Required(ErrorMessage = "G isn't specified"), Range(0, 255, ErrorMessage = "G should be in [0; 255]")]
        public byte G { get; set; }

        /// <summary>
        /// Blue component of the color.
        /// </summary>
        [Required(ErrorMessage = "B isn't specified"), Range(0, 255, ErrorMessage = "B should be in [0; 255]")]
        public byte B { get; set; }
    }
}