using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Entities;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    internal class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "delete";

        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Delete(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Delete(string parameters)
        {
            string[] splitedParameters = parameters.Split();
            Regex deleteParametersRegex = new Regex(@"^\s*where\s+(?<key>\w*)\s*=\s*'(?<value>[\w\/\.]+)'\s*$", RegexOptions.IgnoreCase);

            var match = deleteParametersRegex.Match(parameters);

            var propertyName = match.Groups["key"].Value;
            var propertyValue = match.Groups["value"].Value;

            PropertyInfo[] fileCabinetRecordProperties = typeof(FileCabinetRecord).GetProperties();

            var propertyInfo = fileCabinetRecordProperties.FirstOrDefault(prop => prop.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            try
            {
                if (!match.Success)
                {
                    throw new ArgumentException("Wrong parameters syntax");
                }

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"No such property with name: {propertyName}");
                }

                var deletedRecordsIds = this.FileCabinetService.DeleteRecord(propertyInfo, propertyValue);
                Console.Write("Records { ");
                foreach (var id in deletedRecordsIds)
                {
                    Console.Write($"#{id}, ");
                }

                Console.WriteLine("} are deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete operation failed: {ex.Message}.");
            }
        }
    }
}
