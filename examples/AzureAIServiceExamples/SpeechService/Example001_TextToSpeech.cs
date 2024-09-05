using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace AzureAIServiceExamples.SpeechService;

public class Example001_TextToSpeech(ITestOutputHelper output) : BaseTest(output)
{
    [Fact]
    public async Task RunTextToSpeechAsync()
    {
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(TestConfiguration.AzureAISpeech.ApiKey, TestConfiguration.AzureAISpeech.Region);
        //SpeechConfig speechConfig = SpeechConfig.FromEndpoint(new Uri(TestConfiguration.AzureAISpeech.Endpoint), TestConfiguration.AzureAISpeech.ApiKey);

        speechConfig.SpeechSynthesisLanguage = "zh-CN";

        using (SpeechSynthesizer speechSynthesizer = new(speechConfig))
        {
            SpeechSynthesisResult result = await speechSynthesizer.SpeakTextAsync("我通过了你的朋友验证请求，现在我们可以开始聊天了");

            File.WriteAllBytes("audio.wav", result.AudioData);

            switch (result.Reason)
            {
                case ResultReason.Canceled:
                    SpeechSynthesisCancellationDetails cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
                case ResultReason.SynthesizingAudioCompleted:
                    WriteLine($"Speech synthesized");
                    break;
                default:
                    break;
            }
        }
    }

    [Fact]
    public async Task RunSpeechToTextAsync()
    {
        SpeechConfig speechConfig = SpeechConfig.FromSubscription(TestConfiguration.AzureAISpeech.ApiKey, TestConfiguration.AzureAISpeech.Region);
        //SpeechConfig speechConfig = SpeechConfig.FromEndpoint(new Uri(TestConfiguration.AzureAISpeech.Endpoint), TestConfiguration.AzureAISpeech.ApiKey);
        speechConfig.SpeechRecognitionLanguage = "zh-CN";

        //using AudioConfig audioConfig = AudioConfig.FromWavFileInput(Path.Join(AppContext.BaseDirectory, "speech-service", "files", "A-1-audio.wav"));
        using AudioConfig audioConfig = AudioConfig.FromWavFileInput("audio.wav");

        using SpeechRecognizer speechRecognizer = new(speechConfig, audioConfig);

        SpeechRecognitionResult result = await speechRecognizer.RecognizeOnceAsync();

        switch (result.Reason)
        {
            case ResultReason.RecognizedSpeech:
                WriteLine($"RECOGNIZED: Text={result.Text}");
                break;
            case ResultReason.NoMatch:
                WriteLine($"NO MATCH: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:

                CancellationDetails cancellation = CancellationDetails.FromResult(result);

                WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }
}