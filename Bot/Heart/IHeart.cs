using Discord.Commands;

namespace Bot.Heart {
    public interface IHeart{
        Task ComeAlive(
            CancellationToken cancellationToken
        );
    }
}