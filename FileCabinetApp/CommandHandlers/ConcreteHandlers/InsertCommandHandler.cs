using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Converters;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Insert command handler.
    /// </summary>
    internal class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "insert";

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Insert(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Insert(string parameters)
        {
            List<PropertyInfo> fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties().ToList();

            Regex insertParametersRegex = new Regex(@"^\s*\((?<propertyNames>.+)\)\s+values\s+\((?<propertyValues>.+)\)\s*$");
            Match match = insertParametersRegex.Match(parameters);

            string propertyNames = match.Groups["propertyNames"].Value;
            string propertyValues = match.Groups["propertyValues"].Value;

            try
            {
                if (!match.Success)
                {
                    throw new ArgumentException("Insert parameters are wrong");
                }

                Regex propertyNamesRegex = new Regex(@"(?:\w+)(?=(\s*,\s*)|$)");
                Regex propertyValuesRegex = new Regex(@"(?<=')(?:[\w\.\/\s]+)(?=('\s*,\s*)|('\s*\s*$))");

                var nameMatches = propertyNamesRegex.Matches(propertyNames);
                var valuesMatches = propertyValuesRegex.Matches(propertyValues);
                if (nameMatches.Count != fileCabinetRecordProperties.Count)
                {
                    throw new ArgumentException("Should be all properties presented");
                }

                if (valuesMatches.Count != fileCabinetRecordProperties.Count)
                {
                    throw new ArgumentException("Should be all property values presented");
                }

                FileCabinetRecord newRecord = new FileCabinetRecord();
                List<PropertyInfo> usedFileCabinetRecordProperties = new List<PropertyInfo>();
                for (int i = 0; i < fileCabinetRecordProperties.Count; i++)
                {
                    PropertyInfo? property = fileCabinetRecordProperties.FirstOrDefault(p => p.Name.Equals(nameMatches[i].Value, StringComparison.OrdinalIgnoreCase));
                    if (property == null)
                    {
                        throw new ArgumentException($"Wrong property name : {nameMatches[i].Value}");
                    }

                    usedFileCabinetRecordProperties.Add(property);
                    TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                    property.SetValue(newRecord, converter.ConvertFromInvariantString(valuesMatches[i].Value));
                }

                if (usedFileCabinetRecordProperties.Intersect(fileCabinetRecordProperties).Count() != fileCabinetRecordProperties.Count)
                {
                    throw new ArgumentException($"Some of properties are repeating: {propertyNames}");
                }

                RecordData recordData = new RecordData(newRecord);

                this.FileCabinetService.CreateRecordWithId(newRecord.Id, recordData);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Insert failed. {ex.Message}.");
            }
        }
    }
}