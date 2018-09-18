using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Services;

namespace ZestMonitor.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        public ManualProposalPaymentsService ProposalPaymentsService { get; set; }

        public AdminController(ManualProposalPaymentsService ProposalPaymentsService)
        {
            this.ProposalPaymentsService = ProposalPaymentsService ?? throw new ArgumentNullException(nameof(ProposalPaymentsService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProposal(ProposalPaymentsModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await this.ProposalPaymentsService.Create(model);
            return NoContent();
        }
    }
}