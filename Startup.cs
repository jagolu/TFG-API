﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ClockSkew = TimeSpan.FromMinutes(5),
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        //ValidIssuer = Configuration["Jwt:Issuer"],
                        //ValidAudience = Configuration["Jwt:Issuer"],
                        ValidIssuer = "https://localhost:4200/",
                        ValidAudience = "https://localhost:4200/",

                        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ejemplodekey"))
                    };
                });


            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options => {
                options.AddPolicy("_myAllowSpecificOrigins",
                    builder => {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDBContext context)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            app.UseCors("_myAllowSpecificOrigins");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();

            DBInitializer.Initialize(context);
        }
    }
}
