using DoctorsAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[Authorize] // All actions require login by default
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger,
                          UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [AllowAnonymous] // Allow everyone to view home page
    public IActionResult Index()
    {
        // If not logged in, show default home view
        if (!User.Identity.IsAuthenticated)
            return View();

        // Get current user and roles
        var user = _userManager.GetUserAsync(User).Result;
        var roles = _userManager.GetRolesAsync(user).Result;

        // Redirect based on role
        if (roles.Contains("Patient"))
            return RedirectToAction("PatientIndex", "Appointment");
        else if (roles.Contains("Doctor"))
            return RedirectToAction("DoctorIndex", "Appointment");

        // Fallback
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
