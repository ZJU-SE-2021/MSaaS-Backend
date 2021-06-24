using System;
using System.Text.Json.Serialization;

namespace MsaasBackend.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PhysicianId { get; set; }
        public Physician Physician { get; set; }

        public MedicalRecord MedicalRecord { get; set; }

        public DateTime Time { get; set; }

        public string Description { get; set; }

        public AppointmentDto ToDto() => new()
        {
            Id = Id,
            User = User.ToDto(),
            Physician = Physician.ToDto(),
            Time = Time,
            Description = Description,
            MedicalRecord = MedicalRecord?.ToDto()
        };
    }

    public class AppointmentForm
    {
        public int PhysicianId { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppointmentState
    {
        Created,
        InProgress,
        Finished
    }

    public class AppointmentDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public PhysicianDto Physician { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public MedicalRecordDto MedicalRecord { get; set; }

        public AppointmentState State
        {
            get
            {
                if (MedicalRecord != null) return AppointmentState.Finished;
                return Time < DateTime.Now ? AppointmentState.InProgress : AppointmentState.Created;
            }
        }
    }
}