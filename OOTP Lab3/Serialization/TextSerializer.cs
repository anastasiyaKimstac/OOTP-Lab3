using System.Globalization;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Serialization
{
    /// <summary>
    /// Visitor that serializes employee objects to pipe-delimited text format
    /// </summary>
    public class TextSerializer : IEmployeeVisitor
    {
        private static string Format(decimal value) => value.ToString(CultureInfo.InvariantCulture);

        public string Visit(Manager m)
        {
            return $"Manager|{m.Id}|{m.Name}|{Format(m.Salary)}|{m.YearsOfExperience}|{m.TeamSize}|{m.Department}";
        }

        public string Visit(Developer d)
        {
            return $"Developer|{d.Id}|{d.Name}|{Format(d.Salary)}|{d.YearsOfExperience}|{d.MainLanguage}|{d.ProjectsCompleted}";
        }

        public string Visit(Designer d)
        {
            return $"Designer|{d.Id}|{d.Name}|{Format(d.Salary)}|{d.YearsOfExperience}|{d.DesignTool}|{d.Specialization}";
        }

        public string Visit(Tester t)
        {
            return $"Tester|{t.Id}|{t.Name}|{Format(t.Salary)}|{t.YearsOfExperience}|{t.TestingTool}|{t.BugsFound}";
        }

        public string Visit(Intern i)
        {
            return $"Intern|{i.Id}|{i.Name}|{Format(i.Salary)}|{i.YearsOfExperience}|{i.University}|{i.Semester}";
        }
    }
}