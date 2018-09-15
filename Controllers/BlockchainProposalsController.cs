using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Extensions;
using ZestMonitor.Api.Helpers;
using ZestMonitor.Api.Services;

namespace ZestMonitor.Api.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainProposalsController : ControllerBase
    {
        private BlockchainService BlockchainService { get; }

        public BlockchainProposalsController(BlockchainService blockchainService)
        {
            this.BlockchainService = blockchainService ?? throw new ArgumentNullException(nameof(blockchainService));
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockchainProposals([FromQuery] PagingParams pagingParams)
        {
            var result = await this.BlockchainService.GetPagedProposals(pagingParams);
            if (result == null)
                return BadRequest();
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet("{name:proposalname}")]
        public IActionResult GetBlockchainProposal([FromRoute]string name)
        {
            var result = this.BlockchainService.GetLocalProposal(name);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("metadata")]
        public IActionResult GetProposalMetadata()
        {
            var result = this.BlockchainService.GetProposalMetadata();
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}