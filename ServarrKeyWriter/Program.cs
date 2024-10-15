using ServarrKeyWriter;

var kubernetes = new KubernetesClient();
var secretWatcher = new SecretWatcher(kubernetes);
var arrApi = new ArrApi();
var arrConfig = new ArrConfig();

var restart = (Environment.GetEnvironmentVariable("RESTART_ARR") ?? "true") == "true";

if (!restart) {
    Console.WriteLine("Restart disabled");
}

secretWatcher.Start(apiKey => {
    var oldApiKey = arrConfig.ReadApiKey();

    if (oldApiKey == apiKey) {
        Console.WriteLine("API Key unchanged");
        return false;
    }

    Console.WriteLine("API Key changed");

    var success = arrConfig.WriteApiKey(apiKey);

    if (success) {
        Console.WriteLine("API Key written to file");
    }

    if (success && oldApiKey != null && restart) {
        Console.WriteLine("Restarting application");
        arrApi.Shutdown(oldApiKey);
    }

    return success;
});