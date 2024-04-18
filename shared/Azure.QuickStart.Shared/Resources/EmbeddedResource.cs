namespace Azure.QuickStart.Shared.Resources;

public static class EmbeddedResource
{
    public static string Read<T>(string fileName) where T : class
    {
        string? resourceNamespace = typeof(T).Namespace;

        Assembly? assembly = typeof(T).GetTypeInfo().Assembly ?? throw new ConfigurationException($"[{resourceNamespace}] {fileName} assembly not found");

        string resourceName = $"{resourceNamespace}.{fileName}";

        using Stream? resource = assembly.GetManifestResourceStream(resourceName) ?? throw new ConfigurationException($"{resourceName} resource not found");

        using StreamReader reader = new(resource);

        return reader.ReadToEnd();
    }

    public static Stream? ReadStream<T>(string fileName) where T : class
    {
        string? resourceNamespace = typeof(T).Namespace;

        Assembly? assembly = typeof(T).GetTypeInfo().Assembly ?? throw new ConfigurationException($"[{resourceNamespace}] {fileName} assembly not found");

        string resourceName = $"{resourceNamespace}.{fileName}";

        return assembly.GetManifestResourceStream(resourceName);
    }

    public async static Task<ReadOnlyMemory<byte>> ReadAllAsync<T>(string fileName) where T : class
    {
        await using Stream? resourceStream = ReadStream<T>(fileName);
        using MemoryStream memoryStream = new();

        await resourceStream!.CopyToAsync(memoryStream);

        return new ReadOnlyMemory<byte>(memoryStream.ToArray());
    }
}
