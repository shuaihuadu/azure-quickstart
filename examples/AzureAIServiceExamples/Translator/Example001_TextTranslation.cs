using Azure.AI.Translation.Document;
using Azure.AI.Translation.Text;

namespace AzureAIServiceExamples.Translator;

/// <summary>
/// 使用Azure AI 翻译文档
/// https://learn.microsoft.com/zh-cn/azure/ai-services/translator/
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
    public async Task RunTextTranslationAsync()
    {
        AzureKeyCredential credential = new(TestConfiguration.AzureAITranslator.ApiKey);

        TextTranslationClient client = new(credential, "eastus");

        const string sourceLanguage = "zh";
        const string targetLanguage = "en";

        string inputText = "你吃饭了么？";

        Response<IReadOnlyList<TranslatedTextItem>> response = await client.TranslateAsync(targetLanguage, inputText, sourceLanguage);

        IReadOnlyList<TranslatedTextItem> translatedTextItems = response.Value;

        Assert.NotNull(translatedTextItems);

        TranslatedTextItem translatedTextItem = translatedTextItems.FirstOrDefault()!;

        this.WriteLine(translatedTextItem.Translations.FirstOrDefault()!.Text);
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
