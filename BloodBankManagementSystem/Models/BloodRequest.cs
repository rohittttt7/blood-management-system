using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class BloodRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodGroup { get; set; }

        [Required]
        public int UnitsRequired { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string? Reason { get; set; }

        public bool IsEmergency { get; set; } = false;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Fulfilled

        public string? AdminRemarks { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? ApprovedBy { get; set; } // Admin userId
    }
}
