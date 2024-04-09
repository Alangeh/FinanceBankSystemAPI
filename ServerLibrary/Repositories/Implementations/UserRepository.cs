using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<ApplicationUser>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var applicationUser = await appDbContext.ApplicationUsers.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.IsDeleted = true;
            await Commit();
            return Success();
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await appDbContext.ApplicationUsers.Where(u => !u.IsDeleted).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetAllByStatusAsync(bool status)
        {
            return await appDbContext.ApplicationUsers.Where(u => !u.IsDeleted && u.IsActive == status).ToListAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(int id)
        {
            return await appDbContext.ApplicationUsers.Where(b => !b.IsDeleted && b.Id == id && b.IsActive).FirstAsync();
        }

        public async Task<GeneralResponse> Insert(ApplicationUser item)
        {
            if (!await CheckAccount(item.Email!)) return new GeneralResponse(false, "User Account already exist");
            item.CreateDate = DateTime.UtcNow;
            appDbContext.ApplicationUsers.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(ApplicationUser item)
        {
            var applicationUser = await appDbContext.ApplicationUsers.FindAsync(item.Id);
            if (applicationUser is null) return NotFound();
            applicationUser.Fullname = item.Fullname;
            applicationUser.IsActive = item.IsActive;
            applicationUser.IsDeleted = item.IsDeleted;
            applicationUser.Email = item.Email;
            applicationUser.Password = item.Password;
            applicationUser.PhoneNumber = item.PhoneNumber;

            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Error, User transaction account not found!");
        private static GeneralResponse Success() => new(true, "Process completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckAccount(string email)
        {
            var item = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email!.ToLower().Equals(email.ToLower()));
            return item is null;
        }
    }
}
