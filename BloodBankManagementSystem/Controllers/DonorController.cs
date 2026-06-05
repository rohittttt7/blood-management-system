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
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            // Compare by date only (DateTime.Today, not Now) so a camp scheduled for TODAY
            // isn't hidden just because its midnight timestamp is earlier than the current time.
            var camps = await _context.DonationCamps
                .Where(c => c.CampDate >= DateTime.Today && c.Status == "Scheduled")
                .OrderBy(c => c.CampDate)
                .ToListAsync();

            // Tell the view which of these camps the donor has already enrolled in.
            ViewBag.RegisteredCampIds = donor == null
                ? new List<int>()
                : await _context.CampRegistrations
                    .Where(r => r.DonorId == donor.DonorId)
                    .Select(r => r.CampId)
                    .ToListAsync();

            return View(camps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterForCamp(int campId)
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (donor == null)
            {
                return RedirectToAction("Register", "Account");
            }

            if (donor.Status != "Approved")
            {
                TempData["Error"] = "Your donor profile must be approved before you can enroll in a camp.";
                return RedirectToAction(nameof(UpcomingCamps));
            }

            var camp = await _context.DonationCamps.FirstOrDefaultAsync(c => c.CampId == campId);
            if (camp == null || camp.Status != "Scheduled" || camp.CampDate < DateTime.Today)
            {
                TempData["Error"] = "This camp is not available for registration.";
                return RedirectToAction(nameof(UpcomingCamps));
            }

            var alreadyRegistered = await _context.CampRegistrations
                .AnyAsync(r => r.CampId == campId && r.DonorId == donor.DonorId);
            if (alreadyRegistered)
            {
                TempData["Error"] = "You are already registered for this camp.";
                return RedirectToAction(nameof(UpcomingCamps));
            }

            _context.CampRegistrations.Add(new CampRegistration
            {
                CampId = campId,
                DonorId = donor.DonorId,
                RegisteredDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "You have successfully registered for the camp!";
            return RedirectToAction(nameof(UpcomingCamps));
        }

        public async Task<IActionResult> BloodAvailability()
        {
            var bloodGroups = await _context.BloodGroups.ToListAsync();
            return View(bloodGroups);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == user!.Id);

            if (donor == null)
            {
                return RedirectToAction("Register", "Account");
            }

            return View(donor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int age, double weight, string? medicalHistory)
        {
            if (age < 18 || age > 65 || weight < 50 || weight > 200)
            {
                TempData["Error"] = "Please enter a valid age (18-65) and weight (50-200 kg).";
                return RedirectToAction(nameof(UpdateProfile));
            }

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
