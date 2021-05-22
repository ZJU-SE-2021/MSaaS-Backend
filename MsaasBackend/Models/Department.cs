using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MsaasBackend.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; }

        public DepartmentDto ToDto() => new()
        {
            Id = Id,
            Name = Name,
            HospitalId = HospitalId
        };
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HospitalId { get; set; }
    }

    public class DepartmentCreationForm
    {
        public int HospitalId { get; set; }
        [Required] public string Name { get; set; }
    }
}