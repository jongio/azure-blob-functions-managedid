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
            var connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var host = Environment.GetEnvironmentVariable("AZURE_STORAGE_HOST");
            var account = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT");
            var container = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONTAINER");
            var emulator = connection.Contains("UseDevelopmentStorage=true") && account == "devstoreaccount1";
            var path = emulator ? $"https://{host}/{account}/{container}" : $"https://{account}.{host}/{container}";

            var content = Guid.NewGuid().ToString("n").Substring(0, 8);
            var file = $"{content}.txt";

            // DefaultAzureCredential does not currently support HTTP endpoints, so we have to do this hack.  Hopefully it will soon.
            // Use BlobContainerClient(connectionString, container) ctor for emulator and BlobContainerClient(uri, TokenCredential) for Azure
            var client = emulator ?
                new BlobContainerClient(connection, container) :
                new BlobContainerClient(new Uri(path), new DefaultAzureCredential());


            await client.CreateIfNotExistsAsync();

            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
            {
                await client.UploadBlobAsync(file, stream);
            }

            return (ActionResult)new OkObjectResult($"{file} uploaded.");
        }
    }
}
