using FluentValidation;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Validation
{
    public class ProposalMetaDataValidator : AbstractValidator<ProposalMetadataModel>
    {
        public ProposalMetaDataValidator()
        {
            var message = "No Proposals found";
            RuleFor(x => x.FundedProposalCount).NotNull().WithMessage(message).GreaterThan(-1).WithMessage(message);
            RuleFor(x => x.ValidProposalCount).NotNull().WithMessage(message).GreaterThan(-1).WithMessage(message);
        }
    }
}