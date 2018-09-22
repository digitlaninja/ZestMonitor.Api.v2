using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Extensions;
using ZestMonitor.Api.Helpers;
using ZestMonitor.Api.Services;

// Mainly contains logic used by the Admin project
namespace ZestMonitor.Api.Controllers
{
    [Authorize]
    [Route("api/proposalpayments")]
    public class ProposalPaymentsController : ControllerBase
    {
        private ProposalPaymentsService ProposalPaymentsService { get; }

        public ProposalPaymentsController(ProposalPaymentsService proposalPaymentsService)
        {
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
        }

        [HttpGet]
        public async Task<IActionResult> GetProposalPayments([FromQuery] PagingParams pagingParams)
        {
            var result = await this.ProposalPaymentsService.GetPaged(pagingParams);
            if (result == null)
                return NotFound();

            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet("{hash}")]
        public async Task<IActionResult> GetProposalPayment(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return BadRequest();

            var result = await this.ProposalPaymentsService.Get(hash);
            if (result == null)
                return NotFound();

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

        [HttpDelete("{hash}")]
        public async Task<IActionResult> DeleteProposal([FromRoute] string hash)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deleted = await this.ProposalPaymentsService.Delete(hash);
            if (!deleted)
                return BadRequest();

            return NoContent();
        }
    }
}