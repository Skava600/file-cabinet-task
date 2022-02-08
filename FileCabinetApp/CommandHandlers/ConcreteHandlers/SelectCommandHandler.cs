using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    internal class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";
        private Action<IEnumerable<FileCabinetRecord>, IEnumerable<PropertyInfo>> printer;

        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>, IEnumerable<PropertyInfo>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
        }

        public override void Handle(AppCommandRequest request)
        {
            if (Command.Equals(request.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Select(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Select(string parameters)
        {
            Regex parametersRegex = new Regex(@"^\s*(?<selectingProperties>.+)(?:\s+where\s+(?<searchingProperties>.+))", RegexOptions.IgnoreCase);

            var match = parametersRegex.Match(parameters);

            var selectingProperties = match.Groups["selectingProperties"].Value;
            var searchingProperties = match.Groups["searchingProperties"].Value;
            IEnumerable<FileCabinetRecord> foundRecords;
            try
            {
                if (!match.Success)
                {
                    throw new ArgumentException("Wrong update parameters syntax");
                }

                var splitedSelectingProperties = selectingProperties.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                List<PropertyInfo> selectingPropertiesInfo = new List<PropertyInfo>();
                PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
                foreach (var splitedSelectingProperty in splitedSelectingProperties)
                {
                    var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(splitedSelectingProperty, StringComparison.OrdinalIgnoreCase));
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException($"{splitedSelectingProperty} not valid property name");
                    }

                    selectingPropertiesInfo.Add(propertyInfo);
                }

                string separator;
                if (!string.IsNullOrWhiteSpace(searchingProperties))
                {
                    var searchingPropertiesTuple = ParseProperties(searchingProperties, out separator);
                    foundRecords = separator switch
                    {
                        "and" => this.SearchRecordsByAndPropertiesCondition(searchingPropertiesTuple),
                        "or" => this.SearchRecordsByOrPropertiesCondition(searchingPropertiesTuple),
                        _ => throw new ArgumentException($"Wrong separator : '{separator}'")
                    };
                }
                else
                {
                    foundRecords = this.FileCabinetService.GetRecords();
                }

                this.printer(foundRecords, selectingPropertiesInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Selecting failed : {ex.Message}.");
            }
        }

        private static IEnumerable<Tuple<PropertyInfo, string>> ParseProperties(string properties, out string separator)
        {
            const string andSeparator = " and ";
            const string orSeparator = " or ";

            separator = andSeparator;
            var splitedProperties = properties.Split(andSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (splitedProperties.Length == 1)
            {
                splitedProperties = properties.Split(orSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                separator = orSeparator;
            }
            else
            {
                var splitedProperitesByOr = properties.Split(orSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (splitedProperitesByOr.Length != 1)
                {
                    throw new ArgumentException("You should use only one operator type: 'and' or 'or'");
                }
            }

            if (splitedProperties.Length == 1)
            {
                separator = andSeparator;
            }

            List<Tuple<PropertyInfo, string>> propertiesTuple = new List<Tuple<PropertyInfo, string>>();
            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
            foreach (var property in splitedProperties)
            {
                var propertySplited = property.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (propertySplited.Length != 2)
                {
                    throw new ArgumentException($"Wrong updating property syntax : {property}");
                }

                const int propertyNameIndex = 0;
                const int propertyValueIndex = 1;

                if (!propertySplited[propertyValueIndex].StartsWith('\'') ||
                    !propertySplited[propertyValueIndex].EndsWith('\''))
                {
                    throw new ArgumentException("Property vaue should be in single quotes.");
                }

                propertySplited[propertyValueIndex] = propertySplited[propertyValueIndex][1..^1];

                var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(propertySplited[propertyNameIndex], StringComparison.InvariantCultureIgnoreCase));

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"There is no such property as {propertySplited[propertyNameIndex]}");
                }

                propertiesTuple.Add(new Tuple<PropertyInfo, string>(propertyInfo, propertySplited[propertyValueIndex]));
            }

            separator = separator.Trim();
            return propertiesTuple;
        }

        private IEnumerable<FileCabinetRecord> SearchRecordsByOrPropertiesCondition(IEnumerable<Tuple<PropertyInfo, string>> properties)
        {
            IEnumerable<FileCabinetRecord> allRecords = this.FileCabinetService.GetRecords();
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();
            foreach (var property in properties)
            {
                var converter = TypeDescriptor.GetConverter(property.Item1.PropertyType);
                var searchingValue = converter.ConvertFromInvariantString(property.Item2);
                if (searchingValue == null)
                {
                    throw new ArgumentException($"Invaild searching property value : {property.Item2}");
                }

                switch (property.Item1.PropertyType.Name)
                {
                    case nameof(Int32):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToInt32(property.Item1.GetValue(record)).Equals(Convert.ToInt32(searchingValue))));
                        break;
                    case nameof(String):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToString(property.Item1.GetValue(record))!.Equals(Convert.ToString(searchingValue), StringComparison.InvariantCultureIgnoreCase)));
                        break;
                    case nameof(DateTime):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToDateTime(property.Item1.GetValue(record)).Equals(Convert.ToDateTime(searchingValue))));
                        break;
                    case nameof(Char):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToChar(property.Item1.GetValue(record)).Equals(char.ToUpper(Convert.ToChar(searchingValue)))));
                        break;
                    case nameof(Int16):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToInt16(property.Item1.GetValue(record)).Equals(Convert.ToInt16(searchingValue))));
                        break;
                    case nameof(Decimal):
                        foundRecords.AddRange(allRecords.Where(record => Convert.ToDecimal(property.Item1.GetValue(record)).Equals(Convert.ToDecimal(searchingValue))));
                        break;
                    default: throw new ArgumentException("Something is went wrong");
                }
            }

            return foundRecords;
        }

        private IEnumerable<FileCabinetRecord> SearchRecordsByAndPropertiesCondition(IEnumerable<Tuple<PropertyInfo, string>> properties)
        {
            var allRecords = this.FileCabinetService.GetRecords().ToList();
            foreach (var property in properties)
            {
                var converter = TypeDescriptor.GetConverter(property.Item1.PropertyType);
                var searchingValue = converter.ConvertFromInvariantString(property.Item2);
                if (searchingValue == null)
                {
                    throw new ArgumentException($"Invaild searching property value : {property.Item2}");
                }

                switch (property.Item1.PropertyType.Name)
                {
                    case nameof(Int32):
                        allRecords.RemoveAll(record => !Convert.ToInt32(property.Item1.GetValue(record)).Equals(Convert.ToInt32(searchingValue)));
                        break;
                    case nameof(String):
                        allRecords.RemoveAll(record => !Convert.ToString(property.Item1.GetValue(record))!.Equals(Convert.ToString(searchingValue), StringComparison.InvariantCultureIgnoreCase));
                        break;
                    case nameof(DateTime):
                        allRecords.RemoveAll(record => !Convert.ToDateTime(property.Item1.GetValue(record)).Equals(Convert.ToDateTime(searchingValue)));
                        break;
                    case nameof(Char):
                        allRecords.RemoveAll(record => !Convert.ToChar(property.Item1.GetValue(record)).Equals(char.ToUpper(Convert.ToChar(searchingValue))));
                        break;
                    case nameof(Int16):
                        allRecords.RemoveAll(record => !Convert.ToInt16(property.Item1.GetValue(record)).Equals(Convert.ToInt16(searchingValue)));
                        break;
                    case nameof(Decimal):
                        allRecords.RemoveAll(record => !Convert.ToDecimal(property.Item1.GetValue(record)).Equals(Convert.ToDecimal(searchingValue)));
                        break;
                    default: throw new ArgumentException("Something is went wrong");
                }

                if (allRecords.Count == 0)
                {
                    break;
                }
            }

            return allRecords;
        }
    }
}
