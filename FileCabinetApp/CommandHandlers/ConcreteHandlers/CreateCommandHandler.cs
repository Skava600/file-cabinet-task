﻿using System;
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
    /// Create command handler.
    /// </summary>
    internal class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service"> File cabinet service. </param>
        /// <param name="validator"> Record validator. </param>
        public CreateCommandHandler(IFileCabinetService service, IRecordValidator validator)
        {
            this.fileCabinetService = service;
            this.recordValidator = validator;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest request)
        {
            if (request.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create(request.Parameters);
            }
            else
            {
                base.Handle(request);
            }
        }

        private void Create(string parameters)
        {
            try
            {
                RecordData recordData = new RecordInputReader(this.recordValidator).GetRecordInput();
                Console.WriteLine($"Record #{this.fileCabinetService.CreateRecord(recordData)} is created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}. Input data again.");
                this.Create(parameters);
            }
        }
    }
}