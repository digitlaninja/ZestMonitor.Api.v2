using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
namespace ZestMonitor.Api.Services
{
    public class ProposalPaymentsService
    {
        private IProposalPaymentsRepository ProposalPaymentsRepository { get; }
        public ILogger<ProposalPaymentsService> Logger { get; }

        public ProposalPaymentsService(IProposalPaymentsRepository proposalPaymentsRepository, ILogger<ProposalPaymentsService> logger)
        {
            if (proposalPaymentsRepository == null) throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.ProposalPaymentsRepository = proposalPaymentsRepository;
            Logger = logger;
        }

        public async Task<IEnumerable<ProposalPaymentsModel>> GetAll()
        {
            var proposals = await this.ProposalPaymentsRepository.GetAll();
            return proposals.ToModel();
        }

        public async Task<IEnumerable<ProposalPaymentsModel>> GetPaged(int page = 1, int limit = 10)
        {
            var proposals = await this.ProposalPaymentsRepository.GetPaged(page, limit);
            return proposals.ToList()?.ToModel();
        }

        public async Task<bool> Create(ProposalPaymentsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));


            var entity = model.ToEntity();
            entity.CreatedAt = DateTime.Now;

            await this.ProposalPaymentsRepository.Add(entity);

            var result = await this.ProposalPaymentsRepository.SaveAll();
            if (!result)
                return false;

            return true;
        }

        public void GetBlockchainProposals()
        {
            // try {

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://localhost:51473");
            webRequest.Credentials = new NetworkCredential("replaced", "replaced");

            /// important, otherwise the service can't desirialse your request properly
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject jObject = new JObject();
            jObject.Add(new JProperty("jsonrpc", "1.0"));
            jObject.Add(new JProperty("id", "1"));
            jObject.Add(new JProperty("method", "getinfo"));

            // if (Params.Keys.Count == 0)
            // {
            // jObject.Add(new JProperty("params", new JArray()));
            // }
            // else
            // {
            // JArray props = new JArray();
            // // add the props in the reverse order!
            // for (int i = Params.Keys.Count - 1; i >= 0; i--)
            // {
            //     .... // add the params
            // }
            // jObject.Add(new JProperty("params", props));
            // }

            // // serialize json for the request
            // string s = JsonConvert.SerializeObject(jObject);
            // byte[] byteArray = Encoding.UTF8.GetBytes(s);
            // webRequest.ContentLength = byteArray.Length;
            // Stream dataStream = webRequest.GetRequestStream();
            // dataStream.Write(byteArray, 0, byteArray.Length);
            // dataStream.Close();


            // WebResponse webResponse = webRequest.GetResponse();
            // }
            // params is a collection values which the method requires..



            //             var x = 0;
            // }
            //             catch(Exception ex) {
            //                 this.Logger.LogCritical(ex, $"{ex.InnerException}");
            //             }
        }
    }
}