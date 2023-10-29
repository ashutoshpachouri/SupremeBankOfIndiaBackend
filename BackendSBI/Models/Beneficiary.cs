using System.ComponentModel.DataAnnotations;

namespace BackendSBI.Models
{
    public class Beneficiary
    {
        [Required]
        public string Name { get; set; }
        [Key]
        [Required]
        public string AccountNumber { get; set; }
    }
}
