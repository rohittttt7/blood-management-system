using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodBankManagementSystem.Models;

namespace BloodBankManagementSystem.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private static readonly HashSet<string> AllowedBloodGroups = new(StringComparer.OrdinalIgnoreCase)
        {
            "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
        };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (patient == null)
            {
                return RedirectToAction("Register", "Account");
            }

            ViewBag.Patient = patient;
            ViewBag.TotalRequests = await _context.BloodRequests
                .CountAsync(r => r.PatientId == patient.PatientId);
            ViewBag.PendingRequests = await _context.BloodRequests
                .CountAsync(r => r.PatientId == patient.PatientId && r.Status == "Pending");

            return View();
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);

            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> RequestBlood()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.PatientBloodGroup = patient.BloodGroup;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestBlood(string bloodGroup, int unitsRequired, string? reason, bool isEmergency)
        {
            if (string.IsNullOrWhiteSpace(bloodGroup) || !AllowedBloodGroups.Contains(bloodGroup) || unitsRequired <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid blood group and units required.");
                ViewBag.PatientBloodGroup = bloodGroup;
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (patient != null)
            {
                var request = new BloodRequest
                {
                    PatientId = patient.PatientId,
                    BloodGroup = bloodGroup,
                    UnitsRequired = unitsRequired,
                    Reason = reason,
                    IsEmergency = isEmergency,
                    RequestDate = DateTime.Now,
                    Status = "Pending"
                };

                _context.BloodRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Blood request submitted successfully! Please wait for admin approval.";
                return RedirectToAction(nameof(MyRequests));
            }

            return View();
        }

        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (patient == null)
            {
                return NotFound();
            }

            var requests = await _context.BloodRequests
                .Where(r => r.PatientId == patient.PatientId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> BloodAvailability()
        {
            var bloodGroups = await _context.BloodGroups
                .OrderBy(b => b.BloodType)
                .ToListAsync();
            return View(bloodGroups);
        }

        public async Task<IActionResult> SearchDonors(string? bloodGroup)
        {
            var donors = await _context.Donors
                .Include(d => d.User)
                .Where(d => d.Status == "Approved" && d.IsEligible)
                .Where(d => string.IsNullOrEmpty(bloodGroup) || d.BloodGroup == bloodGroup)
                .ToListAsync();

            ViewBag.SelectedBloodGroup = bloodGroup;
            return View(donors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (patient == null)
            {
                return NotFound();
            }

            var request = await _context.BloodRequests
                .FirstOrDefaultAsync(r => r.RequestId == id && r.PatientId == patient.PatientId);

            if (request != null && request.Status == "Pending")
            {
                _context.BloodRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Request cancelled successfully!";
            }

            return RedirectToAction(nameof(MyRequests));
        }
    }
}
