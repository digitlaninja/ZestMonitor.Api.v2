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
        public async Task<IActionResult> GetProposals(int page = 1, int limit = 10)
        {
            var proposals = await this.ProposalPaymentsService.GetPaged(page, limit);
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

            var created = await this.ProposalPaymentsService.Create(model);
            if (!created)
                return BadRequest();

            return StatusCode(201, model);
        }

    }
}