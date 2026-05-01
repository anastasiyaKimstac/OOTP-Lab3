using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Tester employee type - ensures software quality
    /// </summary>
    public class Tester : Employee
    {
        public string TestingTool { get; set; }
        public int BugsFound { get; set; }

        public Tester()
        {
            TestingTool = "Selenium";
            BugsFound = 0;
            Salary = 55000m;
            Name = "New Tester";
        }

        public override string Accept(IEmployeeVisitor visitor) => visitor.Visit(this);

        private string _employeeType = "Tester";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new Tester
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                TestingTool = this.TestingTool,
                BugsFound = this.BugsFound
            };
        }
    }
}