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
    internal class PurgeCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                Purge(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Purge(string parameters)
        {
            try
            {
                var records = Program.FileCabinetService.GetStat();
                Program.FileCabinetService.Purge();
                Console.WriteLine($"Data file processing is completed: {records.Item2} of {records.Item1} records were purged.");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Purge available only for file data stotage.");
            }
        }
    }
}
