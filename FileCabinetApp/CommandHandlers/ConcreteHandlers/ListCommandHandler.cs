using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.Entities;
using FileCabinetApp.Utils.Iterators;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// List command handler.
    /// </summary>
    internal class ListCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "list";
        private readonly Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        /// <param name="printer"> Record printer. </param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(service)
        {
            this.printer = printer;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.List(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void List(string parameters)
        {
            IEnumerable<FileCabinetRecord> iterator = this.FileCabinetService.GetRecords();

            this.printer(iterator);
        }
    }
}
