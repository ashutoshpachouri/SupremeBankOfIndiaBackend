using System.ComponentModel.DataAnnotations;

namespace BackendSBI.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }
        [Required]
        public string PayeeAccount { get; set; }
        [Required]
        public string PayerAccount { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime TDate { get; set; }
        [Required]
        public string Remark { get; set; }
        public string Mode { get; set; }
    }
}
