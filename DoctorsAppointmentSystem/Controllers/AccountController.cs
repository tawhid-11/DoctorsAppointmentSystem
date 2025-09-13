using DoctorsAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoctorsAppointmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 ApplicationDbContext context,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        // ===================== Register =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true // optional
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ensure role exists before assigning
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }

                await _userManager.AddToRoleAsync(user, model.Role);

                // Save extra info
                if (model.Role == "Patient")
                {
                    _context.Patients.Add(new Patient
                    {
                        FullName = model.FullName,
                        Age = model.Age,
                        Contact = model.Contact,
                        IdentityUserId = user.Id
                    });
                }
                else if (model.Role == "Doctor")
                {
                    _context.Doctors.Add(new Doctor
                    {
                        FullName = model.FullName,
                        Age = model.Age,
                        Contact = model.Contact,
                        IdentityUserId = user.Id
                    });
                }

                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect based on role
                if (model.Role == "Patient")
                    return RedirectToAction("PatientIndex", "Appointment");
                else if (model.Role == "Doctor")
                    return RedirectToAction("DoctorIndex", "Appointment");

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ===================== Login =====================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Get roles for the logged-in user
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Patient"))
                    return RedirectToAction("PatientIndex", "Appointment"); // patient booking page
                else if (roles.Contains("Doctor"))
                    return RedirectToAction("DoctorIndex", "Appointment"); // doctor's appointment list

                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "This account has been locked out due to multiple failed login attempts.");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        // ===================== Logout =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // ===================== Access Denied =====================
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ===================== Helper =====================
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
