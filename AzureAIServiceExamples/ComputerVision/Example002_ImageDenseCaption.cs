namespace AzureAIServiceExamples.ComputerVision;

public class Example002_ImageDenseCaption(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.DenseCaptions, new ImageAnalysisOptions
        {
            GenderNeutralCaption = true
        });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Dense Captions:");

        foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
        {
            WriteLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
        }
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://aka.ms/azsdk/image-analysis/sample.jpg"), VisualFeatures.DenseCaptions, new ImageAnalysisOptions
        {
            GenderNeutralCaption = true
        });

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Dense Caption:");

        foreach (DenseCaption denseCaption in result.DenseCaptions.Values)
        {
            WriteLine($"   Region: '{denseCaption.Text}', Confidence {denseCaption.Confidence:F4}, Bounding box {denseCaption.BoundingBox}");
        }
    }
}
