using Azure.Storage.Blobs;

namespace ch2;

public class Common
{
    public static BlobServiceClient CreateBlobClientStorageFromSAS(string SASConnectionString)
    {
        try
        {
            return new BlobServiceClient(SASConnectionString);
        }
        catch (Exception)
        {
            throw;
        }
    }
}