using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Intern employee type - entry-level position for students
    /// </summary>
    public class Intern : Employee
    {
        public string University { get; set; }
        public int Semester { get; set; }

        public Intern()
        {
            University = "University";
            Semester = 3;
            Salary = 20000m;
            Name = "New Intern";
        }

        public override string Accept(IEmployeeVisitor visitor) => visitor.Visit(this);

        private string _employeeType = "Intern";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new Intern
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                University = this.University,
                Semester = this.Semester
            };
        }
    }
}