using Microsoft.Extensions.Configuration;

namespace ch2;

public class AppSettings
{
    public string SASToken { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public string ContainerName { get; set; } = null!;

    public static AppSettings? LoadAppSettings()
    {
        IConfigurationRoot configRoot = new ConfigurationBuilder()
        .AddJsonFile("AppSettings.json")
        .Build();

        AppSettings? appSettings = configRoot.Get<AppSettings>();

        return appSettings;
    }
}