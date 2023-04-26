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
using FunctionApp1.Model;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Collections;
using System.Reflection.Metadata;

namespace FunctionApp1
{
    public  class Function1
    {
        private readonly CosmosClient client;

        private Container documentcontainer;
        private const string PARTITION_KEY = "/id";


        public Function1(CosmosClient cosmosClient)
        {
            client = cosmosClient;
            documentcontainer = client.GetContainer("Employee", "Items");
        }
    
         
        [FunctionName("GetEmployee")]

        public  async Task<IActionResult> GetEmployee(

            [HttpTrigger(AuthorizationLevel.Function, "get",Route = "GetEmployee")] HttpRequest req, [CosmosDB(databaseName:"Employee",containerName:"Items",
                Connection ="CosmoseDbConnection",
                SqlQuery = "SELECT * FROM c")] Document client,
            ILogger log) 
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<Employee> employees = new();

            var items = documentcontainer.GetItemQueryIterator<Employee>();
           
            while(items.HasMoreResults)
            {
                var response = await items.ReadNextAsync();
                employees.AddRange(response.ToList());
            }
            return new OkObjectResult(employees);
        }
    }
}
