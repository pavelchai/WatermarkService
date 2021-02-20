/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;

namespace WatermarkService.Models
{
    /// <summary>
    /// Response.
    /// </summary>
    public sealed class WatermarkServiceResponse
    {
        /// <summary>
        /// Array of the data of the output images.
        /// </summary>
        public IEnumerable<Image> Images { get; set; }
    }
}