﻿namespace AzureAIServiceExamples.ComputerVision;

public class Example003_Tags(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunLocalImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        using Stream? stream = EmbeddedResource.ReadStream<Example003_Tags>("image-analysis-sample.jpg");

        ImageAnalysisResult result = await client.AnalyzeAsync(BinaryData.FromStream(stream!), VisualFeatures.Tags);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Tags:");

        foreach (DetectedTag tag in result.Tags.Values)
        {
            WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
        }
    }

    [Fact]
    public async Task RunWebImageAsync()
    {
        ImageAnalysisClient client = new(new Uri(TestConfiguration.AzureAIComputerVision.Endpoint), new AzureKeyCredential(TestConfiguration.AzureAIComputerVision.ApiKey));

        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri("https://img2.baidu.com/it/u=2541539141,19662045&fm=253&fmt=auto&app=138&f=JPEG?w=800&h=500"), VisualFeatures.Tags);

        WriteLine($"Image analysis results:");
        WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
        WriteLine($" Tags:");

        foreach (DetectedTag tag in result.Tags.Values)
        {
            WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
        }
    }
}
