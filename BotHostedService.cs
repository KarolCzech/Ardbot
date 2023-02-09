using Bot;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ardbot.Bot{
    public class BotHostedService : BackgroundService {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _config;
        public BotHostedService(
            IServiceScopeFactory serviceScopeFactory, 
            IConfiguration config){
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken){
            using (IServiceScope scope = _serviceScopeFactory.CreateScope()){
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<BotHostedService>>();

                var botHeart = scope.ServiceProvider.GetRequiredService<IHeart>();

                try{
                    await botHeart.ComeAlive(cancellationToken);
                }
                catch(Exception e){

                }
            }
            
        }
    }
}