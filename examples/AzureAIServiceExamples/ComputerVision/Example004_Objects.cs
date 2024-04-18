namespace AzureAIServiceExamples.ComputerVision;

public class Example004_Objects(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream<Example004_Objects>("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.Objects);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Objects:");

        foreach (DetectedObject detectedObject in result.Objects.Values)
        {
            WriteLine($"   Object: '{detectedObject.Tags.First().Name}', Bounding box {detectedObject.BoundingBox.ToString()}");
        }
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://img2.baidu.com/it/u=2541539141,19662045&fm=253&fmt=auto&app=138&f=JPEG?w=800&h=500"), VisualFeatures.Objects);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Objects:");

        foreach (DetectedObject detectedObject in result.Objects.Values)
        {
            WriteLine($"   Object: '{detectedObject.Tags.First().Name}', Bounding box {detectedObject.BoundingBox.ToString()}");
        }
    }
}
