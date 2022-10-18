using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace CuApiTraining
{
    public static class Functions
    {
        [FunctionName("Test")]
        public static async Task<IActionResult> Test(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Testing request called.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("All")]
        public static IActionResult AllWidgets(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "widgets/all")]HttpRequest req,
            [CosmosDB(
                databaseName: "cuapitraining", 
                collectionName: "wigets",
                ConnectionStringSetting = "ConnectionStrings",
                SqlQuery = "select * from wigets")]
                IEnumerable<string> widgets,
            ILogger log)
        {
            log.LogInformation($"Fetched {widgets.Count()} records from db: ");

            foreach (var w in widgets)
            {
                log.LogInformation("  " + w);
            }
            return new OkObjectResult(widgets);
        }
        [FunctionName("New")]
        public static IActionResult NewWidget(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post",
                Route = "widgets/{sn}")]HttpRequest req,
            [CosmosDB(
                databaseName: "ToDoItems",
                collectionName: "Items",
                ConnectionStringSetting = "ConnectionStrings")]out dynamic document,
            ILogger log, 
            string sn)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject(requestBody);
            document.sn = sn;
            document.id = Guid.NewGuid();
            if (String.IsNullOrEmpty(document.name)) document.name = sn;
            string message = $"Wrote new wiget with sn {sn}";
            dynamic responseMessage = new { Status = "OK", Message = message };
            log.LogInformation(message);
            return new OkObjectResult(responseMessage);
        }
    }
}


