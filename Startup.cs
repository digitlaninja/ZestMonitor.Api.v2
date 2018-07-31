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

namespace ZestMonitor.Api
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ILogger _logger { get; set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ZestContext>(x => x.UseMySql(Configuration["ConnectionStrings:Default"]));
            services.AddTransient<Seed>();
            services.AddMvc().AddFluentValidation();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                        p => p.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
            });
            
            services.AddSingleton<IValidator<ProposalPaymentsModel>, ProposalPaymentsValidator>();
            services.AddTransient<ProposalPaymentsService>();
            services.AddTransient<IProposalPaymentsRepository, ProposalPaymentsRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IRepository<IProposalPaymentsRepository>, Repository<IProposalPaymentsRepository>>();
            services.AddTransient<IRepository<IUserRepository>, Repository<IUserRepository>>();
            services.AddTransient<IRepository<User>, Repository<User>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seed, ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddProvider(new ConsoleLoggerProvider((category, logLevel) => logLevel >= LogLevel.Information, false));
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

            app.UseCors("AllowAll");
            app.UseExceptionHandler("/error").WithConventions(x =>
            {
                x.ContentType = "application/json";
                x.MessageFormatter(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred whilst processing your request"
                }));

                x.ForException<DbUpdateException>().ReturnStatusCode(StatusCodes.Status500InternalServerError)
                .UsingMessageFormatter((ex, context) => JsonConvert.SerializeObject(new
                {
                    Message = "Sorry, we had an unexpected error, please contact the administrator."
                }));

                x.OnError((exception, httpContext) =>
                {
                    _logger.LogError(exception.Message);
                    return Task.CompletedTask;
                });

            });

            app.Map("/error", x => x.Run(y => throw new Exception()));

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
