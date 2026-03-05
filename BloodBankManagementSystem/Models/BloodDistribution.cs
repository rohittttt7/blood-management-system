using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class BloodDistribution
    {
        [Key]
        public int DistributionId { get; set; }

        [Required]
        public int RequestId { get; set; }

        [ForeignKey("RequestId")]
        public BloodRequest? BloodRequest { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodGroup { get; set; }

        [Required]
        public int UnitsDistributed { get; set; }

        public DateTime DistributionDate { get; set; } = DateTime.Now;

        public string? DistributedBy { get; set; } // Admin userId

        public string? Remarks { get; set; }
    }
}
