using System;
using Microsoft.Extensions.Logging;

namespace ZestMonitor.Api.Utillities
{
    public class ConvertHelpers
    {
        private ILogger _logger { get; }

        public ConvertHelpers(ILogger<ConvertHelpers> logger){
            this._logger = logger;
        }
        
        public string ByteArrayToString(byte[] bytes){
            try 
            {
                var result = System.Text.Encoding.UTF8.GetString(bytes);
                return result;
            } 
            catch (ArgumentNullException ex) 
            {
                this._logger.LogCritical(ex.Message, ex);
                
                // Keeps stack trace (throw ex will lose the stack trace)
                throw;
            } 
            catch (ArgumentException ex) 
            {
                this._logger.LogCritical($"The byte array contains invalid Unicode code points. at ${ex.Source}");
                throw;
            } 
            catch (Exception ex) 
            {
                this._logger.LogCritical(ex.Message, ex);
                throw;
            }
        }
    }
}