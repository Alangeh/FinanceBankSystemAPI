
using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class TransactionListRepository(AppDbContext appDbContext) : ITransactionListInterface
    {
        public async Task<List<Transaction>> GetAllAsync()
        {
            return await appDbContext.Transactions.Where(u => !u.IsDeleted).ToListAsync();
        }
    }
}
