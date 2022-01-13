using System.Collections.ObjectModel;
using System.Globalization;
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
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const string FileStorageName = "cabinet-records.db";

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "create", "creates a new record", "The 'create' command creates a record to the service." },
            new string[] { "edit", "edites record", "The 'edit <id>' command edites existing record." },
            new string[] { "remove", "removes record", "THE 'remove <id>' command removes existing record." },
            new string[] { "list", "prints the array of records", "The 'list' command prints array of records." },
            new string[] { "find", "prints the array of records found by given property", "The 'find <parameter name> <parameter value>' command prints array of records by given property." },
            new string[] { "export", "exports service data into file .csv or .xml", "The 'export <format> <file path>' command exports service data into specified format." },
            new string[] { "import", "imports servcie data from file .csv or .xml", "The 'export <format> <file path>' command imports service data from file with specified format." },
            new string[] { "stat", "prints the count of records", "The 'stat' command prints count of the records in service." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private static IRecordValidator recordValidator = new DefaultValidator();

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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Create(string parameters)
        {
            try
            {
                RecordData recordData = GetRecordInput();
                Console.WriteLine($"Record #{fileCabinetService.CreateRecord(recordData)} is created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}. Input data again.");
                Create(parameters);
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"Invalid input parameters. Should be integer but received '{parameters}'");
                return;
            }

            if (!fileCabinetService.IsRecordExists(id))
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            try
            {
                RecordData recordData = GetRecordInput();
                fileCabinetService.EditRecord(id, recordData);
                Console.WriteLine($"Record #{id} is updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"Invalid input parameters. Should be integer but received '{parameters}'");
                return;
            }

            try
            {
                fileCabinetService.RemoveRecord(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Find(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> foundRecords;
            try
            {
                string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (inputs.Length < 2)
                {
                    Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                        $"Should be name of property and value through white space.");
                    return;
                }

                const int nameIndex = 0;
                string propertyName = inputs[nameIndex];

                const int valueIndex = 1;
                string propertyValue = inputs[valueIndex].Trim('"');

                if (propertyName.Equals(nameof(FileCabinetRecord.FirstName), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = fileCabinetService.FindByFirstName(propertyValue);
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.LastName), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = fileCabinetService.FindByLastName(propertyValue);
                }
                else if (propertyName.Equals(nameof(FileCabinetRecord.DateOfBirth), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundRecords = fileCabinetService.FindByDateOfBirth(propertyValue);
                }
                else
                {
                    throw new InvalidOperationException($"The {propertyName} isn't valid command searching property. Only " +
                        $"'{nameof(FileCabinetRecord.FirstName)}', '{nameof(FileCabinetRecord.LastName)}' and " +
                        $"'{nameof(FileCabinetRecord.DateOfBirth)}' allowed.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            foreach (var record in foundRecords)
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

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> records = fileCabinetService.GetRecords();

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

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Export(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputs.Length < 2)
            {
                Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                    $"Should be export format and file path through white space.");
                return;
            }

            const int formatIndex = 0;
            string format = inputs[formatIndex];

            const int pathIndex = 1;
            string filePath = inputs[pathIndex];

            if (File.Exists(filePath))
            {
                char answer;

                do
                {
                    Console.Write($"File is exist - rewrite {filePath}? [Y/n] ");
                    answer = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                }
                while (!char.ToLower(answer).Equals('y') && !char.ToLower(answer).Equals('n'));

                if (char.ToLower(answer).Equals('n'))
                {
                    return;
                }
            }

            try
            {
                if (format.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        fileCabinetService.MakeSnapshot().SaveToCsv(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else if (format.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        fileCabinetService.MakeSnapshot().SaveToXml(sw);
                        Console.WriteLine($"All records are exported to file {filePath}.");
                    }
                }
                else
                {
                    Console.WriteLine($"{format} is not correct format, available only xml and csv");
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"Export failed: can't open file {filePath}.");
            }
        }

        private static void Import(string parameters)
        {
            string[] inputs = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputs.Length < 2)
            {
                Console.WriteLine($"The '{parameters}' isn't valid command parameters. " +
                    $"Should be import format and file path through white space.");
                return;
            }

            const int formatIndex = 0;
            string format = inputs[formatIndex];

            const int pathIndex = 1;
            string filePath = inputs[pathIndex];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Import error: file {filePath} is not exist.");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                var snapshot = new FileCabinetServiceSnapshot(Array.Empty<FileCabinetRecord>());
                if (format.Equals("csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    snapshot.LoadFromCsv(reader);
                }
                else if (format.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    snapshot.LoadFromXml(reader);
                }
                else
                {
                    Console.WriteLine($"{format} is not correct format, available only xml and csv");
                }

                fileCabinetService.Restore(snapshot);
                Console.WriteLine($"{snapshot.Records.Count} were imported from {filePath}.");
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static RecordData GetRecordInput()
        {
            Func<string, Tuple<bool, string, string>> stringConverter = InputConverter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = InputConverter.DateTimeConverter;
            Func<string, Tuple<bool, string, char>> charConverter = InputConverter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = InputConverter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = InputConverter.DecimalConverter;

            Func<string, Tuple<bool, string>> firstNameValidator = recordValidator.FirstNameValidator;
            Func<string, Tuple<bool, string>> lastNameValidator = recordValidator.LastNameValidator;
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = recordValidator.DateOfBirthValidator;
            Func<char, Tuple<bool, string>> sexValidator = recordValidator.SexValidator;
            Func<short, Tuple<bool, string>> heightValidator = recordValidator.HeightValidator;
            Func<decimal, Tuple<bool, string>> salaryValidator = recordValidator.SalaryValidator;

            Console.Write("First name: ");
            var firstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            var lastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);

            Console.Write("Sex (M or F): ");
            var sex = ReadInput(charConverter, sexValidator);

            Console.Write("Height: ");
            var height = ReadInput(shortConverter, heightValidator);

            Console.Write("Salary ($): ");
            var salary = ReadInput(decimalConverter, salaryValidator);

            RecordData record = new RecordData(firstName, lastName, dateOfBirth, sex, height, salary);
            return record;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input!);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}