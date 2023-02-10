using System.Reflection;
using Ardbot.Bot.Hands;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Ardbot.Bot.Brain
{
    public class CommandHandler : ICommandHandler{
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;

        //constructor
        public CommandHandler(
            IServiceProvider provider,
            ILogger<CommandHandler> logger,
            DiscordShardedClient client,
            CommandService commands
        ){
            _logger = logger;
            _provider = provider;
            _client = client;
            _commands = commands;
        }

        public async Task InitializeAsync(){
            _logger.LogInformation("InitializeAsync!");

            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);

            _client.MessageReceived += HandleCommandAsync;

            _commands.CommandExecuted += async (optional, context, result) => {
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand){
                // the command failed, let's notify the user that something happened.
                await context.Channel.SendMessageAsync($"error: {result}");
                }
            };

            foreach( var module in _commands.Modules){
                _logger.LogInformation($"1 module was loaded: {module.Name}");
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            if (arg is not SocketUserMessage msg) 
                return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) 
                return;

            // Create a Command Context.
            var context = new ShardedCommandContext(_client, msg);
        
            var markPos = 0;
            if (msg.HasCharPrefix('!', ref markPos) || msg.HasCharPrefix('?', ref markPos))
            {
                var result = await _commands.ExecuteAsync(context, markPos, _provider);
            }
        }
    }


}