using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Services;

namespace ZestMonitor.Api.Controllers
{
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {

        private BlockchainService BlockchainService { get; }

        public BlockchainController(BlockchainService blockchainService)
        {
            this.BlockchainService = blockchainService ?? throw new ArgumentNullException(nameof(blockchainService));
        }

        [HttpGet("proposals")]
        public async Task<IActionResult> GetBlockchainProposals()
        {
            var result = await this.BlockchainService.GetProposals();
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetProposalMetadata()
        {
            var result = new ProposalMetadataModel()
            {
                ValidProposalCount = await this.BlockchainService.GetValidCount(),
                FundedProposalCount = await this.BlockchainService.GetFundedCount()
            };
            return Ok(result);
        }
    }
}