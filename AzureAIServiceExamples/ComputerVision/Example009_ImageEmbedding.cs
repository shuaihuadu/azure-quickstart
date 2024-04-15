namespace AzureAIServiceExamples.ComputerVision;

public class Example009_ImageEmbedding(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunImageSearchImageAsync()
    {
        VisionImageEmbeddingRequest imageEmbeddingRequest1 = new()
        {
            Url = "https://learn.microsoft.com/azure/ai-services/computer-vision/media/quickstarts/presentation.png"
        };
        VisionImageEmbeddingRequest imageEmbeddingRequest2 = new()
        {
            Url = "https://tse4-mm.cn.bing.net/th/id/OIP-C.syAUEuilkSmuuSb2ClI90gAAAA?rs=1&pid=ImgDetMain"
        };

        VisionEmbeddingResponse? imageEmbeddingResponse1 = await this.GetVisionEmbeddingResponseAsync(imageEmbeddingRequest1, AzureAIVisioinEmbeddingType.Image);
        VisionEmbeddingResponse? imageEmbeddingResponse2 = await this.GetVisionEmbeddingResponseAsync(imageEmbeddingRequest2, AzureAIVisioinEmbeddingType.Image);


        if (imageEmbeddingResponse1 is not null && imageEmbeddingResponse2 is not null)
        {
            float cosineSimilarity = TensorPrimitives.CosineSimilarity(imageEmbeddingResponse1.Vector.Span, imageEmbeddingResponse2.Vector.Span);

            Assert.NotEqual(0, cosineSimilarity);

            WriteLine(cosineSimilarity);
        }
    }

    [Fact]
    public async Task RunTextSearchImageAsync()
    {
        VisionImageEmbeddingRequest imageEmbeddingRequest = new()
        {
            Url = "https://learn.microsoft.com/azure/ai-services/computer-vision/media/quickstarts/presentation.png"
        };

        VisionEmbeddingResponse? imageEmbeddingResponse = await this.GetVisionEmbeddingResponseAsync(imageEmbeddingRequest, AzureAIVisioinEmbeddingType.Image);


        VisionTextEmbeddingRequest textEmbeddingRequest = new()
        {
            //Text = "站在屏幕前进行演示"
            Text = "presentation"
        };
        VisionEmbeddingResponse? textEmbeddingResponse = await this.GetVisionEmbeddingResponseAsync(textEmbeddingRequest, AzureAIVisioinEmbeddingType.Text);

        if (imageEmbeddingResponse is not null && textEmbeddingResponse is not null)
        {
            float cosineSimilarity = TensorPrimitives.CosineSimilarity(imageEmbeddingResponse.Vector.Span, textEmbeddingResponse.Vector.Span);

            Assert.NotEqual(0, cosineSimilarity);

            WriteLine(cosineSimilarity);
        }
    }

    private async Task<VisionEmbeddingResponse?> GetVisionEmbeddingResponseAsync<TRequest>(TRequest request, AzureAIVisioinEmbeddingType embeddingType) where TRequest : VisionEmbeddingRequest
    {
        string segment = embeddingType == AzureAIVisioinEmbeddingType.Image ? "vectorizeImage" : "vectorizeText";

        string embeddingApiUrl = $"{TestConfiguration.AzureAIComputerVision.Endpoint}/computervision/retrieval:{segment}?api-version=2024-02-01&model-version=2023-04-15";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", TestConfiguration.AzureAIComputerVision.ApiKey);

        string body = JsonSerializer.Serialize(request);

        byte[] byteData = Encoding.UTF8.GetBytes(body);

        HttpResponseMessage response;

        using var content = new ByteArrayContent(byteData);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        response = await client.PostAsync(embeddingApiUrl, content);

        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<VisionEmbeddingResponse>(responseContent);
    }
}

class VisionEmbeddingRequest
{

}

class VisionImageEmbeddingRequest : VisionEmbeddingRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

class VisionTextEmbeddingRequest : VisionEmbeddingRequest
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

class VisionEmbeddingResponse
{
    [JsonPropertyName("modelVersion")]
    public string ModelVersion { get; set; } = string.Empty;
    [JsonPropertyName("vector")]
    public ReadOnlyMemory<float> Vector { get; set; }
}

enum AzureAIVisioinEmbeddingType
{
    Image,
    Text
}