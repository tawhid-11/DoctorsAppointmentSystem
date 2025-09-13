using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    // Patient
    [Required]
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    // Doctor
    [Required]
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    // Slot
    [Required]
    public int SlotTemplateId { get; set; }
    public SlotTemplate? SlotTemplate { get; set; }

    // Appointment Date
    [Required]
    public DateTime AppointmentDate { get; set; }

    // Status: "Booked", "Cancelled", "Rescheduled"
    [Required]
    public string Status { get; set; }
}
