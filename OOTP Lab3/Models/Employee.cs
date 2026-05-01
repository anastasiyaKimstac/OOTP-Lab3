using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Abstract base class implementing common functionality for all employees
    /// </summary>
    public abstract class Employee : IEmployee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public int YearsOfExperience { get; set; }

        protected Employee()
        {
            Id = Guid.NewGuid();
            Name = "New Employee";
            Salary = 30000m;
            YearsOfExperience = 0;
        }

        public abstract string Accept(IEmployeeVisitor visitor);
        public abstract string EmployeeType { get; protected set; }
        public abstract IEmployee Clone();

        public override string ToString()
        {
            return $"{EmployeeType}: {Name} (${Salary:N0})";
        }
    }
}