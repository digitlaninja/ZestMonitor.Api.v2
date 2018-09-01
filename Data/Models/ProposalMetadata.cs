using FluentValidation.Attributes;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Data.Models
{
    [Validator(typeof(ProposalMetaDataValidator))]
    public class ProposalMetadataModel
    {
        public int FundedProposalCount { get; set; }
        public int ValidProposalCount { get; set; }
    }
}