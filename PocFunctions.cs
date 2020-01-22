using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;

namespace DurableFunctionPoc
{
    public static class PocFunctions
    {

        [FunctionName("BlobTrigger")]
        public static async Task Run([BlobTrigger("source/{name}", Connection = "")]Stream myBlob, string name, [OrchestrationClient]DurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            await starter.StartNewAsync("RunOrchestrator", name);
        }

        [FunctionName("HttpTrigger")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HttpTrigger/{name}")]HttpRequestMessage req,
            string name,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. blob name: {name}");

            string instanceId = await starter.StartNewAsync("RunOrchestrator", name);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("RunOrchestrator")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log)
        {
            string name = context.GetInput<string>();

            log.LogInformation($"start to process file {name}");

            #region Process file
            int i = 20;
            while (i-- > 0)
            {
                Thread.Sleep(5 * 1000);
                log.LogInformation($"sleep 5s");
            }
            #endregion

            log.LogInformation($"process file {name} complete");

            return "Succeed";
        }
    }
}
