namespace Azure.QuickStart.Shared;

public sealed class TestConfiguration
{
    private readonly IConfigurationRoot _configurationRoot;

    private static TestConfiguration? _instance;

    private TestConfiguration(IConfigurationRoot configurationRoot)
    {
        this._configurationRoot = configurationRoot;
    }

    public static void Initialize(IConfigurationRoot configurationRoot)
    {
        _instance = new TestConfiguration(configurationRoot);
    }

    public static AzureOpenAIConfig AzureOpenAI => LoadSection<AzureOpenAIConfig>();
    public static AzureAISearchConfig AzureAISearch => LoadSection<AzureAISearchConfig>();
    public static AzureAISpeechConfig AzureAISpeech => LoadSection<AzureAISpeechConfig>();
    public static AzureAIComputerVisionConfig AzureAIComputerVision => LoadSection<AzureAIComputerVisionConfig>();
    public static BingConfig Bing => LoadSection<BingConfig>();
    public static AzureBlobConfig AzureBlob => LoadSection<AzureBlobConfig>();

    private static T LoadSection<T>([CallerMemberName] string? caller = null)
    {
        if (_instance == null)
        {
            throw new InvalidOperationException("TestConfiguration must be initialized with a call to Initialize(IConfigurationRoot) before accessing configuration values.");
        }

        if (string.IsNullOrEmpty(caller))
        {
            throw new ArgumentNullException(nameof(caller));
        }

        return _instance._configurationRoot.GetSection(caller).Get<T>() ?? throw new ConfigurationNotFoundException(section: caller);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

    public class AzureOpenAIConfig
    {
        public string DeploymentName { get; set; }
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }

    public class AzureAISearchConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string IndexName { get; set; }
    }


    public class AzureAISpeechConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { set; get; }
        public string Region { get; set; }
    }

    public class AzureAIComputerVisionConfig
    {
        public string Endpoint { get; set; }
        public string ApiKey { set; get; }
        public string Region { get; set; }
    }

    public class AzureBlobConfig
    {
        public string ConnectionString { get; set; }
    }

    public class BingConfig
    {
        public string ApiKey { get; set; } = string.Empty;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
}
