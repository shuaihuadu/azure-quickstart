using Azure.AI.Translation.Document;

namespace AzureAIServiceExamples.Translator;

/// <summary>
/// 使用Azure AI 翻译文档
/// https://learn.microsoft.com/zh-cn/azure/ai-services/translator/document-translation/reference/rest-api-guide
/// 注意事项：
/// 1.创建Azure AI Translator，免费版的不支持文档翻译
/// 2.需要创建翻译文档的blob container，并生成SAS，设置对应的SAS权限
/// 3.需要创建翻译完成后的文档的blob container，并生成SAS，设置对应的SAS权限
/// 4.使用Azure.AI.Translation.Document SDK翻译文档
/// </summary>
/// <param name="output"></param>
public class Example001_TextTranslation(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunSingleDocumentTranslationAsync()
    {
        SingleDocumentTranslationClient client = new(new Uri(TestConfiguration.AzureAITranslator.DocumentTranslatorEndpoint), new AzureKeyCredential(TestConfiguration.AzureAITranslator.ApiKey));

        const string fileName = "paper.pdf";

        string document = Path.Join(AppContext.BaseDirectory, "Documents", fileName);

        await using Stream stream = File.OpenRead(document);

        MultipartFormFileData sourceDocument = new(Path.GetFileName(fileName), stream, "application/octet-stream");

        DocumentTranslateContent content = new(sourceDocument);

        Response<BinaryData> response = await client.DocumentTranslateAsync("zh", content);

        Assert.NotNull(response.Value);

        byte[] byteArray = response.Value.ToArray();

        const string outputFile = "paper-译文.pdf";

        await File.WriteAllBytesAsync(outputFile, byteArray);
    }

    [Fact]
    public async Task RunBatchDocumentTranslationAsync()
    {
        Uri sourceUri = new Uri(TestConfiguration.AzureAITranslator.SourceContainerUrl);
        Uri targetUri = new Uri(TestConfiguration.AzureAITranslator.TargetContainerUrl);

        const string targetLanguage = "en";

        DocumentTranslationClient client = new(new Uri(TestConfiguration.AzureAITranslator.DocumentTranslatorEndpoint), new AzureKeyCredential(TestConfiguration.AzureAITranslator.ApiKey));

        DocumentTranslationInput input = new(sourceUri, targetUri, targetLanguage);

        DocumentTranslationOperation operation = await client.StartTranslationAsync(input);

        await operation.WaitForCompletionAsync();

        this.WriteLine($"  Status: {operation.Status}");
        this.WriteLine($"  Created on: {operation.CreatedOn}");
        this.WriteLine($"  Last modified: {operation.LastModified}");
        this.WriteLine($"  Total documents: {operation.DocumentsTotal}");
        this.WriteLine($"    Succeeded: {operation.DocumentsSucceeded}");
        this.WriteLine($"    Failed: {operation.DocumentsFailed}");
        this.WriteLine($"    In Progress: {operation.DocumentsInProgress}");
        this.WriteLine($"    Not started: {operation.DocumentsNotStarted}");

        await foreach (DocumentStatusResult document in operation.Value)
        {
            this.WriteLine($"Document with Id: {document.Id}");
            this.WriteLine($"  Status:{document.Status}");

            if (document.Status == DocumentTranslationStatus.Succeeded)
            {
                this.WriteLine($"  Translated Document Uri: {document.TranslatedDocumentUri}");
                this.WriteLine($"  Translated to language: {document.TranslatedToLanguageCode}.");
                this.WriteLine($"  Document source Uri: {document.SourceDocumentUri}");
            }
            else
            {
                this.WriteLine($"  Error Code: {document.Error.Code}");
                this.WriteLine($"  Message: {document.Error.Message}");
            }
        }

        Assert.NotNull(client);
    }
}
