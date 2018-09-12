using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Factories;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Repositories
{
    public class BlockchainRepository : Repository<BlockchainProposal>, IBlockchainRepository
    {
        public BlockchainRepository(ZestContext context) : base(context) { }

        private readonly string GetProposalsCommand = "{ \"jsonrpc\": \"1.0\", \"id\":\"getproposals\", \"m`ethod\": \"mnbudget\",\"params\":[\"show\"]}";

        public async Task<List<BlockchainProposal>> GetLocalPagedBlockchainProposals()
        {
            return await this.GetAll().ToListAsync();
        }

        public List<BlockchainProposalJson> GetPagedProposals(PagingParams pagingParams)
        {
            var resultKey = this.ExecuteRPCCommand("mnbudget", new[] { "show" });
            var result = JsonConvert.DeserializeObject<List<BlockchainProposalJson>>(resultKey?.ToString());
            return result;
        }

        public DateTime? GetTime(string hash)
        {
            var resultKey = this.ExecuteRPCCommand("getrawtransaction", new object[] { hash, 1 });
            var timeKey = resultKey.SelectToken("time");
            var result = this.ToDateTime(timeKey);
            return result;
        }

        private DateTime? ToDateTime(JToken timeKey)
        {
            var time = JsonConvert.DeserializeObject(timeKey?.ToString());
            var result = Convert.ToDouble(time);

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(result).ToLocalTime();
            return dateTime;
        }

        public List<BlockchainProposalJson> GetProposals()
        {
            var resultKey = this.ExecuteRPCCommand("mnbudget", new[] { "show" });
            var result = JsonConvert.DeserializeObject<List<BlockchainProposalJson>>(resultKey?.ToString());
            return result;
        }

        public BlockchainProposalJson GetProposal(string name)
        {
            var resultKey = this.ExecuteRPCCommand("getbudgetinfo", new[] { name });
            var result = JsonConvert.DeserializeObject<BlockchainProposalJson>(resultKey?.ToString());
            return result;
        }

        public JToken ExecuteRPCCommand(string command, params object[] parameters)
        {
            HttpWebRequest request = this.CreateRequest(command);
            JObject jObject = this.CreateRequestJson(command, parameters);

            string s = JsonConvert.SerializeObject(jObject);
            // serialize json for the request
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            request.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException we)
            {
                throw;
            }
            WebResponse webResponse = null;
            try
            {
                using (webResponse = request.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    var responseData = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(responseData))
                        throw new ArgumentNullException($"{nameof(responseData)} is empty.");

                    var responseJObject = JsonConvert.DeserializeObject<JObject>(responseData);
                    var resultKey = responseJObject.SelectToken("result");
                    return resultKey;
                }
            }
            catch (WebException webex)
            {

                using (Stream str = webex.Response.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    var tempRet = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                    return null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public HttpWebRequest CreateRequest(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:51473");
            request.Method = "POST";
            request.ContentType = "application/json-rpc";
            request.ContentLength = json.Length;
            var auth = "user:pass";

            // encode auth for header
            auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
            request.Headers.Add("Authorization", "Basic " + auth);
            return request;
        }

        public async Task SaveProposals()
        {
            // get block proposals
            var blockProposals = this.GetProposals();

            // for each block proposal get the local proposal, if there isn't one, add it.
            foreach (var blockProposal in blockProposals)
            {
                var existingProposal = await this.Context.Set<BlockchainProposal>().FirstOrDefaultAsync(x => x.Hash == blockProposal.Hash);
                if (existingProposal == null)
                    await this.Add(blockProposal?.ToEntity());
            }
            await this.SaveAll();
        }

        private static DateTime? ToTime(JObject responseJObject)
        {
            var resultKey = responseJObject.SelectToken("result");
            var timeKey = resultKey.SelectToken("time");
            var time = JsonConvert.DeserializeObject(timeKey?.ToString());
            var result = Convert.ToDouble(time);

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(result).ToLocalTime();
            return dateTime;
        }

        private JObject CreateRequestJson(string command, object[] parameters)
        {
            JObject jObject = new JObject();
            jObject["jsonrpc"] = "1.0";
            jObject["id"] = "1";
            jObject["method"] = command;

            if (parameters != null)
            {
                if (parameters.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in parameters)
                    {
                        props.Add(p);
                    }
                    jObject.Add(new JProperty("params", props));
                }
            }
            return jObject;
        }

        public int GetValidCount()
        {
            var proposals = this.GetProposals();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsValid);
            return result;
        }

        public int GetFundedCount()
        {
            var proposals = this.GetProposals();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsEstablished);
            return result;
        }

        public ProposalMetadataModel GetMetadata()
        {
            var proposals = this.GetProposals();
            if (proposals == null)
                return null;

            var validCount = proposals.Count(x => x.IsValid);
            var fundedCount = proposals.Count(x => x.IsEstablished);

            return new ProposalMetadataModel()
            {
                ValidProposalCount = validCount,
                FundedProposalCount = fundedCount
            };
        }
    }
}