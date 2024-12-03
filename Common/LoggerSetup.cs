using Microsoft.Extensions.Configuration;
using Serilog;

namespace Common;

public class LoggerSetup
{
    public ILogger Logger { get; private set; }

    public LoggerSetup()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
            .ReadFrom.Configuration(config)
            .CreateLogger();
    }
}
