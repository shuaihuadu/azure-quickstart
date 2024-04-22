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

            Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync();

            using MemoryStream memoryStream = new();

            response.Value.Content.CopyTo(memoryStream);

            byte[] byteArray = memoryStream.ToArray();

            await File.WriteAllBytesAsync("2.jpg", byteArray);
        }
        catch (RequestFailedException ex)
        {
            WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
            WriteLine(ex.Message);
        }
    }

    [Fact]
    public async Task DeleteBlobAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            string fileName = "1.jpg";

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = container.GetBlobClient(fileName);

            await blobClient.DeleteAsync();
        }
        catch (RequestFailedException ex)
        {
            if ((ex.Status != (int)HttpStatusCode.NotFound))
            {
                WriteLine($"HTTP error code {ex.Status}:{ex.ErrorCode}");
                WriteLine(ex.Message);
            }
        }
    }

    [Fact]
    public void GetBlobUriWithSasAsyncTest()
    {
        BlobServiceClient blobServiceClient = new(TestConfiguration.AzureBlob.ConnectionString);

        try
        {
            string fileName = "1.jpg";

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = container.GetBlobClient(fileName);

            Uri? uriWithSas = this.CreateBlobSasAsync(blobClient);

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

    private Uri? CreateBlobSasAsync(BlobClient blobClient)
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