using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserTransactionAccountRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<UserTransactionAccount>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var userAccountTransaction = await appDbContext.UserAccounts.FindAsync(id);
            if (userAccountTransaction == null)
            {
                return NotFound();
            }

            userAccountTransaction.IsDeleted = true;
            await Commit();
            return Success();
        }

        public async Task<List<UserTransactionAccount>> GetAllAsync()
        {
            return await appDbContext.UserAccounts.Where(u => !u.IsDeleted).ToListAsync();
        }

        public async Task<List<UserTransactionAccount>> GetAllByStatusAsync(bool status)
        {
            return await appDbContext.UserAccounts.Where(u => !u.IsDeleted && u.IsActive == status).ToListAsync();
        }

        public async Task<UserTransactionAccount> GetByIdAsync(int id)
        {
            return await appDbContext.UserAccounts.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted && u.IsActive);
        }

        public async Task<GeneralResponse> Insert(UserTransactionAccount item)
        {
            if (!await CheckAccountNumber(item.AccountNumber!)) return new GeneralResponse(false, "User Transaction Account already exist");
            item.CreateDate = DateTime.Now;
            appDbContext.UserAccounts.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(UserTransactionAccount item)
        {
            var userAccountTransaction = await appDbContext.UserAccounts.FindAsync(item.Id);
            if (userAccountTransaction is null) return NotFound();
            userAccountTransaction.AccountNumber = item.AccountNumber;
            userAccountTransaction.IsActive = item.IsActive;
            userAccountTransaction.IsDeleted = item.IsDeleted;
            userAccountTransaction.Balance = item.Balance;
            userAccountTransaction.AccountType = item.AccountType;

            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Error, User transaction account not found!");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckAccountNumber(string accountNumber)
        {
            var item = await appDbContext.UserAccounts.FirstOrDefaultAsync(x => x.AccountNumber!.ToLower().Equals(accountNumber.ToLower()));
            return item is null;
        }
    }
}
