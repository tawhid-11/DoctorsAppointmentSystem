using Microsoft.EntityFrameworkCore;

namespace DoctorsAppointmentSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.SlotTemplates.Any())
            {
                var slots = new[]
                {
                    new SlotTemplate { SlotTime = "9:00 AM" },
                    new SlotTemplate { SlotTime = "10:00 AM" },
                    new SlotTemplate { SlotTime = "11:00 AM" },
                    new SlotTemplate { SlotTime = "12:00 PM" },
                    new SlotTemplate { SlotTime = "2:00 PM" },
                    new SlotTemplate { SlotTime = "3:00 PM" },
                    new SlotTemplate { SlotTime = "4:00 PM" },
                    new SlotTemplate { SlotTime = "5:00 PM" }
                };

                context.SlotTemplates.AddRange(slots);
                context.SaveChanges();
            }
        }
    }
}
