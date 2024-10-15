using ServarrKeyWriter;

var kubernetes = new KubernetesClient();
var secretWatcher = new SecretWatcher(kubernetes);
var arrApi = new ArrApi();
var arrConfig = new ArrConfig();

var restart = (Environment.GetEnvironmentVariable("RESTART_ARR") ?? "true") == "true";

secretWatcher.Start(apiKey => {
    var oldApiKey = arrConfig.ReadApiKey();
    var success = arrConfig.WriteApiKey(apiKey);

    if (success && oldApiKey != null && restart) {
        arrApi.Restart(oldApiKey);
    }

    return success;
});