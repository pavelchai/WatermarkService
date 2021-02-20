/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WatermarkService.Models
{
    /// <summary>
    /// Request.
    /// </summary>
    public sealed class WatermarkServiceRequest
    {
        /// <summary>
        /// Watermark.
        /// </summary>
        [Required(ErrorMessage = "Watermark isn't specified")]
        public Watermark Watermark { get; set; }

        /// <summary>
        /// Array of the data of the input images.
        /// </summary>
        [Required(ErrorMessage = "Images aren't specified")]
        public IEnumerable<Image> Images { get; set; }
    }
}