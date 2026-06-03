using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BloodBankManagementSystem.Models;
using System.Linq;

namespace BloodBankManagementSystem.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Get statistics for the dashboard
            var totalDonors = _context.Donors?.Count() ?? 0;
            var totalPatients = _context.Patients?.Count() ?? 0;
            var pendingRequests = _context.BloodRequests?.Where(r => r.Status == "Pending").Count() ?? 0;
            var totalBloodUnits = _context.BloodGroups?.Sum(b => b.AvailableUnits) ?? 0;

            ViewBag.TotalDonors = totalDonors;
            ViewBag.TotalPatients = totalPatients;
            ViewBag.PendingRequests = pendingRequests;
            ViewBag.TotalBloodUnits = totalBloodUnits;

            return View();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
    }
}
