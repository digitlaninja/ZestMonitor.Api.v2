using System;
using System.Threading.Tasks;
using FluentValidation.Attributes;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Data.Models
{
    [Validator(typeof(ProposalMetaDataValidator))]
    public class ProposalMetadataModel
    {
        public int FundedProposalAmount { get; set; }
        public int FundedProposalCount { get; set; }
        public int ValidProposalCount { get; set; }
        public string VoteDeadline { get; set; }
        public double? DaysLeft { get; set; }
    }
}