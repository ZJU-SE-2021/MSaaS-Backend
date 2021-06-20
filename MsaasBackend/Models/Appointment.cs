using System;

namespace MsaasBackend.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PhysicianId { get; set; }
        public Physician Physician { get; set; }

        public DateTime Time { get; set; }

        public string Description { get; set; }

        public AppointmentDto ToDto() => new()
        {
            Id = Id,
            User = User.ToDto(),
            Physician = Physician.ToDto(),
            Time = Time,
            Description = Description
        };
    }

    public class AppointmentForm
    {
        public int PhysicianId { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }

    public class AppointmentDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public PhysicianDto Physician { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }
}