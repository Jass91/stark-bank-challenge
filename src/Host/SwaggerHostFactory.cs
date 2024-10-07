﻿using Microsoft.AspNetCore.Hosting;

namespace Stark
{
    public class SwaggerHostFactory
    {
        public static IHost CreateHost()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}
