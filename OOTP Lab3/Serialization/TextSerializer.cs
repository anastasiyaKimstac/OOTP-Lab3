using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            var type = employee.GetType();

            // Пытаемся найти подходящий метод Visit для конкретного типа
            var method = this.GetType().GetMethod("Visit", new[] { type });
            if (method != null && method.DeclaringType == this.GetType())
            {
                try
                {
                    return (string)method.Invoke(this, new object[] { employee });
                }
                catch
                {
                    // Если не удалось, используем рефлексию
                }
            }

            // Универсальная сериализация для любых типов
            return SerializeViaReflection(employee);
        }

        private string SerializeViaReflection(IEmployee employee)
        {
            var parts = new List<string>();
            parts.Add(employee.GetType().Name);
            parts.Add(employee.Id.ToString());
            parts.Add(employee.Name);
            parts.Add(employee.Salary.ToString(CultureInfo.InvariantCulture));
            parts.Add(employee.YearsOfExperience.ToString());

            // Добавляем все дополнительные свойства
            var properties = employee.GetType().GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "Name" && p.Name != "Salary" &&
                           p.Name != "YearsOfExperience" && p.Name != "EmployeeType")
                .OrderBy(p => p.Name);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(employee)?.ToString() ?? "";
                parts.Add(value);
            }

            return string.Join("|", parts);
        }
    }
}