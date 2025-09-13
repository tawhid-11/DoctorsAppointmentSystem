using System.Collections.Generic;

namespace DoctorsAppointmentSystem.Models
{
    public class PatientIndexViewModel
    {
        public List<SlotTemplate> Slots { get; set; } = new(); // available slots
        public List<PatientAppointmentViewModel> Appointments { get; set; } = new(); // upcoming appointments
    }
}
