using Ardbot.Bot.Brain;
using Discord;
using Discord.Commands;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Bot.Heart {
    public class Heart : IHeart{
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly DiscordShardedClient _client;
        private readonly ICommandHandler _commands;

        public Heart(
            ILogger<Heart> logger, 
            IConfiguration config,
            DiscordShardedClient client,
            ICommandHandler commands){
            _logger = logger;
            _config = config;
            _client = client;
            _commands = commands;
        }

        public async Task ComeAlive(CancellationToken cancellationToken){

            await _commands.InitializeAsync();

            _client.ShardReady += async shard =>
            {
                await Task.Delay(5000);
                _logger.LogInformation($"Shard number {shard.ShardId} is connected");
            };

            var token = _config.GetValue<string>("DiscordToken");
            if (string.IsNullOrWhiteSpace(token)){
                _logger.LogError("No token found in config, cannot attempt connection to discord.");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            while(!cancellationToken.IsCancellationRequested){
                await Task.Delay(10000, cancellationToken);
                _logger.LogDebug("I'm alive");
            }
        }
    }
}