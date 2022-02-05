using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.ConcreteHandlers;
using FileCabinetApp.Entities;
using FileCabinetApp.Services;
using FileCabinetApp.Utils.Iterators;
using FileCabinetApp.Validation;
using Microsoft.Extensions.Configuration;

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

        private static readonly Dictionary<string, Action<string>> CommandParameters = new Dictionary<string, Action<string>>
        {
            ["--validation-rules"] = (string validationRules) => Program.validationRules = validationRules.ToLower(),
            ["-v"] = (string validationRules) => Program.validationRules = validationRules.ToLower(),
            ["--storage"] = (string storage) => Program.storage = storage.ToLower(),
            ["-s"] = (string storage) => Program.storage = storage.ToLower(),
            ["--use-stopwatch"] = (string str) => Program.isUsingTimewatch = true,
            ["--use-logger"] = (string str) => Program.isUsingLogger = true,
        };

        private static bool isRunning = true;
        private static IRecordValidator recordValidator = new ValidatorBuilder().CreateDefault();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(recordValidator);
        private static string validationRules = "default";
        private static string storage = "memory";
        private static bool isUsingTimewatch = false;
        private static bool isUsingLogger = false;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            SetServiceBehaviour(args);
            var commandHandler = CreateCommandHandlers();

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
            var createCommandHandler = new CreateCommandHandler(fileCabinetService, Program.validationRules);
            var editCommandHandler = new EditCommandHandler(fileCabinetService, Program.validationRules);
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var listCommandHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrint);
            var findCommandHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrint);
            var removeCommandHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            var insertCommandHandler = new InsertCommandHandler(fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            var updateCommandHandler = new UpdateCommandHandler(fileCabinetService);

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
            exitCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(updateCommandHandler);

            return helpCommandHandler;
        }

        private static void SetServiceBehaviour(string[] args)
        {
            string paramName;
            string paramValue;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string[] param = args[i].Split('=', 2);
                    const int paramIndex = 0;
                    const int paramValueIndex = 1;
                    if (param.Length < 2)
                    {
                        paramName = param[paramIndex];
                        paramValue = string.Empty;
                    }
                    else
                    {
                        paramName = param[paramIndex];
                        paramValue = param[paramValueIndex];
                    }
                }
                else if (args[i].StartsWith('-') && i + 1 < args.Length)
                {
                    paramName = args[i];
                    paramValue = args[i + 1];
                    i++;
                }
                else
                {
                    continue;
                }

                Action<string>? setParameter;
                if (CommandParameters.TryGetValue(paramName, out setParameter))
                {
                    setParameter(paramValue);
                }
                else
                {
                    Console.WriteLine($"error: unknown parameter '{paramName}'");
                    return;
                }
            }

            switch (Program.validationRules)
            {
                case "custom": recordValidator = new ValidatorBuilder().CreateCustom();
                    break;
                case "default": recordValidator = new ValidatorBuilder().CreateDefault();
                    break;
                default: recordValidator = new ValidatorBuilder().CreateDefault();
                    Program.validationRules = "default";
                    break;
            }

            switch (Program.storage)
            {
                case "file":
                    fileCabinetService = new FileCabinetFilesystemService(new FileStream(FileStorageName, FileMode.OpenOrCreate, FileAccess.ReadWrite), recordValidator);
                    break;
                case "memory":
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                    break;
                default:
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                    Program.validationRules = "memory";
                    break;
            }

            if (Program.isUsingTimewatch)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
                Console.WriteLine("Using time watcher for service.");
            }

            if (Program.isUsingLogger)
            {
                fileCabinetService = new ServiceLogger(fileCabinetService);
                Console.WriteLine("Using service logger.");
            }

            Console.WriteLine($"Using {Program.validationRules.ToLower()} validation rules.");
            Console.WriteLine($"Using {Program.storage.ToLower()} storage.");
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                string date = record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture);
                Console.WriteLine($"#{record.Id}, " +
                    $"{record.FirstName}, " +
                    $"{record.LastName}, " +
                    $"{date}, {record.Sex}, " +
                    $"{record.Height}, " +
                    $"{record.Salary}");
            }
        }
    }
}