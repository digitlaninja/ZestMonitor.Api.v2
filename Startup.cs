using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZestMonitor.Api.Data.Seed;
using ZestMonitor.Api.Data.Contexts;
using FluentValidation;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Validation;
using ZestMonitor.Api.Services;
using Microsoft.EntityFrameworkCore;
using GlobalExceptionHandler.WebApi;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Repositories;
using Microsoft.Extensions.Logging.Console;
using ZestMonitor.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Routing;
using ZestMonitor.Api.CustomRouteConstraints;
using Hangfire;
using Hangfire.MySql.Core;
using System.Data;

namespace ZestMonitor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ILogger _logger { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ZestContext>(x => x.UseMySql(Configuration["ConnectionStrings:Default"]));
            services.AddHangfire(config => config.UseStorage(new MySqlStorage(Configuration["ConnectionStrings:Default"])));
            services.AddScoped<Seed>();
            services.RegisterZestDependancies();
            services.AddMvc().AddFluentValidation();
            services.Configure<RouteOptions>(options =>

            options.ConstraintMap.Add("proposalname", typeof(ProposalNameRouteConstraint)));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                        p => p.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seed, ILoggerFactory loggerFactory, BlockchainService blockchainService)
        {
            app.UseCustomExceptionHandler(this._logger);
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.Map("/error", x => x.Run(y => throw new Exception()));
            loggerFactory.AddProvider(new ConsoleLoggerProvider((category, logLevel) => logLevel >= LogLevel.Critical, false));
            loggerFactory.AddConsole();

            this._logger = loggerFactory.CreateLogger<ConsoleLogger>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                if (!seed.ProposalPaymentsHasData())
                    seed.ProposalPayments();
            }
            else
            {
                // app.UseHsts();
            }

            // RecurringJob.AddOrUpdate(() => blockchainService.SaveBlockchainData(), "* * * * *");
            RecurringJob.AddOrUpdate(() => blockchainService.SaveBlockchainData(), "*/5 * * * *");

            app.UseCors("AllowAll");
            // Enforce Authentication configuration (jwt)
            app.UseAuthentication();
            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
