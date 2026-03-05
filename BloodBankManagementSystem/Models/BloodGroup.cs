using System.ComponentModel.DataAnnotations;

namespace BloodBankManagementSystem.Models
{
    public class BloodGroup
    {
        [Key]
        public int BloodGroupId { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodType { get; set; } // A+, A-, B+, B-, O+, O-, AB+, AB-

        public int AvailableUnits { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
