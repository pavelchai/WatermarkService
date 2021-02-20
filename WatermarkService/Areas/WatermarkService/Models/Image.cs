/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.ComponentModel.DataAnnotations;

namespace WatermarkService.Models
{
    /// <summary>
    /// Image.
    /// </summary>
    public sealed class Image
    {
        /// <summary>
        /// Name of the image.
        /// </summary>
        [Required(ErrorMessage = "Name isn't specified")]
        public string Name { get; set; }

        /// <summary>
        /// Data of the image (as Base64 string, with MIME type).
        /// </summary>
        [Required(ErrorMessage = "Data of the image (base64 + MIME type) isn't specified")]
        public string DataBase64 { get; set; }
    }
}