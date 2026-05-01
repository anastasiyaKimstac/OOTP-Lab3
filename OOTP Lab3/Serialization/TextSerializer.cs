using System;
using System.Globalization;
using System.Reflection;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Serialization
{
    /// <summary>
    /// Visitor that serializes employee objects to pipe-delimited text format
    /// Supports dynamic types from plugins
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

        /// <summary>
        /// Serialize any employee (including plugin types) using reflection
        /// </summary>
        public string SerializeDynamic(IEmployee employee)
        {
            var type = employee.GetType();
            var typeName = type.Name;

            // Try to use specific Visit method if available
            var method = this.GetType().GetMethod($"Visit", new[] { type });
            if (method != null)
            {
                return (string)method.Invoke(this, new object[] { employee });
            }

            // Fallback: use reflection for plugin types
            return SerializeViaReflection(employee, typeName);
        }

        private string SerializeViaReflection(IEmployee employee, string typeName)
        {
            var id = employee.Id;
            var name = employee.Name;
            var salary = Format(employee.Salary);
            var experience = employee.YearsOfExperience;

            // Get additional properties via reflection
            var props = employee.GetType().GetProperties();
            var additionalValues = new System.Collections.Generic.List<string>();

            foreach (var prop in props)
            {
                var propName = prop.Name;
                // Skip base properties
                if (propName == "Id" || propName == "Name" || propName == "Salary" ||
                    propName == "YearsOfExperience" || propName == "EmployeeType")
                    continue;

                var value = prop.GetValue(employee)?.ToString() ?? "";
                additionalValues.Add(value);
            }

            return $"{typeName}|{id}|{name}|{salary}|{experience}|{string.Join("|", additionalValues)}";
        }
    }
}