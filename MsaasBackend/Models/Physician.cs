namespace MsaasBackend.Models
{
    public class Physician : User
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}