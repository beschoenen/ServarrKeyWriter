using System.Text;
using k8s;
using k8s.Models;

namespace ServarrKeyWriter;

public class SecretWatcher(KubernetesClient kubernetes)
{
    private readonly string _secretName = Environment.GetEnvironmentVariable("SECRET_NAME") ?? throw new KeyNotFoundException("Please set the SECRET_NAME env variable");

    private readonly string _secretNamespace = Environment.GetEnvironmentVariable("SECRET_NAMESPACE") ?? kubernetes.Config.Namespace;

    private readonly string _secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "api-key";

    private readonly ManualResetEventSlim _event = new(false);

    public void Start(Func<string, bool> callback)
    {
        Console.WriteLine("Starting secret watcher");

        var secretListResponse = kubernetes.Client.CoreV1.ListNamespacedSecretWithHttpMessagesAsync(_secretNamespace, watch: true);

        secretListResponse.Watch<V1Secret, V1SecretList>((_, item) => {
            if (item.Metadata.Name != _secretName) {
                return;
            }

            var apiKey = GetApiKey(item, _secretKey);

            if (apiKey == null) {
                return;
            }

            callback(apiKey);
        });

        _event.Wait();
    }

    public void Stop()
    {
        _event.Set();
    }

    private static string? GetApiKey(V1Secret secret, string key)
    {
        return secret.Data.TryGetValue(key, out var value)
            ? Encoding.Default.GetString(value)
            : null;
    }
}