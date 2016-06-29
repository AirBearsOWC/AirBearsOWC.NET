﻿using AirBears.Web.Filters;
using AirBears.Web.Models;
using AirBears.Web.Profiles;
using AirBears.Web.Services;
using AirBears.Web.Settings;
using AutoMapper;
using Braintree;
using Microsoft.AspNet.Authentication.JwtBearer;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IdentityModel.Tokens;
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
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            AuthKey = new RsaSecurityKey(KeyContainer.GetKeyFromContainer("AirBearsAuthContainer"));
            //AuthKey = new RsaSecurityKey(RsaKeyUtils.GetRandomKey());
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CommonProfile());
                Mapper.AssertConfigurationIsValid();
            });
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

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
                i.Password.RequireNonLetterOrDigit = false;
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

            services.AddSingleton(sp => MapperConfiguration.CreateMapper());
            services.AddInstance(GetBraintreeGateway());
            services.AddInstance(GetTokenAuthOptions());

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
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<AppDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<AppDbContext>().EnsureSeedData();
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

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

            app.UseJwtBearerAuthentication(options =>
            {
                // Basic settings - signing key to validate with, audience and issuer.
                options.TokenValidationParameters.IssuerSigningKey = AuthKey;
                options.TokenValidationParameters.ValidAudience = TokenAudience;
                options.TokenValidationParameters.ValidIssuer = TokenIssuer;

                // When receiving a token, check that we've signed it.
                options.TokenValidationParameters.ValidateSignature = true;

                // When receiving a token, check that it is still valid.
                options.TokenValidationParameters.ValidateLifetime = true;

                // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                // machines which should have synchronised time, this can be set to zero. Where external tokens are
                // used, some leeway here could be useful.
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
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
                Environment = Configuration["Authentication:Braintree:Environment"].Equals("production", StringComparison.InvariantCultureIgnoreCase)
                                    ? Braintree.Environment.PRODUCTION : Braintree.Environment.SANDBOX,
                MerchantId = Configuration["Authentication:Braintree:MerchantId"],
                PublicKey = Configuration["Authentication:Braintree:PublicKey"],
                PrivateKey = Configuration["Authentication:Braintree:PrivateKey"]
            };
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
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
