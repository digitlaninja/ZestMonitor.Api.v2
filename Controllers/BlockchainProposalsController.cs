using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZestMonitor.Api.Data.Models;
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
        public async Task<IActionResult> GetBlockchainProposals()
        {
            var result = await this.BlockchainService.GetProposals();
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("{name:proposalname}")]
        public async Task<IActionResult> GetBlockchainProposal([FromRoute]string name)
        {
            var result = await this.BlockchainService.GetProposal(name);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetProposalMetadata()
        {
            var result = await this.BlockchainService.GetProposalMetadata();
            if (result == null)
                return BadRequest();

            return Ok(result);
        }
    }
}