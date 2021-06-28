namespace MsaasBackend.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public string Symptom { get; set; }

        public string PastMedicalHistory { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }

        public MedicalRecordDto ToDto() => new()
        {
            Id = Id,
            AppointmentId = AppointmentId,
            Symptom = Symptom,
            PastMedicalHistory = PastMedicalHistory,
            Diagnosis = Diagnosis,
            Prescription = Prescription
        };
    }

    public class MedicalRecordForm
    {
        public int AppointmentId { get; set; }

        public string Symptom { get; set; }

        public string PastMedicalHistory { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }
    }

    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public string Symptom { get; set; }

        public string PastMedicalHistory { get; set; }

        public string Diagnosis { get; set; }

        public string Prescription { get; set; }
    }
}