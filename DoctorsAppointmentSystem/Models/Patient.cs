using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Patient
{
    [Key]
    public int PatientId { get; set; }

    
    [StringLength(100)]
    public required string FullName { get; set; }

    [Range(1, 120)]
    public int Age { get; set; }

    
    [Phone]
    public required string Contact { get; set; }

    public ICollection<Appointment>? Appointments { get; set; }

    // Link to Identity User
    public string? IdentityUserId { get; set; }
}
