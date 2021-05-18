using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MsaasBackend.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public List<Physician> Physicians { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HospitalId { get; set; }
    }

    public class DepartmentCreationForm
    {
        [Required] public string Name { get; set; }
    }

    public class PhysicianRegisterForm
    {
        public int PhysicianId { get; set; }
    }
}