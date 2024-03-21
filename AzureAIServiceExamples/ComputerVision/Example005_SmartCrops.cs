namespace AzureAIServiceExamples.ComputerVision;

public class Example005_SmartCrops(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.SmartCrops, new ImageAnalysisOptions
        {
            SmartCropsAspectRatios = [0.9F, 1.33F]
        });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" SmartCrops:");

        foreach (CropRegion cropRegion in result.SmartCrops.Values)
        {
            WriteLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
        }
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        //https://img2.baidu.com/it/u=2541539141,19662045&fm=253&fmt=auto&app=138&f=JPEG?w=800&h=500
        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://aka.ms/azsdk/image-analysis/sample.jpg"), VisualFeatures.SmartCrops, new ImageAnalysisOptions
        {
            SmartCropsAspectRatios = [0.9F, 1.33F]
        });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" SmartCrops:");

        foreach (CropRegion cropRegion in result.SmartCrops.Values)
        {
            WriteLine($"   Aspect ratio: {cropRegion.AspectRatio}, Bounding box: {cropRegion.BoundingBox}");
        }
    }
}
