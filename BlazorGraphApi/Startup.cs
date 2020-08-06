using BlazorGraphApi.Data;
using BlazorGraphApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;

namespace BlazorGraphApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // replace this line
            //services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
            //.AddAzureAD(options => Configuration.Bind("AzureAd", options));

            // with this
            string[] scopes = Configuration.GetValue<string>("CalledApi:CalledApiScopes")?.Split(' ');
            services.AddMicrosoftWebAppAuthentication(Configuration, "AzureAd")
                    .AddMicrosoftWebAppCallsWebApi(Configuration,
                                                   scopes,
                                                   "AzureAd")
                    .AddInMemoryTokenCaches();
            services.AddDownstreamWebApiService(Configuration);
            services.AddMicrosoftGraph(scopes,
                                       Configuration.GetValue<string>("CalledApi:CalledApiUrl"));


            // Added AddMicrosoftIdentityUI()
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            services.AddRazorPages();
            // Add consent handler
            services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
            
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
