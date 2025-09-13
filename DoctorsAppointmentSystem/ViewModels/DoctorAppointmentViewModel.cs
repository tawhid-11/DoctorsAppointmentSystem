using System;

namespace DoctorsAppointmentSystem.Models
{
    public class DoctorAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; }
        public string PatientName { get; set; }
        public string Status { get; set; }
    }
}
