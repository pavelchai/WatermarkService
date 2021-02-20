/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WatermarkService.Models;
using System.Linq;

namespace WatermarkService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "WatermarkService")]
    public sealed class WatermarkServiceController : ControllerBase
    {
        /// <summary>
        /// Gets a list of the avaliable font families.
        /// </summary>
        /// <returns> List of the avaliable font families. </returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return WatermarkServiceProcessor.GetFontFamilies();
        }

        /// <summary>
        /// Adds a watermark to the image.
        /// </summary>
        /// <param name="request"> Request info. </param>
        /// <returns> Response info. </returns>
        [HttpPost]
        [ProducesResponseType(typeof(WatermarkServiceResponse), 200)]
        [ProducesResponseType(400)]
        public IActionResult Post(WatermarkServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string fontFamily = request.Watermark.Font.FontFamily;
            if (!Get().Any(f => f == fontFamily))
            {
                ModelState.AddModelError("FontFamily", "Invalid font family");
                return BadRequest(ModelState);
            }

            return new ObjectResult(WatermarkServiceProcessor.Process(request));
        }
    }
}