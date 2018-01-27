
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace OrderProcessing
{
    public static class SaveAnOrder
    {
        [FunctionName("SaveAnOrder")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req,
            [Table("Orders", Connection = "StorageConnection")] ICollector<Order> ordersTable, TraceWriter log)
        {
            try
            {
                var requestBody = new StreamReader(req.Body).ReadToEnd();
                var order = JsonConvert.DeserializeObject<Order>(requestBody);
                order.PartitionKey = DateTime.UtcNow.DayOfYear.ToString();
                order.RowKey = order.FileName;
                ordersTable.Add(order);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Received data invalid");
            }

            return new OkObjectResult("Success");
        }
    }

    public class Order : TableEntity
    {
        public string CustomerEmail { get; set; }
        public string FileName { get; set; }
        public string RequiredHeight { get; set; }
        public string RequiredWidth { get; set; }
    }
}
