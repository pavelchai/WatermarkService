/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WatermarkService.Models;

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
        /// Adds a watermark to the images.
        /// </summary>
        /// <param name="request"> Request info. </param>
        /// <returns> Response info. </returns>
        [HttpPost]
        [ProducesResponseType(typeof(WatermarkServiceResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(WatermarkServiceRequest request)
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

            var response = await Task.Run(() => WatermarkServiceProcessor.Process(request));
            return new ObjectResult(response);
        }
    }
}