using System.Collections.Generic;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Factories
{
    public static class BlockchainFactory
    {
        public static IEnumerable<BlockchainProposal> ToEntities(this IEnumerable<BlockchainProposalJson> jsonClass)
        {
            if (jsonClass == null)
                throw new System.ArgumentNullException(nameof(jsonClass));

            List<BlockchainProposal> entity = new List<BlockchainProposal>();

            foreach (var json in jsonClass)
            {
                entity.Add(json.ToEntity());
            }

            return entity;
        }

        public static BlockchainProposal ToEntity(this BlockchainProposalJson jsonClass)
        {
            if (jsonClass == null)
                throw new System.ArgumentNullException(nameof(jsonClass));

            var entity = new BlockchainProposal()
            {
                Name = jsonClass.Name,
                Url = jsonClass.Url,
                Hash = jsonClass.Hash,
                FeeHash = jsonClass.FeeHash,
                Yeas = jsonClass.Yeas,
                Nays = jsonClass.Nays,
                Abstains = jsonClass.Abstains,
                IsEstablished = jsonClass.IsEstablished,
                IsValid = jsonClass.IsValid,
                IsValidReason = jsonClass.IsValidReason,
                FValid = jsonClass.FValid,
                Ratio = jsonClass.Ratio
            };
            return entity;
        }
    }
}