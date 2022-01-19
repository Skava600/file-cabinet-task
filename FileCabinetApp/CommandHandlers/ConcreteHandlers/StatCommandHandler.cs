using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.ConcreteHandlers
{
     /// <summary>
     /// Stat command handler.
     /// </summary>
    internal class StatCommandHandler : CommandHandlerBase
    {
        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("stat", StringComparison.InvariantCultureIgnoreCase))
            {
                Stat(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s), {recordsCount.Item2} deleted record(s).");
        }
    }
}
