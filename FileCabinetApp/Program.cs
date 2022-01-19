using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.ConcreteHandlers;
using FileCabinetApp.Converters;
using FileCabinetApp.Entities;
using FileCabinetApp.Models;
using FileCabinetApp.Services;
using FileCabinetApp.Utils.Enums;
using FileCabinetApp.Validation;

namespace FileCabinetApp
{
    /// <summary>
    /// The program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Vladislav Skovorodnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private const string FileStorageName = "cabinet-records.db";

        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private static IRecordValidator recordValidator = new DefaultValidator();

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var commandHandler = CreateCommandHandlers();
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            SetServiceBehaviour(args);

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var createCommandHandler = new CreateCommandHandler(fileCabinetService, recordValidator);
            var editCommandHandler = new EditCommandHandler(fileCabinetService, recordValidator);
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var listCommandHandler = new ListCommandHandler(fileCabinetService);
            var findCommandHandler = new FindCommandHandler(fileCabinetService);
            var removeCommandHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);

            Action<bool> exitApp = x => isRunning = x;
            var exitCommandHandler = new ExitCommandHandler(exitApp);

            helpCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(listCommandHandler);
            listCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(exitCommandHandler);

            return helpCommandHandler;
        }

        private static void SetServiceBehaviour(string[] args)
        {
            args = string.Join(' ', args).Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);

            ValidationRule systemValidationBehaviour = ValidationRule.Default;

            string memoryBehaviour = "default";
            for (int i = 0; i + 1 < args.Length; i++)
            {
                if (args[i].Equals("-v", StringComparison.InvariantCulture) ||
                    args[i].Equals("--validation-rules", StringComparison.InvariantCulture))
                {
                    systemValidationBehaviour = args[i + 1].Equals("CUSTOM", StringComparison.InvariantCultureIgnoreCase) ?
                            ValidationRule.Custom : ValidationRule.Default;
                    i += 1;
                }
                else if (args[i].Equals("-s", StringComparison.InvariantCulture) ||
                        args[i].Equals("--storage", StringComparison.InvariantCulture))
                {
                    memoryBehaviour = args[i + 1].ToLower();
                    i += 1;
                }
                else
                {
                    break;
                }
            }

            recordValidator = systemValidationBehaviour switch
            {
                ValidationRule.Custom => new CustomValidator(),
                _ => new DefaultValidator(),
            };

            fileCabinetService = memoryBehaviour switch
            {
                "file" => new FileCabinetFilesystemService(new FileStream(FileStorageName, FileMode.OpenOrCreate, FileAccess.ReadWrite), recordValidator),
                _ => new FileCabinetMemoryService(recordValidator),
            };

            Console.WriteLine($"Using {systemValidationBehaviour} validation rules.");
            Console.WriteLine($"Using {fileCabinetService.GetType().Name}.");
        }
    }
}