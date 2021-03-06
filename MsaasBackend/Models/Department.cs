using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace MsaasBackend.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; }

        public string Section { get; set; }

        public List<Physician> Physicians { get; set; }

        public DepartmentDto ToDto() => new()
        {
            Id = Id,
            Name = Name,
            Hospital = Hospital.ToDto(),
            Section = Section
        };
    }

    public class DepartmentDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Section { get; set; }

        public HospitalDto Hospital { get; set; }
    }

    public class DepartmentCreationForm
    {
        public int HospitalId { get; set; }
        [Required] public string Name { get; set; }

        public string Section { get; set; }
    }
}