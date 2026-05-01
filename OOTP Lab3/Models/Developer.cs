using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Developer employee type - writes and maintains code
    /// </summary>
    public class Developer : Employee
    {
        public string MainLanguage { get; set; }
        public int ProjectsCompleted { get; set; }

        public Developer()
        {
            MainLanguage = "C#";
            ProjectsCompleted = 0;
            Salary = 70000m;
            Name = "New Developer";
        }

        public override string Accept(IEmployeeVisitor visitor) => visitor.Visit(this);

        private string _employeeType = "Developer";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new Developer
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                MainLanguage = this.MainLanguage,
                ProjectsCompleted = this.ProjectsCompleted
            };
        }
    }
}