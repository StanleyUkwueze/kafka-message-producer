namespace EmployeeApplication.Models
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }

        public Employee(Guid id, string name, string surName)
        {
            Id = id;
            Name = name;
            SurName = surName;
        }

    }
}
