using System.Globalization;
using FileCabinetApp.Converters;
using FileCabinetApp.Models;
using FileCabinetApp.Utils.Config;
using FileCabinetApp.Validation;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Utils.Input
{
    /// <summary>
    /// Record input reader.
    /// </summary>
    public class RecordInputReader
    {
        private readonly ValidationConfigReader configReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordInputReader"/> class.
        /// </summary>
        /// <param name="validationRules"> validation rules: default or custom. </param>
        public RecordInputReader(string validationRules)
        {
            this.configReader = new ValidationConfigReader(validationRules);
        }

        /// <summary>
        /// Reads users record input.
        /// </summary>
        /// <returns> Data for FileCabinetRecord. </returns>
        public RecordData GetRecordInput()
        {
            RecordData record;

            Func<string, Tuple<bool, string, string>> stringConverter = InputConverter.StringConverter;
            Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = InputConverter.DateTimeConverter;
            Func<string, Tuple<bool, string, char>> charConverter = InputConverter.CharConverter;
            Func<string, Tuple<bool, string, short>> shortConverter = InputConverter.ShortConverter;
            Func<string, Tuple<bool, string, decimal>> decimalConverter = InputConverter.DecimalConverter;

            Tuple<int, int> firstNameRules = this.configReader.ReadFirstNameRules();
            Tuple<int, int> lastNameRules = this.configReader.ReadLastNameRules();
            Tuple<DateTime, DateTime> dateOfBirthRules = this.configReader.ReadDateOfBirthRules();
            Tuple<short, short> heightRules = this.configReader.ReadHeightRules();
            char[] sexRules = this.configReader.ReadSexRules();
            Tuple<decimal, decimal> salaryRules = this.configReader.ReadSalaryRules();

            Func<string, Tuple<bool, string>> firstNameValidator =
                name => name.Length < firstNameRules.Item1 || name.Length > firstNameRules.Item2 ?
                new Tuple<bool, string>(false, $"Length of first name must be between {firstNameRules.Item1} and {firstNameRules.Item2}") :
                new Tuple<bool, string>(true, nameof(record.FirstName));
            Func<string, Tuple<bool, string>> lastNameValidator =
                name => name.Length < lastNameRules.Item1 || name.Length > lastNameRules.Item2 ?
                new Tuple<bool, string>(false, $"Length of last name must be between {lastNameRules.Item1} and {lastNameRules.Item2}") :
                new Tuple<bool, string>(true, nameof(record.LastName));
            Func<DateTime, Tuple<bool, string>> dateOfBirthValidator =
                dateOfBirth => dateOfBirth < dateOfBirthRules.Item1 || dateOfBirth > dateOfBirthRules.Item2 ?
                new Tuple<bool, string>(false, $"Date of birth current must be between {dateOfBirthRules.Item1.ToString("d", CultureInfo.InvariantCulture)} and {dateOfBirthRules.Item2.ToString("d", CultureInfo.InvariantCulture)}") :
                new Tuple<bool, string>(true, nameof(record.DateOfBirth));
            Func<char, Tuple<bool, string>> sexValidator =
                sex => !Array.Exists(sexRules, availableSex => char.ToUpperInvariant(sex).Equals(char.ToUpperInvariant(availableSex))) ?
                new Tuple<bool, string>(false, "Not valid sex") :
                new Tuple<bool, string>(true, nameof(record.Sex));
            Func<short, Tuple<bool, string>> heightValidator =
                height => height < heightRules.Item1 || height > heightRules.Item2 ?
                new Tuple<bool, string>(false, $"height must be a number between {heightRules.Item1}  and {heightRules.Item2}") :
                new Tuple<bool, string>(true, nameof(record.Height));
            Func<decimal, Tuple<bool, string>> salaryValidator =
                salary => salary < salaryRules.Item1 || salary > salaryRules.Item2 ?
                new Tuple<bool, string>(false, $"Salary should be between {salaryRules.Item1} and {salaryRules.Item2}.") :
                new Tuple<bool, string>(true, nameof(record.Salary));

            Console.Write("First name: ");
            var firstName = ReadInput(stringConverter, firstNameValidator);

            Console.Write("Last name: ");
            var lastName = ReadInput(stringConverter, lastNameValidator);

            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);

            Console.Write("Sex: ");
            var sex = ReadInput(charConverter, sexValidator);

            Console.Write("Height: ");
            var height = ReadInput(shortConverter, heightValidator);

            Console.Write("Salary ($): ");
            var salary = ReadInput(decimalConverter, salaryValidator);

            record = new RecordData(firstName, lastName, dateOfBirth, sex, height, salary);
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
