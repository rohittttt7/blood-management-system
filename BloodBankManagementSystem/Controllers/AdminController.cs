using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodBankManagementSystem.Models;

namespace BloodBankManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class AdminController : Controller
    {
        private static readonly HashSet<string> AllowedBloodGroups = new(StringComparer.OrdinalIgnoreCase)
        {
            "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"
        };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.TotalDonors = _context.Donors.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.PendingRequests = _context.BloodRequests.Count(r => r.Status == "Pending");
            ViewBag.PendingDonors = _context.Donors.Count(d => d.Status == "Pending");
            ViewBag.TotalBloodUnits = _context.BloodGroups.Sum(b => b.AvailableUnits);
            return View();
        }

        // Donor Management
        public async Task<IActionResult> ManageDonors()
        {
            var donors = await _context.Donors.Include(d => d.User).ToListAsync();
            return View(donors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor != null)
            {
                donor.Status = "Approved";
                donor.IsEligible = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Donor approved successfully!";
            }
            return RedirectToAction(nameof(ManageDonors));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor != null)
            {
                donor.Status = "Rejected";
                donor.IsEligible = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Donor rejected!";
            }
            return RedirectToAction(nameof(ManageDonors));
        }

        // Blood Inventory Management
        public async Task<IActionResult> BloodInventory()
        {
            var bloodGroups = await _context.BloodGroups.ToListAsync();
            return View(bloodGroups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInventory(int id, int units)
        {
            if (units < 0)
            {
                TempData["Error"] = "Units must be zero or more.";
                return RedirectToAction(nameof(BloodInventory));
            }

            var bloodGroup = await _context.BloodGroups.FindAsync(id);
            if (bloodGroup != null)
            {
                bloodGroup.AvailableUnits = units;
                bloodGroup.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inventory updated successfully!";
            }
            return RedirectToAction(nameof(BloodInventory));
        }

        // Blood Request Management
        public async Task<IActionResult> BloodRequests()
        {
            var requests = await _context.BloodRequests
                .Include(r => r.Patient)
                .ThenInclude(p => p!.User)
                .OrderByDescending(r => r.IsEmergency)
                .ThenByDescending(r => r.RequestDate)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id, string remarks)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var request = await _context.BloodRequests.FindAsync(id);
            if (request != null)
            {
                if (!string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "Only pending requests can be approved.";
                    return RedirectToAction(nameof(BloodRequests));
                }

                if (request.UnitsRequired <= 0 || string.IsNullOrWhiteSpace(request.BloodGroup) || !AllowedBloodGroups.Contains(request.BloodGroup))
                {
                    TempData["Error"] = "Invalid request data.";
                    return RedirectToAction(nameof(BloodRequests));
                }

                var bloodGroup = await _context.BloodGroups
                    .FirstOrDefaultAsync(b => b.BloodType == request.BloodGroup);

                if (bloodGroup != null && bloodGroup.AvailableUnits >= request.UnitsRequired)
                {
                    request.Status = "Approved";
                    request.AdminRemarks = remarks;
                    request.ApprovedDate = DateTime.Now;
                    request.ApprovedBy = User.Identity!.Name;

                    // Create distribution record
                    var distribution = new BloodDistribution
                    {
                        RequestId = request.RequestId,
                        BloodGroup = request.BloodGroup,
                        UnitsDistributed = request.UnitsRequired,
                        DistributionDate = DateTime.Now,
                        DistributedBy = User.Identity.Name,
                        Remarks = remarks
                    };

                    _context.BloodDistributions.Add(distribution);

                    // Update inventory
                    bloodGroup.AvailableUnits -= request.UnitsRequired;
                    bloodGroup.LastUpdated = DateTime.Now;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = "Blood request approved and distributed!";
                }
                else
                {
                    TempData["Error"] = "Insufficient blood units available!";
                }
            }
            return RedirectToAction(nameof(BloodRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id, string remarks)
        {
            var request = await _context.BloodRequests.FindAsync(id);
            if (request != null)
            {
                if (!string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "Only pending requests can be rejected.";
                    return RedirectToAction(nameof(BloodRequests));
                }

                request.Status = "Rejected";
                request.AdminRemarks = remarks;
                request.ApprovedDate = DateTime.Now;
                request.ApprovedBy = User.Identity!.Name;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Blood request rejected!";
            }
            return RedirectToAction(nameof(BloodRequests));
        }

        // Donation Records
        public async Task<IActionResult> DonationRecords()
        {
            var records = await _context.DonationRecords
                .Include(d => d.Donor)
                .ThenInclude(d => d!.User)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();
            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> RecordDonation()
        {
            ViewBag.Donors = await _context.Donors
                .Where(d => d.Status == "Approved" && d.IsEligible)
                .Include(d => d.User)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordDonation(int donorId, string bloodGroup, int units, string? remarks)
        {
            if (units <= 0 || string.IsNullOrWhiteSpace(bloodGroup) || !AllowedBloodGroups.Contains(bloodGroup))
            {
                TempData["Error"] = "Invalid donation values.";
                return RedirectToAction(nameof(RecordDonation));
            }

            var donor = await _context.Donors.FindAsync(donorId);
            if (donor != null)
            {
                // Create donation record
                var donation = new DonationRecord
                {
                    DonorId = donorId,
                    BloodGroup = bloodGroup,
                    UnitsCollected = units,
                    DonationDate = DateTime.Now,
                    Remarks = remarks,
                    CollectedBy = User.Identity!.Name,
                    Status = "Completed"
                };

                _context.DonationRecords.Add(donation);

                // Update donor info
                donor.LastDonationDate = DateTime.Now;
                donor.NextEligibleDate = DateTime.Now.AddDays(90); // 3 months
                donor.TotalDonations++;

                // Update blood inventory
                var bloodGroupEntity = await _context.BloodGroups
                    .FirstOrDefaultAsync(b => b.BloodType == bloodGroup);
                if (bloodGroupEntity != null)
                {
                    bloodGroupEntity.AvailableUnits += units;
                    bloodGroupEntity.LastUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Donation recorded successfully!";
                return RedirectToAction(nameof(DonationRecords));
            }

            TempData["Error"] = "Donor not found!";
            return RedirectToAction(nameof(RecordDonation));
        }

        // Donation Camps
        public async Task<IActionResult> DonationCamps()
        {
            var camps = await _context.DonationCamps.ToListAsync();
            return View(camps);
        }

        [HttpGet]
        public IActionResult CreateCamp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCamp(DonationCamp camp)
        {
            if (ModelState.IsValid)
            {
                camp.OrganizedBy = User.Identity!.Name;
                camp.CreatedDate = DateTime.Now;
                camp.Status = "Scheduled";
                _context.DonationCamps.Add(camp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Donation camp created successfully!";
                return RedirectToAction(nameof(DonationCamps));
            }
            return View(camp);
        }

        // User Management
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<Tuple<ApplicationUser, string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new Tuple<ApplicationUser, string>(user, string.Join(", ", roles)));
            }

            return View(userList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "User status updated!";
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            var model = new
            {
                TotalDonations = await _context.DonationRecords.CountAsync(),
                TotalRequests = await _context.BloodRequests.CountAsync(),
                TotalDistributions = await _context.BloodDistributions.CountAsync(),
                BloodGroups = await _context.BloodGroups.ToListAsync(),
                RecentDonations = await _context.DonationRecords
                    .Include(d => d.Donor)
                    .ThenInclude(d => d!.User)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(10)
                    .ToListAsync(),
                RecentRequests = await _context.BloodRequests
                    .Include(r => r.Patient)
                    .ThenInclude(p => p!.User)
                    .OrderByDescending(r => r.RequestDate)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
