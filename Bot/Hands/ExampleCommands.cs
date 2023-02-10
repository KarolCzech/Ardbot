using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace Ardbot.Bot.Hands {
    public class ExampleCommands : ModuleBase<ShardedCommandContext>, IExampleCommands{
        //public CommandService CommandService {get; set;}
        private ILogger<ExampleCommands> _logger;
        public ExampleCommands(ILogger<ExampleCommands> logger){
            _logger = logger;
        }

        [Command("hello", RunMode = RunMode.Async)]
        public async Task Hello(){
            _logger.LogInformation("Someone said hello!");
            await Context.Message.ReplyAsync($"Hello {Context.User.Username}.  I'm Ardbot! Beep boop!");
        }
    }
}