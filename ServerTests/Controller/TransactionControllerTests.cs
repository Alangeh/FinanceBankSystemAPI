using BaseLibrary.Entities;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Controllers;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;


namespace ServerTests.Controller
{
    public class TransactionControllerTests
    {
        private async Task<AppDbContext> GetAppDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Transactions.CountAsync() <= 0)
            {
                databaseContext.Transactions.Add(
                    new Transaction()
                    {
                        Id = 1,
                        SenderId = 1,
                        ReceiverId = 1,
                        SenderBankId = 1,
                        ReceiverBankId = 1,
                        TransferAmount = 1000,
                        TransferConfirmDate = DateTime.Now,
                        TransferConfirmStatus = 1,
                        TransferDate = DateTime.Now,
                        TransferNotes = "This is a note",
                        IsDeleted = false
                    });
                await databaseContext.SaveChangesAsync();
            }

            if (await databaseContext.UserAccounts.CountAsync() <= 0)
            {
                databaseContext.UserAccounts.Add(new UserTransactionAccount
                {
                    Id = 1,
                    UserId = 1,
                    BankId = 1,
                    AccountNumber = "XXX123AAA",
                    AccountType = "EUR",
                    Balance = 123000,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                });
                databaseContext.UserAccounts.Add(new UserTransactionAccount
                {
                    Id = 2,
                    UserId = 2,
                    BankId = 1,
                    AccountNumber = "123XXXAAA",
                    AccountType = "EUR",
                    Balance = 11000,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                });
                await databaseContext.SaveChangesAsync();
            }

            return databaseContext;
        }

        [Fact]
        public async void TransactionRepository_TransferFunds_ReturnOk()
        {
            // Arrangg
            var senderAccountNumber = "XXX123AAA";
            var receiverAccountNumber = "123XXXAAA";
            var transactionAmount = 2000;
            var note = "transaction note";
            var genericRepositoryInteface = A.Fake<IGenericRepositoryInterface<UserTransactionAccount>>();
            var dbContext = await GetAppDbContext();

            var sender = A.Fake<UserTransactionAccount>();
            var receiver = A.Fake<UserTransactionAccount>();
            var controller = new TransactionController(genericRepositoryInteface, dbContext);

            // Act
            var result = await controller.TransferFunds(senderAccountNumber, receiverAccountNumber, transactionAmount, note);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

        }
    }
}
