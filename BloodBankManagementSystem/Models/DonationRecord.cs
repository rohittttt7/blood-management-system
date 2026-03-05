using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class DonationRecord
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        public int DonorId { get; set; }

        [ForeignKey("DonorId")]
        public Donor? Donor { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodGroup { get; set; }

        [Required]
        public int UnitsCollected { get; set; } = 1;

        public DateTime DonationDate { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }

        public string? CollectedBy { get; set; } // Admin who recorded this

        public string Status { get; set; } = "Completed"; // Scheduled, Completed, Cancelled
    }
}
