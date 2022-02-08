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
                    selectingProperties = parameters;
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
            IEnumerable<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();
            foreach (var property in properties)
            {
                foundRecords = foundRecords.Union(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
            }

            return foundRecords;
        }

        private IEnumerable<FileCabinetRecord> SearchRecordsByAndPropertiesCondition(IEnumerable<Tuple<PropertyInfo, string>> properties)
        {
            IEnumerable<FileCabinetRecord> foundRecords = this.FileCabinetService.GetRecords();
            foreach (var property in properties)
            {
                foundRecords = foundRecords.Intersect(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
            }

            return foundRecords;
        }
    }
}
