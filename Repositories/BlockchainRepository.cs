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

        public DateTime? GetTime(string hash)
        {
            var resultKey = this.ExecuteRPCCommand("getrawtransaction", new object[] { hash, 1 });
            if (resultKey == null)
                return null;

            var timeKey = resultKey.SelectToken("time");
            if (timeKey == null)
                return null;

            var result = this.ToDateTime(timeKey);
            return result;
        }

        public int GetCurrentBlockCount()
        {
            var result = this.GetCurrentBlock();
            var currentBlockCount = result.First.Value<int>();
            return currentBlockCount;
        }

        private DateTime? ToDateTime(JToken timeKey)
        {
            if (timeKey == null)
                return null;

            var time = JsonConvert.DeserializeObject(timeKey.ToString());
            var result = Convert.ToDouble(time);

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(result).ToLocalTime();
            return dateTime;
        }

        public IEnumerable<BlockchainProposalJson> GetProposals()
        {
            var resultKey = this.ExecuteRPCCommand("mnbudget", new[] { "show" });
            var result = JsonConvert.DeserializeObject<IEnumerable<BlockchainProposalJson>>(resultKey?.ToString());
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
            HttpWebRequest request = this.CreatePostRequest(command);
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
                return null;
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

        public JToken GetCurrentBlock()
        {
            var command = "getblockcount";
            HttpWebRequest request = this.CreatePostRequest(command);
            JObject jObject = this.CreateRequestJson(command, null);

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
                return -1;
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
                    var resultKey = responseJObject.First;
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

        private HttpWebRequest CreatePostRequest(string command)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:51473");
            request.Method = "POST";
            request.ContentType = "application/json-rpc";
            request.ContentLength = command.Length;
            var auth = "user:pass";

            // encode auth for header
            auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth), Base64FormattingOptions.None);
            request.Headers.Add("Authorization", "Basic " + auth);
            return request;
        }

        private JObject CreateRequestJson(string command, object[] parameters)
        {
            JObject jObject = new JObject();
            jObject["jsonrpc"] = "1.0";
            jObject["id"] = "1";
            jObject["method"] = command;

            if (parameters == null)
                return jObject;

            if (parameters.Length <= 0)
                return jObject;

            JArray props = new JArray();
            foreach (var p in parameters)
            {
                props.Add(p);
            }
            jObject.Add(new JProperty("params", props));
            return jObject;
        }
    }
}