using System;
using System.Globalization;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Serialization
{
    /// <summary>
    /// Deserializes pipe-delimited text back into employee objects
    /// </summary>
    public class TextDeserializer
    {
        private static decimal ParseDecimal(string value)
            => decimal.Parse(value, CultureInfo.InvariantCulture);

        private static Guid ParseGuid(string value)
            => Guid.Parse(value);

        public IEmployee Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be null or empty");

            var parts = data.Split('|');
            if (parts.Length < 5)
                throw new FormatException($"Invalid data format. Expected at least 5 parts, got {parts.Length}");

            return parts[0] switch
            {
                "Manager" => new Manager
                {
                    Id = ParseGuid(parts[1]),
                    Name = parts[2],
                    Salary = ParseDecimal(parts[3]),
                    YearsOfExperience = int.Parse(parts[4]),
                    TeamSize = int.Parse(parts[5]),
                    Department = parts[6]
                },
                "Developer" => new Developer
                {
                    Id = ParseGuid(parts[1]),
                    Name = parts[2],
                    Salary = ParseDecimal(parts[3]),
                    YearsOfExperience = int.Parse(parts[4]),
                    MainLanguage = parts[5],
                    ProjectsCompleted = int.Parse(parts[6])
                },
                "Designer" => new Designer
                {
                    Id = ParseGuid(parts[1]),
                    Name = parts[2],
                    Salary = ParseDecimal(parts[3]),
                    YearsOfExperience = int.Parse(parts[4]),
                    DesignTool = parts[5],
                    Specialization = parts[6]
                },
                "Tester" => new Tester
                {
                    Id = ParseGuid(parts[1]),
                    Name = parts[2],
                    Salary = ParseDecimal(parts[3]),
                    YearsOfExperience = int.Parse(parts[4]),
                    TestingTool = parts[5],
                    BugsFound = int.Parse(parts[6])
                },
                "Intern" => new Intern
                {
                    Id = ParseGuid(parts[1]),
                    Name = parts[2],
                    Salary = ParseDecimal(parts[3]),
                    YearsOfExperience = int.Parse(parts[4]),
                    University = parts[5],
                    Semester = int.Parse(parts[6])
                },
                _ => throw new NotSupportedException($"Unknown employee type: {parts[0]}")
            };
        }
    }
}