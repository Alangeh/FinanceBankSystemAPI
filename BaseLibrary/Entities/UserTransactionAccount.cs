
namespace BaseLibrary.Entities
{
    public class UserTransactionAccount : BaseEntity
    {
        public int UserId { get; set; }
        public int BankId { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; }
        public double Balance { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
