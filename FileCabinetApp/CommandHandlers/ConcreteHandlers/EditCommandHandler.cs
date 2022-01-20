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
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        /// <param name="validator"> Record validator. </param>
        public EditCommandHandler(IFileCabinetService service, IRecordValidator validator)
            : base(service)
        {
            this.recordValidator = validator;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
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
                RecordData recordData = new RecordInputReader().GetRecordInput();
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
