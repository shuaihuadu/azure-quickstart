using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace AzureStorageExamples;

public class Example001_BlobStorageTest(ITestOutputHelper output) : BaseTest(output)
{
    const string ContainerName = $"container-experimental";

    [Fact]
    public async Task CreateContainerAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(ContainerName);

            if (await container.ExistsAsync())
            {
                WriteLine($"Created container {container.Name}");
            }
        }
        catch (RequestFailedException ex)
        {
            WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
            WriteLine(ex.Message);
        }
    }

    [Fact]
    public async Task UploadToBlobAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            string fileName = "1.jpg";

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);

            byte[] bytes = await File.ReadAllBytesAsync(Path.Combine(FileBaseDirectory, fileName));

            Response<BlobContentInfo> response = await container.UploadBlobAsync(fileName, new BinaryData(bytes));

            Response rawResponse = response.GetRawResponse();

            WriteLine(rawResponse.ToString());
        }
        catch (RequestFailedException ex)
        {
            WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
            WriteLine(ex.Message);
        }
    }

    [Fact]
    public async Task DownloadBlobAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            string fileName = "1.jpg";

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = container.GetBlobClient(fileName);

            WriteLine(blobClient.Uri.AbsoluteUri);
        }
        catch (RequestFailedException ex)
        {
            WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
            WriteLine(ex.Message);
        }
    }

    [Fact]
    public async Task GetBlobUriWithSasAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            string fileName = "1.jpg";

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = container.GetBlobClient(fileName);

            Uri? uriWithSas = await this.CreateBlobSasAsync(blobClient);

            if (uriWithSas is not null)
            {
                WriteLine(uriWithSas.AbsoluteUri);
            }
        }
        catch (RequestFailedException ex)
        {
            WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
            WriteLine(ex.Message);
        }
    }

    private async Task<Uri?> CreateBlobSasAsync(BlobClient blobClient)
    {
        if (blobClient.CanGenerateSasUri)
        {
            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(180)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return sasURI;
        }
        else
        {
            return null;
        }
    }
}