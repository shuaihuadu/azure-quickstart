using Microsoft.CognitiveServices.Speech.Audio;

namespace AzureAIServiceExamples.SpeechService;

public class Example002_SpeechToText
{
}


public class ContosoAudioStream : PullAudioInputStreamCallback
{
    public override int Read(byte[] dataBuffer, uint size)
    {
        return 0;
    }
}

//public class ContosoAudioStream : PullAudioInputStreamCallback
//{
//    public ContosoAudioStream() { }

//    public override int Read(byte[] buffer, uint size)
//    {
//        // Returns audio data to the caller.
//        // E.g., return read(config.YYY, buffer, size);
//        return 0;
//    }

//    public override void Close()
//    {
//        // Close and clean up resources.
//    }
//}

//class Program
//{
//    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
//    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

//    async static Task Main(string[] args)
//    {
//        byte channels = 1;
//        byte bitsPerSample = 16;
//        uint samplesPerSecond = 16000; // or 8000
//        var audioFormat = AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, bitsPerSample, channels);
//        var audioConfig = AudioConfig.FromStreamInput(new ContosoAudioStream(), audioFormat);

//        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
//        speechConfig.SpeechRecognitionLanguage = "en-US";
//        var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

//        Console.WriteLine("Speak into your microphone.");
//        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
//        Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
//    }
//}