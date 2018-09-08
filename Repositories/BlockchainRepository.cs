using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly string GetProposalsCommand = "{ \"jsonrpc\": \"1.0\", \"id\":\"getproposals\", \"method\": \"mnbudget\",\"params\":[\"show\"]}";

        public async Task<List<BlockchainProposalJson>> GetPagedProposals(PagingParams pagingParams)
        {
            HttpWebRequest request = CreateRequest(this.GetProposalsCommand);
            var requestStream = request.GetRequestStream();
            using (StreamWriter streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(this.GetProposalsCommand);
            }

            try
            {
                // Make request and read response stream
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(responseData))
                        throw new ArgumentNullException($"{nameof(responseData)} is empty.");

                    var jObject = JObject.Parse(responseData);

                    var resultKey = jObject.SelectToken("result");
                    var result = JsonConvert.DeserializeObject<List<BlockchainProposalJson>>(resultKey?.ToString());
                    return result;

                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        return null;
                    }
                    return null;
                }
            }
        }


        public async Task<List<BlockchainProposalJson>> GetProposals()
        {
            HttpWebRequest request = CreateRequest(this.GetProposalsCommand);
            var requestStream = request.GetRequestStream();
            using (StreamWriter streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(this.GetProposalsCommand);
            }

            try
            {
                // Make request and read response stream
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(responseData))
                        throw new ArgumentNullException($"{nameof(responseData)} is empty.");

                    var jObject = JObject.Parse(responseData);
                    // We need to extract the result object
                    var resultKey = jObject.SelectToken("result");
                    var result = JsonConvert.DeserializeObject<List<BlockchainProposalJson>>(resultKey?.ToString());

                    return result;
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        return null;
                    }
                    return null;
                }
            }
        }

        public async Task<BlockchainProposalJson> GetProposal(string name)
        {
            var command = $"{{ \"jsonrpc\": \"1.0\", \"id\":\"getbudgetinfo\", \"method\": \"getbudgetinfo\",\"params\": [\"{name}\"]}}";

            HttpWebRequest request = CreateRequest(command);
            var requestStream = request.GetRequestStream();
            using (StreamWriter streamWriter = new StreamWriter(requestStream))
            {
                streamWriter.Write(command);
            }

            try
            {
                // Make request and read response stream
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = await streamReader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(responseData))
                        throw new ArgumentNullException($"{nameof(responseData)} is empty.");

                    var jObject = JObject.Parse(responseData);
                    // We need to extract the result object
                    var proposalKey = jObject.SelectToken("result").First();
                    var result = JsonConvert.DeserializeObject<BlockchainProposalJson>(proposalKey?.ToString());

                    return result;
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode != HttpStatusCode.InternalServerError)
                    {
                        return null;
                    }
                    return null;
                }
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

        public async Task<int> GetValidCount()
        {
            var proposals = await this.GetProposals();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsValid);
            return result;
        }

        public async Task<int> GetFundedCount()
        {
            var proposals = await this.GetProposals();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsEstablished);

            return result;
        }

        public async Task<ProposalMetadataModel> GetMetadata()
        {
            var proposals = await this.GetProposals();
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