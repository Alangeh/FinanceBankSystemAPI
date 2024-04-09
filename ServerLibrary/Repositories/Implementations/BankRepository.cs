using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;


namespace ServerLibrary.Repositories.Implementations
{
    public class BankRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Bank>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var bank = await appDbContext.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }
            bank.IsDeleted = true;
            await Commit();
            return Success();
        }

        public async Task<List<Bank>> GetAllAsync()
        {
            return await appDbContext.Banks.Where(b => !b.IsDeleted).ToListAsync();
        }

        public async Task<Bank> GetByIdAsync(int id)
        {
            return await appDbContext.Banks.Where(b => !b.IsDeleted && b.Id == id).FirstAsync();
        }

        public async Task<GeneralResponse> Insert(Bank item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Bank already exist");
            appDbContext.Banks.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Bank item)
        {
            var bank = await appDbContext.Banks.Where(b => !b.IsDeleted && b.Id == item.Id).FirstAsync();
            if (bank is null) return NotFound();
            bank.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Error, Bank not found!");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Banks.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()) && !x.IsDeleted);
            return item is null;
        }
    }
}
