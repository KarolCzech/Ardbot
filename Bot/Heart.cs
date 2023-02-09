using Microsoft.Extensions.Logging;

namespace Bot {
    public class Heart : IHeart{
        private readonly ILogger _logger;

        public Heart(ILogger<Heart> logger){
            _logger = logger;
        }

        public async Task ComeAlive(CancellationToken cancellationToken){
            while(!cancellationToken.IsCancellationRequested){
                await Task.Delay(1, cancellationToken);
                _logger.LogDebug("I'm alive");
            }
        }
    }
}