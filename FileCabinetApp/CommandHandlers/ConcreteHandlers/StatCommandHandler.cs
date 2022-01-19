﻿using System;
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
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        public StatCommandHandler(IFileCabinetService service)
        {
            this.fileCabinetService = service;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("stat", StringComparison.InvariantCultureIgnoreCase))
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
            var recordsCount = this.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s), {recordsCount.Item2} deleted record(s).");
        }
    }
}
