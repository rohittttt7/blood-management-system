using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankManagementSystem.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(5)]
        public string? BloodGroup { get; set; }

        [Required]
        public int Age { get; set; }

        public string? MedicalCondition { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.Now;
    }
}
