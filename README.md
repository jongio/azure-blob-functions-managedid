# How to Upload Blobs to Azure Storage from an Azure Function with Azure Managed Identities

In this 3 part series we are going to learn a few methods for developing an Azure Function that uploads blobs to Azure Storage using the new [Azure Blob Storage SDK and the new Azure Identity SDK's DefaultAzureCredential](https://aka.ms/azsdkpackages). We'll first get everything running locally without any cloud dependencies via a Storage Emulator. We'll then run the same code locally, but configure it to upload blobs to Azure Storage using a Service Principal. And lastly, we'll learn how to deploy an Azure Function to Azure, that uses Azure Storage and [Managed Identities](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/overview). The three different setups allow you to iteratively develop Azure Functions without the cost and inner-loop dev cylce overhead of deploying everything to Azure on every code change.

See this blog series for a complete walkthrough: https://blog.jongallant.com/2020/02/azure-functions-blob-managed-identity-part1


