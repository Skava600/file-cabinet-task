using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Input;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Create command handler.
    /// </summary>
    internal class CreateCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                Create(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Create(string parameters)
        {
            try
            {
                RecordData recordData = RecordInputReader.GetRecordInput();
                Console.WriteLine($"Record #{Program.FileCabinetService.CreateRecord(recordData)} is created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}. Input data again.");
                Create(parameters);
            }
        }
    }
}