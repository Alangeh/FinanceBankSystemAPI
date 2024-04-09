using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(IGenericRepositoryInterface<UserTransactionAccount> genericRepositoryInterface, AppDbContext appDbContext) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await appDbContext.Transactions.ToListAsync());
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds(string senderAccountNumber, string receiverAccountNumber, double transactionAmount, string notes)
        {
            // get sender by account number
            UserTransactionAccount sender = new();
            sender = await appDbContext.UserAccounts.Where(u => u.AccountNumber == senderAccountNumber).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(senderAccountNumber))
                return BadRequest("Invalid Sender Transaction number");

            // get receiver by account number
            UserTransactionAccount receiver = new();
            receiver = await appDbContext.UserAccounts.Where(u => u.AccountNumber == receiverAccountNumber).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(receiverAccountNumber))
                return BadRequest("Invalid Reciever Transaction number");

            //check and update User bank 
            if(transactionAmount > sender.Balance)
            {
                return BadRequest("Insufficient funds");
            }

            sender.Balance = sender.Balance - transactionAmount;

            // add transaction values
            Transaction currentTransaction = new()
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                SenderBankId = sender.BankId,
                ReceiverBankId = receiver.BankId,
                TransferAmount = transactionAmount,
                TransferNotes = notes,
                TransferConfirmStatus = 0,
                TransferConfirmDate = null,
                TransferDate = DateTime.Now,
                IsDeleted = false
            };

            // add transaction to database and save
            appDbContext.Transactions.Add(currentTransaction);
            return Ok(await appDbContext.SaveChangesAsync());
        }

        [HttpPost("confirm-transaction")]
        public async Task<IActionResult> ConfirmTransaction(int transactionId)
        {
            Transaction transaction = new();
            transaction = await appDbContext.Transactions.Where(t => t.Id == transactionId).FirstOrDefaultAsync();
            if (transaction == null) return BadRequest("Transaction not found");

            transaction.TransferConfirmStatus = 1;
            transaction.TransferConfirmDate = DateTime.Now;
            return Ok(await appDbContext.SaveChangesAsync());
        }
    }
}
