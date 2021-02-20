/*
 * Watermark service.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WatermarkService.Models
{
    public static class WatermarkServiceUtils
    {
        public static void AddWatermarkServiceSwaggerDoc(this SwaggerGenOptions options)
        {
            options.SwaggerDoc("WatermarkService", new OpenApiInfo
            {
                Title = "Watermark Service",
                Version = "v1",
                Description = "The service adds the watermarks to the images",
                Contact = new OpenApiContact { Name = "Pavel Chaimardanov", Email = "pchai@yandex.ru" },
                License = new OpenApiLicense { Name = "Use under MIT", Url = new Uri("https://mit-license.org/") }
            });

            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var xmlFile = Path.GetFileName(assemblyName + ".xml");

            options.IncludeXmlComments(xmlFile);
        }

        public static void AddWatermarkServiceSwaggerEndpoint(this SwaggerUIOptions options)
        {
            options.SwaggerEndpoint("/swagger/WatermarkService/swagger.json", "WatermarkService");
        }

        public static void UseWatermarkServiceClient(this IApplicationBuilder app, bool redirectToPage = false)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (redirectToPage)
            {
                app.Use((context, task) => {
                    var next = task();

                    if (context.Request.Path == "/")
                    {
                        context.Response.Redirect("/WatermarkService/");
                    }

                    return next;
                });
            }
        }
    }
}