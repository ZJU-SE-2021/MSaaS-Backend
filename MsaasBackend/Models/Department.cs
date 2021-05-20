using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MsaasBackend.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int HospitalId { get; set; }

        //public virtual Hospital Hospital { get; set; }

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
        [Required] public string Name { get; set; }
    }
}