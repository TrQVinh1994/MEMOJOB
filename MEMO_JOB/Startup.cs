using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MEMO_JOB.Data;
using MEMO_JOB.Models;
using MEMO_JOB.Models.JobSeekers;
using MEMO_JOB.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using MEMO_JOB.Extensions;

namespace MEMO_JOB
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
     

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = @"Server=TRQVINH68;Database=MEMO_JOB;Trusted_Connection=True;User Id=sa;Password=123";
            
            //Seeker
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connection));            
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = false;
                
                config.SignIn.RequireConfirmedEmail = true;
                config.SignIn.RequireConfirmedPhoneNumber = false;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
         

            //
            services.AddDbContext<MEMO_JOBContext>(options =>
            options.UseSqlServer(connection));

            // Add application services.
            services.AddMvc();
            //services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddTransient<IEmailSender, EmailSender>();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //         .AddCookie();

            // Application's cookie settings
            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.Name = "YourAppCookieName";
            //    options.Cookie.HttpOnly = true;
            //    //options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //    options.LoginPath = "/Account/Login";// If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
            //    options.LogoutPath = "/Account/Logout";// If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
            //    options.AccessDeniedPath = "/Account/AccessDenied";// If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
            //    options.SlidingExpiration = true;
            //    options.ExpireTimeSpan = TimeSpan.FromDays(30);
            //    // Requires `using Microsoft.AspNetCore.Authentication.Cookies;`
            //    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            //});
            services.AddScoped<GetRoleLogin>();
            services.AddSingleton<MEMO_JOBContext>();
            services.AddScoped<MEMO_JOBContext>();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                //facebookOptions.AppId = Configuration["AppId"];
                //facebookOptions.AppSecret = Configuration["AppSecret"];
                facebookOptions.AppId = "1858406497755252";
                facebookOptions.AppSecret = "455bfb8e292679c17b7ee2992980f3f2";
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["ClientId"];
                googleOptions.ClientSecret = Configuration["ClientSecret"];
            });

            services.AddSingleton(Configuration);
            services.AddScoped<ValidateReCaptchaAttribute>();

            services.AddAuthorization(options =>
            {

                options.AddPolicy("JobSeekerUser",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("JobSeekerUser");
                    });

            });
            services.AddAuthorization(options =>
            {

                options.AddPolicy("RecruiterUser",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("RecruiterUser");
                    });

            });
            services.AddAuthorization(options =>
            {

                options.AddPolicy("SubRecruiterUser",
                    authBuilder =>
                    {
                        authBuilder.RequireRole("SubRecruiterUser");
                    });

            });
        }
        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Administrator", "Recruiter", "Seeker","SubRecruiter" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //if (!await RoleManager.RoleExistsAsync("Seeker"))
            //{
            //    var role = new IdentityRole("Seeker");
            //   roleResult = await RoleManager.CreateAsync(role);
            //}

            //Here you could create a super user who will maintain the web app
            //var poweruser = new ApplicationUser
            //{

            //    UserName = "abc@gmail.com",
            //    Email = "abc@gmail.com",
            //};
            ////Ensure you have these values in your appsettings.json file
            //string userPWD = "anhnokiaz";
            //var _user = await UserManager.FindByEmailAsync("abc@gmail.com");

            //if (_user == null)
            //{
            //    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
            //    if (createPowerUser.Succeeded)
            //    {
            //        //here we tie the new user to the role
            //        await UserManager.AddToRoleAsync(poweruser, "Administrator");

            //    }
            //}
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=HomeJobSeekers}/{action=Index}/{id?}");
            });
            CreateRoles(serviceProvider).Wait();
        }
    }
}
