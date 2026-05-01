using System;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Models;

namespace SeniorDeveloperPlugin
{
    /// <summary>
    /// Senior Developer - extended from Developer with additional properties
    /// </summary>
    public class SeniorDeveloper : Developer, IPluginEmployee
    {
        public string Seniority { get; set; }
        public int MentoredJuniors { get; set; }
        public string Specialization { get; set; }

        public SeniorDeveloper()
        {
            Seniority = "Senior";
            MentoredJuniors = 0;
            Specialization = "Full Stack";
            Salary = 110000m;
            Name = "New Senior Developer";
        }

        public override string Accept(IEmployeeVisitor visitor)
        {
            // If visitor can handle SeniorDeveloper, use that, otherwise fallback to Developer
            if (visitor is IExtendedEmployeeVisitor extendedVisitor)
            {
                return extendedVisitor.Visit(this);
            }
            return base.Accept(visitor);
        }

        private string _employeeType = "SeniorDeveloper";
        public override string EmployeeType
        {
            get => _employeeType;
            protected set => _employeeType = value;
        }

        public override IEmployee Clone()
        {
            return new SeniorDeveloper
            {
                Id = Guid.NewGuid(),
                Name = this.Name,
                Salary = this.Salary,
                YearsOfExperience = this.YearsOfExperience,
                MainLanguage = this.MainLanguage,
                ProjectsCompleted = this.ProjectsCompleted,
                Seniority = this.Seniority,
                MentoredJuniors = this.MentoredJuniors,
                Specialization = this.Specialization
            };
        }
    }

    /// <summary>
    /// Extended visitor for new employee types
    /// </summary>
    public interface IExtendedEmployeeVisitor : IEmployeeVisitor
    {
        string Visit(SeniorDeveloper seniorDev);
    }
}