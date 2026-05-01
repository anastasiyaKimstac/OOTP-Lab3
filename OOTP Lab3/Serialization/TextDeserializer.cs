using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Serialization
{
    /// <summary>
    /// Deserializes pipe-delimited text back into employee objects
    /// Supports dynamic types from plugins
    /// </summary>
    public class TextDeserializer
    {
        private static decimal ParseDecimal(string value)
            => decimal.Parse(value, CultureInfo.InvariantCulture);

        private static Guid ParseGuid(string value)
            => Guid.Parse(value);

        // Registry for plugin deserializers
        private static Dictionary<string, Func<string[], IEmployee>> _pluginDeserializers
            = new Dictionary<string, Func<string[], IEmployee>>();

        /// <summary>
        /// Register a deserializer for a plugin type
        /// </summary>
        public static void RegisterPluginDeserializer(string typeName, Func<string[], IEmployee> deserializer)
        {
            _pluginDeserializers[typeName] = deserializer;
        }

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
                _ => DeserializePlugin(parts)
            };
        }

        private IEmployee DeserializePlugin(string[] parts)
        {
            var typeName = parts[0];

            // Check if we have a registered deserializer
            if (_pluginDeserializers.TryGetValue(typeName, out var deserializer))
            {
                return deserializer(parts);
            }

            // Try to create instance via reflection
            return DeserializeViaReflection(parts, typeName);
        }

        private IEmployee DeserializeViaReflection(string[] parts, string typeName)
        {
            try
            {
                // Try to find the type in loaded assemblies
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetType($"SeniorDeveloperPlugin.{typeName}");
                    if (type != null)
                    {
                        var instance = (IEmployee)Activator.CreateInstance(type);

                        // Set properties via reflection
                        instance.Id = ParseGuid(parts[1]);
                        instance.Name = parts[2];
                        instance.Salary = ParseDecimal(parts[3]);
                        instance.YearsOfExperience = int.Parse(parts[4]);

                        // Set additional properties
                        var properties = type.GetProperties();
                        int propIndex = 5;
                        foreach (var prop in properties)
                        {
                            if (prop.Name == "Id" || prop.Name == "Name" || prop.Name == "Salary" ||
                                prop.Name == "YearsOfExperience" || prop.Name == "EmployeeType")
                                continue;

                            if (propIndex < parts.Length)
                            {
                                var value = Convert.ChangeType(parts[propIndex], prop.PropertyType);
                                prop.SetValue(instance, value);
                                propIndex++;
                            }
                        }

                        return instance;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to deserialize plugin type {typeName}: {ex.Message}");
            }

            throw new NotSupportedException($"Unknown employee type: {typeName}");
        }
    }
}