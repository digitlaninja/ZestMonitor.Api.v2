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
            services.AddSingleton<IValidator<UserLoginModel>, UserLoginValidator>();
            services.AddSingleton<IValidator<ProposalMetadataModel>, ProposalMetaDataValidator>();

            services.AddScoped<ProposalPaymentsService>();
            services.AddScoped<LocalBlockchainService>();
            services.AddScoped<BlockchainService>();
            services.AddScoped<AuthService>();

            services.AddScoped<IProposalPaymentsRepository, ProposalPaymentsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILocalBlockchainRepository, LocalBlockchainRepository>();
            services.AddScoped<IBlockchainRepository, BlockchainRepository>();
            services.AddScoped<IMasternodeCountRepository, MasternodeCountRepository>();
            services.AddScoped<IBlockCountRepository, BlockCountRepository>();

            // services.AddScoped<IRepository<IProposalPaymentsRepository>, Repository<IProposalPaymentsRepository>>();
            // services.AddScoped<IRepository<IBlockchainRepository>, Repository<IBlockchainRepository>>();
            // services.AddScoped<IRepository<IUserRepository>, Repository<IUserRepository>>();

        }
    }
}