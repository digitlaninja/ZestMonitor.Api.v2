using System;
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
                Ratio = jsonClass.Ratio,
                TotalPayment = jsonClass.TotalPayment,
                TotalPaymentCount = jsonClass.TotalPaymentCount,
                RemainingPaymentCount = jsonClass.RemainingPaymentCount,
                MonthlyPayment = jsonClass.MonthlyPayment,
                BlockStart = jsonClass.BlockStart,
                BlockEnd = jsonClass.BlockEnd,
                UpdatedAt = DateTime.Now
            };
            return entity;
        }

        public static MasternodeCount ToEntity(this MasternodeCountJson jsonClass)
        {
            if (jsonClass == null)
                throw new System.ArgumentNullException(nameof(jsonClass));

            var entity = new MasternodeCount()
            {
                Total = jsonClass.Total,
                Stable = jsonClass.Stable,
                ObfCompat = jsonClass.ObfCompat,
                Enabled = jsonClass.Enabled,
                InQueue = jsonClass.InQueue,
                IPv4 = jsonClass.IPv4,
                IPv6 = jsonClass.IPv6,
                Onion = jsonClass.Onion,
                UpdatedAt = DateTime.Now
            };
            return entity;
        }

        public static IEnumerable<BlockchainProposalModel> ToModels(this IEnumerable<BlockchainProposal> entities)
        {
            if (entities == null)
                throw new System.ArgumentNullException(nameof(entities));

            var models = new List<BlockchainProposalModel>();
            foreach (var entity in entities)
            {
                models.Add(entity.ToModel());
            }
            return models;
        }


        public static BlockchainProposalModel ToModel(this BlockchainProposal entity)
        {
            if (entity == null)
                throw new System.ArgumentNullException(nameof(entity));

            var model = new BlockchainProposalModel()
            {
                Name = entity.Name,
                Url = entity.Url,
                Hash = entity.Hash,
                FeeHash = entity.FeeHash,
                Yeas = entity.Yeas,
                Nays = entity.Nays,
                Abstains = entity.Abstains,
                IsEstablished = entity.IsEstablished ? "Yes" : "No",
                IsValid = entity.IsValid ? "Yes" : "No",
                IsValidReason = entity.IsValidReason,
                FValid = entity.FValid ? "Yes" : "No",
                Ratio = entity.Ratio,
                TotalPayment = entity.TotalPayment
            };
            return model;
        }



    }
}