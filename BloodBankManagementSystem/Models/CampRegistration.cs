using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class CampRegistration
    {
        [Key]
        public int CampRegistrationId { get; set; }

        [Required]
        public int CampId { get; set; }

        [ForeignKey("CampId")]
        public DonationCamp? Camp { get; set; }

        [Required]
        public int DonorId { get; set; }

        [ForeignKey("DonorId")]
        public Donor? Donor { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.Now;
    }
}
