using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    internal class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "update";

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Update(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Update(string parameters)
        {
            Regex parametersRegex = new Regex(@"\s*set\s+(?<updatingProperties>.+)\s+where\s+(?<searchingProperties>.+)$", RegexOptions.IgnoreCase);

            var match = parametersRegex.Match(parameters);

            var updatingProperties = match.Groups["updatingProperties"].Value;
            var searchingProperties = match.Groups["searchingProperties"].Value;

            try
            {
                if (!match.Success)
                {
                    throw new ArgumentException("Wrong update parameters syntax");
                }

                var updatingPropertiesTuple = this.ParseProperties(updatingProperties, ",");
                var searchingPropertiesTuple = this.ParseProperties(searchingProperties, "and");

                var allRecords = this.FileCabinetService.GetRecords().ToList();
                foreach (var searchingProperty in searchingPropertiesTuple)
                {
                    var converter = TypeDescriptor.GetConverter(searchingProperty.Item1.PropertyType);
                    var searchingValue = converter.ConvertFromInvariantString(searchingProperty.Item2);
                    if (searchingValue == null)
                    {
                        throw new ArgumentException($"Invaild searching property value : {searchingProperty.Item2}");
                    }

                    switch (searchingProperty.Item1.PropertyType.Name)
                    {
                        case nameof(Int32):
                            allRecords.RemoveAll(record => !Convert.ToInt32(searchingProperty.Item1.GetValue(record)).Equals(Convert.ToInt32(searchingValue)));
                            break;
                        case nameof(String):
                            allRecords.RemoveAll(record => !Convert.ToString(searchingProperty.Item1.GetValue(record)) !.Equals(Convert.ToString(searchingValue), StringComparison.InvariantCultureIgnoreCase));
                            break;
                        case nameof(DateTime):
                            allRecords.RemoveAll(record => !Convert.ToDateTime(searchingProperty.Item1.GetValue(record)).Equals(Convert.ToDateTime(searchingValue)));
                            break;
                        case nameof(Char):
                            allRecords.RemoveAll(record => !Convert.ToChar(searchingProperty.Item1.GetValue(record)).Equals(char.ToUpper(Convert.ToChar(searchingValue))));
                            break;
                        case nameof(Int16):
                            allRecords.RemoveAll(record => !Convert.ToInt16(searchingProperty.Item1.GetValue(record)).Equals(Convert.ToInt16(searchingValue)));
                            break;
                        case nameof(Decimal):
                            allRecords.RemoveAll(record => !Convert.ToDecimal(searchingProperty.Item1.GetValue(record)).Equals(Convert.ToDecimal(searchingValue)));
                            break;
                        default: throw new ArgumentException("Something is went wrong");
                    }

                    if (allRecords.Count == 0)
                    {
                        break;
                    }
                }

                foreach (var record in allRecords)
                {
                    foreach (var updatingProperty in updatingPropertiesTuple)
                    {
                        if (updatingProperty.Item1.Name.Equals(nameof(record.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new ArgumentException("You can't change id of a record");
                        }

                        var converter = TypeDescriptor.GetConverter(updatingProperty.Item1.PropertyType);
                        var newValue = converter.ConvertFromInvariantString(updatingProperty.Item2);
                        updatingProperty.Item1.SetValue(record, newValue);
                    }

                    var recordData = new RecordData(record);
                    this.FileCabinetService.EditRecord(record.Id, recordData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Updating failed : {ex.Message}.");
            }
        }

        private IEnumerable<Tuple<PropertyInfo, string>> ParseProperties(string properties, string propertySeparator)
        {
            var splitedProperties = properties.Split(propertySeparator, StringSplitOptions.RemoveEmptyEntries);
            List<Tuple<PropertyInfo, string>> propertiesTuple = new List<Tuple<PropertyInfo, string>>();
            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();
            foreach (var property in splitedProperties)
            {
                var propertySplited = property.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                if (propertySplited.Length != 2)
                {
                    throw new ArgumentException($"Wrong updating property syntax : {property}");
                }

                const int propertyNameIndex = 0;
                const int propertyValueIndex = 1;
                propertySplited[propertyNameIndex] = propertySplited[propertyNameIndex].Trim();
                propertySplited[propertyValueIndex] = propertySplited[propertyValueIndex].Trim();

                if (!propertySplited[propertyValueIndex].StartsWith('\'') ||
                    !propertySplited[propertyValueIndex].EndsWith('\''))
                {
                    throw new ArgumentException("Property vaue should be in single quotes.");
                }

                propertySplited[propertyValueIndex] = propertySplited[propertyValueIndex][1..^1];

                var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(propertySplited[propertyNameIndex].Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"There is no such property as {propertySplited[propertyNameIndex]}");
                }

                propertiesTuple.Add(new Tuple<PropertyInfo, string>(propertyInfo, propertySplited[propertyValueIndex]));
            }

            return propertiesTuple;
        }
    }
}
