using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Designer employee type - creates visual designs and prototypes
    /// </summary>
    public class Designer : Employee
    {
        public string DesignTool { get; set; }
        public string Specialization { get; set; }

        public Designer()
        {
            DesignTool = "Figma";
            Specialization = "UI/UX";
            Salary = 65000m;
            Name = "New Designer";
        }

        public override string Accept(IEmployeeVisitor visitor) => visitor.Visit(this);

        private string _employeeType = "Designer";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new Designer
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                DesignTool = this.DesignTool,
                Specialization = this.Specialization
            };
        }
    }
}