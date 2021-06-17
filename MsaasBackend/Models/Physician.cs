using System.ComponentModel.DataAnnotations;

namespace MsaasBackend.Models
{
    public class Physician
    {
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public PhysicianDto ToDto() => new()
        {
            UserId = UserId,
            DepartmentId = DepartmentId
        };
    }

    public class PhysicianRegisterForm
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }
    }

    public class PhysicianDto
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }
    }
}