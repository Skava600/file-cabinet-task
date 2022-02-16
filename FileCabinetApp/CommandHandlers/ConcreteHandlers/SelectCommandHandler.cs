using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Utils.CommandHelper;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Select command handler.
    /// </summary>
    internal class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private const string Command = "select";
        private Action<IEnumerable<FileCabinetRecord>, IEnumerable<PropertyInfo>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService"> File cabinet service. </param>
        /// <param name="printer"> Record printer.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>, IEnumerable<PropertyInfo>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (Command.Equals(request.Command, StringComparison.OrdinalIgnoreCase))
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

            Match match = parametersRegex.Match(parameters);

            string selectingProperties = match.Groups["selectingProperties"].Value;
            string searchingProperties = match.Groups["searchingProperties"].Value;
            IEnumerable<FileCabinetRecord> foundRecords;
            try
            {
                if (!match.Success)
                {
                    selectingProperties = parameters;
                }

                string[] splitedSelectingProperties = selectingProperties.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
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

                if (!string.IsNullOrWhiteSpace(searchingProperties))
                {
                    IEnumerable<Tuple<PropertyInfo, string>> searchingPropertiesTuple = CommandParser.ParseSelectParameters(searchingProperties, out string separator);
                    const string andSeparator = "and";
                    const string orSeparator = "or";

                    if (separator.Equals(andSeparator, StringComparison.Ordinal))
                    {
                        foundRecords = this.FileCabinetService.GetRecords();
                        foreach (var property in searchingPropertiesTuple)
                        {
                            foundRecords = foundRecords.Intersect(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
                        }
                    }
                    else if (separator.Equals(orSeparator, StringComparison.Ordinal))
                    {
                        foundRecords = new List<FileCabinetRecord>();
                        foreach (var property in searchingPropertiesTuple)
                        {
                            foundRecords = foundRecords.Union(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Wrong separator : '{separator}'");
                    }
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
    }
}
