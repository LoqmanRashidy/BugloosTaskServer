using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CommonLayer;
using Datalayer.ViewModels;
using DataLayer.Context;
using DataLayer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceLayer;
using ServiceLayer.BaseSystem;
using ServiceLayer.Users;
using ServicesLayer.BaseSystem;
using ServicesLayer.Courses;
using ServicesLayer.Users;

namespace WebApp
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
            services.AddHealthChecks();

            services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
                options.EnableForHttps = true;
                options.MimeTypes = new[] { "text/plain", "text/css", "application/javascript", "text/html", "application/xml", "text/xml", "application/json", "text/json", "image/svg+xml" };
            });

            services.AddOptions<BearerTokensOptions>()
                .Bind(Configuration.GetSection("BearerTokens"))
                .Validate(bearerTokens =>
                {
                    return bearerTokens.AccessTokenExpiration < bearerTokens.RefreshTokenExpiration;
                }, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
            
            services.AddOptions<ApiSettings>()
                .Bind(Configuration.GetSection("ApiSettings"));

            services.AddOptions<UploadDownloadRoot>()
            .Bind(Configuration.GetSection("UploadDownloadRoot"));

            services.AddTransient<ApplicationDbContext>().AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                    .Replace("|DataDirectory|", Path.Combine(Directory.GetCurrentDirectory())));

            });



            #region Services

            #region System

            services.AddScoped(typeof(IAsyncService<>), typeof(GenericService<>));
            #endregion System

            #region Auth - User

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddScoped<IDbInitializerService, DbInitializerService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<ITokenFactoryService, TokenFactoryService>();

            #endregion Auth - User

            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IStudentCourseService, StudentCourseService>();;


            #endregion Services 




            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
                options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomPolicy.Admin, policy => policy.RequireRole(CustomRoles.Admin));
                options.AddPolicy(CustomPolicy.User, policy => policy.RequireRole(CustomRoles.User));
                options.AddPolicy(CustomPolicy.Teacher, policy => policy.RequireRole(CustomRoles.Teacher));
                options.AddPolicy(CustomPolicy.Student, policy => policy.RequireRole(CustomRoles.Student));
                options.AddPolicy(CustomPolicy.AdminTeacherStudent, policy => policy.RequireRole(CustomRoles.Admin, CustomRoles.Teacher, CustomRoles.Student));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cgf =>
            {
                cgf.RequireHttpsMetadata = false;
                cgf.SaveToken = true;
                cgf.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["BearerTokens:Issuer"], // site that makes the token
                    ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                    ValidAudience = Configuration["BearerTokens:Audience"], // site that consumes the token
                    ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                    ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                    ValidateLifetime = true, // validate the expiration
                    ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                };
                cgf.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                     {
                         var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                         logger.LogError("Authentication failed.", context.Exception);
                         return Task.CompletedTask;
                     },
                    OnTokenValidated = context =>
                    {
                        var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                        return tokenValidatorService.ValidateAsync(context);
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge= context=>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("BuglossPolicy", builder => builder
                            .WithOrigins(
                                   "http://localhost:4200",
                                   "http://localhost:4300",
                                    "https://localhost:4200",
                                   "https://localhost:4300"
                         )
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            });

            services.AddAntiforgery(x => x.HeaderName = "X-XSRF-TOKEN");

        
            services.AddControllers()
             .AddNewtonsoftJson(options =>
             {
                 options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                 options.SerializerSettings.ContractResolver = new DefaultContractResolver();
             });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseCors();
            //app.UseCors("StorPolicy");
            app.UseHttpsRedirection();

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            provider.Mappings[".woff"] = "application/font-woff";
            provider.Mappings[".woff2"] = "application/font-woff";
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            var cultureInfo = new CultureInfo("fa-IR");
            cultureInfo.NumberFormat.CurrencySymbol = "ریال";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Course")),
                RequestPath = "/Course"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "UploadFiles/Person")),
                RequestPath = "/Person"
            });
           
            
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = 401,
                            Msg = "token expired"
                        }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = 500,
                            Msg = error.Error.Message
                        }));
                    }
                    else
                    {
                        await next();
                    }
                });

            });


            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseResponseCompression();
            app.UseRouting();
            app.UseCors("BuglossPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("NewRoute", "api/{controller}/{action}",
                    defaults: new { controller = "WeatherForecast", action = "Get" });
                endpoints.MapControllerRoute("default", "{controller=WeatherForecast}/{action=Get}/{id?}");
               

            });
         

            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            });
        }
    }
}
