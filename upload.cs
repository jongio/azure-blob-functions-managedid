using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace azure_blob_functions_managedid
{
    public static class upload
    {
        [FunctionName("upload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var host = Environment.GetEnvironmentVariable("AZURE_STORAGE_HOST");
            var account = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT");
            var container = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONTAINER");
            var emulator = account == "devstoreaccount1";
            var uri = $"https://{(emulator ? $"{host}/{account}" : $"{account}.{host}")}/{container}";

            // Generate random string for blob content and file name
            var content = Guid.NewGuid().ToString("n").Substring(0, 8);
            var file = $"{content}.txt";

            // For Azurite 3.7+ with HTTPS and OAuth enabled, you can run Azurite with the following
            // azurite --oauth basic --cert cert-name.pem --key cert-name-key.pem
            var client = new BlobContainerClient(new Uri(uri), new DefaultAzureCredential());

            // Create container
            await client.CreateIfNotExistsAsync();

            // Get content stream
            using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));

            // Upload blob
            await client.UploadBlobAsync(file, stream);

            return (ActionResult)new OkObjectResult($"{file} uploaded.");
        }
    }
}
