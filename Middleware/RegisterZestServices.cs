using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Repositories;
using ZestMonitor.Api.Services;
using ZestMonitor.Api.Utillities;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Middleware
{
    public static class RegisterZestServices
    {
        public static void RegisterZestDependancies(this IServiceCollection services)
        {
            services.AddScoped<ConvertHelpers>();

            services.AddSingleton<IValidator<ProposalPaymentsModel>, ProposalPaymentsValidator>();
            services.AddSingleton<IValidator<UserRegistrationModel>, UserRegistrationValidator>();
            
            services.AddScoped<ProposalPaymentsService>();
            services.AddScoped<AuthService>();
            
            services.AddScoped<IProposalPaymentsRepository, ProposalPaymentsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<IProposalPaymentsRepository>, Repository<IProposalPaymentsRepository>>();
            services.AddScoped<IRepository<IUserRepository>, Repository<IUserRepository>>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}