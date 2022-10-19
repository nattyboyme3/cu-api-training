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
using AzureFunctions.Extensions.Swashbuckle;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(CuApiTraining.SwashbuckleStartup))]
namespace CuApiTraining
{   
    public class SwashbuckleStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts => {
                opts.AddCodeParameter = true;
                opts.Documents = new[] {
                    new SwaggerDocument {
                        Name = "v1",
                            Title = "Swagger document",
                            Description = "Integrate Swagger UI With Azure Functions",
                            Version = "v2"
                    }
                };
                opts.ConfigureSwaggerGen = x => {
                    x.CustomOperationIds(apiDesc => {
                        return apiDesc.TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default(Guid).ToString();
                    });
                };
            });
        }
    }
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
                IEnumerable<dynamic> widgets,
            ILogger log)
        {
            log.LogInformation($"Fetched {widgets.Count()} records from db: ");

            foreach (var w in widgets)
            {
                string sn = w.sn ?? "None";
                log.LogInformation("  SN:" + sn);
            }
            return new OkObjectResult(widgets);
        }
        [FunctionName("New")]
        public static IActionResult NewWidget(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post",
                Route = "widgets/{sn}")]HttpRequest req,
            [CosmosDB(
                databaseName: "cuapitraining",
                collectionName: "wigets",
                ConnectionStringSetting = "ConnectionStrings")]out dynamic document,
            ILogger log, 
            string sn)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject(requestBody);
            document.sn = sn;
            document.id = Guid.NewGuid();
            if (String.IsNullOrEmpty((string)document.name)) document.name = sn;
            string message = $"Wrote new wiget with sn {sn}";
            dynamic responseMessage = new { Status = "OK", Message = message };
            log.LogInformation(message);
            return new OkObjectResult(responseMessage);
        }
        [FunctionName("AuthAll")]
        public static IActionResult AuthAllWidgets(
            [HttpTrigger(AuthorizationLevel.Function, "get",
                Route = "auth/widgets/all")]HttpRequest req,
            [CosmosDB(
                databaseName: "cuapitraining",
                collectionName: "wigets",
                ConnectionStringSetting = "ConnectionStrings",
                SqlQuery = "select * from wigets")]
                IEnumerable<dynamic> widgets,
            ILogger log)
        {
            log.LogInformation($"Fetched {widgets.Count()} records from db: ");

            foreach (var w in widgets)
            {
                string sn = w.sn ?? "None";
                log.LogInformation("  SN:" + sn);
            }
            return new OkObjectResult(widgets);
        }
        [FunctionName("AuthNew")]
        public static IActionResult AuthNewWidget(
            [HttpTrigger(AuthorizationLevel.Function, "post",
                Route = "auth/widgets/{sn}")]HttpRequest req,
            [CosmosDB(
                databaseName: "cuapitraining",
                collectionName: "wigets",
                ConnectionStringSetting = "ConnectionStrings")]out dynamic document,
            ILogger log,
            string sn)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject(requestBody);
            document.sn = sn;
            document.id = Guid.NewGuid();
            if (String.IsNullOrEmpty((string)document.name)) document.name = sn;
            string message = $"Wrote new wiget with sn {sn}";
            dynamic responseMessage = new { Status = "OK", Message = message };
            log.LogInformation(message);
            return new OkObjectResult(responseMessage);
        }
    }
    public static class SwaggerFunctions
    {
        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public static Task<HttpResponseMessage> Swagger(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/json")] HttpRequestMessage req,
                [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return Task.FromResult(swasBuckleClient.CreateSwaggerJsonDocumentResponse(req));
        }
        [SwaggerIgnore]
        [FunctionName("SwaggerUI")]
        public static Task<HttpResponseMessage> SwaggerUI(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui")] HttpRequestMessage req,
        [SwashBuckleClient] ISwashBuckleClient swasBuckleClient)
        {
            return Task.FromResult(swasBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
        }
    }
}



