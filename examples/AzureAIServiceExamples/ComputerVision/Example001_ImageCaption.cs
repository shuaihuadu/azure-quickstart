namespace AzureAIServiceExamples.ComputerVision;

public class Example001_ImageCaption(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream<Example001_ImageCaption>("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.Caption, new ImageAnalysisOptions { GenderNeutralCaption = true });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Caption:");
        WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://aka.ms/azsdk/image-analysis/sample.jpg"), VisualFeatures.Caption, new ImageAnalysisOptions
        {
            GenderNeutralCaption = true
        });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Caption:");
        WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");
    }
}
