using Database.Api.Extensions;
using Database.Api.Middleware;
using Database.Core;
using Database.Core.Authentication;
using Database.Core.Users;
using Database.Data;
using Database.Data.Seeder;
using Database.Models.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace Database.Api
{
    public class Startup
    {
        private readonly Logger logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger));

            services.AddDbContext<DatabaseDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseDbContext")), ServiceLifetime.Scoped);

            services.AddIdentityServices(Configuration);

            services.AddTransient<DatabaseSeeder>();
            services.AddTransient(typeof(IService<>), typeof(Service<>));
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IUserService, UserService>();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                using var scope = app.ApplicationServices.CreateScope();

                var seeder = scope.ServiceProvider.GetService<DatabaseSeeder>();

                seeder.Seed().Wait();
            }

            var allowed = Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
            app.UseCors(builder => builder.WithOrigins(allowed).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Values}/{action=Get}/{id?}");
            });
        }
    }
}
