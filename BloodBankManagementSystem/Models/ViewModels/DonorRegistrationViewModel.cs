using System.ComponentModel.DataAnnotations;

namespace BloodBankManagementSystem.Models.ViewModels
{
    public class DonorRegistrationViewModel
    {
        [Required]
        [Display(Name = "Blood Group")]
        public string? BloodGroup { get; set; }

        [Required]
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65")]
        public int Age { get; set; }

        [Required]
        [Range(50, 200, ErrorMessage = "Weight must be between 50 and 200 kg")]
        public double Weight { get; set; }

        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }
    }
}
