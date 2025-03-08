namespace UetdsProgramiNet
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
