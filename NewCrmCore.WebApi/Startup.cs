using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NewCrmCore.Application.Services;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Services.BoundedContext;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure;
using NewCrmCore.NotifyCenter;
using NewCrmCore.WebApi.Middleware;
using NewLibCore.Data.SQL.EMapper;

namespace NewCrmCore.WebApi
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
            EntityMapperConfig.InitDefaultSetting();
            EntityMapperConfig.ConnectionStringName = "NewCrmDatabase";

            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<ISecurityServices, SecurityServices>();
            services.AddTransient<IAppServices, AppServices>();
            services.AddTransient<IDeskServices, DeskServices>();
            services.AddTransient<IWallpaperServices, WallpaperServices>();
            services.AddTransient<ILoggerServices, LoggerServices>();
            services.AddTransient<CommonNotify>();

            services.AddTransient<IUserContext, UserContext>();
            services.AddTransient<IAppContext, Domain.Services.BoundedContext.AppContext>();
            services.AddTransient<IDeskContext, DeskContext>();
            services.AddTransient<ILoggerContext, LoggerContext>();
            services.AddTransient<IMemberContext, MemberContext>();
            services.AddTransient<ISecurityContext, SecurityContext>();
            services.AddTransient<IWallpaperContext, WallpaperContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = Appsetting.Domain,//Audience
                    ValidIssuer = Appsetting.Domain,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Appsetting.SecurityKey))//拿到SecurityKey
                };
            });

            services.AddControllers(config =>
            {
                config.SuppressAsyncSuffixInActionNames = false;
            }).AddJsonOptions(options =>
            {

                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                options.JsonSerializerOptions.Converters.Add(new NewCrmCore.Infrastructure.Converter.BooleanConverter());
                options.JsonSerializerOptions.Converters.Add(new NewCrmCore.Infrastructure.Converter.Int32Converter());
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseGlobalExceptionHandle();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotifyHub>("/hubs");
                endpoints.MapControllers();
                // endpoints.MapControllerRoute("Default", "{controller}/{action}/{id?}", defaults: new { controller = "desk", action = "index" });
            });
        }
    }
}
