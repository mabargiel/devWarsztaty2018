
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OrderProcessing
{
    public static class HttpGetSharedAccessSignatureForBlob
    {
        [FunctionName("HttpGetSharedAccessSignatureForBlob")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest req, 
            [Blob("doneorders", FileAccess.Read, Connection = "StorageConnection")] CloudBlobContainer photosContainer, TraceWriter log)
        {
            var fileName = req.Query["fileName"];
            if (string.IsNullOrEmpty(fileName))
                return new BadRequestResult();

            var photoBlob = await photosContainer.GetBlobReferenceFromServerAsync(fileName);
            var photoUri = GetBlobAsUri(photoBlob);

            return new JsonResult(new {PhotoUri = photoUri});
        }

        private static string GetBlobAsUri(ICloudBlob photoBlob)
        {
            var sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddHours(-1);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            sasConstraints.Permissions = SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read;

            var sasToken = photoBlob.GetSharedAccessSignature(sasConstraints);
            return photoBlob.Uri + sasToken;
        }
    }
}
