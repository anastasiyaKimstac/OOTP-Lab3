using OOTP_Lab3.Models;
using System;
using System.Xml.Linq;

namespace SeniorDeveloperPlugin
{
    /// <summary>
    /// Senior Developer - extended from Developer with additional properties
    /// </summary>
    public class SeniorDeveloper : Developer
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
            YearsOfExperience = 8;
            MainLanguage = "C#";
            ProjectsCompleted = 15;
        }

        public override string EmployeeType => "SeniorDeveloper";

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

        // For serialization
        public string Serialize()
        {
            return $"SeniorDeveloper|{Id}|{Name}|{Salary}|{YearsOfExperience}|{MainLanguage}|{ProjectsCompleted}|{Seniority}|{MentoredJuniors}|{Specialization}";
        }

        public static SeniorDeveloper Deserialize(string[] parts)
        {
            return new SeniorDeveloper
            {
                Id = Guid.Parse(parts[1]),
                Name = parts[2],
                Salary = decimal.Parse(parts[3]),
                YearsOfExperience = int.Parse(parts[4]),
                MainLanguage = parts[5],
                ProjectsCompleted = int.Parse(parts[6]),
                Seniority = parts[7],
                MentoredJuniors = int.Parse(parts[8]),
                Specialization = parts[9]
            };
        }
    }
}