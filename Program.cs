using System.Diagnostics;
using System.Reflection;
using Ardbot.Bot;
using Ardbot.Bot.Brain;
using Ardbot.Bot.Hands;
using Bot.Heart;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;

namespace Ardbot{
    public class Program
    {
        private static async Task Main(string[] args){
            bool isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));

            //TODO: Eventually set this up to get from hosting environment.
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IHostBuilder builder = new HostBuilder().ConfigureServices((hostContext, services) =>{
                //config
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                                                .AddJsonFile($"appsettings.json", true, true)
                                                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                                                .AddCommandLine(args)
                                                .Build();
                services.AddSingleton<IConfiguration>(configuration);

                //logging
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
                services.AddLogging(
                    config => {
                        config.AddSerilog()
                            .AddConfiguration(configuration.GetSection("SeriLog"));
                    }
                );

                var client = new DiscordShardedClient();
                services.AddSingleton(client);

                var commandService = new CommandService(new CommandServiceConfig{
                    LogLevel = LogSeverity.Info,
                    CaseSensitiveCommands = false
                });
                services.AddSingleton(commandService);
                
                services.AddSingleton<IHeart, Heart>();
                services.AddSingleton<IHostedService, BotHostedService>();
                services.AddSingleton<IExampleCommands, ExampleCommands>();
                services.AddSingleton<ICommandHandler, CommandHandler>();
            });

            // if (isService)
            //     await builder.RunAsServiceAsync();
            // else
                await builder.RunConsoleAsync();
        }
    }
}

