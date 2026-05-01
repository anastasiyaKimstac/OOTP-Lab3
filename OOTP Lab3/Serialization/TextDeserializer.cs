using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private static readonly Dictionary<string, Func<string[], IEmployee>> _deserializers = new();
        private static bool _scannedForPlugins = false;

        private static decimal ParseDecimal(string value)
            => decimal.Parse(value, CultureInfo.InvariantCulture);

        private static Guid ParseGuid(string value)
            => Guid.Parse(value);

        /// <summary>
        /// Register a deserializer for a specific type
        /// </summary>
        public void RegisterDeserializer(string typeName, Func<string[], IEmployee> deserializer)
        {
            if (!_deserializers.ContainsKey(typeName))
            {
                _deserializers[typeName] = deserializer;
            }
        }

        /// <summary>
        /// Scan all loaded assemblies for static Deserialize methods
        /// </summary>
        public void ScanAndRegisterPluginDeserializers()
        {
            if (_scannedForPlugins) return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        // Ищем статические методы Deserialize
                        var method = type.GetMethod("Deserialize",
                            BindingFlags.Public | BindingFlags.Static,
                            null, new[] { typeof(string[]) }, null);

                        if (method != null && method.ReturnType == typeof(IEmployee))
                        {
                            try
                            {
                                var deserializer = (Func<string[], IEmployee>)Delegate.CreateDelegate(
                                    typeof(Func<string[], IEmployee>), method);
                                RegisterDeserializer(type.Name, deserializer);
                                System.Diagnostics.Debug.WriteLine($"Registered deserializer for {type.Name}");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to register deserializer for {type.Name}: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to scan assembly: {ex.Message}");
                }
            }

            _scannedForPlugins = true;
        }

        public IEmployee Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Data cannot be null or empty");

            var parts = data.Split('|');
            if (parts.Length < 5)
                throw new FormatException($"Invalid data format. Expected at least 5 parts, got {parts.Length}");

            var typeName = parts[0];

            // Сначала проверяем базовые типы
            var baseEmployee = DeserializeBaseEmployee(typeName, parts);
            if (baseEmployee != null) return baseEmployee;

            // Затем плагины через зарегистрированные десериализаторы
            if (_deserializers.TryGetValue(typeName, out var deserializer))
            {
                try
                {
                    return deserializer(parts);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to deserialize using registered deserializer: {ex.Message}");
                }
            }

            // Универсальная десериализация через рефлексию
            return DeserializeViaReflection(parts);
        }

        private IEmployee DeserializeBaseEmployee(string typeName, string[] parts)
        {
            return typeName switch
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
                _ => null
            };
        }

        private IEmployee DeserializeViaReflection(string[] parts)
        {
            if (parts.Length < 5)
                throw new FormatException("Invalid data format for plugin type");

            var typeName = parts[0];

            // Ищем тип во всех загруженных сборках
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return new Type[0]; }
                })
                .FirstOrDefault(t => t.Name == typeName && typeof(IEmployee).IsAssignableFrom(t) && !t.IsAbstract);

            if (type == null)
                throw new NotSupportedException($"Unknown employee type: {typeName}");

            try
            {
                var instance = (IEmployee)Activator.CreateInstance(type);

                // Установка базовых свойств
                instance.Id = ParseGuid(parts[1]);
                instance.Name = parts[2];
                instance.Salary = ParseDecimal(parts[3]);
                instance.YearsOfExperience = int.Parse(parts[4]);

                // Установка дополнительных свойств
                var properties = type.GetProperties()
                    .Where(p => p.Name != "Id" && p.Name != "Name" && p.Name != "Salary" &&
                               p.Name != "YearsOfExperience" && p.Name != "EmployeeType")
                    .OrderBy(p => p.Name)
                    .ToList();

                for (int i = 0; i < properties.Count && i + 5 < parts.Length; i++)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(parts[i + 5]))
                        {
                            var converted = Convert.ChangeType(parts[i + 5], properties[i].PropertyType);
                            properties[i].SetValue(instance, converted);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to set property {properties[i].Name}: {ex.Message}");
                    }
                }

                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to deserialize {typeName}: {ex.Message}", ex);
            }
        }
    }
}