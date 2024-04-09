using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface ITransactionListInterface
    {
        Task<List<Transaction>> GetAllAsync();
    }
}
