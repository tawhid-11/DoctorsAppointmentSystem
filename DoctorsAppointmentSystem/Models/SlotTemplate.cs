using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class SlotTemplate
{
    [Key]
    public int SlotTemplateId { get; set; }

  
    public required string SlotTime { get; set; } // e.g., "09:00 AM"

    public string? Label { get; set; } // Optional: "Morning Slot 1"

    public ICollection<Appointment>? Appointments { get; set; }
}
