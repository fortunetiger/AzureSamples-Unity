using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Contains functions for downloading files from Azure Blob Storage.
/// </summary>
public static class BlobStorageUtilities 
{
    /// <summary>
    /// Downloads all blobs in an Azure Blob Storage container.
    /// </summary>
    /// <param name="containerName">Name of the container.</param>
    /// <param name="destinationPath">Local location to download to.</param>
    public static async Task DownloadAllBlobsInContainerAsync(string containerName, string destinationPath)
    {
        CloudBlobContainer container = AzureStorageClient.BlobClient.GetContainerReference(containerName);
        var listOfBlobs = container.ListBlobs();

        foreach (var item in listOfBlobs)
        {
            var blockBlob = new CloudBlockBlob(item.Uri, AzureStorageClient.BlobClient.Credentials);
            var path = Path.Combine(destinationPath, blockBlob.Name);

            await blockBlob.DownloadToFileAsync(path, FileMode.Create);
        }
    }

    /// <summary>
    /// Downloads a single blob from Azure Blob Storage.
    /// </summary>
    /// <param name="fileName">Name of the blob to download.</param>
    /// <param name="containerName">Name of the container that contains the blob.</param>
    /// <param name="destinationPath">Local location to download to.</param>
    public static async Task DownloadBlobFromAzureStorageAsync(string fileName, string containerName, string destinationPath)
    {
        var path = Path.Combine(destinationPath, fileName);

        CloudBlobContainer container = AzureStorageClient.BlobClient.GetContainerReference(containerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

        await blockBlob.DownloadToFileAsync(path, FileMode.Create);
    }
}
