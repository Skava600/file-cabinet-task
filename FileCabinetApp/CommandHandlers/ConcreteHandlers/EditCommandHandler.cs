using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Input;
using FileCabinetApp.Validation;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Edit command handler.
    /// </summary>
    internal class EditCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "edit";
        private readonly string validationRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        /// <param name="validationRule"> Validation rule. </param>
        public EditCommandHandler(IFileCabinetService service, string validationRule)
            : base(service)
        {
            this.validationRule = validationRule;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Edit(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"Invalid input parameters. Should be integer but received '{parameters}'");
                return;
            }

            if (!this.FileCabinetService.IsRecordExists(id))
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            try
            {
                RecordData recordData = new RecordInputReader(this.validationRule).GetRecordInput();
                this.FileCabinetService.EditRecord(id, recordData);
                Console.WriteLine($"Record #{id} is updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
