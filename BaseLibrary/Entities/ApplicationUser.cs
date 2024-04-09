
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreateDate { get; set; }
        
        public List<UserTransactionAccount>? TransactionAccounts { get; set; }
    }
}
