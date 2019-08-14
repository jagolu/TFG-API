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
using API.ScheduledTasks.Weekly;
using API.Areas.Alive.Util;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ClockSkew = TimeSpan.FromMinutes(10),
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );


            services.AddCors(options => {
                options.AddPolicy("myCORS", builder => {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithMethods(new string[2] { "GET", "POST" })
                        .AllowCredentials();
                });
            });

            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "webInterface/dist";
            });

            services.AddHttpClient();
            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddHostedService<InitializeVirtualDBHostedService>(); //Comment for developing
            //services.AddHostedService<PayFootballBetHostedService>(); //Comment for developing
            services.AddHostedService<WeeklyGroupHostedService>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDBContext context)
        {
            app.UseCors("myCORS");
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();


            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatter");
                routes.MapHub<NotificationHub>("/notificatter");
            });
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            

            EmailSender.Initialize(Configuration);
            TokenGenerator.Initialize(Configuration);
            PasswordHasher.Initialize(Configuration);
            SendNotification.Initialize(Configuration);
            KickChatNotification.Initialize(Configuration);
            DBInitializer.Initialize(context);

            app.UseSpa(spa => spa.Options.SourcePath = "webInterface");
        }
    }
}
