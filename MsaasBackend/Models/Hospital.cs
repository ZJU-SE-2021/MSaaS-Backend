using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MsaasBackend.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Department> Departments { get; set; }

        public HospitalDto toDto() => new()
        {
            Id = Id,
            Address = Address,
            Name = Name,
        };
    }

    public class HospitalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class HospitalCreationForm
    {
        [Required] public string Name { get; set; }

        [Required] public string Address { get; set; }
    }
}