using FluentValidation.Attributes;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Data.Models
{
    [Validator(typeof(ProposalPaymentsValidator))]
    public class ProposalPaymentsModel
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string ShortDescription { get; set; }
        public int Amount { get; set; }
        public int ExpectedPayment { get; set; }
        public string CreatedAt { get; set; }
    }
}