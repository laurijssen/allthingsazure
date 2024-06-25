using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ch2;

class Program
{
    static void Main(string[] args)
    {
        Task.Run(async () => await StartContainersDemo()).Wait();
    }

    public static async Task StartContainersDemo()
    {
        string BlobFileName = "mykey.pub";
        AppSettings? appSettings = AppSettings.LoadAppSettings();

        Console.WriteLine(appSettings.SASConnectionString);

        BlobServiceClient blobClient = Common.CreateBlobClientStorageFromSAS(appSettings.SASConnectionString);

        var containerReference = blobClient.GetBlobContainerClient(appSettings.ContainerName);

        var blobReference = containerReference.GetBlobClient(BlobFileName);

        BlobProperties blobProperties = await blobReference.GetPropertiesAsync();

        Console.WriteLine($"Access tier: {blobProperties.AccessTier}\t" +
                          $"Inferred: {blobProperties.AccessTierInferred}\t" +
                          $"Date last acces tier change: {blobProperties.AccessTierChangedOn}\t" +
                          $"Archive status: {blobProperties.ArchiveStatus}");

        blobReference.SetAccessTier(AccessTier.Cool);

        blobProperties = await blobReference.GetPropertiesAsync();

        Console.WriteLine($"Access tier: {blobProperties.AccessTier}\t" +
                          $"Inferred: {blobProperties.AccessTierInferred}\t" +
                          $"Date last acces tier change: {blobProperties.AccessTierChangedOn}\t" +
                          $"Archive status: {blobProperties.ArchiveStatus}");

        blobReference.SetAccessTier(AccessTier.Hot);

        blobProperties = await blobReference.GetPropertiesAsync();
        
        Console.WriteLine($"Access tier: {blobProperties.AccessTier}\t" +
                          $"Inferred: {blobProperties.AccessTierInferred}\t" +
                          $"Date last acces tier change: {blobProperties.AccessTierChangedOn}\t" +
                          $"Archive status: {blobProperties.ArchiveStatus}");
    }
}