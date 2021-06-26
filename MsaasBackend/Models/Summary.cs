namespace MsaasBackend.Models
{
    public class SummaryDto
    {
        public AppointmentDto RecentAppointment { get; set; }
        public MedicalRecordDto RecentMedicalRecord { get; set; }
    }
}