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
    internal class StatCommandHandler : ServiceCommandHandlerBase
    {
        private static readonly string Command = "stat";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        public StatCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals(Command, StringComparison.OrdinalIgnoreCase))
            {
                this.Stat(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Stat(string parameters)
        {
            Tuple<int, int> recordsCount = this.FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s), {recordsCount.Item2} deleted record(s).");
        }
    }
}
