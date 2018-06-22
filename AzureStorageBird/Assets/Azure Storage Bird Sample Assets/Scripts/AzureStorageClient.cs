using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


/// <summary>
/// Class with connection info and a singleton for the CloudBlobClient,
/// which is necessary for most Azure Storage functions.
/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.windowsazure.storage.blob.cloudblobclient?view=azure-dotnet
/// </summary>
public static class AzureStorageClient
{
    private const string connectionString = "YOUR_CONNECTION_STRING_HERE";
    private static CloudStorageAccount storageAccount;
    private static CloudBlobClient blobClient;

    public static CloudBlobClient BlobClient
    {
        get
        {
            if (blobClient == null)
            {
                storageAccount = CloudStorageAccount.Parse(connectionString);
                blobClient = storageAccount.CreateCloudBlobClient();
            }

            return blobClient;
        }
    }
}
