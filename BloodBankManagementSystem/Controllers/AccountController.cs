using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BloodBankManagementSystem.Models;
using BloodBankManagementSystem.Models.ViewModels;

namespace BloodBankManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    PinCode = model.PinCode,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role!);

                    // If registering as Donor, create donor profile
                    if (model.Role == "Donor")
                    {
                        return RedirectToAction("CompleteDonorProfile", new { userId = user.Id });
                    }
                    // If registering as Patient, create patient profile
                    else if (model.Role == "Patient")
                    {
                        return RedirectToAction("CompletePatientProfile", new { userId = user.Id });
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CompleteDonorProfile(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteDonorProfile(string userId, DonorRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var donor = new Donor
                {
                    UserId = userId,
                    BloodGroup = model.BloodGroup,
                    Age = model.Age,
                    Weight = model.Weight,
                    MedicalHistory = model.MedicalHistory,
                    IsEligible = true,
                    Status = "Pending",
                    RegisteredDate = DateTime.Now
                };

                _context.Donors.Add(donor);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                TempData["Success"] = "Donor registration completed! Please wait for admin approval.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = userId;
            return View(model);
        }

        [HttpGet]
        public IActionResult CompletePatientProfile(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompletePatientProfile(string userId, string bloodGroup, int age, string? medicalCondition)
        {
            var patient = new Patient
            {
                UserId = userId,
                BloodGroup = bloodGroup,
                Age = age,
                MedicalCondition = medicalCondition,
                RegisteredDate = DateTime.Now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            TempData["Success"] = "Patient registration completed successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email!,
                    model.Password!,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && !user.IsActive)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "Your account has been deactivated.");
                        return View(model);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
