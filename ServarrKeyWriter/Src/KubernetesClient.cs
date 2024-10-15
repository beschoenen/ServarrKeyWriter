using k8s;
using k8s.Exceptions;

namespace ServarrKeyWriter;

public class KubernetesClient
{
    public readonly Kubernetes Client;

    public readonly KubernetesClientConfiguration Config;

    public KubernetesClient()
    {
        KubernetesClientConfiguration? config;

        try {
            config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
        }
        catch (KubeConfigException) {
            config = KubernetesClientConfiguration.InClusterConfig();
        }
        catch {
            throw new KubernetesException("Could not connect to kubernetes cluster");
        }

        Config = config;
        Client = new Kubernetes(config);
    }
}