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
    }
    
    public class MedicalRecordForm
    {
        public string Symptom { get; set; }

        public string PastMedicalHistory { get; set; }

        public string Diagnosis { get; set; }
    }
}