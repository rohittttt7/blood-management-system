using System.ComponentModel.DataAnnotations;

namespace BloodBankManagementSystem.Models
{
    public class DonationCamp
    {
        [Key]
        public int CampId { get; set; }

        [Required]
        [StringLength(200)]
        public string? CampName { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        public DateTime CampDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string? Description { get; set; }

        public string? OrganizedBy { get; set; } // Admin userId

        public int ExpectedDonors { get; set; }

        public int ActualDonors { get; set; } = 0;

        public string Status { get; set; } = "Scheduled"; // Scheduled, Ongoing, Completed, Cancelled

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
