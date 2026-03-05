using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodBankManagementSystem.Models;

namespace BloodBankManagementSystem.Controllers
{
    [Authorize(Roles = "Donor")]
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (donor == null)
            {
                return RedirectToAction("Register", "Account");
            }

            ViewBag.Donor = donor;
            ViewBag.TotalDonations = donor.TotalDonations;
            ViewBag.LastDonation = donor.LastDonationDate?.ToString("dd MMM yyyy") ?? "Never";
            ViewBag.NextEligible = donor.NextEligibleDate?.ToString("dd MMM yyyy") ?? "Now";
            ViewBag.Status = donor.Status;

            return View();
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == user!.Id);

            return View(donor);
        }

        public async Task<IActionResult> DonationHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (donor == null)
            {
                return NotFound();
            }

            var donations = await _context.DonationRecords
                .Where(d => d.DonorId == donor.DonorId)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            return View(donations);
        }

        public async Task<IActionResult> UpcomingCamps()
        {
            var camps = await _context.DonationCamps
                .Where(c => c.CampDate >= DateTime.Now && c.Status == "Scheduled")
                .OrderBy(c => c.CampDate)
                .ToListAsync();

            return View(camps);
        }

        public async Task<IActionResult> BloodAvailability()
        {
            var bloodGroups = await _context.BloodGroups.ToListAsync();
            return View(bloodGroups);
        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int age, double weight, string? medicalHistory)
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (donor != null)
            {
                donor.Age = age;
                donor.Weight = weight;
                donor.MedicalHistory = medicalHistory;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Profile updated successfully!";
            }

            return RedirectToAction(nameof(MyProfile));
        }
    }
}
