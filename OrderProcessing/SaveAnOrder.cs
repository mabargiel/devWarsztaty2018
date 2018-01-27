
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace OrderProcessing
{
    public static class SaveAnOrder
    {
        [FunctionName("SaveAnOrder")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            Order order = null;
            try
            {
                var requestBody = new StreamReader(req.Body).ReadToEnd();
                order = JsonConvert.DeserializeObject<Order>(requestBody);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Received data invalid");
            }

            return new OkObjectResult("Success");
        }
    }

    public class Order
    {
        public string Email { get; set; }
        public string FileName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
