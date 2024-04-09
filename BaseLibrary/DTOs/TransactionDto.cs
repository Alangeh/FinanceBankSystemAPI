
namespace BaseLibrary.DTOs
{
    public class TransactionDto
    {
        public string? SenderAccount { get; set; }
        public string? ReceiverAccount { get; set; }
        public double TransactionAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime? TransactionDate { get; set; }

    }
}
