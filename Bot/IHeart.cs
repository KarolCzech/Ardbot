namespace Bot {
    public interface IHeart{
        Task ComeAlive(CancellationToken cancellationToken);
    }
}