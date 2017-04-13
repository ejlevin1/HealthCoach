using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCoach.App.Config
{
    public class Azure
    {
        public static async Task RegisterServices(IConfiguration configuration, IServiceCollection services)
        {
            string connectionString = configuration.GetValue<string>("AppSettings:StorageConnectionString");

            //Parse the connection string for the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //Create service client for credentialed access to the Blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("my-append-blobs");

            //Create the container if it does not already exist.
            await container.CreateIfNotExistsAsync();

            //Get a reference to an append blob.
            CloudAppendBlob appendBlob = container.GetAppendBlobReference("append-blob.log");

            //Create the append blob. Note that if the blob already exists, the CreateOrReplace() method will overwrite it.
            //You can check whether the blob exists to avoid overwriting it by using CloudAppendBlob.Exists().
            await appendBlob.CreateOrReplaceAsync();

            //register the singleton
            services.AddSingleton<IAzureFileWriter>(new AzureFileWriter(appendBlob));

            //services.AddTransient<IFileWriter, AzureFileWriter>();
        }
    }

    public interface IAzureFileWriter
    {
        void Write(string content);
    }

    public class AzureFileWriter : IAzureFileWriter
    {
        private CloudAppendBlob _blob = null;

        public AzureFileWriter(CloudAppendBlob blob)
        {
            _blob = blob;
        }

        public void Write(string content)
        {
            try
            {
                _blob.AppendTextAsync(String.Format("Timestamp: {0:u} \tLog Entry: {1}{2}",
                    DateTime.UtcNow, content, Environment.NewLine)).Wait();
            }
            catch
            {

            }
        }
    }
}
