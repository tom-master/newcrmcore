using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewCRM.Domain.Services.BoundedContext;
using NewCRM.Domain.Services.Interface;
using NewCRM.Web.Filter;
using NewCrmCore.Application.Services;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Services.BoundedContext;
using NewCrmCore.Domain.Services.Interface;
using NewLibCore;

namespace NewCrmCore.Web
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
			services.AddTransient<IAccountServices, AccountServices>();
			services.AddTransient<ISecurityServices, SecurityServices>();
			services.AddTransient<IAppServices, AppServices>();
			services.AddTransient<IDeskServices, DeskServices>();
			services.AddTransient<IWallpaperServices, WallpaperServices>();
			services.AddTransient<ISkinServices, SkinServices>();
			services.AddTransient<ILoggerServices, LoggerServices>();

			services.AddTransient<IAccountContext, AccountContext>();
			services.AddTransient<IAppContext, NewCRM.Domain.Services.BoundedContext.AppContext>();
			services.AddTransient<IDeskContext, DeskContext>();
			services.AddTransient<ILoggerContext, LoggerContext>();
			services.AddTransient<IMemberContext, MemberContext>();
			services.AddTransient<ISecurityContext, SecurityContext>();
			services.AddTransient<ISkinContext, SkinContext>();
			services.AddTransient<IWallpaperContext, WallpaperContext>();

			AppSettings.Init(Configuration.GetSection("AppSetting"));

			services.AddMvc(config =>
			{
				//config.Filters.Add(new HandleErrorAttribute());
				config.Filters.Add(new ErrorFilter());
				config.Filters.Add(new AuthFilter());
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}



			app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "Default",
					template: "{controller}/{action}/{id?}",defaults: new { controller = "desktop", action = "index" });
			});
		}
	}
}
