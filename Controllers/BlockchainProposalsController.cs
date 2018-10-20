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
        private LocalBlockchainService LocalBlockchainService { get; }
        private BlockchainService BlockchainService { get; }

        public BlockchainProposalsController(LocalBlockchainService localBlockchainService, BlockchainService blockchainService)
        {
            this.LocalBlockchainService = localBlockchainService ?? throw new ArgumentNullException(nameof(localBlockchainService));
            this.BlockchainService = blockchainService ?? throw new ArgumentNullException(nameof(blockchainService));
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockchainProposals([FromQuery] PagingParams pagingParams)
        {
            var result = await this.LocalBlockchainService.GetPagedProposals(pagingParams);
            if (result == null)
                return BadRequest();
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet("{name:proposalname}")]
        public async Task<IActionResult> GetBlockchainProposal([FromRoute]string name)
        {
            var result = await this.LocalBlockchainService.GetProposal(name);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetProposalMetadata()
        {
            var result = await this.LocalBlockchainService.GetProposalMetadata();
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        // [HttpGet("save")]
        // public async Task Save()
        // {
        //     await this.BlockchainService.SaveBlockchainData();
        // }

    }
}