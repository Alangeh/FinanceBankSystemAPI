using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int SenderBankId { get; set; }
        public int ReceiverBankId { get; set; }
        public string? TransferNotes { get; set; }
        public double TransferAmount { get; set; }
        public int TransferConfirmStatus { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime? TransferConfirmDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
