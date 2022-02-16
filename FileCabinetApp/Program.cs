using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
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
            ["--validation-rules"] = (string validationRules) => Program.validationRules = validationRules.ToLowerInvariant(),
            ["-v"] = (string validationRules) => Program.validationRules = validationRules.ToLowerInvariant(),
            ["--storage"] = (string storage) => Program.storage = storage.ToLowerInvariant(),
            ["-s"] = (string storage) => Program.storage = storage.ToLowerInvariant(),
            ["--use-stopwatch"] = (string str) => Program.isUsingTimewatch = true,
            ["--use-logger"] = (string str) => Program.isUsingLogger = true,
        };

        private static bool isRunning = true;
        private static IRecordValidator recordValidator = ValidatorBuilder.CreateDefault();
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(recordValidator);
        private static string validationRules = "default";
        private static string storage = "memory";
        private static bool isUsingTimewatch;
        private static bool isUsingLogger;

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
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            var insertCommandHandler = new InsertCommandHandler(fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            var updateCommandHandler = new UpdateCommandHandler(fileCabinetService);
            var selectCommandHandler = new SelectCommandHandler(fileCabinetService, DefaultRecordPrint);

            Action<bool> exitApp = x => isRunning = x;
            var exitCommandHandler = new ExitCommandHandler(exitApp);

            helpCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(selectCommandHandler);

            return helpCommandHandler;
        }

        private static void SetServiceBehaviour(string[] args)
        {
            string paramName;
            string paramValue;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--", StringComparison.Ordinal))
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
                case "custom": recordValidator = ValidatorBuilder.CreateCustom();
                    break;
                case "default": recordValidator = ValidatorBuilder.CreateDefault();
                    break;
                default: recordValidator = ValidatorBuilder.CreateDefault();
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
                    Program.storage = "memory";
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

            Console.WriteLine($"Using {Program.validationRules.ToLowerInvariant()} validation rules.");
            Console.WriteLine($"Using {Program.storage.ToLowerInvariant()} storage.");
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records, IEnumerable<PropertyInfo> propertyInfos)
        {
            List<int> columnLengths = new List<int>();
            StringBuilder rowSeparator = new StringBuilder();
            foreach (var propertyInfo in propertyInfos)
            {
                int maxLengthValue = records.Max(record =>
                {
                    var value = propertyInfo.GetValue(record);
                    if (value == null)
                    {
                        return 0;
                    }

                    string? stringValue;
                    if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                    {
                        stringValue = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MMM-dd}", value);
                    }
                    else
                    {
                        stringValue = value.ToString();
                    }

                    return stringValue == null ? 0 : stringValue.Length;
                });

                maxLengthValue = maxLengthValue < propertyInfo.Name.Length ? propertyInfo.Name.Length : maxLengthValue;
                columnLengths.Add(maxLengthValue);
                rowSeparator.Append("+" + new string('-', maxLengthValue + 2));
            }

            rowSeparator.Append('+');
            Console.WriteLine(rowSeparator.ToString());

            int index = 0;
            foreach (var propertyInfo in propertyInfos)
            {
                var column = "| " + propertyInfo.Name + new string(' ', columnLengths[index] - propertyInfo.Name.Length + 1);
                Console.Write(column);
                index++;
            }

            Console.WriteLine('|');

            foreach (var record in records)
            {
                Console.WriteLine(rowSeparator.ToString());
                index = 0;
                foreach (var propertyInfo in propertyInfos)
                {
                    var value = propertyInfo.GetValue(record);

                    string? stringValue;
                    if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                    {
                        stringValue = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MMM-dd}", value);
                    }
                    else
                    {
                        stringValue = value == null ? string.Empty : value.ToString();
                    }

                    stringValue = stringValue ?? string.Empty;
                    var column = "| " + stringValue + new string(' ', columnLengths[index] - stringValue.Length + 1);
                    Console.Write(column);
                    index++;
                }

                Console.WriteLine('|');
            }

            Console.WriteLine(rowSeparator.ToString());
        }
    }
}