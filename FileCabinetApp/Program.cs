using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Vladislav Skovorodnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("exit", Exit),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "create", "creates a new record", "The 'create' command creates a record to the service." },
            new string[] { "edit", "edites record", "The 'edit' command edites existing record. Parameters - {id}" },
            new string[] { "list", "prints the array of records", "The 'list' command prints array of records." },
            new string[] { "find", "prints the array of records found by given property", "The 'find' command prints array of records by given property." },
            new string[] { "stat", "prints the count of records", "The 'stat' command prints count of the records in service." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        };

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

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
            GetRecordInput(
                out string? firstName,
                out string? lastName,
                out DateTime dateOfBirth,
                out char sex,
                out short height,
                out decimal salary);

            try
            {
                Console.WriteLine($"Record #{fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, sex, height, salary)} is created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}. Input data again.");
                Create(parameters);
            }
        }

        private static void Edit(string parameters)
        {
            int.TryParse(parameters, out int id);
            if (fileCabinetService.IsRecordExists(id))
            {
                Console.WriteLine($"#{id} record is not found.");
                return;
            }

            GetRecordInput(
                out string? firstName,
                out string? lastName,
                out DateTime dateOfBirth,
                out char sex,
                out short height,
                out decimal salary);

            try
            {
                fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, sex, height, salary);
                Console.WriteLine($"Record #{id} is updated.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}. Input data again.");
                Edit(parameters);
            }
        }

        private static void Find(string parameters)
        {
            string[] findParameters = parameters.Split(" ", 2);
            string property = findParameters[0];
            string value = findParameters[1].Trim('"');
            FileCabinetRecord[] foundRecords = Array.Empty<FileCabinetRecord>();
            if (property.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
            {
                foundRecords = fileCabinetService.FindByFirstName(value);
            }
            else if (property.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
            {
                foundRecords = fileCabinetService.FindByLastName(value);
            }
            else if (property.Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
            {
                foundRecords = fileCabinetService.FindByDateOfBirth(value);
            }
            else
            {
                Console.WriteLine("No such property");
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

            if (foundRecords.Length == 0)
            {
                Console.WriteLine("No records by given property.");
            }
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] records = fileCabinetService.GetRecords();

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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void GetRecordInput(
            out string? firstName,
            out string? lastName,
            out DateTime dateOfBirth,
            out char sex,
            out short height,
            out decimal salary)
        {
            Console.Write("First name: ");
            firstName = Console.ReadLine();

            Console.Write("Last name: ");
            lastName = Console.ReadLine();

            Console.Write("Date of birth: ");
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime.TryParse(
                Console.ReadLine(),
                culture,
                styles,
                out dateOfBirth);

            Console.Write("Sex (M or F): ");
            char.TryParse(Console.ReadLine(), out sex);

            Console.Write("Height: ");
            short.TryParse(Console.ReadLine(), out height);

            Console.Write("Salary ($): ");
            decimal.TryParse(Console.ReadLine(), out salary);
        }
    }
}