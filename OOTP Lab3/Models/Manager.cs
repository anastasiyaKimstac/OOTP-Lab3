using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Manager employee type - responsible for team leadership
    /// </summary>
    public class Manager : Employee
    {
        public int TeamSize { get; set; }
        public string Department { get; set; }

        public Manager()
        {
            TeamSize = 5;
            Department = "IT";
            Salary = 80000m;
            Name = "New Manager";
        }

        public override string Accept(IEmployeeVisitor visitor) => visitor.Visit(this);

        private string _employeeType = "Manager";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new Manager
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                TeamSize = this.TeamSize,
                Department = this.Department
            };
        }
    }
}