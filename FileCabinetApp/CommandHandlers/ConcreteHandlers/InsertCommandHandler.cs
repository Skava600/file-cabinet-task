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
    internal class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "insert";

        public InsertCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
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
            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();

            string[] dividedParams = parameters.Split(") values (");

            try
            {
                if (dividedParams.Length != 2)
                {
                    throw new ArgumentException("Invalid insert parameters");
                }

                Regex propertyNamesRegex = new Regex(@"(?:\w+)(?=(\s*,\s*)|$)");
                Regex propertyValuesRegex = new Regex(@"(?<=')(?:[\w\.\/]+)(?=('\s*,\s*)|('\s*\)\s*$))");

                var nameMatches = propertyNamesRegex.Matches(dividedParams[0]);
                var valuesMatches = propertyValuesRegex.Matches(dividedParams[1]);
                if (nameMatches.Count != fileCabinetRecordProperties.Length)
                {
                    throw new ArgumentException("Wrong property names parameter");
                }

                if (valuesMatches.Count != fileCabinetRecordProperties.Length)
                {
                    throw new ArgumentException("Wrong property values parameter");
                }

                FileCabinetRecord newRecord = new FileCabinetRecord();
                for (int i = 0; i < fileCabinetRecordProperties.Length; i++)
                {
                    var property = fileCabinetRecordProperties.FirstOrDefault(p => p.Name.Equals(nameMatches[i].Value, StringComparison.InvariantCultureIgnoreCase));
                    if (property == null)
                    {
                        throw new ArgumentException($"Wrong property name : {nameMatches[i].Value}");
                    }

                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    property.SetValue(newRecord, converter.ConvertFromInvariantString(valuesMatches[i].Value));
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