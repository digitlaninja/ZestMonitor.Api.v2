using System.Collections.Generic;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using System;

public static class ProposalPaymentsFactory
{
    public static ProposalPaymentsModel ToModel(this ProposalPayments entity)
    {
        if (entity == null)
            throw new System.ArgumentNullException(nameof(entity));

        return new ProposalPaymentsModel()
        {
            Hash = entity.Hash,
            ShortDescription = entity.ShortDescription,
            Amount = entity.Amount,
            ExpectedPayment = entity.ExpectedPayment
        };
    }

    public static IEnumerable<ProposalPaymentsModel> ToModel(this IEnumerable<ProposalPayments> entities)
    {
        if (entities == null)
            throw new System.ArgumentNullException(nameof(entities));

        List<ProposalPaymentsModel> model = new List<ProposalPaymentsModel>();

        foreach (var entity in entities)
        {
            model.Add(entity.ToModel());
        }

        return model;
    }

    public static ProposalPayments ToEntity(this ProposalPaymentsModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new ProposalPayments()
        {
            Hash = model.Hash,
            ShortDescription = model.ShortDescription,
            Amount = model.Amount,
            ExpectedPayment = model.ExpectedPayment
        };
    }
}