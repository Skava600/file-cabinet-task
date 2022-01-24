using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
    /// <summary>
    /// Purge command handler.
    /// </summary>
    internal class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "purge";

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Purge(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Purge(string parameters)
        {
            try
            {
                var records = this.FileCabinetService.GetStat();
                this.FileCabinetService.Purge();
                Console.WriteLine($"Data file processing is completed: {records.Item2} of {records.Item1} records were purged.");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Purge available only for file data stotage.");
            }
        }
    }
}
