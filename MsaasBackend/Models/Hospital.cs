namespace MsaasBackend.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

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

    public class Department
    {
        public int Id { get; set; }
        public Hospital Hospital { get; set; }
        public string Name { get; set; }
    }
}