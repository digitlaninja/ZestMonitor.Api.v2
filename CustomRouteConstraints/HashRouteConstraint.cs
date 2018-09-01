using Microsoft.AspNetCore.Routing.Constraints;

namespace ZestMonitor.Api.CustomRouteConstraints
{
    public class ProposalNameRouteConstraint : RegexRouteConstraint
    {
        public ProposalNameRouteConstraint() : base(@"^[a-zA-Z0-9_-]*$")
        {
        }
    }
}