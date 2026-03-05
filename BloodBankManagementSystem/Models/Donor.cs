using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class Donor
    {
        [Key]
        public int DonorId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodGroup { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public double Weight { get; set; }

        public string? MedicalHistory { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public DateTime? NextEligibleDate { get; set; }

        public int TotalDonations { get; set; } = 0;

        public bool IsEligible { get; set; } = true;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime RegisteredDate { get; set; } = DateTime.Now;
    }
}
