
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace OrderProcessing
{
    public static class RetrieveOrder
    {
        [FunctionName("RetrieveOrder")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, 
            [Table("Orders", Connection = "StorageConnection")] CloudTable ordersTable, TraceWriter log)
        {
            string fileName = req.Query["fileName"];

            if(string.IsNullOrEmpty(fileName))
                return new BadRequestResult();

            var query = new TableQuery<Order>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, fileName));
            var tableQueryResult = await ordersTable.ExecuteQuerySegmentedAsync(query, null);
            var resultList = tableQueryResult.Results;

            if (resultList.Any())
            {
                var firstElement = resultList.First();
                return new JsonResult(new
                {
                    firstElement.CustomerEmail,
                    firstElement.FileName,
                    firstElement.RequiredHeight,
                    firstElement.RequiredWidth
                });
            }

            return new NotFoundResult();
        }
    }
}
