using System;

namespace DoctorsAppointmentSystem.Models
{
    public class PatientAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
    }
}
