namespace ServarrKeyWriter;

public class ArrApi
{
    private static string ArrHost => Environment.GetEnvironmentVariable("ARR_HOST") ?? throw new NullReferenceException("ARR_HOST environment variable is not set");

    private readonly HttpClient _client = new();

    public async Task Restart(string apiKey)
    {
        try {
            await _client.PostAsync($"{ArrHost}/api/v3/system/restart?apiKey={apiKey}", null);
        } catch (HttpRequestException e) {
            Console.WriteLine(e);
        }
    }
}