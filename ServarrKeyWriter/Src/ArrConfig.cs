using System.Xml;

namespace ServarrKeyWriter;

public class ArrConfig
{
    private string ConfigFolder => Environment.GetEnvironmentVariable("CONFIG_FOLDER") ?? "/config";

    private string ConfigFile => "config.xml";

    private string ConfigPath => Path.Combine(ConfigFolder, ConfigFile);

    public string? ReadApiKey()
    {
        if (!File.Exists(ConfigPath)) {
            return null;
        }

        var document = new XmlDocument();

        try {
            document.Load(ConfigPath);

            return document.SelectSingleNode("/Config/ApiKey")?.InnerText;
        }
        catch (XmlException) {
            Console.WriteLine("Invalid xml");
        }

        return null;
    }

    public bool WriteApiKey(string apiKey)
    {
        if (!File.Exists(ConfigPath)) {
            Console.WriteLine($"Config file not found at {ConfigPath}");
            return false;
        }

        var document = new XmlDocument();

        try {
            document.Load(ConfigPath);

            var apiKeyNode = document.SelectSingleNode("/Config/ApiKey");

            if (apiKeyNode == null) {
                throw new XmlException("API key not found");
            }

            apiKeyNode.InnerText = apiKey;

            document.Save(ConfigPath);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }
}