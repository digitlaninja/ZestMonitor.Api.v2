using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Services;

namespace ZestMonitor.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProposalPaymentsController : ControllerBase
    {
        private ProposalPaymentsService ProposalPaymentsService { get; }

        public ProposalPaymentsController(ProposalPaymentsService proposalPaymentsService)
        {
            if (proposalPaymentsService == null) throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.ProposalPaymentsService = proposalPaymentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProposals()
        {
            var proposals = await this.ProposalPaymentsService.GetProposals();
            if (proposals == null)
                return NotFound(new { Error = "Proposals found" });

            var result = proposals.ToList();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProposal([FromBody] ProposalPaymentsModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await this.ProposalPaymentsService.Create(model);
            return Ok(model);
        }

    }
}