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

            var match = parametersRegex.Match(parameters);

            var selectingProperties = match.Groups["selectingProperties"].Value;
            var searchingProperties = match.Groups["searchingProperties"].Value;
            IEnumerable<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();
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

                if (!string.IsNullOrWhiteSpace(searchingProperties))
                {
                    var searchingPropertiesTuple = CommandParser.ParseSelectParameters(searchingProperties, out string separator);
                    Func<IEnumerable<FileCabinetRecord>, IEnumerable<FileCabinetRecord>> combineFoundRecords;
                    const string andSeparator = "and";
                    const string orSeparator = "or";

                    switch (separator)
                    {
                        case andSeparator:
                            combineFoundRecords = foundRecords.Intersect;
                            foundRecords = this.FileCabinetService.GetRecords();
                            break;
                        case orSeparator:
                            combineFoundRecords = foundRecords.Union;
                            foundRecords = new List<FileCabinetRecord>();
                            break;
                        default:
                            throw new ArgumentException($"Wrong separator : '{separator}'");
                    }

                    foreach (var property in searchingPropertiesTuple)
                    {
                        foundRecords = combineFoundRecords(this.FileCabinetService.FindByProperty(property.Item1, property.Item2));
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
