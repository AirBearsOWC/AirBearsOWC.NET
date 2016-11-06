using AirBears.Web.Filters;
using AirBears.Web.Models;
using AirBears.Web.Profiles;
using AirBears.Web.Services;
using AirBears.Web.Settings;
using AutoMapper;
using Braintree;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AirBears.Web
{
    public class Startup
    {
        private const string TokenAudience = "AirBearsUsers";
        private const string TokenIssuer = "AirBearsAPI";

        private MapperConfiguration MapperConfiguration { get; set; }
        private RsaSecurityKey AuthKey { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            AuthKey = new RsaSecurityKey(KeyContainer.GetKeyFromContainer("AirBearsAuthContainer"));
            //AuthKey = new RsaSecurityKey(RsaKeyUtils.GetRandomKey());
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommonProfile());
            });

            //Mapper.AssertConfigurationIsValid();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Enable the use of an [Authorize("Bearer")] attribute on methods and classes to protect.
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(AuthPolicies.Bearer, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddIdentity<User, IdentityRole>(i =>
            {
                // configure identity password policy options
                i.Password.RequireDigit = true;
                i.Password.RequireLowercase = true;
                i.Password.RequireUppercase = true;
                i.Password.RequireNonAlphanumeric = false;
                i.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddMvcOptions(options =>
            {
                options.Filters.Add(new NoCacheHeaderFilter());
            });

            // Add functionality to inject IOptions<T>
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("MiscSettings"));
            services.Configure<SmtpSettings>(Configuration.GetSection("Authentication:GmailPrimary"));
            services.Configure<RecaptchaSettings>(Configuration.GetSection("Authentication:Recaptcha"));

            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            //});

            services.AddSingleton(sp => MapperConfiguration.CreateMapper());
            services.AddSingleton(GetBraintreeGateway());
            services.AddSingleton(GetTokenAuthOptions());

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IGeocodeService, GeocodeService>();
            services.AddTransient<IMailer, Mailer>();
            services.AddTransient<ICaptchaService, RecaptchaService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<AppDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<AppDbContext>().EnsureSeedData(env.IsDevelopment());
                //serviceScope.ServiceProvider.GetService<AppDbContext>().InviteMigratedUsers(serviceScope.ServiceProvider.GetService<IMailer>());
            }

            // Register a simple error handler to catch token expiries and change them to a 401, 
            // and return all other errors as a 500. This should almost certainly be improved for
            // a real application.
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    // This should be much more intelligent - at the moment only expired 
                    // security tokens are caught - might be worth checking other possible 
                    // exceptions such as an invalid signature.
                    if (error != null && (error.Error is SecurityTokenExpiredException || error.Error is SecurityTokenInvalidSignatureException))
                    {
                        context.Response.StatusCode = 401;
                        // What you choose to return here is up to you, in this case a simple 
                        // bit of JSON to say you're no longer authenticated.
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new { authenticated = false, tokenExpired = true }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        // TODO: Shouldn't pass the exception message straight out, change this.
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject
                            (new { success = false, error = error.Error }));
                    }

                    // We're not trying to handle anything else so just let the default 
                    // handler handle.
                    else await next();
                });
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = TokenIssuer,

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = TokenAudience,

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            // Route all unknown requests to app root
            app.Use(async (context, next) =>
            {
                await next();

                // If there's no available file and the request doesn't contain an extension, we're probably trying to access a page.
                // Rewrite request to use app root
                if (!context.Request.Path.StartsWithSegments("/api/") && context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html"; // Put your Angular root page here 
                    context.Response.StatusCode = 200; // Make sure we update the status code, otherwise it returns 404
                    await next();
                }
            });

            if (!env.IsDevelopment())
            {
                app.Use(async (httpContext, next) =>
                {
                    if (!httpContext.Request.IsHttps)
                    {
                        httpContext.Response.Redirect("https://www.airbears.org" + (httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : string.Empty), true);
                        return;
                    }
                    await next();
                });
            }

            app.UseFileServer();
            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc();
        }

        private TokenAuthOptions GetTokenAuthOptions()
        {
            return new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(AuthKey, SecurityAlgorithms.RsaSha256Signature)
            };
        }

        private IBraintreeGateway GetBraintreeGateway()
        {
            return new BraintreeGateway
            {
                Environment = Configuration["Authentication:Braintree:Environment"].Equals("production", StringComparison.CurrentCultureIgnoreCase)
                                    ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX,
                MerchantId = Configuration["Authentication:Braintree:MerchantId"],
                PublicKey = Configuration["Authentication:Braintree:PublicKey"],
                PrivateKey = Configuration["Authentication:Braintree:PrivateKey"]
            };
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }

    public class TokenAuthOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }

    public class KeyContainer
    {
        public static RSAParameters GetKeyFromContainer(string containerName)
        {
            var cp = new CspParameters { KeyContainerName = containerName, Flags = CspProviderFlags.UseMachineKeyStore };
            var rsa = new RSACryptoServiceProvider(2048, cp);

            return rsa.ExportParameters(true);
        }
    }
}
