using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Util;
using API.ScheduledTasks.VirtualBets;
using Microsoft.Extensions.Logging;
using API.ScheduledTasks.InitializeNextMatchDay;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ClockSkew = TimeSpan.FromMinutes(5),
                        //ClockSkew = TimeSpan.FromSeconds(5),  //Debug
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        //ValidAudience = Configuration["Jwt:Issuer"],

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection")
                )
            );


            services.AddCors(options => {
                options.AddPolicy("_myAllowSpecificOrigins",
                    builder => {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "webInterface/dist";
            });

            services.AddHttpClient();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHostedService<InitializeVirtualDBHostedService>();
            services.AddHostedService<UpdateCompetitionHostedService>();

            _logger.LogInformation("Added services");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDBContext context)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseCors("_myAllowSpecificOrigins");
            }
            else {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            

            EmailSender.Initialize(Configuration);
            TokenGenerator.Initialize(Configuration);
            PasswordHasher.Initialize(Configuration);
            
            DBInitializer.Initialize(context);

            app.UseSpa(spa => {
                spa.Options.SourcePath = "webInterface";
            });
        }
    }
}
