using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Services
{
    [Authorize]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        public ProposalPaymentsService ProposalPaymentsService { get;set; }

        public AdminController(ProposalPaymentsService ProposalPaymentsService) {
            this.ProposalPaymentsService = ProposalPaymentsService ?? throw new ArgumentNullException(nameof(ProposalPaymentsService));
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateProposal(ProposalPaymentsModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await this.ProposalPaymentsService.Create(model);
            return NoContent();
        }
    }
}