
using Microsoft.Azure.Storage.Blob;

namespace ch2;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("getting system properties demo");

        AppSettings? appSettings = AppSettings.LoadAppSettings();

        if (appSettings == null)  throw new();

        CloudBlobClient client = Common.CreateBlobClientStorageFromSAS(appSettings.SASToken, appSettings.AccountName);
        CloudBlobContainer container = client.GetContainerReference(appSettings.ContainerName);

        container.CreateIfNotExists();
        container.FetchAttributes();

        Console.WriteLine($"properties for container {container.StorageUri.PrimaryUri}");
        Console.WriteLine($"ETAG: {container.Properties.ETag}");
        Console.WriteLine($"LastModifieUTC: {container.Properties.LastModified}");
        Console.WriteLine($"lease status {container.Properties.LeaseStatus}");

        var list = container.ListBlobs();

        List<string> data = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

        foreach (var d in data)
        {
            Console.WriteLine(d);
        }
    }
}

