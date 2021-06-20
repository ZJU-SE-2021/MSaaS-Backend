using System.ComponentModel.DataAnnotations;

namespace MsaasBackend.Models
{
    public class Physician
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public PhysicianDto ToDto() => new()
        {
            Id = Id,
            Name = User.Name,
            Department = Department.ToDto()
        };
    }

    public class PhysicianRegisterForm
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }
    }

    public class PhysicianDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DepartmentDto Department { get; set; }
    }
}