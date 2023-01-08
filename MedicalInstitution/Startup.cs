using MedicalInstitution.Data;
using MedicalInstitution.Middleware;
using MedicalInstitution.Models;
using MedicalInstitution.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalInstitution
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
            services.AddMvc();
            services.AddControllersWithViews();
            services.AddDbContext<Context>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            string connectionUsers = Configuration.GetConnectionString("AccountConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionUsers));
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddResponseCompression(options => options.EnableForHttps = true);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICached<AppUser>, CachedUser>();
            services.AddScoped<ICached<CostMediciane>, CachedCostMediciane>();
            services.AddScoped<ICached<Disease>, CachedDisease>();
            services.AddScoped<ICached<Doctor>, CachedDoctor>();
            services.AddScoped<ICached<Disease>, CachedDisease>();
            services.AddScoped<ICached<Patient>, CachedPatient>();
            services.AddScoped<ICached<Therapy>, CachedTherapy>();
            services.AddScoped<ICached<Medician>, CachedMedician>();
            services.AddSession();
            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context context, ApplicationDbContext applicationDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseRouting();
            Intializer.Intialize(context);
            app.UseDbInitializer();
            app.UseAuthentication();
            app.UseAuthorization();
            applicationDbContext.GetService<ICached<AppUser>>().AddList("User");
            context.GetService<ICached<CostMediciane>>().AddList("CachedCostMediciane");
            context.GetService<ICached<Disease>>().AddList("CachedDisease");
            context.GetService<ICached<Doctor>>().AddList("CachedDoctor");
            context.GetService<ICached<Medician>>().AddList("CachedMedician");
            context.GetService<ICached<Patient>>().AddList("CachedPatient");
            context.GetService<ICached<Therapy>>().AddList("TherapyPatient");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Therapy}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
