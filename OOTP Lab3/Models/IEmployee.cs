using System;

namespace OOTP_Lab3.Models
{
    /// <summary>
    /// Base interface for all employees in the hierarchy
    /// </summary>
    public interface IEmployee
    {
        string Accept(IEmployeeVisitor visitor);
        string EmployeeType { get; }
        IEmployee Clone();
        Guid Id { get; set; }
        string Name { get; set; }
        decimal Salary { get; set; }
        int YearsOfExperience { get; set; }
    }
}