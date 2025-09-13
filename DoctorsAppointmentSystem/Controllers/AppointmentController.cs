using DoctorsAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AppointmentController(ApplicationDbContext context,
                                 UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ================== Patient View: Book & List Appointments ==================
    public async Task<IActionResult> PatientIndex()
    {
        var userId = _userManager.GetUserId(User);

        // Get patient info
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        if (patient == null)
        {
            TempData["Error"] = "Patient record not found.";
            return View(new PatientIndexViewModel());
        }

        // Available slots
        var slots = await _context.SlotTemplates.ToListAsync();

        // Upcoming appointments
        var appointments = await _context.Appointments
            .Include(a => a.SlotTemplate)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patient.PatientId && a.AppointmentDate.Date >= DateTime.Today)
            .ToListAsync();

        // Map to PatientAppointmentViewModel
        var appointmentModels = appointments.Select(a => new PatientAppointmentViewModel
        {
            AppointmentId = a.AppointmentId,
            AppointmentDate = a.AppointmentDate,
            SlotTime = a.SlotTemplate.SlotTime,
            DoctorName = a.Doctor.FullName,
            Status = a.Status
        }).ToList();

        var model = new PatientIndexViewModel
        {
            Slots = slots,
            Appointments = appointmentModels
        };

        return View(model);
    }

    // ================== Doctor View: List Appointments ==================
    public async Task<IActionResult> DoctorIndex()
    {
        var userId = _userManager.GetUserId(User);

        // Get doctor info
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdentityUserId == userId);
        if (doctor == null)
        {
            TempData["Error"] = "Doctor record not found.";
            return View(new List<DoctorAppointmentViewModel>());
        }

        // Doctor appointments
        var appointments = await _context.Appointments
            .Include(a => a.SlotTemplate)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctor.DoctorId && a.AppointmentDate.Date >= DateTime.Today)
            .Select(a => new DoctorAppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                SlotTime = a.SlotTemplate.SlotTime,
                PatientName = a.Patient.FullName,
                Status = a.Status
            })
            .ToListAsync();

        return View(appointments);
    }

    // ================== Book Appointment (Patient Only) ==================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int slotId, DateTime appointmentDate)
    {
        var userId = _userManager.GetUserId(User);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        if (patient == null)
        {
            TempData["Error"] = "Patient not found!";
            return RedirectToRoleIndex();
        }

        // Example: book for doctor with Id = 1
        var existing = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == 1 &&
                                      a.SlotTemplateId == slotId &&
                                      a.AppointmentDate.Date == appointmentDate.Date &&
                                      a.Status == "Booked");

        if (existing != null)
        {
            TempData["Error"] = "This slot is already booked!";
            return RedirectToRoleIndex();
        }

        var appointment = new Appointment
        {
            PatientId = patient.PatientId,
            DoctorId = 1, // default doctor
            SlotTemplateId = slotId,
            AppointmentDate = appointmentDate,
            Status = "Booked"
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Appointment booked successfully!";
        return RedirectToRoleIndex();
    }

    // ================== Cancel Appointment ==================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int appointmentId)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null)
        {
            TempData["Error"] = "Appointment not found!";
            return RedirectToRoleIndex();
        }

        if ((appointment.AppointmentDate - DateTime.Now).TotalHours < 1)
        {
            TempData["Error"] = "Cannot cancel within 1 hour of the appointment.";
            return RedirectToRoleIndex();
        }

        appointment.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Success"] = "Appointment cancelled successfully!";
        return RedirectToRoleIndex();
    }

    // ================== Reschedule Appointment ==================
    [HttpGet]
    public async Task<IActionResult> Reschedule(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.SlotTemplate)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);

        if (appointment == null || appointment.Status != "Booked")
        {
            TempData["Error"] = "Appointment not found or cannot be rescheduled!";
            return RedirectToRoleIndex();
        }

        ViewBag.Slots = await _context.SlotTemplates.ToListAsync();
        return View(appointment);
    }

    // ================== Doctor: Daily Appointment Summary ==================
    public async Task<IActionResult> DoctorDailySummary()
    {
        var userId = _userManager.GetUserId(User);

        // Get doctor info
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdentityUserId == userId);
        if (doctor == null)
        {
            TempData["Error"] = "Doctor record not found.";
            return View(new List<DoctorDailySummaryViewModel>());
        }

        // Today's appointments
        var today = DateTime.Today;
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.SlotTemplate)
            .Where(a => a.DoctorId == doctor.DoctorId &&
                        a.AppointmentDate.Date == today)
            .Select(a => new DoctorDailySummaryViewModel
            {
                SlotTime = a.SlotTemplate.SlotTime,
                PatientName = a.Patient.FullName,
                Status = a.Status
            })
            .ToListAsync();

        return View(appointments);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reschedule(int appointmentId, int slotId, DateTime appointmentDate)
    {
        var appointment = await _context.Appointments.FindAsync(appointmentId);
        if (appointment == null || appointment.Status != "Booked")
        {
            TempData["Error"] = "Appointment not found or cannot be rescheduled!";
            return RedirectToRoleIndex();
        }

        var existing = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId &&
                                      a.SlotTemplateId == slotId &&
                                      a.AppointmentDate.Date == appointmentDate.Date &&
                                      a.Status == "Booked");

        if (existing != null)
        {
            TempData["Error"] = "Selected slot is already booked!";
            return RedirectToRoleIndex();
        }

        appointment.SlotTemplateId = slotId;
        appointment.AppointmentDate = appointmentDate;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Appointment rescheduled successfully!";
        return RedirectToRoleIndex();
    }



    // ================= Helper: Redirect based on role =================
    private IActionResult RedirectToRoleIndex()
    {
        var userId = _userManager.GetUserId(User);
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        var roles = _userManager.GetRolesAsync(user).Result;

        if (roles.Contains("Doctor"))
            return RedirectToAction("DoctorIndex");
        else
            return RedirectToAction("PatientIndex");
    }
}
