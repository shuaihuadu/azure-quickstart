namespace AzureAIServiceExamples.ComputerVision;

public class Example008_BackgroundRemoval(ITestOutputHelper output) : BaseTest(output)
{
    [Theory]
    [InlineData("backgroundRemoval")]
    [InlineData("foregroundMatting")]
    public async Task RunAsync(string mode)
    {
        string backgroundRemovalApiUrl = $"{TestConfiguration.AzureAIComputerVision.Endpoint}computervision/imageanalysis:segment?api-version=2023-02-01-preview&mode={mode}";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", TestConfiguration.AzureAIComputerVision.ApiKey);

        //string body = @"{""url"":""https://img2.baidu.com/it/u=2541539141,19662045&fm=253&fmt=auto&app=138&f=JPEG?w=800&h=500""}";
        string body = @"{""url"":""https://learn.microsoft.com/zh-cn/azure/ai-services/computer-vision/media/background-removal/building-1.png""}";

        byte[] byteData = Encoding.UTF8.GetBytes(body);

        HttpResponseMessage response;

        using var content = new ByteArrayContent(byteData);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        response = await client.PostAsync(backgroundRemovalApiUrl, content);

        response.EnsureSuccessStatusCode();

        byte[] imageContent = await response.Content.ReadAsByteArrayAsync();

        await File.WriteAllBytesAsync($"{mode}.png", imageContent);
    }
}
