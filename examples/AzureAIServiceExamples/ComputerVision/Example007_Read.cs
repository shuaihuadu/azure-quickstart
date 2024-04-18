namespace AzureAIServiceExamples.ComputerVision;

public class Example007_Read(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream<Example007_Read>("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.Read);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Text:");
        foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
        {
            WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
            foreach (DetectedTextWord word in line.Words)
            {
                WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
            }
        }
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://img2.baidu.com/it/u=2541539141,19662045&fm=253&fmt=auto&app=138&f=JPEG?w=800&h=500"), VisualFeatures.Read);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");

        WriteLine($" Text:");
        foreach (var line in result.Read.Blocks.SelectMany(block => block.Lines))
        {
            WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
            foreach (DetectedTextWord word in line.Words)
            {
                WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
            }
        }
    }
}
